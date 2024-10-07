using System;
using System.Xml.Serialization;
using PowerJson;

namespace PDFPatcher
{
	[XmlRoot("处理选项")]
	public class ConfigurationSerialization
	{
		[XmlAttribute("检查更新时间")]
		public DateTime CheckUpdateDate { get; set; }
		[XmlAttribute("检查更新间隔")]
		public int CheckUpdateInterval { get; set; } = 14;

		[XmlAttribute("保存程序设置")]
		public bool SaveAppSettings { get; set; }

		[XmlAttribute("文档加载模式")]
		public string PdfLoadMode { get; set; }

		[XmlElement("编码设置")]
		public EncodingOptions Encodings { get; set; }
		///<summary>获取导出设置。</summary>
		[XmlElement("信息文件导出设置")]
		public ExporterOptions ExporterOptions { get; set; }
		///<summary>获取导入设置。</summary>
		[XmlElement("信息文件导入设置")]
		public ImporterOptions ImporterOptions { get; set; }
		///<summary>获取生成文档的设置。</summary>
		[XmlElement("PDF文件处理设置")]
		public MergerOptions MergerOptions { get; set; }
		[XmlElement("PDF文档设置")]
		public PatcherOptions PatcherOptions { get; set; }
		[XmlElement("PDF编辑器设置")]
		public PatcherOptions EditorOptions { get; set; }
		[XmlElement("PDF阅读器设置")]
		public ReaderOptions ReaderOptions { get; set; }
		[XmlElement("自动生成书签设置")]
		public AutoBookmarkOptions AutoBookmarkOptions { get; set; }
		[XmlElement("导出图像设置")]
		public ImageExtracterOptions ImageExporterOptions { get; set; }
		[XmlElement("转为图片设置")]
		public MuPDF.ImageRendererOptions ImageRendererOptions { get; set; }
		[XmlElement("提取页面设置")]
		public ExtractPageOptions ExtractPageOptions { get; set; }
		[XmlElement("文本识别设置")]
		public OcrOptions OcrOptions { get; set; }
		[XmlElement("工具栏设置")]
		public ToolbarOptions ToolbarOptions { get; set; }
		[XmlElement("窗口设置")]
		public WindowStatus WindowStatus { get; set; }

		[JsonField("最近使用的文档")]
		[JsonInclude]
		[JsonSerializable]
		internal AppContext.RecentItems Recent { get; set; }
	}
}
