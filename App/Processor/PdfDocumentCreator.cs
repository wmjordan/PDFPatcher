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
using PDFPatcher.Processor.Imaging;
using iTextImage = iTextSharp.text.Image;

namespace PDFPatcher.Processor;

internal sealed class PdfDocumentCreator
{
	internal static readonly string[] SupportedFileTypes = new string[] {
		".pdf", ".tif", ".jpg", ".gif", ".png", ".tiff", ".bmp", ".jpeg", ".jp2", ".j2k"
	};

	private static readonly string[] __BuiltInImageTypes = {".png", ".jpg", ".jpeg", ".bmp", ".jp2", ".j2k"};
	private static readonly string[] __ExtImageTypes = {".tif", ".tiff", ".gif"};

	private static readonly PixelFormat[] __JpgFormats = new PixelFormat[] {
		PixelFormat.Format16bppGrayScale, PixelFormat.Format16bppRgb555, PixelFormat.Format16bppRgb565,
		PixelFormat.Format24bppRgb, PixelFormat.Format32bppRgb, PixelFormat.Format48bppRgb
	};

	internal static readonly PaperSize[] PaperSizes = new PaperSize[] {
		new(PaperSize.AsPageSize, 0, 0), new(PaperSize.FixedWidthAutoHeight, 0, 0), new(PaperSize.AsWidestPage, 0, 0),
		new(PaperSize.AsNarrowestPage, 0, 0), new(PaperSize.AsLargestPage, 0, 0), new(PaperSize.AsSmallestPage, 0, 0),
		new("16 开 (18.4*26.0)", 1840, 2601), new("32 开 (13.0*18.4)", 1300, 1840), new("大 32 开 (14.0*20.3)", 1400, 2030),
		new("A4 (21.0*29.7)", 2100, 2970), new("A3 (29.7*42.0)", 2971, 4201), new("自定义", 0, 0),
		new("————————————", 0, 0), new("8 开 (26.0*36.8)", 2601, 3681), new("16 开 (18.4*26.0)", 1840, 2601),
		new("大 16 开 (21.0*28.5)", 2100, 2851), new("32 开 (13.0*18.4)", 1300, 1840),
		new("大 32 开 (14.0*20.3)", 1400, 2030), new("8 K (27.3*39.3)", 2731, 3931), new("16 K (19.6*27.3)", 1960, 2731),
		new("A0 (84.1*118.9)", 8410, 11890), new("A1 (59.4*84.1)", 5940, 8410), new("A2 (42.0*59.4)", 4200, 5940),
		new("A3 (29.7*42.0)", 2971, 4201), new("A4 (21.0*29.7)", 2100, 2971), new("A5 (14.8*21.0)", 1480, 2100),
		new("A6 (10.5*14.8)", 1050, 1480), new("B0 (100.0*141.3)", 10000, 14130), new("B1 (70.7*100.0)", 7070, 10000),
		new("B2 (50.0*70.7)", 5000, 7070), new("B3 (35.3*50.0)", 3530, 5000), new("B4 (25.0*35.3)", 2500, 3530),
		new("B5 (17.6*25.0)", 1760, 2501), new("B6 (12.5*17.6)", 1250, 1760)
	};

	private readonly MergerOptions _option;
	private readonly ImporterOptions _impOptions;
	private readonly Document _doc;
	private readonly PdfSmartCopy _writer;
	private readonly PaperSize _content;
	private readonly DocumentSink _sink;
	private readonly PageBoxSettings _pageSettings;
	private readonly bool _autoRotate;
	private readonly HorizontalAlignment hAlign;
	private readonly VerticalAlignment vAlign;
	private readonly bool scaleUp, scaleDown;
	private readonly bool areMarginsEqual;
	private bool _portrait;

	/// <summary>
	/// 在传入构造函数选项中保留链接时，获取最近处理的 PDF 文档的书签。
	/// </summary>
	public PdfInfoXmlDocument PdfBookmarks { get; }

