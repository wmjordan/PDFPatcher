using System;
using System.Xml;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	sealed class GotoDestinationProcessor : IInfoDocProcessor
	{
		public bool RemoveOrphanDestination { get; set; }
		public int[] PageRemapper { get; set; }
		public CoordinateTranslationSettings[] TransitionMapper { get; set; }

		#region IBookmarkProcessor 成员

		public bool Process(XmlElement item) {
			int page;

			if (item.ParentNode == null) {
				return false;
			}
			if (item.Name != Constants.Bookmark && item.Name != Constants.PageLinkAttributes.Link) {
				return false;
			}
			var a = item.GetAttribute(Constants.DestinationAttributes.Action);
			if (String.IsNullOrEmpty(a) == false && a != Constants.ActionType.Goto) {
				return false;
			}

			if (item.GetAttribute(Constants.DestinationAttributes.Page).TryParse(out page) == false) {
				RemoveGotoAction(item);
				return true;
			}
			System.Diagnostics.Debug.WriteLine(item.GetAttribute(Constants.BookmarkAttributes.Title));
			if (page > 0) {
				if (TransitionMapper != null) {
					if (PageRemapper != null && page >= PageRemapper.Length) {
						System.Diagnostics.Trace.WriteLine("跳转页码位置无效：" + page);
					}
					else if (TransitionMapper[page] != null) {
						var ct = TransitionMapper[page];
						TranslateDestinationCoordinates(item, ct);
					}
				}
				if (PageRemapper != null) {
					if (page < PageRemapper.Length && (page = PageRemapper[page]) > 0) {
						item.SetAttribute(Constants.DestinationAttributes.Page, page.ToText());
					}
					else if (RemoveOrphanDestination) {
						RemoveOrphan(item);
						return true;
					}
				}
			}
			else if (RemoveOrphanDestination) {
				RemoveOrphan(item);
			}
			else {
				RemoveGotoAction(item);
			}
			return true;
		}

		private static void RemoveGotoAction(XmlElement item) {
			if (item.Name == Constants.ActionType.Goto) {
				item.ParentNode.RemoveChild(item);
			}
			else {
				item.RemoveAttribute(Constants.DestinationAttributes.Action);
				item.RemoveAttribute(Constants.DestinationAttributes.Page);
			}
		}

		private void RemoveOrphan(XmlElement item) {
			if (item.HasChildNodes && item.LocalName == Constants.Bookmark) {
				while (item.HasChildNodes) {
					var c = item.LastChild as XmlElement;
					if (c == null ||
						(c.HasAttribute(Constants.DestinationAttributes.Action) == false
							&& c.HasChildNodes == false)) {
						item.RemoveChild(item.LastChild);
						continue;
					}
					item.ParentNode.InsertAfter(item.LastChild, item);
				}
			}
			item.ParentNode.RemoveChild(item);
		}

		#endregion

		private static void TranslateDestinationCoordinates(XmlElement item, CoordinateTranslationSettings ct) {
			float p;
			if (item.GetAttribute(Constants.Coordinates.Top).TryParse(out p) && p != 0) {
				item.SetAttribute(Constants.Coordinates.Top, (p * ct.YScale + ct.YTranslation).ToText());
			}
			if (item.GetAttribute(Constants.Coordinates.Bottom).TryParse(out p) && p != 0) {
				item.SetAttribute(Constants.Coordinates.Bottom, (p * ct.YScale + ct.YTranslation).ToText());
			}
			if (item.GetAttribute(Constants.Coordinates.Left).TryParse(out p) && p != 0) {
				item.SetAttribute(Constants.Coordinates.Left, (p * ct.XScale + ct.XTranslation).ToText());
			}
			if (item.GetAttribute(Constants.Coordinates.Right).TryParse(out p) && p != 0) {
				item.SetAttribute(Constants.Coordinates.Right, (p * ct.XScale + ct.XTranslation).ToText());
			}
		}

	}
}
