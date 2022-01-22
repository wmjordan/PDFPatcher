using System.Drawing;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Processor.Imaging;

namespace PDFPatcher.Functions.Editor;

internal sealed class SavePageImageCommand : IEditorCommand
{
	private static SaveFileDialog _dialog;

	public void Process(Controller controller, params string[] parameters) {
		PdfViewerControl v = controller.View.Viewer;
		Point l = v.PinPoint;
		PagePosition p = v.TransposeVirtualImageToPagePosition(l.X, l.Y);
		_dialog = InitDialog();
		_dialog.FileName = ((FilePath)controller.Model.DocumentPath).FileNameWithoutExtension + "." + p.Page;

		if (_dialog.ShowDialog() != DialogResult.OK) {
			return;
		}

		_dialog.DefaultExt = ((FilePath)_dialog.FileName).FileExtension;
		Bitmap bmp = v.GetPageImage(p.Page);
		bmp.SaveAs(_dialog.FileName);
	}

	private static SaveFileDialog InitDialog() {
		return _dialog ?? (_dialog = new SaveFileDialog {
			DefaultExt = Constants.FileExtensions.Png,
			Filter = Constants.FileExtensions.ImageFilter
		});
	}
}