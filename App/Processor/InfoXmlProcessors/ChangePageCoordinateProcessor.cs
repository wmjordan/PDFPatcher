using PDFPatcher.Common;

namespace PDFPatcher.Processor
{
	sealed class ChangePageCoordinateProcessor : IPdfInfoXmlProcessor
	{
		public string CoordinateName { get; }
		public int PageNumber { get; }
		public float X { get; }
		public float Y { get; }

		public ChangePageCoordinateProcessor(string coordinateName, int pageNumber, float x, float y) {
			CoordinateName = coordinateName;
			PageNumber = pageNumber;
			X = x;
			Y = y;
		}

		#region IInfoDocProcessor 成员

		public string Name => "调整页码坐标定位";

		public IUndoAction Process(System.Xml.XmlElement item) {
			var undo = new UndoActionGroup();
			undo.Add(new ChangePageNumberProcessor(PageNumber, true, true).Process(item));
			undo.Add(new ChangeCoordinateProcessor(CoordinateName, Y, true, false).Process(item));
			return undo;
		}

		#endregion
	}
}
