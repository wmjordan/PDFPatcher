using System;
using System.Linq;
using System.Xml;
using BrightIdeasSoftware;

namespace PDFPatcher.Functions.Editor
{
	sealed class BookmarkSelectionCommand : IEditorCommand
	{
		readonly string _command;

		public BookmarkSelectionCommand(string command) {
			_command = command;
		}

		public void Process(Controller controller, params string[] parameters) {
			var b = controller.View.Bookmark;
			switch (_command) {
				case Commands.SelectAllItems:
					b.SelectAll();
					break;
				case Commands.SelectNone:
					b.DeselectAll();
					break;
				case Commands.SelectChildren:
					var s = b.GetSelectedElements(false);
					foreach (var item in s) {
						b.Expand(item);
					}
					b.SelectedObjects = s.SelectMany(i => i.SubBookmarks.Cast<Model.BookmarkElement>()).ToList();
					break;
				case Commands.InvertSelection:
					b.InvertSelect();
					break;
				case Commands.CollapseAll:
					b.CollapseAll();
					break;
				case Commands.ExpandAll:
					b.ExpandAll();
					break;
				case Commands.CollapseChildren:
					foreach (var item in b.GetSelectedElements(false)) {
						foreach (XmlNode ci in item.SubBookmarks) {
							b.Collapse(ci);
						}
					}
					break;
				case Commands.ExpandSelection:
					b.Expand(b.SelectedObjects, true);
					break;
				case Commands.CollapseSelection:
					b.Collapse(b.SelectedObjects, true);
					break;
			}
		}

	}
}
