using System;
using System.Collections.Generic;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	sealed class TextFilter : AutoBookmarkFilter
	{
		MatchPattern.IMatcher _matcher;
		public TextFilter (MatchPattern pattern) {
			_matcher = pattern.CreateMatcher ();
		}
		internal override bool Matches (PDFPatcher.Model.AutoBookmarkContext context) {
			if (context.TextLine == null) {
				return _matcher.Matches (context.TextInfo.Text);
			}
			else {
				return _matcher.Matches (context.TextLine.Text);
			}
		}

		internal override void Reset () {
		}
	}
}
