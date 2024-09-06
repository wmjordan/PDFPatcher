using System;
using iTextSharp.text.pdf;

namespace PDFPatcher.Model
{
	readonly struct ResourceReference
	{
		public ResourceReference(PdfIndirectReference resourceRef, PdfName name, PdfDictionary resource) {
			Ref = resourceRef;
			Name = name;
			Resource = resource;
		}

		public PdfIndirectReference Ref { get; }
		public PdfName Name { get; }
		public PdfDictionary Resource { get; }
	}
}
