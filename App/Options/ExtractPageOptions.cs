using System.Xml.Serialization;

namespace PDFPatcher;

public class ExtractPageOptions
{
	public ExtractPageOptions() {
		KeepBookmarks = true;
		KeepDocumentProperties = true;
		RemoveDocumentRestrictions = true;
		RemoveOrphanBookmarks = true;
		NumberFileNames = true;
		SeparateByPage = 1;
	}

	[XmlAttribute("保留文档属性")] public bool KeepDocumentProperties { get; set; }

	[XmlAttribute("保留文档书签")] public bool KeepBookmarks { get; set; }

	[XmlAttribute("删除无效书签")] public bool RemoveOrphanBookmarks { get; set; }

	[XmlAttribute("解除文档限制")] public bool RemoveDocumentRestrictions { get; set; }

	[XmlAttribute("添加编号")] public bool NumberFileNames { get; set; }

	[XmlAttribute("拆分方式")] public int SeparatingMode { get; set; }

	[XmlAttribute("按页数拆分")] public int SeparateByPage { get; set; }

	[XmlIgnore] public string PageRanges { get; set; }

	[XmlIgnore] public string ExcludePageRanges { get; set; }
}