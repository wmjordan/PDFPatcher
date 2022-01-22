using System.Xml.Serialization;

namespace PDFPatcher.Model;

public class PageLabel
{
	[XmlAttribute(Constants.PageLabelsAttributes.PageNumber)]
	public int PageNumber { get; set; }

	[XmlAttribute(Constants.PageLabelsAttributes.Prefix)]
	public string Prefix { get; set; }

	[XmlAttribute(Constants.PageLabelsAttributes.StartPage)]
	public int StartPage { get; set; }

	[XmlAttribute(Constants.PageLabelsAttributes.Style)]
	public string Style { get; set; }
}