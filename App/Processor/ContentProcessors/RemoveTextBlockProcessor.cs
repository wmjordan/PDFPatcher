﻿using System.Collections.Generic;
using iTextSharp.text.pdf;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	sealed class RemoveTextBlockProcessor : IPageProcessor
	{
		int _processedPageCount;

		#region IPageProcessor 成员
		public string Name => "删除文本区";

		public void BeginProcess(DocProcessorContext context) {
			_processedPageCount = 0;
		}
		public bool EndProcess(PdfReader pdf) {
			Tracker.TraceMessage(Tracker.Category.Notice, Name + "功能：");
			Tracker.TraceMessage("　　删除了 " + _processedPageCount + " 页的文本。");
			return false;
		}

		public int EstimateWorkload(PdfReader pdf) {
			return pdf.NumberOfPages * 3;
		}

		public bool Process(PageProcessorContext context) {
			Tracker.IncrementProgress(3);
			var p = context.PageCommands;
			var r = false;
			r = ProcessCommands(p.Commands);
			if (r == true) {
				context.IsPageContentModified = true;
				_processedPageCount++;
			}
			ProcessFormContent(context);
			return r;
		}

		static void ProcessFormContent(PageProcessorContext context) {
			var fl = context.Page.Locate<PdfDictionary>(PdfName.RESOURCES, PdfName.XOBJECT);
			if (fl == null) {
				return;
			}
			foreach (var item in fl) {
				if (PdfReader.GetPdfObject(item.Value) is not PRStream f
					|| PdfName.FORM.Equals(f.GetAsName(PdfName.SUBTYPE)) == false) {
					continue;
				}
				var p = new PdfPageCommandProcessor(f);
				if (ProcessCommands(p.Commands)) {
					using (var ms = new System.IO.MemoryStream()) {
						p.WritePdfCommands(ms);
						ms.Flush();
						f.SetData(ms.ToArray(), ms.Length > 32);
					}
				}
			}
		}

		#endregion

		private static bool ProcessCommands(IList<PdfPageCommand> parent) {
			var r = false;
			EnclosingCommand ec;
			for (int i = parent.Count - 1; i >= 0; i--) {
				ec = parent[i] as EnclosingCommand;
				if (ec == null) {
					continue;
				}
				if (ec.Name.ToString() == "BT") {
					parent.RemoveAt(i);
					r = true;
				}
				else {
					r |= ProcessCommands(ec.Commands);
				}
			}
			return r;
		}

	}
}
