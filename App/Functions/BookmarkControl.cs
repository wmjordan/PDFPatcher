using System;
using System.ComponentModel;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher;

public partial class BookmarkControl : UserControl
{
	//readonly string[] xmlBookmarkType = new string[] { ".xml" };
	//private string[] supportedBookmarkTypes;
	internal event EventHandler<EventArgs> BrowseForFile;

	public BookmarkControl() {
		InitializeComponent();
		//supportedBookmarkTypes = defaultBookmarkTypes;
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
			//supportedBookmarkTypes = value ? xmlBookmarkType : defaultBookmarkTypes;
			_UseForBookmarkExport = value;
	}

	private void _BrowseSourcePdfButton_Click(object sender, EventArgs e) {
		BrowseForFile?.Invoke(sender, e);
		string sourceFile = AppContext.SourceFiles != null && AppContext.SourceFiles.Length > 0
			? AppContext.SourceFiles[0]
			: string.Empty;
		if (FileHelper.IsPathValid(_BookmarkBox.Text) && System.IO.Path.GetFileName(_BookmarkBox.Text).Length > 0) {
			FilePath p = new(_BookmarkBox.Text);
			_OpenBookmarkBox.SetLocation(p);
			_SaveBookmarkBox.SetLocation(p);
		}
		else if (sourceFile.Length > 0) {
			FilePath p = new FilePath(sourceFile).ChangeExtension("xml");
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
		//Common.Form.FeedbackDragFileOver (e, supportedBookmarkTypes);
		e.FeedbackDragFileOver(Constants.FileExtensions.AllBookmarkExtension);
	}

	private void _BookmarkBox_DragDrop(object sender, DragEventArgs e) {
		//Common.Form.DropFileOver ((Control)sender, e, supportedBookmarkTypes);
		((Control)sender).DropFileOver(e, Constants.FileExtensions.AllBookmarkExtension);
	}

	private void _BookmarkBox_TextChanged(object sender, EventArgs e) {
		AppContext.BookmarkFile = _BookmarkBox.Text;
	}

	private void BookmarkControl_Show(object sender, EventArgs e) {
		string t = _BookmarkBox.Text;
		if (Visible && AppContext.MainForm != null) {
			// _BookmarkBox.DataSource = new BindingList<string> (_UseForBookmarkExport ? ContextData.Recent.SavedInfoDocuments : ContextData.Recent.InfoDocuments);
			_BookmarkBox.Contents = AppContext.Recent.InfoDocuments;
		}
		else if (Visible == false) {
			_BookmarkBox.Contents = null;
		}

		_BookmarkBox.Text = t;
	}
}