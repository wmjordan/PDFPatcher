using System;
using System.Collections.Generic;
using MuPDF;
using PDFPatcher.Processor;

namespace PDFPatcher.Model;

sealed class PdfPathDocument : IHierarchicalObject<DocumentObject>
{
	const int pageGroupNumber = 100;
	readonly Document _pdf;
	readonly DocumentObject[] _rootObjects;
	readonly DocumentObject _trailer;
	readonly DocumentObject _hiddenObjects;

	public PdfPathDocument(string pdfPath) {
		_pdf = MuPdfHelper.OpenMuDocument(pdfPath);
		_trailer = new DocumentObject(this, null, "Trailer", _pdf.Trailer, PdfObjectType.Trailer) {
			IsKeyObject = true,
			Description = "文档根节点",
			FriendlyValue = System.IO.Path.GetFileNameWithoutExtension(pdfPath)
		};
		_hiddenObjects = new DocumentObject(this, null, "隐藏对象", null, PdfObjectType.Hidden);
		var l = _pdf.PageCount;
		if (l > 301) {
			var c = new DocumentObject[1 + (l + pageGroupNumber - 1) / pageGroupNumber + 1];
			c[0] = _trailer;
			for (int i = 1; i < c.Length - 1; i++) {
				var a = (i - 1) * pageGroupNumber + 1;
				var b = Math.Min(l, i * pageGroupNumber);
				c[i] = new DocumentObject(this, null, "Pages", null, PdfObjectType.Pages) { IsKeyObject = true, ExtensiveObject = $"{a}-{b}", FriendlyValue = $"第 {a} 至 {b} 页，共 {l} 页" };
			}
			c[c.Length - 1] = _hiddenObjects;
			_rootObjects = c;
		}
		else {
			_rootObjects = [
				_trailer,
				new DocumentObject (this, null, "Pages", null, PdfObjectType.Pages) { IsKeyObject = true, FriendlyValue = "共 " + l + " 页" },
				_hiddenObjects
			];
		}
	}

	public Document Document => _pdf;

	public DocumentObject Trailer => _trailer;

	public int PageCount => _pdf.PageCount;

	public void Close() {
		_pdf.Dispose();
	}

	public int GetPageNumber(PdfReference pdfRef) {
		return Document.LookupPageNumber(pdfRef);
	}

	#region IHierarchicalObject<DocumentObject> 成员

	bool IHierarchicalObject<DocumentObject>.HasChildren => true;

	ICollection<DocumentObject> IHierarchicalObject<DocumentObject>.Children {
		get {
			var c = new DocumentObject[_rootObjects.Length];
			Array.Copy(_rootObjects, c, _rootObjects.Length);
			return c;
		}
	}

	#endregion
}
