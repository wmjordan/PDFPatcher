using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using PowerJson;

namespace PDFPatcher.Model;

public sealed class MatchPattern : ICloneable
{
	[XmlAttribute("名称")] public string Name { get; set; }

	[XmlAttribute("匹配模板")] public string Text { get; set; }

	[XmlAttribute("匹配大小写")] public bool MatchCase { get; set; }

	[XmlAttribute("匹配全标题")] public bool FullMatch { get; set; }

	[XmlAttribute("使用正则表达式")] public bool UseRegularExpression { get; set; }

	public MatchPattern() {
	}

	public MatchPattern(string text, bool matchCase, bool fullMatch, bool useRegExp) {
		Text = text;
		MatchCase = matchCase;
		FullMatch = fullMatch;
		UseRegularExpression = useRegExp;
	}

	public IMatcher CreateMatcher() {
		if (UseRegularExpression) {
			return new RegexMatcher(this);
		}

		return new SimpleMatcher(this);
	}

	#region ICloneable 成员

	public object Clone() {
		return new MatchPattern(Text, MatchCase, FullMatch, UseRegularExpression);
	}

	#endregion

	public interface IMatcher
	{
		bool Matches(string text);
		string Replace(string text, string replacement);
	}

	private sealed class RegexMatcher : IMatcher
	{
		private readonly Regex _regex;
		private readonly bool _fullMatch;

		public RegexMatcher(MatchPattern pattern) {
			_regex = new Regex(pattern.Text,
				RegexOptions.Compiled | (pattern.MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase));
			_fullMatch = pattern.FullMatch;
		}

		public bool Matches(string text) {
			Match m = _regex.Match(text);
			return m.Success && (_fullMatch == false || text.Length == m.Length);
		}

		public string Replace(string text, string replacement) {
			return _regex.Replace(text, replacement);
		}
	}

	private sealed class SimpleMatcher : IMatcher
	{
		private readonly bool _fullMatch;
		private readonly string _text;
		private readonly StringComparison _comparison;

		public SimpleMatcher(MatchPattern pattern) {
			_text = pattern.Text;
			_fullMatch = pattern.FullMatch;
			_comparison = pattern.MatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
		}

		public bool Matches(string text) {
			if (string.IsNullOrEmpty(text)) {
				return true;
			}

			if (_fullMatch && text.Length != _text.Length) {
				return false;
			}

			int i = text.IndexOf(_text, _comparison);
			return i != -1 && (_fullMatch == false || i == 0);
		}

		public string Replace(string text, string replacement) {
			return Replace(text, _text, replacement, _comparison);
		}

		private static string Replace(string original, string pattern, string replacement,
			StringComparison comparisonType) {
			return Replace(original, pattern, replacement, comparisonType, -1);
		}

		private static string Replace(string original, string pattern, string replacement,
			StringComparison comparisonType,
			int stringBuilderInitialSize) {
			if (original == null) {
				return null;
			}

			if (string.IsNullOrEmpty(pattern)) {
				return original;
			}

			int posCurrent = 0;
			int lenPattern = pattern.Length;
			int idxNext = original.IndexOf(pattern, comparisonType);
			StringBuilder result = new(stringBuilderInitialSize < 0
				? Math.Min(4096, original.Length)
				: stringBuilderInitialSize);

			while (idxNext >= 0) {
				result.Append(original, posCurrent, idxNext - posCurrent);
				result.Append(replacement);

				posCurrent = idxNext + lenPattern;

				idxNext = original.IndexOf(pattern, posCurrent, comparisonType);
			}

			result.Append(original, posCurrent, original.Length - posCurrent);

			return result.ToString();
		}
	}
}