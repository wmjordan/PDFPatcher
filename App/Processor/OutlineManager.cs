using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	/// <summary>A class which manages outlines (bookmarks) of PDF documents.</summary>
	static partial class OutlineManager
	{
		static readonly char[] __fullWidthNumbers = "０１２３４５６７８９".ToCharArray();
		static readonly char[] __halfWidthNumbers = "0123456789".ToCharArray();
		static readonly char[] __cmdIdentifiers = ['=', '﹦', '＝', ':', '：'];
		static readonly char[] __pageLabelSeparators = [';', '；', ',', '，', ' '];

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
							Tracker.TraceMessage($"缩进标记改为“{indentString}”");
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
							if (!l[0].TryParse(out pn) || pn < 1) {
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
							if (!String.IsNullOrEmpty(prefix)) {
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
				if (!m.Success) {
					continue;
				}
				title = m.Groups[1].Value;
				pnText = m.Groups[2].Value;
				if (pnText.Length == 0) {
					pageNum = 0;
				}
				else {
					if (pnText.IndexOfAny(__fullWidthNumbers) != -1) {
						digits = Array.ConvertAll(m.Groups[2].Value.ToCharArray(), d => ValueHelper.MapValue(d, __fullWidthNumbers, __halfWidthNumbers, d));
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
						throw new FormatException($"在简易书签第 {lineNum} 行的缩进格式不正确。\n\n说明：下级书签最多只能比上级书签多一个缩进标记。");
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
				if (!isOpen) {
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
				if (s.HasPrefix(VersionString) || s.HasPrefix(VersionString2)) {
					return item;
				}
			}
			return Encoding.Default;
		}

	}
}
