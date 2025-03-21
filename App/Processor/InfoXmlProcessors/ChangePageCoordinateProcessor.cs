namespace PDFPatcher.Processor
{
	sealed class ChangePageCoordinateProcessor(string coordinateName, int pageNumber, float x, float y) : IPdfInfoXmlProcessor
	{
		public string CoordinateName { get; } = coordinateName;
		public int PageNumber { get; } = pageNumber;
		public float X { get; } = x;
		public float Y { get; } = y;

		#region IInfoDocProcessor 成员

		public string Name => "调整页码坐标定位";

		public IUndoAction Process(System.Xml.XmlElement item) {
			var undo = new UndoActionGroup();
			undo.SetAttribute(item, Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
			undo.Add(new ChangePageNumberProcessor(PageNumber, true, false).Process(item));
			undo.Add(new ChangeCoordinateProcessor(CoordinateName, Y, true, false).Process(item));
			return undo;
		}

		#endregion
	}
}
