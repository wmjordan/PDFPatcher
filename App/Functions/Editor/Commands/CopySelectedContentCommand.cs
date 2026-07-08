using System;
using System.Linq;
using System.Windows.Forms;
using MuPDF.Extensions;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor;

sealed class CopySelectedContentCommand : IEditorCommand
{
	public void Process(Controller controller, params string[] parameters) {
		var viewer = controller.View.Viewer;
		var sel = viewer.GetSelection();
		if (sel.Page == 0) {
			return;
		}
		var r = viewer.GetSelectionPageRegion();
		var lines = viewer.FindTextLines(r);
		if (lines != null) {
			Clipboard.SetText(String.Join(Environment.NewLine, lines.Select(i => i.GetText())));
		}
		else {
			using var b = sel.GetSelectedBitmap();
			Clipboard.SetImage(b);
		}
	}
}
