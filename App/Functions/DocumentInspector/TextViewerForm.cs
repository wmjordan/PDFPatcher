using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CLR;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	sealed partial class TextViewerForm : Form
	{
		static readonly Regex __EscapeChars = new Regex("[\u0000-\u001F\u0080-\u00FF]", RegexOptions.Compiled);
		const int ContentStreamIndentCount = 2;
		readonly byte[] _Data;

		///<summary>获取或指定文本内容是否只读。</summary>
		public bool IsTextReadOnly {
			get => _TextBox.ReadOnly;
			set {
				_TextBox.ReadOnly = value;
				_OkButton.Visible = !value;
				_CancelButton.Text = value ? "关闭(&G)" : "取消(&X)";
			}
		}

		///<summary>获取或指定文本内容。</summary>
		public string TextContent {
			get => _TextBox.Text;
			set => _TextBox.Text = value;
		}

		public TextViewerForm() {
			InitializeComponent();
		}

		public TextViewerForm(byte[] data, bool isTextReadonly, bool isContentStream = false) : this() {
			_Data = data;
			_EncodingBox.SelectedIndex = 0;
			_ReformatButton.Visible = isContentStream;
			IsTextReadOnly = isTextReadonly;
			MinimumSize = Size;
		}

		static string EscapeChar(char c) {
			var t = Convert.ToString(c, 8);
			switch (t.Length) {
				case 1: return "\\00" + t;
				case 2: return "\\0" + t;
				default: return "\\" + t;
			}
		}

		void _EncodingBox_SelectedIndexChanged(object sender, EventArgs e) {
			if (!_Data.HasContent()) {
				_TextBox.Clear();
				return;
			}
			Encoding en;
			switch (_EncodingBox.SelectedIndex) {
				case 0: en = Encoding.GetEncoding(936); break;
				case 1: en = Encoding.GetEncoding(1252); break;
				case 2: en = Encoding.UTF8; break;
				default: TextContent = ShowHexBin(); return;
			}
			TextContent = __EscapeChars.Replace(en.GetString(_Data), m => {
				var c = m.Value[0];
				return c.CeqAny('\t', '\r', '\n') ? m.Value : EscapeChar(c);
			});
		}

		string ShowHexBin() {
			return _Data.ToHexBinString(true, ' ', 0, _Data.Length);
		}

		void _ReformatButton_Click(object sender, EventArgs e) {
			if (!_Data.HasContent()) {
				return;
			}
			_TextBox.Clear();
			_TextBox.BackColor = Color.FloralWhite;
			if (_Data.Length < 64 << 10) {
				ShowRichParseResultOfContentStream();
			}
			else {
				ShowReformattedContentStream();
			}
			_ReformatButton.Enabled = false;
		}

		void ShowReformattedContentStream() {
			var sb = new StringBuilder(32);
			int indent = 0;
			foreach (var op in new Processor.ContentParser.ContentStreamParser().Parse(_Data)) {
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
			_TextBox.Text = sb.ToString();
		}

		void ShowRichParseResultOfContentStream() {
			using var tb = _TextBox.BatchUpdate();
			var sb = new StringBuilder(32);
			int indent = 0;
			foreach (var op in new Processor.ContentParser.ContentStreamParser().Parse(_Data)) {
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
					_TextBox.AppendText(sb.ToString());
					sb.Clear();
				}
				if (oi.IsBeginScope) {
					indent += ContentStreamIndentCount;
				}
				_TextBox.SelectionColor = Color.Blue;
				_TextBox.AppendLine(op.Operator);
			}
		}
	}
}
