using System;
using System.Collections.Generic;
using System.Drawing;
using PDFPatcher.Common;

namespace MuPdfSharp
{
	public sealed class RenderResultCache : IDisposable
	{
		const int __bufferSize = 10;
		readonly MuDocument _document;
		Dictionary<int, RenderResult> _buffer = new Dictionary<int, RenderResult>(__bufferSize);
		readonly object _SyncObj = new object();
		public object SyncObj => _SyncObj;

		public RenderResultCache(MuDocument document) {
			_document = document;
		}

		public MuPage LoadPage(int pageNumber) {
			if (_buffer.ContainsKey(pageNumber)) {
				return _buffer[pageNumber].Page;
			}
			var p = _document.LoadPage(pageNumber);
			_buffer.Add(pageNumber, new RenderResult(p));
			return p;
		}

		public Bitmap GetBitmap(int pageNumber) {
			if (_buffer == null || _document.IsDisposed) {
				return null;
			}
			RenderResult result;
			_buffer.TryGetValue(pageNumber, out result);
			return result != null ? result.Image : null;
		}

		public void AddBitmap(int pageNumber, Bitmap bmp) {
			if (_buffer.ContainsKey(pageNumber)) {
				var i = _buffer[pageNumber];
				i.Image?.Dispose();
			}
			_buffer[pageNumber].Image = bmp;
			TrimBitmapBuffer(pageNumber);
		}

		private void TrimBitmapBuffer(int pageNumber) {
			if (__bufferSize >= _buffer.Count) {
				return;
			}

			int x = 0;
			int i = 0;
			foreach (var item in _buffer.Keys) {
				if (Math.Abs(item - pageNumber) > x) {
					x = Math.Abs(item - pageNumber);
					i = item;
				}
			}
			_buffer[i].Dispose();
			_buffer.Remove(i);
			//Tracker.DebugMessage ("removed buffered result " + i);
		}

		public void Clear() {
			if (_buffer.HasContent() == false) {
				return;
			}
			foreach (var item in _buffer) {
				//Tracker.DebugMessage ("Disposing page " + item.Key + " result.");
				item.Value.Dispose();
			}
			_buffer.Clear();
		}

		public void Dispose() {
			Clear();
			_buffer = null;
		}

		sealed class RenderResult : IDisposable
		{
			public MuPage Page { get; private set; }
			public Bitmap Image { get; internal set; }

			public RenderResult(MuPage page) {
				Page = page;
			}

			public void Dispose() {
				Page?.Dispose();
				Image?.Dispose();
			}
		}
	}

}
