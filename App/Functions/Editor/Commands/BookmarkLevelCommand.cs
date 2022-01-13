using System.Collections.Generic;
using System.Xml;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor;

internal sealed class BookmarkLevelCommand : IEditorCommand
{
	private readonly bool _levelUp;

	public BookmarkLevelCommand(bool levelUp) {
		_levelUp = levelUp;
	}

	public void Process(Controller controller, params string[] parameters) {
		BookmarkLevel(controller, _levelUp);
	}

	internal void BookmarkLevel(Controller controller, bool levelUp) {
		BookmarkEditorView b = controller.View.Bookmark;
		List<BookmarkElement> si = b.GetSelectedElements(true);
		b.BeginUpdate();
		IEnumerable<XmlNode> ld = controller.ProcessBookmarks(false, false,
			levelUp ? new LevelUpProcessor() : new LevelDownProcessor());
		if (ld != null) {
			foreach (XmlNode item in ld) {
				b.Expand(item);
			}
		}

		b.RefreshObjects(si);
		b.SelectedObjects = si;
		b.EndUpdate();
	}
}