using System.Xml;
using BrightIdeasSoftware;
using PDFPatcher.Model;

namespace PDFPatcher.Functions.Editor;

internal sealed class BookmarkSelectionCommand : IEditorCommand
{
	private readonly string _command;

	public BookmarkSelectionCommand(string command) {
		_command = command;
	}

	public void Process(Controller controller, params string[] parameters) {
		BookmarkEditorView b = controller.View.Bookmark;
		switch (_command) {
			case Commands.SelectAllItems:
				b.SelectAll();
				break;
			case Commands.SelectNone:
				b.DeselectAll();
				break;
			case Commands.InvertSelectItem:
				b.InvertSelect();
				break;
			case "_CollapseAll":
				b.CollapseAll();
				break;
			case "_ExpandAll":
				b.ExpandAll();
				break;
			case "_CollapseChildren":
				foreach (BookmarkElement item in b.GetSelectedElements(false)) {
					foreach (XmlNode ci in item.SubBookmarks) {
						b.Collapse(ci);
					}
				}

				break;
		}
	}
}