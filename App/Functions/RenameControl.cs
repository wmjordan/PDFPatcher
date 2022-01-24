using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	[ToolboxItem(false)]
	public partial class RenameControl : FunctionControl
	{
		FileListHelper _listHelper;
		static readonly string[] __EnabledCommands = { Commands.Copy, Commands.Delete };

		public override string FunctionName => "重命名文件";

		public override System.Drawing.Bitmap IconImage => Properties.Resources.Rename;

		public RenameControl() {
			InitializeComponent();
		}

		private void PatcherControl_OnLoad(object sender, EventArgs e) {
			//Icon = FormHelper.ToIcon (Properties.Resources.CreateDocument);
			_ItemList.ListViewItemSorter = new ListViewItemComparer(0);

			AppContext.MainForm.SetTooltip(_ItemList, "在此添加需要重命名的 PDF 文件");
			AppContext.MainForm.SetTooltip(_RenameButton, "点击此按钮根据文件属性和输出文件名将 PDF 文件重命名");
			AppContext.MainForm.SetTooltip(_TargetPdfFile.FileList, "生成的目标 PDF 文件路径（鼠标右键点击列表可插入文件名替代符）");
			_ItemList.EmptyListMsg = "请使用“添加文件”按钮添加需要处理的 PDF 文件，或从资源管理器拖放文件到本列表框";

			_TargetPdfFile.FileMacroMenu.LoadStandardInfoMacros();
			_TargetPdfFile.FileMacroMenu.LoadStandardSourceFileMacros();
			_TargetPdfFile.BrowseForFile += new EventHandler<EventArgs>(FileControl_BrowseForFile);
			_TargetPdfFile.TargetFileChangedByBrowseButton += (s, args) => {
				int i;
				var f = _TargetPdfFile.FileDialog.FileName;
				if (_ItemList.Items.Count > 1 && (i = f.LastIndexOf(Path.DirectorySeparatorChar)) != -1) {
					_TargetPdfFile.Text = String.Concat(f.Substring(0, i), Path.DirectorySeparatorChar, Constants.FileNameMacros.FileName, Path.GetExtension(f));
					args.Cancel = true;
				}
			};
			var fi = _FileTypeList.Images;
			fi.AddRange(new System.Drawing.Image[] {
				Properties.Resources.OriginalPdfFile
			});
			_ItemList.FixEditControlWidth();
			_listHelper = new FileListHelper(_ItemList);
			_listHelper.SetupDragAndDrop(AddFiles);
			_listHelper.SetupHotkeys();
			FileListHelper.SetupCommonPdfColumns(_AuthorColumn, _KeywordsColumn, _SubjectColumn, _TitleColumn, _PageCountColumn, _NameColumn, _FolderColumn);
			_RefreshInfoButton.ButtonClick += (s, args) => _listHelper.RefreshInfo(AppContext.Encodings.DocInfoEncoding);
			_RefreshInfoButton.DropDown = _RefreshInfoMenu;
			foreach (var item in Constants.Encoding.EncodingNames) {
				_RefreshInfoMenu.Items.Add(item);
			}
			_RefreshInfoMenu.ItemClicked += (s, args) => _listHelper.RefreshInfo(ValueHelper.MapValue(args.ClickedItem.Text, Constants.Encoding.EncodingNames, Constants.Encoding.Encodings));
			_AddFilesButton.DropDownOpening += FileListHelper.OpenPdfButtonDropDownOpeningHandler;
			_AddFilesButton.DropDownItemClicked += (s, args) => {
				args.ClickedItem.Owner.Hide();
				ExecuteCommand(Commands.OpenFile, args.ClickedItem.ToolTipText);
			};
			RecentFileItemClicked += (s, args) => ExecuteCommand(Commands.OpenFile, args.ClickedItem.ToolTipText);
		}

		public override void SetupCommand(ToolStripItem item) {
			if (__EnabledCommands.Contains(item.Name)
				|| Commands.CommonSelectionCommands.Contains(item.Name)) {
				EnableCommand(item, _ItemList.GetItemCount() > 0 && _ItemList.Focused, true);
			}
			base.SetupCommand(item);
		}

		public override void ExecuteCommand(string commandName, params string[] parameters) {
			if (_listHelper.ProcessCommonMenuCommand(commandName)) {
				return;
			}
			switch (commandName) {
				case Commands.Open:
					var b = _OpenPdfBox;
					_AddFilesButton.DropDown.Items.ClearDropDownItems();
					if (b.ShowDialog() == DialogResult.OK) {
						AddFiles(b.FileNames, true);
					}
					break;
				case Commands.OpenFile:
					AddFiles(parameters, true);
					break;
				default:
					break;
			}
			base.ExecuteCommand(commandName, parameters);
		}

		void FileControl_BrowseForFile(object sender, EventArgs e) {
			_listHelper.PrepareSourceFiles();
		}

		private void _RenameButton_Click(object sender, EventArgs e) {
			var targetPdfFile = _TargetPdfFile.Text.Trim();
			if (String.IsNullOrEmpty(targetPdfFile) && String.IsNullOrEmpty(targetPdfFile = _TargetPdfFile.BrowseTargetFile())) {
				FormHelper.ErrorBox(Messages.TargetFileNotSpecified);
				return;
			}

			var l = _ItemList.GetItemCount();
			if (l == 0) {
				FormHelper.InfoBox("请添加需要重命名的 PDF 文件。");
				return;
			}
			var files = GetSourceItemList();
			_TargetPdfFile.FileList.AddHistoryItem();

			AppContext.MainForm.ResetWorker();
			var worker = AppContext.MainForm.GetWorker();
			worker.DoWork += (dummy, arg) => {
				var items = _listHelper.GetSourceItems<SourceItem.Pdf>(false);
				Processor.Worker.RenameFiles(items, targetPdfFile, _KeepSourceFileBox.Checked);
			};
			worker.RunWorkerAsync();
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

		private List<SourceItem> GetSourceItemList() {
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
			var c = e.Column;
			var ss = c == 0 || c == _PageCountColumn.Index;
			var o = _ItemList.PrimarySortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
			_ItemList.ListViewItemSorter = new ListViewItemComparer(e.Column, ss, o);
		}

		private void _MainToolbar_ButtonClick(object sender, EventArgs e) {
			if (sender == _AddFilesButton) {
				ExecuteCommand(Commands.Open);
			}
		}

		private void _MainToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			ExecuteCommand(e.ClickedItem.Name);
		}

		private void _TestRenameButton_Click(object sender, EventArgs e) {
			if (String.IsNullOrEmpty(_TargetPdfFile.Text)) {
				FormHelper.ErrorBox(Messages.TargetFileNotSpecified);
				return;
			}
			var pdfs = _listHelper.GetSourceItems<SourceItem.Pdf>(false);
			if (pdfs.Count == 0) {
				FormHelper.InfoBox("请添加需要重命名的 PDF 文件。");
				return;
			}

			PreviewRename(pdfs, _TargetPdfFile.Text);
		}

		private void PreviewRename(List<SourceItem.Pdf> items, string template) {
			var i = 0;
			var result = new string[items.Count];
			var source = new string[items.Count];
			FilePath s;
			string t;
			foreach (var item in items) {
				try {
					s = item.FilePath;
					if (s.ExistsFile == false) {
						t = String.Concat("(找不到 PDF 文件：", s, ")");
						continue;
					}
					else {
						t = Processor.Worker.GetExpandedFileName(item, template);
						if (t.Length == 0) {
							t = "<输出文件名无效>";
						}
						else if (Path.GetFileName(t).Length == 0) {
							t = "<输出文件名为空>";
						}
					}
					source[i] = s.ToString();
					result[i] = t;
					i++;
				}
				catch (Exception ex) {
					FormHelper.ErrorBox(ex.Message);
				}
			}
			using (var f = new Functions.RenamePreviewForm(source, result)) {
				f.ShowDialog();
			}
		}

		#region AddDocumentWorker
		private void _AddDocumentWorker_DoWork(object sender, DoWorkEventArgs e) {
			var files = e.Argument as string[];
			Array.ForEach(files, f => ((BackgroundWorker)sender).ReportProgress(0, f));
		}

		private void _AddDocumentWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			AppContext.MainForm.Enabled = true;
			//_listHelper.ResizeItemListColumns ();
		}

		private void _AddDocumentWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			var item = e.UserState as string;
			AddItem(SourceItem.Create(item));
		}

		private void AddItem(SourceItem item) {
			if (item == null) {
				return;
			}
			AddItems(new SourceItem[] { item });
		}

		private void AddItems(System.Collections.ICollection items) {
			var i = _ItemList.GetLastSelectedIndex();
			_ItemList.InsertObjects(++i, items);
			_ItemList.SelectedIndex = --i + items.Count;
		}
		#endregion

		#region IDefaultButtonControl 成员

		public override Button DefaultButton => _RenameButton;

		#endregion

	}
}
