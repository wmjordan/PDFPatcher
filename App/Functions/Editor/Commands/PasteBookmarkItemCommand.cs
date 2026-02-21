using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor;

sealed class PasteBookmarkItemCommand : IEditorCommand
{
	public void Process(Controller controller, params string[] parameters) {
		var b = controller.View.Bookmark;
		PasteBookmarks(b, b.FocusedItem != null
					? b.GetModelObject(b.FocusedItem.Index) as XmlElement
					: controller.Model.Document.BookmarkRoot,
					b.FocusedItem == null);
	}

	internal void PasteBookmarks(BookmarkEditorView b, XmlElement target, bool asChild) {
		try {
			var d = Clipboard.GetDataObject();
			bool c = false;
			if (d.GetData(nameof(PDFPatcher), false) is null) {
				var t = d.GetData(DataFormats.UnicodeText) as string;
				if (!t.IsNullOrWhiteSpace()) {
					var doc = new PdfInfoXmlDocument();
					using (var s = new System.IO.StringReader(t)) {
						OutlineManager.ImportSimpleBookmarks(s, doc);
					}
					BookmarkEditorView.CopiedBookmarks = doc.Bookmarks.ToNodeList<BookmarkElement>() as List<BookmarkElement>;
					c = true;
				}
			}
			if (!BookmarkEditorView.CopiedBookmarks.HasContent()) {
				return;
			}
			b.CopyOrMoveElement(BookmarkEditorView.CopiedBookmarks, target, asChild, true, true, c || b.OperationAffectsDescendants);
		}
		catch (Exception) {
			// ignore
		}
	}
}
