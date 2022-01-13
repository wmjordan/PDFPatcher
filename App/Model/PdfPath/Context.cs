using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model.PdfPath;

internal sealed class Context
{
	public DocumentObject Current { get; private set; }
	public int Position { get; private set; }

	public Context(DocumentObject currentObject, int position) {
		Current = currentObject;
		Position = position;
	}
}