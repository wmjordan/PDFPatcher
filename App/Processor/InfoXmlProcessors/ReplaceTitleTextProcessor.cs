

namespace PDFPatcher.Processor
{
	sealed class ReplaceTitleTextProcessor : IPdfInfoXmlProcessor
	{
		static readonly BookmarkMatcher.SimpleReplacer __replacer = new BookmarkMatcher.SimpleReplacer();

		readonly BookmarkMatcher _matcher;
		readonly string _replacement;
		public ReplaceTitleTextProcessor(string replacement) {
			_matcher = __replacer;
			_replacement = replacement;
		}
		public ReplaceTitleTextProcessor(BookmarkMatcher matcher, string replacement) {
			if (matcher == null) {
				throw new System.ArgumentNullException("matcher");
			}
			_matcher = matcher;
			_replacement = replacement;
		}

		#region IInfoDocProcessor 成员

		public string Name => string.Concat("替换文本为“", _replacement, "”");

		public IUndoAction Process(System.Xml.XmlElement item) {
			var a = item.GetAttributeNode(Constants.BookmarkAttributes.Title);
			if (a == null) {
				return null;
			}
			return _matcher.Replace(item, _replacement);
		}

		#endregion
	}
}
