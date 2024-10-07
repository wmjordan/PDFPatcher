using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PDFPatcher.Functions.Editor
{
	sealed class PagePropertiesCommand : IEditorCommand
	{
		//static readonly Regex __RemoveOcrWhiteSpace = new Regex (@"\s{2,}", RegexOptions.Compiled);
		static PagePropertyForm _dialog;

		public void Process(Controller controller, params string[] parameters) {
			var v = controller.View.Viewer;
			var l = v.PinPoint;
			var p = v.TransposeVirtualImageToPagePosition(l.X, l.Y);
			var f = GetDialog();
			using (var page = controller.Model.PdfDocument.LoadPage(p.Page - 1)) {
				f.Location = v.PointToScreen(v.TransposeVirtualImageToClient(l.X, l.Y));
				f.Show();
				f.LoadPage(page);
			}
		}

		private static PagePropertyForm GetDialog() {
			if (_dialog != null && _dialog.IsDisposed == false) {
				return _dialog;
			}
			_dialog = new PagePropertyForm();
			_dialog.Deactivate += (s, args) => ((Form)s).Visible = false;
			return _dialog;
		}

	}
}
