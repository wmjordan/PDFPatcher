using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher
{
	public partial class TargetFileControl : UserControl
	{
		internal event CancelEventHandler TargetFileChangedByBrowseButton;
		internal event EventHandler<EventArgs> BrowseForFile;

		public TargetFileControl () {
			InitializeComponent ();

			this._FileMacroMenu.ItemClicked += _FileMacroMenu.ProcessInsertMacroCommand;
		}

		///<summary>获取或指定书签文件路径的值。</summary>
		internal HistoryComboBox FileList {
			get { return _TargetPdfBox; }
		}
		internal FileDialog FileDialog {
			get { return _SavePdfBox; }
		}
		internal Functions.MacroMenu FileMacroMenu {
			get { return _FileMacroMenu; }
		}

		public string BrowseTargetFile () {
			_BrowseTargetPdfButton_Click (_BrowseTargetPdfButton, null);
			return this._TargetPdfBox.Text;
		}

		/// <summary>
		/// 获取或设置文件下拉框的文本。
		/// </summary>
		public override string Text {
			get {
				return this._TargetPdfBox.Text;
			}
			set {
				this._TargetPdfBox.Text = value;
			}
		}

		/// <summary>
		/// 获取或设置文件下拉框前的标签文本。
		/// </summary>
		[DefaultValue ("输出 PD&F 文件：")]
		public string Label {
			get { return label1.Text; }
			set { label1.Text = value; }
		}

		private void _BrowseTargetPdfButton_Click (object sender, EventArgs e) {
			if (BrowseForFile != null) {
				BrowseForFile (sender, e);
			}
			FilePath sourceFile = (AppContext.SourceFiles != null && AppContext.SourceFiles.Length > 0) ? AppContext.SourceFiles[0] : String.Empty;
			var t = _TargetPdfBox.Text;
			if (t.Length > 0 && FileHelper.IsPathValid (t) && Path.GetFileName (t).Length > 0) {
				_SavePdfBox.SetLocation (t);
			}
			else if (sourceFile.FileName.Length > 0) {
				t = FileHelper.GetNewFileNameFromSourceFile (sourceFile, Constants.FileExtensions.Pdf);
				_SavePdfBox.SetLocation (t);
			}
			if (_SavePdfBox.ShowDialog () == DialogResult.OK) {
				if (this.TargetFileChangedByBrowseButton != null) {
					var a = new CancelEventArgs ();
					this.TargetFileChangedByBrowseButton (this, a);
					if (a.Cancel) {
						return;
					}
				}
				this.Text = _SavePdfBox.FileName;
			}
		}

		private void _TargetPdfBox_TextChanged (object sender, EventArgs e) {
			AppContext.TargetFile = this._TargetPdfBox.Text;
		}

		private void _TargetPdfBox_DragEnter (object sender, DragEventArgs e) {
			FormHelper.FeedbackDragFileOver (e, Constants.FileExtensions.Pdf);
		}

		private void _TargetPdfBox_DragDrop (object sender, DragEventArgs e) {
			FormHelper.DropFileOver ((Control)sender, e, Constants.FileExtensions.Pdf);
		}

		private void TargetFileControl_Show (object sender, EventArgs e) {
			var t = this.Text;
			if (this.Visible && AppContext.MainForm != null) {
				this._TargetPdfBox.Contents = AppContext.Recent.TargetPdfFiles;
			}
			else if (this.Visible == false) {
				this._TargetPdfBox.Contents = null;
			}
			this.Text = t;
		}

	}
}
