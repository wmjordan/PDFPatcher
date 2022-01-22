using System;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Functions.Editor;
using PDFPatcher.Model;
using PDFPatcher.Processor;
using PDFPatcher.Properties;
using Rectangle = MuPdfSharp.Rectangle;
using TextInfo = PDFPatcher.Functions.Editor.TextInfo;

namespace PDFPatcher.Functions
{
	public sealed partial class EditorControl : FunctionControl, IDocumentEditor, IEditView
	{
		static readonly Color __DarkModeColor = Color.DarkGray;
		static readonly Color __GreenModeColor = Color.FromArgb(0xCC, 0xFF, 0xCC);

		static readonly CommandRegistry<Controller> __Commands = InitCommands();
		readonly Controller _controller;
		AutoBookmarkForm _autoBookmarkForm;
		SearchBookmarkForm _searchForm;

		public EditorControl() {
			InitializeComponent();
			_controller = new Controller(this);
		}

		public override string FunctionName => "文档编辑器";

		public override Bitmap IconImage => Resources.Editor;

		public event EventHandler<DocumentChangedEventArgs> DocumentChanged;

		public string DocumentPath {
			get => _controller?.Model.DocumentPath;
			set {
				_controller.Model.DocumentPath = value;
				if (DocumentChanged != null) {
					DocumentChanged(this, new DocumentChangedEventArgs(value));
				}
			}
		}

		public void CloseDocument() {
			_ViewerBox.CloseFile();
		}

		public void Reopen() {
			_ViewerBox.Reopen();
		}

