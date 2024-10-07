using System;
using System.Collections.Generic;

namespace PDFPatcher.Model
{
	interface IHierarchicalObject<T>
	{
		bool HasChildren { get; }
		ICollection<T> Children { get; }
	}
}