	public PdfDocumentCreator(DocumentSink sink, MergerOptions option, ImporterOptions impOptions,
		Document document, PdfSmartCopy writer) {
		_sink = sink;
		_option = option;
		_impOptions = impOptions;
		_doc = document;
		_writer = writer;
		PageBoxSettings ps = _pageSettings = option.PageSettings;
		_content = new PaperSize(ps.PaperSize.PaperName, option.ContentWidth, option.ContentHeight);
		_portrait = _content.Height > _content.Width;
		_autoRotate = ps.AutoRotation && _content.Height != _content.Width;
		hAlign = ps.HorizontalAlign;
		vAlign = ps.VerticalAlign;
		scaleUp = option.AutoScaleUp;
		scaleDown = option.AutoScaleDown;
		areMarginsEqual = ps.Margins.Top == ps.Margins.Left
		                  && ps.Margins.Top == ps.Margins.Right
		                  && ps.Margins.Top == ps.Margins.Bottom;
		if (impOptions.ImportBookmarks) {
			PdfBookmarks = new PdfInfoXmlDocument();
			BookmarkRootElement root = PdfBookmarks.BookmarkRoot;
		}

		if (_content.SpecialSize == SpecialPaperSize.None) {
			_doc.SetPageSize(new Rectangle(ps.PaperSize.Width, ps.PaperSize.Height));
		}
	}

	internal void ProcessFile(SourceItem sourceFile, BookmarkContainer bookmarkContainer) {
		if (sourceFile.Type != SourceItem.ItemType.Empty) {
			Tracker.TraceMessage(Tracker.Category.InputFile, sourceFile.FilePath.ToString());
		}

		BookmarkElement b = CreateAutoBookmark(sourceFile, bookmarkContainer);
		switch (sourceFile.Type) {
			case SourceItem.ItemType.Empty:
				Tracker.TraceMessage("添加空白页。");
				AddEmptyPage();
				SetBookmarkAction(b);
				break;
			case SourceItem.ItemType.Pdf:
				Tracker.TraceMessage("添加文档：" + sourceFile);
				AddPdfPages(sourceFile as SourceItem.Pdf, b);
				Tracker.IncrementProgress(sourceFile.FileSize);
				break;
			case SourceItem.ItemType.Image:
				Tracker.TraceMessage("添加图片：" + sourceFile);
				AddImagePage(sourceFile, b);
				Tracker.IncrementProgress(sourceFile.FileSize);
				break;
			case SourceItem.ItemType.Folder:
				Tracker.TraceMessage("添加文件夹：" + sourceFile);
				break;
			default:
				break;
		}

		if (sourceFile.HasSubItems) {
			bool p = false;
			int pn = _writer.CurrentPageNumber;
			foreach (SourceItem item in sourceFile.Items) {
				ProcessFile(item, b ?? bookmarkContainer);
				if (p) {
					continue;
				}

				p = true;
				BookmarkElement t = b;
				while (b?.Page == 0) {
					b.Page = pn;
					b.DestinationView = Constants.DestinationAttributes.ViewType.XYZ;
					b.Top = _doc.PageSize.Height;
					b = b.ParentBookmark;
				}

				b = t;
			}
		}
	}

	private void AddImagePage(SourceItem source, BookmarkElement bookmark) {
		string ext = source.FilePath.FileExtension.ToLowerInvariant();
		if (__BuiltInImageTypes.Contains(ext)) {
			iTextImage image = LoadImage(source, ext);
			if (ext == Constants.FileExtensions.Jpg || ext == Constants.FileExtensions.Jpeg) {
				if (JpgHelper.TryGetExifOrientation(source.FilePath, out ushort o) && o != 0) {
					switch (o) {
						case 6:
							image.RotationDegrees = -90;
							break;
						case 3:
							image.RotationDegrees = 180;
							break;
						case 8:
							image.RotationDegrees = 90;
							break;
					}
				}
			}

			if (image == null) {
				Tracker.TraceMessage("无法添加文件：" + source.FilePath);
			}
			else {
				AddImage(image);
				SetBookmarkAction(bookmark);
			}
		}
		else if (__ExtImageTypes.Contains(ext)) {
			FreeImageBitmap fi = null;
			try {
				fi = FreeImageBitmap.FromFile(source.FilePath);
				int c = fi.FrameCount;
				for (int i = 0; i < c; i++) {
					Image img = LoadImageFrame(source as SourceItem.Image, _option.RecompressWithJbig2, ref fi);
					AddImage(img);
					if (i == 0) {
						SetBookmarkAction(bookmark);
					}
				}
			}
			finally {
				if (fi != null) {
					fi.Dispose();
				}
			}
		}
	}

	private void SetBookmarkAction(BookmarkElement bookmark) {
		if (bookmark == null) {
			return;
		}

		bookmark.Page = _writer.PageEmpty ? _writer.CurrentPageNumber - 1 : _writer.CurrentPageNumber;
		bookmark.DestinationView = Constants.DestinationAttributes.ViewType.XYZ;
		bookmark.Top = _doc.PageSize.Height;
	}