		private static CommandRegistry<Controller> InitCommands() {
			CommandRegistry<Controller> d = new CommandRegistry<Controller>();
			d.Register(new LoadDocumentCommand(true, false), Commands.Open);
			d.Register(new LoadDocumentCommand(true, true), Commands.ImportBookmark);
			d.Register(new LoadDocumentCommand(false, false), Commands.OpenFile);
			d.Register(new InsertBookmarkCommand(), Commands.EditorInsertBookmark);
			d.Register(new SaveDocumentCommand(false, true), "_SaveButton", Commands.SaveBookmark);
			d.Register(new SaveDocumentCommand(true, true), Commands.SaveAsInfoFile);
			d.Register(new SaveDocumentCommand(true, false), Commands.Action, Commands.EditorSavePdf);
			d.Register(new BookmarkLevelCommand(true), Commands.EditorBookmarkLevelUp);
			d.Register(new BookmarkLevelCommand(false), Commands.EditorBookmarkLevelDown);
			d.Register(new DocumentPropertyCommand(), Commands.DocumentProperties);
			d.Register(new CopyBookmarkItemCommand(), Commands.Copy);
			d.Register(new PasteBookmarkItemCommand(), Commands.Paste);
			d.Register(new DeleteBookmarkItemCommand(), Commands.EditorBookmarkDelete, Commands.Delete);
			d.Register(new BookmarkStyleCommand(SetTextStyleProcessor.Style.SetBold), Commands.EditorBookmarkBold);
			d.Register(new BookmarkStyleCommand(SetTextStyleProcessor.Style.SetItalic), Commands.EditorBookmarkItalic);
			d.Register(new BookmarkPageCommand(1), Commands.EditorBookmarkPageNumberIncrement);
			d.Register(new BookmarkPageCommand(-1), Commands.EditorBookmarkPageNumberDecrement);
			d.Register(new BookmarkPageCommand(0), Commands.EditorBookmarkPageNumberShift);
			d.Register(
				new SimpleBookmarkCommand<ClearDestinationOffsetProcessor,
					ClearDestinationOffsetProcessor.PositionType>(ClearDestinationOffsetProcessor.PositionType.XY),
				"_ClearPositionXY");
			d.Register(
				new SimpleBookmarkCommand<ClearDestinationOffsetProcessor,
					ClearDestinationOffsetProcessor.PositionType>(ClearDestinationOffsetProcessor.PositionType.X),
				"_ClearPositionX");
			d.Register(
				new SimpleBookmarkCommand<ClearDestinationOffsetProcessor,
					ClearDestinationOffsetProcessor.PositionType>(ClearDestinationOffsetProcessor.PositionType.Y),
				"_ClearPositionY");
			d.Register(new SimpleBookmarkCommand<BookmarkOpenStatusProcessor, bool>(true), "_SetOpenStatusTrue");
			d.Register(new SimpleBookmarkCommand<BookmarkOpenStatusProcessor, bool>(false), "_SetOpenStatusFalse");
			foreach (string item in Constants.DestinationAttributes.ViewType.Names) {
				d.Register(new BookmarkActionCommand(item), item);
			}

			d.Register(new BookmarkActionCommand(Constants.Coordinates.Unchanged), Constants.Coordinates.Unchanged);
			d.Register(new BookmarkActionCommand("_ChangeCoordinates"), "_ChangeCoordinates");
			d.Register(new BookmarkActionCommand("_BookmarkAction"), "_BookmarkAction");
			d.Register(new SimpleBookmarkCommand<DestinationGotoTopProcessor>(), "_SetGotoTop");
			d.Register(new SimpleBookmarkCommand<ForceInternalLinkProcessor>(), "_ForceInternalLink");
			d.Register(new BookmarkSelectionCommand(Commands.SelectAllItems), Commands.SelectAllItems);
			d.Register(new BookmarkSelectionCommand(Commands.SelectNone), Commands.SelectNone);
			d.Register(new BookmarkSelectionCommand(Commands.InvertSelectItem), Commands.InvertSelectItem);
			d.Register(new BookmarkSelectionCommand("_CollapseAll"), "_CollapseAll");
			d.Register(new BookmarkSelectionCommand("_ExpandAll"), "_ExpandAll");
			d.Register(new BookmarkSelectionCommand("_CollapseChildren"), "_CollapseChildren");
			d.Register(new OcrPageCommand(), Commands.EditorOcrPage);
			d.Register(new PagePropertiesCommand(), Commands.EditorPageProperties);
			d.Register(new SavePageImageCommand(), Commands.EditorSavePageImage);
			BookmarkMarkerCommand.RegisterCommands(d);
			ViewerCommand.RegisterCommands(d);
			QuickSelectCommand.RegisterCommands(d);
			return d;
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if (DesignMode) {
				return;
			}

			ListRecentFiles = _OpenButton_DropDownOpening;
			RecentFileItemClicked = _OpenButton_DropDownItemClicked;
			_ViewerToolbar.Left = _BookmarkToolbar.Right;
			//_MainToolbar.ToggleEnabled (false, _editButtonNames);

			_controller.PrepareBookmarkDocument();

			ToolStripItemCollection di = _ChangeZoomRate.DropDownItems;
			di.AddRange(Array.ConvertAll(Constants.DestinationAttributes.ViewType.Names,
				n => new ToolStripMenuItem { Name = n, Text = n }));
			di.RemoveByKey(Constants.DestinationAttributes.ViewType.FitR);
			di[0].Text += "...";
			di.Insert(0,
				new ToolStripMenuItem { Name = Constants.Coordinates.Unchanged, Text = Constants.Coordinates.Unchanged });
			_ChangeZoomRate.DropDownItemClicked += _MainToolbar_ItemClicked;
			_ChangeCase.DropDownItemClicked += (object s, ToolStripItemClickedEventArgs args) => {
				FormHelper.HidePopupMenu(args.ClickedItem);
				_EditMenu.Hide();
				int i = Array.IndexOf(SetCaseProcessor.CaseNames, args.ClickedItem.Text);
				if (i != -1) {
					_controller.ProcessBookmarks(new SetCaseProcessor((SetCaseProcessor.LetterCase)i));
				}
			};
			foreach (int item in Enum.GetValues(typeof(SetCaseProcessor.LetterCase))) {
				_ChangeCase.DropDownItems.Add(SetCaseProcessor.CaseNames[item]);
			}

			_SetOpenStatus.DropDownItemClicked += _MainToolbar_ItemClicked;

			AppContext.MainForm.SetTooltip(_IncludeDecendantBox, "选中此选项后，加粗、斜体等其它修改书签的操作将应用到选中书签的子书签");
			_IncludeDecendantBox.CheckedChanged += (s, args) => {
				_BookmarkBox.OperationAffectsDescendants = _IncludeDecendantBox.Checked;
			};

			_UndoButton.DropDownOpening += (object s, EventArgs args) => {
				ToolStripItemCollection i = _UndoMenu.Items;
				i.Clear();
				foreach (string item in _controller.Model.Undo.GetActionNames(16)) {
					i.Add(item);
				}
			};
			_UndoButton.DropDownItemClicked += (object s, ToolStripItemClickedEventArgs args) => {
				int i = args.ClickedItem.Owner.Items.IndexOf(args.ClickedItem) + 1;
				_controller.Undo(i);
			};
			QuickSelectCommand.RegisterMenuItems(_QuickSelect.DropDownItems);
			_BookmarkBox.CellClick += (s, args) => {
				if (args.ColumnIndex != 0 || args.ClickCount > 1 || ModifierKeys != Keys.None) {
					return;
				}

				ScrollToSelectedBookmarkLocation();
				//var bs = el.GetAttribute (Constants.BookmarkAttributes.Style);
				//switch (bs) {
				//    case Constants.BookmarkAttributes.StyleType.Bold:
				//        _BookmarkBoldButton.Checked = true;
				//        _BookmarkItalicButton.Checked = false;
				//        break;
				//    case Constants.BookmarkAttributes.StyleType.BoldItalic:
				//        _BookmarkBoldButton.Checked = true;
				//        _BookmarkItalicButton.Checked = true;
				//        break;
				//    case Constants.BookmarkAttributes.StyleType.Italic:
				//        _BookmarkBoldButton.Checked = false;
				//        _BookmarkItalicButton.Checked = true;
				//        break;
				//    default:
				//        _BookmarkBoldButton.Checked = false;
				//        _BookmarkItalicButton.Checked = false;
				//        break;
				//}
			};
			_BookmarkBox.CellEditStarting += (s, args) => {
				if (args.Column.Index == 0) {
					ScrollToSelectedBookmarkLocation();
				}
			};
			_BookmarkBox.BeforeLabelEdit += (s, args) => {
				((TreeListView)s).SelectedIndex = args.Item;
				ScrollToSelectedBookmarkLocation();
			};
			_CurrentPageBox.KeyUp += (s, args) => {
				if (args.KeyCode != Keys.Enter) {
					return;
				}

				int p;
				if (!_CurrentPageBox.Text.TryParse(out p)) {
					return;
				}

				_ViewerBox.CurrentPageNumber = p;
				_CurrentPageBox.Text = p.ToText();
			};
			_ViewerButton.DropDownOpening += (s, args) => {
				SetupMenu(_ViewerButton.DropDownItems);
			};
			_OcrMenu.DropDownItemClicked += (s, args) => {
				_ViewerBox.OcrLanguage = (int)(args.ClickedItem.Tag ?? 0);
			};
			_OcrMenu.DropDownOpening += (s, args) => {
				ToolStripItemCollection m = _OcrMenu.DropDownItems;
				if (m.Count == 1) {
					for (int i = 0; i < Constants.Ocr.LangIDs.Length; i++) {
						ToolStripMenuItem item = new ToolStripMenuItem(Constants.Ocr.LangNames[i]);
						m.Add(item);
						item.Tag = Constants.Ocr.LangIDs[i];
						item.Enabled = ModiOcr.IsLanguageInstalled(Constants.Ocr.LangIDs[i]);
					}
				}

				foreach (ToolStripMenuItem item in _OcrMenu.DropDownItems) {
					item.Checked = _ViewerBox.OcrLanguage == (int)(item.Tag ?? 0);
				}
			};
			_ZoomBox.Text = Constants.DestinationAttributes.ViewType.FitH;
			_ZoomBox.TextChanged += (s, args) => {
				_ViewerBox.LiteralZoom = _ZoomBox.Text;
			};
			_ViewerBox.Enabled = false;
			_ViewerBox.DocumentLoaded += (s, args) => {
				_CurrentPageBox.ToolTipText = "文档共" + _ViewerBox.Document.PageCount + "页";
			};
			_ViewerBox.ZoomChanged += (s, args) => {
				_ZoomBox.ToolTipText = "当前显示比例：" + (_ViewerBox.ZoomFactor * 100).ToInt32() + "%";
			};
			_ViewerBox.PageChanged += (s, args) => {
				_CurrentPageBox.Text = _ViewerBox.CurrentPageNumber.ToText();
				//var b = _ViewerBox.PageBound;
				//_PageInfoBox.Text = string.Concat ("尺寸：", Math.Round (b.Width, 1).ToText (), " * ", Math.Round (b.Height, 1).ToText ());
			};
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

				Point l = args.Location;
				PagePosition p = _ViewerBox.TransposeClientToPagePosition(l.X, l.Y);
				if (p.Page == 0) {
					return;
				}

				TextInfo ti = _ViewerBox.FindTextLines(p);
				string t = ti.TextLines != null ? ti.TextLines[0].Text : String.Empty;
				_PageInfoBox.Text = string.Concat("页面：", p.Page, "; 位置：", Math.Round(p.PageX, 2), " * ",
					Math.Round(p.PageY, 2),
					ti.Spans.HasContent()
						? String.Concat("; 字体：", ti.Page.GetFont(ti.Spans[0]).Name, " ", ti.Spans[0].Size)
						: null, t.Length > 0 ? "; 文本：" : null, t);
			};
			_ViewerBox.MouseClick += _ViewBox_MouseClick;
			_ViewerToolbar.Enabled = false;

