using System;
using System.ComponentModel;

namespace PDFPatcher.Processor.ContentParser;

/// <summary>
/// 用于标注 PDF 操作符名称的自定义特性
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class PdfOperatorAttribute(string name) : Attribute
{
	public string Name { get; } = name;
}

public enum RenderCommandKind
{
	// 未知 / 容错
	[PdfOperator("")]
	[Description("Unknown")]
	Unknown,

	// 图形状态（容器）
	[PdfOperator("q")]
	[Description("Save graphics state")]
	GSave,
	[PdfOperator("Q")]
	[Description("Restore graphics state")]
	GRestore,

	// 文本对象（容器）
	[PdfOperator("BT")]
	[Description("Begin text object")]
	BeginText,
	[PdfOperator("ET")]
	[Description("End text object")]
	EndText,

	// 兼容性段（容器）
	[PdfOperator("BX")]
	[Description("Begin compatibility section")]
	BeginCompatibilitySection,
	[PdfOperator("EX")]
	[Description("End compatibility section")]
	EndCompatibilitySection,

	// 标记内容（容器）
	[PdfOperator("BMC")]
	[Description("Begin marked-content sequence")]
	BeginMarkedContent,
	[PdfOperator("BDC")]
	[Description("Begin marked-content sequence with property list")]
	BeginMarkedContentWithProps,
	[PdfOperator("EMC")]
	[Description("End marked-content sequence")]
	EndMarkedContent,

	// 标记内容（点）
	[PdfOperator("MP")]
	[Description("Define marked-content point")]
	DefineMarkedContentPoint,
	[PdfOperator("DP")]
	[Description("Define marked-content point with property list")]
	DefineMarkedContentPointWithProps,

	// 内联图像（容器）
	[PdfOperator("BI")]
	[Description("Begin inline image object")]
	BeginInlineImage,
	[PdfOperator("ID")]
	[Description("Begin inline image data")]
	BeginInlineImageData,
	[PdfOperator("EI")]
	[Description("End inline image object")]
	EndInlineImage,

	// 文本绘制
	[PdfOperator("Tj")]
	[Description("Show text")]
	ShowText,
	[PdfOperator("TJ")]
	[Description("Show text, allowing individual glyph positioning")]
	ShowTextWithSpacing,
	[PdfOperator("'")]
	[Description("Move to next line and show text")]
	NextLineShowText,
	[PdfOperator("\"")]
	[Description("Set word and character spacing, move to next line, and show text")]
	MoveToNextLineAndShowText,

	// 文本状态
	[PdfOperator("Tf")]
	[Description("Set text font and size")]
	SetFont,
	[PdfOperator("Tc")]
	[Description("Set character spacing")]
	SetCharSpacing,
	[PdfOperator("Tw")]
	[Description("Set word spacing")]
	SetWordSpacing,
	[PdfOperator("Tz")]
	[Description("Set horizontal text scaling")]
	SetHorizontalScaling,
	[PdfOperator("TL")]
	[Description("Set text leading")]
	SetTextLeading,
	[PdfOperator("Tr")]
	[Description("Set text rendering mode")]
	SetTextRenderMode,
	[PdfOperator("Ts")]
	[Description("Set text rise")]
	SetTextRise,
	[PdfOperator("Td")]
	[Description("Move text position")]
	MoveText,
	[PdfOperator("TD")]
	[Description("Move text position and set leading")]
	MoveTextSetLeading,
	[PdfOperator("Tm")]
	[Description("Set text matrix and text line matrix")]
	SetTextMatrix,
	[PdfOperator("T*")]
	[Description("Move to start of next text line")]
	NextLine,

	// Type 3 字形
	[PdfOperator("d0")]
	[Description("Set glyph width in Type 3 font")]
	SetGlyphWidthInType3Font,
	[PdfOperator("d1")]
	[Description("Set glyph width and bounding box in Type 3 font")]
	SetGlyphWidthAndBoundingBoxInType3Font,

