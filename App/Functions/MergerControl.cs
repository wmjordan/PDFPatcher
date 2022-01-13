using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;
using PDFPatcher.Properties;

namespace PDFPatcher.Functions;

[ToolboxItem(false)]
public partial class MergerControl : FunctionControl
{
	private readonly string[] _bookmarkStyleButtonNames;
	private readonly SourceItem _itemsContainer = new SourceItem.Empty();
	private FileListHelper _listHelper;

	public MergerControl() {
		InitializeComponent();
		_bookmarkStyleButtonNames = new[] { "_BoldStyleButton", "_BookmarkColorButton", "_ItalicStyleButton" };
	}

	public override string FunctionName => "合并文档";

	public override Bitmap IconImage => Resources.Merger;

	#region IDefaultButtonControl 成员

	public override Button DefaultButton => _ImportButton;

	#endregion

	private void MergerControl_Load(object sender, EventArgs args) {
		//this.Icon = Common.FormHelper.ToIcon (Properties.Resources.CreateDocument);
		//_MainToolbar.ToggleEnabled (false, _bookmarkStyleButtonNames);
		_BookmarkColorButton.SelectedColorChanged += (s, e) => { RefreshBookmarkColor(); };

		AppContext.MainForm.SetTooltip(_BookmarkControl.FileList, "为目标 PDF 文件添加书签的信息文件（可选）");
		AppContext.MainForm.SetTooltip(_ItemList, "在此添加需要合并的 PDF 文件、图片文件或包含上述类型文件的文件夹");
		AppContext.MainForm.SetTooltip(_ImportButton, "点击此按钮将列表的文件合并为一个 PDF 文件");
		AppContext.MainForm.SetTooltip(_TargetPdfFile.FileList, "生成的目标 PDF 文件路径");
		_ItemList.EmptyListMsg = "请使用“添加文件”按钮添加需要合并的文件，或从资源管理器拖放文件到本列表框";

		ImageList.ImageCollection fi = _FileTypeList.Images;
		fi.AddRange(
			new Image[] { Resources.EmptyPage, Resources.OriginalPdfFile, Resources.Image, Resources.ImageFolder });

		_BookmarkControl.FileDialog.CheckFileExists = false;
		_BookmarkControl.BrowseForFile += FileControl_BrowseForFile;
		_TargetPdfFile.BrowseForFile += FileControl_BrowseForFile;
		_IndividualMergerModeBox.CheckedChanged += (s, e) => {
			_BookmarkControl.Enabled = !_IndividualMergerModeBox.Checked;
		};
		_listHelper = new FileListHelper(_ItemList);
		_ItemList.FixEditControlWidth();
		_ItemList.BeforeSorting += (s, e) => e.Canceled = true;
		_ItemList.CanExpandGetter = x => ((SourceItem)x).HasSubItems;
		_ItemList.ChildrenGetter = x => ((SourceItem)x).Items;
		_ItemList.SelectedIndexChanged += (s, e) => {
			int i = _ItemList.GetFirstSelectedIndex();
			bool en = false;
			if (i != -1) {
				BookmarkSettings b = (_ItemList.GetModelObject(i) as SourceItem).Bookmark;
				if (b != null && string.IsNullOrEmpty(b.Title) == false) {
					en = true;
					_BoldStyleButton.Checked = b.IsBold;
					_ItalicStyleButton.Checked = b.IsItalic;
				}
			}

			_MainToolbar.ToggleEnabled(en, _bookmarkStyleButtonNames);
		};
		_ItemList.CellEditStarting += (s, e) => _MainToolbar.Enabled = false;
		_ItemList.CellEditFinishing += (s, e) => _MainToolbar.Enabled = true;
		_ItemList.CanDrop += ItemList_CanDropFile;
		_ItemList.Dropped += ItemList_FileDropped;
		_ItemList.ModelCanDrop += ItemList_CanDropModel;
		_ItemList.ModelDropped += ItemList_Dropped;
		_ItemListMenu.Opening += (s, e) => {
			foreach (ToolStripItem item in _ItemListMenu.Items) {
				SetupCommand(item);
			}
		};
		_ItemList.AsTyped<SourceItem>()
			.ConfigColumn(_NameColumn, c => {
				c.AspectGetter = o => o.FileName ?? "<空白页>";
				c.ImageGetter = o => (int)o.Type;
			})
			.ConfigColumn(_BookmarkColumn, c => {
				c.AspectGetter = o => o.Bookmark != null ? o.Bookmark.Title : null;
				c.AspectPutter = (o, v) => {
					string s = v as string;
					if (string.IsNullOrEmpty(s)) {
						o.Bookmark = null;
					}
					else if (o.Bookmark == null) {
						o.Bookmark = new BookmarkSettings(s);
					}
					else {
						o.Bookmark.Title = s;
					}
				};
			})
			.ConfigColumn(_PageRangeColumn, c => {
				c.AspectGetter = o => (o as SourceItem.Pdf)?.PageRanges;
				c.AspectPutter = (o, v) => {
					if (o is SourceItem.Pdf p) {
						p.PageRanges = v as string;
						if (string.IsNullOrEmpty(p.PageRanges)) {
							p.PageRanges = "1-" + p.PageCount.ToText();
						}
					}
				};
			})
			.ConfigColumn(_FolderColumn, c => c.AspectGetter = o => o.FolderName);
		_AddFilesButton.DropDownOpening += FileListHelper.OpenPdfButtonDropDownOpeningHandler;
		_AddFilesButton.DropDownItemClicked += (s, e) => {
			_RecentFileMenu.Hide();
			ExecuteCommand(Commands.OpenFile, e.ClickedItem.ToolTipText);
		};
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
		if (_ItemList.IsCellEditing || _ItemList.Focused == false) {
			return base.ProcessCmdKey(ref msg, keyData);
		}

		switch (keyData) {
			case Keys.Delete:
				ExecuteCommand(Commands.Delete);
				return true;
			case Keys.Control | Keys.B:
				_BoldStyleButton.PerformClick();
				return true;
			case Keys.Control | Keys.I:
				_ItalicStyleButton.PerformClick();
				return true;
			default:
				return base.ProcessCmdKey(ref msg, keyData);
		}
	}

