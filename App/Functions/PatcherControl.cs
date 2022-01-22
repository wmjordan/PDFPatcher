using System;
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
public partial class PatcherControl : FunctionControl
{
	private FileListHelper _listHelper;

	public PatcherControl() {
		InitializeComponent();
	}

	public override string FunctionName => "批量修改文档";

	public override Bitmap IconImage => Resources.CreateDocument;

	public override Button DefaultButton => _ImportButton;

	private void PatcherControl_OnLoad(object sender, EventArgs e) {
		//this.Icon = Common.FormHelper.ToIcon (Properties.Resources.CreateDocument);

		AppContext.MainForm.SetTooltip(_ConfigButton, "点击此处设置 PDF 文件的修改方式选项");
		AppContext.MainForm.SetTooltip(_ActionsBox, "双击项目编辑操作选项；右键点击项目弹出上下文菜单");
		AppContext.MainForm.SetTooltip(_ItemList, "在此添加需要补丁修改的 PDF 文件");
		AppContext.MainForm.SetTooltip(_ImportButton, "点击此按钮执行补丁生成新的 PDF 文件，该文件具有信息文件和 PDF 选项中的设定");
		AppContext.MainForm.SetTooltip(_TargetPdfFile.FileList, "生成的目标 PDF 文件路径（鼠标右键点击列表可插入文件名替代符）");
		_ItemList.EmptyListMsg = "请使用“添加文件”按钮添加需要处理的 PDF 文件，或从资源管理器拖放文件到本列表框";

		_ConfigButton.Click += (s, args) => AppContext.MainForm.SelectFunctionList(Function.PatcherOptions);

		_AddFilesButton.ButtonClick += (s, args) => { ExecuteCommand(Commands.Open); };
		_AddFilesButton.DropDownOpening += FileListHelper.OpenPdfButtonDropDownOpeningHandler;
		_AddFilesButton.DropDownItemClicked += (s, args) => {
			_RecentFileMenu.Hide();
			ExecuteCommand(Commands.OpenFile, args.ClickedItem.ToolTipText);
		};

		_TargetPdfFile.FileMacroMenu.LoadStandardInfoMacros();
		_TargetPdfFile.FileMacroMenu.LoadStandardSourceFileMacros();
		_TargetPdfFile.BrowseForFile += FileControl_BrowseForFile;
		_TargetPdfFile.TargetFileChangedByBrowseButton += (s, args) => {
			int i;
			string f = _TargetPdfFile.FileDialog.FileName;
			if (_ItemList.Items.Count <= 1 || (i = f.LastIndexOf(Path.DirectorySeparatorChar)) == -1) {
				return;
			}

			_TargetPdfFile.Text = string.Concat(f.Substring(0, i), Path.DirectorySeparatorChar,
				Constants.FileNameMacros.FileName, Path.GetExtension(f));
			args.Cancel = true;
		};
		ImageList.ImageCollection fi = _FileTypeList.Images;
		fi.AddRange(new Image[] { Resources.OriginalPdfFile });

		_ItemList.FixEditControlWidth();
		_ItemList.ListViewItemSorter = new ListViewItemComparer(0);
		_listHelper = new FileListHelper(_ItemList);
		_listHelper.SetupHotkeys();
		_listHelper.SetupDragAndDrop(AddFiles);
		FileListHelper.SetupCommonPdfColumns(_AuthorColumn, _KeywordsColumn, _SubjectColumn, _TitleColumn,
			_PageCountColumn, _NameColumn, _FolderColumn);
		_RefreshInfoButton.ButtonClick += (s, args) => _listHelper.RefreshInfo(AppContext.Encodings.DocInfoEncoding);
		_RefreshInfoButton.DropDown = _RefreshInfoMenu;
		foreach (string item in Constants.Encoding.EncodingNames) {
			_RefreshInfoMenu.Items.Add(item);
		}

		_RefreshInfoMenu.ItemClicked += (s, args) => {
			_listHelper.RefreshInfo(ValueHelper.MapValue(args.ClickedItem.Text, Constants.Encoding.EncodingNames,
				Constants.Encoding.Encodings));
		};

		RecentFileItemClicked = (s, args) => {
			args.ClickedItem.Owner.Hide();
			AddFiles(new[] { args.ClickedItem.ToolTipText }, true);
		};
	}

