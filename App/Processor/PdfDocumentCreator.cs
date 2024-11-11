using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml;
using FreeImageAPI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;
using iTextImage = iTextSharp.text.Image;

namespace PDFPatcher.Processor
{
	sealed class PdfDocumentCreator
	{
		internal static readonly string[] SupportedFileTypes = new string[] { ".pdf", ".tif", ".jpg", ".gif", ".png", ".tiff", ".bmp", ".jpeg", ".jp2", ".j2k" };
		static readonly string[] __BuiltInImageTypes = { ".png", ".jpg", ".jpeg", ".bmp", ".jp2", ".j2k" };
		static readonly string[] __MultiFrameImageTypes = { ".tif", ".tiff", ".gif" };
		static readonly PixelFormat[] __JpgFormats = new PixelFormat[] {
									PixelFormat.Format16bppGrayScale,
									PixelFormat.Format16bppRgb555,
									PixelFormat.Format16bppRgb565,
									PixelFormat.Format24bppRgb,
									PixelFormat.Format32bppRgb,
									PixelFormat.Format48bppRgb};

		internal static readonly PaperSize[] PaperSizes = new PaperSize[] {
				new PaperSize (PaperSize.AsPageSize, 0, 0),
				new PaperSize (PaperSize.FixedWidthAutoHeight, 0, 0),
				new PaperSize (PaperSize.AsWidestPage, 0, 0),
				new PaperSize (PaperSize.AsNarrowestPage, 0, 0),
				new PaperSize (PaperSize.AsLargestPage, 0, 0),
				new PaperSize (PaperSize.AsSmallestPage, 0, 0),
				new PaperSize (PaperSize.AsFirstPage, 0, 0),
				new PaperSize ("16 开 (18.4*26.0)", 1840, 2600),
				new PaperSize ("32 开 (13.0*18.4)", 1300, 1840),
				new PaperSize ("大 32 开 (14.0*20.3)", 1400, 2030),
				new PaperSize ("A4 (21.0*29.7)", 2100, 2970),
				new PaperSize ("A3 (29.7*42.0)", 2970, 4200),
				new PaperSize ("自定义", 0, 0),
				new PaperSize ("————————————", 0, 0),
				new PaperSize ("8 开 (26.0*36.8)", 2600, 3680),
				new PaperSize ("16 开 (18.4*26.0)", 1840, 2600),
				new PaperSize ("大 16 开 (21.0*28.5)", 2100, 2850),
				new PaperSize ("32 开 (13.0*18.4)", 1300, 1840),
				new PaperSize ("大 32 开 (14.0*20.3)", 1400, 2030),
				new PaperSize ("8 K (27.3*39.3)", 2730, 3930),
				new PaperSize ("16 K (19.6*27.3)", 1960, 2730),
				new PaperSize ("A0 (84.1*118.9)", 8410, 11890),
				new PaperSize ("A1 (59.4*84.1)", 5940, 8410),
				new PaperSize ("A2 (42.0*59.4)", 4200, 5940),
				new PaperSize ("A3 (29.7*42.0)", 2970, 4200),
				new PaperSize ("A4 (21.0*29.7)", 2100, 2970),
				new PaperSize ("A5 (14.8*21.0)", 1480, 2100),
				new PaperSize ("A6 (10.5*14.8)", 1050, 1480),
				new PaperSize ("B0 (100.0*141.3)", 10000, 14130),
				new PaperSize ("B1 (70.7*100.0)", 7070, 10000),
				new PaperSize ("B2 (50.0*70.7)", 5000, 7070),
				new PaperSize ("B3 (35.3*50.0)", 3530, 5000),
				new PaperSize ("B4 (25.0*35.3)", 2500, 3530),
				new PaperSize ("B5 (17.6*25.0)", 1760, 2500),
				new PaperSize ("B6 (12.5*17.6)", 1250, 1760)
			};
		readonly MergerOptions _option;
		readonly Document _doc;
		readonly PdfCopy _writer;
		readonly PaperSize _content;
		readonly DocumentSink _sink;
		readonly PageBoxSettings _pageSettings;
		readonly bool _autoRotate;
		readonly HorizontalAlignment hAlign;
		readonly VerticalAlignment vAlign;
		readonly bool scaleUp, scaleDown;
		readonly bool areMarginsEqual;
		bool _portrait;
		int _inputDocumentCount;

