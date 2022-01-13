﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor;

internal sealed class PasteBookmarkItemCommand : IEditorCommand
{
	public void Process(Controller controller, params string[] parameters) {
		BookmarkEditorView b = controller.View.Bookmark;
		b.PasteBookmarks(b.FocusedItem != null
				? b.GetModelObject(b.FocusedItem.Index) as XmlElement
				: controller.Model.Document.BookmarkRoot,
			b.FocusedItem == null);
	}
}