using System;
using System.Collections.Generic;
using System.Text;
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
		foreach (AutoBookmarkFilter item in _filters) {
			if (item.Matches(context) == false) {
				return false;
			}
		}

		return true;
	}

	internal override void Reset() {
		foreach (AutoBookmarkFilter item in _filters) {
			item.Reset();
		}
	}
}