		/// <summary>
		/// 在传入构造函数选项中保留链接时，获取最近处理的 PDF 文档的书签。
		/// </summary>
		public PdfInfoXmlDocument PdfBookmarks { get; }
		/// <summary>获取输入的文档数量。</summary>
		public int InputDocumentCount => _inputDocumentCount;

		public PdfDocumentCreator(DocumentSink sink, MergerOptions option, ImporterOptions impOptions, Document document, PdfCopy writer) {
			_sink = sink;
			_option = option;
			_doc = document;
			_writer = writer;
			var ps = _pageSettings = option.PageSettings;
			_content = new PaperSize(ps.PaperSize.PaperName, option.ContentWidth, option.ContentHeight);
			_portrait = _content.Height > _content.Width;
			_autoRotate = ps.AutoRotation && (_content.Height != _content.Width);
			hAlign = ps.HorizontalAlign;
			vAlign = ps.VerticalAlign;
			scaleUp = option.AutoScaleUp;
			scaleDown = option.AutoScaleDown;
			areMarginsEqual = (ps.Margins.Top == ps.Margins.Left
				&& ps.Margins.Top == ps.Margins.Right
				&& ps.Margins.Top == ps.Margins.Bottom);
			if (impOptions.ImportBookmarks) {
				PdfBookmarks = new PdfInfoXmlDocument();
				var root = PdfBookmarks.BookmarkRoot;
			}
			if (_content.SpecialSize == SpecialPaperSize.None) {
				_doc.SetPageSize(new Rectangle(ps.PaperSize.Width, ps.PaperSize.Height));
			}
		}

		internal void ProcessFile(SourceItem sourceFile, BookmarkContainer bookmarkContainer) {
			if (sourceFile.Type != SourceItem.ItemType.Empty) {
				Tracker.TraceMessage(Tracker.Category.InputFile, sourceFile.FilePath.ToString());
			}
			var b = CreateAutoBookmark(sourceFile, bookmarkContainer);
			switch (sourceFile.Type) {
				case SourceItem.ItemType.Empty:
					Tracker.TraceMessage("添加空白页。");
					AddEmptyPage();
					SetBookmarkAction(b);
					++_inputDocumentCount;
					break;
				case SourceItem.ItemType.Pdf:
					Tracker.TraceMessage("添加文档：" + sourceFile);
					AddPdfPages(sourceFile as SourceItem.Pdf, b);
					Tracker.IncrementProgress(sourceFile.FileSize);
					++_inputDocumentCount;
					break;
				case SourceItem.ItemType.Image:
					Tracker.TraceMessage("添加图片：" + sourceFile);
					AddImagePage(sourceFile, b);
					Tracker.IncrementProgress(sourceFile.FileSize);
					++_inputDocumentCount;
					break;
				case SourceItem.ItemType.Folder:
					if (sourceFile.HasContent == false) {
						Tracker.TraceMessage("已忽略不包含内容的文件夹：" + sourceFile);
						return;
					}
					Tracker.TraceMessage("添加文件夹：" + sourceFile);
					break;
			}

			if (!sourceFile.HasSubItems) {
				return;
			}

			var p = false;
			var pn = _writer.CurrentPageNumber;
			foreach (var item in sourceFile.Items) {
				ProcessFile(item, b ?? bookmarkContainer);
				if (p) {
					continue;
				}
				p = true;
				var t = b;
				while (b?.Page == 0) {
					b.Page = pn;
					b.DestinationView = Constants.DestinationAttributes.ViewType.XYZ;
					b.Top = _doc.PageSize.Height;
					b = b.ParentBookmark;
				}
				b = t;
			}
		}

