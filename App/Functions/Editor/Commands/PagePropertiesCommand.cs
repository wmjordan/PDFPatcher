using System.Windows.Forms;
using MuPdfSharp;
using Point = System.Drawing.Point;

namespace PDFPatcher.Functions.Editor;

internal sealed class PagePropertiesCommand : IEditorCommand
{
	//static readonly Regex __RemoveOcrWhiteSpace = new Regex (@"\s{2,}", RegexOptions.Compiled);
	private static PagePropertyForm _dialog;

	public void Process(Controller controller, params string[] parameters) {
		PdfViewerControl v = controller.View.Viewer;
		Point l = v.PinPoint;
		PagePosition p = v.TransposeVirtualImageToPagePosition(l.X, l.Y);
		PagePropertyForm f = GetDialog();
		using MuPage page = controller.Model.PdfDocument.LoadPage(p.Page);
		f.LoadPage(page);
		f.Location = v.PointToScreen(v.TransposeVirtualImageToClient(l.X, l.Y));
		f.Show();
	}

	private static PagePropertyForm GetDialog() {
		if (_dialog is { IsDisposed: false }) {
			return _dialog;
		}

		_dialog = new PagePropertyForm();
		_dialog.Deactivate += (s, _) => ((Form)s).Visible = false;
		return _dialog;
	}
}