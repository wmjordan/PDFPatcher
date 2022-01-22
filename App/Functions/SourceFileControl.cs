using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher;

public partial class SourceFileControl : UserControl
{
	private bool _controlLockDown;

	public SourceFileControl() {
		_controlLockDown = true;
		InitializeComponent();
		_controlLockDown = false;
		Files = new string[] { };
	}

	///<summary>获取文件下拉列表框。</summary>
	internal HistoryComboBox FileList { get; private set; }

	/// <summary>
	///     获取选定的 PDF 文件列表。
	/// </summary>
	internal string[] Files { get; private set; }

	/// <summary>
	///     获取选定的 PDF 文件列表的第一项。
	/// </summary>
	internal string FirstFile => Files != null && Files.Length > 0 ? Files[0] : string.Empty;

	[DefaultValue(null)]
	public override string Text {
		get => FileList.Text;
		set => FileList.Text = value;
	}

	///<summary>点击浏览按钮更改选中文件后触发的事件。</summary>
	public event EventHandler BrowseSelectedFiles;

	/////<summary>获取或指定文本标签的值。</summary>
	//[Description ("文本标签的值")]
	//public string Label {
	//    get { return this.label1.Text; }
	//    set { this.label1.Text = value; }
	//}

	/////<summary>获取或指定是否可多选文件。</summary>
	//[Description ("是否可多选文件")]
	//public bool MultiSelect {
	//    get { return this._OpenPdfBox.Multiselect; }
	//    set { this._OpenPdfBox.Multiselect = value; }
	//}

	private void _BrowseSourcePdfButton_Click(object sender, EventArgs e) {
		string t = FileList.Text;
		if (t.Length > 0
			&& FileHelper.IsPathValid(t)
			&& Path.GetFileName(t).Length > 0) {
			_OpenPdfBox.FileName = t;
		}

		if (_OpenPdfBox.ShowDialog() == DialogResult.OK) {
			SelectFiles(_OpenPdfBox.FileNames);
			if (BrowseSelectedFiles != null) {
				BrowseSelectedFiles(sender, e);
			}
		}
	}

	private void SelectFiles(string[] files) {
		string t = FileList.Text;
		if (files.Length > 1) {
			Text = string.Concat("<选定了 ", files.Length, " 个文件>", Path.GetDirectoryName(files[0]));
		}
		else if (files[0] != t) {
			Text = files[0];
		}

		Files = files;
	}

	private void _SourcePdfBox_TextChanged(object sender, EventArgs e) {
		if (_controlLockDown) {
			return;
		}

		if (FileHelper.HasFileNameMacro(FileList.Text) == false) {
			SelectFiles(new[] { FileList.Text });
		}
	}

	private void _SourcePdfBox_DragEnter(object sender, DragEventArgs e) {
		e.FeedbackDragFileOver(Constants.FileExtensions.Pdf);
	}

	private void _SourcePdfBox_DragDrop(object sender, DragEventArgs e) {
		string[] files = e.DropFileOver(Constants.FileExtensions.Pdf);
		SelectFiles(files);
	}

	private void SourceFileControl_Show(object sender, EventArgs e) {
		_controlLockDown = true;
		string t = Text;
		if (Visible && AppContext.MainForm != null) {
			FileList.Contents = AppContext.Recent.SourcePdfFiles;
		}
		else if (Visible == false) {
			FileList.Contents = null;
		}

		Text = t;
		_controlLockDown = false;
	}

	private void label1_Click(object sender, EventArgs e) {
	}

	private void _SourcePdfBox_SelectedIndexChanged(object sender, EventArgs e) {
	}
}