		void AddImagePage(SourceItem source, BookmarkElement bookmark) {
			var ext = source.FilePath.FileExtension.ToLowerInvariant();
			var isIndexed = false;
			if (__BuiltInImageTypes.Contains(ext)) {
				try {
					using (var fi = new FreeImageBitmap(source.FilePath, (FREE_IMAGE_LOAD_FLAGS)0x0800/*仅加载图像尺寸信息*/)) {
						if (fi.HasPalette && fi.ImageFormat != FREE_IMAGE_FORMAT.FIF_JPEG && fi.ImageFormat != FREE_IMAGE_FORMAT.FIF_JP2) {
							isIndexed = true;
							goto ADVANCED_LOAD;
						}
					}
				}
				catch (FreeImageException) {
					Tracker.TraceMessage("无法添加文件：" + source.FilePath);
					return;
				}
				var image = LoadImage(source, ext);
				if (image == null) {
					Tracker.TraceMessage("无法添加文件：" + source.FilePath);
					return;
				}
				if (_option.DpiX > 0 || _option.DpiY > 0) {
					image.SetDpi(_option.DpiX.SubstituteDefault(image.DpiX), _option.DpiY.SubstituteDefault(image.DpiY));
				}
				var cs = image.Additional?.GetAsArray(PdfName.COLORSPACE);
				if (cs?.Size == 4 && PdfName.INDEXED.Equals(cs[0])) {
					isIndexed = true;
				}
				else {
					if ((ext == Constants.FileExtensions.Jpg || ext == Constants.FileExtensions.Jpeg)
						&& Imaging.JpgHelper.TryGetExifOrientation(source.FilePath, out var o)
						&& o != 0) {
						switch (o) {
							case 6: image.RotationDegrees = -90; break;
							case 3: image.RotationDegrees = 180; break;
							case 8: image.RotationDegrees = 90; break;
						}
					}
					AddImage(image);
					SetBookmarkAction(bookmark);
					return;
				}
			}
			ADVANCED_LOAD:
			if (isIndexed || __MultiFrameImageTypes.Contains(ext)) {
				FreeImageBitmap fi = null;
				try {
					fi = FreeImageBitmap.FromFile(source.FilePath);
					var c = fi.FrameCount;
					for (int i = 0; i < c; i++) {
						fi.SelectActiveFrame(i);
						var img = LoadImageFrame(source as SourceItem.Image, _option.RecompressWithJbig2, ref fi);
						if (_option.DpiX > 0 || _option.DpiY > 0) {
							img.SetDpi(_option.DpiX.SubstituteDefault(img.DpiX), _option.DpiY.SubstituteDefault(img.DpiY));
						}
						AddImage(img);
						if (i == 0) {
							SetBookmarkAction(bookmark);
						}
					}
				}
				finally {
					fi?.Dispose();
				}
			}
		}

		void SetBookmarkAction(BookmarkElement bookmark) {
			if (bookmark == null) {
				return;
			}
			bookmark.Page = _writer.PageEmpty ? _writer.CurrentPageNumber - 1 : _writer.CurrentPageNumber;
			bookmark.DestinationView = Constants.DestinationAttributes.ViewType.XYZ;
			bookmark.Top = _doc.PageSize.Height;
		}

		void AddEmptyPage() {
			switch (_content.SpecialSize) {
				case SpecialPaperSize.None:
				case SpecialPaperSize.AsSpecificPage:
					// 插入空白页
					_doc.NewPage();
					_writer.PageEmpty = false;
					break;
				case SpecialPaperSize.AsPageSize:
					if (_doc.PageSize.Width > 0) {
						goto case SpecialPaperSize.AsSpecificPage;
					}
					break;
				default:
					Tracker.TraceMessage("没有指定页面尺寸，无法插入空白页。");
					break;
			}
		}

