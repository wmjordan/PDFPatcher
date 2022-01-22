using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

internal sealed class ImportOcrResultProcessor : IDocProcessor
{
	private static readonly PdfName PieceInfo = new("PieceInfo");
	private static readonly PdfName ApplicationName = new(Constants.AppEngName);
	private static readonly PdfName LastModified = new("LastModified");
	private static readonly PdfName OcrFont = new("OcrFont");
	private static readonly PdfName OcrFontV = new("OcrFontV");
	private static readonly PdfName GbkEucH = new("GBK-EUC-H");
	private static readonly PdfName GbkEucV = new("GBK-EUC-V");
	private static readonly PdfName DescendantFonts = new("DescendantFonts");
	private static readonly PdfName FontName = new("STSong-Light");
	private static readonly PdfName OcrResultBmcName = new("XXOcrResult");
	private static readonly Encoding GbkEncoding = Encoding.GetEncoding("GBK");

	private PdfIndirectReference font;
	private PdfIndirectReference fontV;

	private void ImportOcrResult(DocProcessorContext context, XmlReader x) {
		int l = 0;
		PdfReader pdf = context.Pdf;
		int pn = pdf.NumberOfPages;
		XmlDocument xd = new();
		while (x.EOF == false) {
			// 读取一页识别结果
			if (x.MoveToContent() != XmlNodeType.Element
				|| x.Name != Constants.Ocr.Result
				|| x.GetAttribute(Constants.Content.PageNumber).TryParse(out int p) == false
				|| p < 1 || p > pn) {
				x.Skip();
				continue;
			}

			using (XmlReader r = x.ReadSubtree()) {
				xd.Load(r);
			}

			PdfDictionary page = pdf.GetPageN(p);
			PdfPageCommandProcessor cp = new();
			cp.ProcessContent(pdf.GetPageContent(p), page.GetAsDict(PdfName.RESOURCES));
			IList<PdfPageCommand> commands = cp.Commands;
			ClearPreviousOcrResult(commands);
			// 用“q”操作符括起旧的命令
			if (commands.Count > 1) {
				EnclosingCommand q = EnclosingCommand.Create("q", null);
				foreach (PdfPageCommand item in commands) {
					q.Commands.Add(item);
				}

				commands.Clear();
				commands.Add(q);
			}

			// 写入各图像的识别结果
			XmlNodeList ir = xd.SelectNodes(Constants.Ocr.Result + "/" + Constants.Ocr.Image);
			int fontUse = 0;
			foreach (XmlElement image in ir) {
#if DEBUG
				EnclosingCommand bt = EnclosingCommand.Create("BT", null);
#else
					var bt = EnclosingCommand.Create ("BT", null,
						PdfPageCommand.Create ("Tr", new PdfNumber (3))
						);
#endif
				EnclosingCommand bmc = EnclosingCommand.Create("BMC", new PdfObject[] { OcrResultBmcName }, bt);
				fontUse |= ImportImageOcrResult(bt, image);
				commands.Add(bmc);
			}

			cp.WritePdfCommands(pdf, p);
			if ((fontUse & 1) > 0) {
				CreatePageOcrFontReference(context, page, font);
			}

			if ((fontUse & 2) > 0) {
				CreatePageOcrFontReference(context, page, fontV);
			}

			if (l >= pn) {
				continue;
			}

			l++;
			Tracker.IncrementProgress(1);
		}
	}

	private static void ClearPreviousOcrResult(IList<PdfPageCommand> commands) {
		for (int i = commands.Count - 1; i >= 0; i--) {
			if (commands[i] is not EnclosingCommand c
				|| c.HasOperand == false
				|| c.Name.ToString() != "BMC"
				|| OcrResultBmcName.Equals(c.Operands[0]) == false) {
				continue;
			}

			commands.RemoveAt(i);
		}
	}

	private void CreateGlobalOcrFontReference(DocProcessorContext context) {
		PdfDictionary c = context.Pdf.Catalog;
		PdfDictionary d;
		if ((d = c.Locate<PdfDictionary>(false, PieceInfo, ApplicationName)) == null) {
			d = c.CreateDictionaryPath(PieceInfo, ApplicationName);
			d.Put(LastModified, new PdfString("D:" + DateTime.Now.ToString("yyMMddHHmmss")));
		}

		font = CreateOcrFont(context, d, false);
		fontV = CreateOcrFont(context, d, true);
	}

	private static PdfIndirectReference CreateOcrFont(DocProcessorContext context, PdfDictionary d, bool isVertical) {
		PdfName fontName = isVertical ? OcrFontV : OcrFont;
		PdfIndirectReference fontRef = d.GetAsIndirectObject(fontName);
		if (fontRef != null && d.GetDirectObject(fontName) as PdfDictionary != null) {
			return fontRef;
		}

		fontRef = CreateOcrFont(context, isVertical);
		d.Put(fontName, fontRef);

		return fontRef;
	}

