using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FreeImageAPI;
using iTextSharp.text.pdf;
using PDFPatcher.Model;

namespace PDFPatcher.Processor.Imaging
{
	[DebuggerDisplay("REF = {ReferenceCount}; Size = {Width} * {Height}; {PixelFormat}")]
	internal sealed class ImageInfo
	{
		public string FileName { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int BitsPerComponent { get; private set; }
		public PixelFormat PixelFormat { get; private set; }
		public int ReferenceCount { get; set; }
		public bool VerticalFlip { get; set; }
		public string ExtName { get; private set; }
		public string LastDecodeError { get; private set; }
		public PdfName ColorSpace { get; private set; }
		public PdfName PaletteColorSpace { get; private set; }
		public byte[] PaletteBytes { get; private set; }
		public byte[] ICCProfile { get; private set; }
		public FreeImageBitmap Mask { get; private set; }
		public int PaletteEntryCount { get; private set; }
		public PdfImageData InlineImage { get; }
		public bool IsPageImage { get; }

		internal ImageInfo() { }
		internal ImageInfo(PdfImageData image) {
			InlineImage = image;
		}
		internal ImageInfo(PdfIndirectReference pdfIndirect) {
			InlineImage = new PdfImageData(pdfIndirect);
		}
		internal ImageInfo(PdfIndirectReference pdfIndirect, bool isPageImage) {
			InlineImage = new PdfImageData(pdfIndirect);
			IsPageImage = IsPageImage;
		}
		internal ImageInfo(PRStream stream) {
			InlineImage = new PdfImageData(stream);
		}
		internal byte[] DecodeImage(ImageExtracterOptions options) {
			return DecodeImage(this, options);
		}

		private static byte[] DecodeImage(ImageInfo info, ImageExtracterOptions options) {
			byte[] decodedBytes;
			var data = info.InlineImage;
			info.ExtName = Constants.FileExtensions.Dat;

			info.Width = data.TryGetInt32(PdfName.WIDTH, 0);
			info.Height = data.TryGetInt32(PdfName.HEIGHT, 0);
			if (info.Width < options.MinWidth || info.Height < options.MinHeight) {
				if (info.InlineImage.PdfRef != null) {
					Tracker.TraceMessage($"忽略了一幅编号为 {info.InlineImage}，尺寸为 {info.Width}*{info.Height}的图像。");
				}
				else {
					Tracker.TraceMessage($"忽略了一幅尺寸为 {info.Width}*{info.Height}的内嵌图像。");
				}
				return null;
			}
			info.BitsPerComponent = data.TryGetInt32(PdfName.BITSPERCOMPONENT, 1);
			info.PixelFormat = PixelFormat.Format8bppIndexed;
			var decParams = data.GetObjectDirectOrFromContainerArray(PdfName.DECODEPARMS, PdfObject.DICTIONARY);
			var filters = data.GetObjectDirectOrFromContainerArray(PdfName.FILTER, PdfObject.NAME);
			decodedBytes = DecodeStreamContent(data, filters);
			var filter = filters.Count > 0 ? (filters[filters.Count - 1] as PdfName ?? PdfName.DEFAULT).ToString() : "BMP";
			var decParam = decParams.Count > 0 ? decParams[decParams.Count - 1] as PdfDictionary : null;
			ExportColorspace(data.GetDirectObject(PdfName.COLORSPACE), info);
			switch (filter) {
				case "/DCTDecode":
				case "/DCT":
					info.ExtName = Constants.FileExtensions.Jpg;
					goto case "JPG";
				case "/JPXDecode":
				case "/JPX":
					info.ExtName = Constants.FileExtensions.Jp2;
					goto EXIT;
				case "/CCITTFaxDecode":
				case "/CCF":
				case "/JBIG2Decode":
					info.ExtName = Constants.FileExtensions.Tif;
					var k = 0;
					var blackIs1 = false;
					var byteAlign = false;
					var endOfLine = false;
					var endOfBlock = true;
					if (decParam != null) {
						k = decParam.TryGetInt32(PdfName.K, 0);
						blackIs1 = decParam.TryGetBoolean(PdfName.BLACKIS1, false);
						byteAlign = decParam.TryGetBoolean(PdfName.ENCODEDBYTEALIGN, false);
						endOfBlock = decParam.TryGetBoolean(PdfName.ENDOFBLOCK, true);
						endOfLine = decParam.TryGetBoolean(PdfName.ENDOFLINE, false);
					}
					blackIs1 = IsDecodeParamInverted(data, blackIs1);
					if (options.InvertBlackAndWhiteImages) {
						blackIs1 = !blackIs1;
					}
					byte[] outBuf;
					if (filter == "/JBIG2Decode") {
						var globals = new byte[0];
						if (decParam != null) {
							var gRef = decParam.GetAsIndirectObject(PdfName.JBIG2GLOBALS);
							if (gRef != null && PdfReader.GetPdfObjectRelease(gRef) is PRStream gs) {
								globals = PdfReader.GetStreamBytes(gs);
							}
						}
						outBuf = JBig2Decoder.Decode(decodedBytes, globals);
						if (outBuf == null) {
							info.LastDecodeError = "导出 JBig2 编码图片失败。";
							return null;
						}
						if (blackIs1 == false) {
							InvertBits(outBuf);
						}
					}
					else {
						outBuf = TiffHelper.Decode(info, decodedBytes, k, endOfLine, byteAlign, endOfBlock, blackIs1);
					}
					info.PixelFormat = PixelFormat.Format1bppIndexed;
					info.BitsPerComponent = 1;
					decodedBytes = outBuf;
					break;
				case "/FlateDecode":
				case "/Fl":
				case "/LZWDecode":
					info.ExtName = Constants.FileExtensions.Png;
					info.PixelFormat = GetPixelFormat(decodedBytes.Length, info);
					if (info.PixelFormat == PixelFormat.Undefined) {
						info.LastDecodeError = "无法判定图像的颜色格式。";
						info.ExtName = Constants.FileExtensions.Dat;
						return null;
					}
					else if (info.PixelFormat == PixelFormat.Format1bppIndexed) {
						blackIs1 = IsDecodeParamInverted(data, false);
						if (options.InvertBlackAndWhiteImages) {
							blackIs1 = !blackIs1;
						}
						if (blackIs1) {
							InvertBits(decodedBytes);
						}
					}
					break;
				case "BMP":
					info.ExtName = Constants.FileExtensions.Png;
					break;
				case "JPG":
					if (options.MergeImages == false) {
						goto EXIT;
					}
					using (var ms = new MemoryStream(decodedBytes))
					using (var bm = PdfName.DEVICECMYK.Equals(info.ColorSpace)
						? new FreeImageBitmap(ms, FREE_IMAGE_LOAD_FLAGS.JPEG_CMYK)
						: new FreeImageBitmap(ms)) {
						info.PixelFormat = bm.PixelFormat;
						switch (bm.ColorType) {
							case FREE_IMAGE_COLOR_TYPE.FIC_CMYK:
								info.ColorSpace = PdfName.DEVICECMYK;
								break;
							case FREE_IMAGE_COLOR_TYPE.FIC_MINISBLACK:
							case FREE_IMAGE_COLOR_TYPE.FIC_MINISWHITE:
								info.ColorSpace = PdfName.DEVICEGRAY;
								break;
							case FREE_IMAGE_COLOR_TYPE.FIC_PALETTE:
								info.ColorSpace = PdfName.INDEXED;
								break;
							case FREE_IMAGE_COLOR_TYPE.FIC_RGB:
							case FREE_IMAGE_COLOR_TYPE.FIC_RGBALPHA:
								info.ColorSpace = PdfName.DEVICERGB;
								break;
						}
						info.BitsPerComponent =
							info.PixelFormat == PixelFormat.Format1bppIndexed ? 1
							: info.PixelFormat == PixelFormat.Format4bppIndexed ? 4
							: 8;
					}
					goto EXIT;
				default:
					info.PixelFormat = PixelFormat.Undefined;
					info.LastDecodeError = "未支持的图像数据格式：" + filter;
					return null;
			}
			if (PdfName.DEVICECMYK.Equals(info.ColorSpace)) {
				info.ExtName = Constants.FileExtensions.Tif;
			}
		EXIT:
			PRStream sm;
			if (options.ExtractSoftMask
				&& (sm = (data.GetAsStream(PdfName.SMASK) as PRStream)
					?? (data.GetAsStream(PdfName.MASK) as PRStream)) != null
				) {
				var mi = new ImageInfo(sm);
				var mask = DecodeImage(mi, new ImageExtracterOptions() { InvertBlackAndWhiteImages = !options.InvertSoftMask });
				if (mask != null) {
					info.Mask = ImageExtractor.CreateFreeImageBitmap(mi, ref mask, options.VerticalFlipImages, false);
				}
			}
			return decodedBytes;
		}

		static void InvertBits(byte[] outBuf) {
			int len = outBuf.Length;
			for (int t = 0; t < len; ++t) {
				outBuf[t] ^= 0xff;
			}
		}

		internal void CreatePaletteAndIccProfile(FreeImageBitmap bmp) {
			if (PixelFormat == PixelFormat.Format1bppIndexed) {
				ColorSpace = PdfName.DEVICEGRAY;
			}
			CreatePalette(bmp);
			if (PaletteEntryCount > 0) {
				if (PaletteEntryCount < 3) {
					PixelFormat = PixelFormat.Format1bppIndexed;
					bmp.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP);
				}
				else if (PaletteEntryCount < 17) {
					if (PixelFormat != PixelFormat.Format4bppIndexed) {
						PixelFormat = PixelFormat.Format4bppIndexed;
						bmp.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_04_BPP);
					}
				}
			}
			if (ICCProfile != null) {
				bmp.CreateICCProfile(ICCProfile);
			}
		}

