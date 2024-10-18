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
		readonly Stack<EnclosingCommand> _commandStack = new Stack<EnclosingCommand>();
		EnclosingCommand _currentCommand;
		float _textWidth;

		public bool HasCommand => Commands.Count > 0;
		/// <summary>
		/// 分析内容后得到的 PDF 命令操作符及操作数列表。
		/// </summary>
		public IList<PdfPageCommand> Commands { get; } = new List<PdfPageCommand>();
		public string LastError { get; private set; }

		public PdfPageCommandProcessor() {
			PopulateOperators();
			RegisterContentOperator("TJ", new AccumulatedShowTextArray());
		}

		public PdfPageCommandProcessor(PRStream form)
			: this() {
			ProcessContent(PdfReader.GetStreamBytes(form), form.Locate<PdfDictionary>(PdfName.RESOURCES));
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
			PdfPageCommand cmd;
			try {
				base.InvokeOperator(oper, operands);
			}
			catch (Exception ex) {
				cmd = new InvalidCommand(oper, operands) {
					Error = LastError = oper + " " + (ex is IndexOutOfRangeException ? "指令参数不足"
						: ex is ArgumentOutOfRangeException ? "指令参数不足"
						: ex is InvalidCastException ? "指令参数类型不匹配"
						: ex.Message)
				};
				goto ADD_COMMAND;
			}

			switch (oper.ToString()) {
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
				case "Do":
				case "S":
				case "s":
				case "F":
				case "f":
				case "f*":
				case "B":
				case "B*":
				case "b":
				case "b*":
				case "n":
					cmd = new OutputCommand(oper, operands);
					break;
				case "q":
				case "BT":
				case "BMC":
				case "BDC":
				case "BX":
					cmd = new EnclosingCommand(oper, operands);
					break;
				case "Q":
				case "ET":
				case "EMC":
				case "EX":
					// 兼容结构异常的 PDF 文档（github: #121）
					if (_commandStack.Count > 0) {
						_commandStack.Pop();
						_currentCommand = _commandStack.Count > 0 ? _commandStack.Peek() : null;
					}
					else {
						_currentCommand = null;
						cmd = new InvalidCommand(oper, operands) { Error = "嵌套指令不配对" };
						goto ADD_COMMAND;
					}
					return;
				case "BI":
					cmd = new InlineImageCommand(oper, operands);
					break;
				default:
					cmd = new AdjustCommand(oper, operands);
					break;
			}
			ADD_COMMAND:
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
