using System;

namespace PDFPatcher.Functions.Editor;

sealed class BookmarkSearchReplaceCommand : IEditorCommand
{
	SearchBookmarkForm _searchForm;

	public void Process(Controller controller, params string[] parameters) {
		if (controller.View is FunctionControl view) {
			if (_searchForm == null || _searchForm.IsDisposed) {
				_searchForm = new SearchBookmarkForm(controller);
				view.Deselected += HideOnDocumentDeselected;
			}
			if (!_searchForm.Visible) {
				_searchForm.Show(view);
			}
			_searchForm.BringToFront();
		}
	}

	void HideOnDocumentDeselected(object sender, EventArgs e) {
		_searchForm?.Close();
	}
}
