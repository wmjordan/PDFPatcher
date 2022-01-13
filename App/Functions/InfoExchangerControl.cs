﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;
using PDFPatcher.Properties;

namespace PDFPatcher.Functions;

[ToolboxItem(false)]
public partial class InfoExchangerControl : FunctionControl
{
	private FileListHelper _listHelper;

	public InfoExchangerControl() {
		InitializeComponent();
	}

	public override string FunctionName => "导出导入信息文件";

	public override Bitmap IconImage => Resources.CreateDocument;

	#region IDefaultButtonControl 成员

	public override Button DefaultButton => _ImportButton;

	#endregion

	private void PatcherControl_OnLoad(object sender, EventArgs e) {
		//this.Icon = Common.FormHelper.ToIcon (Properties.Resources.CreateDocument);
		_ItemList.ListViewItemSorter = new ListViewItemComparer(0);

		AppContext.MainForm.SetTooltip(_BookmarkControl.FileList, "需要导出或导入的信息文件路径");
		AppContext.MainForm.SetTooltip(_ActionsBox, "双击项目编辑操作选项；右键点击项目弹出上下文菜单");
		AppContext.MainForm.SetTooltip(_ItemList, "在此添加需要导出或导入信息文件的 PDF 文件");
		AppContext.MainForm.SetTooltip(_ImportButton, "点击此按钮执行补丁生成新的 PDF 文件，该文件具有信息文件的设定");
		AppContext.MainForm.SetTooltip(_TargetPdfFile.FileList, "在此指定导入信息文件后生成的目标 PDF 文件路径（鼠标右键点击列表可插入文件名替代符）");
		_ItemList.EmptyListMsg = "请使用“添加文件”按钮添加需要处理的 PDF 文件，或从资源管理器拖放文件到本列表框";

		_ConfigButton.Click += (s, args) => AppContext.MainForm.SelectFunctionList(Function.PatcherOptions);
		_InfoConfigButton.Click += (s, args) => AppContext.MainForm.SelectFunctionList(Function.InfoFileOptions);

		new TypedColumn<IProcessor>(_ActionNameColumn) {
			AspectGetter = o => o.Name,
			ImageGetter = o => {
				if (o is IPageProcessor) {
					return Resources.PageProcessor;
				}

				return Resources.DocumentProcessor;
			}
		};
		_TargetPdfFile.FileMacroMenu.LoadStandardInfoMacros();
		_TargetPdfFile.FileMacroMenu.LoadStandardSourceFileMacros();
		_BookmarkControl.FileDialog.CheckFileExists = false;
		_BookmarkControl.BrowseForFile += FileControl_BrowseForFile;
		_TargetPdfFile.BrowseForFile += FileControl_BrowseForFile;
		_TargetPdfFile.TargetFileChangedByBrowseButton += (s, args) => {
			int i;
			string f = _TargetPdfFile.FileDialog.FileName;
			if (_ItemList.Items.Count > 1 && (i = f.LastIndexOf(Path.DirectorySeparatorChar)) != -1) {
				_TargetPdfFile.Text = string.Concat(f.Substring(0, i), Path.DirectorySeparatorChar,
					Constants.FileNameMacros.FileName, Path.GetExtension(f));
				args.Cancel = true;
			}
		};
		ImageList.ImageCollection fi = _FileTypeList.Images;
		fi.AddRange(new Image[] { Resources.OriginalPdfFile });
		//_ItemList.SelectedIndexChanged += (s, args) => {
		//	if (_ItemList.SelectedIndex != -1 && _TargetPdfFile.Text.Trim ().Length > 0) {
		//		var f = _ItemList.GetModelObject (_ItemList.SelectedIndex) as SourceItem;
		//		AppContext.MainForm.StatusText = "输出文件：" + f.GetTargetPdfFileName (_TargetPdfFile.Text.Trim ());
		//	}
		//	else {
		//		AppContext.MainForm.StatusText = String.Empty;
		//	}
		//};
		_ItemList.FixEditControlWidth();
		_listHelper = new FileListHelper(_ItemList);
		_listHelper.SetupDragAndDrop(AddFiles);
		FileListHelper.SetupCommonPdfColumns(_AuthorColumn, _KeywordsColumn, _SubjectColumn, _TitleColumn,
			_PageCountColumn, _NameColumn, _FolderColumn);
		_RefreshInfoButton.ButtonClick +=
			(s, args) => _listHelper.RefreshInfo(AppContext.Encodings.DocInfoEncoding);
		_RefreshInfoButton.DropDown = _RefreshInfoMenu;
		foreach (string item in Constants.Encoding.EncodingNames) {
			_RefreshInfoMenu.Items.Add(item);
		}

		_RefreshInfoMenu.ItemClicked += (s, args) =>
			_listHelper.RefreshInfo(ValueHelper.MapValue(args.ClickedItem.Text, Constants.Encoding.EncodingNames,
				Constants.Encoding.Encodings));
		_AddFilesButton.DropDownOpening += FileListHelper.OpenPdfButtonDropDownOpeningHandler;
		_AddFilesButton.DropDownItemClicked += (s, args) => {
			args.ClickedItem.Owner.Hide();
			AddFiles(new[] { args.ClickedItem.ToolTipText }, true);
		};
	}

