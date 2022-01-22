using System.Xml;
using PDFPatcher.Common;

namespace PDFPatcher.Processor;

internal sealed class ChangePageNumberProcessor : IPdfInfoXmlProcessor
{
	public ChangePageNumberProcessor(int amount) : this(amount, false, false) { }

	public ChangePageNumberProcessor(int amount, bool isAbsolute, bool skipZero) {
		IsAbsolute = isAbsolute;
		Amount = amount;
		SkipZero = skipZero;
	}

	public bool IsAbsolute { get; }
	public int Amount { get; }
	public bool SkipZero { get; }

	#region IInfoDocProcessor 成员

	public string Name => "更改目标页码";

	public IUndoAction Process(XmlElement item) {
		int p;
		string a = item.GetAttribute(Constants.DestinationAttributes.Action);
		if (((string.IsNullOrEmpty(a) && SkipZero == false) || a is Constants.ActionType.Goto or Constants.ActionType.GotoR) == false &&
			item.HasAttribute(Constants.DestinationAttributes.Page) == false) {
			return null;
		}

		if (item.GetAttribute(Constants.DestinationAttributes.Page).TryParse(out p)) {
			if (IsAbsolute) {
				if (p == Amount) {
					return null;
				}

				p = Amount;
			}
			else {
				p += Amount;
			}

			return p < 1 ? null : UndoAttributeAction.GetUndoAction(item, Constants.DestinationAttributes.Page, p.ToText());
		}

		UndoActionGroup undo = new();
		undo.SetAttribute(item, Constants.DestinationAttributes.Page, Amount.ToText());
		if (string.IsNullOrEmpty(a)) {
			undo.SetAttribute(item, Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
		}

		return undo;
	}

	#endregion
}