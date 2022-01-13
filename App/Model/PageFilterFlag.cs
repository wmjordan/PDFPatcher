using System;

namespace PDFPatcher.Model;

[Flags]
public enum PageFilterFlag
{
	NotSpecified,
	Odd = 1,
	Even = 2,
	Portrait = 4,
	Landscape = 8,
	All = Odd | Even | Portrait | Landscape
}