		void AddPdfPages(SourceItem.Pdf sourceFile, BookmarkContainer bookmark) {
			var pdf = _sink.GetPdfReader(sourceFile.FilePath);
			if (pdf.ConfirmUnethicalMode() == false) {
				Tracker.TraceMessage("忽略了没有权限处理的文件：" + sourceFile.FilePath);
				if (_sink.DecrementReference(sourceFile.FilePath) < 1) {
					pdf.Close();
				}
				return;
			}
			var ranges = PageRangeCollection.Parse(sourceFile.PageRanges, 1, pdf.NumberOfPages, true);
			var pageRemapper = new int[pdf.NumberOfPages + 1];
			Rectangle r;
			// 统一页面旋转角度
			if (_option.UnifyPageOrientation) {
				var rv = _option.RotateVerticalPages;
				var a = _option.RotateAntiClockwise ? -90 : 90;
				for (int i = pdf.NumberOfPages; i > 0; i--) {
					var p = pdf.GetPageN(i);
					r = p.GetPageVisibleRectangle();
					if (rv && r.Width < r.Height
						|| rv == false && r.Width > r.Height) {
						p.Put(PdfName.ROTATE, (r.Rotation + a) % 360);
					}
				}
			}
			if (bookmark != null) {
				var n = _writer.CurrentPageNumber + 1;
				if (_writer.PageEmpty) {
					n--;
				}
				bookmark.SetAttribute(Constants.DestinationAttributes.Page, n.ToText());
				bookmark.SetAttribute(Constants.DestinationAttributes.View, Constants.DestinationAttributes.ViewType.XYZ);
				r = pdf.GetPageN(ranges[0].StartValue).GetPageVisibleRectangle();
				float t = 0;
				switch (r.Rotation % 360 / 90) {
					case 0: t = r.Top; break;
					case 1: t = r.Right; break;
					case 2: t = r.Bottom; break;
					case 3: t = r.Left; break;
				}
				bookmark.SetAttribute(Constants.Coordinates.Top, t.ToText());
			}
			bool importImagesOnly = sourceFile.ImportImagesOnly;
			int pn = pdf.NumberOfPages;
			ImageExtractor imgExp = null;
			if (importImagesOnly) {
				imgExp = new ImageExtractor(sourceFile.ExtractImageOptions);
			}
			if (_option.KeepBookmarks) {
				pdf.ConsolidateNamedDestinations();
			}
			var pp = new byte[pdf.NumberOfPages + 1]; // 已处理过的页面
			var cts = _pageSettings.PaperSize.SpecialSize != SpecialPaperSize.AsPageSize ? new CoordinateTranslationSettings[pdf.NumberOfPages + 1] : null; // 页面的位置偏移量
			foreach (var range in ranges) {
				foreach (var i in range) {
					if (i < 1 || i > pn) {
						goto Exit;
					}
					if (pageRemapper != null) {
						pageRemapper[i] = _writer.CurrentPageNumber;
					}
					_doc.NewPage();
					if (imgExp != null) {
						imgExp.ExtractPageImages(pdf, i);
						foreach (var item in imgExp.InfoList) {
							if (item.FileName != null) {
								ProcessFile(new SourceItem.Image(item.FileName), bookmark);
								File.Delete(item.FileName);
							}
						}
					}
					else {
						if (pp[i] == 0) {
							var page = pdf.GetPageN(i);
							//if (DocInfoImporter.RemovePageAdditionalInfo (_docSettings, page)) {
							//    pdf.ResetReleasePage ();
							//}
							//if (_docSettings.AutoMaskBWImages) {
							//    SetBWImageMask (page);
							//    pdf.ResetReleasePage ();
							//}
							//PdfHelper.ClearPageLinks (pdf, i);
							if (_pageSettings.PaperSize.SpecialSize != SpecialPaperSize.AsPageSize) {
								pdf.ResetReleasePage();
								var ct = PageDimensionProcessor.ResizePage(page, _pageSettings, null);
								if (_pageSettings.ScaleContent) {
									PageDimensionProcessor.ScaleContent(pdf, i, ct);
								}
								if (cts != null) {
									cts[i] = ct;
								}
								pdf.ResetReleasePage();
							}
							//var og = new OperatorGroup (null);
							//if (_docSettings.FixContents) {
							//    og.Operators.Add (PdfContentStreamProcessor.NopOperator);
							//}
							//if (og.Operators.Count > 0) {
							//    var cp = new PdfPageCommandProcessor ();
							//    cp.ProcessContent (cb ?? pdf.GetPageContent (i), pdf.GetPageN (i).GetAsDict (PdfName.RESOURCES));
							//    cp.WritePdfCommands (pdf, i);
							//}

							pp[i] = 1;
						}
						_writer.AddPage(_writer.GetImportedPage(pdf, i));
					}
				Exit:
					Tracker.IncrementProgress(1);
				}
			}
			r = pdf.GetPageNRelease(ranges[ranges.Count - 1].EndValue).GetPageVisibleRectangle();
			r.Normalize();
			_doc.SetPageSize(r);
			_portrait = r.Height > r.Width;
			_content.Width = r.Width;
			_content.Height = r.Height;
			if (_option.ExtraEmptyPageForOddPdf && imgExp == null && ranges.TotalPages % 2 == 1) {
				_writer.AddPage(new Rectangle(r.Width, r.Height, 0), 0);
			}
			if (_option.KeepBookmarks) {
				bookmark = KeepBookmarks(bookmark, pdf, pageRemapper, cts);
			}
			if (_sink.DecrementReference(sourceFile.FilePath) < 1) {
				_writer.FreeReader(pdf);
				pdf.Close();
			}
		}

