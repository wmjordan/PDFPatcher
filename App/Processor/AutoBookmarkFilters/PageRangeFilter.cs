using System.Diagnostics;
using System.Linq;
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

		return _range.Any(item => p <= item.EndValue && p >= item.StartValue);
	}

	internal override void Reset() {
		_range = null;
	}
}