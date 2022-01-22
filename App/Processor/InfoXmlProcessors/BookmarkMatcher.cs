using System.Xml;
using System.Xml.XPath;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

internal abstract class BookmarkMatcher
{
	internal abstract bool Match(XmlElement item);
	internal abstract IUndoAction Replace(XmlElement item, string replacement);

	internal static BookmarkMatcher Create(string pattern, MatcherType type, bool matchCase, bool fullMatch) {
		if (type == MatcherType.XPath) {
			return new XPathMatcher(pattern);
		}

		return new RegexMatcher(pattern, matchCase, type == MatcherType.Regex, fullMatch);
	}

	internal enum MatcherType
	{
		Normal, Regex, XPath
	}

	private sealed class RegexMatcher : BookmarkMatcher
	{
		private readonly MatchPattern.IMatcher _matcher;

		internal RegexMatcher(string pattern, bool matchCase, bool regexSearch, bool fullMatch) {
			_matcher = new MatchPattern(pattern, matchCase, fullMatch, regexSearch).CreateMatcher();
		}

		internal override bool Match(XmlElement item) {
			string t = item.GetAttribute(Constants.BookmarkAttributes.Title);
			return _matcher.Matches(t);
		}

		internal override IUndoAction Replace(XmlElement item, string replacement) {
			XmlAttribute a = item.GetAttributeNode(Constants.BookmarkAttributes.Title);
			if (a == null) {
				return null;
			}

			string t = a.Value;
			//if (_regexSearch) {
			if (!_matcher.Matches(t)) {
				return null;
			}

			string r = _matcher.Replace(t, replacement);
			if (r == t) {
				return null;
			}

			return UndoAttributeAction.GetUndoAction(item, Constants.BookmarkAttributes.Title, r);

			//}
			//else if (t != replacement) {
			//    var undo = UndoAttributeAction.GetUndoAction (item, Constants.BookmarkAttributes.Title);
			//    item.SetAttribute (Constants.BookmarkAttributes.Title, replacement);
			//    return undo;
			//}
		}
	}

	private sealed class XPathMatcher : BookmarkMatcher
	{
		private readonly XPathExpression _xpath;

		internal XPathMatcher(string pattern) {
			_xpath = XPathExpression.Compile("*[" + pattern + "]");
		}

		internal override bool Match(XmlElement item) {
			return item.CreateNavigator().Matches(_xpath);
		}

		internal override IUndoAction Replace(XmlElement item, string replacement) {
			XmlAttribute a = item.GetAttributeNode(Constants.BookmarkAttributes.Title);
			if (a == null) {
				return null;
			}

			XPathNavigator n = item.CreateNavigator().SelectSingleNode(_xpath);
			if (n == null || a.Value == replacement) {
				return null;
			}

			return UndoAttributeAction.GetUndoAction(item, Constants.BookmarkAttributes.Title, replacement);
		}
	}

	internal sealed class SimpleReplacer : BookmarkMatcher
	{
		internal override bool Match(XmlElement item) {
			return true;
		}

		internal override IUndoAction Replace(XmlElement item, string replacement) {
			XmlAttribute a = item.GetAttributeNode(Constants.BookmarkAttributes.Title);
			if (a == null) {
				return null;
			}

			if (a.Value != replacement) {
				return UndoAttributeAction.GetUndoAction(item, Constants.BookmarkAttributes.Title, replacement);
			}

			return null;
		}
	}
}