using System;
using System.Collections.Generic;
using System.Xml;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor
{
	sealed class DeleteBookmarkItemCommand : IEditorCommand
	{
		public void Process(Controller controller, params string[] parameters) {
			RemoveItems(controller, controller.View.Bookmark.GetSelectedElements(false));
		}

		static void RemoveItems(Controller controller, System.Collections.IList si) {
			if (si.Count == 0) {
				return;
			}
			var b = controller.View.Bookmark;
			b.RemoveObjects(si);
			var undo = new UndoActionGroup();
			var l = new List<XmlNode>();
			foreach (XmlElement item in si) {
				if (item == null || item.ParentNode == null) {
					continue;
				}
				undo.Add(new AddElementAction(item));
				var p = item.ParentNode;
				p.RemoveChild(item);
				if (!l.Contains(p)) {
					l.Add(p);
				}
			}
			foreach (var item in l) {
				if (item.ParentNode != null) {
					b.RefreshObject(item);
				}
			}
			controller.Model.Undo.AddUndo("删除书签", undo);
		}

	}
}
