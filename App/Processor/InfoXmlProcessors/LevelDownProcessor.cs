namespace PDFPatcher.Processor
{
	sealed class LevelDownProcessor : IPdfInfoXmlProcessor
	{
		#region IInfoDocProcessor 成员

		public string Name => "设置书签为子书签";

		public IUndoAction Process(System.Xml.XmlElement item) {
			if (item == item.ParentNode.FirstChild) {
				return null;
			}
			var undo = new UndoActionGroup();
			var n = item.SelectSingleNode($"preceding-sibling::{Constants.Bookmark}[1]");
			if (n == null) {
				return null;
			}

			undo.Add(new AddElementAction(item));
			n.AppendChild(item);
			undo.Add(new RemoveElementAction(item));
			return undo;
		}

		#endregion
	}
}
