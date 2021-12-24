using System;
using iTextSharp.text.pdf;

namespace PDFPatcher.Model
{
	internal sealed class PdfImageData : PdfDictionary
	{
		PdfIndirectReference _pdfRef;
		byte[] _bytes;
		public int DataType { get; private set; }
		public PdfIndirectReference PdfRef { get { return _pdfRef; } }
		public byte[] RawBytes {
			get {
				return _bytes;
			}
		}

		public PdfImageData (PRStream stream) {
			foreach (var item in stream) {
				this.Put (item.Key, item.Value);
			}
			_bytes = PdfReader.GetStreamBytesRaw (stream);
			this.DataType = PdfObject.STREAM;
		}

		public PdfImageData (PdfIndirectReference pdfRef) {
			var s = PdfReader.GetPdfObjectRelease (pdfRef) as PRStream;
			foreach (var item in s) {
				this.Put (item.Key, item.Value);
			}
			_pdfRef = pdfRef;
			_bytes = PdfReader.GetStreamBytesRaw (s);
			this.DataType = PdfObject.INDIRECT;
		}

		public PdfImageData (PdfDictionary source, byte[] bytes) {
			foreach (var item in source) {
				this.Put (item.Key, item.Value);
			}
			_bytes = bytes;
			this.DataType = PdfObject.NULL;
		}

		public override string ToString () {
			return (_pdfRef != null ? String.Concat (_pdfRef.Generation, " ", _pdfRef.Number) : "<内嵌图像>") + " " + _bytes.Length;
		}
	}

}