	public override void SetupCommand(ToolStripItem item) {
		string n = item.Name;
		if (item.OwnerItem != null && item.OwnerItem.Name == Commands.Selection) {
			EnableCommand(item, _ItemList.GetItemCount() > 0 && _ItemList.Focused, true);
		}
		else if (n.StartsWith(Commands.Copy, StringComparison.Ordinal) ||
				 n.StartsWith(Commands.Paste, StringComparison.Ordinal) || n == Commands.Delete) {
			EnableCommand(item, _ItemList.GetItemCount() > 0 && _ItemList.GetFirstSelectedIndex() > -1, true);
		}
		else if (n == Commands.Options) {
			item.Text = "合并文档设置(&S)...";
			item.ToolTipText = "设置合并后的 PDF 文档";
			EnableCommand(item, true, true);
			item.Tag = nameof(Function.MergerOptions);
		}

		base.SetupCommand(item);
	}

	private void FileControl_BrowseForFile(object sender, EventArgs e) {
		_listHelper.PrepareSourceFiles();
	}

	/// <summary>
	///     复制或移动书签。
	/// </summary>
	/// <param name="source">需要复制或移动的源书签。</param>
	/// <param name="target">目标书签。</param>
	/// <param name="child">是否复制为子节点。</param>
	/// <param name="after">是否复制到后面。</param>
	/// <param name="copy">是否复制书签。</param>
	/// <param name="deepCopy">是否深度复制书签。</param>
	internal void CopyOrMoveElement(List<SourceItem> source, SourceItem target, bool child, bool after, bool copy,
		bool deepCopy) {
		if (copy) {
			List<SourceItem> clones = new(source.Count);
			foreach (SourceItem item in source) {
				clones.Add(item.Clone());
			}

			source = clones;
		}
		else {
			foreach (SourceItem item in source) {
				//_ItemList.Collapse (item);
				GetParentSourceItem(item).Items.Remove(item);
			}

			_ItemList.RemoveObjects(source);
		}

		if (child && target != null) {
			if (after) {
				target.Items.AddRange(source);
			}
			else {
				SourceItem[] a = target.Items.ToArray();
				target.Items.Clear();
				target.Items.AddRange(source);
				target.Items.AddRange(a);
			}

			if (target == _itemsContainer) {
				_ItemList.SetObjects(target.Items);
			}
			else {
				_ItemList.Expand(target);
			}
		}
		else {
			SourceItem p = GetParentSourceItem(target);
			if (after) {
				p.Items.InsertRange(target != null ? p.Items.IndexOf(target) + 1 : p.Items.Count, source);
			}
			else {
				p.Items.InsertRange(target != null ? p.Items.IndexOf(target) : p.Items.Count, source);
			}

			if (p == _itemsContainer) {
				_ItemList.SetObjects(_itemsContainer.Items);
			}
		}

		_ItemList.SelectedObjects = source;
	}

