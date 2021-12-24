using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Functions.Editor
{
	sealed class DocumentPropertyCommand : IEditorCommand
	{
		public void Process (Controller controller, params string[] parameters) {
			using (var f = new DocumentInfoForm () {
				Document = controller.Model.PdfDocument,
				InfoDucument = controller.Model.Document
			}) {
				f.ShowDialog ();
			}
		}

	}
}
