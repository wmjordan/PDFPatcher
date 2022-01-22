using System.Collections.Generic;

namespace PDFPatcher.Model;

internal interface IHierarchicalObject<T>
{
	ICollection<T> Children { get; }
}