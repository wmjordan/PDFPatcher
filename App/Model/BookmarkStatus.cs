using System.Xml.Serialization;

namespace PDFPatcher.Model
{
	public enum BookmarkStatus
	{
		[XmlEnum("保持不变")]
		AsIs,
		[XmlEnum("全部关闭")]
		CollapseAll,
		[XmlEnum("全部打开")]
		ExpandAll,
		[XmlEnum("打开首层")]
		ExpandTop
	}
}
