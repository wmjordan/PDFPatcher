using BrightIdeasSoftware;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor
{
	sealed class BookmarkStyleCommand : IEditorCommand
	{
		readonly SetTextStyleProcessor.Style _style;

		public BookmarkStyleCommand(SetTextStyleProcessor.Style style) {
			_style = style;
		}

		public void Process(Controller controller, params string[] parameters) {
			var b = controller.View.Bookmark;
			if (b.FocusedItem == null) {
				return;
			}
			var i = b.GetFirstSelectedModel<Model.BookmarkElement>();
			if (i == null) {
				return;
			}
			controller.ProcessBookmarks(new SetTextStyleProcessor(i, _style));
		}

	}
}
