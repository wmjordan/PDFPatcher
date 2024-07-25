using System;
using System.ComponentModel;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher
{
	public partial class SourceFileControl : UserControl
	{
		bool _controlLockDown;
		public SourceFileControl() {
			_controlLockDown = true;
			InitializeComponent();
			_controlLockDown = false;
			Files = new string[] { };
		}

		///<summary>获取文件下拉列表框。</summary>
		internal HistoryComboBox FileList => _SourcePdfBox;

		///<summary>点击浏览按钮更改选中文件后触发的事件。</summary>
		public event EventHandler BrowseSelectedFiles;

		/// <summary>
		/// 获取选定的 PDF 文件列表。
		/// </summary>
		internal string[] Files { get; set; }
		/// <summary>
		/// 获取选定的 PDF 文件列表的第一项。
		/// </summary>
		internal string FirstFile => Files != null && Files.Length > 0 ? Files[0] : String.Empty;

		[DefaultValue(null)]
		public override string Text {
			get => _SourcePdfBox.Text;
			set => _SourcePdfBox.Text = value;
		}

		public void BrowseFile() {
			_BrowseSourcePdfButton.PerformClick();
		}

		void _BrowseSourcePdfButton_Click(object sender, EventArgs e) {
			var t = _SourcePdfBox.Text;
			if (t.Length > 0
				&& FileHelper.IsPathValid(t)
				&& System.IO.Path.GetFileName(t).Length > 0) {
				_OpenPdfBox.FileName = t;
			}
			if (_OpenPdfBox.ShowDialog() == DialogResult.OK) {
				SelectFiles(_OpenPdfBox.FileNames);
				if (BrowseSelectedFiles != null) {
					BrowseSelectedFiles(sender, e);
				}
			}
		}

		void SelectFiles(string[] files) {
			var t = _SourcePdfBox.Text;
			if (files.Length > 1) {
				Text = String.Concat("<选定了 ", files.Length, " 个文件>", System.IO.Path.GetDirectoryName(files[0]));
			}
			else if (files[0] != t) {
				Text = files[0];
			}
			Files = files;
		}

		void _SourcePdfBox_TextChanged(object sender, EventArgs e) {
			if (_controlLockDown) {
				return;
			}
			if (FileHelper.HasFileNameMacro(_SourcePdfBox.Text) == false) {
				SelectFiles(new string[] { _SourcePdfBox.Text });
			}
		}

		void _SourcePdfBox_DragEnter(object sender, DragEventArgs e) {
			FormHelper.FeedbackDragFileOver(e, Constants.FileExtensions.Pdf);
		}

		void _SourcePdfBox_DragDrop(object sender, DragEventArgs e) {
			var files = FormHelper.DropFileOver(e, Constants.FileExtensions.Pdf);
			SelectFiles(files);
		}

		void SourceFileControl_Show(object sender, EventArgs e) {
			_controlLockDown = true;
			var t = Text;
			if (Visible && AppContext.MainForm != null) {
				_SourcePdfBox.Contents = AppContext.Recent.SourcePdfFiles;
			}
			else if (Visible == false) {
				_SourcePdfBox.Contents = null;
			}
			Text = t;
			_controlLockDown = false;
		}
	}
}
