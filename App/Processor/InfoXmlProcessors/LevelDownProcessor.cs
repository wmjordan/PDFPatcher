using System.Xml;

namespace PDFPatcher.Processor;

internal sealed class LevelDownProcessor : IPdfInfoXmlProcessor
{
	#region IInfoDocProcessor 成员

	public string Name => "设置书签为子书签";

	public IUndoAction Process(XmlElement item) {
		if (item == item.ParentNode.FirstChild) {
			return null;
		}

		UndoActionGroup undo = new();
		XmlNode n = item.SelectSingleNode("preceding-sibling::" + Constants.Bookmark + "[1]");
		if (n != null) {
			undo.Add(new AddElementAction(item));
			n.AppendChild(item);
			undo.Add(new RemoveElementAction(item));
			return undo;
		}

		return null;
	}

	#endregion
}