
namespace PDFPatcher.Processor
{
	sealed class ClearDestinationOffsetProcessor : IPdfInfoXmlProcessor, IPdfInfoXmlProcessor<ClearDestinationOffsetProcessor.PositionType>
	{
		public enum PositionType { X, Y, XY }

		PositionType _type;
		public PositionType Parameter {
			get { return _type; }
			set {
				_type = value;
				switch (_type) {
					case PositionType.X:
						_name = "横";
						break;
					case PositionType.Y:
						_name = "纵";
						break;
					default:
						_name = string.Empty;
						break;
				}
			}
		}

		string _name;

		public ClearDestinationOffsetProcessor () {
		}
		public ClearDestinationOffsetProcessor (PositionType type) {
			Parameter = type;
			switch (type) {
				case PositionType.X:
					_name = "横";
					break;
				case PositionType.Y:
					_name = "纵";
					break;
				default:
					_name = string.Empty;
					break;
			}
		}
		#region IInfoDocProcessor 成员

		public string Name {
			get { return "清除" + _name + "坐标定位偏移值"; }
		}

		public IUndoAction Process (System.Xml.XmlElement item) {
			if (item.GetAttribute (Constants.DestinationAttributes.View) == Constants.DestinationAttributes.ViewType.FitR) {
				return null;
			}
			switch (_type) {
				case PositionType.X:
					return ClearPositionOffset (item, Constants.Coordinates.Left);
				case PositionType.Y:
					return ClearPositionOffset (item, Constants.Coordinates.Top);
				case PositionType.XY:
					var x = ClearPositionOffset (item, Constants.Coordinates.Left);
					var y = ClearPositionOffset (item, Constants.Coordinates.Top);
					if (x != null && y != null) {
						var g = new UndoActionGroup ();
						g.Add (x);
						g.Add (y);
						return g;
					}
					else if (x != null) {
						return x;
					}
					else if (y != null) {
						return y;
					}
					break;
				default:
					break;
			}
			return null;
		}

		private static IUndoAction ClearPositionOffset (System.Xml.XmlElement item, string coordinate) {
			if (item.HasAttribute (coordinate)) {
				var l = item.GetAttribute (coordinate);
				if (l.Trim () == "0") {
					return null;
				}
				item.RemoveAttribute (coordinate);
				return new SetAttributeAction (item, coordinate, l);
			}
			return null;
		}

		#endregion
	}
}
