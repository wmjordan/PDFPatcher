using System.Xml;

namespace PDFPatcher.Processor;

internal sealed class ClearDestinationOffsetProcessor : IPdfInfoXmlProcessor<ClearDestinationOffsetProcessor.PositionType>
{
	public enum PositionType { X, Y, XY }

	private string _name;

	private PositionType _type;

	public PositionType Parameter {
		get => _type;
		set {
			_type = value;
			_name = _type switch {
				PositionType.X => "横",
				PositionType.Y => "纵",
				_ => string.Empty
			};
		}
	}

	#region IInfoDocProcessor 成员

	public string Name => "清除" + _name + "坐标定位偏移值";

	public IUndoAction Process(XmlElement item) {
		if (item.GetAttribute(Constants.DestinationAttributes.View) == Constants.DestinationAttributes.ViewType.FitR) {
			return null;
		}

		switch (_type) {
			case PositionType.X:
				return ClearPositionOffset(item, Constants.Coordinates.Left);
			case PositionType.Y:
				return ClearPositionOffset(item, Constants.Coordinates.Top);
			case PositionType.XY:
				IUndoAction x = ClearPositionOffset(item, Constants.Coordinates.Left);
				IUndoAction y = ClearPositionOffset(item, Constants.Coordinates.Top);
				if (x != null && y != null) {
					UndoActionGroup g = new();
					g.Add(x);
					g.Add(y);
					return g;
				}
				else if (x != null) {
					return x;
				}
				else if (y != null) {
					return y;
				}

				break;
		}

		return null;
	}

	private static IUndoAction ClearPositionOffset(XmlElement item, string coordinate) {
		if (!item.HasAttribute(coordinate)) {
			return null;
		}

		string l = item.GetAttribute(coordinate);
		if (l.Trim() == "0") {
			return null;
		}

		item.RemoveAttribute(coordinate);
		return new SetAttributeAction(item, coordinate, l);
	}

	#endregion
}