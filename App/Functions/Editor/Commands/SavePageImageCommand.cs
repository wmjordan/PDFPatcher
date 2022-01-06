using System;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Processor.Imaging;

namespace PDFPatcher.Functions.Editor
{
	sealed class SavePageImageCommand : IEditorCommand
	{
		static SaveFileDialog _dialog;

		public void Process(Controller controller, params string[] parameters) {
			var v = controller.View.Viewer;
			var l = v.PinPoint;
			var p = v.TransposeVirtualImageToPagePosition(l.X, l.Y);
			_dialog = InitDialog();
			_dialog.FileName = ((FilePath)controller.Model.DocumentPath).FileNameWithoutExtension + "." + p.Page;

			if (_dialog.ShowDialog() == DialogResult.OK) {
				_dialog.DefaultExt = ((FilePath)_dialog.FileName).FileExtension;
				var bmp = v.GetPageImage(p.Page);
				bmp.SaveAs(_dialog.FileName);
			}
		}

		static SaveFileDialog InitDialog() {
			return _dialog ?? (_dialog = new SaveFileDialog {
				DefaultExt = Constants.FileExtensions.Png,
				Filter = Constants.FileExtensions.ImageFilter
			});
		}
	}
}
