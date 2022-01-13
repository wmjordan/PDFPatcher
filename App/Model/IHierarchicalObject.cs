using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model;

internal interface IHierarchicalObject<T>
{
	bool HasChildren { get; }
	ICollection<T> Children { get; }
}