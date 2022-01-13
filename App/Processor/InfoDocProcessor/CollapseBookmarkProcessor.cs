using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

internal sealed class CollapseBookmarkProcessor : IInfoDocProcessor
{
	public BookmarkStatus BookmarkStatus { get; set; }

	public CollapseBookmarkProcessor() {
	}

	#region IBookmarkProcessor 成员

	public bool Process(XmlElement bookmark) {
		switch (BookmarkStatus) {
			case BookmarkStatus.AsIs:
				return false;
			case BookmarkStatus.CollapseAll:
				bookmark.SetAttribute(Constants.BookmarkAttributes.Open, Constants.Boolean.False);
				return true;
			case BookmarkStatus.ExpandAll:
				bookmark.SetAttribute(Constants.BookmarkAttributes.Open, Constants.Boolean.True);
				return true;
			case BookmarkStatus.ExpandTop:
				XmlNode p = bookmark.ParentNode;
				if (p != null && p.Name == Constants.DocumentBookmark) {
					bookmark.SetAttribute(Constants.BookmarkAttributes.Open, Constants.Boolean.True);
				}
				else {
					bookmark.SetAttribute(Constants.BookmarkAttributes.Open, Constants.Boolean.False);
				}

				return true;
			default:
				return false;
		}
	}

	#endregion
}