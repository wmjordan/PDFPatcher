using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model.PdfPath
{
	public interface IPathAxis
	{
		PathAxisType Type { get; }
		DocumentObject SelectObject (DocumentObject source, string name, IEnumerable<IPathPredicate> predicates);
		IList<DocumentObject> SelectObjects (DocumentObject source, string name, IEnumerable<IPathPredicate> predicates);
	}

}