	private SourceItem GetParentSourceItem(SourceItem item) {
		SourceItem p = _ItemList.GetParentModel(item);
		if (p == null) {
			p = _itemsContainer;
		}

		return p;
	}

	private void AddFiles(string[] files) {
		if (files == null || files.Length == 0) {
			return;
		}

		if ((ModifierKeys & Keys.Control) != Keys.None) {
			_ItemList.ClearObjects();
		}

		if (files.Length > 3) {
			AppContext.MainForm.Enabled = false;
		}

		SourceItem.SortFileList(files);
		_AddDocumentWorker.RunWorkerAsync(files);
	}

	private void _AddFolder_DropDownOpening(object sender, EventArgs e) {
		ToolStripItemCollection l = _AddFolderButton.DropDown.Items;
		l.ClearDropDownItems();
		foreach (string item in AppContext.Recent.Folders) {
			if (FileHelper.IsPathValid(item) && string.IsNullOrEmpty(Path.GetFileName(item)) == false) {
				l.Add(FileHelper.GetEllipticPath(item, 50)).ToolTipText = item;
			}
		}
	}

	private void _AddFolderButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		_RecentFolderMenu.Hide();
		string f = e.ClickedItem.ToolTipText;
		if (Directory.Exists(f) == false) {
			FormHelper.ErrorBox("文件夹不存在。");
			return;
		}

