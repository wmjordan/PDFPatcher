using System.Collections.Generic;

namespace PDFPatcher.Model.PdfPath;

public interface IPathExpression : IPathValue
{
	IList<IPathPredicate> Predicates { get; }
	IPathAxis Axis { get; }
	string Name { get; }
	DocumentObject SelectObject(DocumentObject source);
}