using System;
using iTextSharp.text.pdf;

namespace PDFPatcher.Model
{
	internal sealed class PdfImageData : PdfDictionary
	{
		public int DataType { get; }
		public PdfIndirectReference PdfRef { get; }
		public byte[] RawBytes { get; }

		public PdfImageData(PRStream stream) {
			foreach (var item in stream) {
				Put(item.Key, item.Value);
			}
			RawBytes = PdfReader.GetStreamBytesRaw(stream);
			DataType = PdfObject.STREAM;
		}

		public PdfImageData(PdfIndirectReference pdfRef) {
			var s = PdfReader.GetPdfObjectRelease(pdfRef) as PRStream;
			foreach (var item in s) {
				Put(item.Key, item.Value);
			}
			PdfRef = pdfRef;
			RawBytes = PdfReader.GetStreamBytesRaw(s);
			DataType = PdfObject.INDIRECT;
		}

		public PdfImageData(PdfDictionary source, byte[] bytes) {
			foreach (var item in source) {
				Put(item.Key, item.Value);
			}
			RawBytes = bytes;
			DataType = PdfObject.NULL;
		}

		public override string ToString() {
			return (PdfRef != null ? $"{PdfRef.Generation} {PdfRef.Number}" : "<内嵌图像>") + " " + RawBytes.Length;
		}
	}

}
