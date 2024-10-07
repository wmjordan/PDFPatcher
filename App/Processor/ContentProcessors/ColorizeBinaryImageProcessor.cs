using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	/// <summary>
	/// 设置黑白图片的颜色。
	/// </summary>
	sealed class ColorizeBinaryImageProcessor : IPageProcessor
	{
		int _processedPageCount;

		#region IPageProcessor 成员
		public string Name => "设置黑白图片颜色";

		public void BeginProcess(DocProcessorContext context) {
			_processedPageCount = 0;
		}
		public bool EndProcess(PdfReader pdf) {
			Tracker.TraceMessage(Tracker.Category.Notice, Name + "功能：");
			Tracker.TraceMessage("　　修改了 " + _processedPageCount + " 个黑白图片的颜色。");
			return false;
		}

		public int EstimateWorkload(PdfReader pdf) {
			return pdf.NumberOfPages;
		}

		public bool Process(PageProcessorContext context) {
			Tracker.IncrementProgress(1);
			var images = context.Page.Locate<PdfDictionary>(PdfName.RESOURCES, PdfName.XOBJECT);
			if (images == null) {
				return false;
			}
			var bw = new List<PdfName>();
			foreach (var item in images) {
				if (PdfReader.GetPdfObject(item.Value) is not PRStream im
					|| PdfName.IMAGE.Equals(im.GetAsName(PdfName.SUBTYPE)) == false
					|| im.TryGetInt32(PdfName.BITSPERCOMPONENT, 0) != 1
					) {
					continue;
				}
				bw.Add(item.Key);
			}
			if (ProcessCommands(context.PageCommands.Commands, bw)) {
				context.IsPageContentModified = true;
				_processedPageCount++;
				return true;
			}
			return false;
		}

		#endregion

		static bool ProcessCommands(IList<PdfPageCommand> parent, IList<PdfName> bwImages) {
			var r = false;
			PdfPageCommand cmd;
			for (int i = 0; i < parent.Count; i++) {
				cmd = parent[i];
				if (cmd is EnclosingCommand ec) {
					r |= ProcessCommands(ec.Commands, bwImages);
					continue;
				}
				if (cmd.Name.ToString() == "Do") {
					foreach (var item in bwImages) {
						if (item.Equals(cmd.Operands[0])) {
							parent.Insert(i, new AdjustCommand("RG", new PdfNumber(1), new PdfNumber(0), new PdfNumber(0)));
							parent.Insert(i, new AdjustCommand("rg", new PdfNumber(0), new PdfNumber(1), new PdfNumber(0)));
							return true;
						}
					}
				}
			}
			return r;
		}

	}
}
