using System;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	[System.Diagnostics.DebuggerDisplay("P {Position}: {MinValue}->{MaxValue}")]
	sealed class TextPositionFilter(byte position, float min, float max) : AutoBookmarkFilter
	{
		public byte Position { get; } = position;
		public float MaxValue { get; } = max;
		public float MinValue { get; } = min;

		internal override bool Matches(AutoBookmarkContext context) {
			if (context.TextLine == null) {
				return MatchPosition(context.TextInfo.Region);
			}
			foreach (var item in context.TextLine.Texts) {
				if (MatchPosition(item.Region)) {
					return true;
				}
			}
			return false;
		}

		bool MatchPosition(Bound bound) {
			return Position switch {
				1 => bound.Top > MinValue && bound.Top < MaxValue,
				2 => bound.Bottom > MinValue && bound.Bottom < MaxValue,
				3 => bound.Left > MinValue && bound.Left < MaxValue,
				4 => bound.Right > MinValue && bound.Right < MaxValue,
				_ => false,
			};
		}

		internal override void Reset() {
		}
	}
}
