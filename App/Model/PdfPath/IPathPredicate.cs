namespace PDFPatcher.Model.PdfPath;

public interface IPathPredicate
{
	PredicateOperatorType Operator { get; }
	bool Match(DocumentObject source, IPathValue value1, IPathValue value2);
}