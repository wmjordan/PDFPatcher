using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	sealed class ImportOcrResultProcessor : IDocProcessor
	{
		static readonly PdfName PieceInfo = new PdfName("PieceInfo");
		static readonly PdfName ApplicationName = new PdfName(Constants.AppEngName);
		static readonly PdfName LastModified = new PdfName("LastModified");
		static readonly PdfName OcrFont = new PdfName("OcrFont");
		static readonly PdfName OcrFontV = new PdfName("OcrFontV");
		static readonly PdfName GbkEucH = new PdfName("GBK-EUC-H");
		static readonly PdfName GbkEucV = new PdfName("GBK-EUC-V");
		static readonly PdfName DescendantFonts = new PdfName("DescendantFonts");
		static readonly PdfName FontName = new PdfName("STSong-Light");
		static readonly PdfName OcrResultBmcName = new PdfName("XXOcrResult");
		static readonly Encoding GbkEncoding = Encoding.GetEncoding("GBK");

		PdfIndirectReference font;
		PdfIndirectReference fontV;

		public ImportOcrResultProcessor() {
		}
		#region IDocProcessor 成员
		public string Name => "导入光学字符识别结果";

		public int EstimateWorkload(PdfReader pdf) {
			return pdf.NumberOfPages;
		}

		public void BeginProcess(DocProcessorContext context) {
			var x = context.ExtraData[DocProcessorContext.OcrData] as XmlReader;
			if (x == null) {
				return;
			}
			if (x.Name == Constants.PdfInfo) {
				x.Read();
				x.MoveToContent();
			}
			CreateGlobalOcrFontReference(context);
		}

		public bool Process(DocProcessorContext context) {
			var x = context.ExtraData[DocProcessorContext.OcrData] as XmlReader;
			if (x == null || x.Name != Constants.Ocr.Result) {
				return false;
			}
			ImportOcrResult(context, x);
			return true;
		}

		public void EndProcess(DocProcessorContext context) {
		}
		#endregion

		private void ImportOcrResult(DocProcessorContext context, XmlReader x) {
			int l = 0;
			var pdf = context.Pdf;
			int pn = pdf.NumberOfPages;
			var xd = new XmlDocument();
			while (x.EOF == false) {
				// 读取一页识别结果
				int p;
				if (x.MoveToContent() != XmlNodeType.Element
					|| x.Name != Constants.Ocr.Result
					|| x.GetAttribute(Constants.Content.PageNumber).TryParse(out p) == false
					|| p < 1 || p > pn) {
					x.Skip();
					continue;
				}
				using (var r = x.ReadSubtree()) {
					xd.Load(r);
				}
				PdfDictionary page = pdf.GetPageN(p);
				var cp = new PdfPageCommandProcessor();
				cp.ProcessContent(pdf.GetPageContent(p), page.GetAsDict(PdfName.RESOURCES));
				var commands = cp.Commands;
				ClearPreviousOcrResult(commands);
				// 用“q”操作符括起旧的命令
				if (commands.Count > 1) {
					var q = EnclosingCommand.Create("q", null);
					foreach (var item in commands) {
						q.Commands.Add(item);
					}
					commands.Clear();
					commands.Add(q);
				}
				// 写入各图像的识别结果
				var ir = xd.SelectNodes(Constants.Ocr.Result + "/" + Constants.Ocr.Image);
				var fontUse = 0;
				foreach (XmlElement image in ir) {
#if DEBUG
					var bt = EnclosingCommand.Create("BT", null);
#else
					var bt = EnclosingCommand.Create ("BT", null,
						PdfPageCommand.Create ("Tr", new PdfNumber (3))
						);
#endif
					var bmc = EnclosingCommand.Create("BMC", new PdfObject[] { OcrResultBmcName }, bt);
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
				if (l < pn) {
					l++;
					Tracker.IncrementProgress(1);
				}
			}
		}

		private static void ClearPreviousOcrResult(IList<PdfPageCommand> commands) {
			for (int i = commands.Count - 1; i >= 0; i--) {
				var c = commands[i] as EnclosingCommand;
				if (c == null
					|| c.HasOperand == false
					|| c.Name.ToString() != "BMC"
					|| OcrResultBmcName.Equals(c.Operands[0]) == false) {
					continue;
				}
				commands.RemoveAt(i);
			}
		}

		private void CreateGlobalOcrFontReference(DocProcessorContext context) {
			var c = context.Pdf.Catalog;
			PdfDictionary d;
			if ((d = c.Locate<PdfDictionary>(false, PieceInfo, ApplicationName)) == null) {
				d = c.CreateDictionaryPath(PieceInfo, ApplicationName);
				d.Put(LastModified, new PdfString("D:" + DateTime.Now.ToString("yyMMddHHmmss")));
			}
			font = CreateOcrFont(context, d, false);
			fontV = CreateOcrFont(context, d, true);
		}

		private static PdfIndirectReference CreateOcrFont(DocProcessorContext context, PdfDictionary d, bool isVertical) {
			var fontName = isVertical ? OcrFontV : OcrFont;
			PdfIndirectReference fontRef = d.GetAsIndirectObject(fontName);
			if (fontRef == null || (d.GetDirectObject(fontName) as PdfDictionary) == null) {
				fontRef = CreateOcrFont(context, isVertical);
				d.Put(fontName, fontRef);
			}
			return fontRef;
		}

		private static PdfIndirectReference CreateOcrFont(DocProcessorContext context, bool isVertical) {
			var f = new PdfDictionary(PdfName.FONT);

			f.Put(PdfName.SUBTYPE, PdfName.TYPE0);
			f.Put(PdfName.BASEFONT, isVertical ? OcrFontV : OcrFont);
			f.Put(PdfName.ENCODING, isVertical ? GbkEucV : GbkEucH);

			var a = new PdfArray();
			f.Put(DescendantFonts, a);
			// DescendantFont
			var df = new PdfDictionary(PdfName.FONT);
			a.Add(context.Pdf.AddPdfObject(df));

			df.Put(PdfName.SUBTYPE, PdfName.CIDFONTTYPE0);
			df.Put(PdfName.BASEFONT, FontName);
			var csi = new PdfDictionary();
			csi.Put(PdfName.REGISTRY, new PdfString("Adobe"));
			csi.Put(PdfName.ORDERING, new PdfString("GB1"));
			csi.Put(PdfName.SUPPLEMENT, new PdfNumber(3));
			df.Put(PdfName.CIDSYSTEMINFO, csi);
			csi = null;

			// FontDescriptor
			var fd = new PdfDictionary(PdfName.FONTDESCRIPTOR);
			df.Put(PdfName.FONTDESCRIPTOR, context.Pdf.AddPdfObject(fd));
			fd.Put(PdfName.ASCENT, new PdfNumber(857));
			fd.Put(PdfName.CAPHEIGHT, new PdfNumber(857));
			fd.Put(PdfName.DESCENT, new PdfNumber(-143));
			fd.Put(PdfName.FLAGS, new PdfNumber(4));
			fd.Put(PdfName.FONTBBOX, new PdfArray(new int[] { -250, -143, 600, 857 }));
			fd.Put(PdfName.FONTNAME, FontName);
			//fd.Put (PdfName.ITALICANGLE, new PdfNumber (0));
			fd.Put(PdfName.STEMV, new PdfNumber(91));
			fd.Put(new PdfName("StemH"), new PdfNumber(91));
			return context.Pdf.AddPdfObject(f);
		}

		private PdfName CreatePageOcrFontReference(DocProcessorContext context, PdfDictionary page, PdfIndirectReference fontRef) {
			var f = page.CreateDictionaryPath(PdfName.RESOURCES, PdfName.FONT);
			foreach (var item in f) {
				if (PdfHelper.PdfReferencesAreEqual(fontRef, item.Value as PdfIndirectReference)) {
					return item.Key;
				}
			}
			context.IsModified = true;
			var n = PdfHelper.PdfReferencesAreEqual(fontRef, font) ? OcrFont : OcrFontV;
			f.Put(n, fontRef);
			return n;
		}

		static int ImportImageOcrResult(IPdfPageCommandContainer container, XmlElement result) {
			int w = 0, h = 0;
			var sc = container.Commands;
			var chars = result.SelectNodes(Constants.Ocr.Content);
			if (chars.Count == 0) {
				return 0;
			}
			if (result.GetAttribute(Constants.Coordinates.Width).TryParse(out w) == false
				|| result.GetAttribute(Constants.Coordinates.Height).TryParse(out h) == false
				|| w <= 0
				|| h <= 0
				) {
				Tracker.TraceMessage(String.Concat("识别结果的“", Constants.Ocr.Image, "”元素", (w <= 0 ? "宽属性无效" : String.Empty),
					(h <= 0 ? "高属性无效" : String.Empty), "。"));
				return 0;
			}
			var m = result.GetAttribute(Constants.Content.OperandNames.Matrix);
			if (String.IsNullOrEmpty(m)) {
				Tracker.TraceMessage(String.Concat("识别结果的“", Constants.Ocr.Image, "”元素缺少", Constants.Content.OperandNames.Matrix, "属性。"));
				return 0;
			}
			var matrix = DocInfoImporter.ToSingleArray(m, true);
			if (matrix == null || matrix.Length < 6) {
				Tracker.TraceMessage(String.Concat("识别结果的“", Constants.Ocr.Image, "”元素中，", Constants.Content.OperandNames.Matrix, "属性值无效。"));
				return 0;
			}
			var info = new OcrContentInfo(w, h, matrix);
			sc.Add(PdfPageCommand.Create("Tm",
				new PdfNumber(matrix[OcrContentInfo.A1] / w), new PdfNumber(matrix[OcrContentInfo.A2] / w),
				new PdfNumber(matrix[OcrContentInfo.B1] / h), new PdfNumber(matrix[OcrContentInfo.B2] / h),
				new PdfNumber(matrix[OcrContentInfo.DX]), new PdfNumber(matrix[OcrContentInfo.DY])
				)); // 设置初始偏移
			var fSize = -1f;
			bool isV = false, hasHFont = false, hasVFont = false;
			foreach (var item in chars) {
				if (info.GetInfo(item as XmlElement) && String.IsNullOrEmpty(info.Text) == false) {
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
			}
			return (hasHFont ? 1 : 0) + (hasVFont ? 2 : 0);
		}

		sealed class OcrContentInfo
		{
			internal const int A1 = 0, A2 = 1, B1 = 2, B2 = 3, DX = 4, DY = 5; // 矩阵数组索引
			private int ImageWidth { get; }
			private int ImageHeight { get; }
			internal string Text { get; private set; }
			internal int DeltaX => _dx;
			internal int DeltaY => _dy;
			internal bool IsVertical => _isVertical;
			internal float FontSize => _size;

			bool _isVertical;
			//float _ix, _iy, _dx, _dy;
			string _text;
			int _top, _bottom, _left, _right, _size;
			int _cx, _cy, _dx, _dy;
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
			internal bool GetInfo(XmlElement ocrInfoItem) {
				if (ocrInfoItem.GetAttribute(Constants.Coordinates.Top).TryParse(out _top) == false
					|| ocrInfoItem.GetAttribute(Constants.Coordinates.Bottom).TryParse(out _bottom) == false
					|| ocrInfoItem.GetAttribute(Constants.Coordinates.Left).TryParse(out _left) == false
					|| ocrInfoItem.GetAttribute(Constants.Coordinates.Right).TryParse(out _right) == false
					|| _top < 0 || _bottom < 0 || _left < 0 || _right < 0
					|| _top > ImageHeight || _bottom > ImageHeight || _left > ImageWidth || _right > ImageWidth
					|| String.IsNullOrEmpty(_text = ocrInfoItem.GetAttribute(Constants.Ocr.Text))
					) {
					return false;
				}
				_isVertical = ocrInfoItem.GetAttribute(Constants.Coordinates.Direction) == Constants.Coordinates.Vertical;
				_size = Math.Abs(_isVertical ? _right - _left : _bottom - _top);
				if (_isVertical == false) {
					_bottom = ImageHeight - _bottom;
					_dx = _left - _cx;
					_dy = _bottom - _cy;
					_cx = _left;
					_cy = _bottom;
				}
				else {
					_top = ImageHeight - _top;
					_dx = _left - _cx;
					_dy = _top - _cy;
					_cx = _left;
					_cy = _top;
				}
				Text = _text;
				return true;
			}
		}
	}
}
