﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor;

internal sealed class DeleteBookmarkItemCommand : IEditorCommand
{
	public void Process(Controller controller, params string[] parameters) {
		RemoveItems(controller, controller.View.Bookmark.GetSelectedElements(false));
	}

	private void RemoveItems(Controller controller, System.Collections.IList si) {
		if (si.Count == 0) {
			return;
		}

		BookmarkEditorView b = controller.View.Bookmark;
		b.RemoveObjects(si);
		UndoActionGroup undo = new();
		List<XmlNode> l = new();
		foreach (XmlElement item in si) {
			if (item == null || item.ParentNode == null) {
				continue;
			}

			undo.Add(new AddElementAction(item));
			XmlNode p = item.ParentNode;
			p.RemoveChild(item);
			if (l.Contains(p) == false) {
				l.Add(p);
			}
		}

		foreach (XmlNode item in l) {
			if (item.ParentNode != null) {
				b.RefreshObject(item);
			}
		}

		controller.Model.Undo.AddUndo("删除书签", undo);
	}
}