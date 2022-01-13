using System.ComponentModel;
using System.Xml.Serialization;

namespace PDFPatcher.Model;

public class PageBoxSettings
{
	public PageBoxSettings() {
		PaperSize = new PaperSize(PaperSize.AsPageSize, 0, 0);
		Margins = new Margins();
		AutoRotation = true;
	}

	[XmlElement("边框调整值")] public Margins Margins { get; set; }
	[XmlElement("指定尺寸")] public PaperSize PaperSize { get; set; }

	[XmlAttribute("页码范围")] public string PageRanges { get; set; }
	[XmlAttribute("页面筛选")] public PageFilterFlag Filter { get; set; }

	///<summary>获取或指定是否自动旋转页面适合图片纵横比。</summary>
	[XmlAttribute("自动旋转")]
	[DefaultValue(true)]
	public bool AutoRotation { get; set; }

	[XmlAttribute("旋转角度")] public int Rotation { get; set; }
	[XmlAttribute("拉伸内容")] public bool ScaleContent { get; set; }
	[XmlAttribute("水平对齐")] public HorizontalAlignment HorizontalAlign { get; set; }
	[XmlAttribute("垂直对齐")] public VerticalAlignment VerticalAlign { get; set; }
	[XmlAttribute("基准页面")] public int BasePage { get; set; }

	public bool NeedResize => PaperSize.SpecialSize != SpecialPaperSize.AsPageSize;
	public bool NeedAdjustMargins => Margins.IsEmpty == false;
}

public enum VerticalAlignment
{
	Middle, Top, Bottom
}

public enum HorizontalAlignment
{
	Center, Left, Right
}