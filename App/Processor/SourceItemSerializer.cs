using System;
using System.Collections.Generic;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	internal static class SourceItemSerializer
	{
		static readonly BookmarkSettings __EmptyBookmark = new BookmarkSettings ();

		/// <summary>
		/// 保存合并文件功能的列表。
		/// </summary>
		/// <param name="list">文件列表项目。</param>
		/// <param name="path">文件列表的保存路径。</param>
		internal static void Serialize (IList<SourceItem> list, string path) {
			var d = new PdfInfoXmlDocument ();
			var b = d.BookmarkRoot;
			foreach (var item in list) {
				SerializeSourceItem (d, b, item);
			}
			try {
				d.Save (path);
			}
			catch (Exception ex) {
				FormHelper.ErrorBox ("在保存文件列表时遇到错误：" + ex.Message);
			}
		}

		private static void SerializeSourceItem (PdfInfoXmlDocument doc, BookmarkContainer container, SourceItem item) {
			var e = doc.CreateBookmark (item.Bookmark ?? __EmptyBookmark);
			e.SetValue (Constants.DestinationAttributes.Path, item.FilePath.ToString());
			if (item.Type == SourceItem.ItemType.Pdf) {
				e.SetValue (Constants.PageRange, ((SourceItem.Pdf)item).PageRanges);
			}
			container.AppendChild (e);
			if (item.HasSubItems) {
				foreach (var sub in item.Items) {
					SerializeSourceItem (doc, e, sub);
				}
			}
		}

		internal static List<SourceItem> Deserialize (string path) {
			var d = new PdfInfoXmlDocument ();
			try {
				d.Load (path);
			}
			catch (Exception ex) {
				FormHelper.ErrorBox ("在加载文件列表时遇到错误：" + ex.Message);
			}
			var bl = d.Bookmarks;
			var l = new List<SourceItem> (bl.Count);
			foreach (BookmarkElement item in bl) {
				DeserializeSourceItem (l, item);
			}
			return l;
		}

		private static void DeserializeSourceItem (List<SourceItem> list, BookmarkElement bookmark) {
			var b = new BookmarkSettings (bookmark);
			var p = bookmark.GetValue (Constants.DestinationAttributes.Path);
			var s = SourceItem.Create (p, false);
			if (b.Title.IsNullOrWhiteSpace () == false || b.IsOpened || b.IsBold || b.IsItalic || b.ForeColor.IsEmptyOrTransparent () == false) {
				s.Bookmark = b;
			}
			if (s.Type == SourceItem.ItemType.Pdf) {
				((SourceItem.Pdf)s).PageRanges = bookmark.GetValue (Constants.PageRange);
			}
			list.Add (s);
			if (bookmark.HasSubBookmarks) {
				foreach (BookmarkElement sub in bookmark.SubBookmarks) {
					DeserializeSourceItem (s.Items, sub);
				}
			}
		}
	}
}
