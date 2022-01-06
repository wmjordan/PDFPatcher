using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FreeImageAPI;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor.Imaging;

namespace PDFPatcher.Processor
{
	sealed class ImageExtractor
	{
		readonly PdfPageImageProcessor _parser;
		int _totalImageCount;
		int _imageCount;
		int _activePage; // 在导出文件图片时，使用此属性命名文件
		readonly string _fileMask;
		int _pageRotation;
		readonly ImageExtracterOptions _options;
		readonly List<ImageInfo> _imageInfoList = new List<ImageInfo>();
		readonly HashSet<PdfObject> _Refs = new HashSet<PdfObject>();

		internal List<ImageInfo> InfoList => _imageInfoList;

		readonly List<ImageDisposition> _imagePosList = new List<ImageDisposition>();
		internal List<ImageDisposition> PosList => _imagePosList;
		internal bool PrintImageLocation { get; set; }

		public ImageExtractor(ImageExtracterOptions options, PdfReader reader) {
			_fileMask = String.IsNullOrEmpty(options.FileMask) ? "0" : options.FileMask;
			_options = options;
			_parser = new PdfPageImageProcessor(_imagePosList, _imageInfoList);
		}

		internal void ExtractPageImages(PdfReader reader, int pageNum) {
			if (pageNum < 1 || pageNum > reader.NumberOfPages) {
				return;
			}
			_activePage = pageNum;
			_parser.Reset();
			_imageCount = 0;
			_imageInfoList.Clear();
			_imagePosList.Clear();
			var o = reader.GetPageNRelease(pageNum);
			if (o == null) {
				return;
			}

			// 收集页面上的图片
			var pp = o.Locate<PdfDictionary>(PdfName.RESOURCES, PdfName.XOBJECT);
			if (pp != null) {
				ExtractImageInstances(pp, false);
			}
			// 收集批注中的图片
			if (_options.ExtractAnnotationImages) {
				var an = o.Locate<PdfArray>(PdfName.ANNOTS);
				if (an != null) {
					foreach (var item in an.ArrayList) {
						ExtractImageInstances(PdfReader.GetPdfObjectRelease(item) as PdfDictionary, true);
					}
				}
			}
			_pageRotation = PdfHelper.GetPageRotation(o);
			if (_imageInfoList.Count == 0) {
				return;
			}
			_parser.ProcessContent(reader.GetPageContent(pageNum), o.Locate<PdfDictionary>(PdfName.RESOURCES));
			_imagePosList.Sort();
			_imageInfoList.Sort((x, y) => {
				var xi = _imagePosList.Find((info) => info.Image == x);
				var yi = _imagePosList.Find((info) => info.Image == y);
				if (xi == null) {
					return yi == null ? 0 : -1;
				}
				else if (yi == null) {
					return -1;
				}
				return xi.CompareTo(yi);
			});
			foreach (var item in _imageInfoList) {
				try {
					ExtractImage(item);
				}
				catch (FreeImageException ex) {
					if (item.ReferenceCount > 0) {
						Tracker.TraceMessage(Tracker.Category.Error, "在导出第 " + pageNum + " 页图片时遇到错误：" + ex.Message);
					}
				}
			}
			if (_options.MergeImages && _imagePosList.Count > 1) {
				// 合并相同宽度、相同类型的图片
				MergeImages();
			}
		}

