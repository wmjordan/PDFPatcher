using System.Xml.Serialization;

namespace PDFPatcher.Model;

public enum PageResizing
{
    [XmlEnum("保持不变")] None = 0,
    [XmlEnum("更改页面尺寸")] Resize = 1,
    [XmlEnum("拉伸页面内容")] Scale = 2
}

public enum ResizingMode
{
    [XmlEnum("相对调整")] Relative = 0,
    [XmlEnum("绝对调整")] Absolute = 1,
    [XmlEnum("同指定页")] AsPage = 2
}