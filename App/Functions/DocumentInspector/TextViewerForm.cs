using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	public partial class TextViewerForm : Form
	{
		///<summary>获取或指定文本内容是否只读。</summary>
		public bool IsTextReadOnly {
			get => _TextBox.ReadOnly;
			set {
				_TextBox.ReadOnly = value;
				_OkButton.Visible = !value;
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

		public TextViewerForm(string textContent, bool isTextReadonly) : this() {
			TextContent = textContent;
			IsTextReadOnly = isTextReadonly;
		}
	}
}
