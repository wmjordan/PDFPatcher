namespace PDFPatcher.Processor.ContentParser;

/// <summary>
/// 操作符语义类型：定义操作符的作用性质
/// </summary>
public enum OperatorSemantic
{
	/// <summary>状态调整/设置：修改图形状态、颜色、文本属性、构造路径等，不直接产生输出</summary>
	StateSetup,

	/// <summary>内容输出：直接绘制文本、形状、图像等可见内容</summary>
	ContentOutput,

	/// <summary>结构分组：配对的块操作符，如 q/Q, BT/ET, BI/EI 等</summary>
	GroupStructure
}
