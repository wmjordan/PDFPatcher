using iTextSharp.text.pdf;

namespace PDFPatcher.Processor
{
	sealed class RemoveAnnotationProcessor : IPageProcessor
	{
		readonly PdfName _AnnotationType;
		int _processedPageCount;

		public RemoveAnnotationProcessor(PdfName annotationType) {
			_AnnotationType = annotationType;
		}
		#region IPageProcessor 成员
		public string Name => "删除批注";
		public void BeginProcess(DocProcessorContext context) {
			_processedPageCount = 0;
		}
		public bool EndProcess(PdfReader pdf) {
			Tracker.TraceMessage(Tracker.Category.Notice, Name + "功能：");
			Tracker.TraceMessage("　　删除了 " + _processedPageCount + " 页的批注。");
			return false;
		}
		public int EstimateWorkload(PdfReader pdf) {
			return pdf.NumberOfPages;
		}

		public bool Process(PageProcessorContext context) {
			Tracker.IncrementProgress(1);
			var anns = context.Page.GetAsArray(PdfName.ANNOTS);
			if (anns == null) {
				return false;
			}
			if (_AnnotationType == null) {
				context.Page.Remove(PdfName.ANNOTS);
				return true;
			}
			bool removed = false;
			var l = anns.Size;
			for (int i = l - 1; i >= 0; i--) {
				var ann = PdfReader.GetPdfObject(anns[i]) as PdfDictionary;
				if (ann == null) {
					continue;
				}
				if (_AnnotationType.Equals(ann.GetAsName(PdfName.SUBTYPE)) == false) {
					continue;
				}
				anns.Remove(i);
				removed = true;
			}
			if (anns.Size == 0) {
				context.Page.Remove(PdfName.ANNOTS);
			}
			if (removed) {
				context.IsPageContentModified = true;
				_processedPageCount++;
			}
			return removed;
		}

		#endregion
	}
}
