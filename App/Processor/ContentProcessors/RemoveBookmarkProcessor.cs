using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;

namespace PDFPatcher.Processor
{
	sealed class RemoveBookmarkProcessor : IDocProcessor
	{
		#region IDocProcessor 成员

		public string Name => "删除导航书签";

		public void BeginProcess(DocProcessorContext context) {
		}

		public void EndProcess(DocProcessorContext context) {
		}

		public int EstimateWorkload(PdfReader pdf) {
			return 1;
		}

		public bool Process(DocProcessorContext context) {
			Tracker.IncrementProgress(1);
			OutlineManager.KillOutline(context.Pdf);
			return true;
		}

		#endregion
	}
}