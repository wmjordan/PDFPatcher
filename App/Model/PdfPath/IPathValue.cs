namespace PDFPatcher.Model.PdfPath;

public interface IPathValue
{
	PathValueType ValueType { get; }
}

public interface IConstantPathValue : IPathValue
{
}