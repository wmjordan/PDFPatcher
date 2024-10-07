using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using PDFPatcher.Common;

namespace PDFPatcher.Model
{
	public sealed class MatchPattern : ICloneable, IXmlSerializable
	{
		[XmlAttribute("名称")]
		public string Name { get; set; }

		[XmlAttribute("匹配模板")]
		public string Text { get; set; }

		[XmlAttribute("匹配大小写")]
		public bool MatchCase { get; set; }

		[XmlAttribute("匹配全标题")]
		public bool FullMatch { get; set; }

		[XmlAttribute("使用正则表达式")]
		public bool UseRegularExpression { get; set; }

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

		public override string ToString() {
			return String.IsNullOrEmpty(Name)
				? $"{(UseRegularExpression ? "正则表达式" : "文本")}{(MatchCase ? "区分大小写" : String.Empty)}匹配 {Text}"
				: $"匹配{Name}";
		}

		#region ICloneable 成员

		public object Clone() {
			return new MatchPattern(Text, MatchCase, FullMatch, UseRegularExpression);
		}

		#endregion

		#region IXmlSerializable 成员

		public System.Xml.Schema.XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader) {
			if (reader.Read() == false || reader.Name != "pattern") {
				return;
			}
			Name = reader.GetAttribute("name");
			Text = reader.GetAttribute("text");
			MatchCase = reader.GetValue("matchCase", false);
			FullMatch = reader.GetValue("fullMatch", false);
			UseRegularExpression = reader.GetValue("useRegex", false);
		}

		public void WriteXml(XmlWriter writer) {
			writer.WriteStartElement("pattern");
			writer.WriteValue("name", Name, null);
			writer.WriteValue("text", Text, null);
			writer.WriteValue("matchCase", MatchCase, false);
			writer.WriteValue("fullMatch", FullMatch, false);
			writer.WriteValue("useRegex", UseRegularExpression, false);
			writer.WriteEndElement();
		}

		#endregion

		public interface IMatcher
		{
			bool Matches(string text);
			string Replace(string text, string replacement);
		}
		sealed class RegexMatcher(MatchPattern pattern) : IMatcher
		{
			readonly Regex _regex = new Regex(pattern.Text,
							 RegexOptions.Compiled | RegexOptions.CultureInvariant | (pattern.MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase));
			readonly bool _fullMatch = pattern.FullMatch;

			public bool Matches(string text) {
				var m = _regex.Match(text);
				return m.Success && (_fullMatch == false || text.Length == m.Length);
			}
			public string Replace(string text, string replacement) {
				return _regex.Replace(text, replacement);
			}
		}
		sealed class SimpleMatcher(MatchPattern pattern) : IMatcher
		{
			readonly bool _fullMatch = pattern.FullMatch;
			readonly string _text = pattern.Text;
			readonly StringComparison _comparison = pattern.MatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

			public bool Matches(string text) {
				if (String.IsNullOrEmpty(text)) {
					return true;
				}
				if (_fullMatch && text.Length != _text.Length) {
					return false;
				}
				var i = text.IndexOf(_text, _comparison);
				return i != -1 && (_fullMatch == false || i == 0);
			}
			public string Replace(string text, string replacement) {
				return Replace(text, _text, replacement, _comparison);
			}
			static string Replace(string original, string pattern, string replacement, StringComparison comparisonType) {
				return Replace(original, pattern, replacement, comparisonType, -1);
			}

			static string Replace(string original, string pattern, string replacement, StringComparison comparisonType, int stringBuilderInitialSize) {
				if (original == null) {
					return null;
				}

				if (String.IsNullOrEmpty(pattern)) {
					return original;
				}

				var posCurrent = 0;
				var lenPattern = pattern.Length;
				var idxNext = original.IndexOf(pattern, comparisonType);
				var result = StringBuilderCache.Acquire(stringBuilderInitialSize < 0 ? Math.Min(4096, original.Length) : stringBuilderInitialSize);

				while (idxNext >= 0) {
					result.Append(original, posCurrent, idxNext - posCurrent)
						.Append(replacement);

					posCurrent = idxNext + lenPattern;

					idxNext = original.IndexOf(pattern, posCurrent, comparisonType);
				}

				result.Append(original, posCurrent, original.Length - posCurrent);

				return StringBuilderCache.GetStringAndRelease(result);
			}
		}
	}
}
