using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Processor;

namespace PDFPatcher.Model
{
	[System.Diagnostics.DebuggerDisplay("Name = {Name}({FriendlyName}); Value = {Value}; {HasChildren}")]
	public sealed class DocumentObject : IHierarchicalObject<DocumentObject>
	{
		static readonly string[] __ReversalRefNames = new string[] { "Parent", "Prev", "First", "Last", "P" };
		static readonly int[] __CompoundTypes = new int[] { PdfObject.DICTIONARY, PdfObject.ARRAY, PdfObject.STREAM };
		static readonly DocumentObject[] __Leaf = new DocumentObject[0];

		internal PdfPathDocument OwnerDocument { get; private set; }
		internal DocumentObject Parent { get; private set; }
		internal string Name { get; set; }
		internal PdfObject Value { get; set; }
		internal string Description { get; set; }
		internal object ExtensiveObject { get; set; }
		internal PdfObjectType Type { get; private set; }
		internal bool IsKeyObject { get; set; }
		internal object ImageKey { get; set; }
		/// <summary>
		/// 获取友好形式的名称。
		/// </summary>
		internal string FriendlyName { get; set; }
		/// <summary>
		/// 获取友好形式的值。
		/// </summary>
		internal string FriendlyValue { get; set; }

		public string LiteralValue => GetItemValueText(Value, ExtensiveObject as PdfObject);
		public bool HasChildren {
			get {
				if (Type != PdfObjectType.Normal
					&& (Type == PdfObjectType.Trailer
						|| Type == PdfObjectType.Pages
						|| Type == PdfObjectType.Page
						|| Type == PdfObjectType.PageCommands
						|| Type == PdfObjectType.Hidden
						|| Type == PdfObjectType.PageCommand && Children.Count > 0)) {
					return true;
				}
				var po = Value ?? ExtensiveObject as PdfObject;
				if (po == null) {
					return false;
				}
				if (PdfHelper.CompoundTypes.Contains(po.Type)) {
					return true;
				}
				else if (po.Type == PdfObject.INDIRECT) {
					if (Type == PdfObjectType.GoToPage) {
						return false;
					}
					var r = ExtensiveObject as PdfObject;
					if (r == null) {
						return false;
					}
					if (r.Type == PdfObject.DICTIONARY && Parent.Type == PdfObjectType.Outline && Name == "Next") {
						return false;
					}
					return r.Type == PdfObject.DICTIONARY && __ReversalRefNames.Contains(Name) == false
						|| r.Type == PdfObject.ARRAY
						|| r.Type == PdfObject.STREAM
						;
				}
				return false;
			}
		}
		IList<DocumentObject> _Children;
		public ICollection<DocumentObject> Children {
			get {
				if (_Children == null) {
					PopulateChildren(false);
					if (_Children == null) {
						_Children = __Leaf;
					}
				}
				return _Children;
			}
		}

		internal DocumentObject(PdfPathDocument ownerDocument, DocumentObject parent, string name, PdfObject value) :
			this(ownerDocument, parent, name, value, PdfObjectType.Normal) {
		}
		internal DocumentObject(PdfPathDocument ownerDocument, DocumentObject parent, string name, PdfObject value, PdfObjectType type) {
			OwnerDocument = ownerDocument;
			Parent = parent;
			if (value != null) {
				if (value.Type == PdfObject.INDIRECT) {
					var r = PdfReader.GetPdfObjectRelease(value);
					if (r != null) {
						ExtensiveObject = r;
						if (r.Type == PdfObject.DICTIONARY) {
							int page = ownerDocument.GetPageNumber(value as PdfIndirectReference);
							if (page > 0) {
								Description = $"指向第 {page} 页";
								type = PdfObjectType.GoToPage;
							}
						}
						else if (r.Type == PdfObject.STREAM) {
							var subType = ((PdfDictionary)r).GetAsName(PdfName.SUBTYPE);
							if (PdfName.IMAGE.Equals(subType)) {
								type = PdfObjectType.Image;
							}
							else if (PdfName.FORM.Equals(subType)) {
								type = PdfObjectType.Form;
							}
							else if (parent?.Name == "AP"
								&& PdfName.ANNOT.Equals((PdfReader.GetPdfObjectRelease(parent.Parent?.Value) as PdfDictionary)?.GetAsName(PdfName.TYPE))) {
								type = PdfObjectType.Form;
							}
						}
					}
				}
				else if (value.Type == PdfObject.DICTIONARY) {
					if (parent != null && (parent.Type == PdfObjectType.Page || parent.Type == PdfObjectType.Form) && name == "Resources") {
						type = PdfObjectType.Resources;
					}
				}
			}
			Name = name; Value = value; Type = type;
		}

