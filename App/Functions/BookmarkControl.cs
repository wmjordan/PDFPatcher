using System;
using System.ComponentModel;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher
{
	public partial class BookmarkControl : UserControl
	{
		internal event EventHandler<EventArgs> BrowseForFile;

		public BookmarkControl() {
			InitializeComponent();
		}

		///<summary>获取或指定书签文件路径的下拉列表框。</summary>
		internal HistoryComboBox FileList => _BookmarkBox;

		internal FileDialog FileDialog => _UseForBookmarkExport ? (FileDialog)_SaveBookmarkBox : _OpenBookmarkBox;

		[Description("标签文本上显示的文本")]
		public string LabelText {
			get => label1.Text;
			set => label1.Text = value;
		}

		[DefaultValue(null)]
		///<summary>获取或指定书签文件路径的值。</summary>
		public override string Text {
			get => _BookmarkBox.Text;
			set => _BookmarkBox.Text = value;
		}

		private bool _UseForBookmarkExport;
		///<summary>获取或指定是否用于导出书签。</summary>
		[DefaultValue(false)]
		[Description("点击浏览按钮时是否打开保存对话框")]
		public bool UseForBookmarkExport {
			get => _UseForBookmarkExport;
			set =>
				_UseForBookmarkExport = value;
		}

		private void _BrowseSourcePdfButton_Click(object sender, EventArgs e) {
			BrowseForFile?.Invoke(sender, e);
			var sourceFile = (AppContext.SourceFiles != null && AppContext.SourceFiles.Length > 0) ? AppContext.SourceFiles[0] : String.Empty;
			if (FileHelper.IsPathValid(_BookmarkBox.Text) && System.IO.Path.GetFileName(_BookmarkBox.Text).Length > 0) {
				var p = new FilePath(_BookmarkBox.Text);
				_OpenBookmarkBox.SetLocation(p);
				_SaveBookmarkBox.SetLocation(p);
			}
			else if (sourceFile.Length > 0) {
				var p = new FilePath(sourceFile).ChangeExtension("xml");
				_SaveBookmarkBox.SetLocation(p);
				_OpenBookmarkBox.SetLocation(p);
			}
			if (_UseForBookmarkExport) {
				if (_SaveBookmarkBox.ShowDialog() == DialogResult.OK) {
					_BookmarkBox.Text = _SaveBookmarkBox.FileName;
				}
			}
			else if (_OpenBookmarkBox.ShowDialog() == DialogResult.OK) {
				if (_OpenBookmarkBox.FileName == _BookmarkBox.Text) {
					return;
				}
				_BookmarkBox.Text = _OpenBookmarkBox.FileName;
			}
		}

		private void _BookmarkBox_DragEnter(object sender, DragEventArgs e) {
			e.FeedbackDragFileOver(Constants.FileExtensions.AllBookmarkExtension);
		}

		private void _BookmarkBox_DragDrop(object sender, DragEventArgs e) {
			((Control)sender).DropFileOver(e, Constants.FileExtensions.AllBookmarkExtension);
		}

		private void _BookmarkBox_TextChanged(object sender, EventArgs e) {
			AppContext.BookmarkFile = _BookmarkBox.Text;
		}

		private void BookmarkControl_Show(object sender, EventArgs e) {
			var t = _BookmarkBox.Text;
			if (Visible && AppContext.MainForm != null) {
				_BookmarkBox.Contents = AppContext.Recent.InfoDocuments;
			}
			else if (!Visible) {
				_BookmarkBox.Contents = null;
			}
			_BookmarkBox.Text = t;
		}

	}
}