	public override void ExecuteCommand(string commandName, params string[] parameters) {
		switch (commandName) {
			case Commands.Open: {
					OpenFileDialog b = _OpenPdfBox;
					if (b.ShowDialog() == DialogResult.OK) {
						AddFiles(b.FileNames, true);
					}

					break;
				}
			case Commands.OpenFile:
				AddFiles(parameters, true);
				break;
			default: {
					if (_listHelper.ProcessCommonMenuCommand(commandName) == false) {
						base.ExecuteCommand(commandName, parameters);
					}

					break;
				}
		}
	}

	public override void SetupCommand(ToolStripItem item) {
		string n = item.Name;
		if (Commands.CommonSelectionCommands.Contains(n) || n == Commands.Delete) {
			item.Enabled = _ItemList.GetItemCount() > 0 && _ItemList.Focused;
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

		switch (files.Length) {
			case > 3:
				AppContext.MainForm.Enabled = false;
				break;
			case 0:
				return;
		}

		_AddDocumentWorker.RunWorkerAsync(files);
	}

	private void _ImportButton_Click(object sender, EventArgs e) {
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
		_TargetPdfFile.FileList.AddHistoryItem();

		AppContext.MainForm.ResetWorker();
		BackgroundWorker worker = AppContext.MainForm.GetWorker();
		worker.DoWork += (dummy, arg) => {
			object[] a = arg.Argument as object[];
			string t = a[0] as string;
			if (files.Count > 1) {
				string targetFolder = null;
				bool m = FileHelper.HasFileNameMacro(t); // 包含替换符
				if (m == false) {
					targetFolder = Path.GetDirectoryName(t);
				}

				Tracker.SetTotalProgressGoal(files.Count);
				foreach (SourceItem file in files) {
					if (file.Type == SourceItem.ItemType.Pdf) {
						// 确定信息文件名
						// 优先采用与输入文件同名的 XML 信息文件
						FilePath f = new(FileHelper.CombinePath(file.FolderName,
							Path.ChangeExtension(file.FileName, Constants.FileExtensions.Xml)));
						if (f.ExistsFile == false) {
							// 次之采用与输入文件同名的 TXT 信息文件
							f = f.ChangeExtension(Constants.FileExtensions.Txt);
							if (f.ExistsFile == false) {
								// 次之采用同一个信息文件
								f = file.FilePath.ChangeExtension(Constants.FileExtensions.Xml);
								if (f.ExistsFile == false) {
									f = FilePath.Empty;
								}
							}
						}

						Worker.PatchDocument(file as SourceItem.Pdf,
							m ? t : FileHelper.CombinePath(targetFolder, file.FilePath.FileName),
							f.ToString(),
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
		worker.RunWorkerAsync(new object[] { targetPdfFile, null });
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
		_ItemList.ListViewItemSorter = e.ClickedItem.Name switch {
			"_SortByAlphaItem" => new ListViewItemComparer(0, false),
			"_SortByNaturalNumberItem" => new ListViewItemComparer(0, true),
			_ => _ItemList.ListViewItemSorter
		};
	}

	private void _ImageList_ColumnClick(object sender, ColumnClickEventArgs e) {
		int c = e.Column;
		bool ss = c == 0 || c == _PageCountColumn.Index;
		SortOrder o = _ItemList.PrimarySortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
		_ItemList.ListViewItemSorter = new ListViewItemComparer(e.Column, ss, o);
	}

	private void _MainToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		_listHelper.ProcessCommonMenuCommand(e.ClickedItem.Name);
	}

	#region AddDocumentWorker

	private void _AddDocumentWorker_DoWork(object sender, DoWorkEventArgs e) {
		string[] files = e.Argument as string[];
		Array.ForEach(files, f => {
			((BackgroundWorker)sender).ReportProgress(0, f);
		});
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