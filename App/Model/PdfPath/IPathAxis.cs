﻿using System.Collections.Generic;

namespace PDFPatcher.Model.PdfPath;

public interface IPathAxis
{
	PathAxisType Type { get; }
	DocumentObject SelectObject(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates);
	IList<DocumentObject> SelectObjects(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates);
}