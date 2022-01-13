using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor;

internal sealed class CopyBookmarkItemCommand : IEditorCommand
{
	public void Process(Controller controller, params string[] parameters) {
		controller.View.Bookmark.CopySelectedBookmark();
	}
}