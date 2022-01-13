using PDFPatcher.Common;

namespace PDFPatcher.Processor;

internal sealed class ChangeCoordinateProcessor : IPdfInfoXmlProcessor
{
	public string CoordinateName { get; private set; }
	public float Value { get; private set; }
	public bool IsAbsolute { get; private set; }
	public bool IsProportional { get; private set; }

	public ChangeCoordinateProcessor(string coordinateName, float value, bool absolute, bool proportional) {
		CoordinateName = coordinateName;
		Value = value;
		IsAbsolute = absolute;
		IsProportional = proportional;
	}

	#region IInfoDocProcessor 成员

	public string Name => string.Concat(IsAbsolute ? "更改" : IsProportional ? "缩放" : "调整", CoordinateName, "坐标定位");

	public IUndoAction Process(System.Xml.XmlElement item) {
		float c;
		string v;
		item.GetAttribute(CoordinateName).TryParse(out c);
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