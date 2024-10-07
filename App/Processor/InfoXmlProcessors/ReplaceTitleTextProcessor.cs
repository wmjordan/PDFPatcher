namespace PDFPatcher.Processor
{
	sealed class ReplaceTitleTextProcessor : IPdfInfoXmlProcessor
	{
		static readonly BookmarkMatcher.SimpleReplacer __replacer = new();

		readonly BookmarkMatcher _matcher;
		readonly string _replacement;

		public ReplaceTitleTextProcessor(string replacement) {
			_matcher = __replacer;
			_replacement = replacement;
		}
		public ReplaceTitleTextProcessor(BookmarkMatcher matcher, string replacement) {
			_matcher = matcher ?? throw new System.ArgumentNullException(nameof(matcher));
			_replacement = replacement;
		}

		#region IInfoDocProcessor 成员

		public string Name => $"替换文本为“{_replacement}”";

		public IUndoAction Process(System.Xml.XmlElement item) {
			return item.HasAttribute(Constants.BookmarkAttributes.Title)
				? _matcher.Replace(item, _replacement)
				: null;
		}

		#endregion
	}
}