		BookmarkContainer KeepBookmarks(BookmarkContainer bookmark, PdfReader pdf, int[] pageRemapper, CoordinateTranslationSettings[] cts) {
			var bm = OutlineManager.GetBookmark(pdf, new UnitConverter() { Unit = Constants.Units.Point });
			var processors = new List<IInfoDocProcessor>();
			if (_option.ViewerPreferences.CollapseBookmark != BookmarkStatus.AsIs) {
				processors.Add(new CollapseBookmarkProcessor() { BookmarkStatus = _option.ViewerPreferences.CollapseBookmark });
			}
			if (_option.ViewerPreferences.RemoveZoomRate) {
				processors.Add(new RemoveZoomRateProcessor());
			}
			if (_option.ViewerPreferences.ForceInternalLink) {
				processors.Add(new ForceInternalDestinationProcessor());
			}
			processors.Add(new GotoDestinationProcessor() {
				RemoveOrphanDestination = _option.RemoveOrphanBookmarks,
				PageRemapper = pageRemapper,
				TransitionMapper = cts
			});
			ProcessInfoItem(bm, processors);
			if (bookmark != null) {
				bookmark.SetAttribute(Constants.BookmarkAttributes.Open,
					_option.ViewerPreferences.CollapseBookmark == BookmarkStatus.CollapseAll
						? Constants.Boolean.False : Constants.Boolean.True);
			}
			else if (PdfBookmarks != null) {
				bookmark = PdfBookmarks.BookmarkRoot;
			}
			else {
				return bookmark;
			}
			if (bm != null) {
				XmlNode c;
				while ((c = bm.FirstChild) != null) {
					if (c.NodeType == XmlNodeType.Element) {
						bookmark.AppendChild(bookmark.OwnerDocument.ImportNode(c, true));
					}
					bm.RemoveChild(c);
				}
			}
			return bookmark;
		}

		internal static void ProcessInfoItem(XmlElement item, ICollection<IInfoDocProcessor> processors) {
			if (item == null) {
				return;
			}
			foreach (var p in processors) {
				p.Process(item);
			}

			var c = item.FirstChild;
			XmlNode r;
			XmlElement ce;
			while (c != null) {
				ce = c as XmlElement;
				r = c.PreviousSibling;
				if (ce != null) {
					ProcessInfoItem(ce, processors);
				}
				if (c.ParentNode == null) {
					// 节点在处理过程中被删除
					c = r != null ? r.NextSibling : item.FirstChild;
				}
				else {
					c = c.NextSibling;
				}
			}
		}

		//internal static void SetBWImageMask (PdfDictionary page) {
		//    var xo = PdfHelper.Locate<PdfDictionary> (page, true, PdfName.RESOURCES, PdfName.XOBJECT);
		//    foreach (var item in xo) {
		//        var o = PdfReader.GetPdfObject (item.Value) as PdfDictionary;
		//        if (o == null
		//            || PdfName.IMAGE.Equals(o.Get(PdfName.SUBTYPE)) == false
		//            || PdfHelper.ValueIs (o.GetAsBoolean (PdfName.IMAGEMASK), true)
		//            || PdfHelper.ValueIs (o.GetAsNumber (PdfName.BITSPERCOMPONENT), 1) == false) {
		//            continue;
		//        }
		//        o.Put (PdfName.IMAGEMASK, PdfBoolean.PDFTRUE);
		//        o.Remove (PdfName.MASK);
		//        var cs = o.GetAsArray (PdfName.COLORSPACE);
		//        if (cs != null && cs.Size == 4
		//            && PdfHelper.ValueIs (cs.GetAsName (0), PdfName.INDEXED)
		//            && PdfHelper.ValueIs (cs.GetAsName (1), PdfName.DEVICERGB)
		//            && PdfHelper.ValueIs (cs.GetAsNumber (2), 1)) {
		//            PdfObject cl = cs.GetDirectObject (3);
		//            byte[] l = null;
		//            if (cs.IsString ()) {
		//                l = (cl as PdfString).GetOriginalBytes ();
		//            }
		//            else if (cs.IsStream ()) {
		//                l = PdfReader.GetStreamBytes (cl as PRStream);
		//            }
		//            Array.Resize (ref l, 6);
		//            if (l[0] == 0xFF) {

		//            }
		//        }
		//        o.Remove (PdfName.COLORSPACE);
		//    }
		//}

