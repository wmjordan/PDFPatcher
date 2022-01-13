using System.Collections.Generic;
using iTextSharp.text.pdf;

namespace PDFPatcher.Model;

internal sealed class PdfImageData : PdfDictionary
{
	public PdfImageData(PRStream stream) {
		foreach (KeyValuePair<PdfName, PdfObject> item in stream) {
			Put(item.Key, item.Value);
		}

		RawBytes = PdfReader.GetStreamBytesRaw(stream);
		DataType = STREAM;
	}

	public PdfImageData(PdfIndirectReference pdfRef) {
		PRStream s = PdfReader.GetPdfObjectRelease(pdfRef) as PRStream;
		foreach (KeyValuePair<PdfName, PdfObject> item in s) {
			Put(item.Key, item.Value);
		}

		PdfRef = pdfRef;
		RawBytes = PdfReader.GetStreamBytesRaw(s);
		DataType = INDIRECT;
	}

	public PdfImageData(PdfDictionary source, byte[] bytes) {
		foreach (KeyValuePair<PdfName, PdfObject> item in source) {
			Put(item.Key, item.Value);
		}

		RawBytes = bytes;
		DataType = NULL;
	}

	public int DataType { get; }
	public PdfIndirectReference PdfRef { get; }

	public byte[] RawBytes { get; }

	public override string ToString() {
		return (PdfRef != null ? string.Concat(PdfRef.Generation, " ", PdfRef.Number) : "<内嵌图像>") + " " +
		       RawBytes.Length;
	}
}