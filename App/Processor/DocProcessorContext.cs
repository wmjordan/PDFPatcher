using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDFPatcher.Processor;

internal sealed class DocProcessorContext
{
	internal const int OcrData = 9010;
	internal const int CoordinateTransition = 9020;

	public DocProcessorContext(PdfProcessingEngine engine, PdfWriter writer, Document outputDocument) {
		Pdf = engine.Pdf;
		Writer = writer;
		OutputDocument = outputDocument;
		ExtraData = engine.ExtraData;
	}

	public DocProcessorContext(PdfProcessingEngine engine, PdfWriter writer)
		: this(engine, writer, null) {
	}

	public PdfReader Pdf { get; }
	public PdfWriter Writer { get; }
	public Document OutputDocument { get; }
	public bool IsModified { get; set; }
	public Dictionary<int, object> ExtraData { get; }
}