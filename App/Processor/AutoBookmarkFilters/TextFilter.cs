using System;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	sealed class TextFilter(MatchPattern pattern) : AutoBookmarkFilter
	{
		readonly MatchPattern.IMatcher _matcher = pattern.CreateMatcher();

		internal override bool Matches(AutoBookmarkContext context) {
			return  _matcher.Matches(context.TextLine == null ? context.TextInfo.Text : context.TextLine.Text);
		}

		internal override void Reset() {
		}
	}
}
