using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;

namespace PDFPatcher.Processor
{
	sealed class RemoveThumbnailProcessor : IPageProcessor
	{
		int _processedItemCount;

		#region IPageProcessor 成员
		public string Name => "删除缩略图";
		public void BeginProcess(DocProcessorContext context) {
			_processedItemCount = 0;
		}
		public bool EndProcess(PdfReader pdf) {
			Tracker.TraceMessage(Tracker.Category.Notice, Name + "功能：");
			Tracker.TraceMessage("　　删除了 " + _processedItemCount + " 幅缩略图。");
			return false;
		}
		public int EstimateWorkload(PdfReader pdf) {
			return pdf.NumberOfPages;
		}

		public bool Process(PageProcessorContext context) {
			Tracker.IncrementProgress(1);
			if (context.Page.Contains(PdfName.THUMB) == false) {
				return false;
			}
			context.Page.Remove(PdfName.THUMB);
			_processedItemCount++;
			return true;
		}

		#endregion
	}
}
