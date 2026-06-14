using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CLR;
using PDFPatcher.Common;
using PDFPatcher.Processor.ContentParser;

namespace PDFPatcher.Functions;

static class ContentStreamViewer
{
	const int ContentStreamIndentCount = 2;

	public static void SetContent(RichTextBox textBox, byte[] contentBytes) {
		var results = ContentStreamParser.Parse(contentBytes).Select(i => new ContentState(null, i));
		if (contentBytes.Length < 64 << 10) {
			ShowRichParseResultOfContentStream(textBox, results);
		}
		else {
			ShowReformattedContentStream(textBox, results);
		}
	}

	public static void SetContent(RichTextBox textBox, ContentProcessor processor, int pageNumber) {
		ShowRichParseResultOfContentStream(textBox, processor.Process(pageNumber - 1));
	}

	static void ShowReformattedContentStream(RichTextBox textBox, IEnumerable<ContentState> parseStates) {
		var sb = new StringBuilder(32);
		int indent = 0;
		foreach (var state in parseStates) {
			var op = state.Operation;
			var operands = op.Operands;
			var oi = op.Info;
			if (oi.IsEndScope) {
				indent -= ContentStreamIndentCount;
			}
			if (indent > 0) {
				sb.Append(' ', indent);
			}
			if (operands.Length != 0) {
				for (int i = 0; i < operands.Length; i++) {
					if (i != 0) {
						sb.Append(' ');
					}
					sb.Append(operands[i].ToString());
				}
				sb.Append(' ');
			}
			if (oi.IsBeginScope) {
				indent += ContentStreamIndentCount;
			}

			if (state.GraphicsState != null
				&& op.Kind.CeqAny(RenderCommandKind.ShowText, RenderCommandKind.ShowTextWithSpacing, RenderCommandKind.NextLineShowText, RenderCommandKind.MoveToNextLineAndShowText)) {
				sb.Append(op.Operator)
					.Append(" %")
					.AppendLine(state.Text.Replace("\n", "\\n"));
			}
			else {
				sb.AppendLine(op.Operator);
			}
		}
		textBox.Text = sb.ToString();
	}

	static void ShowRichParseResultOfContentStream(RichTextBox textBox, IEnumerable<ContentState> parseStates) {
		using var tb = textBox.BatchUpdate();
		var sb = new StringBuilder(32);
		int indent = 0;
		foreach (var state in parseStates) {
			var op = state.Operation;
			var operands = op.Operands;
			var oi = op.Info;
			if (oi.IsEndScope) {
				indent -= ContentStreamIndentCount;
			}
			if (indent > 0) {
				sb.Append(' ', indent);
			}
			if (operands.Length != 0) {
				for (int i = 0; i < operands.Length; i++) {
					if (i != 0) {
						sb.Append(' ');
					}
					sb.Append(operands[i].ToString());
				}
				sb.Append(' ');
			}
			if (sb.Length != 0) {
				textBox.AppendText(sb.ToString());
				sb.Clear();
			}
			if (oi.IsBeginScope) {
				indent += ContentStreamIndentCount;
			}
			textBox.SelectionColor = Color.Blue;

			if (state.GraphicsState != null) {
				switch (op.Kind) {
					case RenderCommandKind.ShowText:
					case RenderCommandKind.MoveToNextLineAndShowText:
					case RenderCommandKind.NextLineShowText:
					case RenderCommandKind.ShowTextWithSpacing:
						ShowComment(textBox, op, state.Text.Replace("\n", "\\n"));
						break;
					case RenderCommandKind.SetFont:
						ShowComment(textBox, op, state.GraphicsState.CurrentFontName);
						break;
				}
			}
			textBox.AppendLine(op.Operator);
		}
	}

	static void ShowComment(RichTextBox textBox, Operation op, string comment) {
		textBox.AppendText(op.Operator);
		textBox.SelectionColor = Color.Green;
		textBox.AppendLine(" %" + comment);
	}
}
