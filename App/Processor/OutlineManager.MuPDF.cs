using System;
using System.Xml;
using CLR;
using MuPDF;
using MuPDF.Extensions;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	partial class OutlineManager
	{
		/// <summary>
		/// 从 PDF 导出书签为 XML 元素。
		/// </summary>
		public static XmlElement GetBookmark(Document pdf, UnitConverter unitConverter) {
			var catalog = pdf.Root;
			var outlines = catalog.Get<PdfDictionary>(PdfNames.Outlines);
			if (outlines is null)
				return null;
			if (unitConverter == null) {
				throw new NullReferenceException("unitConverter");
			}
			var doc = new XmlDocument();
			using (var w = doc.AppendElement(Constants.DocumentBookmark).CreateNavigator().AppendChild()) {
				BookmarkDepth(
					pdf,
					pdf.LoadNameTree(PdfNames.Dests),
					new PdfActionExporter(unitConverter),
					(outlines.GetValue(PdfNames.First).UnderlyingObject) as PdfDictionary,
					w);
			}
			return doc.DocumentElement;
		}

		static void BookmarkDepth(Document pdf, PdfDictionary names, PdfActionExporter exporter, PdfDictionary outline, XmlWriter target) {
			while (Op.IsTrue(outline)) {
				target.WriteStartElement(Constants.Bookmark);

				target.WriteAttributeString(Constants.BookmarkAttributes.Title,
					StringHelper.ReplaceControlAndBomCharacters(outline.Get<PdfString>(PdfNames.Title).Decode(AppContext.Encodings.BookmarkEncoding))
					);

				var color = outline.Get<PdfArray>(PdfNames.C);
				DocInfoExporter.ExportColor(color, target);

				var style = outline.Get<PdfInteger>(PdfNames.F);
				if (Op.IsTrue(style)) {
					int f = style.Value & 0x03;
					if (f > 0) {
						target.WriteAttributeString(Constants.BookmarkAttributes.Style, Constants.BookmarkAttributes.StyleType.Names[f]);
					}
				}

				var count = outline.Get<PdfInteger>(PdfNames.Count);
				if (Op.IsTrue(count)) {
					target.WriteAttributeString(Constants.BookmarkAttributes.Open, count.Value < 0 ? Constants.Boolean.False : Constants.Boolean.True);
				}

				var dest = outline.Get<PdfObject>(PdfNames.Dest);
				if (dest.IsNull == false) {
					exporter.ExportGotoAction(pdf, dest, names, target);
				}
				else {
					exporter.ExportAction(pdf, outline.Get<PdfDictionary>(PdfNames.A), names, target);
				}
				var first = outline.Get<PdfDictionary>(PdfNames.First);
				if (first is not null) {
					BookmarkDepth(pdf, names, exporter, first, target);
				}
				outline = outline.Get<PdfDictionary>(PdfNames.Next);
				target.WriteEndElement();
			}
		}

		//private static Object[] CreateOutlines(Document target, PdfReference parent, XmlElement kids, int maxPageNumber, bool namedAsNames) {
		//	var bookmarks = kids.SelectNodes(Constants.Bookmark);
		//	var refs = new PdfReference[bookmarks.Count];
		//	for (int k = 0; k < refs.Length; ++k) {
		//		refs[k] = target.CreateObject();
		//	}

		//	int ptr = 0;
		//	int count = 0;
		//	foreach (XmlElement child in bookmarks) {
		//		object[] lower = null;
		//		if (child.SelectSingleNode(Constants.Bookmark) != null)
		//			lower = CreateOutlines(target, refs[ptr], child, maxPageNumber, namedAsNames);
		//		var outline = target.NewDictionary(4);
		//		++count;
		//		if (lower != null) {
		//			outline.Set(PdfNames.First, (PdfReference)lower[0]);
		//			outline.Set(PdfNames.Last, (PdfReference)lower[1]);
		//			int n = (int)lower[2];
		//			// 默认关闭书签
		//			if (child.GetAttribute(Constants.BookmarkAttributes.Open) != Constants.Boolean.True) {
		//				outline.Set(PdfNames.Count, -n);
		//			}
		//			else {
		//				outline.Set(PdfNames.Count, n);
		//				count += n;
		//			}
		//		}
		//		outline.Set(PdfNames.Parent, parent);
		//		if (ptr > 0) {
		//			outline.Set(PdfNames.Prev, refs[ptr - 1]);
		//		}
		//		if (ptr < refs.Length - 1) {
		//			outline.Set(PdfNames.Next, refs[ptr + 1]);
		//		}

		//		outline.Set(PdfNames.Title, child.GetAttribute(Constants.BookmarkAttributes.Title));
		//		DocInfoImporter.ImportColor(child, outline);
		//		var style = child.GetAttribute(Constants.BookmarkAttributes.Style);
		//		if (!String.IsNullOrEmpty(style)) {
		//			int bits = Array.IndexOf(Constants.BookmarkAttributes.StyleType.Names, style);
		//			if (bits == -1) {
		//				bits = 0;
		//			}
		//			if (bits != 0)
		//				outline.Set(PdfNames.F, bits);
		//		}
		//		DocInfoImporter.ImportAction(target, outline, child, maxPageNumber, namedAsNames);
		//		target.AddToBody(outline, refs[ptr]);
		//		++ptr;
		//	}
		//	return [refs[0], refs[refs.Length - 1], count];
		//}

		//internal static PdfReference WriteOutline(Document target, XmlElement bookmarks, int maxPageNumber) {
		//	if (bookmarks == null || bookmarks.SelectSingleNode(Constants.Bookmark) == null) {
		//		return null;
		//	}
		//	var top = target.NewDictionary(4);
		//	var topRef = target.PdfReference;
		//	var kids = CreateOutlines(target, topRef, bookmarks, maxPageNumber, false);
		//	top.Set(PdfNames.Type, PdfNames.Outlines);
		//	top.Set(PdfNames.First, (PdfReference)kids[0]);
		//	top.Set(PdfNames.Last, (PdfReference)kids[1]);
		//	top.Set(PdfNames.Count, (int)kids[2]);
		//	target.AddToBody(top, topRef);
		//	target.ExtraCatalog.Set(PdfNames.Outlines, topRef);
		//	return topRef;
		//}

		//internal static void KillOutline(Document pdf) {
		//	var catalog = pdf.Root;
		//	var o = catalog[PdfNames.Outlines];
		//	if (o.IsNull) {
		//		return;
		//	}
		//	if (o is PdfReference r) {
		//		OutlineTravel(pdf, r);
		//		pdf.DeleteObject(r.Number);
		//	}
		//	catalog.Remove(PdfNames.Outlines);
		//	if (PdfNames.UseOutlines.Equals(catalog[PdfNames.PageMode])) {
		//		catalog.Remove(PdfNames.PageMode);
		//	}
		//}

		//static void OutlineTravel(Document pdf, PdfReference outline) {
		//	while (!outline.IsNull) {
		//		var outlineR = outline.Resolve();
		//		if (outlineR is PdfDictionary d) {
		//			var first = d[PdfNames.First] as PdfReference;
		//			if (!first.IsNull) {
		//				OutlineTravel(pdf, first);
		//			}
		//			pdf.DeleteObject(d[PdfNames.Dest] as PdfReference);
		//			pdf.DeleteObject(d[PdfNames.A] as PdfReference);
		//			outline = d[PdfNames.Next] as PdfReference;
		//		}
		//		else {
		//			outline = null;
		//		}
		//	}
		//}
	}
}