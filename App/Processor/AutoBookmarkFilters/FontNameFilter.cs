using System;
using System.Collections.Generic;

namespace PDFPatcher.Processor
{
	[System.Diagnostics.DebuggerDisplay("FontName = {FontName}; MatchFullName = {MatchFullName}")]
	public class FontNameFilter : AutoBookmarkFilter
	{
		public string FontName { get; set; }
		public bool MatchFullName { get; set; }

		readonly Dictionary<int, bool> _matchResultCache;

		public FontNameFilter() {
			_matchResultCache = new Dictionary<int, bool>();
		}

		public FontNameFilter(string fontName, bool matchFullName) : this() {
			FontName = fontName;
			MatchFullName = matchFullName;
		}

		internal override bool Matches(PDFPatcher.Model.AutoBookmarkContext context) {
			if (context.TextLine == null) {
				var font = context.TextInfo.Font;
				return MatchFont(font);
			}
			else {
				foreach (var item in context.TextLine.Texts) {
					if (MatchFont(item.Font)) {
						return true;
					}
				}
				return false;
			}
		}

		private bool MatchFont(PDFPatcher.Model.FontInfo font) {
			if (font == null) {
				return true;
			}
			bool result;
			if (_matchResultCache.TryGetValue(font.FontID, out result)) {
				return result;
			}

			if (MatchFullName) {
				//_matchResultCache[font.FontID] = String.Compare (this.FontName, font.PostscriptFontName, StringComparison.OrdinalIgnoreCase) == 0;
				_matchResultCache[font.FontID] = String.Compare(FontName, font.FontName, StringComparison.OrdinalIgnoreCase) == 0;
			}
			else {
				_matchResultCache[font.FontID] = font.PostscriptFontName.IndexOf(FontName, StringComparison.OrdinalIgnoreCase) > -1;
			}
			return _matchResultCache[font.FontID];
		}

		internal override void Reset() {
			_matchResultCache.Clear();
		}

	}
}
