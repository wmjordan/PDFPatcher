using PDFPatcher.Processor.ContentParser;

namespace PDFPatcher.Functions.Editor;

sealed class PagePropertiesCommand : IEditorCommand
{
	static PagePropertyForm _dialog;

	public void Process(Controller controller, params string[] parameters) {
		var v = controller.View.Viewer;
		var l = v.PinPoint;
		var p = v.TransposeVirtualImageToPagePosition(l.X, l.Y);
		var f = GetDialog();
		using var page = controller.Model.PdfDocument.LoadPage(p.Page - 1);
		f.Location = v.PointToScreen(v.TransposeVirtualImageToClient(l.X, l.Y));
		f.Show();
		f.LoadPage(controller.Model.PdfDocument, page);
	}

	private static PagePropertyForm GetDialog() {
		if (_dialog?.IsDisposed == false) {
			return _dialog;
		}
		_dialog = new PagePropertyForm();
		_dialog.Deactivate += (s, args) => {
			_dialog.Close();
			_dialog.Dispose();
			_dialog = null;
		};
		return _dialog;
	}

}
