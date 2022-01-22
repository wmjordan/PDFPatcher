using System.Diagnostics;
using System.Linq;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

[DebuggerDisplay("P {Position}: {MinValue}->{MaxValue}")]
internal sealed class TextPositionFilter : AutoBookmarkFilter
{
	public TextPositionFilter(byte position, float min, float max) {
		Position = position;
		MaxValue = max;
		MinValue = min;
	}

	public byte Position { get; private set; }
	public float MaxValue { get; private set; }
	public float MinValue { get; private set; }

	internal override bool Matches(AutoBookmarkContext context) {
		return context.TextLine == null ? MatchPosition(context.TextInfo.Region) : context.TextLine.Texts.Any(item => MatchPosition(item.Region));
	}

	private bool MatchPosition(Bound bound) {
		return Position switch {
			1 => bound.Top > MinValue && bound.Top < MaxValue,
			2 => bound.Bottom > MinValue && bound.Bottom < MaxValue,
			3 => bound.Left > MinValue && bound.Left < MaxValue,
			4 => bound.Right > MinValue && bound.Right < MaxValue,
			_ => false
		};
	}

	internal override void Reset() {
	}
}