		ExecuteCommand(Commands.OpenFile, f);
	}

	private void _ImportButton_Click(object sender, EventArgs e) {
		string infoFile = _BookmarkControl.Text.Trim();
		string targetPdfFile = _TargetPdfFile.Text.Trim();
		if (string.IsNullOrEmpty(targetPdfFile) &&
			string.IsNullOrEmpty(targetPdfFile = _TargetPdfFile.BrowseTargetFile())) {
			FormHelper.ErrorBox(Messages.TargetFileNotSpecified);
			return;
		}

		if (FileHelper.IsPathValid(targetPdfFile) == false) {
			FormHelper.ErrorBox("输出文件名无效。" +
								(FileHelper.HasFileNameMacro(targetPdfFile)
									? "\n合并 PDF 文件功能不支持替代符。"
									: string.Empty));
			return;
		}

		//for (int i = _ItemList.GetItemCount () - 1; i >= 0; i--) {
		//    var f = _ItemList.GetModelObject (i) as SourceItem;
		//    if (f.Type == SourceItem.ItemType.Pdf) {
		//        AppContext.Recent.AddHistoryItem (AppContext.Recent.SourcePdfFiles, f.FilePath);
		//    }
		//}
		int l = _ItemList.GetItemCount();
		if (l == 0) {
			FormHelper.InfoBox("请添加用于生成 PDF 文件的图片或 PDF 源文件。");
			return;
		}
		//var si = new List<SourceItem> (l);
		//for (int i = 0; i < l; i++) {
		//    var item = _ItemList.GetModelObject (_ItemList.GetNthItemInDisplayOrder (i).Index) as SourceItem;
		//    //if (item.Type == SourceFileItem.ItemType.Pdf && item.Path.EndsWith (Constants.FileExtensions.Pdf, StringComparison.OrdinalIgnoreCase)) {
		//    //    ContextData.Recent.AddHistoryItem (ContextData.Recent.SourcePdfFiles, item.Path);
		//    //}
		//    si.Add (item);
		//}

		_BookmarkControl.FileList.AddHistoryItem();
		_TargetPdfFile.FileList.AddHistoryItem();
		bool fm = _IndividualMergerModeBox.Checked;
		List<SourceItem> fl = fm ? new List<SourceItem>(_itemsContainer.Items.Count) : _itemsContainer.Items;
		if (fm) {
			foreach (SourceItem item in _itemsContainer.Items) {
				if (item.HasSubItems) {
					fl.Add(item);
				}
			}

			if (fl.Count == 0) {
				Tracker.TraceMessage(Tracker.Category.Error, "合并文件列表没有包含子项的首层项目。");
			}
		}

		AppContext.MainForm.ResetWorker();
		BackgroundWorker worker = AppContext.MainForm.GetWorker();

		worker.DoWork += (dummy, arg) => {
			object[] args = arg.Argument as object[];
			ICollection<SourceItem> items = args[0] as ICollection<SourceItem>;
			string target = args[1] as string;
			if ((bool)args[3]) {
				Tracker.SetTotalProgressGoal(items.Count);
				foreach (SourceItem item in items) {
					string tn = FileHelper.CombinePath(Path.GetDirectoryName(target),
						item.FileName + Constants.FileExtensions.Pdf);
					switch (item.Type) {
						case SourceItem.ItemType.Empty:
							Tracker.TraceMessage(Tracker.Category.Error, "首层项目不能为空白页。");
							break;
						case SourceItem.ItemType.Pdf:
						case SourceItem.ItemType.Image:
							Worker.MergeDocuments(new[] { item }, tn, null);
							break;
						case SourceItem.ItemType.Folder:
							Worker.MergeDocuments(item.Items, tn, null);
							break;
					}

					Tracker.IncrementTotalProgress();
				}
			}
			else {
				Worker.MergeDocuments(items, args[1] as string, args[2] as string);
			}
		};
		worker.RunWorkerAsync(new object[] { fl, targetPdfFile, infoFile, fm });
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

	private void _MainToolbar_ButtonClick(object sender, EventArgs e) {
		if (sender == _AddFilesButton) {
			ExecuteCommand(Commands.Open);
		}
		else if (sender == _ConfigButton) {
			AppContext.MainForm.SelectFunctionList(Function.MergerOptions);
		}
		else if (sender == _AddFolderButton) {
			using (OpenFileDialog f = new() {
				FileName = "【选择目录】",
				Filter = _OpenImageBox.Filter,
				CheckFileExists = false,
				Title = "选择包含图片或 PDF 的文件夹，点击“打开”按钮"
			}) {
				if (f.ShowDialog() == DialogResult.OK) {
					string p = Path.GetDirectoryName(f.FileName);
					if (string.IsNullOrEmpty(Path.GetFileName(p))) {
						FormHelper.ErrorBox("选择的文件夹无效，不允许选择根目录。");
						return;
					}

					ExecuteCommand(Commands.OpenFile, p);
					AppContext.RecentItems.AddHistoryItem(AppContext.Recent.Folders, p);
				}
			}
		}
	}

	private void _MainToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		ExecuteCommand(e.ClickedItem);
	}

	public override void ExecuteCommand(string commandName, params string[] parameters) {
		switch (commandName) {
			case Commands.Open:
				if (_OpenImageBox.ShowDialog() == DialogResult.OK) {
					ExecuteCommand(Commands.OpenFile, _OpenImageBox.FileNames);
				}

				break;
			case Commands.OpenFile:
				AddFiles(parameters);
				break;
			case Commands.LoadList:
				using (OpenFileDialog f = new() {
					Title = "请选择需要打开的文件列表",
					Filter = Constants.FileExtensions.XmlFilter,
					DefaultExt = Constants.FileExtensions.Xml
				}) {
					if (f.ShowDialog() == DialogResult.OK) {
						_ItemList.DeselectAll();
						_ItemList.ClearObjects();
						_itemsContainer.Items.Clear();
						_itemsContainer.Items.AddRange(SourceItemSerializer.Deserialize(f.FileName));
						_ItemList.Objects = _itemsContainer.Items;
					}
				}

				break;
			case Commands.SaveList:
				using (SaveFileDialog f = new() {
					Title = "请输入需要保存文件列表的文件名",
					Filter = Constants.FileExtensions.XmlFilter,
					DefaultExt = Constants.FileExtensions.Xml
				}) {
					if (f.ShowDialog() == DialogResult.OK) {
						SourceItemSerializer.Serialize(_itemsContainer.Items, f.FileName);
					}
				}

				break;
			case Commands.Delete:
				if (_ItemList.GetItemCount() == 0) {
					return;
				}

				IList l = _ItemList.SelectedObjects;
				if (l.Count == 0) {
					if (FormHelper.YesNoBox("是否清空文件列表？") == DialogResult.Yes) {
						_ItemList.ClearObjects();
						_itemsContainer.Items.Clear();
					}
				}
				else {
					foreach (SourceItem item in _ItemList.SelectedObjects) {
						GetParentSourceItem(item).Items.Remove(item);
						_ItemList.RemoveObject(item);
					}
				}

				break;
			case Commands.Copy:
				StringBuilder sb = new(200);
				List<SourceItem> sl = GetSourceItems<SourceItem>(true);
				if (sl.HasContent() == false) {
					sl = GetSourceItems<SourceItem>(false);
					if (sl.HasContent() == false) {
						return;
					}
				}

				foreach (SourceItem item in sl) {
					if (item.Type == SourceItem.ItemType.Empty) {
						continue;
					}

					if (item.Type == SourceItem.ItemType.Pdf) {
						SourceItem.Pdf pi = item as SourceItem.Pdf;
						sb.AppendLine(string.Join("\t", pi.FilePath.ToString(),
							item.Bookmark != null ? item.Bookmark.Title : string.Empty, pi.PageRanges,
							pi.PageCount.ToText()));
					}
					else if (item.Type == SourceItem.ItemType.Image) {
						SourceItem.Image im = item as SourceItem.Image;
						sb.AppendLine(string.Join("\t", im.FilePath.ToString(),
							item.Bookmark != null ? item.Bookmark.Title : string.Empty, "-", im.PageCount.ToText()));
					}
					else if (item.Type == SourceItem.ItemType.Folder) {
						sb.AppendLine(item.FilePath.ToString());
					}
				}

				if (sb.Length > 0) {
					Clipboard.SetText(sb.ToString());
				}

				break;
			case "_InsertEmptyPage":
				AddItem(new SourceItem.Empty());
				break;
			case Commands.SelectAllItems:
				_ItemList.SelectAll();
				break;
			case Commands.SelectNone:
				_ItemList.SelectObjects(null);
				break;
			case Commands.InvertSelectItem:
				_ItemList.InvertSelect();
				break;
			case Commands.SelectAllImages:
				SelectItemsByType(SourceItem.ItemType.Image);
				break;
			case Commands.SelectAllPdf:
				SelectItemsByType(SourceItem.ItemType.Pdf);
				break;
			case Commands.SelectAllFolders:
				SelectItemsByType(SourceItem.ItemType.Folder);
				break;
			case "_EditItemProperty":
			case "_SetPdfOptions":
				_ItemList_ItemActivate(null, null);
				break;
			case "_SetCroppingOptions":
				SetImageCropping();
				break;
			case "_RefreshFolder":
				foreach (SourceItem.Folder item in _ItemList.SelectedObjects) {
					if (item != null) {
						item.Reload();
					}
				}

				_ItemList.RefreshObjects(_ItemList.SelectedObjects);
				break;
			case "_PdfOptions":
				AppContext.MainForm.SelectFunctionList(Function.PatcherOptions);
				break;
			case "_BoldStyleButton":
				bool cb = !_BoldStyleButton.Checked;
				foreach (SourceItem item in _ItemList.SelectedObjects) {
					if (item != null && item.Bookmark != null) {
						item.Bookmark.IsBold = cb;
					}
				}

				goto case "__Refresh";
			case "_ItalicStyleButton":
				bool ci = !_ItalicStyleButton.Checked;
				foreach (SourceItem item in _ItemList.SelectedObjects) {
					if (item != null && item.Bookmark != null) {
						item.Bookmark.IsItalic = ci;
					}
				}

				goto case "__Refresh";
			case "_BookmarkColorButton":
				RefreshBookmarkColor();
				break;
			case "_CopyFileName":
				CopySelectedItems(item => item.FilePath.FileNameWithoutExtension);
				break;
			case "_CopyBookmarkText":
				CopySelectedItems(
					item => item != null && item.Bookmark != null ? item.Bookmark.Title : string.Empty);
				break;
			case "_PasteBookmarkText":
				string ct = Clipboard.GetText(TextDataFormat.UnicodeText);
				if (string.IsNullOrEmpty(ct) || _ItemList.GetItemCount() == 0) {
					break;
				}

				int li = _ItemList.GetLastItemInDisplayOrder().Index;
				using (StringReader sr = new(ct)) {
					int i = _ItemList.GetFirstSelectedIndex();
					if (i == -1) {
						i = 0;
					}

					while (i <= li && sr.Peek() != -1) {
						SourceItem b = _ItemList.GetModelObject(i) as SourceItem;
						if (b != null) {
							if (b.Bookmark == null) {
								b.Bookmark = new BookmarkSettings(sr.ReadLine());
							}
							else {
								b.Bookmark.Title = sr.ReadLine();
							}
						}

						int di = _ItemList.GetDisplayOrderOfItemIndex(i);
						++di;
						OLVListItem ni = _ItemList.GetNthItemInDisplayOrder(di);
						if (ni == null) {
							break;
						}

						i = ni.Index;
					}
				}

				break;
			case "_ClearBookmarkTitle":
				foreach (SourceItem item in _ItemList.SelectedObjects) {
					if (item != null && item.Bookmark != null) {
						item.Bookmark = null;
					}
				}

				goto case "__Refresh";
			case "_SetBookmarkTitle":
				foreach (SourceItem item in _ItemList.SelectedObjects) {
					if (item != null) {
						BookmarkSettings b;
						string t = Path.GetFileNameWithoutExtension(item.FileName);
						if ((b = item.Bookmark) == null) {
							b = item.Bookmark = new BookmarkSettings(t);
						}
						else {
							item.Bookmark.Title = t;
						}
					}
				}

				goto case "__Refresh";
			case "__Refresh":
				_ItemList.RefreshObjects(_ItemList.SelectedObjects);
				break;
		}
	}

	private void CopySelectedItems(Converter<SourceItem, string> converter) {
		StringBuilder bt = new(200);
		foreach (SourceItem item in _ItemList.SelectedObjects) {
			bt.AppendLine(converter(item));
		}

		if (bt.Length > 0) {
			Clipboard.SetText(bt.ToString());
		}
	}

	private void RefreshBookmarkColor() {
		Color sc = _BookmarkColorButton.Color == Color.White
			? Color.Transparent
			: _BookmarkColorButton.Color;
		foreach (SourceItem item in _ItemList.SelectedObjects) {
			if (item != null && item.Bookmark != null) {
				item.Bookmark.ForeColor = sc;
			}
		}

		_ItemList.RefreshObjects(_ItemList.SelectedObjects);
	}

	private void SelectItemsByType(SourceItem.ItemType type) {
		List<SourceItem> r = new();
		SelectItemsByType(type, r, _itemsContainer);
		if (r.Count > 0) {
			_ItemList.SelectedObjects = r;
		}
		else {
			_ItemList.SelectedObjects = null;
		}
	}

	private void SelectItemsByType(SourceItem.ItemType type, List<SourceItem> result, SourceItem container) {
		foreach (SourceItem item in container.Items) {
			if (item.Type == type) {
				_ItemList.Reveal(item, false);
				result.Add(item);
			}

			if (item.HasSubItems) {
				SelectItemsByType(type, result, item);
			}
		}
	}

	private void _ItemList_ItemActivate(object sender, EventArgs e) {
		ListViewItem vi = GetFocusedPdfItem();
		if (vi != null) {
			SourceItem.Pdf pdfItem = _ItemList.GetModelObject(vi.Index) as SourceItem.Pdf;
			using (SourcePdfOptionForm f = new(pdfItem)) {
				if (f.ShowDialog() == DialogResult.OK) {
					_ItemList.RefreshObject(pdfItem);
				}
			}

			return;
		}

		SetImageCropping();
	}

	private void SetImageCropping() {
		IList items = _ItemList.SelectedObjects;
		if (items.Count == 0) {
			return;
		}

		SourceItem.Image s = items[0] as SourceItem.Image;
		int c = 1;
		for (int i = 1; i < items.Count; i++) {
			SourceItem.Image image = items[i] as SourceItem.Image;
			if (image == null || image.Type == SourceItem.ItemType.Pdf) {
				continue;
			}

			if (s == null) {
				s = image;
				continue;
			}

			c++;
			if (s.Cropping.Equals(image.Cropping) == false) {
				if (FormHelper.YesNoBox("选择的图片具有不同的设置，是否重置为统一的值？") == DialogResult.No) {
					return;
				}

				break;
			}
		}

		if (s == null) {
			return;
		}

		SourceItem.Image o = new(c > 1 ? (FilePath)(c + " 个文件") : s.FilePath);
		s.Cropping.CopyTo(o.Cropping);
		using (SourceImageOptionForm f = new(o)) {
			if (f.ShowDialog() == DialogResult.OK) {
				foreach (SourceItem.Image image in items) {
					if (image == null || image.Type == SourceItem.ItemType.Pdf) {
						continue;
					}

					o.Cropping.CopyTo(image.Cropping);
				}
			}
		}
	}

	private ListViewItem GetFocusedPdfItem() {
		ListViewItem vi = _ItemList.FocusedItem;
		if (vi == null
			//|| vi.Selected == false
			|| vi.Text.EndsWith(Constants.FileExtensions.Pdf, StringComparison.OrdinalIgnoreCase) == false) {
			return null;
		}

		return vi;
	}

	private void _ItemListMenu_Opening(object sender, CancelEventArgs e) {
		ListViewItem vi = _ItemList.FocusedItem;
		if (vi == null) {
			_ItemListMenu.ToggleEnabled(false, "_SetPdfOptions", "_RefreshFolder", "_SetCroppingOptions");
			return;
		}

		SourceItem s = _ItemList.GetModelObject(vi.Index) as SourceItem;
		_ItemListMenu.Items["_SetPdfOptions"].Enabled = s.Type == SourceItem.ItemType.Pdf;
		_ItemListMenu.Items["_SetCroppingOptions"].Enabled = s.Type == SourceItem.ItemType.Image;
		_ItemListMenu.Items["_RefreshFolder"].Enabled = s.Type == SourceItem.ItemType.Folder;
	}

	private List<T> GetSourceItems<T>(bool selectedOnly) where T : SourceItem {
		if (_ItemList.GetItemCount() == 0) {
			return null;
		}

		IEnumerable l = selectedOnly ? _ItemList.SelectedObjects : _ItemList.Objects;
		List<T> items = new(selectedOnly ? 10 : _ItemList.GetItemCount());
		SelectItems(l, items);
		return items;
	}

	private static void SelectItems<T>(IEnumerable list, List<T> results) where T : SourceItem {
		foreach (T item in list) {
			if (item == null) {
				continue;
			}

			results.Add(item);
			if (item.HasSubItems) {
				SelectItems(item.Items, results);
			}
		}
	}

	private void _ItemList_FormatRow(object sender, FormatRowEventArgs e) {
		SourceItem si = e.Model as SourceItem;
		BookmarkSettings bs = si.Bookmark;
		if (bs == null) {
			return;
		}

		e.Item.UseItemStyleForSubItems = false;
		e.UseCellFormatEvents = false;
		ListViewItem.ListViewSubItem c = e.Item.SubItems[1];
		c.ForeColor = bs.ForeColor.IsEmptyOrTransparent() ? Color.Black : bs.ForeColor;
		if (bs.IsBold || bs.IsItalic) {
			c.Font = new Font(c.Font,
				(bs.IsBold ? FontStyle.Bold : FontStyle.Regular) |
				(bs.IsItalic ? FontStyle.Italic : FontStyle.Regular));
		}
	}

	#region 拖放操作

	private void ItemList_CanDropFile(object sender, OlvDropEventArgs e) {
		DataObject o = e.DataObject as DataObject;
		if (o == null) {
			return;
		}

		StringCollection f = o.GetFileDropList();
		OLVListItem d = e.DropTargetItem;
		bool child = d != null &&
					 e.MouseLocation.X > d.Position.X + (d.GetBounds(ItemBoundsPortion.ItemOnly).Width / 2);
		bool after = d == null || e.MouseLocation.Y > d.Position.Y + (d.Bounds.Height / 2);
		foreach (string item in f) {
			if (Directory.Exists(item)) {
				e.Handled = true;
				e.Effect = DragDropEffects.Copy;
				e.InfoMessage = string.Concat("添加目录", item, "到", child ? "所有子项" : string.Empty,
					after ? "后面" : "前面");
				return;
			}

			string ext = Path.GetExtension(item).ToLowerInvariant();
			if (ext == Constants.FileExtensions.Pdf ||
				Constants.FileExtensions.AllSupportedImageExtension.Contains(ext)) {
				e.Handled = true;
				e.Effect = DragDropEffects.Copy;
				e.InfoMessage = string.Concat("添加文件", item, "到", child ? "所有子项" : string.Empty,
					after ? "后面" : "前面");
				return;
			}
		}
	}

	private void ItemList_FileDropped(object sender, OlvDropEventArgs e) {
		DataObject o = e.DataObject as DataObject;
		if (o == null) {
			return;
		}

		StringCollection f = o.GetFileDropList();
		string[] fl = new string[f.Count];
		f.CopyTo(fl, 0);
		SourceItem.SortFileList(fl);
		List<SourceItem> sl = new(fl.Length);
		foreach (string item in fl) {
			SourceItem si = SourceItem.Create(item);
			if (si == null) {
				continue;
			}

			sl.Add(si);
		}

		SourceItem ti = e.ListView.GetModelObject(e.DropTargetIndex) as SourceItem;
		OLVListItem d = e.DropTargetItem;
		bool child = d != null &&
					 e.MouseLocation.X > d.Position.X + (d.GetBounds(ItemBoundsPortion.ItemOnly).Width / 2);
		bool after = d != null && e.MouseLocation.Y > d.Position.Y + (d.Bounds.Height / 2);
		CopyOrMoveElement(sl, ti, child, after, false, true);
	}

	private void ItemList_CanDropModel(object sender, ModelDropEventArgs e) {
		IList si = e.SourceModels;
		SourceItem ti = e.TargetModel as SourceItem;
		if (si == null || si.Count == 0 || e.TargetModel == null) {
			e.Effect = DragDropEffects.None;
			return;
		}

		bool copy = (ModifierKeys & Keys.Control) != Keys.None;
		if (copy == false) {
			if (e.DropTargetItem.Selected) {
				e.Effect = DragDropEffects.None;
				return;
			}

			List<SourceItem> al = _ItemList.GetAncestorsOrSelf(ti);
			foreach (SourceItem item in si) {
				if (al.IndexOf(item) != -1) {
					e.Effect = DragDropEffects.None;
					e.InfoMessage = "目标项不能是源项目的子项。";
					return;
				}
			}
		}

		OLVListItem d = e.DropTargetItem;
		Point ml = e.MouseLocation;
		bool child = ml.X > d.Position.X + (d.GetBounds(ItemBoundsPortion.ItemOnly).Width / 2);
		bool append = ml.Y > d.Position.Y + (d.Bounds.Height / 2);
		if (child == false && copy == false) {
			int xi = e.DropTargetIndex + (append ? 1 : -1);
			if (xi > -1 && xi < e.ListView.GetItemCount()
						&& e.ListView.Items[xi].Selected
						&& GetParentSourceItem(ti) ==
						GetParentSourceItem(_ItemList.GetModelObject(xi) as SourceItem)) {
				e.Effect = DragDropEffects.None;
				return;
			}
		}

		e.Effect = copy ? DragDropEffects.Copy : DragDropEffects.Move;
		e.InfoMessage = string.Concat(copy ? "复制" : "移动", "到", child ? "所有子项" : string.Empty,
			append ? "后面" : "前面");
	}

	private void ItemList_Dropped(object sender, ModelDropEventArgs e) {
		SourceItem t = e.TargetModel as SourceItem;
		List<SourceItem> si = (e.SourceListView as TreeListView).GetSelectedModels<SourceItem>();
		if (si == null) {
			return;
		}

		SourceItem ti = e.TargetModel as SourceItem;
		OLVListItem d = e.DropTargetItem;
		bool child = e.MouseLocation.X > d.Position.X + (d.GetBounds(ItemBoundsPortion.ItemOnly).Width / 2);
		bool after = e.MouseLocation.Y > d.Position.Y + (d.Bounds.Height / 2);
		bool copy = (ModifierKeys & Keys.Control) != Keys.None;
		bool deepCopy = copy && (ModifierKeys & Keys.Shift) != Keys.None;
		int tii = _ItemList.TopItemIndex;
		CopyOrMoveElement(si, ti, child, after, copy, deepCopy);
		e.RefreshObjects();
		_ItemList.TopItemIndex = tii;
	}

	#endregion

	#region AddDocumentWorker

	private void _AddDocumentWorker_DoWork(object sender, DoWorkEventArgs e) {
		string[] files = e.Argument as string[];
		Array.ForEach(files, f => {
			((BackgroundWorker)sender).ReportProgress(0, f);
		});
	}

	private void _AddDocumentWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
		AppContext.MainForm.Enabled = true;
		//ResizeItemListColumns ();
	}

	//private void ResizeItemListColumns () {
	//    var c = _ItemList.Columns[0];
	//    _ItemList.AutoResizeColumns (ColumnHeaderAutoResizeStyle.ColumnContent);
	//    if (c.Width < 100) {
	//        c.Width = 100;
	//    }
	//    for (int i = 1; i < _ItemList.Columns.Count; i++) {
	//        c = _ItemList.Columns[i];
	//        if (c.Width < 50) {
	//            c.Width = 50;
	//        }
	//    }
	//}

	private void _AddDocumentWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
		AddItem(SourceItem.Create(e.UserState as string));
	}

	private void AddItem(SourceItem item) {
		if (item == null) {
			return;
		}

		AddItems(new[] { item });
	}

	private void AddItems(ICollection<SourceItem> items) {
		int i = _ItemList.GetLastSelectedIndex();
		if (i == -1) {
			i = _ItemList.FocusedItem != null ? _ItemList.FocusedItem.Index : -1;
		}

		if (i == -1) {
			_itemsContainer.Items.AddRange(items);
			_ItemList.Objects = _itemsContainer.Items;
			_ItemList.SelectedObjects = new List<SourceItem>(items);
			return;
		}

		SourceItem m = _ItemList.GetModelObject(i) as SourceItem;
		SourceItem p = _ItemList.GetParentModel(m);
		if (p == null) {
			i = _itemsContainer.Items.IndexOf(m);
			_itemsContainer.Items.InsertRange(++i, items);
			_ItemList.Objects = _itemsContainer.Items;
			_ItemList.SelectedObjects = new List<SourceItem>(items);
			_ItemList.RebuildAll(true);
			return;
		}

		i = p.Items.IndexOf(m);
		p.Items.InsertRange(++i, items);
		_ItemList.RefreshObject(p);
		_ItemList.SelectedObjects = new List<SourceItem>(items);
	}

	#endregion
}