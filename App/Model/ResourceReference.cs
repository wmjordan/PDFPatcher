using System;
using iTextSharp.text.pdf;

namespace PDFPatcher.Model
{
	readonly struct ResourceReference(PdfIndirectReference resourceRef, PdfName name, PdfDictionary resource)
	{
		public PdfIndirectReference Ref { get; } = resourceRef;
		public PdfName Name { get; } = name;
		public PdfDictionary Resource { get; } = resource;
	}
}
