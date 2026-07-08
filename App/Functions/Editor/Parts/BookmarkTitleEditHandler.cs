using System;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace PDFPatcher.Functions.Editor.Parts;

/// <summary>
/// 处理书签栏书签标题在编辑过程的动作。
/// </summary>
sealed class BookmarkTitleEditHandler
{
	readonly Controller _controller;
	readonly BookmarkEditorView _bookmarkBox;

	public BookmarkTitleEditHandler(Controller controller) {
		_controller = controller;
		_bookmarkBox = controller.View.Bookmark;
		_bookmarkBox.BeforeLabelEdit += BookmarkBoxBeforeLabelEdit;
		_bookmarkBox.CellClick += BookmarkBoxCellClick;
		_bookmarkBox.CellEditStarting += BookmarkBoxCellEditStarting;
	}

	void BookmarkBoxBeforeLabelEdit(object sender, LabelEditEventArgs e) {
		if (AppContext.Reader.LocateBookmarkOnEdit) {
			((TreeListView)sender).SelectedIndex = e.Item;
			ScrollToSelectedBookmarkLocation();
		}
	}

	void BookmarkBoxCellClick(object sender, CellClickEventArgs e) {
		if (e.ColumnIndex != 0 || e.ClickCount > 1 || Control.ModifierKeys != Keys.None) {
			return;
		}
		ScrollToSelectedBookmarkLocation();
	}

	void BookmarkBoxCellEditStarting(object sender, CellEditEventArgs e) {
		if (AppContext.Reader.LocateBookmarkOnEdit && e.Column.Index == 0) {
			ScrollToSelectedBookmarkLocation();
		}
	}

	void ScrollToSelectedBookmarkLocation() {
		_controller.ExecuteCommand(Commands.EditorViewerScrollToBookmark);
	}
}