		internal bool RemoveChildByName(string name) {
			if (HasChildren == false) {
				return false;
			}
			for (int i = _Children.Count - 1; i >= 0; i--) {
				if (_Children[i].Name == name) {
					if (_Children is Array) {
						_Children = new List<DocumentObject>(_Children);
					}
					_Children.RemoveAt(i);
					var po = Value;
					if (po != null) {
						while (po.Type == PdfObject.INDIRECT) {
							po = PdfReader.GetPdfObject(po);
						}
						if (po.Type == PdfObject.ARRAY) {
							((PdfArray)po).Remove(i);
						}
						else if (po.Type == PdfObject.DICTIONARY || po.Type == PdfObject.STREAM) {
							((PdfDictionary)po).Remove(new PdfName(name));
						}
					}
					return true;
				}
			}
			return false;
		}

		internal DocumentObject FindReferenceAncestor() {
			var d = this;
			do {
				if (d.Value?.Type == PdfObject.INDIRECT) {
					return d;
				}
			} while ((d = d.Parent) != null);
			return null;
		}

		internal bool UpdateDocumentObject(object value) {
			var po = Value as PdfObject;
			if (po == null) {
				return false;
			}
			switch (po.Type) {
				case PdfObject.STRING:
					var s = value as string;
					if (s == ((PdfString)po).ToUnicodeString()) {
						break;
					}
					Value = s.ToPdfString();
					break;
				case PdfObject.NUMBER:
					double n;
					if (((string)value).TryParse(out n)) {
						Value = new PdfNumber(n);
						break;
					}
					return false;
				case PdfObject.NAME:
					Value = new PdfName((string)value); break;
				case PdfObject.BOOLEAN:
					Value = new PdfBoolean((bool)value); break;
			}
			if (Parent != null) {
				if ((Parent.ExtensiveObject ?? Parent.Value) is PdfDictionary pd) {
					pd.Put(new PdfName(Name), Value);
					_Children = null;
					return true;
				}
				if ((Parent.ExtensiveObject ?? Parent.Value) is PdfArray pa) {
					pa.ArrayList[Int32.Parse(Name) - 1] = Value;
					_Children = null;
					return true;
				}
			}
			return false;
		}

		private static string GetItemValueText(PdfObject po, PdfObject eo) {
			if (po == null && eo == null) {
				goto Exit;
			}
			if (po == null) {
				po = eo;
				eo = null;
			}
			switch (po.Type) {
				case PdfObject.DICTIONARY: return $"<<{((PdfDictionary)po).Size} 子项>>";
				case PdfObject.INDIRECT:
					return eo == null || __CompoundTypes.Contains(eo.Type)
						? ((PdfIndirectReference)po).ToString()
						: $"{(PdfIndirectReference)po}→{GetItemValueText(null, eo)}";
				case PdfObject.NAME: return PdfHelper.DecodeKeyName(po);
				case PdfObject.NUMBER: return ((PdfNumber)po).DoubleValue.ToText();
				case PdfObject.STRING: return ((PdfString)po).Decode(null);
				case PdfObject.STREAM: goto case PdfObject.DICTIONARY;
				case PdfObject.ARRAY: return PdfHelper.GetArrayString((PdfArray)po);
				case PdfObject.BOOLEAN: return ((PdfBoolean)po).ToString();
				case PdfObject.NULL: return "Null";
			}
		Exit:
			return null;
		}

		internal string GetContextName() {
			var d = this;
			string contextName = null;
			if (d.Type != PdfObjectType.Normal) {
				switch (d.Type) {
					case PdfObjectType.Page:
						return "Page";
					case PdfObjectType.Image:
						return "Image";
					case PdfObjectType.Form:
						return "Form";
				}
			}
			while ((d.IsKeyObject == false || String.IsNullOrEmpty(contextName = d.Name)) && (d = d.Parent) != null) {
			}
			return contextName;
		}