	public override void ExecuteCommand(string commandName, params string[] parameters) {
		if (commandName == Commands.Open) {
			OpenFileDialog b = _OpenPdfBox;
			if (b.ShowDialog() == DialogResult.OK) {
				AddFiles(b.FileNames, true);
			}
		}
		else if (commandName == Commands.OpenFile) {
			AddFiles(parameters, true);
		}
		else if (commandName == Commands.SaveBookmark) {
			_ExportBookmarkButton.PerformClick();
		}
		else if (_listHelper.ProcessCommonMenuCommand(commandName) == false) {
			base.ExecuteCommand(commandName, parameters);
		}
	}

	public override void SetupCommand(ToolStripItem item) {
		string n = item.Name;
		if (Commands.CommonSelectionCommands.Contains(n)
			|| n == Commands.Delete
			|| n == Commands.Action) {
			item.Enabled = _ItemList.GetItemCount() > 0;
		}
		else if (n == Commands.SaveBookmark) {
			item.Enabled = _ItemList.GetItemCount() > 0;
			item.Text = "导出书签文件(&Q)";
			item.Visible = true;
		}
		else if (n == Commands.Options) {
			item.Text = "修改文档设置(&S)...";
			item.ToolTipText = "设置修改后的 PDF 文档";
			EnableCommand(item, true, true);
			item.Tag = nameof(Function.PatcherOptions);
		}

		base.SetupCommand(item);
	}

	private void FileControl_BrowseForFile(object sender, EventArgs e) {
		_listHelper.PrepareSourceFiles();
	}

	private void AddFiles(string[] files, bool alertInvalidFiles) {
		if (files == null || files.Length == 0) {
			return;
		}

		if ((ModifierKeys & Keys.Control) != Keys.None || _AutoClearListBox.Checked) {
			_ItemList.ClearObjects();
		}

		if (files.Length > 3) {
			AppContext.MainForm.Enabled = false;
		}

		if (files.Length == 0) {
			return;
		}

		_AddDocumentWorker.RunWorkerAsync(files);
	}

	private void _ImportButton_Click(object sender, EventArgs e) {
		string infoFile = _BookmarkControl.Text.Trim();
		string targetPdfFile = _TargetPdfFile.Text.Trim();
		if (string.IsNullOrEmpty(targetPdfFile) &&
			string.IsNullOrEmpty(targetPdfFile = _TargetPdfFile.BrowseTargetFile())) {
			FormHelper.ErrorBox(Messages.TargetFileNotSpecified);
			return;
		}
		//if (_mode == ProcessMode.Merge && Common.FileHelper.IsPathValid (targetPdfFile) == false) {
		//    Common.FormHelper.ErrorBox ("输出文件名无效。" + (Common.FileUtility.HasFileNameMacro (targetPdfFile) ? "\n制作 PDF 文件功能不支持替代符。" : String.Empty));
		//    return;
		//}

		int l = _ItemList.GetItemCount();
		if (l == 0) {
			FormHelper.InfoBox("请添加需要处理的 PDF 文件。");
			return;
		}

		List<SourceItem> files = GetSourceItemList();
		_BookmarkControl.FileList.AddHistoryItem();
		_TargetPdfFile.FileList.AddHistoryItem();

		AppContext.MainForm.ResetWorker();
		BackgroundWorker worker = AppContext.MainForm.GetWorker();
		worker.DoWork += (dummy, arg) => {
			object[] a = arg.Argument as object[];
			string t = a[0] as string;
			if (files.Count > 1) {
				Tracker.SetTotalProgressGoal(files.Count);
				foreach (SourceItem file in files) {
					if (file.Type == SourceItem.ItemType.Pdf) {
						Worker.PatchDocument(file as SourceItem.Pdf,
							file.GetTargetPdfFileName(t),
							file.GetInfoFileName(),
							AppContext.Importer,
							AppContext.Patcher);
					}
					else {
						Tracker.TraceMessage("输入文件不是 PDF 文件。");
					}

					Tracker.IncrementTotalProgress();
					if (AppContext.Abort) {
						return;
					}
				}
			}
			else {
				if (files[0].Type != SourceItem.ItemType.Pdf) {
					Tracker.TraceMessage("输入文件不是 PDF 文件。");
					return;
				}

				Worker.PatchDocument(files[0] as SourceItem.Pdf,
					t,
					a[1] as string,
					AppContext.Importer,
					AppContext.Patcher);
			}
		};
		worker.RunWorkerAsync(new object[] { targetPdfFile, infoFile });
	}

