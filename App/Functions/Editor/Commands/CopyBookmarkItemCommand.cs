using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions.Editor;

sealed class CopyBookmarkItemCommand : IEditorCommand
{
	public void Process(Controller controller, params string[] parameters) {
		BookmarkEditorView.CopiedBookmarks = controller.View.Bookmark.GetSelectedElements(false);
		var d = new DataObject(nameof(PDFPatcher), String.Empty);
		if (BookmarkEditorView.CopiedBookmarks.HasContent()) {
			var sb = StringBuilderCache.Acquire();
			var p = BookmarkEditorView.CopiedBookmarks;
			BookmarkText(sb, p, 0);
			d.SetText(StringBuilderCache.GetStringAndRelease(sb));
		}
		Clipboard.SetDataObject(d, false, 10, 100);
	}

	static void BookmarkText(StringBuilder sb, IList<BookmarkElement> p, int indent) {
		foreach (var b in p) {
			sb.Append(' ', indent)
				.Append(b.Title.Replace('\t', ' '))
				.Append('\t')
				.AppendLine(b.Page.ToText());
			BookmarkText(sb, b.SubBookmarks.ToNodeList<BookmarkElement>(), indent + 1);
		}
	}
}
