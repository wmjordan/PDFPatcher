using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	sealed class ForceInternalDestinationProcessor : IInfoDocProcessor
	{
		#region IBookmarkProcessor 成员

		public bool Process(System.Xml.XmlElement item) {
			if (item.ParentNode == null) {
				return false;
			}

			if (item.Name != Constants.Bookmark && item.Name != Constants.PageLinkAttributes.Link) {
				return false;
			}

			var a = item.GetAttribute(Constants.DestinationAttributes.Action);
			switch (a) {
				case Constants.ActionType.GotoR:
				case Constants.ActionType.Launch:
				case Constants.ActionType.Uri:
					if (item.GetAttribute(Constants.DestinationAttributes.Path).EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase)) {
						item.RemoveAttribute(Constants.DestinationAttributes.Action);
						if (item.HasAttribute(Constants.DestinationAttributes.Page) == false
							&& item.HasAttribute(Constants.DestinationAttributes.Named) == false
							&& item.HasAttribute(Constants.DestinationAttributes.NamedN) == false
							) {
							item.SetAttribute(Constants.DestinationAttributes.Page, "1");
						}
					}
					return true;
				default:
					return false;
			}
		}

		#endregion

	}
}