	private void AddEmptyPage() {
		if (_content.SpecialSize == SpecialPaperSize.None ||
		    _content.SpecialSize == SpecialPaperSize.AsSpecificPage) {
			// 插入空白页
			_doc.NewPage();
			_writer.PageEmpty = false;
		}
		else {
			Tracker.TraceMessage("没有指定页面尺寸，无法插入空白页。");
		}
	}

	private void AddPdfPages(SourceItem.Pdf sourceFile, BookmarkContainer bookmark) {
		PdfReader pdf = _sink.GetPdfReader(sourceFile.FilePath);
		if (pdf.ConfirmUnethicalMode() == false) {
			Tracker.TraceMessage("忽略了没有权限处理的文件：" + sourceFile.FilePath);
			if (_sink.DecrementReference(sourceFile.FilePath) < 1) {
				pdf.Close();
			}

			return;
		}

		PageRangeCollection ranges = PageRangeCollection.Parse(sourceFile.PageRanges, 1, pdf.NumberOfPages, true);
		int[] pageRemapper = new int[pdf.NumberOfPages + 1];
		// 统一页面旋转角度
		if (_option.UnifyPageOrtientation) {
			bool rv = _option.RotateVerticalPages;
			int a = _option.RotateAntiClockwise ? -90 : 90;
			for (int i = pdf.NumberOfPages; i > 0; i--) {
				PdfDictionary p = pdf.GetPageN(i);
				Rectangle r = PdfHelper.GetPageVisibleRectangle(p);
				if ((rv && r.Width < r.Height)
				    || (rv == false && r.Width > r.Height)) {
					p.Put(PdfName.ROTATE, (r.Rotation + a) % 360);
				}
			}
		}

		if (bookmark != null) {
			int n = _writer.CurrentPageNumber + 1;
			if (_writer.PageEmpty) {
				n--;
			}

			bookmark.SetAttribute(Constants.DestinationAttributes.Page, n.ToText());
			bookmark.SetAttribute(Constants.DestinationAttributes.View,
				Constants.DestinationAttributes.ViewType.XYZ);
			Rectangle r = PdfHelper.GetPageVisibleRectangle(pdf.GetPageN(ranges[0].StartValue));
			float t = 0;
			switch (r.Rotation % 360 / 90) {
				case 0:
					t = r.Top;
					break;
				case 1:
					t = r.Right;
					break;
				case 2:
					t = r.Bottom;
					break;
				case 3:
					t = r.Left;
					break;
			}

			bookmark.SetAttribute(Constants.Coordinates.Top, t.ToText());
		}

		SourceItem.Pdf pdfItem = sourceFile as SourceItem.Pdf;
		bool importImagesOnly = pdfItem.ImportImagesOnly;
		int pn = pdf.NumberOfPages;
		ImageExtractor imgExp = null;
		if (importImagesOnly) {
			imgExp = new ImageExtractor(pdfItem.ExtractImageOptions, pdf);
		}

		if (_option.KeepBookmarks) {
			pdf.ConsolidateNamedDestinations();
		}

		byte[] pp = new byte[pdf.NumberOfPages + 1]; // 已处理过的页面
		CoordinateTranslationSettings[] cts = _pageSettings.PaperSize.SpecialSize != SpecialPaperSize.AsPageSize
			? new CoordinateTranslationSettings[pdf.NumberOfPages + 1]
			: null; // 页面的位置偏移量
		foreach (PageRange r in ranges) {
			foreach (int i in r) {
				if (i < 1 || i > pn) {
					goto Exit;
				}

				if (pageRemapper != null) {
					pageRemapper[i] = _writer.CurrentPageNumber;
				}

				_doc.NewPage();
				if (imgExp != null) {
					imgExp.ExtractPageImages(pdf, i);
					foreach (ImageInfo item in imgExp.InfoList) {
						if (item.FileName != null) {
							ProcessFile(new SourceItem.Image(item.FileName), bookmark);
							File.Delete(item.FileName);
						}
					}
				}
				else {
					if (pp[i] == 0) {
						PdfDictionary page = pdf.GetPageN(i);
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
							CoordinateTranslationSettings ct =
								PageDimensionProcessor.ResizePage(page, _pageSettings, null);
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

		if (_option.KeepBookmarks) {
			bookmark = KeepBookmarks(bookmark, pdf, pageRemapper, cts);
		}

		if (_sink.DecrementReference(sourceFile.FilePath) < 1) {
			_writer.FreeReader(pdf);
			pdf.Close();
		}
	}

	private BookmarkContainer KeepBookmarks(BookmarkContainer bookmark, PdfReader pdf, int[] pageRemapper,
		CoordinateTranslationSettings[] cts) {
		XmlElement bm = OutlineManager.GetBookmark(pdf, new UnitConverter() {Unit = Constants.Units.Point});
		List<IInfoDocProcessor> processors = new();
		if (_option.ViewerPreferences.CollapseBookmark != BookmarkStatus.AsIs) {
			processors.Add(
				new CollapseBookmarkProcessor() {BookmarkStatus = _option.ViewerPreferences.CollapseBookmark});
		}

		if (_option.ViewerPreferences.RemoveZoomRate) {
			processors.Add(new RemoveZoomRateProcessor());
		}

		if (_option.ViewerPreferences.ForceInternalLink) {
			processors.Add(new ForceInternalDestinationProcessor());
		}

		processors.Add(new GotoDestinationProcessor() {
			RemoveOrphanDestination = _option.RemoveOrphanBookmarks, PageRemapper = pageRemapper, TransitionMapper = cts
		});
		ProcessInfoItem(bm, processors);
		if (bookmark != null) {
			bookmark.SetAttribute(Constants.BookmarkAttributes.Open,
				_option.ViewerPreferences.CollapseBookmark == BookmarkStatus.CollapseAll
					? Constants.Boolean.False
					: Constants.Boolean.True);
		}
		else if (PdfBookmarks != null) {
			bookmark = PdfBookmarks.BookmarkRoot;
		}
		else {
			return bookmark;
		}

		if (bm != null) {
			while (bm.FirstChild != null) {
				if (bm.FirstChild.NodeType == XmlNodeType.Element) {
					bookmark.AppendChild(bookmark.OwnerDocument.ImportNode(bm.FirstChild, true));
				}

				bm.RemoveChild(bm.FirstChild);
			}
		}

		return bookmark;
	}

	internal static void ProcessInfoItem(XmlElement item, ICollection<IInfoDocProcessor> processors) {
		if (item == null) {
			return;
		}

		foreach (IInfoDocProcessor p in processors) {
			p.Process(item);
		}

		XmlNode c = item.FirstChild;
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
				//while (c.HasChildNodes) {
				//    var cc = c.FirstChild as XmlElement;
				//    if (cc == null ||
				//        (cc.HasAttribute (Constants.DestinationAttributes.Action) == false
				//            && cc.HasChildNodes == false)) {
				//        c.RemoveChild (cc);
				//        continue;
				//    }
				//    item.InsertAfter (cc, r);
				//}
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

	private static iTextImage LoadImage(SourceItem sourceFile, string ext) {
		SourceItem.Image imageItem = sourceFile as SourceItem.Image;
		SourceItem.CropOptions cropOptions = imageItem.Cropping;
		if (imageItem == null || cropOptions.NeedCropping == false) {
			return Image.GetInstance(sourceFile.FilePath.ToString());
		}

		ext = ext.ToLowerInvariant();
		using (FreeImageBitmap fi = new(sourceFile.FilePath)) {
			if (fi.Height < cropOptions.MinHeight // 不满足尺寸限制
			    || fi.Width < cropOptions.MinWidth
			    || fi.Height <= cropOptions.Top + cropOptions.Bottom // 裁剪后尺寸小于 0
			    || fi.Width <= cropOptions.Left + cropOptions.Right
			   ) {
				return Image.GetInstance(sourceFile.FilePath.ToString());
			}

			if (ext == Constants.FileExtensions.Jpg || ext == ".jpeg") {
				// is JPEG file
				FilePath t = sourceFile.FilePath.EnsureExtension(Constants.FileExtensions.Jpg);
				if (FreeImageBitmap.JPEGCrop(sourceFile.FilePath, t, cropOptions.Left, cropOptions.Top,
					    fi.Width - cropOptions.Right, fi.Height - cropOptions.Bottom)) {
					iTextImage image;
					using (FileStream fs = new(t, FileMode.Open)) {
						image = Image.GetInstance(fs);
					}

					File.Delete(t);
					return image;
				}
			}

			using (FreeImageBitmap tmp = fi.Copy(cropOptions.Left, cropOptions.Top, fi.Width - cropOptions.Right,
				       fi.Height - cropOptions.Bottom))
			using (MemoryStream ms = new()) {
				tmp.Save(ms, fi.ImageFormat);
				ms.Flush();
				ms.Position = 0;
				return Image.GetInstance(ms);
			}
		}
	}

	private BookmarkElement CreateAutoBookmark(SourceItem sourceFile, XmlElement bookmarkContainer) {
		if (PdfBookmarks == null
		    || sourceFile.Bookmark == null
		    || string.IsNullOrEmpty(sourceFile.Bookmark.Title)) {
			return null;
		}

		BookmarkElement b = PdfBookmarks.CreateBookmark(sourceFile.Bookmark);
		bookmarkContainer.AppendChild(b);
		return b;
	}

	private void AddImage(iTextImage image) {
		if (_option.AutoMaskBWImages && image.IsMaskCandidate()) {
			image.MakeMask();
		}

		image.ScalePercent(72f / image.DpiX.SubstituteDefault(72) * 100f,
			72f / image.DpiY.SubstituteDefault(72) * 100f);
		if (_content.SpecialSize == SpecialPaperSize.AsPageSize) {
			_doc.SetPageSize(new Rectangle(image.ScaledWidth + _doc.LeftMargin + _doc.RightMargin,
				image.ScaledHeight + _doc.TopMargin + _doc.BottomMargin));
		}
		else if (_content.SpecialSize == SpecialPaperSize.FixedWidthAutoHeight) {
			if ((scaleDown && image.ScaledWidth > _content.Width) ||
			    (scaleUp && image.ScaledWidth < _content.Width)) {
				image.ScaleToFit(_content.Width, 999999);
			}

			_doc.SetPageSize(new Rectangle(_content.Width,
				image.ScaledHeight + _doc.TopMargin + _doc.BottomMargin));
		}
		else {
			if (_autoRotate
			    && ( // 页面不足以放下当前尺寸的图片
				    ((image.ScaledHeight > _content.Height || image.ScaledWidth > _content.Width)
				     && ((image.ScaledWidth > image.ScaledHeight && _portrait == true)
				         || (image.ScaledHeight > image.ScaledWidth && _portrait == false)))
				    ||
				    // 图片较小，可以还原为原始的页面方向
				    (_portrait != (_option.ContentHeight > _option.ContentWidth)
				     && image.ScaledHeight <= _content.Height && image.ScaledWidth <= _content.Width
				     && image.ScaledHeight <= _content.Width && image.ScaledWidth <= _content.Height)
			    )
			   ) {
				float t = _content.Height;
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

	private static Image LoadImageFrame(SourceItem.Image source, bool recompressWithJbig2, ref FreeImageBitmap fi) {
		iTextImage image;
		SourceItem.CropOptions cropOptions = source.Cropping;
		FREE_IMAGE_FORMAT format;
		if (fi.ImageFormat == FREE_IMAGE_FORMAT.FIF_GIF
		    || fi.InfoHeader.biCompression == FreeImage.BI_PNG) {
			format = FREE_IMAGE_FORMAT.FIF_PNG;
		}
		else if (fi.ColorDepth > 8
		         && fi.ColorType == FREE_IMAGE_COLOR_TYPE.FIC_RGB
		         && fi.HasPalette == false
		         && __JpgFormats.Contains(fi.PixelFormat)) {
			format = FREE_IMAGE_FORMAT.FIF_JPEG;
		}
		else if (fi.InfoHeader.biCompression == FreeImage.BI_JPEG) {
			format = FREE_IMAGE_FORMAT.FIF_JPEG;
		}
		else if (fi.ColorDepth > 16) {
			format = FREE_IMAGE_FORMAT.FIF_PNG;
		}
		else {
			format = fi.ImageFormat;
		}

		using (MemoryStream ms = new()) {
			if (cropOptions.NeedCropping
			    && (fi.Height < cropOptions.MinHeight // 不满足尺寸限制
			        || fi.Width < cropOptions.MinWidth
			        || fi.Height <= cropOptions.Top + cropOptions.Bottom // 裁剪后尺寸小于 0
			        || fi.Width <= cropOptions.Left + cropOptions.Right) == false) {
				FreeImageBitmap temp = fi.Copy(cropOptions.Left, cropOptions.Top, fi.Width - cropOptions.Right,
					fi.Height - cropOptions.Bottom);
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
				image = iTextImage.GetInstance(fi.Width, fi.Height, JBig2Encoder.Encode(fi), null);
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

		//image.ScaleAbsoluteHeight (fi.Height * 72 / fi.VerticalResolution);
		//image.ScaleAbsoluteWidth (fi.Width * 72 / fi.HorizontalResolution);
		return image;
	}
}