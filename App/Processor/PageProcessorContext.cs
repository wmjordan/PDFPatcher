using iTextSharp.text.pdf;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

internal sealed class PageProcessorContext
{
	private PdfDictionary _Page;

	private PdfPageCommandProcessor _processor;

	public PageProcessorContext(PdfReader pdf, int pageNumber) {
		Pdf = pdf;
		PageNumber = pageNumber;
	}

	/// <summary>正在处理的 PDF 文档。</summary>
	public PdfReader Pdf { get; }

	/// <summary>正在处理的页码。</summary>
	public int PageNumber { get; }

	/// <summary>标记页面内容是否已被更改。</summary>
	public bool IsPageContentModified { get; set; }

	/// <summary>获取正在处理的页面。</summary>
	public PdfDictionary Page => _Page ??= Pdf.GetPageN(PageNumber);

	/// <summary>获取正在处理的页面指令集合。</summary>
	public IPdfPageCommandContainer PageCommands {
		get {
			if (_processor != null) {
				return _processor;
			}

			_processor = new PdfPageCommandProcessor();
			PdfDictionary resources = Page.Locate<PdfDictionary>(PdfName.RESOURCES);
			_processor.ProcessContent(PdfReader.GetPageContent(Page), resources);

			return _processor;
		}
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