		void CreatePalette(FreeImageBitmap bmp) {
			if (bmp.HasPalette == false) {
				Trace.WriteLine("Bitmap does not have palette.");
				return;
			}
			var pal = bmp.Palette;
			if (PdfName.DEVICEGRAY.Equals(PaletteColorSpace)) {
				if (PaletteBytes == null) {
					pal.CreateGrayscalePalette();
					PaletteEntryCount = pal.Count;
				}
				else {
					var pattern = PaletteBytes;
					var l = pattern.Length;
					var l2 = pal.Count;
					int i;
					byte p;
					for (i = 0; i < l && i < l2; i++) {
						p = pattern[i];
						pal.SetValue(new RGBQUAD(Color.FromArgb(p, p, p)), i);
					}
					PaletteEntryCount = i;
				}
			}
			else {
				var pattern = PaletteBytes;
				if (pattern == null) {
					bmp.Palette.CreateGrayscalePalette();
					PaletteEntryCount = bmp.Palette.Count;
				}
				else {
					var i = 0;
					var l = pattern.Length;
					var l2 = pal.Count;
					for (int pi = 0; pi < l && i < l2; pi++) {
						pal.SetValue(new RGBQUAD(Color.FromArgb(pattern[pi++], pi < l ? pattern[pi++] : 0, pi < l ? pattern[pi] : 0)), i);
						i++;
					}
					PaletteEntryCount = i;
				}
			}
		}