		private void ExtractImageInstances(PdfDictionary source, bool includeDescendants) {
			if (source == null) {
				return;
			}
			foreach (var item in source) {
				if (_options.SkipRedundantImages && _Refs.Add(item.Value) == false
					|| PdfName.SMASK.Equals(item.Key)
					|| PdfName.MASK.Equals(item.Key)) {
					continue;
				}
				var obj = PdfReader.GetPdfObject(item.Value);
				var stream = obj as PRStream;
				if (stream == null) {
					goto NEXT;
				}
				PdfName subType = stream.GetAsName(PdfName.SUBTYPE);
				if (PdfName.IMAGE.Equals(subType)) {
					try {
						_imageInfoList.Add(new ImageInfo(item.Value as PRIndirectReference));
					}
					catch (NullReferenceException) {
						Debug.WriteLine(item.Value);
					}
				}
				else if (PdfName.FORM.Equals(subType)) {
					var fr = stream.Locate<PdfDictionary>(PdfName.RESOURCES, PdfName.XOBJECT);
					if (fr == null) {
						continue;
					}
					foreach (var fri in fr) {
						stream = PdfReader.GetPdfObject(fri.Value) as PRStream;
						if (stream != null) {
							subType = stream.GetAsName(PdfName.SUBTYPE);
							if (PdfName.IMAGE.Equals(subType)) {
								_imageInfoList.Add(new ImageInfo(fri.Value as PRIndirectReference));
							}
							else if (includeDescendants || PdfName.FORM.Equals(subType)) {
								ExtractImageInstances(stream, true);
							}
						}
						else if (includeDescendants) {
							ExtractImageInstances(stream, true);
						}
					}
				}
			NEXT:
				if ((obj.Type == PdfObject.DICTIONARY || obj.Type == PdfObject.STREAM) && includeDescendants) {
					ExtractImageInstances(obj as PdfDictionary, true);
				}
			}
		}

		internal void ExtractImage(ImageInfo info) {
			if (_totalImageCount == 0 && Directory.Exists(_options.OutputPath) == false) {
				Directory.CreateDirectory(_options.OutputPath);
			}
			var bytes = info.DecodeImage(_options);
			if (bytes == null) {
				return;
			}
			if (info.LastDecodeError != null) {
				Tracker.TraceMessage(Tracker.Category.Error, info.LastDecodeError);
				return;
			}
			var fileName = GetNewImageFileName();
			if (info.ExtName == Constants.FileExtensions.Png
				|| info.ExtName == Constants.FileExtensions.Tif
				//|| info.MaskBytes != null
				) {
				SaveBitmap(info, bytes, fileName);
			}
			else {
				SaveImageBytes(info, bytes, fileName);
			}
			if (info.MaskBytes != null) {
				using (var m = new FreeImageBitmap(
					info.MaskSize.Width, info.MaskSize.Height,
					(info.MaskSize.Width + 7) / 8, PixelFormat.Format1bppIndexed,
					info.MaskBytes)) {
					//var r = bmp.Composite (false, null, m);
					m.Palette.CreateGrayscalePalette();
					m.Save(fileName + "[mask]" + Constants.FileExtensions.Tif);
				}
			}
			_totalImageCount++;
		}

