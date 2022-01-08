using iTextSharp.text.pdf;

namespace PDFPatcher.Processor
{
	sealed class FixContentProcessor : IPageProcessor
	{
		int _processedPageCount;

		#region IPageProcessor 成员
		public string Name => "修复并删除冗余内容";

		public void BeginProcess(DocProcessorContext context) {
			_processedPageCount = 0;
		}
		public bool EndProcess(PdfReader pdf) {
			Tracker.TraceMessage(Tracker.Category.Notice, Name + "功能：");
			Tracker.TraceMessage("　　删除了 " + _processedPageCount + " 页的冗余内容。");
			return false;
		}

		public int EstimateWorkload(PdfReader pdf) {
			return pdf.NumberOfPages * 3;
		}

		public bool Process(PageProcessorContext context) {
			Tracker.IncrementProgress(3);
			if (ProcessCommands(context.PageCommands)) {
				context.IsPageContentModified = true;
				_processedPageCount++;
				return true;
			}
			return false;
		}

		#endregion

		static bool ProcessCommands(Model.IPdfPageCommandContainer container) {
			var r = false;
			var cl = container.Commands;
			var l = cl.Count;
			for (int i = 0; i < l; i++) {
				if (cl[i] is Model.EnclosingCommand ec) {
					r |= ProcessCommands(ec);
				}
			}
			return r;
		}
	}
}
