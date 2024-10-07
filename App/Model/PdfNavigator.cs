using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using iTextSharp.text.pdf;
using PDFPatcher.Common;

namespace PDFPatcher.Model
{
	sealed class PdfNavigator : XPathNavigator
	{
		sealed class HtmlNameTable : XmlNameTable
		{
			readonly NameTable _nameTable = new();
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

		readonly PdfPathDocument _doc;
		readonly HtmlNameTable _nameTable = new HtmlNameTable();
		DocumentObject _currentObject;
		int _childIndex = -1;

		public PdfNavigator(PdfPathDocument document) {
			_doc = document;
		}

		public PdfNavigator(PdfNavigator source) {
			_doc = source._doc;
			_currentObject = source._currentObject;
		}

		public PdfObject PdfObject => _currentObject.ExtensiveObject as PdfObject;

		#region XPathNavigator implementation
		public override string BaseURI => String.Empty;

		public override XPathNavigator Clone() {
			return new PdfNavigator(this);
		}

		public override bool IsEmptyElement => !_currentObject.HasChildren;

		public override bool IsSamePosition(XPathNavigator other) {
			return other is PdfNavigator o && _currentObject == o._currentObject;
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
			if (!_currentObject.HasChildren) {
				return false;
			}
			_childIndex = 0;
			_currentObject = ((IList<DocumentObject>)_currentObject.Children)[0];
			return true;
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope) {
			return false;
		}

		public override bool MoveToId(string id) {
			// 将当前对象设置为该页
			return id.HasCaseInsensitivePrefix("PAGE") && id.Substring(4).TryParse(out int p) && p < _doc.PageCount;
		}

		public override bool MoveToNext() {
			if (_currentObject.Parent == null) {
				return false;
			}
			if (_childIndex < _currentObject.Parent.Children.Count - 1) {
				_childIndex++;
				_currentObject = ((IList<DocumentObject>)_currentObject.Children)[_childIndex];
				return true;
			}
			return false;
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
			if (_childIndex > 0) {
				_childIndex--;
				_currentObject = ((IList<DocumentObject>)_currentObject.Children)[_childIndex];
				return true;
			}
			return false;
		}

		public override string Name => _nameTable.GetOrAdd(_currentObject.Name);

		public override XmlNameTable NameTable => _nameTable;

		public override string NamespaceURI => String.Empty;

		public override XPathNodeType NodeType {
			get {
				switch (_currentObject.Type) {
					case PdfObjectType.Normal:
						var po = _currentObject.ExtensiveObject as PdfObject;
						return po.Type switch {
							PdfObject.ARRAY or PdfObject.DICTIONARY or PdfObject.STREAM => XPathNodeType.Element,
							_ => XPathNodeType.Attribute,
						};
					case PdfObjectType.Trailer:
						return XPathNodeType.Root;
					case PdfObjectType.Root:
					case PdfObjectType.Pages:
					case PdfObjectType.Page:
					case PdfObjectType.Image:
					case PdfObjectType.Form:
					case PdfObjectType.Resources:
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

		public override string Prefix => String.Empty;

		public override string Value => _currentObject.FriendlyValue;
		#endregion

		#region XPathNavigator overrides
		public override void DeleteSelf() {
			var p = _currentObject.Parent;
			p.Children.Remove(_currentObject);
			_currentObject = p;
			_childIndex = ((IList<DocumentObject>)p.Parent.Children).IndexOf(p);
		}

		public override object UnderlyingObject => _currentObject;

		public override bool ValueAsBoolean => PdfObject is PdfBoolean { BooleanValue: true };


		#endregion

	}
}
