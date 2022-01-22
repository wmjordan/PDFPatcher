using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PDFPatcher.Common;
using PDFPatcher.Model;
using GraphicsState = PDFPatcher.Model.GraphicsState;

namespace PDFPatcher.Processor;

internal sealed class PdfPageCommandProcessor : PdfContentStreamProcessor, IPdfPageCommandContainer
{
	private readonly Stack<EnclosingCommand> _commandStack;
	private EnclosingCommand _currentCommand;
	private float _textWidth;

	public PdfPageCommandProcessor() {
		PopulateOperators();
		RegisterContentOperator("TJ", new AccumulatedShowTextArray());
		Commands = new List<PdfPageCommand>();
		_commandStack = new Stack<EnclosingCommand>();
	}

	public PdfPageCommandProcessor(PRStream form)
		: this() {
		PdfDictionary resources = form.Locate<PdfDictionary>(PdfName.RESOURCES);
		ProcessContent(PdfReader.GetStreamBytes(form), resources);
	}

	public bool HasCommand => Commands.Count > 0;

	/// <summary>
	///     分析内容后得到的 PDF 命令操作符及操作数列表。
	/// </summary>
	public IList<PdfPageCommand> Commands { get; }

	protected override void DisplayPdfString(PdfString str) {
		GraphicsState gs = CurrentGraphicState;
		FontInfo font = gs.Font;
		if (font == null) {
			return;
		}

		float totalWidth = (from c in font.DecodeText(str) let w = font.GetWidth(c) / 1000.0f let wordSpacing = c == ' ' ? gs.WordSpacing : 0f select ((w * gs.FontSize) + gs.CharacterSpacing + wordSpacing) * gs.HorizontalScaling).Sum();

		_textWidth = totalWidth;
	}

	protected override void InvokeOperator(PdfLiteral oper, List<PdfObject> operands) {
		base.InvokeOperator(oper, operands);
		PdfPageCommand cmd;
		string o = oper.ToString();
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
				cmd = EnclosingCommand.IsStartingCommand(o)
					? new EnclosingCommand(oper, operands)
					: new PdfPageCommand(oper, operands);
				break;
		}

		if (EnclosingCommand.IsEndingCommand(o)) {
			_commandStack.Pop();
			_currentCommand = _commandStack.Count > 0 ? _commandStack.Peek() : null;
			return;
		}

		if (_currentCommand != null) {
			_currentCommand.Commands.Add(cmd);
		}
		else {
			Commands.Add(cmd);
		}

		if (cmd is not EnclosingCommand ec) {
			return;
		}

		_commandStack.Push(ec);
		_currentCommand = ec;
	}

	/// <summary>
	///     将 <see cref="Operands" /> 的内容写入到目标 <see cref="System.IO.Stream" />。
	/// </summary>
	/// <param name="target">目标流对象。</param>
	internal void WritePdfCommands(Stream target) {
		foreach (PdfPageCommand item in Commands) {
			item.WriteToPdf(target);
		}
	}

	/// <summary>
	///     将 <see cref="Operands" /> 的内容写入到目标 <paramref name="pdf" /> 的第 <paramref name="pageNumber" /> 页。
	/// </summary>
	/// <param name="pdf">目标 <see cref="PdfReader" />。</param>
	/// <param name="pageNumber">要写入的页码。</param>
	internal void WritePdfCommands(PdfReader pdf, int pageNumber) {
		using MemoryStream ms = new();
		WritePdfCommands(ms);
		ms.Flush();
		pdf.SafeSetPageContent(pageNumber, ms.ToArray());
	}

	internal void WritePdfCommands(PageProcessorContext context) {
		WritePdfCommands(context.Pdf, context.PageNumber);
	}

	private static string GetOperandsTextValue(List<PdfObject> operands) {
		List<string> n =
			operands.ConvertAll(po => po.Type == PdfObject.NUMBER ? ((PdfNumber)po).DoubleValue.ToText() : null);
		n.RemoveAt(n.Count - 1);
		return string.Join(" ", n.ToArray());
	}

	private TextInfo GetTextInfo(PdfString text) {
		GraphicsState gs = CurrentGraphicState;
		Matrix m = TextMatrix;
		return new TextInfo {
			PdfString = text,
			Text = gs.Font != null ? gs.Font.DecodeText(text) : string.Empty,
			Size = gs.FontSize * m[Matrix.I11],
			Region = new Bound(m[Matrix.I31], m[Matrix.I32], m[Matrix.I31] + _textWidth, 0),
			Font = gs.Font
		};
	}
}