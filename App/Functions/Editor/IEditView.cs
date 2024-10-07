using System.Windows.Forms;

namespace PDFPatcher.Functions.Editor
{
	internal interface IEditView
	{
		bool AffectsDescendantBookmarks { get; }
		ToolStripSplitButton UndoButton { get; }
		AutoBookmarkForm AutoBookmark { get; }
		BookmarkEditorView Bookmark { get; }
		ViewerControl Viewer { get; }
		ToolStrip ViewerToolbar { get; }
		ToolStrip BookmarkToolbar { get; }
		SplitContainer MainPanel { get; }
		string DocumentPath { get; set; }
	}
}