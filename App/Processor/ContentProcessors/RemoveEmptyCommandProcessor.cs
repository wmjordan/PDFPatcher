using System.Collections.Generic;
using System.IO;
using iTextSharp.text.pdf;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

//todo: 删除无直接输出效果的指令
//todo: 删除空白页
internal sealed class RemoveEmptyCommandProcessor : IPageProcessor
{
	private int _processedPageCount;

	private static bool ProcessCommands(IList<PdfPageCommand> parent) {
		bool r = false;
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

	#region IPageProcessor 成员

	public string Name => "删除冗余指令";

	public void BeginProcess(DocProcessorContext context) {
		_processedPageCount = 0;
	}

	public bool EndProcess(PdfReader pdf) {
		Tracker.TraceMessage(Tracker.Category.Notice, Name + "功能：");
		Tracker.TraceMessage("　　删除了 " + _processedPageCount + " 页的冗余指令。");
		return false;
	}

	public int EstimateWorkload(PdfReader pdf) {
		return pdf.NumberOfPages * 3;
	}

	public bool Process(PageProcessorContext context) {
		Tracker.IncrementProgress(3);
		IPdfPageCommandContainer p = context.PageCommands;
		bool r = false;
		r = ProcessCommands(p.Commands);
		if (r) {
			context.IsPageContentModified = true;
			_processedPageCount++;
		}

		ProcessFormContent(context);
		return r;
	}

	private static void ProcessFormContent(PageProcessorContext context) {
		PdfDictionary fl = context.Page.Locate<PdfDictionary>(PdfName.RESOURCES, PdfName.XOBJECT);
		if (fl == null) {
			return;
		}

		foreach (KeyValuePair<PdfName, PdfObject> item in fl) {
			PRStream f = PdfReader.GetPdfObject(item.Value) as PRStream;
			if (f == null
				|| PdfName.FORM.Equals(f.GetAsName(PdfName.SUBTYPE)) == false) {
				continue;
			}

			PdfPageCommandProcessor p = new(f);
			if (ProcessCommands(p.Commands)) {
				using (MemoryStream ms = new()) {
					p.WritePdfCommands(ms);
					ms.Flush();
					f.SetData(ms.ToArray(), ms.Length > 32);
				}
			}
		}
	}

	#endregion
}