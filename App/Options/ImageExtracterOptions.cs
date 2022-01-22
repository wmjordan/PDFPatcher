using System.Xml.Serialization;
using PowerJson;

namespace PDFPatcher;

public class ImageExtracterOptions
{
    public ImageExtracterOptions() {
        AutoOutputFolder = true;
        FileMask = "0000";
        MergeJpgToPng = true;
    }

    [XmlAttribute("自动指定输出位置")] public bool AutoOutputFolder { get; set; }

    [XmlAttribute("避免重复导出图片")] public bool SkipRedundantImages { get; set; }

    ///<summary>获取或指定是否合并相同页面、相同宽度的图片。</summary>
    [XmlAttribute("合并图片")]
    public bool MergeImages { get; set; }

    ///<summary>获取或指定是否将合并的 JPEG 图片存为无损的 PNG 图片。</summary>
    [XmlAttribute("合并JPG图片为PNG")]
    public bool MergeJpgToPng { get; set; }

    ///<summary>获取或指定是否垂直翻转导出的 PNG 或 TIFF 图片。</summary>
    [XmlAttribute("垂直翻转图片")]
    public bool VerticalFlipImages { get; set; }

    /// <summary>获取或指定是否反转黑白图片的颜色。</summary>
    [XmlAttribute("反转黑白图片颜色")]
    public bool InvertBlackAndWhiteImages { get; set; }

    [XmlAttribute("黑白图片导出为PNG")] public bool MonoPng { get; set; }

    ///<summary>获取或指定是否导出批注内的图片。</summary>
    [XmlAttribute("导出批注图片")]
    public bool ExtractAnnotationImages { get; set; }

    [XmlAttribute("最小高度")] public int MinHeight { get; set; }

    [XmlAttribute("最小宽度")] public int MinWidth { get; set; }

    ///<summary>获取或指定导出页面图像所保存的目录路径。</summary>
    [XmlAttribute("导出路径")]
    public string OutputPath { get; set; }

    ///<summary>获取或指定导出文件的名称掩码。</summary>
    [XmlAttribute("文件名称掩码")]
    public string FileMask { get; set; }

    [XmlAttribute("导出掩模")] public bool ExtractSoftMask { get; set; }

    [XmlAttribute("取反掩模")] public bool InvertSoftMask { get; set; }

    [XmlIgnore] [JsonInclude(false)] public string PageRange { get; set; }
}