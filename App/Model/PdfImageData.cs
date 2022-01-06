using System;
using iTextSharp.text.pdf;

namespace PDFPatcher.Model
{
	internal sealed class PdfImageData : PdfDictionary
	{
		readonly PdfIndirectReference _pdfRef;
		readonly byte[] _bytes;
		public int DataType { get; private set; }
		public PdfIndirectReference PdfRef => _pdfRef;
		public byte[] RawBytes => _bytes;

		public PdfImageData(PRStream stream) {
			foreach (var item in stream) {
				Put(item.Key, item.Value);
			}
			_bytes = PdfReader.GetStreamBytesRaw(stream);
			DataType = PdfObject.STREAM;
		}

		public PdfImageData(PdfIndirectReference pdfRef) {
			var s = PdfReader.GetPdfObjectRelease(pdfRef) as PRStream;
			foreach (var item in s) {
				Put(item.Key, item.Value);
			}
			_pdfRef = pdfRef;
			_bytes = PdfReader.GetStreamBytesRaw(s);
			DataType = PdfObject.INDIRECT;
		}

		public PdfImageData(PdfDictionary source, byte[] bytes) {
			foreach (var item in source) {
				Put(item.Key, item.Value);
			}
			_bytes = bytes;
			DataType = PdfObject.NULL;
		}

		public override string ToString() {
			return (_pdfRef != null ? String.Concat(_pdfRef.Generation, " ", _pdfRef.Number) : "<内嵌图像>") + " " + _bytes.Length;
		}
	}

}
