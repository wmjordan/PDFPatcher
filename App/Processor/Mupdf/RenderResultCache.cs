﻿using System;
using System.Collections.Generic;
using System.Drawing;
using PDFPatcher.Common;

namespace MuPDF
{
	public sealed class RenderResultCache : IDisposable
	{
		const int __bufferSize = 10;
		readonly Document _document;
		Dictionary<int, RenderResult> _buffer = new Dictionary<int, RenderResult>(__bufferSize);
		readonly object _SyncObj = new object();
		public object SyncObj => _SyncObj;

		public RenderResultCache(Document document) {
			_document = document;
		}

		public Page LoadPage(int pageNumber) {
			if (_buffer.TryGetValue(pageNumber, out var r)) {
				return r.Page;
			}
			var p = _document.LoadPage(pageNumber - 1);
			_buffer.Add(pageNumber, new RenderResult(p));
			return p;
		}

		public Bitmap GetBitmap(int pageNumber) {
			return TryGetRenderResult(pageNumber, out RenderResult result)
				? result.Image
				: null;
		}
		public TextPage GetTextPage(int pageNumber) {
			return TryGetRenderResult(pageNumber, out RenderResult result)
				? result.TextPage
				: null;
		}

		bool TryGetRenderResult(int pageNumber, out RenderResult result) {
			result = null;
			return _buffer != null
				&& !_document.IsDisposed
				&& _buffer.TryGetValue(pageNumber, out result);
		}

		public void SetBitmap(int pageNumber, Bitmap bmp) {
			if (_buffer.TryGetValue(pageNumber, out var image)) {
				image.Image?.Dispose();
			}
			_buffer[pageNumber].Image = bmp;
			TrimBitmapBuffer(pageNumber);
		}

		public void SetTextPage(int pageNumber, TextPage textPage) {
			if (_buffer.TryGetValue(pageNumber, out var image)) {
				image.TextPage?.Dispose();
			}
			_buffer[pageNumber].TextPage = textPage;
		}

		void TrimBitmapBuffer(int pageNumber) {
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
		}

		public void Clear() {
			if (!_buffer.HasContent()) {
				return;
			}
			foreach (var item in _buffer) {
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
			public Page Page { get; private set; }
			public Bitmap Image { get; internal set; }
			public TextPage TextPage { get; internal set; }

			public RenderResult(Page page) {
				Page = page;
			}

			public void Dispose() {
				Page?.Dispose();
				Image?.Dispose();
				TextPage?.Dispose();
			}
		}
	}

}
