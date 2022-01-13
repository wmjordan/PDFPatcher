using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model
{
	interface IHierarchicalObject<T>
	{
		bool HasChildren { get; }
		ICollection<T> Children { get; }
	}
}