		static bool IsDecodeParamInverted(PdfDictionary data, bool blackIs1) {
			var a = data.GetAsArray(PdfName.DECODE);
			if (a?.Size == 2 && a[0].Type == PdfObject.NUMBER) {
				blackIs1 = ((PdfNumber)a[0]).IntValue == (blackIs1 ? 0 : 1);
			}
			return blackIs1;
		}

		static byte[] DecodeStreamContent(PdfImageData data, IList<PdfObject> filters) {
			var buffer = data.RawBytes;
			if (filters.Count == 0) {
				return buffer;
			}
			var dp = new List<PdfObject>();
			var dpo = PdfReader.GetPdfObjectRelease(data.Get(PdfName.DECODEPARMS));
			if (dpo == null || (!dpo.IsDictionary() && !dpo.IsArray()))
				dpo = PdfReader.GetPdfObjectRelease(data.Get(PdfName.DP));
			if (dpo != null) {
				if (dpo.IsDictionary())
					dp.Add(dpo);
				else if (dpo.IsArray())
					dp = ((PdfArray)dpo).ArrayList;
			}

			for (int i = 0; i < filters.Count; i++) {
				var name = (filters[i] as PdfName).ToString();
				switch (name) {
					case "/FlateDecode":
					case "/Fl":
						buffer = PdfReader.FlateDecode(buffer);
						goto case "DecodePredictor";
					case "/ASCIIHexDecode":
					case "/AHx":
						buffer = PdfReader.ASCIIHexDecode(buffer);
						break;
					case "/ASCII85Decode":
					case "/A85":
						buffer = PdfReader.ASCII85Decode(buffer);
						break;
					case "/LZWDecode":
						buffer = PdfReader.LZWDecode(buffer);
						goto case "DecodePredictor";
					case "/Crypt":
						break;
					case "/DCTDecode":
					case "/JPXDecode":
					case "/CCITTFaxDecode":
					case "/JBIG2Decode":
						if (i != filters.Count - 1) {
							Tracker.TraceMessage(Tracker.Category.Error, "文件格式错误：" + name + " 解码器不是最后一个解码器。");
						}
						break;
					case "DecodePredictor":
						if (i < dp.Count) {
							buffer = PdfReader.DecodePredictor(buffer, (PdfObject)dp[i]);
						}
						break;
					default:
						Trace.WriteLine(Tracker.Category.Error, "不支持的流编码格式：" + name);
						break;
				}
			}
			return buffer;
		}

