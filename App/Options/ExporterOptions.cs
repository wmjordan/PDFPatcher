using System;
using System.Xml.Serialization;
using PDFPatcher.Common;
using E = System.Text.Encoding;

namespace PDFPatcher;

public class ExporterOptions
{
	private E _Encoding = E.Default;
	private string _EncodingName;

	///<summary>获取或指定是否导出文档属性。</summary>
	[XmlAttribute("导出文档属性")]
	public bool ExportDocProperties { get; set; } = true;

	///<summary>获取或指定是否导出书签。</summary>
	[XmlAttribute("导出文档书签")]
	public bool ExportBookmarks { get; set; } = true;

	///<summary>获取或指定是否导出页面内的连接。</summary>
	[XmlAttribute("导出页面链接")]
	public bool ExtractPageLinks { get; set; } = true;

	///<summary>获取或指定是否导出阅读器设置。</summary>
	[XmlAttribute("导出阅读器设置")]
	public bool ExportViewerPreferences { get; set; } = true;

	///<summary>获取或指定是否导出页面的阅读设置。</summary>
	[XmlAttribute("导出页面设置")]
	public bool ExtractPageSettings { get; set; } = true;

	///<summary>获取或指定是否导出文档编录信息。</summary>
	[XmlAttribute("导出编录信息")]
	public bool ExportCatalog { get; set; }

	///<summary>获取或指定是否导出页面内容信息。</summary>
	[XmlAttribute("导出页面内容")]
	public bool ExtractPageContent { get; set; }

	///<summary>获取或指定需要导出的页码范围。页码范围可用“-”表示起止页码，如有多个页码，可用“;”、“,”或“ ”（空格）隔开，如“1;4-15;2 56”，表示依次导出第1页、第4至15页、第2页和第56页的内容。</summary>
	[XmlAttribute("导出页码范围")]
	public string ExtractPageRange { get; set; }

	///<summary>获取或指定页面字典的值。</summary>
	[XmlAttribute("导出页面字典")]
	public bool ExtractPageDictionary { get; set; }

	///<summary>获取或指定是否导出页面中的图片为独立的文件。</summary>
	[XmlAttribute("导出图片")]
	public bool ExtractImages { get; set; }

	///<summary>获取或指定是否解码导出页面中的文本。</summary>
	[XmlAttribute("导出解码文本")]
	public bool ExportDecodedText { get; set; }

	///<summary>获取或指定是否解码导出页面指令。</summary>
	[XmlAttribute("导出命令操作符")]
	public bool ExportContentOperators { get; set; }

	///<summary>获取或指定导出二进制流的字节数。</summary>
	[XmlAttribute("导出二进制流")]
	public int ExportBinaryStream { get; set; } = 200;

	///<summary>获取或指定导出前是否解析命名位置。</summary>
	[XmlAttribute("解析命名位置")]
	public bool ConsolidateNamedDestinations { get; set; }

	///<summary>获取导出图像的选项。</summary>
	[XmlIgnore]
	public ImageExtracterOptions Images { get; } = new();

	[XmlElement("导出尺寸单位")] public UnitConverter UnitConverter { get; set; } = new();

	///<summary>获取或指定导出文件时所用的编码。</summary>
	[XmlAttribute("文本编码")]
	public string Encoding {
		get {
			if (_Encoding.EncodingName == E.Default.EncodingName) {
				return Constants.Encoding.SystemDefault;
			}

			return _EncodingName;
		}
		set {
			if (string.IsNullOrEmpty(value) || value == Constants.Encoding.SystemDefault) {
				_Encoding = E.Default;
			}
			else {
				try {
					_Encoding = E.GetEncoding(value);
					_EncodingName = value;
				}
				catch (Exception) {
					_EncodingName = Constants.Encoding.SystemDefault;
				}
			}
		}
	}

	public E GetEncoding() {
		return _Encoding;
	}
}