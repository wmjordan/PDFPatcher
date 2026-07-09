using System;
using System.Xml.Serialization;

namespace PDFPatcher;

public class ReaderOptions
{
	[XmlAttribute("整页翻页")]
	public bool FullPageScroll { get; set; }
	[XmlAttribute("黑白显示")]
	public bool GrayScale { get; set; }
	[XmlAttribute("显示文本边框")]
	public bool ShowTextBoder { get; set; }
	[XmlAttribute("隐藏文档标注")]
	public bool HideAnnotation { get; set; }
	[XmlAttribute("缩放状态")]
	public string Zoom { get; set; }
	[XmlAttribute("滚动方向")]
	public Functions.Editor.ContentDirection ContentDirection { get; set; }
	[XmlAttribute("书签栏")]
	public BookmarkState BookmarkState { get; set; }
	[XmlAttribute("书签字体")]
	public string BookmarkFont { get; set; }
	[XmlAttribute("书签字体尺寸")]
	public float BookmarkFontSize { get; set; }
	[XmlAttribute("连续编辑书签")]
	public bool ContinuousBookmarkEdit { get; set; }
	[XmlAttribute("编辑书签转到页面")]
	public bool LocateBookmarkOnEdit { get; set; }

	internal static readonly string[] ZoomModes = ["适合页面", "适合页宽", "适合页高",
		"20%", "30%", "50%", "75%", "100%",
		"133%", "150%", "200%", "300%", "400%"];
}

public enum BookmarkState
{
	Auto,
	Show,
	Hide
}
