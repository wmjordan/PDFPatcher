namespace PDFPatcher.Processor
{
	sealed class DestinationGotoTopProcessor : IPdfInfoXmlProcessor
	{
		#region IInfoDocProcessor 成员

		public string Name => "设置点击目标到页首";

		public IUndoAction Process(System.Xml.XmlElement item) {
			if (item.HasAttribute(Constants.DestinationAttributes.Page)) {
				var undo = new UndoActionGroup();
				undo.SetAttribute(item, Constants.DestinationAttributes.View,
					Constants.DestinationAttributes.ViewType.XYZ);
				undo.SetAttribute(item, Constants.Coordinates.Top, "10000");
				undo.RemoveAttribute(item, Constants.Coordinates.ScaleFactor);
				return undo;
			}

			return null;
		}

		#endregion
	}
}