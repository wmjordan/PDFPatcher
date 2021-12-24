using PDFPatcher.Common;

namespace PDFPatcher.Processor
{
	sealed class ChangeZoomRateProcessor : IPdfInfoXmlProcessor
	{
		public string ZoomMethod { get; private set; }
		public float ZoomRate { get; private set; }

		public ChangeZoomRateProcessor (object zoomRate) {
			if (zoomRate is string || zoomRate == null) {
				this.ZoomMethod = zoomRate as string;
				this.ZoomRate = -1;
			}
			else if (zoomRate is float) {
				this.ZoomRate = (float)zoomRate;
			}
		}

		#region IInfoDocProcessor 成员

		public string Name {
			get { return "更改缩放比例"; }
		}

		public IUndoAction Process (System.Xml.XmlElement item) {
			UndoActionGroup undo;
			if (this.ZoomRate >= 0) {
				undo = new UndoActionGroup ();
				undo.SetAttribute (item, Constants.DestinationAttributes.View, Constants.DestinationAttributes.ViewType.XYZ);
				undo.SetAttribute (item, Constants.Coordinates.ScaleFactor, this.ZoomRate.ToText ());
			}
			else if (string.IsNullOrEmpty (this.ZoomMethod)) {
				undo = new UndoActionGroup ();
				undo.SetAttribute (item, Constants.DestinationAttributes.View, Constants.DestinationAttributes.ViewType.XYZ);
				undo.RemoveAttribute (item, Constants.Coordinates.ScaleFactor);
			}
			else {
				return UndoAttributeAction.GetUndoAction (item, Constants.DestinationAttributes.View, this.ZoomMethod);
			}
			return undo;
		}

		#endregion
	}
}
