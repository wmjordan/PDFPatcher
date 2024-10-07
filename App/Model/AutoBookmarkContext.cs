using System;

namespace PDFPatcher.Model
{
	sealed class AutoBookmarkContext
	{
		internal int CurrentPageNumber { get; set; }
		internal int TotalPageNumber { get; set; }
		internal bool IsTextMerged { get; set; }
		internal TextInfo TextInfo { get; set; }
		internal TextLine TextLine { get; set; }
		internal iTextSharp.text.Rectangle PageBox { get; set; }
	}
}
