using System;
using BrightIdeasSoftware;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor;

internal sealed class BookmarkStyleCommand : IEditorCommand
{
	private readonly SetTextStyleProcessor.Style _style;

	public BookmarkStyleCommand(SetTextStyleProcessor.Style style) {
		_style = style;
	}

	public void Process(Controller controller, params string[] parameters) {
		BookmarkEditorView b = controller.View.Bookmark;
		if (b.FocusedItem == null) {
			return;
		}

		BookmarkElement i = b.GetFirstSelectedModel<BookmarkElement>();
		if (i == null) {
			return;
		}

		controller.ProcessBookmarks(new SetTextStyleProcessor(i, _style));
	}
}