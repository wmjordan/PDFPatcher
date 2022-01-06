using System;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor
{
	sealed class BookmarkLevelCommand : IEditorCommand
	{
		readonly bool _levelUp;
		public BookmarkLevelCommand(bool levelUp) {
			_levelUp = levelUp;
		}
		public void Process(Controller controller, params string[] parameters) {
			BookmarkLevel(controller, _levelUp);
		}

		internal void BookmarkLevel(Controller controller, bool levelUp) {
			var b = controller.View.Bookmark;
			var si = b.GetSelectedElements(true);
			b.BeginUpdate();
			var ld = controller.ProcessBookmarks(false, false, levelUp ? new LevelUpProcessor() as IPdfInfoXmlProcessor : new LevelDownProcessor());
			if (ld != null) {
				foreach (var item in ld) {
					b.Expand(item);
				}
			}
			b.RefreshObjects(si);
			b.SelectedObjects = si;
			b.EndUpdate();
		}

	}
}
