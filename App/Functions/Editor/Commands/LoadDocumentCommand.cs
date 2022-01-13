using System;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor;

internal sealed class LoadDocumentCommand : IEditorCommand
{
	private readonly bool _showDialog, _importBookmark;

	public LoadDocumentCommand(bool showDialog, bool importBookmark) {
		_showDialog = showDialog;
		_importBookmark = importBookmark;
	}

	public void Process(Controller controller, params string[] parameters) {
		if (_showDialog) {
			using (OpenFileDialog f = new() {
				       DefaultExt = _importBookmark ? Constants.FileExtensions.Xml : Constants.FileExtensions.Pdf,
				       Title = _importBookmark ? "打开需要导入的书签文件" : "打开需要编辑的文件",
				       Filter = Constants.FileExtensions.AllEditableFilter
			       }) {
				if (f.ShowDialog() != DialogResult.OK) {
					return;
				}

				parameters = new string[] {f.FileName};
			}
		}

		try {
			controller.LoadDocument(parameters[0], _importBookmark);
		}
		catch (Exception ex) {
			FormHelper.ErrorBox("在加载信息文件时出现错误：" + ex.Message);
		}
	}
}