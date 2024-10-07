using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDFPatcher.Processor
{
	sealed class DocProcessorContext(PdfProcessingEngine engine, PdfWriter writer, Document outputDocument)
	{
		internal const int OcrData = 9010;
		internal const int CoordinateTransition = 9020;

		public PdfReader Pdf { get; } = engine.Pdf;
		public PdfWriter Writer { get; } = writer;
		public Document OutputDocument { get; } = outputDocument;
		public bool IsModified { get; set; }
		public Dictionary<int, object> ExtraData { get; } = engine.ExtraData;

		public DocProcessorContext(PdfProcessingEngine engine, PdfWriter writer)
			: this(engine, writer, null) {
		}
	}
}
