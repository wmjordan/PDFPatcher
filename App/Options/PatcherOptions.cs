using System.Collections.Generic;
using System.Xml.Serialization;
using PDFPatcher.Model;
using PowerJson;

namespace PDFPatcher;

[XmlRoot("文档设置")]
public class PatcherOptions : DocumentOptions
{
	public PatcherOptions() {
		PageSettings = new List<PageBoxSettings>();
		FontSubstitutions = new List<FontSubstitution>();
		UnifiedPageSettings = new PageBoxSettings();
	}

	[XmlAttribute("嵌入字库")] public bool EmbedFonts { get; set; }
	[XmlAttribute("删除文本尾随空白")] public bool TrimTrailingWhiteSpace { get; set; }
	[XmlAttribute("允许替换字库")] public bool EnableFontSubstitutions { get; set; }
	[XmlAttribute("修复内容流")] public bool FixContents { get; set; }
	[XmlAttribute("删除批注")] public bool RemoveAnnotations { get; set; }
	[XmlAttribute("删除导航书签")] public bool RemoveBookmarks { get; set; }
	[XmlAttribute("删除页面开头指令")] [XmlIgnore] public int RemoveLeadingCommandCount { get; set; }
	[XmlAttribute("删除页面结尾指令")] [XmlIgnore] public int RemoveTrailingCommandCount { get; set; }
	[XmlAttribute("删除使用限制")] public bool RemoveUsageRights { get; set; }
	[XmlAttribute("删除文档自动动作")] public bool RemoveDocAutoActions { get; set; }
	[XmlAttribute("删除页面自动动作")] public bool RemovePageAutoActions { get; set; }
	[XmlAttribute("删除页面表单")] public bool RemovePageForms { get; set; }
	[XmlAttribute("删除链接批注")] public bool RemovePageLinks { get; set; }
	[XmlAttribute("删除页面元数据")] public bool RemovePageMetaData { get; set; }
	[XmlAttribute("删除页面文本")] public bool RemovePageTextBlocks { get; set; }
	[XmlAttribute("删除页面缩略图")] public bool RemovePageThumbnails { get; set; }
	[XmlAttribute("删除XML元数据")] public bool RemoveXmlMetadata { get; set; }
	[XmlAttribute("优化黑白图片压缩算法")] public bool RecompressWithJbig2 { get; set; }

	[XmlElement("页面布局")] public PageBoxSettings UnifiedPageSettings { get; set; }

	[XmlArray("页面设置")]
	[XmlArrayItem("设置项")]
	public List<PageBoxSettings> PageSettings { get; }

	[XmlArray("字体替换")]
	[XmlArrayItem("替换项")]
	public List<FontSubstitution> FontSubstitutions { get; }
}

public class FontSubstitution
{
	[XmlAttribute("原字体")] public string OriginalFont { get; set; }

	[XmlAttribute("新字体")]
	[JsonField("SubstitutionFont")]
	public string Substitution { get; set; }

	[XmlAttribute("原字符")] public string OriginalCharacters { get; set; }
	[XmlAttribute("替换字符")] public string SubstituteCharacters { get; set; }
}