namespace PDFPatcher.Processor.ContentParser;

sealed class Operation(OperatorInfo opInfo, Token[] operands)
{
	public RenderCommandKind Kind => Info.Kind;
	public string Operator => Info.Name;

	public OperatorInfo Info { get; } = opInfo;
	public Token[] Operands { get; } = operands;

	public override string ToString() => Operands.Length == 0
		? Operator
		: $"{string.Join(" ", (object[])Operands)} {Operator}";

	public long GetInt64Operand(int index, long defaultValue = 0) {
		return index < Operands.Length
			? Operands[index].AsInt64(defaultValue)
			: defaultValue;
	}
	public float GetSingleOperand(int index, float defaultValue = 0) {
		return index < Operands.Length
			? Operands[index].AsFloat(defaultValue)
			: defaultValue;
	}
	public double GetDoubleOperand(int index, double defaultValue = 0) {
		return index < Operands.Length
			? Operands[index].AsDouble(defaultValue)
			: defaultValue;
	}
	public string GetStringOperand(int index, string defaultValue = default) {
		return index < Operands.Length
			? Operands[index].AsString(defaultValue)
			: defaultValue;
	}
}
