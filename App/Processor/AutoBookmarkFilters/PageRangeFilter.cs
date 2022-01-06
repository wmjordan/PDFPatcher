using System;

namespace PDFPatcher.Processor
{
	[System.Diagnostics.DebuggerDisplay("{_rangeText}")]
	public class PageRangeFilter : AutoBookmarkFilter
	{
		private Model.PageRangeCollection _range;
		private readonly string _rangeText;

		public PageRangeFilter(string range) {
			_rangeText = range;
			_range = null;
		}

		internal override bool Matches(PDFPatcher.Model.AutoBookmarkContext context) {
			var p = context.CurrentPageNumber;
			if (_range == null) {
				_range = Model.PageRangeCollection.Parse(_rangeText, 1, context.TotalPageNumber, false);
			}
			foreach (var item in _range) {
				if (p <= item.EndValue && p >= item.StartValue) {
					return true;
				}
			}
			return false;
		}

		internal override void Reset() {
			_range = null;
		}

	}
}
