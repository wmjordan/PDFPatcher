using PDFPatcher.Common;

namespace PDFPatcher.Processor
{
	sealed class ChangeCoordinateProcessor(string coordinateName, float value, bool absolute, bool proportional) : IPdfInfoXmlProcessor
	{
		public string CoordinateName { get; } = coordinateName;
		public float Value { get; } = value;
		public bool IsAbsolute { get; } = absolute;
		public bool IsProportional { get; } = proportional;

		#region IInfoDocProcessor 成员

		public string Name => $"{(IsAbsolute ? "更改" : IsProportional ? "缩放" : "调整")}{CoordinateName}坐标定位";

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
}
