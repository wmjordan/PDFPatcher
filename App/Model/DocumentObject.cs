﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Processor;

namespace PDFPatcher.Model;

[DebuggerDisplay("Name = {Name}({FriendlyName}); Value = {Value}; {HasChildren}")]
public sealed class DocumentObject : IHierarchicalObject<DocumentObject>
{
	private static readonly string[] __ReversalRefNames = {"Parent", "Prev", "First", "Last", "P"};
	private static readonly int[] __CompoundTypes = {PdfObject.DICTIONARY, PdfObject.ARRAY, PdfObject.STREAM};
	private static readonly DocumentObject[] __Leaf = new DocumentObject[0];

	private IList<DocumentObject> _Children;

	internal DocumentObject(PdfPathDocument ownerDocument, DocumentObject parent, string name, PdfObject value) :
		this(ownerDocument, parent, name, value, PdfObjectType.Normal) {
	}

	internal DocumentObject(PdfPathDocument ownerDocument, DocumentObject parent, string name, PdfObject value,
		PdfObjectType type) {
		OwnerDocument = ownerDocument;
		Parent = parent;
		if (value != null && value.Type == PdfObject.INDIRECT) {
			PdfObject r = PdfReader.GetPdfObjectRelease(value);
			if (r != null) {
				ExtensiveObject = r;
				if (r.Type == PdfObject.DICTIONARY) {
					int page = ownerDocument.GetPageNumber(value as PdfIndirectReference);
					if (page > 0) {
						Description = string.Concat("指向第 ", page, " 页");
						type = PdfObjectType.GoToPage;
					}
				}
				else if (r.Type == PdfObject.STREAM &&
				         PdfName.IMAGE.Equals(((PdfDictionary)r).GetAsName(PdfName.SUBTYPE))) {
					type = PdfObjectType.Image;
				}
			}
		}

		Name = name;
		Value = value;
		Type = type;
	}

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
	///     获取友好形式的名称。
	/// </summary>
	internal string FriendlyName { get; set; }

	/// <summary>
	///     获取友好形式的值。
	/// </summary>
	internal string FriendlyValue { get; set; }

	public string LiteralValue => GetItemValueText(Value, ExtensiveObject as PdfObject);

	public bool HasChildren {
		get {
			if (Type != PdfObjectType.Normal
			    && (Type == PdfObjectType.Trailer || Type == PdfObjectType.Pages || Type == PdfObjectType.Page ||
			        Type == PdfObjectType.PageCommands || Type == PdfObjectType.Hidden ||
			        (Type == PdfObjectType.PageCommand && Children.Count > 0))) {
				return true;
			}

			PdfObject po = Value ?? ExtensiveObject as PdfObject;
			if (po == null) {
				return false;
			}

			if (PdfHelper.CompoundTypes.Contains(po.Type)) {
				return true;
			}

			if (po.Type == PdfObject.INDIRECT) {
				if (Type == PdfObjectType.GoToPage) {
					return false;
				}

				PdfObject r = ExtensiveObject as PdfObject;
				if (r == null) {
					return false;
				}

				if (r.Type == PdfObject.DICTIONARY && Parent.Type == PdfObjectType.Outline && Name == "Next") {
					return false;
				}

				return (r.Type == PdfObject.DICTIONARY && __ReversalRefNames.Contains(Name) == false)
				       || r.Type == PdfObject.ARRAY
				       || r.Type == PdfObject.STREAM;
			}

			return false;
		}
	}

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
				PdfObject po = Value;
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
		DocumentObject d = this;
		do {
			if (d.Value != null && d.Value.Type == PdfObject.INDIRECT) {
				return d;
			}
		} while ((d = d.Parent) != null);

