using System.Xml;
using PDFPatcher.Common;

namespace PDFPatcher.Processor;

internal sealed class ChangeCoordinateProcessor : IPdfInfoXmlProcessor
{
	public ChangeCoordinateProcessor(string coordinateName, float value, bool absolute, bool proportional) {
		CoordinateName = coordinateName;
		Value = value;
		IsAbsolute = absolute;
		IsProportional = proportional;
	}

	public string CoordinateName { get; }
	public float Value { get; }
	public bool IsAbsolute { get; }
	public bool IsProportional { get; }

	#region IInfoDocProcessor 成员

	public string Name => string.Concat(IsAbsolute ? "更改" : IsProportional ? "缩放" : "调整", CoordinateName, "坐标定位");

	public IUndoAction Process(XmlElement item) {
		string v;
		item.GetAttribute(CoordinateName).TryParse(out float c);
		if (IsAbsolute) {
			if (c != Value) {
				v = Value.ToText();
			}
			else {
				return null;
			}
		}
		else if (Value != 0) {
			v = (IsProportional ? Value * c : Value + c).ToText();
		}
		else {
			return null;
		}

		return UndoAttributeAction.GetUndoAction(item, CoordinateName, v);
	}

	#endregion
}