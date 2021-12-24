using System;
using System.Windows.Forms;

namespace PDFPatcher
{
	public partial class PasswordEntryForm : Form
	{
		public PasswordEntryForm (string sourceFile) {
			InitializeComponent ();

			sourceFile = System.IO.Path.GetFileName (sourceFile);
			this.Text += "：" + sourceFile;
			this._MessageLabel.Text = this._MessageLabel.Text.Replace ("PDF 文件", String.Concat ("PDF 文件 ", sourceFile, " "));
		}

		/// <summary>
		/// 获取密码框的文本。
		/// </summary>
		public string Password {
			get { return _PasswordBox.Text; }
		}

		private void _OkButton_Click (object sender, EventArgs e) {
			this.DialogResult = DialogResult.OK;
			this.Close ();
		}

		private void _CancelButton_Click (object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
			this.Close ();
		}
	}
}
