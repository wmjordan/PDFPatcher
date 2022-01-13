﻿using System;
using System.Collections.Generic;
using System.Drawing;
using PDFPatcher.Common;

namespace MuPdfSharp;

public sealed class RenderResultCache : IDisposable
{
	private const int __bufferSize = 10;
	private readonly MuDocument _document;
	private Dictionary<int, RenderResult> _buffer = new(__bufferSize);

	public RenderResultCache(MuDocument document) {
		_document = document;
	}

	public object SyncObj { get; } = new();

	public void Dispose() {
		Clear();
		_buffer = null;
	}

	public MuPage LoadPage(int pageNumber) {
		if (_buffer.ContainsKey(pageNumber)) {
			return _buffer[pageNumber].Page;
		}

		MuPage p = _document.LoadPage(pageNumber);
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
			RenderResult i = _buffer[pageNumber];
			if (i.Image != null) {
				i.Image.Dispose();
			}
		}

		_buffer[pageNumber].Image = bmp;
		TrimBitmapBuffer(pageNumber);
	}

	private void TrimBitmapBuffer(int pageNumber) {
		if (_buffer.Count > __bufferSize) {
			int x = 0;
			int i = 0;
			foreach (int item in _buffer.Keys) {
				if (Math.Abs(item - pageNumber) > x) {
					x = Math.Abs(item - pageNumber);
					i = item;
				}
			}

			_buffer[i].Dispose();
			_buffer.Remove(i);
			//Tracker.DebugMessage ("removed buffered result " + i);
		}
	}

	public void Clear() {
		if (_buffer.HasContent() == false) {
			return;
		}

		foreach (KeyValuePair<int, RenderResult> item in _buffer) {
			//Tracker.DebugMessage ("Disposing page " + item.Key + " result.");
			item.Value.Dispose();
		}

		_buffer.Clear();
	}

	private sealed class RenderResult : IDisposable
	{
		public RenderResult(MuPage page) {
			Page = page;
		}

		public MuPage Page { get; }
		public Bitmap Image { get; internal set; }

		public void Dispose() {
			if (Page != null) {
				Page.Dispose();
			}

			if (Image != null) {
				Image.Dispose();
			}
		}
	}
}