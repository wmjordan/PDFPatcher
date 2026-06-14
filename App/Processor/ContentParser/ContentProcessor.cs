using System;
using System.Collections.Generic;
using System.Text;
using CLR;
using MuPDF;

namespace PDFPatcher.Processor.ContentParser;

sealed class ContentProcessor : IDisposable
{
	readonly Document _document;
	readonly Stack<GraphicsState> _stateStack = new();
	ResourceStack _resources;
	GraphicsState _state = new();
	StringBuilder _textBuffer;
	bool _disposed;

	public ContentProcessor(Document document) {
		_document = document;
		_state.Ctm = Matrix.Identity;
	}

	public IEnumerable<ContentState> Process(int pageNumber) {
		using var p = _document.LoadPage(pageNumber);
		foreach (var state in Process(p.GetContentBytes(), new ResourceStack(p.Resources))) {
			yield return state;
		}
	}

	public IEnumerable<ContentState> Process(byte[] contentBytes, ResourceStack resourceStack) {
		_resources = resourceStack;
		return Process(ContentStreamParser.Parse(contentBytes));
	}

	IEnumerable<ContentState> Process(IEnumerable<Operation> operations) {
		foreach (var op in operations) {
			switch (op.Info.Kind) {
				// 图形状态
				case RenderCommandKind.GSave:
					_stateStack.Push(_state.Clone());
					break;
				case RenderCommandKind.GRestore:
					_state = _stateStack.Pop();
					break;
				case RenderCommandKind.ConcatMatrix: // cm
					_state.Ctm = MakeMatrix(op.Operands).Concat(_state.Ctm);
					break;

				// 字体与文本状态
				case RenderCommandKind.SetFont: // Tf
					var fontName = op.GetStringOperand(0);
					var fontSize = op.Operands[1].AsFloat();
					var font = _resources.LookupResource(PdfNames.Font, fontName);
					_state.CurrentFont = font is not null
						? FontDescriptor.Load(_document, _resources.Current, font)
						: null;
					_state.FontSize = fontSize;
					break;
				case RenderCommandKind.SetCharSpacing: // Tc
					_state.CharSpacing = op.GetDoubleOperand(0);
					break;
				case RenderCommandKind.SetTextMatrix: // Tm
					_state.TextMatrix = MakeMatrix(op.Operands);
					_state.TextLineMatrix = _state.TextMatrix;
					break;
				case RenderCommandKind.MoveText: // Td
					_state.TextMatrix.TranslateTo(op.Operands[0].AsFloat(), op.Operands[1].AsFloat());
					break;

				// 文本显示
				case RenderCommandKind.ShowText: // Tj
				case RenderCommandKind.NextLineShowText: // '
				case RenderCommandKind.MoveToNextLineAndShowText: // "
				case RenderCommandKind.ShowTextWithSpacing: // TJ
					ProcessText(op);
					break;

				case RenderCommandKind.SetWordSpacing: // Tw
					_state.WordSpacing = op.GetDoubleOperand(0);
					break;
				case RenderCommandKind.SetHorizontalScaling: // Tz
					_state.HorizontalScaling = op.GetDoubleOperand(0);
					break;
				case RenderCommandKind.SetTextLeading: // TL
					_state.TextLeading = op.GetDoubleOperand(0);
					break;
				//case RenderCommandKind.SetTextRenderMode: // Tr
				//	_state.TextRenderMode = (int)(long)op.Operands[0].Value;
				//	break;
				case RenderCommandKind.SetTextRise: // Ts
					_state.TextRise = op.GetDoubleOperand(0);
					break;
				case RenderCommandKind.MoveTextSetLeading: // TD
					var tx = op.GetSingleOperand(0);
					var ty = op.GetSingleOperand(1);
					_state.TextLeading = -ty; // 规范：TD 设置 leading 为 -ty
					_state.TextMatrix.TranslateTo(tx, ty);
					break;
				case RenderCommandKind.NextLine: // T* 移动到下一行起始：相当于 Td 0 -TL
					_state.TextMatrix.TranslateTo(0, (float)-_state.TextLeading);
					break;
				case RenderCommandKind.BeginText: // BT 初始化文本矩阵和文本行矩阵为单位矩阵
					_state.TextMatrix = Matrix.Identity;
					_state.TextLineMatrix = Matrix.Identity;
					break;
				case RenderCommandKind.EndText: // ET 重置文本状态，但通常不影响后续
					break;
				case RenderCommandKind.SetGraphicsState: // gs
					// 从资源中获取 ExtGState 字典并应用参数（如果需要）
					GetExtGState(op.GetStringOperand(0));
					break;
				case RenderCommandKind.PaintXObject: // Do
					// 调用处理 XObject 的方法
					ProcessXObject(op.GetStringOperand(0));
					break;
				//case RenderCommandKind.EndInlineImage: // EI
				//	ProcessInlineImage((InlineImageContent)op.Operands[0].Value);
				//	break;
			}

			yield return new ContentState(_state, op);
		}
	}

