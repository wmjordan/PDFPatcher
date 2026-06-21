using System;
using System.Collections.Generic;
using MuPDF;

namespace PDFPatcher.Processor.ContentParser;

sealed class ResourceStack
{
	readonly Stack<PdfDictionary> _stack = new();

	public ResourceStack(PdfDictionary pageResources) {
		if (pageResources is not null) {
			_stack.Push(pageResources);
		}
	}

	public PdfDictionary Current => _stack.Count != 0 ? _stack.Peek() : null;

	public void Push(PdfDictionary res) {
		if (res is not null) {
			_stack.Push(res);
		}
	}

	public PdfDictionary Pop() => _stack.Pop();

	public PdfDictionary LookupResource(PdfNames resourceType, string resName) {
		foreach (var layer in _stack) {
			var res = layer.GetValue(resourceType).UnderlyingObject;
			if (res is PdfDictionary dict) {
				return dict.GetValue(resName).UnderlyingObject as PdfDictionary;
			}
		}
		return null;
	}

	// 同理 GetXObject, GetExtGState ...
}
