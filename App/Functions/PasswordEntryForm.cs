using System;
using System.Windows.Forms;

namespace PDFPatcher
{
	public partial class PasswordEntryForm : Form
	{
		public PasswordEntryForm(string sourceFile) {
			InitializeComponent();

			sourceFile = System.IO.Path.GetFileName(sourceFile);
			Text += "：" + sourceFile;
			_MessageLabel.Text = _MessageLabel.Text.Replace("PDF 文件", String.Concat("PDF 文件 ", sourceFile, " "));
		}

		/// <summary>
		/// 获取密码框的文本。
		/// </summary>
		public string Password => _PasswordBox.Text;

		private void _OkButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}

		private void _CancelButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
