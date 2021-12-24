using System;
using System.Collections.Generic;
using System.Text;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	interface IDocProcessor : IProcessor
	{
		/// <summary>
		/// 估算工作量。
		/// </summary>
		/// <param name="pdf">需要处理的文档。</param>
		/// <returns>整数工作量（用于显示进度条）。</returns>
		int EstimateWorkload (iTextSharp.text.pdf.PdfReader pdf);

		/// <summary>
		/// 在处理页面前调用，初始化处理器。
		/// </summary>
		/// <param name="context">包含传入文档的 <see cref="DocProcessorContext"/></param>
		void BeginProcess (DocProcessorContext context);

		/// <summary>
		/// 处理传入的文档。
		/// </summary>
		/// <param name="context">包含传入文档的 <see cref="DocProcessorContext"/></param>
		/// <returns>更改文档内容后返回 true。</returns>
		bool Process (DocProcessorContext context);

		/// <summary>
		/// 在处理页面后调用。
		/// </summary>
		/// <param name="context">包含传入文档的 <see cref="DocProcessorContext"/></param>
		void EndProcess (DocProcessorContext context);

	}
}
