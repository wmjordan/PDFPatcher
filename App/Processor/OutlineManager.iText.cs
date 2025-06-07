using System;
using System.Collections.Generic;
using System.Xml;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	partial class OutlineManager
	{
		/// <summary>
		/// 从 PDF 导出书签为 XML 元素。
		/// </summary>
		public static XmlElement GetBookmark(PdfReader reader, UnitConverter unitConverter) {
			var catalog = reader.Catalog;
			var outlines = catalog.Locate<PdfDictionary>(PdfName.OUTLINES);
			if (outlines == null)
				return null;
			if (unitConverter == null) {
				throw new NullReferenceException("unitConverter");
			}
			var pages = reader.GetPageRefMapper();
			var doc = new XmlDocument();
			doc.AppendElement(Constants.DocumentBookmark);
			var names = reader.GetNamedDestinations();
			using (var w = doc.DocumentElement.CreateNavigator().AppendChild()) {
				var a = new PdfActionExporter(unitConverter);
				BookmarkDepth(
					reader,
					names,
					a,
					(PdfDictionary)PdfReader.GetPdfObjectRelease(outlines.Get(PdfName.FIRST)),
					pages,
					w);
			}
			return doc.DocumentElement;
		}

		static void BookmarkDepth(PdfReader reader, Dictionary<string, PdfObject> names, PdfActionExporter exporter, PdfDictionary outline, Dictionary<int, int> pageRefMap, XmlWriter target) {
			while (outline != null) {
				target.WriteStartElement(Constants.Bookmark);

				target.WriteAttributeString(Constants.BookmarkAttributes.Title,
					StringHelper.ReplaceControlAndBomCharacters(outline.GetAsString(PdfName.TITLE).Decode(AppContext.Encodings.BookmarkEncoding))
					);

				var color = outline.Locate<PdfArray>(PdfName.C);
				DocInfoExporter.ExportColor(color, target);

				var style = outline.Locate<PdfNumber>(PdfName.F);
				if (style != null) {
					int f = style.IntValue & 0x03;
					if (f > 0) {
						target.WriteAttributeString(Constants.BookmarkAttributes.Style, Constants.BookmarkAttributes.StyleType.Names[f]);
					}
				}

				if (outline.Get(PdfName.COUNT) is PdfNumber count) {
					target.WriteAttributeString(Constants.BookmarkAttributes.Open, count.IntValue < 0 ? Constants.Boolean.False : Constants.Boolean.True);
				}

				var dest = outline.Locate<PdfObject>(PdfName.DEST);
				if (dest != null) {
					exporter.ExportGotoAction(dest, names, target, pageRefMap);
				}
				else {
					exporter.ExportAction(outline.Locate<PdfDictionary>(PdfName.A), names, pageRefMap, target);
				}
				var first = outline.Locate<PdfDictionary>(PdfName.FIRST);
				if (first != null) {
					BookmarkDepth(reader, names, exporter, first, pageRefMap, target);
				}
				outline = outline.Locate<PdfDictionary>(PdfName.NEXT);
				target.WriteEndElement();
			}
		}

		private static Object[] CreateOutlines(PdfWriter writer, PdfIndirectReference parent, XmlElement kids, int maxPageNumber, bool namedAsNames) {
			var bookmarks = kids.SelectNodes(Constants.Bookmark);
			var refs = new PdfIndirectReference[bookmarks.Count];
			for (int k = 0; k < refs.Length; ++k) {
				refs[k] = writer.PdfIndirectReference;
			}

			int ptr = 0;
			int count = 0;
			foreach (XmlElement child in bookmarks) {
				object[] lower = null;
				if (child.SelectSingleNode(Constants.Bookmark) != null)
					lower = CreateOutlines(writer, refs[ptr], child, maxPageNumber, namedAsNames);
				var outline = new PdfDictionary();
				++count;
				if (lower != null) {
					outline.Put(PdfName.FIRST, (PdfIndirectReference)lower[0]);
					outline.Put(PdfName.LAST, (PdfIndirectReference)lower[1]);
					int n = (int)lower[2];
					// 默认关闭书签
					if (child.GetAttribute(Constants.BookmarkAttributes.Open) != Constants.Boolean.True) {
						outline.Put(PdfName.COUNT, -n);
					}
					else {
						outline.Put(PdfName.COUNT, n);
						count += n;
					}
				}
				outline.Put(PdfName.PARENT, parent);
				if (ptr > 0)
					outline.Put(PdfName.PREV, refs[ptr - 1]);
				if (ptr < refs.Length - 1)
					outline.Put(PdfName.NEXT, refs[ptr + 1]);
				outline.Put(PdfName.TITLE, child.GetAttribute(Constants.BookmarkAttributes.Title));
				DocInfoImporter.ImportColor(child, outline);
				var style = child.GetAttribute(Constants.BookmarkAttributes.Style);
				if (!String.IsNullOrEmpty(style)) {
					int bits = Array.IndexOf(Constants.BookmarkAttributes.StyleType.Names, style);
					if (bits == -1) {
						bits = 0;
					}
					if (bits != 0)
						outline.Put(PdfName.F, bits);
				}
				DocInfoImporter.ImportAction(writer, outline, child, maxPageNumber, namedAsNames);
				writer.AddToBody(outline, refs[ptr]);
				++ptr;
			}
			return [refs[0], refs[refs.Length - 1], count];
		}

		internal static PdfIndirectReference WriteOutline(PdfWriter writer, XmlElement bookmarks, int maxPageNumber) {
			if (bookmarks == null || bookmarks.SelectSingleNode(Constants.Bookmark) == null) {
				return null;
			}
			var top = new PdfDictionary();
			var topRef = writer.PdfIndirectReference;
			var kids = CreateOutlines(writer, topRef, bookmarks, maxPageNumber, false);
			top.Put(PdfName.TYPE, PdfName.OUTLINES);
			top.Put(PdfName.FIRST, (PdfIndirectReference)kids[0]);
			top.Put(PdfName.LAST, (PdfIndirectReference)kids[1]);
			top.Put(PdfName.COUNT, (int)kids[2]);
			writer.AddToBody(top, topRef);
			writer.ExtraCatalog.Put(PdfName.OUTLINES, topRef);
			return topRef;
		}

		internal static void KillOutline(PdfReader source) {
			var catalog = source.Catalog;
			var o = catalog.Get(PdfName.OUTLINES);
			if (o == null) {
				return;
			}
			if (o != null) {
				var outlines = o as PRIndirectReference;
				OutlineTravel(outlines);
				PdfReader.KillIndirect(outlines);
			}
			catalog.Remove(PdfName.OUTLINES);
			PdfReader.KillIndirect(catalog.Get(PdfName.OUTLINES));
			if (PdfName.USEOUTLINES.Equals(catalog.GetAsName(PdfName.PAGEMODE))) {
				catalog.Remove(PdfName.PAGEMODE);
			}
		}

		private static void OutlineTravel(PRIndirectReference outline) {
			while (outline != null) {
				var outlineR = (PdfDictionary)PdfReader.GetPdfObjectRelease(outline);
				PdfReader.KillIndirect(outline);
				if (outlineR != null) {
					var first = (PRIndirectReference)outlineR.Get(PdfName.FIRST);
					if (first != null) {
						OutlineTravel(first);
					}
					PdfReader.KillIndirect(outlineR.Get(PdfName.DEST));
					PdfReader.KillIndirect(outlineR.Get(PdfName.A));
					outline = (PRIndirectReference)outlineR.Get(PdfName.NEXT);
				}
				else {
					outline = null;
				}
			}
		}
	}
}