		static iTextImage LoadImage(SourceItem sourceFile, string ext) {
			var imageItem = sourceFile as SourceItem.Image;
			var cropOptions = imageItem.Cropping;
			if (imageItem == null || cropOptions.NeedCropping == false) {
				return Image.GetInstance(sourceFile.FilePath.ToString());
			}
			ext = ext.ToLowerInvariant();
			using (var fi = new FreeImageBitmap(sourceFile.FilePath)) {
				if (fi.Height < cropOptions.MinHeight // 不满足尺寸限制
					|| fi.Width < cropOptions.MinWidth
					|| fi.Height <= cropOptions.Top + cropOptions.Bottom // 裁剪后尺寸小于 0
					|| fi.Width <= cropOptions.Left + cropOptions.Right
					) {
					return Image.GetInstance(sourceFile.FilePath.ToString());
				}
				if (ext == Constants.FileExtensions.Jpg || ext == Constants.FileExtensions.Jpeg) {
					// is JPEG file
					var t = sourceFile.FilePath.EnsureExtension(Constants.FileExtensions.Jpg);
					if (FreeImageBitmap.JPEGCrop(sourceFile.FilePath, t, cropOptions.Left, cropOptions.Top, fi.Width - cropOptions.Right, fi.Height - cropOptions.Bottom)) {
						iTextImage image;
						using (var fs = new FileStream(t, FileMode.Open)) {
							image = Image.GetInstance(fs);
						}
						File.Delete(t);
						return image;
					}
				}

				using (var tmp = fi.Copy(cropOptions.Left, cropOptions.Top, fi.Width - cropOptions.Right, fi.Height - cropOptions.Bottom))
				using (MemoryStream ms = new MemoryStream()) {
					tmp.Save(ms, fi.ImageFormat);
					ms.Flush();
					ms.Position = 0;
					return Image.GetInstance(ms);
				}
			}
		}

		BookmarkElement CreateAutoBookmark(SourceItem sourceFile, XmlElement bookmarkContainer) {
			if (PdfBookmarks == null
				|| sourceFile.Bookmark == null
				|| String.IsNullOrEmpty(sourceFile.Bookmark.Title)) {
				return null;
			}
			var b = PdfBookmarks.CreateBookmark(sourceFile.Bookmark);
			bookmarkContainer.AppendChild(b);
			return b;
		}

		void AddImage(iTextImage image) {
			if (_option.AutoMaskBWImages && image.Bpc == 1 && image.IsMaskCandidate()) {
				image.MakeMask();
			}
			image.ScalePercent(72f / image.DpiX.SubstituteDefault(72) * 100f, 72f / image.DpiY.SubstituteDefault(72) * 100f);
			if (_content.SpecialSize == SpecialPaperSize.AsPageSize) {
				_doc.SetPageSize(new Rectangle(image.ScaledWidth + _doc.LeftMargin + _doc.RightMargin, image.ScaledHeight + _doc.TopMargin + _doc.BottomMargin));
			}
			else if (_content.SpecialSize == SpecialPaperSize.FixedWidthAutoHeight) {
				if ((scaleDown && image.ScaledWidth > _content.Width) ||
					(scaleUp && image.ScaledWidth < _content.Width)) {
					image.ScaleToFit(_content.Width, 999999);
				}
				_doc.SetPageSize(new Rectangle(_content.Width, image.ScaledHeight + _doc.TopMargin + _doc.BottomMargin));
			}
			else {
				if (_autoRotate
					&& (// 页面不足以放下当前尺寸的图片
						(image.ScaledHeight > _content.Height || image.ScaledWidth > _content.Width)
							&& (image.ScaledWidth > image.ScaledHeight && _portrait == true
								|| image.ScaledHeight > image.ScaledWidth && _portrait == false)
						||
						// 图片较小，可以还原为原始的页面方向
						(_portrait != image.Height > image.Width
							&& image.ScaledHeight <= _content.Height && image.ScaledWidth <= _content.Width
							&& image.ScaledHeight <= _content.Width && image.ScaledWidth <= _content.Height)
					)
					) {
					var t = _content.Height;
					_content.Height = _content.Width;
					_content.Width = t;
					_doc.SetPageSize(new Rectangle(_doc.PageSize.Height, _doc.PageSize.Width));
					if (areMarginsEqual == false) {
						if (_portrait) {
							_doc.SetMargins(_doc.BottomMargin, _doc.TopMargin, _doc.LeftMargin, _doc.RightMargin);
						}
						else {
							_doc.SetMargins(_doc.TopMargin, _doc.BottomMargin, _doc.RightMargin, _doc.LeftMargin);
						}
					}
					_portrait = !_portrait;
				}
				if ((scaleDown && (image.ScaledHeight > _content.Height || image.ScaledWidth > _content.Width))
					|| (scaleUp && image.ScaledHeight < _content.Height && image.ScaledWidth < _content.Width)) {
					image.ScaleToFit(_content.Width, _content.Height);
				}
				float px = 0, py = 0;
				if (hAlign == HorizontalAlignment.Center) {
					px = (_content.Width - image.ScaledWidth) / 2f;
				}
				else if (hAlign == HorizontalAlignment.Right) {
					px = _content.Width - image.ScaledWidth;
				}
				if (vAlign == VerticalAlignment.Middle) {
					py = (_content.Height - image.ScaledHeight) / 2f;
				}
				else if (vAlign == VerticalAlignment.Top) {
					py = _content.Height - image.ScaledHeight;
				}
				image.SetAbsolutePosition(_doc.LeftMargin + px, _doc.BottomMargin + py);
			}
			_doc.NewPage();
			_doc.Add(image);
			_doc.NewPage();
		}

