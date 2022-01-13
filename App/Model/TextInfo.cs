using System.Diagnostics;
using System.Drawing;
using iTextSharp.text.pdf;

namespace PDFPatcher.Model;

[DebuggerDisplay("Region={Region.Bottom},{Region.Left}; Size={Size}; Text={Text}")]
internal sealed class TextInfo : ITextRegion
{
	public PdfString PdfString { get; set; }
	internal float Size { get; set; }
	internal FontInfo Font { get; set; }
	internal Color Color { get; set; }
	internal float LetterWidth { get; set; }
	public Bound Region { get; set; }
	public string Text { get; set; }

	internal static int CompareRegionX(ITextRegion a, ITextRegion b) {
		if (a == b) {
			return 0;
		}

		return CompareRegionX(a, b, true);
	}

	private static int CompareRegionX(ITextRegion a, ITextRegion b, bool checkAlignment) {
		if (checkAlignment && a.Region.IsAlignedWith(b.Region, WritingDirection.Hortizontal) == false) {
			return CompareRegionY(a, b, false);
		}

		float x1 = a.Region.Center;
		float x2 = b.Region.Center;
		return x1 < x2 ? -1
			: x1 == x2 ? 0
			: 1;
	}

	internal static int CompareRegionY(ITextRegion a, ITextRegion b) {
		if (a == b) {
			return 0;
		}

		return CompareRegionY(a, b, true);
	}

	private static int CompareRegionY(ITextRegion a, ITextRegion b, bool checkAlignment) {
		if (checkAlignment && a.Region.IsAlignedWith(b.Region, WritingDirection.Vertical) == false) {
			return CompareRegionX(a, b, false);
		}

		float y1 = a.Region.Middle;
		float y2 = b.Region.Middle;
		return (y1 > y2) ^ a.Region.IsTopDown ? -1
			: y1 == y2 ? 0
			: 1;
	}
}