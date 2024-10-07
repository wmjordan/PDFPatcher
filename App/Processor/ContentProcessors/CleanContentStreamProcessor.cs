using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	/// <summary>
	/// 清理内容流中无用输出指令的处理器。
	/// </summary>
	sealed class CleanContentStreamProcessor : IPageProcessor
	{
		int _processedPageCount;

		#region IPageProcessor 成员
		public string Name => "清理内容指令";
		public void BeginProcess(DocProcessorContext context) {
			_processedPageCount = 0;
		}
		public bool EndProcess(PdfReader pdf) {
			Tracker.TraceMessage(Tracker.Category.Notice, Name + "功能：");
			Tracker.TraceMessage($"　　删除了 {_processedPageCount} 页的冗余指令。");
			return false;
		}

		public int EstimateWorkload(PdfReader pdf) {
			return pdf.NumberOfPages * 3;
		}

		public bool Process(PageProcessorContext context) {
			Tracker.IncrementProgress(3);
			var p = context.PageCommands;
			var r = TrimTrailingAdjustCommands(p);
			if (r) {
				context.IsPageContentModified = true;
				_processedPageCount++;
			}
			return r;
		}

		static bool TrimTrailingAdjustCommands(IPdfPageCommandContainer container) {
			var cmds = container.Commands;
			var l = cmds.Count;
			var m = false;
			for (int i = l - 1; i >= 0; i--) {
				var cmd = cmds[i];
				if (cmd is EnclosingCommand ec) {
					m |= TrimTrailingAdjustCommands(ec);
				}
				if (cmd.HasOutput) {
					break;
				}
				else {
					cmds.RemoveAt(i);
					m = true;
				}
			}
			return m;
		}
		#endregion
	}
}