		static PixelFormat GetPixelFormat(int byteLength, ImageInfo info) {
			var pf = PixelFormat.Undefined;
			var components = byteLength / info.Width / info.Height;
			switch (info.BitsPerComponent) {
				case 1: pf = PixelFormat.Format1bppIndexed; break;
				case 2:
					pf = PixelFormat.Format1bppIndexed;
					Trace.WriteLine("Warning: unsupported bpc = 2");
					break;
				case 4: pf = PixelFormat.Format4bppIndexed; break;
				case 8:
					switch (components) {
						case 0: // 兼容异常图片（github：#119）
							Trace.WriteLine("Warning: Not enough bytes.");
							goto case 1;
						case 1:
							pf = PixelFormat.Format8bppIndexed;
							break;
						case 2:
							pf = PixelFormat.Format16bppRgb555;
							break;
						case 3:
							pf = PixelFormat.Format24bppRgb;
							break;
						case 4:
							pf = PixelFormat.Format32bppRgb;
							break;
						default:
							Trace.WriteLine("Warning: Unknown colors.");
							break;
					}
					break;
				case 16:
					pf = PixelFormat.Format48bppRgb;
					break;
				default:
					Debug.WriteLine("Warning: bitsPerComponent missing or incorrect (" + info.BitsPerComponent + ").");
					if (components > 0) {
						goto case 8;
					}
					else {
						var areaPixels = (info.Width + 7) / 8 * info.Height;
						switch (areaPixels / byteLength) {
							case 1: pf = PixelFormat.Format1bppIndexed; info.BitsPerComponent = 1; break;
							case 2: pf = PixelFormat.Format1bppIndexed; info.BitsPerComponent = 2; break;
							case 4: pf = PixelFormat.Format4bppIndexed; info.BitsPerComponent = 4; break;
							default: pf = PixelFormat.Format8bppIndexed; info.BitsPerComponent = 8; break;
						}
					}
					break;
			}
			return pf;
		}

		static void ExportColorspace(PdfObject cs, ImageInfo info) {
			if (cs == null) {
				return;
			}
			info.ColorSpace = cs as PdfName;
			if (info.ColorSpace != null) {
				return;
			}

			if (cs.Type != PdfObject.ARRAY) {
				return;
			}

			var colorspace = (PdfArray)cs;
			// todo: 是否需要将所有 ColorSpace 换成 PaletteColorSpace
			if (PdfName.ICCBASED.Equals(colorspace.GetAsName(0))) {
				info.ColorSpace = ((PRStream)colorspace.GetDirectObject(1)).GetAsName(PdfName.ALTERNATE);
				return;
			}
			if (PdfName.INDEXED.Equals(colorspace.GetAsName(0))) {
				var o = colorspace.GetDirectObject(1);
				info.PaletteColorSpace = o as PdfName;
				if (info.PaletteColorSpace == null && o is PdfArray arr && arr.Size == 2) {
					if (PdfName.ICCBASED.Equals(arr.GetAsName(0)) && arr.Size == 2) {
						var iccs = arr.GetDirectObject(1) as PRStream;
						info.ColorSpace = iccs.GetAsName(PdfName.ALTERNATE) ?? PdfName.DEVICERGB;
						info.ICCProfile = PdfReader.GetStreamBytes(iccs);
					}
					else {
						info.ColorSpace = arr.GetAsName(0);
					}
				}
				var csp = colorspace.GetDirectObject(3);
				if (csp.IsString()) {
					info.PaletteBytes = ((PdfString)csp).GetOriginalBytes();
				}
				else if (csp is PRStream s) {
					info.PaletteBytes = PdfReader.GetStreamBytes(s);
				}
			}
		}

		internal void ConvertDecodedBytes(ref byte[] bytes) {
			if (PixelFormat == PixelFormat.Format24bppRgb) {
				// from RGB array to BGR GDI+ data
				byte b;
				for (int i = 0; i < bytes.Length; i += 3) {
					b = bytes[i];
					bytes[i] = bytes[i + 2];
					bytes[i + 2] = b;
				}
			}
			else if (PixelFormat == PixelFormat.Format1bppIndexed && BitsPerComponent == 2) {
				// 支持四级灰度的图像
				var l = bytes.Length;
				var newBytes = new byte[l << 1];
				var i = 0;
				foreach (var b in bytes) {
					newBytes[i++] = (byte)(((b & 0xC0) >> 0x02) + ((b & 0x30) >> 0x04));
					newBytes[i++] = (byte)(((b & 0x0C) << 0x02) + (b & 0x03));
				}
				if (PaletteBytes != null) {
					var pattern = PaletteBytes;
					Array.Resize(ref pattern, 16 * 3);
					PaletteBytes = pattern;
				}
				PixelFormat = PixelFormat.Format4bppIndexed;
				BitsPerComponent = 4;
				ColorSpace = PdfName.DEVICEGRAY;
				bytes = newBytes;
			}
		}


	}


}