		return null;
	}

	internal bool UpdateDocumentObject(object value) {
		PdfObject po = Value;
		if (po == null) {
			return false;
		}

		switch (po.Type) {
			case PdfObject.STRING:
				string s = value as string;
				if (s == (po as PdfString).ToUnicodeString()) {
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
				Value = new PdfName((string)value);
				break;
			case PdfObject.BOOLEAN:
				Value = new PdfBoolean((bool)value);
				break;
		}

		if (Parent != null) {
			PdfDictionary pd = (Parent.ExtensiveObject ?? Parent.Value) as PdfDictionary;
			if (pd != null) {
				pd.Put(new PdfName(Name), Value);
				_Children = null;
				return true;
			}

			PdfArray pa = (Parent.ExtensiveObject ?? Parent.Value) as PdfArray;
			if (pa != null) {
				pa.ArrayList[int.Parse(Name) - 1] = Value;
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
			case PdfObject.DICTIONARY: return string.Concat("<<", (po as PdfDictionary).Size, " 子项>>");
			case PdfObject.INDIRECT:
				if (eo == null || __CompoundTypes.Contains(eo.Type)) {
					return (po as PdfIndirectReference).ToString();
				}
				else {
					return string.Concat((po as PdfIndirectReference).ToString(), "→", GetItemValueText(null, eo));
				}
			case PdfObject.NAME: return PdfHelper.DecodeKeyName(po);
			case PdfObject.NUMBER: return (po as PdfNumber).DoubleValue.ToText();
			case PdfObject.STRING: return (po as PdfString).Decode(null);
			case PdfObject.STREAM: goto case PdfObject.DICTIONARY;
			case PdfObject.ARRAY: return PdfHelper.GetArrayString(po as PdfArray);
			case PdfObject.BOOLEAN: return (po as PdfBoolean).ToString();
			case PdfObject.NULL: return "Null";
		}

		Exit:
		return null;
	}

	internal string GetContextName() {
		DocumentObject d = this;
		string contextName = null;
		if (d.Type != PdfObjectType.Normal) {
			if (d.Type == PdfObjectType.Page) {
				return "Page";
			}

			if (d.Type == PdfObjectType.Image) {
				return "Image";
			}
		}

		while ((d.IsKeyObject == false || string.IsNullOrEmpty(contextName = d.Name)) && (d = d.Parent) != null) {
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
		PdfObject po = ExtensiveObject as PdfObject ?? Value;
		_Children = __Leaf;
		if (po == null) {
			return;
		}

		if (po.Type == PdfObject.DICTIONARY || po.Type == PdfObject.STREAM) {
			PdfDictionary pd = po as PdfDictionary;
			DocumentObject[] r = new DocumentObject[pd.Size + (Type == PdfObjectType.Page ? 1 : 0)];
			int n = 0;
			foreach (KeyValuePair<PdfName, PdfObject> item in pd) {
				DocumentObject d = new(OwnerDocument, this, PdfHelper.DecodeKeyName(item.Key), item.Value);
				r[n++] = d;
				PdfStructInfo i = PdfStructInfo.GetInfo(GetContextName(), d.Name);
				if (i.Name != null && i.IsKeyObject) {
					d.IsKeyObject = true;
				}

				if (string.IsNullOrEmpty(i.ImageKey) == false) {
					d.ImageKey = i.ImageKey;
				}
			}

			if (Type != PdfObjectType.Normal) {
				if (Type == PdfObjectType.Page) {
					r[n++] = new DocumentObject(OwnerDocument, this, Constants.Content.Operators, null,
						PdfObjectType.PageCommands) {IsKeyObject = true};
				}
				else if (Type == PdfObjectType.Trailer) {
					DocumentObject d = Array.Find(r, o => { return o.Name == "Root"; });
					if (d != null) {
						d.Type = PdfObjectType.Root;
					}
				}
				else if (Type == PdfObjectType.Root) {
					DocumentObject d = Array.Find(r, o => { return o.Name == "Outlines"; });
					if (d != null) {
						d.Type = PdfObjectType.Outline;
					}
				}
				else if (Type == PdfObjectType.Outline) {
					List<DocumentObject> o = new(r);
					PdfObject or = pd.Get(PdfName.FIRST);
					pd = PdfReader.GetPdfObject(or) as PdfDictionary;
					if (pd != null) {
						o.Add(
							new DocumentObject(OwnerDocument, this, Constants.Bookmark, or, PdfObjectType.Outline) {
								Description = pd.Contains(PdfName.TITLE)
									? pd.GetAsString(PdfName.TITLE).ToUnicodeString()
									: null
							});
						while ((or = pd.Get(PdfName.NEXT)) != null &&
						       (pd = PdfReader.GetPdfObject(or) as PdfDictionary) != null) {
							o.Add(new DocumentObject(OwnerDocument, this, Constants.Bookmark, or,
								PdfObjectType.Outline) {
								Description = pd.Contains(PdfName.TITLE)
									? pd.GetAsString(PdfName.TITLE).ToUnicodeString()
									: null
							});
						}
					}

					_Children = o;
					return;
				}
			}

			_Children = r;
		}
		else if (po.Type == PdfObject.ARRAY) {
			PdfArray pd = po as PdfArray;
			DocumentObject[] r = new DocumentObject[pd.Size];
			int n = 0;
			foreach (PdfObject item in pd.ArrayList) {
				DocumentObject d = new(OwnerDocument, this, (++n).ToText(), item);
				r[n - 1] = d;
				PdfStructInfo i = PdfStructInfo.GetInfo(GetContextName(), d.Name);
				if (i.Name != null && i.IsKeyObject) {
					d.IsKeyObject = true;
				}

				if (string.IsNullOrEmpty(i.ImageKey) == false) {
					d.ImageKey = i.ImageKey;
				}
			}

			_Children = r;
		}
	}

	private void PopulateChildrenForSpecialObject() {
		PdfReader pdf = OwnerDocument.Document;
		if (Type == PdfObjectType.Pages) {
			if (pdf.NumberOfPages == 0) {
				return;
			}

			PageRangeCollection r = PageRangeCollection.Parse(ExtensiveObject as string, 1, pdf.NumberOfPages, true);
			DocumentObject[] pn = new DocumentObject[r.TotalPages];
			int i = 0;
			foreach (PageRange item in r) {
				foreach (int p in item) {
					pn[i++] = new DocumentObject(OwnerDocument, this, "第" + p + "页", null, PdfObjectType.Page) {
						ExtensiveObject = p
					};
				}
			}

			_Children = pn;
		}
		else if (Type == PdfObjectType.PageCommands) {
			// 解释页面指令
			int pn = (int)Parent.ExtensiveObject;
			PdfPageCommandProcessor cp = new();
			cp.ProcessContent(pdf.GetPageContent(pn), pdf.GetPageN(pn).GetAsDict(PdfName.RESOURCES));
			foreach (PdfPageCommand item in cp.Commands) {
				PopulatePageCommand(item);
			}
		}
		else if (Type == PdfObjectType.PageCommand) {
			_Children = __Leaf;
		}
		else if (Type == PdfObjectType.Hidden) {
			List<int> ul = PdfHelper.ListUnusedObjects(pdf, AppContext.LoadPartialPdfFile);
			ExtensiveObject = ul;
			List<DocumentObject> uo = new();
			foreach (int item in ul) {
				PdfObject u = pdf.GetPdfObjectRelease(item);
				if (u != null) {
					uo.Add(new DocumentObject(OwnerDocument, this, item.ToText(), u));
				}
			}

			_Children = uo;
		}
	}

	private void PopulatePageCommand(PdfPageCommand item) {
		string fn;
		string op = item.Name.ToString();
		if (PdfPageCommand.GetFriendlyCommandName(op, out fn) == false) {
			fn = "未知操作符";
		}

		DocumentObject o = new(OwnerDocument, this, fn, null, PdfObjectType.PageCommand) {
			FriendlyName = string.Concat(fn, "(", op, ")"), ExtensiveObject = op
		};
		if (item.Type == PdfPageCommandType.Text) {
			TextCommand t = item as TextCommand;
			o.FriendlyValue = t.TextInfo.PdfString.GetOriginalBytes().ToHexBinString();
			o.Description = t.TextInfo.Text;
			if (item.Name.ToString() == "TJ") {
				PdfArray a = item.Operands[0] as PdfArray;
				if (a.Size > 0) {
					PaceAndTextCommand pt = item as PaceAndTextCommand;
					int i = 0;
					CreateChildrenList(ref o._Children);
					foreach (PdfObject ti in a.ArrayList) {
						DocumentObject d = new(OwnerDocument, o, (++i).ToText(), ti);
						if (ti.Type == PdfObject.STRING) {
							d.FriendlyValue = (ti as PdfString).GetOriginalBytes().ToHexBinString();
							d.Description = pt.DecodedTexts[i - 1];
						}

						o._Children.Add(d);
					}
				}
			}
		}
		else if (item.Type == PdfPageCommandType.Font) {
			FontCommand f = item as FontCommand;
			o.FriendlyValue = string.Concat(
				f.FontName,
				" (", Constants.Content.OperandNames.ResourceName, "：", f.ResourceName.ToString(), "); ",
				Constants.Content.OperandNames.Size, "：", f.FontSize.DoubleValue.ToText()
			);
		}
		else if (item.Type == PdfPageCommandType.Enclosure) {
			if (item.Operands.HasContent()) {
				int i = 0;
				CreateChildrenList(ref o._Children);
				foreach (PdfObject t in item.Operands) {
					o._Children.Add(new DocumentObject(OwnerDocument, o, (++i).ToText(), t));
				}
			}

			EnclosingCommand e = item as EnclosingCommand;
			if (e.HasCommand == false) {
				return;
			}

			foreach (PdfPageCommand cmd in e.Commands) {
				o.PopulatePageCommand(cmd);
			}
		}
		else if (item.Type == PdfPageCommandType.InlineImage) {
			PdfImageData s = item.Operands[0] as PdfImageData;
			CreateChildrenList(ref o._Children);
			foreach (KeyValuePair<PdfName, PdfObject> ii in s) {
				o._Children.Add(new DocumentObject(OwnerDocument, o, PdfHelper.DecodeKeyName(ii.Key), ii.Value));
			}
		}
		else {
			o.FriendlyValue = item.GetOperandsText();
		}

		CreateChildrenList(ref _Children);
		_Children.Add(o);
	}

	private static void CreateChildrenList(ref IList<DocumentObject> list) {
		if (list == null || list == __Leaf) {
			list = new List<DocumentObject>();
		}
	}
}