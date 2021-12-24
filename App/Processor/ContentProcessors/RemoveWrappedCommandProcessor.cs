using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	sealed class RemoveWrappedCommandProcessor : IPageProcessor
	{
		int _processedPageCount;
		private int _RemoveLeading, _RemoveTrailing;

		public RemoveWrappedCommandProcessor(int removeLeadingCommandCount, int removeTrailingCommandCount) {
			_RemoveLeading = removeLeadingCommandCount;
			_RemoveTrailing = removeTrailingCommandCount;
		}

		#region IPageProcessor 成员
		public string Name { get { return "删除页面起始或结束指令"; } }

		public void BeginProcess (DocProcessorContext context) {
			_processedPageCount = 0;
		}
		public bool EndProcess (PdfReader pdf) {
			Tracker.TraceMessage (Tracker.Category.Notice, this.Name + "功能：");
			Tracker.TraceMessage ("　　删除了 " + _processedPageCount + " 页的指令。");
			return false;
		}

		public int EstimateWorkload (PdfReader pdf) {
			return pdf.NumberOfPages * 3;
		}

		public bool Process (PageProcessorContext context) {
			Tracker.IncrementProgress (3);
			var p = context.PageCommands;
			var r = ProcessCommands (p.Commands);
			if (r) {
				context.IsPageContentModified = true;
				_processedPageCount++;
			}
			return r;
		}

		#endregion

		bool ProcessCommands (IList<PdfPageCommand> parent) {
			var r = false;
			if (_RemoveLeading > 0) {
				for (int i = _RemoveLeading - 1; i >= 0 && parent.Count > 0; i--) {
					parent.RemoveAt(i);
				}
				r = true;
			}
			if (_RemoveTrailing > 0) {
				for (int i = _RemoveTrailing - 1; i >= 0 && parent.Count > 0; i--) {
					parent.RemoveAt(parent.Count - 1);
				}
				r = true;
			}
			return r;
		}

	}
}
