using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Processor
{
	[System.Diagnostics.DebuggerDisplay("P {Position}: {MinValue}->{MaxValue}")]
	sealed class TextPositionFilter : AutoBookmarkFilter
	{
		public byte Position { get; private set; }
		public float MaxValue { get; private set; }
		public float MinValue { get; private set; }

		public TextPositionFilter(byte position, float min, float max) {
			Position = position;
			MaxValue = max;
			MinValue = min;
		}

		internal override bool Matches(PDFPatcher.Model.AutoBookmarkContext context) {
			if (context.TextLine == null) {
				return MatchPosition(context.TextInfo.Region);
			}
			else {
				foreach (var item in context.TextLine.Texts) {
					if (MatchPosition(item.Region)) {
						return true;
					}
				}
				return false;
			}
		}

		private bool MatchPosition(PDFPatcher.Model.Bound bound) {
			switch (Position) {
				case 1: return bound.Top > MinValue && bound.Top < MaxValue;
				case 2: return bound.Bottom > MinValue && bound.Bottom < MaxValue;
				case 3: return bound.Left > MinValue && bound.Left < MaxValue;
				case 4: return bound.Right > MinValue && bound.Right < MaxValue;
				default:
					return false;
			}
		}

		internal override void Reset() {
		}
	}
}
