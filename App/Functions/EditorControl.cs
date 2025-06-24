﻿using System;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using BrightIdeasSoftware;
using MuPDF.Extensions;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions
{
	public sealed partial class EditorControl : FunctionControl, IDocumentEditor, Editor.IEditView
	{
		static readonly Color __DarkModeColor = Color.DarkGray;
		static readonly Color __GreenModeColor = Color.FromArgb(0xCC, 0xFF, 0xCC);

		SearchBookmarkForm _searchForm;
		AutoBookmarkForm _autoBookmarkForm;
		readonly Editor.Controller _controller;

		public EditorControl() {
			InitializeComponent();
			_controller = new Editor.Controller(this);
			this.OnFirstLoad(OnLoad);
		}

		public override string FunctionName => "文档编辑器";

		public override Bitmap IconImage => Properties.Resources.Editor;

		public string DocumentPath {
			get => _controller?.Model.DocumentPath;
			set {
				_controller.Model.DocumentPath = value;
				DocumentChanged?.Invoke(this, new DocumentChangedEventArgs(value));
			}
		}
		public bool IsBusy => _controller.IsBusy;
		public bool IsDirty => _controller.Model.Undo.IsDirty;

		public event EventHandler<DocumentChangedEventArgs> DocumentChanged;

		void OnLoad() {
			new Editor.Parts.BookmarkInViewSynchronizer(_BookmarkBox, _ViewerBox);
			new Editor.Parts.BookmarkTitleEditHandler(_controller);
			ListRecentFiles = _OpenButton_DropDownOpening;
			RecentFileItemClicked = _OpenButton_DropDownItemClicked;
			var s = this.GetDpiScale();
			var size = new Size((int)(s * 16), (int)(s * 16));
			_BookmarkToolbar.ScaleIcons(size);
			_ViewerToolbar.ScaleIcons(size);
			_EditMenu.ScaleIcons(size);
			_RecentFileMenu.ScaleIcons(size);
			_SelectionMenu.ScaleIcons(size);
			_UndoMenu.ScaleIcons(size);
			_ViewerMenu.ScaleIcons(size);
			_BookmarkBox.ScaleColumnWidths(s);
			_ViewerToolbar.Left = _BookmarkToolbar.Right;
			_MainPanel.SplitterDistance = (int)(_MainPanel.SplitterDistance * FormHelper.GetDpiScale(this));
			_MainPanel.FixedPanel = FixedPanel.Panel1;
			//_MainToolbar.ToggleEnabled (false, _editButtonNames);

			CreateChangeZoomRateItems();

			_ChangeZoomRate.DropDownItemClicked += _MainToolbar_ItemClicked;
			_ChangeCase.DropDownItemClicked += (object s, ToolStripItemClickedEventArgs args) => {
				args.ClickedItem.HidePopupMenu();
				_EditMenu.Hide();
				var i = Array.IndexOf(SetCaseProcessor.CaseNames, args.ClickedItem.Text);
				if (i != -1) {
					_controller.ProcessBookmarks(new SetCaseProcessor((SetCaseProcessor.LetterCase)i));
				}
			};
			foreach (int item in Enum.GetValues(typeof(SetCaseProcessor.LetterCase))) {
				_ChangeCase.DropDownItems.Add(SetCaseProcessor.CaseNames[item]);
			}
			_SetOpenStatus.DropDownItemClicked += _MainToolbar_ItemClicked;

			AppContext.MainForm.SetTooltip(_IncludeDecendantBox, "选中此选项后，加粗、斜体等其它修改书签的操作将应用到选中书签的子书签");
			_IncludeDecendantBox.CheckedChanged += (s, args) => _BookmarkBox.OperationAffectsDescendants = _IncludeDecendantBox.Checked;

			_UndoButton.DropDownOpening += (object s, EventArgs args) => {
				var mi = _UndoMenu.Items;
				mi.Clear();
				foreach (var item in _controller.Model.Undo.GetActionNames(16)) {
					mi.Add(item);
				}
			};
			_UndoButton.DropDownItemClicked += (s, args) => _controller.Undo(args.ClickedItem.Owner.Items.IndexOf(args.ClickedItem) + 1);
			Editor.QuickSelectCommand.RegisterMenuItems(_QuickSelect.DropDownItems);
			_CurrentPageBox.KeyUp += (s, args) => {
				int d;
				switch (args.KeyCode) {
					case Keys.Enter:
						d = 0;
						break;
					case Keys.Up:
					case Keys.OemMinus:
						d = -1;
						break;
					case Keys.Down:
					case Keys.Add:
						d = 1;
						break;
					case Keys.Home:
						_ViewerBox.CurrentPageNumber = 1;
						return;
					case Keys.End:
						_ViewerBox.CurrentPageNumber = -1;
						return;
					default:
						return;
				}
				if (_CurrentPageBox.Text.TryParse(out int p)) {
					_ViewerBox.CurrentPageNumber = p + d;
				}
			};
			_ViewerButton.DropDownOpening += (s, args) => SetupMenu(_ViewerButton.DropDownItems);
			_OcrMenu.DropDownItemClicked += (s, args) => _ViewerBox.OcrLanguage = (int)(args.ClickedItem.Tag ?? 0);
			_OcrMenu.DropDownOpening += (s, args) => {
				var m = _OcrMenu.DropDownItems;
				if (m.Count == 1) {
					for (int i = 0; i < Constants.Ocr.LangIDs.Length; i++) {
						var item = new ToolStripMenuItem(Constants.Ocr.LangNames[i]);
						m.Add(item);
						item.Tag = Constants.Ocr.LangIDs[i];
						item.Enabled = ModiOcr.IsLanguageInstalled(Constants.Ocr.LangIDs[i]);
					}
				}
				foreach (ToolStripMenuItem item in _OcrMenu.DropDownItems) {
					item.Checked = _ViewerBox.OcrLanguage == (int)(item.Tag ?? 0);
				}
			};
			_ZoomBox.Enabled = false;
			_ZoomBox.TextChanged += (s, args) => _ViewerBox.LiteralZoom = _ZoomBox.Text;
			_ViewerBox.Enabled = false;
			_ViewerBox.DocumentLoaded += _ViewerBoxInitializeAfterDocumentLoad;
			_ViewerBox.ZoomChanged += (s, args) => {
				_ZoomBox.ToolTipText = "当前显示比例：" + (_ViewerBox.ZoomFactor * 100).ToInt32() + "%";
				AppContext.Reader.Zoom = _ViewerBox.LiteralZoom;
				_ZoomBox.Text = _ViewerBox.LiteralZoom;
			};
			_ViewerBox.PageChanged += (s, args) => _CurrentPageBox.Text = _ViewerBox.CurrentPageNumber.ToText();
			_ViewerBox.ContentDirectionChanged += (s, args) => AppContext.Reader.ContentDirection = ((ViewerControl)s).ContentDirection;
			_ViewerBox.PageScrollModeChanged += (s, args) => AppContext.Reader.FullPageScroll = ((ViewerControl)s).FullPageScroll;
			//_ViewerBox.SelectionChanged += (s, args) =>
			//{
			//	var t = args.Selection.SelectedText;
			//	if (String.IsNullOrEmpty (t) == false) {
			//		var p = _ViewerBox.ViewBox.SelectionRegion;
			//        ShowInsertBookmarkDialog (_ViewerBox.ViewBox.PointToClient (MousePosition), _ViewerBox.MapPositionFromImagePoint (p.Left.ToInt32 (), p.Top.ToInt32 ()), t);
			//	}
			//};
			//_ViewerBox.MouseMode = Editor.MouseMode.Selection;
			_ViewerBox.MouseMove += (s, args) => {
				if (_ViewerBox.FirstPage == 0) {
					return;
				}
				var l = args.Location;
				var p = _ViewerBox.TransposeClientToPagePosition(l.X, l.Y);
				if (p.Page == 0) {
					return;
				}
				var ti = _ViewerBox.FindTextLines(p);
				var t = ti.ToString();
				_PageInfoBox.Text = string.Concat("页面：",
					p.Page,
					"; 位置：",
					Math.Round(p.PageX, 2),
					" * ",
					Math.Round(p.PageY, 2),
					ti.Spans.HasContent() ? String.Concat("; 字体：", String.Join(";", ti.GetFontNames()), " ", ti.Spans[0].Size) : null,
					t != null ? "; 文本：" : null,
					t);
			};
			_ViewerBox.MouseClick += _ViewBox_MouseClick;
			_ViewerToolbar.Enabled = false;

			Disposed += (s, args) => _controller.Destroy();
		}

		private void CreateChangeZoomRateItems() {
			var di = _ChangeZoomRate.DropDownItems;
			di.AddRange(Array.ConvertAll(Constants.DestinationAttributes.ViewType.Names, n => new ToolStripMenuItem { Name = n, Text = n }));
			di.RemoveByKey(Constants.DestinationAttributes.ViewType.FitR);
			di[0].Text += "...";
			di.Insert(0, new ToolStripMenuItem { Name = Constants.Coordinates.Unchanged, Text = Constants.Coordinates.Unchanged });
			for (int i = 0; i < di.Count; i++) {
				di[i].Text += $"(&{i})";
			}
		}

		void _ViewerBoxInitializeAfterDocumentLoad(object sender, EventArgs e) {
			_ViewerBox.ContentDirection = AppContext.Reader.ContentDirection;
			_ViewerBox.FullPageScroll = AppContext.Reader.FullPageScroll;
			_CurrentPageBox.ToolTipText = $"文档共{_ViewerBox.Document.PageCount}页\nHome：转到第一页\nEnd：转到最后一页";
			_ZoomBox.Text = _ViewerBox.LiteralZoom = AppContext.Reader.Zoom.SubstituteDefault(Constants.DestinationAttributes.ViewType.FitH);
			_ZoomBox.Enabled = true;
		}

		//protected override void OnClick (EventArgs e) {
		//	base.OnClick (e);
		//	_controller.HideInsertBookmarkForm ();
		//}
		internal override void OnDeselected() {
			base.OnDeselected();
			_searchForm?.Close();
		}

		void _ViewBox_MouseClick(object sender, MouseEventArgs args) {
			if (_ViewerBox.FirstPage == 0) {
				return;
			}
			var l = args.Location;
			if (args.Button != MouseButtons.Right) {
				return;
			}

			_ViewerBox.PinPoint = _ViewerBox.PointToImage(l);
			SetupMenu(_ViewerMenu.Items);
			_ViewerMenu.Show(_ViewerBox, l);
			if (!_ViewerBox.IsClientPointInSelection(l)) {
				_ViewerBox.SelectNone();
			}
			//_ViewerBox.Invalidate ();
			//var sp = _ViewerBox.FindTextSpanAtPoint (p);
			//var t = sp != null ? sp.Text : String.Empty;
			//_controller.ShowInsertBookmarkDialog (l, p, t);
		}

		void _MainToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			e.ClickedItem.HidePopupMenu();
			ExecuteCommand(e.ClickedItem.Name);
		}

		void ButtonClicked(object sender, EventArgs e) {
			if (sender == _UndoButton) {
				_controller.Undo(1);
			}
			else if (sender == _AddFilesButton) {
				ExecuteCommand(Commands.Open);
			}
		}

		public override void SetupCommand(ToolStripItem item) {
			var n = item.Name;
			var l = _controller.Model.DocumentPath != null;
			var m = item as ToolStripMenuItem;
			if (_controller.Model.IsLoadingDocument) {
				item.Enabled = false;
				return;
			}
			switch (n) {
				case Commands.Action:
					EnableCommand(item, l, true);
					item.ToolTipText = "将书签和编辑结果写入到 PDF 文件中";
					break;
				case Commands.SaveBookmark:
				case Commands.SaveAsInfoFile:
					EnableCommand(item, l, true);
					break;
				case Commands.SelectAllFolders:
				case Commands.SelectAllImages:
				case Commands.SelectAllPdf:
				case Commands.ItemTypeSeparator:
					EnableCommand(item, false, false);
					break;
				case Commands.ResetOptions:
					// keep disabled
					break;
				case Commands.DocumentProperties:
					item.Enabled = _ViewerBox.Document != null && !_ViewerBox.Document.IsDisposed;
					item.Visible = true;
					break;
				case "_ScrollVertical": m.Checked = _ViewerBox.ContentDirection == Editor.ContentDirection.TopToDown; break;
				case "_ScrollHorizontal": m.Checked = _ViewerBox.ContentDirection == Editor.ContentDirection.RightToLeft; break;
				case "_TrueColorSpace": m.Checked = !_ViewerBox.GrayScale; break;
				case "_GrayColorSpace": m.Checked = _ViewerBox.GrayScale; break;
				case "_InvertColor": m.Checked = _ViewerBox.InvertColor; break;
				case "_MoveMode": m.Checked = _ViewerBox.MouseMode == Editor.MouseMode.Move; break;
				case "_SelectionMode": m.Checked = _ViewerBox.MouseMode == Editor.MouseMode.Selection; break;
				case "_FullPageScroll": m.Checked = _ViewerBox.FullPageScroll; break;
				case "_ShowTextBorders": m.Checked = _ViewerBox.ShowTextBorders; break;
				case "_DarkMode": m.Checked = _ViewerBox.TintColor == __DarkModeColor; break;
				case "_GreenMode": m.Checked = _ViewerBox.TintColor == __GreenModeColor; break;
				case "_ShowAnnotations": m.Checked = !_ViewerBox.HideAnnotations; break;
				case "_ShowBookmarks": m.Checked = !_MainPanel.Panel1Collapsed; break;
				case "_OcrPage": item.Enabled = ModiOcr.ModiInstalled && _ViewerBox.OcrLanguage != Constants.Ocr.NoLanguage; break;
				case "_OcrDetectPunctuation":
					item.Enabled = ModiOcr.ModiInstalled && _ViewerBox.OcrLanguage != Constants.Ocr.NoLanguage;
					m.Checked = _ViewerBox.OcrOptions.DetectContentPunctuations;
					break;
				case "_InsertWithOcrOnly":
					m.Checked = _controller.Model.InsertBookmarkWithOcrOnly;
					break;
				case "_EnableOcr":
					item.Enabled = ModiOcr.ModiInstalled;
					item.ToolTipText = item.Enabled ? String.Empty : Messages.ModiNotAvailable;
					break;
				case "_OcrDisabled":
					m.Checked = _ViewerBox.OcrLanguage == Constants.Ocr.NoLanguage;
					break;
				case "_CopySelection":
					item.Enabled = _ViewerBox.SelectionRegion.Contains(_ViewerBox.PinPoint);
					break;
				case "_AutoBookmark":
					if (m.DropDownItems.Count == 0) {
						m.DropDownItemClicked += _MainToolbar_ItemClicked;
						for (int i = 1; i < 8; i++) {
							m.DropDownItems.Add(new ToolStripMenuItem($"&{i} 级标题") { Name = "_AutoBookmarkLevel" + i });
						}
					}
					break;
				default:
					EnableCommand(item, true, true);
					break;
			}
			base.SetupCommand(item);
		}

		public override void ExecuteCommand(string cmd, params string[] parameters) {
			switch (cmd) {
				#region 书签命令
				case "_InsertBookmark":
					_controller.InsertBookmark();
					break;
				case "_MergeBookmark":
					_controller.MergeBookmark(_BookmarkBox.GetSelectedElements());
					break;
				case "_SearchReplace":
					if (_searchForm == null || _searchForm.IsDisposed) {
						_searchForm = new SearchBookmarkForm(_controller);
					}
					if (!_searchForm.Visible) {
						_searchForm.Show(this);
					}
					_searchForm.BringToFront();
					break;
				#endregion
				#region 阅读器工具栏命令
				case "_AutoBookmark":
					_controller.ShowAutoBookmarkForm();
					break;
				case "_CopySelection":
					var sel = _ViewerBox.GetSelection();
					if (sel.Page > 0) {
						var r = _ViewerBox.GetSelectionPageRegion();
						var lines = _ViewerBox.FindTextLines(r);
						if (lines != null) {
							Clipboard.SetText(String.Join(Environment.NewLine, lines.Select(i => i.GetText())));
						}
						else {
							using (var b = sel.GetSelectedBitmap()) {
								Clipboard.SetImage(b);
							}
						}
					}
					break;
				case "_InsertWithOcrOnly":
					_controller.Model.InsertBookmarkWithOcrOnly = !_controller.Model.InsertBookmarkWithOcrOnly;
					break;
				#endregion
				default:
					if (cmd.HasPrefix("_AutoBookmarkLevel")) {
						_controller.ConfigAutoBookmarkTextStyles(
							cmd.Substring("_AutoBookmarkLevel".Length).ToInt32(),
							_ViewerBox.FindTextLines(_ViewerBox.TransposeVirtualImageToPagePosition(_ViewerBox.PinPoint.X, _ViewerBox.PinPoint.Y)));
						break;
					}
					_controller.ExecuteCommand(cmd, parameters);
					break;
			}
		}

		public void CloseDocument() {
			_ViewerBox.CloseFile();
		}

		public void Reopen() {
			_ViewerBox.Reopen();
		}

		void _BookmarkColorButton_SelectedColorChanged(object sender, EventArgs e) {
			var c = _BookmarkColorButton.Color;
			_controller.ProcessBookmarks(new SetTextColorProcessor(c));
		}

		void _OpenButton_DropDownOpening(object sender, EventArgs e) {
			var m = (sender as ToolStripDropDownItem);
			var l = m.DropDown.Items;
			l.ClearDropDownItems();
			l.AddSourcePdfFiles();
			if (l.Count > 0) {
				l.Add(new ToolStripSeparator());
			}
			l.AddInfoFiles();
		}

		void _OpenButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			e.ClickedItem.HidePopupMenu();
			ExecuteCommand(Commands.OpenFile, e.ClickedItem.ToolTipText);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if (_BookmarkBox.IsCellEditing || _CurrentPageBox.Focused) {
				return base.ProcessCmdKey(ref msg, keyData);
			}
			switch (keyData ^ Keys.Control) {
				case Keys.B:
					ExecuteCommand("_BookmarkBoldButton"); return true;
				case Keys.I:
					ExecuteCommand("_BookmarkItalicButton"); return true;
				case Keys.Z: _controller.Undo(1); return true;
				case Keys.F:
					ExecuteCommand("_SearchReplace"); return true;
				case Keys.R:
					ExecuteCommand(Commands.ImportBookmark); return true;
				case Keys.Q:
					ExecuteCommand(Commands.SaveBookmark); return true;
				case Keys.S:
					ExecuteCommand(Commands.Action); return true;
				case Keys.O:
					ExecuteCommand(Commands.Open); return true;
				case Keys.C:
					ExecuteCommand(Commands.Copy); return true;
				case Keys.V:
					ExecuteCommand(Commands.Paste); return true;
				case Keys.W:
					ExecuteCommand(Commands.EditorBookmarkSetCurrentCoordinates);
					return true;
				case Keys.Alt | Keys.W:
					ExecuteCommand(Commands.EditorBookmarkSetCurrentCoordinates);
					_controller.View.Bookmark.SelectNext();
					return true;
				case Keys.Down:
					_controller.InsertBookmark(InsertBookmarkPositionType.AfterCurrent);
					return true;
				case Keys.Up:
					_controller.InsertBookmark(InsertBookmarkPositionType.BeforeCurrent);
					return true;
				case Keys.Right:
					_controller.InsertBookmark(InsertBookmarkPositionType.AsChild);
					return true;
				case Keys.Left:
					_controller.InsertBookmark(InsertBookmarkPositionType.AfterParent);
					return true;
			}
			switch (keyData ^ Keys.Shift) {
				case Keys.Tab:
					ExecuteCommand("_LevelUp"); return true;
				case Keys.D8:
					ExecuteCommand("_ShiftMultiPageNumber"); return true;
				case Keys.Up:
					_controller.View.Bookmark.SelectPrevious(); return true;
				case Keys.Down:
					_controller.View.Bookmark.SelectNext(); return true;
				case Keys.Right:
					_controller.View.Bookmark.ExpandSelected(true); return true;
				case Keys.Left:
					_controller.View.Bookmark.CollapseSelected(true); return true;
				case Keys.Enter:
					_controller.ExecuteCommand(Commands.EditorViewerScrollToBookmark); return true;
			}
			switch (keyData) {
				case Keys.Insert:
					_controller.InsertBookmark(); return true;
				case Keys.Delete:
					ExecuteCommand(Commands.Delete); return true;
				case Keys.Add:
				case Keys.Oemplus:
					ExecuteCommand("_IncrementPageNumber"); return true;
				case Keys.Subtract:
				case Keys.OemMinus:
					ExecuteCommand("_DecrementPageNumber"); return true;
				case Keys.Multiply:
					ExecuteCommand("_ShiftMultiPageNumber"); return true;
				case Keys.P:
					if (_BookmarkBox.FocusedItem != null) {
						_BookmarkBox.EditSubItem(_BookmarkBox.FocusedItem as BrightIdeasSoftware.OLVListItem, _BookmarkBox.BookmarkPageColumn.Index);
					}
					return true;
				case Keys.Tab:
					ExecuteCommand("_LevelDown"); return true;
				case Keys.F2:
					if (_BookmarkBox.FocusedItem != null) {
						_BookmarkBox.FocusedItem.BeginEdit();
					}
					else {
						_BookmarkBox.SelectedItem?.BeginEdit();
					}
					return true;
				case Keys.Space:
					if (_BookmarkBox.FocusedItem != null) {
						_ViewerBox.CurrentPageNumber = (_BookmarkBox.FocusedObject as BookmarkElement).Page;
					}
					return true;
				case Keys.ProcessKey:
				case Keys.Oem4:
				case Keys.Oem6:
					if (msg.Msg == 256) {
						switch ((int)msg.LParam) {
							case 0x001A0001:
								ExecuteCommand("_PreviousPage");
								return true;
							case 0x001B0001:
								ExecuteCommand("_NextPage");
								return true;
						}
					}
					break;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		void _BookmarkBox_DragEnter(object sender, DragEventArgs e) {
			e.FeedbackDragFileOver(Constants.FileExtensions.PdfAndAllBookmarkExtension);
		}

		void _BookmarkBox_DragDrop(object sender, DragEventArgs e) {
			if (this.DropFileOver(e, Constants.FileExtensions.PdfAndAllBookmarkExtension)
				&& (!_controller.Model.Undo.IsDirty
					|| AppContext.MainForm.ConfirmYesBox(Messages.ConfirmAbandonDocument))) {
				_controller.LoadDocument(Text, false);
			}
		}

		#region Editor.IEditView
		bool Editor.IEditView.AffectsDescendantBookmarks => _IncludeDecendantBox.Checked || ModifierKeys == Keys.Shift;

		ToolStripSplitButton Editor.IEditView.UndoButton => _UndoButton;

		AutoBookmarkForm Editor.IEditView.AutoBookmark {
			get {
				if (_autoBookmarkForm?.IsDisposed != false) {
					_autoBookmarkForm = new AutoBookmarkForm(_controller);
				}
				return _autoBookmarkForm;
			}
		}

		BookmarkEditorView Editor.IEditView.Bookmark => _BookmarkBox;

		ViewerControl Editor.IEditView.Viewer => _ViewerBox;

		ToolStrip Editor.IEditView.ViewerToolbar => _ViewerToolbar;

		ToolStrip Editor.IEditView.BookmarkToolbar => _BookmarkToolbar;

		SplitContainer Editor.IEditView.MainPanel => _MainPanel;

		#endregion
	}
}
