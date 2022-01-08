using System.Xml.Serialization;
using PDFPatcher.Model;

namespace PDFPatcher
{
	public class ViewerOptions
	{
		///<summary>获取或指定初始查看状态的值。</summary>
		[XmlAttribute("初始状态")]
		public string InitialView { get; set; }
		///<summary>获取或指定双页阅读方向的值。</summary>
		[XmlAttribute("阅读方向")]
		public string Direction { get; set; }
		///<summary>获取或指定阅读器初始模式的值。</summary>
		[XmlAttribute("初始模式")]
		public string InitialMode { get; set; }
		///<summary>获取或指定是否删除 XYZ 目标的缩放比例，或将 Fit、FitH、FitV 转换为 XYZ。</summary>
		[XmlAttribute("删除缩放比例")]
		public bool RemoveZoomRate { get; set; }
		[XmlAttribute("强制内部链接")]
		public bool ForceInternalLink { get; set; }
		///<summary>获取或指定是否将书签状态设置为关闭。</summary>
		[XmlAttribute("书签状态")]
		public BookmarkStatus CollapseBookmark { get; set; }
		[XmlAttribute("指定阅读器设置")]
		public bool SpecifyViewerPreferences { get; set; }
		[XmlAttribute("隐藏菜单")]
		public bool HideMenu { get; set; }
		[XmlAttribute("隐藏工具栏")]
		public bool HideToolbar { get; set; }
		[XmlAttribute("隐藏程序界面")]
		public bool HideUI { get; set; }
		[XmlAttribute("适合窗口")]
		public bool FitWindow { get; set; }
		[XmlAttribute("窗口居中")]
		public bool CenterWindow { get; set; }
		[XmlAttribute("显示文档标题")]
		public bool DisplayDocTitle { get; set; }
	}
}