	void ProcessXObject(string name) {
		var res = _resources.LookupResource(PdfNames.XObject, name);
	}

	void GetExtGState(string name) {
		var gs = _resources.LookupResource(PdfNames.ExtGState, name);
		if (gs is PdfDictionary dict) {
			var font = dict.GetValue(PdfNames.Font);
			if (font is PdfArray a) {
				_state.CurrentFont = FontDescriptor.Load(_document, _resources.Current, (PdfDictionary)a[0].UnderlyingObject);
				_state.FontSize = a[1].FloatValue;
			}
		}
	}

	void ProcessText(Operation op) {
		if (_state.CurrentFont == null)
			return; // 无字体，无法解码

		string text;
		var sb = _textBuffer == null
			? (_textBuffer = new StringBuilder(256))
			: _textBuffer.Clear();
		if (op.Info.Kind == RenderCommandKind.ShowTextWithSpacing) // TJ
		{
			foreach (var item in (List<Token>)op.Operands[0].Value) {
				if (item.Type.CeqAny(TokenType.String, TokenType.HexString)) {
					_state.CurrentFont.DecodeText(item.Buffer, item.Offset, item.Length, sb);
				}
				// 数字表示字符间距调整，不影响文本内容
			}
			text = sb.ToString();
		}
		else { // Tj
			Token operand = op.Operands[0];
			_state.CurrentFont.DecodeText(operand.Buffer, operand.Offset, operand.Length, sb);
			text = sb.ToString();
		}
		_state.Text = text;

		// 输出或记录文本（可在此处结合当前变换矩阵计算实际位置）
		Console.WriteLine($"Text {text}");
	}

	static Matrix MakeMatrix(Token[] tokens) {
		if (tokens.Length != 6) {
			throw new ArgumentException("构建矩阵的参数不是 6 个");
		}
		return new Matrix(tokens[0].AsFloat(), tokens[1].AsFloat(), tokens[2].AsFloat(), tokens[3].AsFloat(), tokens[4].AsFloat(), tokens[5].AsFloat());
	}

	private void Dispose(bool disposing) {
		if (!_disposed) {
			_disposed = true;
		}
	}

	~ContentProcessor() {
		Dispose(disposing: false);
	}

	public void Dispose() {
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

sealed class ContentState(GraphicsState graphicsState, Operation operation) : EventArgs
{
	public GraphicsState GraphicsState { get; } = graphicsState;
	public Operation Operation { get; } = operation;

	public string FontName => GraphicsState.CurrentFontName;
	public double FontSize => GraphicsState.FontSize;
	public Matrix TextMatrix => GraphicsState.TextMatrix;
	public Matrix TextLineMatrix => GraphicsState.TextLineMatrix;
	public Matrix Ctm => GraphicsState.Ctm;
	public string Text => GraphicsState.Text;

	public RenderCommandKind CommandKind => Operation.Kind;
	public string Operator => Operation.Info.Name;
}