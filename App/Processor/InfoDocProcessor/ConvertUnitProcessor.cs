using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PDFPatcher.Processor;

internal sealed class ConvertUnitProcessor : IInfoDocProcessor
{
	public float UnitFactor { get; set; }

	public ConvertUnitProcessor() {
		UnitFactor = 1;
	}

	#region IInfoDocProcessor 成员

	public bool Process(XmlElement item) {
		ConvertUnit(item, Constants.Coordinates.Bottom);
		ConvertUnit(item, Constants.Coordinates.Left);
		ConvertUnit(item, Constants.Coordinates.Right);
		ConvertUnit(item, Constants.Coordinates.Top);
		return true;
	}

	#endregion

	private bool ConvertUnit(XmlElement item, string attribute) {
		XmlAttribute a = item.GetAttributeNode(attribute);
		if (a != null) {
			a.Value = Common.UnitConverter.ToPoint(a.Value, UnitFactor);
			return true;
		}

		return false;
	}
}