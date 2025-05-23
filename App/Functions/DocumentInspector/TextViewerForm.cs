using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	sealed partial class TextViewerForm : Form
	{
		static readonly Regex __EscapeChars = new Regex("[\u0000-\u001F\u0080-\u00FF]", RegexOptions.Compiled);
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

		public TextViewerForm(byte[] data, bool isTextReadonly) : this() {
			_Data = data;
			_EncodingBox.SelectedIndex = 0;
			IsTextReadOnly = isTextReadonly;
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
			if (_Data.HasContent() == false) {
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
				return (c != '\t' && c != '\r' && c != '\n') ? EscapeChar(c) : m.Value;
			});
		}

		string ShowHexBin() {
			return _Data.ToHexBinString(true, ' ', 0, _Data.Length);
		}
	}
}
