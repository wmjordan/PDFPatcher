using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	/// <summary>A class which manages outlines (bookmarks) of PDF documents.</summary>
	static class OutlineManager
	{
		// modifed: added split array for action parameters
		static readonly char[] __ActionSplitters = new char[] { ' ', '\t', '\r', '\n' };
		static readonly char[] __fullWidthNums = "０１２３４５６７８９".ToCharArray();
		static readonly char[] __halfWidthNums = "0123456789".ToCharArray();
		static readonly char[] __cmdIdentifiers = new char[] { '=', '﹦', '＝', ':', '：' };
		static readonly char[] __pageLabelSeparators = new char[] { ';', '；', ',', '，', ' ' };

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

				var count = outline.Get(PdfName.COUNT) as PdfNumber;
				if (count != null) {
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

		private static Object[] CreateOutlines(PdfWriter writer, PdfIndirectReference parent, XmlElement kids, int maxPageNumber, bool namedAsNames) {
			var bookmarks = kids.SelectNodes(Constants.Bookmark);
			var refs = new PdfIndirectReference[bookmarks.Count];
			for (int k = 0; k < refs.Length; ++k)
				refs[k] = writer.PdfIndirectReference;
			int ptr = 0;
			int count = 0;
			foreach (XmlElement child in bookmarks) {
				Object[] lower = null;
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
				if (String.IsNullOrEmpty(style) == false) {
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
			return new Object[] { refs[0], refs[refs.Length - 1], count };
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

		public static string EscapeBinaryString(String s) {
			var buf = StringBuilderCache.Acquire();
			var cc = s.ToCharArray();
			int len = cc.Length;
			for (int k = 0; k < len; ++k) {
				char c = cc[k];
				if (c < ' ') {
					buf.Append('\\');
					int v = c;
					var octal = "";
					do {
						int x = v % 8;
						octal = x.ToText() + octal;
						v /= 8;
					} while (v > 0);
					buf.Append(octal.PadLeft(3, '0'));
				}
				else if (c == '\\') {
					buf.Append("\\\\");
				}
				else {
					buf.Append(c);
				}
			}
			return StringBuilderCache.GetStringAndRelease(buf);
		}

		public static string UnescapeBinaryString(String s) {
			var buf = StringBuilderCache.Acquire();
			var cc = s.ToCharArray();
			int len = cc.Length;
			for (int k = 0; k < len; ++k) {
				char c = cc[k];
				if (c != '\\') {
					buf.Append(c);
					continue;
				}
				if (++k >= len) {
					buf.Append('\\');
					break;
				}
				c = cc[k];
				if (c < '0' || c > '7') {
					buf.Append(c);
					continue;
				}
				int n = c - '0';
				++k;
				for (int j = 0; j < 2 && k < len; ++j) {
					c = cc[k];
					if (c < '0' || c > '7') {
						break;
					}
					++k;
					n = n * 8 + c - '0';
				}
				--k;
				buf.Append((char)n);
			}
			return StringBuilderCache.GetStringAndRelease(buf);
		}

		internal static void ImportSimpleBookmarks(TextReader source, PdfInfoXmlDocument target) {
			string cmd, cmdData, s, title, indentString = "\t", pnText;
			bool isOpen = false; // 书签是否默认打开
			int pageOffset = 0, pageNum;
			int currentIndent = -1, indent, p;
			int lineNum = 0;
			char[] digits;
			var pattern = new Regex(@"(.+?)[\s\.…　\-_]*(-?[0-9０１２３４５６７８９]+)?\s*$", RegexOptions.Compiled);
			var docInfo = target.InfoNode;
			var root = target.BookmarkRoot;
			var pageLabels = target.PageLabelRoot;
			BookmarkContainer currentBookmark = root;
			BookmarkElement bookmark;
			while (source.Peek() != -1) {
				s = source.ReadLine();
				lineNum++;
				if (s.Trim().Length == 0) {
					continue;
				}

				if ((s[0] == '#' || s[0] == '＃') && (p = s.IndexOfAny(__cmdIdentifiers)) != -1) {
					cmd = s.Substring(1, p - 1);
					cmdData = s.Substring(p + 1);
					switch (cmd) {
						case "首页页码":
							if (cmdData.TryParse(out pageOffset)) {
								Tracker.TraceMessage("首页页码改为 " + pageOffset);
								pageOffset--;
							}
							break;
						case "缩进标记":
							indentString = cmdData;
							Tracker.TraceMessage(String.Concat("缩进标记改为“", indentString, "”"));
							break;
						case "版本":
							if (lineNum == 1) {
								var v = cmdData.Trim();
								target.DocumentElement.SetAttribute(Constants.Info.ProductVersion, v);
								Tracker.TraceMessage("导入简易书签文件，版本为：" + v);
							}
							break;
						case "打开书签":
							cmdData = cmdData.ToLowerInvariant();
							isOpen = (cmdData == "是" || cmdData == "true" || cmdData == "y" || cmdData == "yes" || cmdData == "1");
							break;
						case Constants.Info.DocumentPath:
							target.PdfDocumentPath = cmdData.Trim();
							break;
						case Constants.PageLabels:
							var l = cmdData.Split(__pageLabelSeparators, 3);
							if (l.Length < 1) {
								Tracker.TraceMessage(Constants.PageLabels + "格式不正确，至少应指定起始页码。");
								continue;
							}
							int pn;
							if (l[0].TryParse(out pn) == false || pn < 1) {
								Tracker.TraceMessage(Constants.PageLabels + "格式不正确：起始页码应为正整数。");
								continue;
							}
							var style = l[1].Length > 0
								? ValueHelper.MapValue(l[1][0],
									Constants.PageLabelStyles.SimpleInfoIdentifiers,
									Constants.PageLabelStyles.Names,
									Constants.PageLabelStyles.Names[1])
								: Constants.PageLabelStyles.Names[1];
							var prefix = l.Length > 2 ? l[2] : null;
							var pl = target.CreateElement(Constants.PageLabelsAttributes.Style) as XmlElement;
							pl.SetAttribute(Constants.PageLabelsAttributes.PageNumber, pn.ToText());
							pl.SetAttribute(Constants.PageLabelsAttributes.Style, style);
							if (String.IsNullOrEmpty(prefix) == false) {
								pl.SetAttribute(Constants.PageLabelsAttributes.Prefix, prefix);
							}
							pageLabels.AppendChild(pl);
							continue;
						case Constants.Info.Title:
						case Constants.Info.Subject:
						case Constants.Info.Keywords:
						case Constants.Info.Author:
							docInfo.SetAttribute(cmd, cmdData);
							break;
					}
					continue;
				}
				indent = p = 0;
				while (s.IndexOf(indentString, p) == p) {
					p += indentString.Length;
					indent++;
				}
				var m = pattern.Match(s, p);
				if (m.Success == false) {
					continue;
				}
				title = m.Groups[1].Value;
				pnText = m.Groups[2].Value;
				if (pnText.Length == 0) {
					pageNum = 0;
				}
				else {
					if (pnText.IndexOfAny(__fullWidthNums) != -1) {
						digits = Array.ConvertAll(m.Groups[2].Value.ToCharArray(), d => ValueHelper.MapValue(d, __fullWidthNums, __halfWidthNums, d));
						pnText = new string(digits, 0, digits.Length);
					}
					if (pnText.TryParse(out pageNum)) {
						pageNum += pageOffset;
					}
				}
				bookmark = target.CreateBookmark();
				if (indent == currentIndent) {
					currentBookmark.ParentNode.AppendChild(bookmark);
				}
				else if (indent > currentIndent) {
					currentBookmark.AppendChild(bookmark);
					if (indent - currentIndent > 1) {
						throw new FormatException(String.Concat("在简易书签第 ", lineNum, " 行的缩进格式不正确。\n\n说明：下级书签最多只能比上级书签多一个缩进标记。"));
					}
					currentIndent++;
				}
				else /* indent < currentIndent */ {
					while (currentIndent > indent && currentBookmark.ParentNode != root) {
						currentBookmark = currentBookmark.ParentNode as BookmarkContainer;
						currentIndent--;
					}
					currentBookmark.ParentNode.AppendChild(bookmark);
				}
				bookmark.Title = title;
				if (isOpen == false) {
					bookmark.IsOpen = false;
				}
				if (pageNum > 0) {
					bookmark.Page = pageNum;
				}
				currentBookmark = bookmark;
			}
		}

		internal static void ImportSimpleBookmarks(string path, PdfInfoXmlDocument target) {
			using (TextReader r = new StreamReader(path, DetectEncoding(path))) {
				ImportSimpleBookmarks(r, target);
			}
		}

		public static void WriteSimpleBookmarkInstruction(TextWriter writer, string item, string value) {
			if (String.IsNullOrEmpty(value)) {
				return;
			}
			writer.Write("#");
			writer.Write(item);
			writer.Write("=");
			writer.WriteLine(value);
		}

		/// <summary>
		/// 将 XML 书签输出为简易书签。
		/// </summary>
		/// <param name="writer">输出目标。</param>
		/// <param name="container">书签节点。</param>
		/// <param name="indent">缩进量。</param>
		/// <param name="indentChar">缩进字符串。</param>
		public static void WriteSimpleBookmark(TextWriter writer, BookmarkContainer container, int indent, string indentChar) {
			foreach (BookmarkElement item in container.SubBookmarks) {
				for (int i = 0; i < indent; i++) {
					writer.Write(indentChar);
				}
				writer.Write(item.Title);
				writer.Write("\t\t");
				writer.Write(item.Page.ToText());
				writer.WriteLine();
				WriteSimpleBookmark(writer, item, indent + 1, indentChar);
			}
		}

		private static Encoding DetectEncoding(string path) {
			const string VersionString = "#版本";
			const string VersionString2 = "＃版本";

			var b = new byte[20];
			using (var r = new FileStream(path, FileMode.Open)) {
				if (r.Length < b.Length) {
					throw new FormatException("简易书签文件内容不足。");
				}
				r.Read(b, 0, b.Length);
			}
			foreach (var item in Constants.Encoding.Encodings) {
				if (item == null) {
					continue;
				}
				var s = item.GetString(b);
				if (s.StartsWith(VersionString, StringComparison.Ordinal) || s.StartsWith(VersionString2, StringComparison.Ordinal)) {
					return item;
				}
			}
			return Encoding.Default;
		}

	}
}
