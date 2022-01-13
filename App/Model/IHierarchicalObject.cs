using System.Collections.Generic;

namespace PDFPatcher.Model;

internal interface IHierarchicalObject<T>
{
	bool HasChildren { get; }
	ICollection<T> Children { get; }
}