using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	internal sealed class OperatorGroup : PdfContentStreamProcessor.IContentOperator
	{
		/// <summary>
		/// 处理内容的 <see cref="PdfContentStreamProcessor.IContentOperator"/> 列表。
		/// </summary>
		public List<PdfContentStreamProcessor.IContentOperator> Operators { get; private set; }

		public OperatorGroup (IEnumerable<PdfContentStreamProcessor.IContentOperator> operators) {
			this.Operators = new List<PdfContentStreamProcessor.IContentOperator> ();
			if (operators != null) {
				this.Operators.AddRange (operators);
			}
		}

		#region IContentOperator 成员

		public void Invoke (PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			foreach (var item in this.Operators) {
				item.Invoke (processor, oper, operands);
			}
		}

		#endregion

	}
}
