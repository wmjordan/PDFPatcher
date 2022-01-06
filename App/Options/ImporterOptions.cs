using System.Xml.Serialization;

namespace PDFPatcher
{
	public class ImporterOptions
	{
		///<summary>获取或指定是否导入文档属性信息。</summary>
		[XmlAttribute("导入文档属性")]
		public bool ImportDocProperties { get; set; }

		///<summary>获取或指定是否导入书签。</summary>
		[XmlAttribute("导入文档书签")]
		public bool ImportBookmarks { get; set; }

		///<summary>获取或指定是否导入页面内的连接。</summary>
		[XmlAttribute("导入页面链接")]
		public bool ImportPageLinks { get; set; }

		///<summary>获取或指定是否保留页面内的连接。</summary>
		[XmlAttribute("保留页面链接")]
		public bool KeepPageLinks { get; set; }

		///<summary>获取或指定是否导入阅读器设置。</summary>
		[XmlAttribute("导入阅读器设置")]
		public bool ImportViewerPreferences { get; set; }

		///<summary>获取或指定是否导入页面的阅读设置。</summary>
		[XmlAttribute("导入页面设置")]
		public bool ImportPageSettings { get; set; }

		public ImporterOptions() {
			ImportDocProperties = true;
			ImportBookmarks = true;
			ImportPageLinks = true;
			ImportViewerPreferences = true;
			ImportPageSettings = true;
		}
	}
}
