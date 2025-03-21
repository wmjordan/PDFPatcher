using System;
using System.Xml;
using PDFPatcher.Common;

namespace PDFPatcher.Processor
{
	sealed class ChangePageNumberProcessor(int amount, bool isAbsolute, bool skipZero, bool takeFollowing = false) : IPdfInfoXmlProcessor
	{
		public bool IsAbsolute { get; } = isAbsolute;
		public int Amount { get; } = amount;
		public bool SkipZero { get; } = skipZero;
		public bool TakeFollowing { get; } = takeFollowing;

		public ChangePageNumberProcessor(int amount) : this(amount, false, false, false) { }

		#region IInfoDocProcessor 成员

		public string Name => "更改目标页码";

		public IUndoAction Process(XmlElement item) {
			int p = item.GetValue(Constants.DestinationAttributes.Page, 0);
			var a = item.GetValue(Constants.DestinationAttributes.Action);
			if (a != Constants.ActionType.Goto && a != Constants.ActionType.GotoR
				&& (a is null && TakeFollowing) == false && Amount == 0
				|| String.IsNullOrEmpty(a) == false && p < 0) {
				return null;
			}
			if (p == 0 && TakeFollowing) {
				return TakeFollowingBookmarkDestination(item, Amount);
			}
			if (SkipZero
				&& item.HasAttribute(Constants.DestinationAttributes.Page) == false) {
				return null;
			}
			if (item.GetValue(Constants.DestinationAttributes.Page).TryParse(out p)) {
				if (IsAbsolute) {
					if (p == Amount) {
						return null;
					}
					p = Amount;
				}
				else {
					p += Amount;
				}
				return p < 1
					? null
					: UndoAttributeAction.GetUndoAction(item, Constants.DestinationAttributes.Page, p.ToText());
			}
			else {
				var undo = new UndoActionGroup();
				undo.SetAttribute(item, Constants.DestinationAttributes.Page, Amount.ToText());
				if (String.IsNullOrEmpty(a)) {
					undo.SetAttribute(item, Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
				}
				return undo;
			}
		}

		static UndoActionGroup TakeFollowingBookmarkDestination(XmlElement bookmark, int amount) {
			foreach (var item in bookmark.ChildrenOrFollowingSiblings()) {
				var p = item.GetValue(Constants.DestinationAttributes.Page);
				int n;
				if (String.IsNullOrEmpty(p) || (n = p.ToInt32()) <= 0) {
					continue;
				}
				var a = item.GetValue(Constants.DestinationAttributes.Action);
				if (a != Constants.ActionType.Goto && a != Constants.ActionType.GotoR) {
					continue;
				}
				n += amount;
				if (n <= 0) {
					break;
				}
				var undo = new UndoActionGroup();
				undo.SetAttribute(bookmark, Constants.DestinationAttributes.Page, (n + amount).ToText());
				undo.SetAttribute(bookmark, Constants.DestinationAttributes.Action, a);
				CopyAttributes(bookmark, item, undo, Constants.Coordinates.Top, Constants.Coordinates.Left, Constants.Coordinates.Bottom, Constants.Coordinates.Right, Constants.Coordinates.ScaleFactor);
				return undo;
			}
			return null;
		}

		static void CopyAttributes(XmlElement bookmark, XmlElement item, UndoActionGroup undo, params string[] names) {
			foreach (var name in names) {
				var value = item.GetValue(name);
				if (String.IsNullOrEmpty(value) == false) {
					undo.SetAttribute(bookmark, name, value);
				}
			}
		}

		#endregion
	}
}