	private static PdfIndirectReference CreateOcrFont(DocProcessorContext context, bool isVertical) {
		PdfDictionary f = new(PdfName.FONT);

		f.Put(PdfName.SUBTYPE, PdfName.TYPE0);
		f.Put(PdfName.BASEFONT, isVertical ? OcrFontV : OcrFont);
		f.Put(PdfName.ENCODING, isVertical ? GbkEucV : GbkEucH);

		PdfArray a = new();
		f.Put(DescendantFonts, a);
		// DescendantFont
		PdfDictionary df = new(PdfName.FONT);
		a.Add(context.Pdf.AddPdfObject(df));

		df.Put(PdfName.SUBTYPE, PdfName.CIDFONTTYPE0);
		df.Put(PdfName.BASEFONT, FontName);
		PdfDictionary csi = new();
		csi.Put(PdfName.REGISTRY, new PdfString("Adobe"));
		csi.Put(PdfName.ORDERING, new PdfString("GB1"));
		csi.Put(PdfName.SUPPLEMENT, new PdfNumber(3));
		df.Put(PdfName.CIDSYSTEMINFO, csi);
		csi = null;

		// FontDescriptor
		PdfDictionary fd = new(PdfName.FONTDESCRIPTOR);
		df.Put(PdfName.FONTDESCRIPTOR, context.Pdf.AddPdfObject(fd));
		fd.Put(PdfName.ASCENT, new PdfNumber(857));
		fd.Put(PdfName.CAPHEIGHT, new PdfNumber(857));
		fd.Put(PdfName.DESCENT, new PdfNumber(-143));
		fd.Put(PdfName.FLAGS, new PdfNumber(4));
		fd.Put(PdfName.FONTBBOX, new PdfArray(new[] { -250, -143, 600, 857 }));
		fd.Put(PdfName.FONTNAME, FontName);
		//fd.Put (PdfName.ITALICANGLE, new PdfNumber (0));
		fd.Put(PdfName.STEMV, new PdfNumber(91));
		fd.Put(new PdfName("StemH"), new PdfNumber(91));
		return context.Pdf.AddPdfObject(f);
	}

	private PdfName CreatePageOcrFontReference(DocProcessorContext context, PdfDictionary page,
		PdfIndirectReference fontRef) {
		PdfDictionary f = page.CreateDictionaryPath(PdfName.RESOURCES, PdfName.FONT);
		foreach (KeyValuePair<PdfName, PdfObject> item in f) {
			if (PdfHelper.PdfReferencesAreEqual(fontRef, item.Value as PdfIndirectReference)) {
				return item.Key;
			}
		}

		context.IsModified = true;
		PdfName n = PdfHelper.PdfReferencesAreEqual(fontRef, font) ? OcrFont : OcrFontV;
		f.Put(n, fontRef);
		return n;
	}

	private static int ImportImageOcrResult(IPdfPageCommandContainer container, XmlElement result) {
		int w = 0, h = 0;
		IList<PdfPageCommand> sc = container.Commands;
		XmlNodeList chars = result.SelectNodes(Constants.Ocr.Content);
		if (chars.Count == 0) {
			return 0;
		}

		if (result.GetAttribute(Constants.Coordinates.Width).TryParse(out w) == false
			|| result.GetAttribute(Constants.Coordinates.Height).TryParse(out h) == false
			|| w <= 0
			|| h <= 0
		   ) {
			Tracker.TraceMessage(string.Concat("识别结果的“", Constants.Ocr.Image, "”元素", w <= 0 ? "宽属性无效" : string.Empty,
				h <= 0 ? "高属性无效" : string.Empty, "。"));
			return 0;
		}

		string m = result.GetAttribute(Constants.Content.OperandNames.Matrix);
		if (string.IsNullOrEmpty(m)) {
			Tracker.TraceMessage(string.Concat("识别结果的“", Constants.Ocr.Image, "”元素缺少",
				Constants.Content.OperandNames.Matrix, "属性。"));
			return 0;
		}

		float[] matrix = DocInfoImporter.ToSingleArray(m, true);
		if (matrix == null || matrix.Length < 6) {
			Tracker.TraceMessage(string.Concat("识别结果的“", Constants.Ocr.Image, "”元素中，",
				Constants.Content.OperandNames.Matrix, "属性值无效。"));
			return 0;
		}

		OcrContentInfo info = new(w, h, matrix);
		sc.Add(PdfPageCommand.Create("Tm",
			new PdfNumber(matrix[OcrContentInfo.A1] / w), new PdfNumber(matrix[OcrContentInfo.A2] / w),
			new PdfNumber(matrix[OcrContentInfo.B1] / h), new PdfNumber(matrix[OcrContentInfo.B2] / h),
			new PdfNumber(matrix[OcrContentInfo.DX]), new PdfNumber(matrix[OcrContentInfo.DY])
		)); // 设置初始偏移
		float fSize = -1f;
		bool isV = false, hasHFont = false, hasVFont = false;
		foreach (object item in chars) {
			if (!info.GetInfo(item as XmlElement) || string.IsNullOrEmpty(info.Text) != false) {
				continue;
			}

			PdfName fn;
			if (info.IsVertical) {
				hasVFont = true;
				fn = OcrFontV;
			}
			else {
				hasHFont = true;
				fn = OcrFont;
			}

			if (info.FontSize != fSize) {
				sc.Add(PdfPageCommand.Create("Tf", fn, new PdfNumber(info.FontSize)));
				fSize = info.FontSize;
				isV = info.IsVertical;
			}
			else if (isV != info.IsVertical) {
				sc.Add(PdfPageCommand.Create("Tf", fn, new PdfNumber(fSize)));
			}

			sc.Add(PdfPageCommand.Create("Td", new PdfNumber(info.DeltaX), new PdfNumber(info.DeltaY)));
			sc.Add(PdfPageCommand.Create("Tj", new PdfString(GbkEncoding.GetBytes(info.Text))));
		}

		return (hasHFont ? 1 : 0) + (hasVFont ? 2 : 0);
	}