		internal IList<DocumentObject> PopulateChildren(bool refresh) {
			if (refresh) {
				_Children = null;
			}
			if (_Children == null) {
				if (Type == PdfObjectType.Page && Value == null) {
					Value = OwnerDocument.Document.GetPageN((int)ExtensiveObject);
				}
				else if (Type != PdfObjectType.Normal) {
					PopulateChildrenForSpecialObject();
				}
				if (_Children == null) {
					PopulateChildrenForNormalObject();
				}
			}
			return _Children;
		}

		private void PopulateChildrenForNormalObject() {
			var po = (ExtensiveObject as PdfObject) ?? Value;
			_Children = __Leaf;
			if (po == null) {
				return;
			}
			if (po.Type == PdfObject.DICTIONARY || po.Type == PdfObject.STREAM) {
				var pd = po as PdfDictionary;
				var cs = Type == PdfObjectType.Page || Type == PdfObjectType.Form; // 是否有 content stream
				var r = new DocumentObject[pd.Size + (cs ? 1 : 0)];
				var n = 0;
				foreach (var item in pd) {
					var d = new DocumentObject(OwnerDocument, this, PdfHelper.DecodeKeyName(item.Key), item.Value);
					r[n++] = d;
					var i = PdfStructInfo.GetInfo(GetContextName(), d.Name);
					if (i.Name != null && i.IsKeyObject) {
						d.IsKeyObject = true;
					}
					if (String.IsNullOrEmpty(i.ImageKey) == false) {
						d.ImageKey = i.ImageKey;
					}
				}
				if (cs) {
					r[n++] = new DocumentObject(OwnerDocument, this, Constants.Content.Operators, null, PdfObjectType.PageCommands) { IsKeyObject = true };
				}
				else {
					switch (Type) {
						case PdfObjectType.Trailer: {
								var d = Array.Find(r, (o) => o.Name == "Root");
								if (d != null) {
									d.Type = PdfObjectType.Root;
								}
							}
							break;
						case PdfObjectType.Root: {
								var d = Array.Find(r, (o) => o.Name == "Outlines");
								if (d != null) {
									d.Type = PdfObjectType.Outline;
								}
							}
							break;
						case PdfObjectType.Outline: {
								var o = new List<DocumentObject>(r);
								var or = pd.Get(PdfName.FIRST);
								pd = PdfReader.GetPdfObject(or) as PdfDictionary;
								if (pd != null) {
									o.Add(new DocumentObject(OwnerDocument, this, Constants.Bookmark, or, PdfObjectType.Outline) {
										Description = pd.Contains(PdfName.TITLE) ? pd.GetAsString(PdfName.TITLE).ToUnicodeString() : null
									});
									while ((or = pd.Get(PdfName.NEXT)) != null
										&& (pd = PdfReader.GetPdfObject(or) as PdfDictionary) != null) {
										o.Add(new DocumentObject(OwnerDocument, this, Constants.Bookmark, or, PdfObjectType.Outline) {
											Description = pd.Contains(PdfName.TITLE) ? pd.GetAsString(PdfName.TITLE).ToUnicodeString() : null
										});
									}
								}
								_Children = o;
								return;
							}
					}
				}
				_Children = r;
			}
			else if (po.Type == PdfObject.ARRAY) {
				var pd = (PdfArray)po;
				var r = new DocumentObject[pd.Size];
				var n = 0;
				foreach (var item in pd.ArrayList) {
					var d = new DocumentObject(OwnerDocument, this, (++n).ToText(), item);
					r[n - 1] = d;
					var i = PdfStructInfo.GetInfo(GetContextName(), d.Name);
					if (i.Name != null && i.IsKeyObject) {
						d.IsKeyObject = true;
					}
					if (String.IsNullOrEmpty(i.ImageKey) == false) {
						d.ImageKey = i.ImageKey;
					}
				}
				_Children = r;
			}
		}