			Disposed += (s, args) => { _controller.Destroy(); };
		}

		private void ScrollToSelectedBookmarkLocation() {
			int i = _BookmarkBox.GetFirstSelectedIndex();
			//_MainToolbar.ToggleEnabled (i != -1, _editButtonNames);
			if (i == -1) {
				return;
			}

			BookmarkElement el = _BookmarkBox.GetModelObject(i) as BookmarkElement;
			if (_controller.Model.LockDownViewer != false || _BookmarkBox.SelectedIndices.Count != 1 ||
				(i = el.Page) <= 0) {
				return;
			}

			PdfViewerControl v = _ViewerBox;
			if (_controller.Model.PdfDocument == null || el.Page <= 0 || el.Page > _ViewerBox.Document.PageCount) {
				return;
			}

			Rectangle b = _ViewerBox.GetPageBound(el.Page);
			v.ScrollToPosition(new PagePosition(el.Page,
				v.HorizontalFlow ? el.Left > b.Width ? b.Width : el.Left : 0,
				v.HorizontalFlow || el.Top == 0 ? 0 : el.Top.LimitInRange(b.Top, b.Bottom),
				0, 0, true)
			);
		}

		//protected override void OnClick (EventArgs e) {
		//	base.OnClick (e);
		//	_controller.HideInsertBookmarkForm ();
		//}
		internal override void OnDeselected() {
			base.OnDeselected();
			_searchForm?.Close();
		}

