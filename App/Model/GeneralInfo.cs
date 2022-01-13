using System;
using System.Xml.Serialization;

namespace PDFPatcher.Model;

[XmlRoot(Constants.Info.ThisName)]
public class GeneralInfo
{
	[XmlAttribute("指定文档元数据属性")] public bool SpecifyMetaData { get; set; }
	[XmlAttribute("重写XML元数据属性")] public bool RewriteXmp { get; set; }

	[XmlAttribute(Constants.Info.Title)] public string Title { get; set; }
	[XmlAttribute(Constants.Info.Author)] public string Author { get; set; }
	[XmlAttribute(Constants.Info.Subject)] public string Subject { get; set; }

	[XmlAttribute(Constants.Info.Keywords)]
	public string Keywords { get; set; }

	[XmlIgnore] public string Creator { get; set; }
	[XmlIgnore] public string Producer { get; set; }
	[XmlIgnore] public string CreationDate { get; set; }
	[XmlIgnore] public string ModDate { get; set; }
}