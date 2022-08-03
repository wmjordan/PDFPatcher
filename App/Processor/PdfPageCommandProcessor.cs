using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;
using Matrix = iTextSharp.text.pdf.parser.Matrix;

namespace PDFPatcher.Processor
{
	sealed class PdfPageCommandProcessor : PdfContentStreamProcessor, IPdfPageCommandContainer
	{

		public bool HasCommand => Commands.Count > 0;
		/// <summary>
		/// 分析内容后得到的 PDF 命令操作符及操作数列表。
		/// </summary>
		public IList<PdfPageCommand> Commands { get; }
		readonly Stack<EnclosingCommand> _commandStack;
		EnclosingCommand _currentCommand;
		float _textWidth;

		public PdfPageCommandProcessor() {
			PopulateOperators();
			RegisterContentOperator("TJ", new AccumulatedShowTextArray());
			Commands = new List<PdfPageCommand>();
			_commandStack = new Stack<EnclosingCommand>();
		}

		public PdfPageCommandProcessor(PRStream form)
			: this() {
			var resources = form.Locate<PdfDictionary>(PdfName.RESOURCES);
			ProcessContent(PdfReader.GetStreamBytes(form), resources);
		}

		protected override void DisplayPdfString(PdfString str) {
			var gs = CurrentGraphicState;
			var font = gs.Font;
			if (font == null) {
				return;
			}
			float totalWidth = 0;
			foreach (var c in font.DecodeText(str)) {
				var w = font.GetWidth(c) / 1000.0f;
				var wordSpacing = (c == ' ' ? gs.WordSpacing : 0f);
				totalWidth += (w * gs.FontSize + gs.CharacterSpacing + wordSpacing) * gs.HorizontalScaling;
			}

			_textWidth = totalWidth;
		}

		protected override void InvokeOperator(PdfLiteral oper, List<PdfObject> operands) {
			base.InvokeOperator(oper, operands);
			PdfPageCommand cmd;
			var o = oper.ToString();
			switch (o) {
				case "TJ":
					cmd = new PaceAndTextCommand(oper, operands, GetTextInfo(new PdfString()), CurrentGraphicState.Font);
					break;
				case "Tj":
				case "'":
				case "\"":
					cmd = new TextCommand(oper, operands, GetTextInfo(operands[0] as PdfString));
					break;
				case "Tf":
					cmd = new FontCommand(oper, operands, CurrentGraphicState.Font.FontName);
					break;
				case "cm":
				case "Tm":
					cmd = new MatrixCommand(oper, operands);
					break;
				case "BI":
					cmd = new InlineImageCommand(oper, operands);
					break;
				default:
					cmd = EnclosingCommand.IsStartingCommand(o) ? new EnclosingCommand(oper, operands) : new PdfPageCommand(oper, operands);
					break;
			}

			if (EnclosingCommand.IsEndingCommand(o)) {
				// 兼容结构异常的 PDF 文档（github: #121）
				if (_commandStack.Count > 0) {
					_commandStack.Pop();
					_currentCommand = _commandStack.Count > 0 ? _commandStack.Peek() : null;
				}
				else {
					_currentCommand = null;
				}
				return;
			}
			if (_currentCommand != null) {
				_currentCommand.Commands.Add(cmd);
			}
			else {
				Commands.Add(cmd);
			}
			if (cmd is EnclosingCommand ec) {
				_commandStack.Push(ec);
				_currentCommand = ec;
			}
		}

		/// <summary>
		/// 将 <see cref="Operands"/> 的内容写入到目标 <see cref="System.IO.Stream"/>。
		/// </summary>
		/// <param name="target">目标流对象。</param>
		internal void WritePdfCommands(System.IO.Stream target) {
			foreach (var item in Commands) {
				item.WriteToPdf(target);
			}
		}

		/// <summary>
		/// 将 <see cref="Operands"/> 的内容写入到目标 <paramref name="pdf"/> 的第 <paramref name="pageNumber"/> 页。
		/// </summary>
		/// <param name="pdf">目标 <see cref="PdfReader"/>。</param>
		/// <param name="pageNumber">要写入的页码。</param>
		internal void WritePdfCommands(PdfReader pdf, int pageNumber) {
			using (var ms = new System.IO.MemoryStream()) {
				WritePdfCommands(ms);
				ms.Flush();
				pdf.SafeSetPageContent(pageNumber, ms.ToArray());
			}
		}

		internal void WritePdfCommands(PageProcessorContext context) {
			WritePdfCommands(context.Pdf, context.PageNumber);
		}

		private static string GetOperandsTextValue(List<PdfObject> operands) {
			var n = operands.ConvertAll((po) => po.Type == PdfObject.NUMBER ? ValueHelper.ToText(((PdfNumber)po).DoubleValue) : null);
			n.RemoveAt(n.Count - 1);
			return String.Join(" ", n.ToArray());
		}

		private TextInfo GetTextInfo(PdfString text) {
			var gs = CurrentGraphicState;
			var m = TextMatrix;
			return new TextInfo {
				PdfString = text,
				Text = gs.Font != null ? gs.Font.DecodeText(text) : String.Empty,
				Size = gs.FontSize * m[Matrix.I11],
				Region = new Bound(m[Matrix.I31], m[Matrix.I32], m[Matrix.I31] + _textWidth, 0),
				Font = gs.Font
			};
		}
	}
}
