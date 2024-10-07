
namespace PDFPatcher.Processor
{
	sealed class ClearDestinationOffsetProcessor : IPdfInfoXmlProcessor, IPdfInfoXmlProcessor<ClearDestinationOffsetProcessor.PositionType>
	{
		public enum PositionType { X, Y, XY }

		string _name;
		PositionType _type;
		public PositionType Parameter {
			get => _type;
			set {
				_type = value;
				_name = _type switch {
					PositionType.X => "横",
					PositionType.Y => "纵",
					_ => string.Empty,
				};
			}
		}

		public ClearDestinationOffsetProcessor() {
		}
		public ClearDestinationOffsetProcessor(PositionType type) {
			Parameter = type;
		}
		#region IInfoDocProcessor 成员

		public string Name => $"清除{_name}坐标定位偏移值";

		public IUndoAction Process(System.Xml.XmlElement item) {
			if (item.GetAttribute(Constants.DestinationAttributes.View) == Constants.DestinationAttributes.ViewType.FitR) {
				return null;
			}
			switch (_type) {
				case PositionType.X:
					return ClearPositionOffset(item, Constants.Coordinates.Left);
				case PositionType.Y:
					return ClearPositionOffset(item, Constants.Coordinates.Top);
				case PositionType.XY:
					var x = ClearPositionOffset(item, Constants.Coordinates.Left);
					var y = ClearPositionOffset(item, Constants.Coordinates.Top);
					if (x != null && y != null) {
						var g = new UndoActionGroup();
						g.Add(x);
						g.Add(y);
						return g;
					}
					if (x != null) {
						return x;
					}
					if (y != null) {
						return y;
					}
					break;
			}
			return null;
		}

		static IUndoAction ClearPositionOffset(System.Xml.XmlElement item, string coordinate) {
			if (!item.HasAttribute(coordinate)) {
				return null;
			}

			var l = item.GetAttribute(coordinate);
			if (l.Trim() == "0") {
				return null;
			}
			item.RemoveAttribute(coordinate);
			return new SetAttributeAction(item, coordinate, l);
		}

		#endregion
	}
}
