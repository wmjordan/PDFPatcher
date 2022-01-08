using System;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor
{
	sealed class OcrPageCommand : IEditorCommand
	{
		public void Process(Controller controller, params string[] parameters) {
			var v = controller.View.Viewer;
			var pp = v.TransposeVirtualImageToPagePosition(v.PinPoint.X, v.PinPoint.Y);
			if (pp.Page == 0) {
				return;
			}
			var or = v.OcrPage(pp.Page, true);
			if (or != null) {
				Clipboard.SetText(String.Join(Environment.NewLine, v.CleanUpOcrResult(or)));
			}
			else {
				FormHelper.InfoBox("页面不包含可识别的文本，或出现识别引擎错误。");
			}
		}

	}
}
