﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using CLR;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	[ToolboxItem(false)]
	public partial class InfoExchangerControl : FunctionControl
	{
		FileListHelper _listHelper;

		public override string FunctionName => "导出导入信息文件";

		public override System.Drawing.Bitmap IconImage => Properties.Resources.ExportInfoFile;

		public InfoExchangerControl() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
		}

		void OnLoad() {
			_MainToolbar.ScaleIcons(16);
			_ItemList.ListViewItemSorter = new ListViewItemComparer(0);

			AppContext.MainForm.SetTooltip(_BookmarkControl.FileList, "需要导出或导入的信息文件路径");
			AppContext.MainForm.SetTooltip(_ActionsBox, "双击项目编辑操作选项；右键点击项目弹出上下文菜单");
			AppContext.MainForm.SetTooltip(_ItemList, "在此添加需要导出或导入信息文件的 PDF 文件");
			AppContext.MainForm.SetTooltip(_ImportButton, "点击此按钮执行补丁生成新的 PDF 文件，该文件具有信息文件的设定");
			AppContext.MainForm.SetTooltip(_TargetPdfFile.FileList, "在此指定导入信息文件后生成的目标 PDF 文件路径（鼠标右键点击列表可插入文件名替代符）");
			_ItemList.EmptyListMsg = "请使用“添加文件”按钮添加需要处理的 PDF 文件，或从资源管理器拖放文件到本列表框";

			_ConfigButton.Click += (s, args) => AppContext.MainForm.SelectFunctionList(Function.PatcherOptions);
			_InfoConfigButton.Click += (s, args) => AppContext.MainForm.SelectFunctionList(Function.InfoFileOptions);

			new TypedColumn<Processor.IProcessor>(_ActionNameColumn) {
				AspectGetter = (o) => o.Name,
				ImageGetter = (o) => o is Processor.IPageProcessor
					? Properties.Resources.PageProcessor
					: Properties.Resources.DocumentProcessor
			};
			_TargetPdfFile.FileMacroMenu.LoadStandardInfoMacros();
			_TargetPdfFile.FileMacroMenu.LoadStandardSourceFileMacros();
			_BookmarkControl.FileDialog.CheckFileExists = false;
			_BookmarkControl.BrowseForFile += FileControl_BrowseForFile;
			_TargetPdfFile.BrowseForFile += FileControl_BrowseForFile;
			_TargetPdfFile.TargetFileChangedByBrowseButton += (s, args) => {
				int i;
				var f = _TargetPdfFile.FileDialog.FileName;
				if (_ItemList.Items.Count > 1 && (i = f.LastIndexOf(Path.DirectorySeparatorChar)) != -1) {
					_TargetPdfFile.Text = string.Concat(f.Substring(0, i), Path.DirectorySeparatorChar, Constants.FileNameMacros.FileName, Path.GetExtension(f));
					args.Cancel = true;
				}
			};
			var fi = _FileTypeList.Images;
			fi.AddRange([Properties.Resources.OriginalPdfFile]);
			_ItemList.FixEditControlWidth();
			_ItemList.ScaleColumnWidths();
			_listHelper = new FileListHelper(_ItemList);
			_listHelper.SetupDragAndDrop(AddFiles);
			FileListHelper.SetupCommonPdfColumns(_AuthorColumn, _KeywordsColumn, _SubjectColumn, _TitleColumn, _PageCountColumn, _NameColumn, _FolderColumn, _FileTimeColumn);
			_RefreshInfoButton.ButtonClick += (s, args) => _listHelper.RefreshInfo(AppContext.Encodings.DocInfoEncoding);
			_RefreshInfoButton.DropDown = _RefreshInfoMenu;
			foreach (var item in Constants.Encoding.EncodingNames) {
				_RefreshInfoMenu.Items.Add(item);
			}
			_RefreshInfoMenu.ItemClicked += (s, args) => _listHelper.RefreshInfo(ValueHelper.MapValue(args.ClickedItem.Text, Constants.Encoding.EncodingNames, Constants.Encoding.Encodings));
			_AddFilesButton.DropDownOpening += FileListHelper.OpenPdfButtonDropDownOpeningHandler;
			_AddFilesButton.DropDownItemClicked += (s, args) => {
				args.ClickedItem.Owner.Hide();
				AddFiles([args.ClickedItem.ToolTipText], true);
			};
		}

		public override void ExecuteCommand(string commandName, params string[] parameters) {
			if (commandName == Commands.Open) {
				var b = _OpenPdfBox;
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
			else if (!_listHelper.ProcessCommonMenuCommand(commandName)) {
				base.ExecuteCommand(commandName, parameters);
			}
		}

		public override void SetupCommand(ToolStripItem item) {
			var n = item.Name;
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

		void FileControl_BrowseForFile(object sender, EventArgs e) {
			_listHelper.PrepareSourceFiles();
		}

		void AddFiles(string[] files, bool alertInvalidFiles) {
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

		void _ImportButton_Click(object sender, EventArgs e) {
			var infoFile = _BookmarkControl.Text.Trim();
			var targetPdfFile = _TargetPdfFile.Text.Trim();
			if (string.IsNullOrEmpty(targetPdfFile) && string.IsNullOrEmpty(targetPdfFile = _TargetPdfFile.BrowseTargetFile())) {
				FormHelper.ErrorBox(Messages.TargetFileNotSpecified);
				return;
			}
			var l = _ItemList.GetItemCount();
			if (l == 0) {
				FormHelper.InfoBox("请添加需要处理的 PDF 文件。");
				return;
			}
			var files = GetSourceItemList();
			_BookmarkControl.FileList.AddHistoryItem();
			_TargetPdfFile.FileList.AddHistoryItem();

			AppContext.MainForm.ResetWorker();
			var worker = AppContext.MainForm.GetWorker();
			worker.DoWork += (dummy, arg) => {
				var a = arg.Argument as object[];
				var t = a[0] as string;
				if (files.Count > 1) {
					Tracker.SetTotalProgressGoal(files.Count);
					foreach (var file in files) {
						if (file.Type == SourceItem.ItemType.Pdf) {
							Processor.Worker.PatchDocument(file as SourceItem.Pdf,
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
					Processor.Worker.PatchDocument(files[0] as SourceItem.Pdf,
						t,
						a[1] as string,
						AppContext.Importer,
						AppContext.Patcher);
				}
			};
			worker.RunWorkerAsync(new object[] { targetPdfFile, infoFile });
		}

		void _ExportBookmarkButton_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(_BookmarkControl.Text)) {
				FormHelper.ErrorBox(Messages.InfoDocNotSpecified);
				return;
			}

			var files = GetSourceItemList();

			if (files.Count == 0) {
				FormHelper.InfoBox(Messages.SourceFileNotFound);
				return;
			}

			_BookmarkControl.FileList.AddHistoryItem();

			AppContext.MainForm.ResetWorker();
			var w = AppContext.MainForm.GetWorker();
			w.DoWork += (dummy, arg) => {
				var b = AppContext.BookmarkFile;
				var fs = arg.Argument as List<SourceItem>;
				if (fs.Count > 1) {
					var p = b.Contains(Path.DirectorySeparatorChar) || b.Contains(Path.AltDirectorySeparatorChar) ? Path.GetDirectoryName(b) : Path.GetFileNameWithoutExtension(b);
					var ext = Path.GetExtension(b);
					Tracker.SetTotalProgressGoal(fs.Count);
					foreach (var file in fs) {
						Processor.Worker.ExportInfo(file.FilePath, file.FilePath.Directory.Combine(p).Combine(file.FilePath.ChangeExtension(ext).FileName));
						Tracker.IncrementTotalProgress();
						if (AppContext.Abort) {
							return;
						}
					}
				}
				else {
					if (fs[0].Type != SourceItem.ItemType.Pdf) {
						Tracker.TraceMessage("输入文件不是 PDF 文件。");
						return;
					}
					Processor.Worker.ExportInfo(fs[0].FilePath, b);
				}
			};
			w.RunWorkerAsync(files);
		}

		List<SourceItem> GetSourceItemList() {
			var l = _ItemList.GetItemCount();
			var files = new List<SourceItem>(l);
			for (int i = 0; i < l; i++) {
				var item = _ItemList.GetModelObject(_ItemList.GetNthItemInDisplayOrder(i).Index) as SourceItem;
				if (item.Type == SourceItem.ItemType.Pdf
					&& FileHelper.HasExtension(item.FilePath, Constants.FileExtensions.Pdf)) {
					AppContext.RecentItems.AddHistoryItem(AppContext.Recent.SourcePdfFiles, item.FilePath.ToString());
				}
				files.Add(item);
			}
			return files;
		}

		void _SortMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			switch (e.ClickedItem.Name) {
				case "_SortByAlphaItem":
					_ItemList.ListViewItemSorter = new ListViewItemComparer(0, false);
					break;
				case "_SortByNaturalNumberItem":
					_ItemList.ListViewItemSorter = new ListViewItemComparer(0, true);
					break;
			}
		}

		void _ImageList_ColumnClick(object sender, ColumnClickEventArgs e) {
			var c = e.Column;
			var ss = c.CeqAny(0, _PageCountColumn.Index);
			var o = _ItemList.PrimarySortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
			_ItemList.ListViewItemSorter = new ListViewItemComparer(e.Column, ss, o);
		}

		void _MainToolbar_ButtonClick(object sender, EventArgs e) {
			if (sender == _AddFilesButton) {
				var b = _OpenPdfBox;
				_AddFilesButton.DropDown.Items.ClearDropDownItems();
				if (b.ShowDialog() == DialogResult.OK) {
					AddFiles(b.FileNames, true);
				}
			}
		}

		void _MainToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			_listHelper.ProcessCommonMenuCommand(e.ClickedItem.Name);
		}

		#region AddDocumentWorker
		void _AddDocumentWorker_DoWork(object sender, DoWorkEventArgs e) {
			var files = e.Argument as string[];
			Array.ForEach(files, f => ((BackgroundWorker)sender).ReportProgress(0, f));
		}

		void _AddDocumentWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			AppContext.MainForm.Enabled = true;
			_listHelper.ResizeItemListColumns();
		}

		void _AddDocumentWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			var item = e.UserState as string;
			AddItem(SourceItem.Create(item));
		}

		void AddItem(SourceItem item) {
			if (item == null) {
				return;
			}
			AddItems(new SourceItem[] { item });
		}

		void AddItems(System.Collections.ICollection items) {
			var i = _ItemList.GetLastSelectedIndex();
			_ItemList.InsertObjects(++i, items);
			_ItemList.SelectedIndex = --i + items.Count;
		}
		#endregion

		#region IDefaultButtonControl 成员

		public override Button DefaultButton => _ImportButton;

		#endregion

	}
}
