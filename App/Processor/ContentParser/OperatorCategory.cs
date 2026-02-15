namespace PDFPatcher.Processor.ContentParser;

/// <summary>
/// 操作符功能类别枚举
/// </summary>
public enum OperatorCategory
{
	/// <summary>未知或未分类</summary>
	Unknown = 0,
	/// <summary>图形状态</summary>
	GraphicsState,
	/// <summary>颜色相关 (Gray, RGB, CMYK, Pattern, ColorSpace)</summary>
	Color,
	/// <summary>路径构造</summary>
	Path,
	/// <summary>路径绘制</summary>
	Painting,
	/// <summary>文本相关 (Text State, Text Positioning, Text Showing)</summary>
	Text,
	/// <summary>标记内容</summary>
	MarkedContent,
	/// <summary>兼容性段</summary>
	Compatibility,
	/// <summary>内联图像</summary>
	Image,
	/// <summary>XObject 和 Shading</summary>
	XObject
}
