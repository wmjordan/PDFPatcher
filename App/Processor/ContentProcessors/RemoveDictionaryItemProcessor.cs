using iTextSharp.text.pdf;

namespace PDFPatcher.Processor;

/// <summary>
///     删除指定字典名称项目的处理器。
/// </summary>
internal sealed class RemoveDictionaryItemProcessor : IPageProcessor
{
	private readonly PdfName _ItemName;

	public RemoveDictionaryItemProcessor(PdfName itemName) {
		_ItemName = itemName;
	}

	#region IDocProcessor 成员

	public bool Process(DocProcessorContext context) {
		if (context.Pdf.Catalog.Contains(_ItemName)) {
			context.Pdf.Catalog.Remove(_ItemName);
			return true;
		}

		return false;
	}

	#endregion

	#region IPageProcessor 成员

	public string Name => "删除字典项目";

	public void BeginProcess(DocProcessorContext context) {
	}

	public bool EndProcess(PdfReader pdf) {
		return false;
	}

	public int EstimateWorkload(PdfReader pdf) {
		return 0;
	}

	public bool Process(PageProcessorContext context) {
		if (context.Page.Contains(_ItemName)) {
			context.Page.Remove(_ItemName);
			return true;
		}

		return false;
	}

	#endregion
}