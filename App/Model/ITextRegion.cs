using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model;

internal interface ITextRegion
{
	string Text { get; }
	Bound Region { get; }
}

internal interface IDirectionalBoundObject : ITextRegion
{
	WritingDirection Direction { get; }
}