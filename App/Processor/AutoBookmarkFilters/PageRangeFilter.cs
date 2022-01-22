using System.Diagnostics;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

[DebuggerDisplay("{_rangeText}")]
public class PageRangeFilter : AutoBookmarkFilter
{
	private readonly string _rangeText;
	private PageRangeCollection _range;

	public PageRangeFilter(string range) {
		_rangeText = range;
		_range = null;
	}

	internal override bool Matches(AutoBookmarkContext context) {
		int p = context.CurrentPageNumber;
		_range ??= PageRangeCollection.Parse(_rangeText, 1, context.TotalPageNumber, false);

		foreach (PageRange item in _range) {
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