using System;
using System.Collections.Generic;
using System.Diagnostics;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

[DebuggerDisplay("FontName = {FontName}; MatchFullName = {MatchFullName}")]
public class FontNameFilter : AutoBookmarkFilter
{
	private readonly Dictionary<int, bool> _matchResultCache;

	public FontNameFilter() {
		_matchResultCache = new Dictionary<int, bool>();
	}

	public FontNameFilter(string fontName, bool matchFullName) : this() {
		FontName = fontName;
		MatchFullName = matchFullName;
	}

	public string FontName { get; set; }
	public bool MatchFullName { get; set; }

	internal override bool Matches(AutoBookmarkContext context) {
		if (context.TextLine == null) {
			FontInfo font = context.TextInfo.Font;
			return MatchFont(font);
		}

		foreach (TextInfo item in context.TextLine.Texts) {
			if (MatchFont(item.Font)) {
				return true;
			}
		}

		return false;
	}

	private bool MatchFont(FontInfo font) {
		if (font == null) {
			return true;
		}

		bool result;
		if (_matchResultCache.TryGetValue(font.FontID, out result)) {
			return result;
		}

		if (MatchFullName) {
			//_matchResultCache[font.FontID] = String.Compare (this.FontName, font.PostscriptFontName, StringComparison.OrdinalIgnoreCase) == 0;
			_matchResultCache[font.FontID] =
				string.Compare(FontName, font.FontName, StringComparison.OrdinalIgnoreCase) == 0;
		}
		else {
			_matchResultCache[font.FontID] =
				font.PostscriptFontName.IndexOf(FontName, StringComparison.OrdinalIgnoreCase) > -1;
		}

		return _matchResultCache[font.FontID];
	}

	internal override void Reset() {
		_matchResultCache.Clear();
	}
}