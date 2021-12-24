using System;
using PDFPatcher.Common;

namespace PDFPatcher.Processor
{
	sealed class ChangePageNumberProcessor : IPdfInfoXmlProcessor
	{
		public bool IsAbsolute { get; private set; }
		public int Amount { get; private set; }
		public bool SkipZero { get; private set; }

		public ChangePageNumberProcessor (int amount) : this (amount, false, false) { }
		public ChangePageNumberProcessor (int amount, bool isAbsolute, bool skipZero) {
			this.IsAbsolute = isAbsolute;
			this.Amount = amount;
			this.SkipZero = skipZero;
		}

		#region IInfoDocProcessor 成员

		public string Name {
			get { return "更改目标页码"; }
		}

		public IUndoAction Process (System.Xml.XmlElement item) {
			int p;
			var a = item.GetAttribute (Constants.DestinationAttributes.Action);
			if ((String.IsNullOrEmpty (a) && SkipZero == false || a == Constants.ActionType.Goto || a == Constants.ActionType.GotoR) == false && item.HasAttribute (Constants.DestinationAttributes.Page) == false) {
				return null;
			}
			if (item.GetAttribute (Constants.DestinationAttributes.Page).TryParse (out p)) {
				if (this.IsAbsolute) {
					if (p == this.Amount) {
						return null;
					}
					p = this.Amount;
				}
				else {
					p += this.Amount;
				}
				if (p < 1) {
					return null;
				}
				return UndoAttributeAction.GetUndoAction (item, Constants.DestinationAttributes.Page, p.ToText ());
			}
			else {
				var undo = new UndoActionGroup ();
				undo.SetAttribute (item, Constants.DestinationAttributes.Page, this.Amount.ToText ());
				if (String.IsNullOrEmpty (a)) {
					undo.SetAttribute (item, Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
				}
				return undo;
			}
		}

		#endregion
	}
}
