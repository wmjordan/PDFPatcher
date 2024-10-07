using System;

namespace PDFPatcher.Processor
{
	[System.Diagnostics.DebuggerDisplay("{MinSize}->{MaxSize}")]
	public class TextSizeFilter : AutoBookmarkFilter
	{
		public float MinSize { get; }
		public float MaxSize { get; }

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

		internal override bool Matches(Model.AutoBookmarkContext context) {
			if (context.TextLine == null) {
				var size = context.TextInfo.Size;
				return MatchSize(size);
			}
			foreach (var item in context.TextLine.Texts) {
				if (MatchSize(item.Size)) {
					return true;
				}
			}
			return false;
		}

		bool MatchSize(float size) {
			return MinSize <= size && size <= MaxSize;
		}

		internal override void Reset() {
		}
	}
}
