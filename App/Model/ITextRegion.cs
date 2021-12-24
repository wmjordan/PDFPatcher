using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model
{
	interface ITextRegion
	{
		string Text { get; }
		Bound Region { get; }
	}

	interface IDirectionalBoundObject : ITextRegion
	{
		WritingDirection Direction { get; }
	}
}
