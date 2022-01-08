using System.Collections.Generic;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	internal sealed class MultiConditionFilter : AutoBookmarkFilter
	{
		readonly List<AutoBookmarkFilter> _filters = new List<AutoBookmarkFilter>();

		public MultiConditionFilter(AutoBookmarkCondition.MultiCondition condition) {
			foreach (var item in condition.Conditions) {
				_filters.Add(item.CreateFilter());
			}
		}

		internal override bool Matches(AutoBookmarkContext context) {
			foreach (var item in _filters) {
				if (item.Matches(context) == false) {
					return false;
				}
			}
			return true;
		}

		internal override void Reset() {
			foreach (var item in _filters) {
				item.Reset();
			}
		}
	}
}
