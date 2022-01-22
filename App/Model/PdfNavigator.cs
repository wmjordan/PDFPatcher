using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using iTextSharp.text.pdf;
using PDFPatcher.Common;

namespace PDFPatcher.Model;

internal sealed class PdfNavigator : XPathNavigator
{
	private readonly PdfPathDocument _doc;
	private readonly HtmlNameTable _nameTable = new();
	private int _childIndex = -1;
	private DocumentObject _currentObject;

	public PdfNavigator(PdfNavigator source) {
		_doc = source._doc;
		_currentObject = source._currentObject;
	}

	public PdfObject PdfObject => _currentObject.ExtensiveObject as PdfObject;

	private sealed class HtmlNameTable : XmlNameTable
	{
		private readonly NameTable _nameTable = new();

		public override string Add(string array) {
			return _nameTable.Add(array);
		}

		public override string Add(char[] array, int offset, int length) {
			return _nameTable.Add(array, offset, length);
		}

		public override string Get(string array) {
			return _nameTable.Get(array);
		}

		public override string Get(char[] array, int offset, int length) {
			return _nameTable.Get(array, offset, length);
		}

		internal string GetOrAdd(string array) {
			return Get(array) ?? Add(array);
		}
	}

	#region XPathNavigator implementation

	public override string BaseURI => string.Empty;

	public override XPathNavigator Clone() {
		return new PdfNavigator(this);
	}

	public override bool IsEmptyElement => !_currentObject.HasChildren;

	public override bool IsSamePosition(XPathNavigator other) {
		if (other is not PdfNavigator o) {
			return false;
		}

		return _currentObject == o._currentObject;
	}

	public override string LocalName => _nameTable.GetOrAdd(_currentObject.FriendlyName ?? _currentObject.Name);

	public override bool MoveTo(XPathNavigator other) {
		if (other is not PdfNavigator o) {
			return false;
		}

		if (_doc != o._doc) {
			return false;
		}

		_currentObject = o._currentObject;
		return true;
	}

	public override bool MoveToFirstAttribute() {
		return false;
	}

	public override bool MoveToFirstChild() {
		_currentObject.PopulateChildren(false);
		if (_currentObject.HasChildren == false) {
			return false;
		}

		_childIndex = 0;
		_currentObject = (_currentObject.Children as IList<DocumentObject>)[0];
		return true;
	}

	public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope) {
		return false;
	}

	public override bool MoveToId(string id) {
		if (!id.StartsWith("PAGE", StringComparison.OrdinalIgnoreCase)) {
			return false;
		}

		return id.Substring(4).TryParse(out int p) && p < _doc.PageCount;
	}

	public override bool MoveToNext() {
		if (_currentObject.Parent == null) {
			return false;
		}

		if (_childIndex >= _currentObject.Parent.Children.Count - 1) {
			return false;
		}

		_childIndex++;
		_currentObject = (_currentObject.Children as IList<DocumentObject>)[_childIndex];
		return true;

	}

	public override bool MoveToNextAttribute() {
		return false;
	}

	public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope) {
		return false;
	}

	public override bool MoveToParent() {
		if (_currentObject.Parent == null) {
			return false;
		}

		_currentObject = _currentObject.Parent;
		_childIndex = -1;
		return true;
	}

	public override bool MoveToPrevious() {
		if (_currentObject.Parent == null) {
			return false;
		}

		if (_childIndex <= 0) {
			return false;
		}

		_childIndex--;
		_currentObject = (_currentObject.Children as IList<DocumentObject>)[_childIndex];
		return true;

	}

	public override string Name => _nameTable.GetOrAdd(_currentObject.Name);

	public override XmlNameTable NameTable => _nameTable;

	public override string NamespaceURI => string.Empty;

	public override XPathNodeType NodeType {
		get {
			switch (_currentObject.Type) {
				case PdfObjectType.Normal:
					PdfObject po = _currentObject.ExtensiveObject as PdfObject;
					switch (po.Type) {
						case PdfObject.ARRAY:
						case PdfObject.DICTIONARY:
						case PdfObject.STREAM:
							return XPathNodeType.Element;
						default:
							return XPathNodeType.Attribute;
					}
				case PdfObjectType.Trailer:
					return XPathNodeType.Root;
				case PdfObjectType.Root:
				case PdfObjectType.Pages:
				case PdfObjectType.Page:
				case PdfObjectType.Image:
				case PdfObjectType.Outline:
				case PdfObjectType.PageCommand:
				case PdfObjectType.PageCommands:
				case PdfObjectType.GoToPage:
					return XPathNodeType.Element;
				case PdfObjectType.Hidden:
				default:
					return XPathNodeType.Comment;
			}
		}
	}

	public override string Prefix => string.Empty;

	public override string Value => _currentObject.FriendlyValue;

	#endregion

	#region XPathNavigator overrides

	public override void DeleteSelf() {
		DocumentObject p = _currentObject.Parent;
		p.Children.Remove(_currentObject);
		_currentObject = p;
		_childIndex = (p.Parent.Children as IList<DocumentObject>).IndexOf(p);
	}

	public override object UnderlyingObject => _currentObject;

	public override bool ValueAsBoolean => (PdfObject as PdfBoolean)?.BooleanValue == true;

	#endregion
}