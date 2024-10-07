using System;
using System.Collections.Generic;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	[System.Diagnostics.DebuggerDisplay("FontName = {FontName}; MatchFullName = {MatchFullName}")]
	public class FontNameFilter(string fontName, bool matchFullName) : AutoBookmarkFilter
	{
		public string FontName { get; } = fontName;
		public bool MatchFullName { get; } = matchFullName;

		readonly Dictionary<int, bool> _matchResultCache = [];

		internal override bool Matches(AutoBookmarkContext context) {
			if (context.TextLine == null) {
				return MatchFont(context.TextInfo.Font);
			}
			foreach (var item in context.TextLine.Texts) {
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
			return _matchResultCache[font.FontID] = MatchFullName
				? String.Equals(FontName, font.FontName, StringComparison.OrdinalIgnoreCase)
				: font.PostscriptFontName.IndexOf(FontName, StringComparison.OrdinalIgnoreCase) > -1;
		}

		internal override void Reset() {
			_matchResultCache.Clear();
		}

	}
}
