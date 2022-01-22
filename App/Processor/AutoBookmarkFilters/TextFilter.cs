using PDFPatcher.Model;

namespace PDFPatcher.Processor;

internal sealed class TextFilter : AutoBookmarkFilter
{
	private readonly MatchPattern.IMatcher _matcher;

	public TextFilter(MatchPattern pattern) {
		_matcher = pattern.CreateMatcher();
	}

	internal override bool Matches(AutoBookmarkContext context) {
		return _matcher.Matches(context.TextLine == null ? context.TextInfo.Text : context.TextLine.Text);
	}

	internal override void Reset() {
	}
}