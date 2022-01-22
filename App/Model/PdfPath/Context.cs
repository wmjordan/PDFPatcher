namespace PDFPatcher.Model.PdfPath
{
	sealed class Context
	{
		public DocumentObject Current { get; }
		public int Position { get; }

		public Context(DocumentObject currentObject, int position) {
			Current = currentObject;
			Position = position;
		}
	}
}
