using System;
using System.Collections.Generic;
using System.Xml;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

internal static class SourceItemSerializer
{
	private static readonly BookmarkSettings __EmptyBookmark = new();

	/// <summary>
	///     保存合并文件功能的列表。
	/// </summary>
	/// <param name="list">文件列表项目。</param>
	/// <param name="path">文件列表的保存路径。</param>
	internal static void Serialize(IList<SourceItem> list, string path) {
		PdfInfoXmlDocument d = new();
		BookmarkRootElement b = d.BookmarkRoot;
		foreach (SourceItem item in list) {
			SerializeSourceItem(d, b, item);
		}

		try {
			d.Save(path);
		}
		catch (Exception ex) {
			FormHelper.ErrorBox("在保存文件列表时遇到错误：" + ex.Message);
		}
	}

	private static void SerializeSourceItem(PdfInfoXmlDocument doc, BookmarkContainer container, SourceItem item) {
		BookmarkElement e = doc.CreateBookmark(item.Bookmark ?? __EmptyBookmark);
		e.SetValue(Constants.DestinationAttributes.Path, item.FilePath.ToString());
		if (item.Type == SourceItem.ItemType.Pdf) {
			e.SetValue(Constants.PageRange, ((SourceItem.Pdf)item).PageRanges);
		}

		container.AppendChild(e);
		if (item.HasSubItems) {
			foreach (SourceItem sub in item.Items) {
				SerializeSourceItem(doc, e, sub);
			}
		}
	}

	internal static List<SourceItem> Deserialize(string path) {
		PdfInfoXmlDocument d = new();
		try {
			d.Load(path);
		}
		catch (Exception ex) {
			FormHelper.ErrorBox("在加载文件列表时遇到错误：" + ex.Message);
		}

		XmlNodeList bl = d.Bookmarks;
		List<SourceItem> l = new(bl.Count);
		foreach (BookmarkElement item in bl) {
			DeserializeSourceItem(l, item);
		}

		return l;
	}

	private static void DeserializeSourceItem(List<SourceItem> list, BookmarkElement bookmark) {
		BookmarkSettings b = new(bookmark);
		string p = bookmark.GetValue(Constants.DestinationAttributes.Path);
		SourceItem s = SourceItem.Create(p, false);
		if (b.Title.IsNullOrWhiteSpace() == false || b.IsOpened || b.IsBold || b.IsItalic ||
		    b.ForeColor.IsEmptyOrTransparent() == false) {
			s.Bookmark = b;
		}

		if (s.Type == SourceItem.ItemType.Pdf) {
			((SourceItem.Pdf)s).PageRanges = bookmark.GetValue(Constants.PageRange);
		}

		list.Add(s);
		if (bookmark.HasSubBookmarks) {
			foreach (BookmarkElement sub in bookmark.SubBookmarks) {
				DeserializeSourceItem(s.Items, sub);
			}
		}
	}
}