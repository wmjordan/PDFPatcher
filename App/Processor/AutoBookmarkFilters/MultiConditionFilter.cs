using System.Collections.Generic;
using System.Linq;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

internal sealed class MultiConditionFilter : AutoBookmarkFilter
{
	private readonly List<AutoBookmarkFilter> _filters = new();

	public MultiConditionFilter(AutoBookmarkCondition.MultiCondition condition) {
		foreach (AutoBookmarkCondition item in condition.Conditions) {
			_filters.Add(item.CreateFilter());
		}
	}

	internal override bool Matches(AutoBookmarkContext context) {
		return _filters.All(item => item.Matches(context));
	}

	internal override void Reset() {
		foreach (AutoBookmarkFilter item in _filters) {
			item.Reset();
		}
	}
}