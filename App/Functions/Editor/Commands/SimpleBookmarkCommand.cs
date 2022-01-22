using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor;

internal sealed class SimpleBookmarkCommand<T> : IEditorCommand where T : IPdfInfoXmlProcessor, new()
{
	public void Process(Controller controller, params string[] parameters) {
		BookmarkEditorView b = controller.View.Bookmark;
		if (b.FocusedItem == null) {
			return;
		}

		controller.ProcessBookmarks(new T());
	}
}

internal sealed class SimpleBookmarkCommand<T, P> : IEditorCommand where T : IPdfInfoXmlProcessor<P>, new()
{
	private readonly P _parameter;

	public SimpleBookmarkCommand(P parameter) {
		_parameter = parameter;
	}

	public void Process(Controller controller, params string[] parameters) {
		BookmarkEditorView b = controller.View.Bookmark;
		if (b.FocusedItem == null) {
			return;
		}

		controller.ProcessBookmarks(new T { Parameter = _parameter });
	}
}