using CLR;

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
}
