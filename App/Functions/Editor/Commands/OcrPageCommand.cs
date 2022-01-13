using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor;

internal sealed class OcrPageCommand : IEditorCommand
{
	public void Process(Controller controller, params string[] parameters) {
		PdfViewerControl v = controller.View.Viewer;
		PagePosition pp = v.TransposeVirtualImageToPagePosition(v.PinPoint.X, v.PinPoint.Y);
		if (pp.Page == 0) {
			return;
		}

		List<TextLine> or = v.OcrPage(pp.Page, true);
		if (or != null) {
			Clipboard.SetText(string.Join(Environment.NewLine, v.CleanUpOcrResult(or)));
		}
		else {
			FormHelper.InfoBox("页面不包含可识别的文本，或出现识别引擎错误。");
		}
	}
}