		private void _ViewBox_MouseClick(object sender, MouseEventArgs args) {
			if (_ViewerBox.FirstPage == 0) {
				return;
			}

			Point l = args.Location;
			if (args.Button != MouseButtons.Right) {
				return;
			}

			_ViewerBox.PinPoint = _ViewerBox.PointToImage(l);
			SetupMenu(_ViewerMenu.Items);
			_ViewerMenu.Show(_ViewerBox, l);
			if (_ViewerBox.IsClientPointInSelection(l) == false) {
				_ViewerBox.SelectNone();
			}
			//_ViewerBox.Invalidate ();
			//var sp = _ViewerBox.FindTextSpanAtPoint (p);
			//var t = sp != null ? sp.Text : String.Empty;
			//_controller.ShowInsertBookmarkDialog (l, p, t);
		}

		private void _MainToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			FormHelper.HidePopupMenu(e.ClickedItem);
			ExecuteCommand(e.ClickedItem.Name);
		}

		private void ButtonClicked(object sender, EventArgs e) {
			if (sender == _UndoButton) {
				_controller.Undo(1);
			}
			else if (sender == _AddFilesButton) {
				ExecuteCommand(Commands.Open);
			}
		}

		public override void SetupCommand(ToolStripItem item) {
			string n = item.Name;
			bool l = _controller.Model.DocumentPath != null;
			ToolStripMenuItem m = item as ToolStripMenuItem;
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
				case Commands.Options:
					item.Text = "设置文件修改方式(&X)...";
					item.ToolTipText = "设置在编辑器修改 PDF 文档的选项";
					EnableCommand(item, true, true);
					item.Tag = nameof(Function.EditorOptions);
					break;
				case Commands.ResetOptions:
					// keep disabled
					break;
				case Commands.DocumentProperties:
					item.Enabled = _ViewerBox.Document != null && _ViewerBox.Document.IsDocumentOpened;
					item.Visible = true;
					break;
				case "_ScrollVertical":
					m.Checked = _ViewerBox.ContentDirection == ContentDirection.TopToDown;
					break;
				case "_ScrollHorizontal":
					m.Checked = _ViewerBox.ContentDirection == ContentDirection.RightToLeft;
					break;
				case "_TrueColorSpace":
					m.Checked = _ViewerBox.GrayScale == false;
					break;
				case "_GrayColorSpace":
					m.Checked = _ViewerBox.GrayScale;
					break;
				case "_InvertColor":
					m.Checked = _ViewerBox.InvertColor;
					break;
				case "_MoveMode":
					m.Checked = _ViewerBox.MouseMode == MouseMode.Move;
					break;
				case "_SelectionMode":
					m.Checked = _ViewerBox.MouseMode == MouseMode.Selection;
					break;
				case "_FullPageScroll":
					m.Checked = _ViewerBox.FullPageScroll;
					break;
				case "_ShowTextBorders":
					m.Checked = _ViewerBox.ShowTextBorders;
					break;
				case "_DarkMode":
					m.Checked = _ViewerBox.TintColor == __DarkModeColor;
					break;
				case "_GreenMode":
					m.Checked = _ViewerBox.TintColor == __GreenModeColor;
					break;
				case "_ShowAnnotations":
					m.Checked = _ViewerBox.HideAnnotations == false;
					break;
				case "_ShowBookmarks":
					m.Checked = _MainPanel.Panel1Collapsed == false;
					break;
				case "_OcrPage":
					item.Enabled = ModiOcr.ModiInstalled && _ViewerBox.OcrLanguage != Constants.Ocr.NoLanguage;
					break;
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
							m.DropDownItems.Add(
								new ToolStripMenuItem("&" + i + " 级标题") { Name = "_AutoBookmarkLevel" + i });
						}
					}

					break;
				case "_FullScreen":
					m.Checked = AppContext.MainForm.FullScreen;
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

					if (_searchForm.Visible == false) {
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
					Selection sel = _ViewerBox.GetSelection();
					if (sel.Page > 0) {
						using (Bitmap b = sel.GetSelectedBitmap()) {
							Clipboard.SetImage(b);
						}
					}

					break;
				case "_InsertPageLabel":
					_controller.LabelAtPage(
						_ViewerBox.TransposeVirtualImageToPagePosition(_ViewerBox.PinPoint.X, _ViewerBox.PinPoint.Y));
					break;
				case "_InsertWithOcrOnly":
					_controller.Model.InsertBookmarkWithOcrOnly = !_controller.Model.InsertBookmarkWithOcrOnly;
					break;

				#endregion

				default:
					if (cmd.StartsWith("_AutoBookmarkLevel", StringComparison.Ordinal)) {
						_controller.ConfigAutoBookmarkTextStyles(
							cmd.Substring("_AutoBookmarkLevel".Length).ToInt32(),
							_ViewerBox.FindTextLines(
								_ViewerBox.TransposeVirtualImageToPagePosition(_ViewerBox.PinPoint.X,
									_ViewerBox.PinPoint.Y)));
						break;
					}

					__Commands.Process(cmd, _controller, parameters);
					break;
			}
		}

		private void _BookmarkColorButton_SelectedColorChanged(object sender, EventArgs e) {
			Color c = _BookmarkColorButton.Color;
			_controller.ProcessBookmarks(new SetTextColorProcessor(c));
		}

		private void _OpenButton_DropDownOpening(object sender, EventArgs e) {
			ToolStripDropDownItem m = (sender as ToolStripDropDownItem);
			ToolStripItemCollection l = m.DropDown.Items;
			l.ClearDropDownItems();
			l.AddSourcePdfFiles();
			if (l.Count > 0) {
				l.Add(new ToolStripSeparator());
			}

			l.AddInfoFiles();
		}

		private void _OpenButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			FormHelper.HidePopupMenu(e.ClickedItem);
			ExecuteCommand(Commands.OpenFile, e.ClickedItem.ToolTipText);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if (_BookmarkBox.IsCellEditing || _BookmarkBox.IsLabelEditing || _CurrentPageBox.Focused) {
				return base.ProcessCmdKey(ref msg, keyData);
			}

			switch (keyData ^ Keys.Control) {
				case Keys.B:
					ExecuteCommand("_BookmarkBoldButton");
					return true;
				case Keys.I:
					ExecuteCommand("_BookmarkItalicButton");
					return true;
				case Keys.Z:
					_controller.Undo(1);
					return true;
				case Keys.F:
					ExecuteCommand("_SearchReplace");
					return true;
				case Keys.R:
					ExecuteCommand(Commands.ImportBookmark);
					return true;
				case Keys.Q:
					ExecuteCommand(Commands.SaveBookmark);
					return true;
				case Keys.S:
					ExecuteCommand(Commands.Action);
					return true;
				case Keys.O:
					ExecuteCommand(Commands.Open);
					return true;
				case Keys.C:
					ExecuteCommand(Commands.Copy);
					return true;
				case Keys.V:
					ExecuteCommand(Commands.Paste);
					return true;
			}

			switch (keyData ^ Keys.Shift) {
				case Keys.Tab:
					ExecuteCommand("_LevelUp");
					return true;
			}

			switch (keyData) {
				case Keys.Insert:
					_controller.InsertBookmark();
					return true;
				case Keys.Delete:
					ExecuteCommand(Commands.Delete);
					return true;
				case Keys.Add:
					ExecuteCommand("_IncrementPageNumber");
					return true;
				case Keys.Subtract:
					ExecuteCommand("_DecrementPageNumber");
					return true;
				case Keys.P:
					if (_BookmarkBox.FocusedItem != null) {
						_BookmarkBox.EditSubItem(_BookmarkBox.FocusedItem as OLVListItem,
							_BookmarkBox.BookmarkPageColumn.Index);
					}

					return true;
				case Keys.Tab:
					ExecuteCommand("_LevelDown");
					return true;
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
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void _BookmarkBox_DragEnter(object sender, DragEventArgs e) {
			FormHelper.FeedbackDragFileOver(e, Constants.FileExtensions.PdfAndAllBookmarkExtension);
		}

		private void _BookmarkBox_DragDrop(object sender, DragEventArgs e) {
			if (FormHelper.DropFileOver(this, e, Constants.FileExtensions.PdfAndAllBookmarkExtension)) {
				_controller.LoadDocument(Text, false);
			}
		}


		#region Editor.IEditView

		bool IEditView.AffectsDescendantBookmarks => _IncludeDecendantBox.Checked || ModifierKeys == Keys.Shift;

		ToolStripSplitButton IEditView.UndoButton => _UndoButton;

		AutoBookmarkForm IEditView.AutoBookmark {
			get {
				if (_autoBookmarkForm == null || _autoBookmarkForm.IsDisposed) {
					_autoBookmarkForm = new AutoBookmarkForm(_controller);
				}

				return _autoBookmarkForm;
			}
		}

		BookmarkEditorView IEditView.Bookmark => _BookmarkBox;

		PdfViewerControl IEditView.Viewer => _ViewerBox;

		ToolStrip IEditView.ViewerToolbar => _ViewerToolbar;

		ToolStrip IEditView.BookmarkToolbar => _BookmarkToolbar;

		SplitContainer IEditView.MainPanel => _MainPanel;

		#endregion
	}
}