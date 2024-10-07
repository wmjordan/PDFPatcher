using System;

namespace PDFPatcher.Processor
{
	[System.Diagnostics.DebuggerDisplay("{_rangeText}")]
	public class PageRangeFilter(string range) : AutoBookmarkFilter
	{
		private Model.PageRangeCollection _range;
		private readonly string _rangeText = range;

		internal override bool Matches(Model.AutoBookmarkContext context) {
			var p = context.CurrentPageNumber;
			_range ??= Model.PageRangeCollection.Parse(_rangeText, 1, context.TotalPageNumber, false);
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
