using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	sealed class RemoveFormProcessor : IPageProcessor
	{
		int _processedPageCount;

		#region IPageProcessor 成员
		public string Name { get { return "删除表单区域"; } }

		public void BeginProcess (DocProcessorContext context) {
			_processedPageCount = 0;
		}
		public bool EndProcess (PdfReader pdf) {
			Tracker.TraceMessage (Tracker.Category.Notice, this.Name + "功能：");
			Tracker.TraceMessage ("　　删除了 " + _processedPageCount + " 页的表单区域。");
			return false;
		}

		public int EstimateWorkload (PdfReader pdf) {
			return pdf.NumberOfPages * 3;
		}

		public bool Process (PageProcessorContext context) {
			Tracker.IncrementProgress (3);
			var p = context.PageCommands;
			var r = false;
			var fl = ProcessFormContent (context);
			if (fl.HasContent ()) {
				r = true;
				ProcessCommands (p.Commands, fl);
			}
			if (r) {
				context.IsPageContentModified = true;
				_processedPageCount++;
			}
			return r;
		}

		private static HashSet<PdfName> ProcessFormContent (PageProcessorContext context) {
			var fl = context.Page.Locate<PdfDictionary> (PdfName.RESOURCES, PdfName.XOBJECT);
			if (fl == null) {
				return null;
			}
			var r = new HashSet<PdfName> ();
			foreach (var item in fl) {
				var f = PdfReader.GetPdfObject (item.Value) as PRStream;
				if (f == null
					|| PdfName.FORM.Equals (f.GetAsName (PdfName.SUBTYPE)) == false) {
					continue;
				}
				r.Add (item.Key);
			}
			foreach (var item in r) {
				fl.Remove (item);
			}
			if (fl.Size == 0) {
				context.Page.Locate<PdfDictionary> (PdfName.RESOURCES).Remove (PdfName.XOBJECT);
			}
			return r;
		}

		#endregion

		private bool ProcessCommands (IList<Model.PdfPageCommand> parent, HashSet<PdfName> formNames) {
			var r = false;
			for (int i = parent.Count - 1; i >= 0; i--) {
				var cmd = parent[i];
				var ec = cmd as Model.EnclosingCommand;
				if (ec != null) {
					r |= ProcessCommands (ec.Commands, formNames);
				}
				if (cmd.Name.ToString () == "Do" && cmd.HasOperand && formNames.Contains (cmd.Operands[0] as PdfName)) {
					parent.RemoveAt (i);
					r = true;
				}
			}
			return r;
		}

	}
}