		private void PopulateChildrenForSpecialObject() {
			var pdf = OwnerDocument.Document;
			if (Type == PdfObjectType.Pages) {
				if (pdf.NumberOfPages == 0) {
					return;
				}
				var r = PageRangeCollection.Parse(ExtensiveObject as string, 1, pdf.NumberOfPages, true);
				var pn = new DocumentObject[r.TotalPages];
				var i = 0;
				foreach (var item in r) {
					foreach (var p in item) {
						pn[i++] = new DocumentObject(OwnerDocument, this, $"第{p}页", null, PdfObjectType.Page) { ExtensiveObject = p };
					}
				}
				_Children = pn;
			}
			else if (Type == PdfObjectType.PageCommands) {
				// 解释页面指令
				var cp = new PdfPageCommandProcessor();
				if (Parent.Type == PdfObjectType.Page) {
					var pn = (int)Parent.ExtensiveObject;
					cp.ProcessContent(pdf.GetPageContent(pn), pdf.GetPageN(pn).GetAsDict(PdfName.RESOURCES));
				}
				else if (Parent.Type == PdfObjectType.Form) {
					var form = PdfReader.GetPdfObjectRelease(Parent.Value) as PRStream;
					cp.ProcessContent(PdfReader.GetStreamBytes(form), form.GetAsDict(PdfName.RESOURCES));
				}
				foreach (var item in cp.Commands) {
					PopulatePageCommand(item);
				}
			}
			else if (Type == PdfObjectType.PageCommand) {
				_Children = __Leaf;
			}
			else if (Type == PdfObjectType.Hidden) {
				var ul = PdfHelper.ListUnusedObjects(pdf, AppContext.LoadPartialPdfFile);
				ExtensiveObject = ul;
				var uo = new List<DocumentObject>();
				foreach (var item in ul) {
					var u = pdf.GetPdfObjectRelease(item);
					if (u != null) {
						uo.Add(new DocumentObject(OwnerDocument, this, item.ToText(), u));
					}
				}
				_Children = uo;
			}
		}

		private void PopulatePageCommand(PdfPageCommand item) {
			string fn;
			var op = item.Name.ToString();
			if (PdfPageCommand.GetFriendlyCommandName(op, out fn) == false) {
				fn = "未知操作符";
			}
			var o = new DocumentObject(OwnerDocument, this, fn, null, PdfObjectType.PageCommand) {
				FriendlyName = $"{fn}({op})",
				ExtensiveObject = op
			};
			if (item.Type == PdfPageCommandType.Text) {
				var t = item as TextCommand;
				o.FriendlyValue = t.TextInfo.PdfString.GetOriginalBytes().ToHexBinString();
				o.Description = t.TextInfo.Text;
				if (item.Name.ToString() == "TJ") {
					var a = item.Operands[0] as PdfArray;
					if (a.Size > 0) {
						var pt = item as PaceAndTextCommand;
						var i = 0;
						CreateChildrenList(ref o._Children);
						foreach (var ti in a.ArrayList) {
							var d = new DocumentObject(OwnerDocument, o, (++i).ToText(), ti);
							if (ti.Type == PdfObject.STRING) {
								d.FriendlyValue = ((PdfString)ti).GetOriginalBytes().ToHexBinString();
								d.Description = pt.DecodedTexts[i - 1];
							}
							o._Children.Add(d);
						}
					}
				}
			}
			else if (item.Type == PdfPageCommandType.Font) {
				var f = item as FontCommand;
				o.FriendlyValue = String.Concat(
					Constants.Content.OperandNames.ResourceName, "：", f.ResourceName.ToString(), "; ",
					Constants.Content.OperandNames.Size, "：", f.FontSize.DoubleValue.ToText()
					);
				o.Description = f.FontName;
			}
			else if (item.Type == PdfPageCommandType.Enclosure) {
				if (item.Operands.HasContent()) {
					var i = 0;
					CreateChildrenList(ref o._Children);
					foreach (var t in item.Operands) {
						o._Children.Add(new DocumentObject(OwnerDocument, o, (++i).ToText(), t));
					}
				}
				var e = (EnclosingCommand)item;
				if (e.HasCommand == false) {
					return;
				}
				foreach (var cmd in e.Commands) {
					o.PopulatePageCommand(cmd);
				}
			}
			else if (item.Type == PdfPageCommandType.InlineImage) {
				var s = (PdfImageData)item.Operands[0];
				CreateChildrenList(ref o._Children);
				foreach (var ii in s) {
					o._Children.Add(new DocumentObject(OwnerDocument, o, PdfHelper.DecodeKeyName(ii.Key), ii.Value));
				}
			}
			else {
				o.FriendlyValue = item.GetOperandsText();
			}
			CreateChildrenList(ref _Children);
			_Children.Add(o);
		}

		static void CreateChildrenList(ref IList<DocumentObject> list) {
			if (list == null || list == __Leaf) {
				list = new List<DocumentObject>();
			}
		}

	}
}
