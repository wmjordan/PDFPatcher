using System;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor
{
	sealed class InsertPageLabelCommand : IEditorCommand
	{
		public void Process(Controller context, params string[] parameters) {
			var v = context.View.Viewer;
			var position = v.TransposeVirtualImageToPagePosition(v.PinPoint.X, v.PinPoint.Y);
			if (position.Page == 0) {
				return;
			}
			var labels = context.Model.PageLabels;
			var f = new InsertPageLabelForm {
				Location = Cursor.Position.Transpose(-16, -16),
				PageNumber = position.Page,
				Model = context.Model,
				Viewer = v
			};
			f.FormClosing += InsertPageLabelFormClosed;
			f.Show();
			var pl = labels.Find(position.Page);
			if (!pl.IsEmpty) {
				f.SetValues(pl);
			}
		}

		static void InsertPageLabelFormClosed(object sender, EventArgs e) {
			var form = sender as InsertPageLabelForm;
			form.FormClosing -= InsertPageLabelFormClosed;
			if (form.DialogResult == DialogResult.Cancel) {
				return;
			}
			var l = form.Model.PageLabels;
			if (form.DialogResult == DialogResult.OK) {
				if (l == null) {
					return;
				}
				l.Add(form.PageLabel);
			}
			else if (form.DialogResult == DialogResult.Abort) {
				form.Model.PageLabels.Remove(form.PageLabel);
			}
			var pl = form.Model.Document.PageLabelRoot;
			pl.InnerText = String.Empty;
			foreach (var item in l) {
				pl.AppendChild(form.Model.Document.CreatePageLabel(item));
			}
			form.Viewer.PageLabels = form.Model.PageLabels;
			form.Viewer.Invalidate();
		}
	}
}
