namespace PDFPatcher.Model.PdfPath;

internal sealed class Context
{
	public Context(DocumentObject currentObject, int position) {
		Current = currentObject;
		Position = position;
	}

	public DocumentObject Current { get; }
	public int Position { get; }
}