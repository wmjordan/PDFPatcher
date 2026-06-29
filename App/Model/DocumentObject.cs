using System;
using System.Collections.Generic;
using System.Linq;
using CLR;
using PDFPatcher.Common;
using PDFPatcher.Processor;
using MuPDF;
using MuPDF.Extensions;
using PDFPatcher.Processor.ContentParser;

namespace PDFPatcher.Model
{
	[System.Diagnostics.DebuggerDisplay("Name = {Name}({FriendlyName}); Value = {Value}; {HasChildren}")]
	public sealed class DocumentObject : IHierarchicalObject<DocumentObject>
	{
		static readonly string[] __ReversalRefNames = ["Parent", "Prev", "First", "Last", "P"];
		static readonly DocumentObject[] __Leaf = [];

		internal PdfPathDocument OwnerDocument { get; }
		internal DocumentObject Parent { get; }
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
					&& (Type.CeqAny(PdfObjectType.Trailer, PdfObjectType.Pages, PdfObjectType.Page, PdfObjectType.PageCommands, PdfObjectType.Hidden)
						|| Type == PdfObjectType.PageCommand && Children.Count > 0)) {
					return true;
				}
				var po = Value ?? ExtensiveObject as PdfObject;
				if (po is null) {
					return false;
				}
				if (po.IsContainer()) {
					return true;
				}
				if (po.TypeKind == Kind.Reference) {
					if (Type == PdfObjectType.GoToPage
						|| ExtensiveObject is not PdfObject r
						|| r.TypeKind == Kind.Dictionary && Parent.Type == PdfObjectType.Outline && Name == "Next") {
						return false;
					}
					return r.TypeKind == Kind.Dictionary && !__ReversalRefNames.Contains(Name)
						|| r.TypeKind.CeqAny(Kind.Array, Kind.Stream)
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
					_Children ??= __Leaf;
				}
				return _Children;
			}
		}

		internal DocumentObject(PdfPathDocument ownerDocument, DocumentObject parent, string name, PdfObject value) :
			this(ownerDocument, parent, name, value, PdfObjectType.Normal) {
		}
		internal DocumentObject(PdfPathDocument ownerDocument, DocumentObject parent, string name, Token value) :
			this(ownerDocument, parent, name, null, PdfObjectType.Token) {
			FriendlyValue = value.ToString();
			ExtensiveObject = value;
		}
		internal DocumentObject(PdfPathDocument ownerDocument, DocumentObject parent, string name, PdfObject value, PdfObjectType type) {
			OwnerDocument = ownerDocument;
			Parent = parent;
			if (value is not null) {
				switch (value.TypeKind) {
					case Kind.Reference:
						if ((ExtensiveObject = value.UnderlyingObject) is PdfDictionary o) {
							if (o.TypeKind == Kind.Dictionary) {
								int page = ownerDocument.GetPageNumber(value as PdfReference);
								if (page >= 0) {
									Description = $"指向第 {page + 1} 页";
									type = PdfObjectType.GoToPage;
								}
								else if (o.IsType(PdfNames.Font) && parent.Name == "Font") {
									if (o.TryGet<PdfName>(PdfNames.BaseFont, out var fontName)) {
										Description = fontName.ToString(); // PdfName.DecodeName(fontName.ToString());
									}
								}
							}
							else if (o.TypeKind == Kind.Stream) {
								if (o.TryGet<PdfName>(PdfNames.Subtype, out var subType)) {
									if (subType.Equals(PdfNames.Image)) {
										type = PdfObjectType.Image;
									}
									else if (subType.Equals(PdfNames.Form)) {
										type = PdfObjectType.Form;
									}
									else if (parent?.Name == "AP"
										&& ((parent.Parent?.Value.UnderlyingObject) as PdfDictionary)?.IsType(PdfNames.Annot) == true) {
										type = PdfObjectType.Form;
									}
								}
							}
						}
						break;
					case Kind.Dictionary:
						if (parent?.Type.CeqAny(PdfObjectType.Page, PdfObjectType.Form) == true
							&& name == "Resources") {
							type = PdfObjectType.Resources;
						}
						break;
				}
			}
			Name = name; Value = value; Type = type;
		}

		internal bool RemoveChildByName(string name) {
			if (!HasChildren) {
				return false;
			}
			for (int i = _Children.Count - 1; i >= 0; i--) {
				if (_Children[i].Name == name) {
					if (_Children is Array) {
						_Children = new List<DocumentObject>(_Children);
					}
					_Children.RemoveAt(i);
					var po = Value;
					if (po is not null) {
						po = po.UnderlyingObject;
						switch (po.TypeKind) {
							case Kind.Array:
								((PdfArray)po).RemoveAt(i);
								break;
							case Kind.Dictionary:
							case Kind.Stream:
								((PdfDictionary)po).Remove(name);
								break;
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
				if (d.Value?.TypeKind == Kind.Reference) {
					return d;
				}
			} while ((d = d.Parent) != null);
			return null;
		}

		internal bool UpdateDocumentObject(object value) {
			if (Value is not PdfObject po) {
				return false;
			}
			switch (po.TypeKind) {
				case Kind.String:
					var s = value as string;
					if (s == ((PdfString)po).Value) {
						return false;
					}
					Value = new PdfString(s);
					break;
				case Kind.Integer:
				case Kind.Float:
					if (((string)value).TryParse(out float v)) {
						Value = new PdfFloat(v);
						break;
					}
					return false;
				case Kind.Name:
					if (Value is PdfName n && n.Name != (string)value) {
						Value = new PdfName((string)value);
						break;
					}
					return false;
				case Kind.Boolean:
					Value = ((bool)value) ? PdfBoolean.True : PdfBoolean.False;
					break;
			}
			if (Parent != null) {
				if ((Parent.ExtensiveObject ?? Parent.Value) is PdfDictionary pd) {
					pd[new PdfName(Name)] = Value;
					_Children = null;
					return true;
				}
				if ((Parent.ExtensiveObject ?? Parent.Value) is PdfArray pa) {
					pa[Int32.Parse(Name) - 1] = Value;
					_Children = null;
					return true;
				}
			}
			return false;
		}

		internal int IndexOfChild(DocumentObject child, bool sameName = false) {
			var l = Children as IList<DocumentObject>;
			var c = 0;
			if (!sameName) {
				c = l.IndexOf(child);
				return c < 0 ? c : c + 1;
			}
			var n = child.Name;
			for (int i = 0; i < l.Count; i++) {
				var o = l[i];
				if (o.Name == n) {
					++c;
					if (o == child) {
						return c;
					}
				}
			}
			return -1;
		}

		DocumentObject GetPageObject() {
			var p = this;
			while (p?.Type != PdfObjectType.Page) {
				p = p.Parent;
			}
			return p;
		}

		private static string GetItemValueText(PdfObject po, PdfObject eo) {
			if (po == null && eo == null) {
				goto Exit;
			}
			if (po == null) {
				po = eo;
				eo = null;
			}
			switch (po.TypeKind) {
				case Kind.Dictionary: return $"<<{((PdfDictionary)po).Count} 子项>>";
				case Kind.Reference:
					return eo == null || eo.IsContainer()
						? ((PdfReference)po).ToString()
						: $"{(PdfReference)po}→{GetItemValueText(null, eo)}";
				case Kind.Name: return ((PdfName)po).Name;
				case Kind.Integer: return ((PdfInteger)po).Value.ToText();
				case Kind.Float: return ((PdfFloat)po).Value.ToText();
				case Kind.String: return ((PdfString)po).Decode(null);
				case Kind.Stream: goto case Kind.Dictionary;
				case Kind.Array: return ((PdfArray)po).GetArrayString();
				case Kind.Boolean: return ((PdfBoolean)po).ToString();
				case Kind.Null: return "Null";
			}
		Exit:
			return null;
		}

		internal string GetContextName() {
			var d = this;
			string contextName = null;
			if (d.Type != PdfObjectType.Normal) {
				switch (d.Type) {
					case PdfObjectType.Page: return "Page";
					case PdfObjectType.Image: return "Image";
					case PdfObjectType.Form: return "Form";
				}
			}
			while ((!d.IsKeyObject || String.IsNullOrEmpty(contextName = d.Name)) && (d = d.Parent) != null) {
			}
			return contextName;
		}

		internal IList<DocumentObject> PopulateChildren(bool refresh) {
			if (refresh) {
				_Children = null;
			}
			if (_Children == null) {
				if (Type == PdfObjectType.Page && Value == null) {
					Value = OwnerDocument.Document.GetPageDictionary((int)ExtensiveObject);
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
			if (po.TypeKind.CeqAny(Kind.Dictionary, Kind.Stream)) {
				var pd = po as PdfDictionary;
				var cs = Type.CeqAny(PdfObjectType.Page, PdfObjectType.Form); // 是否有 content stream
				var r = new DocumentObject[pd.Count + (cs ? 1 : 0)];
				var n = 0;
				foreach (var item in pd) {
					var d = new DocumentObject(OwnerDocument, this, item.Key.Name, item.Value);
					r[n++] = d;
					var i = PdfStructInfo.GetInfo(GetContextName(), d.Name);
					if (i.Name != null && i.IsKeyObject) {
						d.IsKeyObject = true;
					}
					if (!String.IsNullOrEmpty(i.ImageKey)) {
						d.ImageKey = i.ImageKey;
					}
				}
				if (cs) {
					r[n++] = new DocumentObject(OwnerDocument, this, Constants.Content.Operators, null, PdfObjectType.PageCommands) { IsKeyObject = true };
				}
				else {
					switch (Type) {
						case PdfObjectType.Trailer:
							Array.Find(r, (o) => o.Name == "Root")
								?.Type = PdfObjectType.Root;
							break;
						case PdfObjectType.Root:
							Array.Find(r, (o) => o.Name == "Outlines")
									?.Type = PdfObjectType.Outline;
							break;
						case PdfObjectType.Outline: {
								var o = new List<DocumentObject>(r);
								if (pd.TryGet<PdfDictionary>(PdfNames.First, out pd)) {
									o.Add(new DocumentObject(OwnerDocument, this, Constants.Bookmark, pd, PdfObjectType.Outline) {
										Description = pd.Get<PdfString>(PdfNames.Title)?.Value
									});
									while (pd.TryGet<PdfDictionary>(PdfNames.Next, out pd)) {
										o.Add(new DocumentObject(OwnerDocument, this, Constants.Bookmark, pd, PdfObjectType.Outline) {
											Description = pd.Get<PdfString>(PdfNames.Title)?.Value
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
			else if (po.TypeKind == Kind.Array) {
				var pd = (PdfArray)po;
				var r = new DocumentObject[pd.Count];
				var n = 0;
				foreach (var item in pd) {
					var d = new DocumentObject(OwnerDocument, this, (++n).ToText(), item);
					r[n - 1] = d;
					var i = PdfStructInfo.GetInfo(GetContextName(), d.Name);
					if (i.Name != null && i.IsKeyObject) {
						d.IsKeyObject = true;
					}
					if (!String.IsNullOrEmpty(i.ImageKey)) {
						d.ImageKey = i.ImageKey;
					}
				}
				_Children = r;
			}
		}

		private void PopulateChildrenForSpecialObject() {
			var pdf = OwnerDocument.Document;
			switch (Type) {
				case PdfObjectType.Pages: {
						if (pdf.PageCount == 0) {
							return;
						}
						var r = PageRangeCollection.Parse(ExtensiveObject as string, 1, pdf.PageCount, true);
						var pn = new DocumentObject[r.TotalPages];
						var i = 0;
						foreach (var item in r) {
							foreach (var p in item) {
								pn[i++] = new DocumentObject(OwnerDocument, this, $"第{p}页", null, PdfObjectType.Page) { ExtensiveObject = p - 1 };
							}
						}
						_Children = pn;
						break;
					}
				case PdfObjectType.PageCommands: {
						// 解释页面指令
						using var cp = new ContentProcessor(pdf);
						IEnumerable<ContentState> contentStates;
						try {
							if (Parent.Type == PdfObjectType.Page) {
								contentStates = cp.Process((int)Parent.ExtensiveObject);
							}
							else if (Parent.Type == PdfObjectType.Form) {
								var form = Parent.Value.UnderlyingObject as PdfStream;
								var res = form.Get<PdfDictionary>(PdfNames.Resources);
								var pageRes = pdf.GetPageDictionary((int)GetPageObject().ExtensiveObject).Get<PdfDictionary>(PdfNames.Resources);
								Processor.ContentParser.ResourceStack rs = new(res);
								rs.Push(pageRes);
								contentStates = cp.Process(form.GetBytes(), rs);
							}
							else {
								return;
							}
						}
						catch (Exception ex) {
							Description = ex.Message;
							return;
						}
						new PageContentOperationPopulator(this).Process(contentStates);
						break;
					}
				case PdfObjectType.PageCommand:
					_Children = __Leaf;
					break;
				//case PdfObjectType.Hidden: // 未实现
				//		break;
				//
			}
		}

		void CreateChildrenList() {
			ref var list = ref _Children;
			if (list == null || list == __Leaf) {
				list = [];
			}
		}

		static DocumentObject MakeDocumentObjectFromCommand(ContentState item, DocumentObject container) {
			var op = item.Operation;
			var fn = op.Info.Description;
			var o = new DocumentObject(container.OwnerDocument, container, fn, null, PdfObjectType.PageCommand) {
				FriendlyName = fn + "(" + op.Operator + ")",
				ExtensiveObject = op
			};
			return o;
		}

		sealed class PageContentOperationPopulator(DocumentObject container)
		{
			readonly Stack<DocumentObject> _ContainerStack = [];
			DocumentObject _Container = container;

			public void Process(IEnumerable<ContentState> contentStates) {
				foreach (var item in contentStates) {
					PopulatePageCommand(item, _Container);
				}
			}

			void PopulatePageCommand(ContentState state, DocumentObject container) {
				int i;
				var o = MakeDocumentObjectFromCommand(state, container);
				switch (state.Operation.Kind) {
					case RenderCommandKind.ShowText:
					case RenderCommandKind.NextLineShowText:
					case RenderCommandKind.MoveToNextLineAndShowText:
					case RenderCommandKind.ShowTextWithSpacing:
						var t = state.Operation;
						o.FriendlyValue = t.Operands[0].ToString();
						o.Description = state.GraphicsState.Text;
						if (state.Operation.Kind == RenderCommandKind.ShowTextWithSpacing) {
							var a = (List<Token>)state.Operation.Operands[0].Value;
							if (a.Count > 0) {
								i = 0;
								o.CreateChildrenList();
								var sb = StringBuilderCache.Acquire();
								foreach (var ti in a) {
									var d = new DocumentObject(container.OwnerDocument, o, (++i).ToText(), ti);
									if (ti.Type == TokenType.String || ti.Type == TokenType.HexString) {
										d.FriendlyValue = ti.Value.ToString();
										state.GraphicsState.CurrentFont.DecodeText(ti.Buffer, ti.Offset, ti.Length, sb.Clear());
										d.Description = sb.ToString();
									}
									o._Children.Add(d);
								}
							}
						}
						break;
					case RenderCommandKind.SetFont:
						var f = state.GraphicsState;
						o.FriendlyValue = String.Concat(
							Constants.Content.OperandNames.ResourceName, "：", f.CurrentFontName, "; ",
							Constants.Content.OperandNames.Size, "：", f.FontSize.ToText()
							);
						o.Description = f.CurrentFontName;
						break;
					case RenderCommandKind.EndInlineImage:
						var s = (InlineImageContent)state.Operation.Operands[0].Value;
						container.CreateChildrenList();
						foreach (var ii in s.Dictionary) {
							container._Children.Add(new DocumentObject(container.OwnerDocument, o, ii.Key, ii.Value));
						}
						return;
					case RenderCommandKind.Unknown:
						o.ExtensiveObject = "?";
						if (state.Operation.Operands.HasContent()) {
							i = 0;
							o.CreateChildrenList();
							foreach (var op in state.Operation.Operands) {
								o._Children.Add(new DocumentObject(container.OwnerDocument, o, (++i).ToText(), op));
							}
						}
						break;
					default:
						if (state.Operation.Info.IsBeginScope) {
							if (state.Operation.Operands.HasContent()) {
								i = 0;
								o.CreateChildrenList();
								foreach (var op in state.Operation.Operands) {
									o._Children.Add(new DocumentObject(container.OwnerDocument, o, (++i).ToText(), op));
								}
							}
							_ContainerStack.Push(_Container);
							_Container = o;
						}
						else if (state.Operation.Info.IsEndScope) {
							_Container = _ContainerStack.Count != 0
								? _ContainerStack.Pop()
								: _Container; // weird path, should not be here
							return;
						}
						o.FriendlyValue = String.Join(" ", (object[])state.Operation.Operands);
						break;
				}
				container.CreateChildrenList();
				container._Children.Add(o);
			}
		}
	}
}
