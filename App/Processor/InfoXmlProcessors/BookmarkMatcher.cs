using System.Xml;

namespace PDFPatcher.Processor
{
	internal abstract class BookmarkMatcher
	{
		internal enum MatcherType
		{
			Normal, Regex, XPath
		}

		internal abstract bool Match (XmlElement item);
		internal abstract IUndoAction Replace (XmlElement item, string replacement);
		internal static BookmarkMatcher Create (string pattern, MatcherType type, bool matchCase, bool fullMatch) {
			if (type == MatcherType.XPath) {
				return new XPathMatcher (pattern);
			}
			else {
				return new RegexMatcher (pattern, matchCase, type == MatcherType.Regex, fullMatch);
			}
		}
		sealed class RegexMatcher : BookmarkMatcher
		{
			readonly Model.MatchPattern.IMatcher _matcher;
			internal RegexMatcher (string pattern, bool matchCase, bool regexSearch, bool fullMatch) {
				_matcher = new Model.MatchPattern (pattern, matchCase, fullMatch, regexSearch).CreateMatcher ();
			}
			internal override bool Match (XmlElement item) {
				var t = item.GetAttribute (Constants.BookmarkAttributes.Title);
				return _matcher.Matches (t);
			}
			internal override IUndoAction Replace (XmlElement item, string replacement) {
				var a = item.GetAttributeNode (Constants.BookmarkAttributes.Title);
				if (a == null) {
					return null;
				}
				var t = a.Value;
				//if (_regexSearch) {
				if (_matcher.Matches (t)) {
					var r = _matcher.Replace (t, replacement);
					if (r == t) {
						return null;
					}
					return UndoAttributeAction.GetUndoAction (item, Constants.BookmarkAttributes.Title, r);
				}
				//}
				//else if (t != replacement) {
				//    var undo = UndoAttributeAction.GetUndoAction (item, Constants.BookmarkAttributes.Title);
				//    item.SetAttribute (Constants.BookmarkAttributes.Title, replacement);
				//    return undo;
				//}
				return null;
			}
		}
		sealed class XPathMatcher : BookmarkMatcher
		{
			readonly System.Xml.XPath.XPathExpression _xpath;
			internal XPathMatcher (string pattern) {
				_xpath = System.Xml.XPath.XPathExpression.Compile ("*[" + pattern + "]");
			}
			internal override bool Match (XmlElement item) {
				return item.CreateNavigator ().Matches (_xpath);
			}
			internal override IUndoAction Replace (XmlElement item, string replacement) {
				var a = item.GetAttributeNode (Constants.BookmarkAttributes.Title);
				if (a == null) {
					return null;
				}
				var n = item.CreateNavigator ().SelectSingleNode (_xpath);
				if (n == null || a.Value == replacement) {
					return null;
				}
				return UndoAttributeAction.GetUndoAction (item, Constants.BookmarkAttributes.Title, replacement);
			}
		}
		internal sealed class SimpleReplacer : BookmarkMatcher
		{
			internal override bool Match (XmlElement item) {
				return true;
			}
			internal override IUndoAction Replace (XmlElement item, string replacement) {
				var a = item.GetAttributeNode (Constants.BookmarkAttributes.Title);
				if (a == null) {
					return null;
				}
				if (a.Value != replacement) {
					return UndoAttributeAction.GetUndoAction (item, Constants.BookmarkAttributes.Title, replacement);
				}
				return null;
			}
		}
	}
}