		private void SaveImageBytes(ImageInfo info, byte[] bytes, string fileName) {
			var vFlip = _options.VerticalFlipImages ^ info.VerticalFlip;
			var n = fileName + info.ExtName;
			if (PrintImageLocation) {
				Tracker.TraceMessage(Tracker.Category.OutputFile, n);
				Tracker.TraceMessage("导出图片：" + n);
			}
			if ((vFlip || _pageRotation != 0) && info.ExtName == Constants.FileExtensions.Jp2) {
				//var ctx = MuPdfSharp.ContextHandle.Create();
				//var pixmap = new MuPdfSharp.PixmapData(ctx, ctx.LoadJpeg2000(bytes));
				//var b = pixmap.ToBitmap(new MuPdfSharp.ImageRendererOptions());
				try {
					using (var ms = new MemoryStream(bytes))
					using (var bmp = new FreeImageBitmap(ms)) {
						RotateBitmap(bmp, _pageRotation, vFlip);
						info.CreatePaletteAndIccProfile(bmp);
						try {
							bmp.Save(n);
						}
						catch (FreeImageException) {
							File.Delete(n);
							bmp.Save(new FilePath(n).ChangeExtension(Constants.FileExtensions.Png));
						}
					}
				}
				catch (FreeImageException ex) {
					Tracker.TraceMessage(ex);
					bytes.DumpBytes(n);
				}
			}
			else if (PdfName.DEVICECMYK.Equals(info.ColorSpace)) {
				using (var ms = new MemoryStream(bytes))
				using (var bmp = new FreeImageBitmap(ms, FREE_IMAGE_LOAD_FLAGS.JPEG_CMYK | FREE_IMAGE_LOAD_FLAGS.TIFF_CMYK)) {
					RotateBitmap(bmp, _pageRotation, vFlip);
					if (bmp.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_24_BPP)) {
						SwapRedBlue(bmp);
						n = fileName + Constants.FileExtensions.Png;
						bmp.Save(n, FREE_IMAGE_FORMAT.FIF_PNG);
					}
					else {
						n = fileName + Constants.FileExtensions.Tif;
						bmp.Save(n, FREE_IMAGE_FORMAT.FIF_TIFF, FREE_IMAGE_SAVE_FLAGS.TIFF_CMYK | FREE_IMAGE_SAVE_FLAGS.TIFF_DEFLATE);
					}
					if (PrintImageLocation) {
						Tracker.TraceMessage("导出图片：" + n);
					}
				}
			}
			else {
				using (FileStream f = new FileStream(n, FileMode.Create)) {
					f.Write(bytes, 0, bytes.Length);
				}
				if (info.ExtName == Constants.FileExtensions.Jpg) {
					if (vFlip) {
						TransformJpeg(n, FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_FLIP_V);
					}
					if (_pageRotation != 0) {
						TransformJpeg(n,
							_pageRotation == 90 ? FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_ROTATE_90
							: _pageRotation == 180 ? FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_ROTATE_180
							: _pageRotation == 270 ? FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_ROTATE_270
							: FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_NONE);
					}
				}
			}
			info.FileName = n;
		}

		private static void SwapRedBlue(FreeImageBitmap bmp) {
			var r = bmp.GetChannel(FREE_IMAGE_COLOR_CHANNEL.FICC_RED);
			var b = bmp.GetChannel(FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE);
			bmp.SetChannel(b, FREE_IMAGE_COLOR_CHANNEL.FICC_RED);
			bmp.SetChannel(r, FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE);
			return;
			//var stride = bmp.Stride;
			//var Scan0 = bmp.Scan0;

			//unsafe {
			//	var p = (byte*)(void*)Scan0;
			//	var nOffset = stride - bmp.Width * 3;
			//	byte temp;
			//	var h = bmp.Height;
			//	var w = bmp.Width;
			//	for (int y = 0; y < h; ++y) {
			//		for (int x = 0; x < w; ++x) {
			//			temp = p[0];
			//			p[0] = p[2];
			//			p[2] = temp;
			//			p += 3;
			//		}
			//		p += nOffset;
			//	}
			//}
		}

