using PDFPatcher.Common;

namespace PDFPatcher.Processor.ContentParser;

/// <summary>
/// 表示一个 PDF 操作符的元数据。可从 <see cref="OperatorTable"/> 的静态字段获取预定义的 <see cref="OperatorInfo"/> 实例。
/// </summary>
sealed class OperatorInfo(
	RenderCommandKind kind,
	string name,
	string description,
	OperatorCategory category,
	OperatorSemantic semantic,
	byte operandMask,
	bool isBeginScope = false,
	bool isEndScope = false,
	RenderCommandKind pairedBeginKind = RenderCommandKind.Unknown)
{
	public RenderCommandKind Kind { get; } = kind;
	public string Name { get; } = name;
	public string Description { get; } = description;
	public OperatorCategory Category { get; } = category;
	public OperatorSemantic Semantic { get; } = semantic;

	/// <summary>
	/// 参数数量掩码。
	/// 使用位操作校验：如果参数数量为 N，则检查<![CDATA[ (OperandMask & (1 << N)) != 0]]>。
	/// 如果为 0xFF，则表示参数数量可变。
	/// </summary>
	public byte OperandMask { get; } = operandMask;
	public bool IsBeginScope { get; } = isBeginScope;
	public bool IsEndScope { get; } = isEndScope;

	/// <summary>
	/// 对于结束范围的操作符，记录对应的开始范围 Kind（例如 Q -> q）。
	/// 对于非结束范围的操作符，值为 Unknown。
	/// </summary>
	public RenderCommandKind PairedBeginKind { get; } = pairedBeginKind;

	public bool MatchesOperandCount(int count) {
		// 如果是可变/特殊标记，直接通过
		if (OperandMask == 0xFF) return true;

		// PDF 规范中，操作符参数通常极少超过 6 个
		// 超过 6 个的（如数组操作或特殊流），通常已在 0xFF 处处理，或者直接视为不匹配
		if (count > 6) return false;

		// 位运算校验：检查第 count 位是否为 1
		return (OperandMask & (1 << count)) != 0;
	}

	public void Deconstruct(out RenderCommandKind kind, out string name) {
		kind = Kind;
		name = Name;
	}

	public override string ToString() {
		return Name;
	}
}

