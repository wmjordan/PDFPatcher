using System;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	sealed class PageDimensionProcessor : IPageProcessor
	{
		CoordinateTranslationSettings[] _cts;
		bool _resizePages, _adjustMargins;
		PageRangeCollection _pageRanges;
		PaperSize _refPaperSize;
		public PageBoxSettings Settings { get; set; }

		internal static CoordinateTranslationSettings ResizePage(PdfDictionary page, PageBoxSettings settings, PaperSize refPaperSize) {
			var size = refPaperSize ?? settings.PaperSize.Clone();
			var hAlign = settings.HorizontalAlign;
			var vAlign = settings.VerticalAlign;
			var pb = page.GetAsArray(PdfName.CROPBOX);
			var b = pb != null ? PdfReader.GetNormalizedRectangle(pb) : null;
			pb = page.GetAsArray(PdfName.MEDIABOX);
			if (pb == null) {
				throw new PdfException("页面缺少 MediaBox。");
			}
			var mb = PdfReader.GetNormalizedRectangle(pb);
			var n = PdfHelper.GetPageRotation(page);
			if (n == 90 || n == 270) {
				size = new PaperSize(size.PaperName, size.Height, size.Width);
			}
			// 自动旋转页面适应原页面的方向
			if (settings.AutoRotation && size.SpecialSize == SpecialPaperSize.None && (size.Width > size.Height) ^ (mb.Width > mb.Height)) {
				size = new PaperSize(size.PaperName, size.Height, size.Width);
			}
			if (b == null) {
				b = new Rectangle(mb);
			}
			float d, z = 1, zx = 1, zy = 1;
			float dx = 0, dy = 0;
			float sw = b.Width, sh = b.Height; // resized width and height
			if (size.SpecialSize == SpecialPaperSize.FixedWidthAutoHeight || size.SpecialSize == SpecialPaperSize.AsWidestPage || size.SpecialSize == SpecialPaperSize.AsNarrowestPage) {
				size.Height = b.Height * size.Width / b.Width;
			}

			if (settings.ScaleContent) {
				zx = size.Width / b.Width;
				zy = size.Height / b.Height;
				z = zx < zy ? zx : zy;
				sw *= z;
				sh *= z;
				b.Left *= z;
				b.Bottom *= z;
				b.Top *= z;
				b.Right *= z;
			}
			if (b.Width != size.Width) {
				d = size.Width - sw;
				dx = hAlign == HorizontalAlignment.Left ? 0 : hAlign == HorizontalAlignment.Right ? d : d / 2;
				b.Left -= dx;
				b.Right = b.Left + size.Width;
			}
			if (b.Height != size.Height) {
				d = size.Height - sh;
				dy = vAlign == VerticalAlignment.Bottom ? d : vAlign == VerticalAlignment.Top ? 0 : d / 2;
				b.Top += dy;
				b.Bottom = b.Top - size.Height;
			}

			var a = new float[] { b.Left, b.Bottom, b.Right, b.Top };
			page.Put(PdfName.CROPBOX, new PdfArray(a));
			ResizeBox(page, mb, b);
			if (page.GetAsArray(PdfName.BLEEDBOX) != null) {
				ResizeBox(page, PdfReader.GetNormalizedRectangle(page.GetAsArray(PdfName.BLEEDBOX)), b);
			}
			if (page.GetAsArray(PdfName.TRIMBOX) != null) {
				ResizeBox(page, PdfReader.GetNormalizedRectangle(page.GetAsArray(PdfName.TRIMBOX)), b);
			}
			if (page.GetAsArray(PdfName.ARTBOX) != null) {
				ResizeBox(page, PdfReader.GetNormalizedRectangle(page.GetAsArray(PdfName.ARTBOX)), b);
			}
			//if (p.Contains (PdfName.BLEEDBOX)) {
			//    p.Put (PdfName.BLEEDBOX, pr);
			//}
			//if (p.Contains(PdfName.TRIMBOX)) {
			//    p.Put (PdfName.TRIMBOX, pr);
			//}
			//if (p.Contains(PdfName.ARTBOX)) {
			//    p.Put (PdfName.ARTBOX, pr);
			//}

			var ct = new CoordinateTranslationSettings();
			if (settings.ScaleContent) {
				ct.XScale = ct.YScale = z;
			}
			else {
				ct.XTranslation = -dx;
				ct.YTranslation = -dy;
			}
			return ct;
		}

		static void ResizeBox(PdfDictionary page, Rectangle box, Rectangle refBox) {
			page.Put(PdfName.MEDIABOX, new PdfArray(new float[]{
				box.Left < refBox.Left ? box.Left : refBox.Left,
				box.Bottom < refBox.Bottom ? box.Bottom : refBox.Bottom,
				box.Right > refBox.Right ? box.Right : refBox.Right,
				box.Top > refBox.Top ? box.Top : refBox.Top
			}));
		}

		static bool RotatePage(PdfDictionary page, int pageNumber, PageBoxSettings settings) {
			if (settings.Rotation == 0) {
				return false;
			}
			var mb = GetPageBox(page);
			var ls = mb.Width > mb.Height; // Landscape
			if (ls && (settings.Filter & PageFilterFlag.Portrait) == PageFilterFlag.Portrait
				|| ls == false && (settings.Filter & PageFilterFlag.Landscape) == PageFilterFlag.Landscape) {
				return false;
			}
			var n = (PdfHelper.GetPageRotation(page) + settings.Rotation) % 360;
			if (n != 0) {
				page.Put(PdfName.ROTATE, n);
			}
			else {
				page.Remove(PdfName.ROTATE);
			}
			return true;
		}

		static Rectangle GetPageBox(PdfDictionary page) {
			var pb = page.GetAsArray(PdfName.CROPBOX) ?? page.GetAsArray(PdfName.MEDIABOX);
			if (pb == null) {
				throw new PdfException("页面缺少 MediaBox。");
			}
			return PdfReader.GetNormalizedRectangle(pb);
		}

		static bool FilterPageNumber(int pageNumber, PageFilterFlag filter) {
			var odd = (pageNumber & 1) > 0;
			if (odd && (filter & PageFilterFlag.Even) == PageFilterFlag.Even
				|| (odd == false && (filter & PageFilterFlag.Odd) == PageFilterFlag.Odd)
				) {
				return false;
			}
			return true;
		}

		/// <summary>
		/// 无损拉伸平移页面。
		/// </summary>
		/// <param name="pdf">PDF 文档。</param>
		/// <param name="pageNumber">页码。</param>
		/// <param name="ct">拉伸及平移参数。</param>
		internal static byte[] ScaleContent(PdfReader pdf, int pageNumber, CoordinateTranslationSettings ct) {
			var newContent = Encoding.ASCII.GetBytes(String.Join(" ", new string[] {
				ct.XScale.ToText (), "0",
				"0", ct.YScale.ToText (),
				ct.XTranslation.ToText (), ct.YTranslation.ToText (), "cm "
			}));
			var cb = pdf.GetPageContent(pageNumber);
			Array.Resize(ref newContent, cb.Length + newContent.Length);
			cb.CopyTo(newContent, newContent.Length - cb.Length);
			pdf.SafeSetPageContent(pageNumber, newContent);

			var page = pdf.GetPageN(pageNumber);
			RewriteAnnotationCoordinates(ct, page);
			return newContent;
		}

		static void ScaleContent(PageProcessorContext context, CoordinateTranslationSettings ct) {
			var cmds = context.PageCommands.Commands;
			if (cmds.Count > 0 && cmds[0].Type == PdfPageCommandType.Matrix) {
				var c = cmds[0] as MatrixCommand;
				if (c.Name.ToString() == "cm") {
					c.Multiply(new double[] { ct.XScale, 0, 0, ct.YScale, ct.XTranslation, ct.YTranslation });
				}
			}
			else {
				cmds.Insert(0, new MatrixCommand(MatrixCommand.CM, ct.XScale, 0, 0, ct.YScale, ct.XTranslation, ct.YTranslation));
			}
			RewriteAnnotationCoordinates(ct, context.Page);
		}

		static void RewriteAnnotationCoordinates(CoordinateTranslationSettings ct, PdfDictionary page) {
			var ann = page.GetAsArray(PdfName.ANNOTS);
			if (ann == null) {
				return;
			}
			foreach (var item in ann.ArrayList) {
				var an = PdfReader.GetPdfObject(item) as PdfDictionary;
				if (an != null) {
					var rect = an.GetAsArray(PdfName.RECT);
					if (rect != null && rect.Size == 4) {
						rect[0] = new PdfNumber((rect[0] as PdfNumber).FloatValue * ct.XScale + ct.XTranslation);
						rect[1] = new PdfNumber((rect[1] as PdfNumber).FloatValue * ct.YScale + ct.YTranslation);
						rect[2] = new PdfNumber((rect[2] as PdfNumber).FloatValue * ct.XScale + ct.XTranslation);
						rect[3] = new PdfNumber((rect[3] as PdfNumber).FloatValue * ct.YScale + ct.YTranslation);
					}
				}
			}
		}

		internal static void AdjustMargins(PdfDictionary page, Margins margins) {
			if (margins.IsRelative) {
				var box = page.GetAsArray(PdfName.CROPBOX) ?? page.GetAsArray(PdfName.MEDIABOX);
				var r = PdfReader.GetNormalizedRectangle(box);
				margins = new Margins(margins.Left * r.Width, margins.Top * r.Height, margins.Right * r.Width, margins.Bottom * r.Height);
			}
			AdjustBoxDimension(page, margins, PdfName.CROPBOX);
			AdjustBoxDimension(page, margins, PdfName.MEDIABOX);
			AdjustBoxDimension(page, margins, PdfName.BLEEDBOX);
			AdjustBoxDimension(page, margins, PdfName.TRIMBOX);
			AdjustBoxDimension(page, margins, PdfName.ARTBOX);
		}

		static void AdjustBoxDimension(PdfDictionary page, Margins margins, PdfName boxName) {
			var b = page.GetAsArray(boxName);
			if (b == null) {
				return;
			}
			var r = PdfReader.GetNormalizedRectangle(b);
			page.Put(boxName, new PdfArray(new float[] {
				r.Left - margins.Left,
				r.Bottom - margins.Bottom,
				r.Right + margins.Right,
				r.Top + margins.Top
			}));
		}

		#region IPageProcessor 成员

		public int EstimateWorkload(PdfReader pdf) {
			return pdf.NumberOfPages;
		}

		public void BeginProcess(DocProcessorContext context) {
			_resizePages = Settings.NeedResize;
			if (_resizePages) {
				context.ExtraData[DocProcessorContext.CoordinateTransition]
					= _cts
					= new CoordinateTranslationSettings[context.Pdf.NumberOfPages + 1];
			}
			_adjustMargins = Settings.NeedAdjustMargins;
			_pageRanges = String.IsNullOrEmpty(Settings.PageRanges) ? null : PageRangeCollection.Parse(Settings.PageRanges, 1, context.Pdf.NumberOfPages, true);
			//todo 为新增加的适应拉伸模式设置参考尺寸
			switch (Settings.PaperSize.SpecialSize) {
				case SpecialPaperSize.AsSpecificPage:
					break;
				case SpecialPaperSize.AsWidestPage:
				case SpecialPaperSize.AsNarrowestPage:
				case SpecialPaperSize.AsLargestPage:
				case SpecialPaperSize.AsSmallestPage:
					_refPaperSize = GetRefPaperSize(context);
					if (_refPaperSize == null) {
						throw new InvalidOperationException("无法匹配与指定尺寸及页码范围相符的页面。");
					}
					break;
			}
		}

		private PaperSize GetRefPaperSize(DocProcessorContext context) {
			Rectangle refRectangle = null;
			var specialSize = Settings.PaperSize.SpecialSize;
			foreach (var range in _pageRanges ?? PageRangeCollection.CreateSingle(1, context.Pdf.NumberOfPages)) {
				foreach (var page in range) {
					var r = context.Pdf.GetPageSizeWithRotation(page);
					if (refRectangle == null) {
						refRectangle = r;
						continue;
					}
					switch (specialSize) {
						case SpecialPaperSize.AsWidestPage:
							if (r.Width > refRectangle.Width) {
								refRectangle = r;
							}
							break;
						case SpecialPaperSize.AsNarrowestPage:
							if (r.Width < refRectangle.Width) {
								refRectangle = r;
							}
							break;
						case SpecialPaperSize.AsLargestPage:
							if (r.Width * r.Height > refRectangle.Width * refRectangle.Height) {
								refRectangle = r;
							}
							break;
						case SpecialPaperSize.AsSmallestPage:
							if (r.Width * r.Height < refRectangle.Width * refRectangle.Height) {
								refRectangle = r;
							}
							break;
					}
				}
			}
			return refRectangle != null
				? new PaperSize(Settings.PaperSize.PaperName, refRectangle.Width, specialSize != SpecialPaperSize.AsNarrowestPage && specialSize != SpecialPaperSize.AsWidestPage ? refRectangle.Height : 0)
				: null;
		}

		public bool Process(PageProcessorContext context) {
			var f = Settings.Filter;
			if (FilterPageNumber(context.PageNumber, f) == false) {
				return false;
			}
			if (_pageRanges != null && _pageRanges.IsInRange(context.PageNumber) == false) {
				return false;
			}
			context.Pdf.ResetReleasePage();
			if (_resizePages) {
				var ct = ResizePage(context.Page, Settings, _refPaperSize);
				if (Settings.ScaleContent) {
					ScaleContent(context, ct);
					context.IsPageContentModified = true;
				}
				_cts[context.PageNumber] = ct;
				ct = null;
			}
			if (_adjustMargins) {
				AdjustMargins(context.Page, Settings.Margins);
			}
			if (Settings.Rotation != 0) {
				RotatePage(context.Page, context.PageNumber, Settings);
			}
			context.Pdf.ResetReleasePage();
			return true;
		}

		public bool EndProcess(PdfReader pdf) {
			return false;
		}

		#endregion

		#region IProcessor 成员

		public string Name => "修改页面尺寸";

		#endregion
	}
}
