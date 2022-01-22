using System.Xml;

namespace PDFPatcher.Processor;

internal sealed class RemoveZoomRateProcessor : IInfoDocProcessor
{
	#region IInfoDocProcessor 成员

	public bool Process(XmlElement item) {
		XmlAttribute d = item.GetAttributeNode(Constants.DestinationAttributes.View);
		if (d != null && d.Value != Constants.DestinationAttributes.ViewType.XYZ) {
			d.Value = Constants.DestinationAttributes.ViewType.XYZ;
		}

		item.RemoveAttribute(Constants.Coordinates.ScaleFactor);
		return true;
	}

	#endregion
}