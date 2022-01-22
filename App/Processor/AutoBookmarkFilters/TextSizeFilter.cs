using System.Diagnostics;
using System.Linq;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

[DebuggerDisplay("{MinSize}->{MaxSize}")]
public class TextSizeFilter : AutoBookmarkFilter
{
	public TextSizeFilter(float a, float b) {
		if (a > b) {
			MinSize = b;
			MaxSize = a;
		}
		else {
			MinSize = a;
			MaxSize = b;
		}
	}

	public float MinSize { get; }

	public float MaxSize { get; }

	internal override bool Matches(AutoBookmarkContext context) {
		if (context.TextLine == null) {
			float size = context.TextInfo.Size;
			return MatchSize(size);
		}

		return context.TextLine.Texts.Any(item => MatchSize(item.Size));
	}

	private bool MatchSize(float size) {
		return MinSize <= size && size <= MaxSize;
	}

	internal override void Reset() {
	}
}