	private void _ExportBookmarkButton_Click(object sender, EventArgs e) {
		if (string.IsNullOrEmpty(_BookmarkControl.Text)) {
			FormHelper.ErrorBox(Messages.InfoDocNotSpecified);
			return;
		}

		List<SourceItem> files = GetSourceItemList();

		if (files.Count == 0) {
			FormHelper.InfoBox(Messages.SourceFileNotFound);
			return;
		}

		_BookmarkControl.FileList.AddHistoryItem();

		AppContext.MainForm.ResetWorker();
		BackgroundWorker w = AppContext.MainForm.GetWorker();
		w.DoWork += (dummy, arg) => {
			object[] a = arg.Argument as object[];
			if (files.Count > 1) {
				string p = Path.GetDirectoryName(a[0] as string);
				string ext = Path.GetExtension(a[0] as string);
				Tracker.SetTotalProgressGoal(files.Count);
				foreach (SourceItem file in files) {
					Worker.ExportInfo(file.FilePath.ToString(),
						file.FilePath.Directory.Combine(p).Combine(file.FilePath.ChangeExtension(ext)).ToString());
					Tracker.IncrementTotalProgress();
					if (AppContext.Abort) {
						return;
					}
				}
			}
			else {
				if (files[0].Type != SourceItem.ItemType.Pdf) {
					Tracker.TraceMessage("输入文件不是 PDF 文件。");
					return;
				}

				Worker.ExportInfo(files[0].FilePath.ToString(), a[0] as string);
			}
		};
		w.RunWorkerAsync(new object[] { AppContext.BookmarkFile });
	}

	private List<SourceItem> GetSourceItemList() {
		int l = _ItemList.GetItemCount();
		List<SourceItem> files = new(l);
		for (int i = 0; i < l; i++) {
			SourceItem item = _ItemList.GetModelObject(_ItemList.GetNthItemInDisplayOrder(i).Index) as SourceItem;
			if (item.Type == SourceItem.ItemType.Pdf
				&& FileHelper.HasExtension(item.FilePath, Constants.FileExtensions.Pdf)) {
				AppContext.RecentItems.AddHistoryItem(AppContext.Recent.SourcePdfFiles, item.FilePath.ToString());
			}

			files.Add(item);
		}

		return files;
	}

	private void _SortMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		switch (e.ClickedItem.Name) {
			case "_SortByAlphaItem":
				_ItemList.ListViewItemSorter = new ListViewItemComparer(0, false);
				break;
			case "_SortByNaturalNumberItem":
				_ItemList.ListViewItemSorter = new ListViewItemComparer(0, true);
				break;
		}
	}

	private void _ImageList_ColumnClick(object sender, ColumnClickEventArgs e) {
		int c = e.Column;
		bool ss = c == 0 || c == _PageCountColumn.Index;
		SortOrder o = _ItemList.PrimarySortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
		_ItemList.ListViewItemSorter = new ListViewItemComparer(e.Column, ss, o);
	}

	private void _MainToolbar_ButtonClick(object sender, EventArgs e) {
		if (sender == _AddFilesButton) {
			OpenFileDialog b = _OpenPdfBox;
			_AddFilesButton.DropDown.Items.ClearDropDownItems();
			if (b.ShowDialog() == DialogResult.OK) {
				AddFiles(b.FileNames, true);
			}
		}
	}

	private void _MainToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		_listHelper.ProcessCommonMenuCommand(e.ClickedItem.Name);
	}

	private void PreviewRename(List<SourceItem.Pdf> items, string template) {
		int i = 0;
		string[] result = new string[items.Count];
		string[] source = new string[items.Count];
		FilePath s;
		string t;
		foreach (SourceItem.Pdf item in items) {
			try {
				s = item.FilePath;
				if (s.ExistsFile == false) {
					t = string.Concat("(找不到 PDF 文件：", s, ")");
					continue;
				}

				t = Worker.RenameFile(template, item);
				if (t.Length == 0) {
					t = "<输出文件名无效>";
				}
				else if (Path.GetFileName(t).Length == 0) {
					t = "<输出文件名为空>";
				}

				source[i] = s.ToString();
				result[i] = t;
				i++;
			}
			catch (Exception ex) {
				FormHelper.ErrorBox(ex.Message);
			}
		}

		using (RenamePreviewForm f = new(source, result)) {
			f.ShowDialog();
		}
	}

	#region AddDocumentWorker

	private void _AddDocumentWorker_DoWork(object sender, DoWorkEventArgs e) {
		string[] files = e.Argument as string[];
		Array.ForEach(files, f => ((BackgroundWorker)sender).ReportProgress(0, f));
	}

	private void _AddDocumentWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
		AppContext.MainForm.Enabled = true;
		_listHelper.ResizeItemListColumns();
	}

	private void _AddDocumentWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
		string item = e.UserState as string;
		AddItem(SourceItem.Create(item));
	}

	private void AddItem(SourceItem item) {
		if (item == null) {
			return;
		}

		AddItems(new[] { item });
	}

	private void AddItems(ICollection items) {
		int i = _ItemList.GetLastSelectedIndex();
		_ItemList.InsertObjects(++i, items);
		_ItemList.SelectedIndex = --i + items.Count;
	}

	#endregion
}