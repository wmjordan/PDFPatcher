using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;

namespace PDFPatcher.Processor;

internal interface IPageProcessor : IProcessor
{
	/// <summary>
	/// 估算工作量。
	/// </summary>
	/// <param name="pdf">需要处理的文档。</param>
	/// <returns>整数工作量（用于显示进度条）。</returns>
	int EstimateWorkload(PdfReader pdf);

	/// <summary>
	/// 在处理页面前调用，初始化处理器。
	/// </summary>
	/// <param name="context">包含传入文档的 <see cref="DocProcessorContext"/></param>
	/// <returns>更改文档内容后返回 true。</returns>
	void BeginProcess(DocProcessorContext context);

	/// <summary>
	/// 处理传入的页面。
	/// </summary>
	/// <param name="context">包含传入页面的 <see cref="PageProcessorContext"/></param>
	/// <returns>更改页面内容后返回 true。</returns>
	bool Process(PageProcessorContext context);

	/// <summary>
	/// 完成处理文档的操作，在完成处理所有页面后被调用。
	/// </summary>
	/// <param name="pdf">需要处理的文档。</param>
	/// <returns>更改文档内容后返回 true。</returns>
	bool EndProcess(PdfReader pdf);
}