		static Image LoadImageFrame(SourceItem.Image source, bool recompressWithJbig2, ref FreeImageBitmap fi) {
			iTextImage image;
			var cropOptions = source.Cropping;
			FREE_IMAGE_FORMAT format;
			if (fi.ImageFormat == FREE_IMAGE_FORMAT.FIF_GIF
				|| fi.InfoHeader.biCompression == FreeImage.BI_PNG) {
				format = FREE_IMAGE_FORMAT.FIF_PNG;
			}
			else if (fi.ColorDepth > 8
				&& fi.ColorType == FREE_IMAGE_COLOR_TYPE.FIC_RGB
				&& fi.HasPalette == false
				&& __JpgFormats.Contains(fi.PixelFormat)
				&& (fi.ImageFormat != FREE_IMAGE_FORMAT.FIF_TIFF || fi.InfoHeader.biCompression != 0)) {
				format = FREE_IMAGE_FORMAT.FIF_JPEG;
			}
			else if (fi.PixelFormat == PixelFormat.Format1bppIndexed && fi.ImageFormat != FREE_IMAGE_FORMAT.FIF_TIFF && recompressWithJbig2 == false) {
				format = FREE_IMAGE_FORMAT.FIF_TIFF;
			}
			else if (fi.ColorDepth > 8) {
				format = FREE_IMAGE_FORMAT.FIF_PNG;
			}
			else if (fi.InfoHeader.biCompression == FreeImage.BI_JPEG) {
				format = FREE_IMAGE_FORMAT.FIF_JPEG;
			}
			else {
				format = fi.ImageFormat;
			}
			using (MemoryStream ms = new MemoryStream()) {
				if (cropOptions.NeedCropping
					&& (fi.Height < cropOptions.MinHeight // 不满足尺寸限制
						|| fi.Width < cropOptions.MinWidth
						|| fi.Height <= cropOptions.Top + cropOptions.Bottom // 裁剪后尺寸小于 0
						|| fi.Width <= cropOptions.Left + cropOptions.Right) == false) {
					var temp = fi.Copy(cropOptions.Left, cropOptions.Top, fi.Width - cropOptions.Right, fi.Height - cropOptions.Bottom);
					temp.Save(ms, format);
					fi.Dispose();
					fi = temp;
				}
				else {
					fi.Save(ms, format);
				}
				ms.Flush();
				ms.Position = 0;
				if (recompressWithJbig2 && fi.PixelFormat == PixelFormat.Format1bppIndexed) {
					image = iTextImage.GetInstance(fi.Width, fi.Height, Imaging.JBig2Encoder.Encode(fi), null);
				}
				else {
					try {
						image = iTextImage.GetInstance(ms.ToArray(), true);
					}
					catch (IndexOutOfRangeException) {
						// 在某些场合下 FreeImage 保存的流无法被读取，尝试读原始文件，让 iTextImage 自行解析
						image = iTextImage.GetInstance(source.FilePath.ReadAllBytes(), true);
					}
				}
			}
			if (fi.HorizontalResolution != fi.VerticalResolution) {
				image.SetDpi(fi.HorizontalResolution.ToInt32(), fi.VerticalResolution.ToInt32());
			}
			return image;
		}
	}
}
