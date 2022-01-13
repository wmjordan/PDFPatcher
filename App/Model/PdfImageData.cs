using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;

namespace PDFPatcher.Model;

internal sealed class PdfImageData : PdfDictionary
{
	private readonly PdfIndirectReference _pdfRef;
	private readonly byte[] _bytes;
	public int DataType { get; private set; }
	public PdfIndirectReference PdfRef => _pdfRef;
	public byte[] RawBytes => _bytes;

	public PdfImageData(PRStream stream) {
		foreach (KeyValuePair<PdfName, PdfObject> item in stream) {
			Put(item.Key, item.Value);
		}

		_bytes = PdfReader.GetStreamBytesRaw(stream);
		DataType = STREAM;
	}

	public PdfImageData(PdfIndirectReference pdfRef) {
		PRStream s = PdfReader.GetPdfObjectRelease(pdfRef) as PRStream;
		foreach (KeyValuePair<PdfName, PdfObject> item in s) {
			Put(item.Key, item.Value);
		}

		_pdfRef = pdfRef;
		_bytes = PdfReader.GetStreamBytesRaw(s);
		DataType = INDIRECT;
	}

	public PdfImageData(PdfDictionary source, byte[] bytes) {
		foreach (KeyValuePair<PdfName, PdfObject> item in source) {
			Put(item.Key, item.Value);
		}

		_bytes = bytes;
		DataType = NULL;
	}

	public override string ToString() {
		return (_pdfRef != null ? string.Concat(_pdfRef.Generation, " ", _pdfRef.Number) : "<内嵌图像>") + " " +
		       _bytes.Length;
	}
}