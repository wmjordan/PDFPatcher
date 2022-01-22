using iTextSharp.text;

namespace PDFPatcher.Model;

internal sealed class AutoBookmarkContext
{
    internal int CurrentPageNumber { get; set; }
    internal int TotalPageNumber { get; set; }
    internal bool IsTextMerged { get; set; }
    internal TextInfo TextInfo { get; set; }
    internal TextLine TextLine { get; set; }
    internal Rectangle PageBox { get; set; }
}