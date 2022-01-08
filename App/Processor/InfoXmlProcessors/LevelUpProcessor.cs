﻿namespace PDFPatcher.Processor
{
	sealed class LevelUpProcessor : IPdfInfoXmlProcessor
	{
		#region IInfoDocProcessor 成员

		public string Name => "设置书签为父书签";

		public IUndoAction Process(System.Xml.XmlElement item) {
			if (item.ParentNode.Name != Constants.Bookmark) {
				return null;
			}
			var undo = new UndoActionGroup();
			var fs = item.SelectNodes("following-sibling::" + Constants.Bookmark);
			foreach (System.Xml.XmlElement f in fs) {
				undo.Add(new AddElementAction(f));
				item.AppendChild(f);
				undo.Add(new RemoveElementAction(f));
			}
			undo.Add(new AddElementAction(item));
			item.ParentNode.ParentNode.InsertAfter(item, item.ParentNode);
			undo.Add(new RemoveElementAction(item));
			return undo;
		}

		#endregion
	}
}
