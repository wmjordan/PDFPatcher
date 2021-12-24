using System;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PDFPatcher.Common;

namespace PDFPatcher.Model
{
	/// <summary>
	/// 在合并文件功能中使用的书签设置。
	/// </summary>
	public class BookmarkSettings : ICloneable, IXmlSerializable
	{
		public string Title { get; set; }
		public bool IsBold { get; set; }
		public bool IsItalic { get; set; }
		public bool IsOpened { get; set; }
		public Color ForeColor { get; set; }

		public BookmarkSettings () {
			ForeColor = Color.Transparent;
		}

		public BookmarkSettings (string title) {
			Title = title;
		}

		public BookmarkSettings Clone () {
			return (BookmarkSettings)MemberwiseClone ();
		}

		public BookmarkSettings (BookmarkElement element) {
			Title = element.Title;
			IsBold = (element.TextStyle & FontStyle.Bold) == FontStyle.Bold;
			IsItalic = (element.TextStyle & FontStyle.Italic) == FontStyle.Italic;
			IsOpened = element.IsOpen;
			ForeColor = element.ForeColor;
		}

		#region ICloneable 成员

		object ICloneable.Clone () {
			return Clone ();
		}

		#endregion

		#region IXmlSerializable 成员

		public System.Xml.Schema.XmlSchema GetSchema () {
			return null;
		}

		public void ReadXml (XmlReader reader) {
			Title = reader.GetAttribute ("title");
			IsBold = reader.GetValue ("bold", false);
			IsItalic = reader.GetValue ("italic", false);
			IsOpened = reader.GetValue ("opened", false);
			ForeColor = Color.FromArgb (reader.GetValue ("color", Color.Empty.ToArgb ()));
		}

		public void WriteXml (XmlWriter writer) {
			writer.WriteStartElement ("bookmark");
			writer.WriteAttributeString ("title", Title);
			writer.WriteValue ("bold", IsBold, false);
			writer.WriteValue ("italic", IsItalic, false);
			writer.WriteValue ("opened", IsOpened, false);
			writer.WriteValue ("color", ForeColor.ToArgb (), Color.Empty.ToArgb ());
			writer.WriteEndElement ();
		}

		#endregion
	}
}
