using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions;

sealed class ContentStreamViewer
{
	const int ContentStreamIndentCount = 2;

	public static void SetContent(RichTextBox textBox, byte[] contentBytes) {
		if (contentBytes.Length < 64 << 10) {
			ShowRichParseResultOfContentStream(textBox, contentBytes);
		}
		else {
			ShowReformattedContentStream(textBox, contentBytes);
		}
	}


	static void ShowReformattedContentStream(RichTextBox textBox, byte[] contentBytes) {
		var sb = new StringBuilder(32);
		int indent = 0;
		foreach (var op in new Processor.ContentParser.ContentStreamParser().Parse(contentBytes)) {
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
			sb.AppendLine(op.Operator);
		}
		textBox.Text = sb.ToString();
	}

	static void ShowRichParseResultOfContentStream(RichTextBox textBox, byte[] contentBytes) {
		using var tb = textBox.BatchUpdate();
		var sb = new StringBuilder(32);
		int indent = 0;
		foreach (var op in new Processor.ContentParser.ContentStreamParser().Parse(contentBytes)) {
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
			textBox.AppendLine(op.Operator);
		}
	}

}
