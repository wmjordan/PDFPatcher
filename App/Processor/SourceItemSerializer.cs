using System;
using System.Collections.Generic;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	internal static class SourceItemSerializer
	{
		static readonly BookmarkSettings __EmptyBookmark = new BookmarkSettings();

		/// <summary>
		/// 保存合并文件功能的列表。
		/// </summary>
		/// <param name="list">文件列表项目。</param>
		/// <param name="path">文件列表的保存路径。</param>
		internal static void Serialize(IList<SourceItem> list, FilePath path) {
			var d = new PdfInfoXmlDocument();
			var b = d.BookmarkRoot;
			foreach (var item in list) {
				SerializeSourceItem(d, b, item, path);
			}
			try {
				d.Save(path);
			}
			catch (Exception ex) {
				AppContext.MainForm.ErrorBox("保存文件列表时遇到错误", ex);
			}
		}

		static void SerializeSourceItem(PdfInfoXmlDocument doc, BookmarkContainer container, SourceItem item, FilePath basePath) {
			var e = doc.CreateBookmark(item.Bookmark ?? __EmptyBookmark);
			switch (item.Type) {
				case SourceItem.ItemType.Empty:
					break;
				case SourceItem.ItemType.Pdf:
					e.SetValue(Constants.PageRange, ((SourceItem.Pdf)item).PageRanges);
					goto default;
				default:
					e.SetValue(Constants.DestinationAttributes.Path, basePath.GetRelativePath(item.FilePath));
					break;
			}
			container.AppendChild(e);
			if (item.HasSubItems) {
				foreach (var sub in item.Items) {
					SerializeSourceItem(doc, e, sub, basePath);
				}
			}
		}

		internal static List<SourceItem> Deserialize(FilePath path) {
			var d = new PdfInfoXmlDocument();
			try {
				d.Load(path);
			}
			catch (Exception ex) {
				AppContext.MainForm.ErrorBox("在加载文件列表时遇到错误", ex);
			}
			var bl = d.Bookmarks;
			var l = new List<SourceItem>(bl.Count);
			path = path.Directory;
			foreach (BookmarkElement item in bl) {
				DeserializeSourceItem(l, item, path);
			}
			return l;
		}

		static void DeserializeSourceItem(List<SourceItem> list, BookmarkElement bookmark, FilePath basePath) {
			var b = new BookmarkSettings(bookmark);
			var p = bookmark.GetValue(Constants.DestinationAttributes.Path);
			var s = String.IsNullOrEmpty(p) ? SourceItem.Create() : SourceItem.Create(basePath.Combine(p), false);
			if (b.Title.IsNullOrWhiteSpace() == false || b.IsOpened || b.IsBold || b.IsItalic || b.ForeColor.IsEmptyOrTransparent() == false) {
				s.Bookmark = b;
			}
			if (s.Type == SourceItem.ItemType.Pdf) {
				((SourceItem.Pdf)s).PageRanges = bookmark.GetValue(Constants.PageRange);
			}
			list.Add(s);
			if (bookmark.HasSubBookmarks) {
				foreach (BookmarkElement sub in bookmark.SubBookmarks) {
					DeserializeSourceItem(s.Items, sub, basePath);
				}
			}
		}
	}
}
