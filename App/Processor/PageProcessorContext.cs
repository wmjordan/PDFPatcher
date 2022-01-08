using iTextSharp.text.pdf;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	sealed class PageProcessorContext
	{
		/// <summary>正在处理的 PDF 文档。</summary>
		public PdfReader Pdf { get; }

		/// <summary>正在处理的页码。</summary>
		public int PageNumber { get; }

		/// <summary>标记页面内容是否已被更改。</summary>
		public bool IsPageContentModified { get; set; }

		PdfDictionary _Page;
		/// <summary>获取正在处理的页面。</summary>
		public PdfDictionary Page {
			get {
				if (_Page == null) {
					_Page = Pdf.GetPageN(PageNumber);
				}
				return _Page;
			}
		}

		PdfPageCommandProcessor _processor;
		/// <summary>获取正在处理的页面指令集合。</summary>
		public Model.IPdfPageCommandContainer PageCommands {
			get {
				if (_processor == null) {
					_processor = new PdfPageCommandProcessor();
					var resources = Page.Locate<PdfDictionary>(PdfName.RESOURCES);
					_processor.ProcessContent(PdfReader.GetPageContent(Page), resources);
				}
				return _processor;
			}
		}

		public PageProcessorContext(PdfReader pdf, int pageNumber) {
			Pdf = pdf;
			PageNumber = pageNumber;
		}

		/// <summary>写入页面指令到当前处理的页面。</summary>
		internal void WritePageCommands() {
			_processor.WritePdfCommands(Pdf, PageNumber);
		}

		//internal void UpdateContentBytes () {
		//    if (_ContentBytes == null) {
		//        return;
		//    }
		//    Pdf.SafeSetPageContent (PageNumber, _ContentBytes);
		//    Pdf.ResetReleasePage ();
		//}
	}
}