	// 颜色（stroke / non-stroke）
	[PdfOperator("CS")]
	[Description("Set color space for stroking operations")]
	SetColorSpaceStroking,
	[PdfOperator("cs")]
	[Description("Set color space for non-stroking operations")]
	SetColorSpaceNonStroking,
	[PdfOperator("SC")]
	[Description("Set color for stroking operations")]
	SetColorStroking,
	[PdfOperator("sc")]
	[Description("Set color for non-stroking operations")]
	SetColorNonStroking,
	[PdfOperator("SCN")]
	[Description("Set color for stroking operations (ICCBased and special color spaces)")]
	SetColorPatternStroking,
	[PdfOperator("scn")]
	[Description("Set color for non-stroking operations (ICCBased and special color spaces)")]
	SetColorPatternNonStroking,
	[PdfOperator("G")]
	[Description("Set gray level for stroking operations")]
	SetGrayStroking,
	[PdfOperator("g")]
	[Description("Set gray level for non-stroking operations")]
	SetGrayNonStroking,
	[PdfOperator("RG")]
	[Description("Set RGB color for stroking operations")]
	SetRGBStroking,
	[PdfOperator("rg")]
	[Description("Set RGB color for non-stroking operations")]
	SetRGBNonStroking,
	[PdfOperator("K")]
	[Description("Set CMYK color for stroking operations")]
	SetCMYKStroking,
	[PdfOperator("k")]
	[Description("Set CMYK color for non-stroking operations")]
	SetCMYKNonStroking,

	// 路径构造
	[PdfOperator("m")]
	[Description("Begin new sub-path")]
	MoveTo,
	[PdfOperator("l")]
	[Description("Append straight line segment to path")]
	LineTo,
	[PdfOperator("c")]
	[Description("Append curved segment to path (three control points)")]
	CurveTo,
	[PdfOperator("v")]
	[Description("Append curved segment to path (initial point replicated)")]
	CurveToNoFirstControl,
	[PdfOperator("y")]
	[Description("Append curved segment to path (final point replicated)")]
	CurveToNoSecondControl,
	[PdfOperator("re")]
	[Description("Append rectangle to path")]
	Rectangle,
	[PdfOperator("h")]
	[Description("Close sub-path")]
	ClosePath,

	// 路径绘制（填充与描边）
	[PdfOperator("S")]
	[Description("Stroke path")]
	StrokePath,
	[PdfOperator("s")]
	[Description("Close and stroke path")]
	CloseAndStrokePath,
	[PdfOperator("f")]
	[Description("Fill path using non-zero winding number rule")]
	FillPathNonZero,
	[PdfOperator("F")]
	[Description("Fill path using non-zero winding number rule (deprecated in PDF 2.0)")]
	FillPathNonZeroDeprecated,
	[PdfOperator("f*")]
	[Description("Fill path using even-odd rule")]
	FillPathEvenOdd,
	[PdfOperator("B")]
	[Description("Fill and stroke path using non-zero winding number rule")]
	FillStrokePathNonZero,
	[PdfOperator("B*")]
	[Description("Fill and stroke path using even-odd rule")]
	FillStrokePathEvenOdd,
	[PdfOperator("b")]
	[Description("Close, fill, and stroke path using non-zero winding number rule")]
	CloseFillStrokePathNonZero,
	[PdfOperator("b*")]
	[Description("Close, fill, and stroke path using even-odd rule")]
	CloseFillStrokePathEvenOdd,
	[PdfOperator("n")]
	[Description("End path without filling or stroking")]
	EndPath,

	// 路径绘制（裁剪）
	[PdfOperator("W")]
	[Description("Set clipping path using non-zero winding number rule")]
	SetClipPath,
	[PdfOperator("W*")]
	[Description("Set clipping path using even-odd rule")]
	SetClipPathEvenOdd,

	// 图形状态
	[PdfOperator("w")]
	[Description("Set line width")]
	SetLineWidth,
	[PdfOperator("J")]
	[Description("Set line cap style")]
	SetLineCap,
	[PdfOperator("j")]
	[Description("Set line join style")]
	SetLineJoin,
	[PdfOperator("M")]
	[Description("Set miter limit")]
	SetMiterLimit,
	[PdfOperator("d")]
	[Description("Set line dash pattern")]
	SetDashPattern,
	[PdfOperator("ri")]
	[Description("Set color rendering intent")]
	SetRenderingIntent,
	[PdfOperator("i")]
	[Description("Set flatness tolerance")]
	SetFlatness,
	[PdfOperator("gs")]
	[Description("Set parameters from graphics state parameter dictionary")]
	SetGraphicsState,

	// 变换矩阵
	[PdfOperator("cm")]
	[Description("Concatenate matrix to current transformation matrix")]
	ConcatMatrix,

	// XObject / Shading
	[PdfOperator("Do")]
	[Description("Invoke named XObject")]
	PaintXObject,
	[PdfOperator("sh")]
	[Description("Paint area defined by shading pattern")]
	PaintShading,
}
