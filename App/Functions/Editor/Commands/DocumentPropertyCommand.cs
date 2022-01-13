﻿namespace PDFPatcher.Functions.Editor;

internal sealed class DocumentPropertyCommand : IEditorCommand
{
	public void Process(Controller controller, params string[] parameters) {
		using (DocumentInfoForm f = new() {
			       Document = controller.Model.PdfDocument, InfoDucument = controller.Model.Document
		       }) {
			f.ShowDialog();
		}
	}
}