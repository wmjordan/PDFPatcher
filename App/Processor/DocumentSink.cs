using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	/// <summary>
	/// 用于合并 PDF 文档的文档池。
	/// </summary>
	sealed class DocumentSink
	{
		readonly Dictionary<string, PdfReaderReference> _sink = new Dictionary<string, PdfReaderReference> (StringComparer.OrdinalIgnoreCase);
		public int Workload { get; private set; }
		public bool HasDuplicateFiles { get; private set; }
		public DocumentSink (IEnumerable<SourceItem> items, bool useSink) {
			EvaluateWorkload (items, useSink);
		}

		public PdfReader GetPdfReader (string path) {
			if (_sink.TryGetValue(path, out PdfReaderReference rr)) {
				if (rr.Reader == null) {
					rr.Reader = PdfHelper.OpenPdfFile(path, AppContext.LoadPartialPdfFile, false);
				}
				return rr.Reader;
			}
			return null;
		}

		public int DecrementReference (string path) {
			if (_sink.TryGetValue(path, out PdfReaderReference r)) {
				var c = --r.Reference;
				if (c == 0) {
					_sink.Remove(path);
				}
				return c;
			}
			return 0;
		}

		private void EvaluateWorkload (IEnumerable<SourceItem> items, bool useSink) {
			foreach (var item in items) {
				switch (item.Type) {
					case SourceItem.ItemType.Empty:
						break;
					case SourceItem.ItemType.Pdf:
						var p = item as SourceItem.Pdf;
						Workload += PageRangeCollection.Parse (p.PageRanges, 1, p.PageCount, true).TotalPages;
						if (useSink) {
							if (_sink.TryGetValue(item.FilePath.ToString(), out PdfReaderReference r)) {
								r.Reference++;
								HasDuplicateFiles = true;
								break;
							}
							_sink.Add (item.FilePath.ToString(), new PdfReaderReference ());
						}
						Workload += item.FileSize;
						break;
					case SourceItem.ItemType.Image:
						Workload += item.FileSize;
						break;
					case SourceItem.ItemType.Folder:
						break;
				}
				if (item.HasSubItems) {
					EvaluateWorkload (item.Items, useSink);
				}
			}
		}

		sealed class PdfReaderReference
		{
			internal PdfReader Reader;
			internal int Reference = 1;
		}
	}
}
