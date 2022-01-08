namespace PDFPatcher.Processor
{
	[System.Diagnostics.DebuggerDisplay("{MinSize}->{MaxSize}")]
	public class TextSizeFilter : AutoBookmarkFilter
	{
		readonly float _minSize, _maxSize;
		public float MinSize => _minSize;
		public float MaxSize => _maxSize;

		public TextSizeFilter(float a, float b) {
			if (a > b) {
				_minSize = b;
				_maxSize = a;
			}
			else {
				_minSize = a;
				_maxSize = b;
			}
		}

		internal override bool Matches(PDFPatcher.Model.AutoBookmarkContext context) {
			if (context.TextLine == null) {
				var size = context.TextInfo.Size;
				return MatchSize(size);
			}
			else {
				foreach (var item in context.TextLine.Texts) {
					if (MatchSize(item.Size)) {
						return true;
					}
				}
				return false;
			}
		}

		private bool MatchSize(float size) {
			return _minSize <= size && size <= _maxSize;
		}

		internal override void Reset() {
		}

	}
}
