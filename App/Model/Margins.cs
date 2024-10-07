using System;
using System.Xml.Serialization;

namespace PDFPatcher.Model
{
	public class Margins
	{
		[XmlAttribute(Constants.Coordinates.Top)]
		public float Top { get; set; }
		[XmlAttribute(Constants.Coordinates.Right)]
		public float Right { get; set; }
		[XmlAttribute(Constants.Coordinates.Left)]
		public float Left { get; set; }
		[XmlAttribute(Constants.Coordinates.Bottom)]
		public float Bottom { get; set; }
		[XmlAttribute(Constants.Coordinates.ScaleFactor)]
		public bool IsRelative { get; set; }

		public bool IsEmpty => Top == 0 && Bottom == 0 && Left == 0 && Right == 0;

		public Margins() {
		}

		public Margins(float left, float top, float right, float bottom) {
			Top = top;
			Left = left;
			Bottom = bottom;
			Right = right;
		}
	}
}