		private static void TransformJpeg(string fileName, FREE_IMAGE_JPEG_OPERATION operation) {
			var tmpName = fileName + Constants.FileExtensions.Tmp;
			if (FreeImageBitmap.JPEGTransform(fileName, tmpName, operation, true)) {
				File.Delete(fileName);
				File.Move(tmpName, fileName);
				return;
			}
			File.Delete(tmpName);
			RotateFlipType type;
			switch (operation) {
				case FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_FLIP_H:
					type = RotateFlipType.RotateNoneFlipX;
					break;
				case FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_FLIP_V:
					type = RotateFlipType.RotateNoneFlipY;
					break;
				case FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_ROTATE_90:
					type = RotateFlipType.Rotate270FlipNone;
					break;
				case FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_ROTATE_180:
					type = RotateFlipType.Rotate180FlipNone;
					break;
				case FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_ROTATE_270:
					type = RotateFlipType.Rotate90FlipNone;
					break;
				default:
					Tracker.TraceMessage(Tracker.Category.Error, "无损翻转 JPG 图片失败：" + fileName);
					return;
			}
			using (var bmp = new FreeImageBitmap(fileName)) {
				bmp.RotateFlip(type);
				if (bmp.UniqueColors < 256) {
					bmp.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP);
				}
				bmp.Save(new FilePath(fileName).ChangeExtension(Constants.FileExtensions.Png));
			}
			File.Delete(fileName);
		}

		private void SaveBitmap(ImageInfo info, byte[] bytes, string fileName) {
			var vFlip = _options.VerticalFlipImages ^ info.VerticalFlip;
			var ext = info.ExtName;
			if (info.PixelFormat == PixelFormat.Format1bppIndexed) {
				ext = _options.MonoPng == false ? Constants.FileExtensions.Tif : Constants.FileExtensions.Png;
			}
			var n = fileName + ext;
			if (PrintImageLocation) {
				Tracker.TraceMessage(Tracker.Category.OutputFile, n);
				Tracker.TraceMessage("导出图片：" + n);
			}
			if (PdfName.DEVICECMYK.Equals(info.ColorSpace)) {
				// TODO: 转换字节数组的 CMYK 为 RGB 后加载到 FreeImageBitmap
				//info.PixelFormat = PixelFormat.Undefined;
				using (var bmp = new FreeImageBitmap(
					//info.Width,
					//info.Height,
					//GetStride (info, bytes, vFlip),
					//PixelFormat.Format32bppArgb, bytes
					new MemoryStream(bytes), FREE_IMAGE_LOAD_FLAGS.JPEG_CMYK
					)) {
					if (info.ICCProfile != null) {
						bmp.CreateICCProfile(info.ICCProfile);
					}
					RotateBitmap(bmp, _pageRotation, false);
					if (bmp.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_24_BPP)) {
						SwapRedBlue(bmp);
						n = fileName + Constants.FileExtensions.Png;
						bmp.Save(n, FREE_IMAGE_FORMAT.FIF_PNG);
					}
					else {
						bmp.Save(n,
							FREE_IMAGE_FORMAT.FIF_TIFF,
							FREE_IMAGE_SAVE_FLAGS.TIFF_CMYK | FREE_IMAGE_SAVE_FLAGS.TIFF_DEFLATE);
					}
				}
			}
			else {
				using (var bmp = CreateFreeImageBitmap(info, ref bytes, vFlip, true)) {
					if (ext == Constants.FileExtensions.Png
							&& _options.InvertBlackAndWhiteImages
							&& info.PixelFormat == PixelFormat.Format1bppIndexed
							&& bmp.Palette.Length == 2) {
						bmp.SwapPaletteIndices(0, 1);
					}
					RotateBitmap(bmp, _pageRotation, false);
					if (ext == Constants.FileExtensions.Tif) {
						TiffHelper.Save(bmp, n);
					}
					//else if (info.MaskBytes != null) {
					//    bmp.Save (fileName + Constants.FileExtensions.Png);
					//}
					else {
						try {
							bmp.Save(n, FREE_IMAGE_FORMAT.FIF_PNG);
						}
						catch (System.Runtime.InteropServices.SEHException ex) {
							Tracker.TraceMessage(Tracker.Category.Error, "保存图片时出现错误，请联系程序开发者：" + n);
						}
					}
				}
			}
			info.FileName = n;
		}

		static void RotateBitmap(FreeImageBitmap bitmap, int rotation, bool vflip) {
			if (rotation == 0 && vflip == false) {
				return;
			}
			RotateFlipType r;
			switch (rotation) {
				case 0: r = RotateFlipType.RotateNoneFlipY; break;
				case 90: r = vflip ? RotateFlipType.Rotate270FlipY : RotateFlipType.Rotate270FlipNone; break;
				case 180: r = vflip ? RotateFlipType.Rotate180FlipY : RotateFlipType.Rotate180FlipNone; break;
				case 270: r = vflip ? RotateFlipType.Rotate90FlipY : RotateFlipType.Rotate90FlipNone; break;
				default: return;
			}
			bitmap.RotateFlip(r);
		}

		internal static FreeImageBitmap CreateFreeImageBitmap(ImageInfo info, ref byte[] bytes, bool vFlip, bool loadPaletteAndIccp) {
			if (info.ExtName != Constants.FileExtensions.Jpg && info.ExtName != Constants.FileExtensions.Jp2) {
				info.ConvertDecodedBytes(ref bytes);
			}
			FreeImageBitmap bmp;
			if (PdfName.DEVICECMYK.Equals(info.ColorSpace)) {
				bmp = new FreeImageBitmap(new MemoryStream(bytes), FREE_IMAGE_LOAD_FLAGS.TIFF_CMYK);
			}
			else if (info.ExtName == Constants.FileExtensions.Jp2 || info.ExtName == Constants.FileExtensions.Jpg) {
				bmp = new FreeImageBitmap(new MemoryStream(bytes));
			}
			else {
				bmp = new FreeImageBitmap(info.Width, info.Height, GetStride(info, bytes, vFlip), info.PixelFormat, bytes);
			}
			if (loadPaletteAndIccp) {
				info.CreatePaletteAndIccProfile(bmp);
			}
			return bmp;
		}

		private static int GetStride(ImageInfo info, byte[] bytes, bool vFlip) {
			if (PdfName.COLORSPACE.Equals(info.ColorSpace)) {
				return vFlip ? -(info.Width << 2) : (info.Width << 2);
			}
			var components = bytes.Length / info.Width / info.Height;
			var stride = components > 0
				? info.Width * components
				: (info.Width + 8 / info.BitsPerComponent - 1) / (8 / info.BitsPerComponent);
			return vFlip ? -stride : stride;
		}

		private string GetNewImageFileName() {
			_imageCount++;
			return String.Concat(
				FileHelper.CombinePath(_options.OutputPath, _activePage.ToString(_fileMask)),
				_imageCount > 1 ? "[" + _imageCount + "]" : null);
		}

		private void MergeImages() {
			var l = _imagePosList.Count;
			for (int i = 0; i < l; i++) {
				var imageI = _imagePosList[i];
				// 由于在导出图像时仅为 PNG 和 TIF 指定 ImageInfo 的 PixelFormat，因此合并过程中仅处理这两类文件
				if (imageI.Image.ReferenceCount < 1 // 图像已处理
					|| imageI.Image.PixelFormat == PixelFormat.Undefined // 不属于可合并的类型
					|| l - i < 2 // 是最后一张图片
					) {
					continue;
				}
				var imageParts = new ImageInfo[l - i];
				var w = imageI.Image.Width;
				var h = 0;
				var i2 = 0;
				for (int j = i; j < l; j++) {
					var imageJ = _imagePosList[j];
					if (imageJ.Image.ReferenceCount < 1 // 图像已处理
						|| imageJ.Image.Width != w // 宽度不相符
						|| Math.Abs(Math.Round(imageJ.X - imageI.X)) > 1 // 位置相差超过 1 点
																		 //|| imageJ.Image.PixelFormat != imageI.Image.PixelFormat // 格式不匹配
																		 //|| imageJ.Image.ColorSpace == null
																		 //|| imageJ.Image.ColorSpace.Equals (imageI.Image.ColorSpace) == false // Colorspace 不匹配
						) {
						continue;
					}
					imageParts[i2] = imageJ.Image;
					h += imageJ.Image.Height;
					_imagePosList[j].Image.ReferenceCount--; // 避免重复处理
					i2++;
				}
				if (i2 == 0) { // 没有符合合并条件的图片
					continue;
				}
				if (i2 == 1) {
					_imagePosList[i].Image.ReferenceCount++;
					continue;
				}
				if (i2 < imageParts.Length) {
					Array.Resize(ref imageParts, i2);
				}
				if (PrintImageLocation) {
					Tracker.TraceMessage("合并图片：" + String.Join("、", Array.ConvertAll<ImageInfo, string>(imageParts, p => Path.GetFileName(p.FileName))));
				}
				var ext = Path.GetExtension(imageI.Image.FileName).ToLowerInvariant();
				if (imageI.Image.PixelFormat == PixelFormat.Format1bppIndexed) {
					ext = Constants.FileExtensions.Tif;
				}
				var f = GetNewImageFileName();
				using (FreeImageBitmap bmp = new FreeImageBitmap(w, h, imageI.Image.PixelFormat)) {
					h = 0;
					byte palEntryCount = 0;
					var bmpPal = bmp.HasPalette ? bmp.Palette.AsArray : null;
					foreach (var part in imageParts) {
						using (var bmp2 = FreeImageBitmap.FromFile(part.FileName)) {
							var pl = part.PaletteEntryCount;
							if (pl > 0 && bmp.HasPalette && bmp2.HasPalette) {
								//var palMapSrc = new byte[pl];
								//var palMapDest = new byte[pl];
								//uint mi = 0;
								for (int pi = 0; pi < pl; pi++) {
									var p = Array.IndexOf(bmpPal, part.PaletteArray[pi], 0, palEntryCount);
									if (p == -1) {
										if (palEntryCount == 255) {
											if (bmpPal != null) {
												bmp.Palette.AsArray = bmpPal;
											}
											// 调色板不足以存放合并后的图片颜色
											if (bmp.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_24_BPP)
												&& bmp2.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_24_BPP)) {
												ext = Constants.FileExtensions.Png;
												bmpPal = null;
												goto Paste;
											}
											else {
												throw new OverflowException("调色板溢出，无法合并图片。");
											}
										}
										if (palEntryCount >= bmpPal.Length && palEntryCount < 129) {
											bmp.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP);
											Array.Resize(ref bmpPal, 256);
										}
										bmpPal[palEntryCount] = part.PaletteArray[pi];
										p = palEntryCount;
										++palEntryCount;
									}
									//if (p != pi) {
									//	palMapSrc[mi] = (byte)pi;
									//	palMapDest[mi] = (byte)(p);
									//	mi++;
									//}
								}
								//bmp2.ApplyPaletteIndexMapping (palMapSrc, palMapDest, mi, false);
							}
							else if (pl > 0 && bmp2.HasPalette) {
								bmp2.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_24_BPP);
							}
						Paste:
							if (bmpPal != null) {
								bmp.Palette.AsArray = bmpPal;
							}
							if (bmp.HasPalette && bmp2.HasPalette) {
								var a1 = bmp.Palette.AsArray;
								var a2 = bmp2.Palette.AsArray;
								var sp = new byte[palEntryCount];
								var dp = new byte[palEntryCount];
								var di = 0;
								for (int ai = 0; ai < a2.Length; ai++) {
									var p = Array.IndexOf(a1, a2[ai], 0, palEntryCount);
									if (p != ai && p > -1) {
										sp[di] = (byte)ai;
										dp[di] = (byte)p;
										++di;
									}
								}
								//todo: 两幅图像调色板不一致时需调换颜色再复制数据
								//if (di > 0) {
								//	bmp2.ApplyPaletteIndexMapping(sp, dp, (uint)di, true);
								//}
							}
							if (bmp.Paste(bmp2, 0, h, Int32.MaxValue) == false) {
								if (bmp.HasPalette && bmp2.HasPalette == false) {
									bmp.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_24_BPP);
									if (bmp.Paste(bmp2, 0, h, Int32.MaxValue) == false) {
										Tracker.TraceMessage("合并图片失败");
									}
									bmpPal = null;
								}
							}
							h += bmp2.Height;
						}
					}
					if (bmpPal != null) {
						bmp.Palette.AsArray = bmpPal;
					}
					if (ext == Constants.FileExtensions.Jpg && _options.MergeJpgToPng) {
						ext = Constants.FileExtensions.Png;
					}
					else if (bmp.PixelFormat == PixelFormat.Format1bppIndexed) {
						if (_options.MonoPng == false) {
							ext = Constants.FileExtensions.Tif;
						}
						else {
							ext = Constants.FileExtensions.Png;
						}
					}
					f += ext;
					if (PrintImageLocation) {
						Tracker.TraceMessage(Tracker.Category.OutputFile, f);
						Tracker.TraceMessage("保存合并后的图片：" + f);
					}
					if (ext == Constants.FileExtensions.Tif) {
						TiffHelper.Save(bmp, f);
					}
					else {
						bmp.Save(f);
					}
					var mii = new ImageInfo { FileName = f, ReferenceCount = 1, Height = h, Width = w };
					_imageInfoList.Add(mii);
					_imagePosList.Add(new ImageDisposition(_imagePosList[i].Ctm, mii));
				}
			}
			foreach (var item in _imageInfoList) {
				if (item.ReferenceCount < 1) {
					File.Delete(item.FileName);
					item.FileName = null;
				}
			}
			_imageInfoList.Sort((ImageInfo x, ImageInfo y) => string.Compare(x.FileName, y.FileName, StringComparison.OrdinalIgnoreCase));
			_totalImageCount -= _imageCount;
			_imageCount = 0;
			var newFileNames = new List<string>();
			foreach (var item in _imageInfoList) {
				if (item.FileName != null && item.InlineImage == null) {
					string n;
					do {
						n = GetNewImageFileName() + Path.GetExtension(item.FileName);
					} while (_imagePosList.Exists((i) => i.Image.FileName == n) || newFileNames.Contains(n));
					if (PrintImageLocation) {
						Tracker.TraceMessage(String.Concat("重命名合并后的文件 ", item.FileName, " 为 ", n));
						Tracker.TraceMessage(Tracker.Category.OutputFile, n);
					}
					newFileNames.Add(n);
					File.Delete(n);
					File.Move(item.FileName, n);
					item.FileName = n;
				}
			}
			_totalImageCount += _imageCount;
		}

		sealed class PdfPageImageProcessor : PdfContentStreamProcessor
		{
			readonly List<ImageDisposition> _posList;
			readonly List<ImageInfo> _infoList;

			public PdfPageImageProcessor(List<ImageDisposition> posList, List<ImageInfo> infoList) {
				PopulateOperators();
				_posList = posList;
				_infoList = infoList;
			}
			protected override void InvokeOperator(PdfLiteral oper, List<PdfObject> operands) {
				base.InvokeOperator(oper, operands);
				switch (oper.ToString()) {
					case "Do":
						var xobjects = Resource.GetAsDict(PdfName.XOBJECT);
						var r = xobjects.GetAsIndirectObject(operands[0] as PdfName);
						var info = _infoList.Find(
							i => i.InlineImage.PdfRef != null
								&& i.InlineImage.PdfRef.Number == r.Number
								&& i.InlineImage.PdfRef.Generation == r.Generation);
						if (info != null) {
							info.ReferenceCount++;
							_posList.Add(new ImageDisposition(CurrentGraphicState.TransMatrix, info));
						}
						else {
							Trace.WriteLine(String.Concat("Image ", r, " not found."));
						}
						break;
					case "BI":
						info = new ImageInfo(new PdfImageData(operands[0] as PdfDictionary, ((PdfImageData)operands[0]).RawBytes));
						info.ReferenceCount++;
						_infoList.Add(info);
						_posList.Add(new ImageDisposition(CurrentGraphicState.TransMatrix, info));
						break;
				}
			}
		}

	}
}