	private sealed class OcrContentInfo
	{
		internal const int A1 = 0, A2 = 1, B1 = 2, B2 = 3, DX = 4, DY = 5; // 矩阵数组索引
		private int _cx, _cy;

		//float _ix, _iy, _dx, _dy;
		private string _text;

		private int _top, _bottom, _left, _right, _size;

		//float _m11, _m12, _m21, _m22, _mx, _my;
		internal OcrContentInfo(int imageWidth, int imageHeight, float[] matrix) {
			ImageHeight = imageHeight;
			ImageWidth = imageWidth;
			//_m11 = matrix[A1];
			//_m12 = matrix[A2];
			//_m21 = matrix[B1];
			//_m22 = matrix[B2];
			//_mx = matrix[DX];
			//_my = matrix[DY];
			//_ix = 1 / (float)imageWidth;
			//_iy = 1 / (float)imageHeight;
		}

		private int ImageWidth { get; }
		private int ImageHeight { get; }
		internal string Text { get; private set; }
		internal int DeltaX { get; private set; }

		internal int DeltaY { get; private set; }

		internal bool IsVertical { get; private set; }

		internal float FontSize => _size;

		internal bool GetInfo(XmlElement ocrInfoItem) {
			if (ocrInfoItem.GetAttribute(Constants.Coordinates.Top).TryParse(out _top) == false
				|| ocrInfoItem.GetAttribute(Constants.Coordinates.Bottom).TryParse(out _bottom) == false
				|| ocrInfoItem.GetAttribute(Constants.Coordinates.Left).TryParse(out _left) == false
				|| ocrInfoItem.GetAttribute(Constants.Coordinates.Right).TryParse(out _right) == false
				|| _top < 0 || _bottom < 0 || _left < 0 || _right < 0
				|| _top > ImageHeight || _bottom > ImageHeight || _left > ImageWidth || _right > ImageWidth
				|| string.IsNullOrEmpty(_text = ocrInfoItem.GetAttribute(Constants.Ocr.Text))
			   ) {
				return false;
			}

			IsVertical = ocrInfoItem.GetAttribute(Constants.Coordinates.Direction) == Constants.Coordinates.Vertical;
			_size = Math.Abs(IsVertical ? _right - _left : _bottom - _top);
			if (IsVertical == false) {
				_bottom = ImageHeight - _bottom;
				DeltaX = _left - _cx;
				DeltaY = _bottom - _cy;
				_cx = _left;
				_cy = _bottom;
			}
			else {
				_top = ImageHeight - _top;
				DeltaX = _left - _cx;
				DeltaY = _top - _cy;
				_cx = _left;
				_cy = _top;
			}

			Text = _text;
			return true;
		}
	}

	#region IDocProcessor 成员

	public string Name => "导入光学字符识别结果";

	public int EstimateWorkload(PdfReader pdf) {
		return pdf.NumberOfPages;
	}

	public void BeginProcess(DocProcessorContext context) {
		if (context.ExtraData[DocProcessorContext.OcrData] is not XmlReader x) {
			return;
		}

		if (x.Name == Constants.PdfInfo) {
			x.Read();
			x.MoveToContent();
		}

		CreateGlobalOcrFontReference(context);
	}

	public bool Process(DocProcessorContext context) {
		if (context.ExtraData[DocProcessorContext.OcrData] is not XmlReader x || x.Name != Constants.Ocr.Result) {
			return false;
		}

		ImportOcrResult(context, x);
		return true;
	}

	public void EndProcess(DocProcessorContext context) {
	}

	#endregion
}