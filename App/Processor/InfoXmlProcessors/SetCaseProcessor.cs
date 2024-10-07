using System;
using System.Collections.Generic;

namespace PDFPatcher.Processor
{
	sealed class SetCaseProcessor(SetCaseProcessor.LetterCase letterCase) : IPdfInfoXmlProcessor
	{
		static readonly System.Globalization.TextInfo __currentTextInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
		static readonly char[] FullWidthNumbers = "０１２３４５６７８９０".ToCharArray();
		static readonly char[] HalfWidthNumbers = "01234567890".ToCharArray();
		static readonly char[] ChineseNumbers = "○一二三四五六七八九〇".ToCharArray();
		static readonly char[] TraditionalChineseNumbers = "零壹贰叁肆伍陆柒捌玖零".ToCharArray();

		internal const string FullWidthPunctuations = "！＂＃＄％＆＇（）＊＋，－．／：；＜＝＞？＠［＼］＾＿｀｛｜｝～";
		internal const string HalfWidthPunctuations = "!\"#$%&'()*+,-./;:<=>?@[\\]^_`{|}~";

		internal static string[] CaseNames = new string[]{
			"首字母大写(&S)", "英文大写(&Y)", "英文小写(&X)",
			"全角数字(&Z)", "全角字母(&Q)", "全角标点(&B)",
			"半角数字(&N)", "半角字母(&M)", "半角标点(&D)",
			"中文数字(&W)", "大写中文数字(&H)",
			"繁体汉字转简体(&J)", "简体汉字转繁体(&F)"
		};

		public enum LetterCase
		{
			Title, Upper, Lower,
			FullWidthNumber, FullWidthAlphabetic, FullWidthPunctuation,
			HalfWidthNumber, HalfWidthAlphabetic, HalfWidthPunctuation,
			ChineseNumber, TraditionalChineseNumbers,
			TraditionalToSimplifiedCjk, SimplifiedToTraditionalCjk
		}

		public LetterCase Case { get; } = letterCase;

		#region IInfoDocProcessor 成员

		public string Name => "设置书签文本为" + CaseNames[(int)Case];

		public IUndoAction Process(System.Xml.XmlElement item) {
			var a = item.GetAttributeNode(Constants.BookmarkAttributes.Title);
			if (a == null) {
				return null;
			}
			var source = a.Value;
			var value = ConvertCase(source, Case);
			if (source == value) {
				return null;
			}
			return UndoAttributeAction.GetUndoAction(item, Constants.BookmarkAttributes.Title, value);
		}

		#endregion

		static string ConvertCase(string source, LetterCase targetCase) {
			string value;
			switch (targetCase) {
				case LetterCase.Lower:
					return source.ToLowerInvariant();
				case LetterCase.Upper:
					return source.ToUpperInvariant();
				case LetterCase.Title:
					return __currentTextInfo.ToTitleCase(source.ToLowerInvariant());
				case LetterCase.FullWidthAlphabetic:
					return HWL2FWL.Convert(source);
				case LetterCase.FullWidthNumber:
					return HWN2FWN.Convert(source);
				case LetterCase.FullWidthPunctuation:
					return HWP2FWP.Convert(source);
				case LetterCase.HalfWidthAlphabetic:
					return FWL2HWL.Convert(source);
				case LetterCase.HalfWidthNumber:
					return FWN2HWN.Convert(source);
				case LetterCase.HalfWidthPunctuation:
					return FWP2HWP.Convert(source);
				case LetterCase.ChineseNumber:
					value = Translate(source, FullWidthNumbers, ChineseNumbers);
					return Translate(value, HalfWidthNumbers, ChineseNumbers);
				case LetterCase.TraditionalChineseNumbers:
					value = Translate(source, FullWidthNumbers, TraditionalChineseNumbers);
					value = Translate(value, HalfWidthNumbers, TraditionalChineseNumbers);
					return Translate(value, ChineseNumbers, TraditionalChineseNumbers);
				case LetterCase.SimplifiedToTraditionalCjk:
					return SC2TC.Convert(source);
				case LetterCase.TraditionalToSimplifiedCjk:
					return TC2SC.Convert(source);
				default: throw new ArgumentOutOfRangeException("LetterCase");
			}
		}

		// todo 优化转换效率：避免 ToCharArray 和无条件创建新字符串
		static string Translate(string s, char[] source, char[] target) {
			var cs = s.ToCharArray();
			for (int i = 0; i < cs.Length; i++) {
				ref char c = ref cs[i];
				var p = Array.IndexOf(source, c);
				if (p != -1) {
					c = target[p];
				}
			}
			return new string(cs);
		}

		static char FullWidthLetterToHalfWidth(char ch) {
			return ch >= 'ａ' && ch <= 'ｚ' ? (char)(ch - 'ａ' + 'a')
				: ch >= 'Ａ' && ch <= 'Ｚ' ? (char)(ch - 'Ａ' + 'A')
				: ch;
		}

		static char HalfWidthLetterToFullWidth(char ch) {
			return ch >= 'a' && ch <= 'z' ? (char)(ch + 'ａ' - 'a')
				: ch >= 'A' && ch <= 'Z' ? (char)(ch + 'Ａ' - 'A')
				: ch;
		}
		static char FullWidthNumberToHalfWidth(char ch) {
			return ch >= '０' && ch <= '９' ? (char)(ch - '０' + '0') : ch;
		}

		static char HalfWidthNumberToFullWidth(char ch) {
			return ch >= '0' && ch <= '9' ? (char)(ch + '０' - '0') : ch;
		}

		static class SC2TC
		{
			public static readonly Converter<string, string> Convert = new StringMapper(new CharacterMapper(Constants.Chinese.Simplified, Constants.Chinese.Traditional).Map).Convert;
		}
		static class TC2SC
		{
			public static readonly Converter<string, string> Convert = new StringMapper(new CharacterMapper(Constants.Chinese.Traditional, Constants.Chinese.Simplified).Map).Convert;
		}
		static class FWP2HWP
		{
			public static readonly Converter<string, string> Convert = new StringMapper(new CharacterMapper(FullWidthPunctuations, HalfWidthPunctuations).Map).Convert;
		}
		static class HWP2FWP
		{
			public static readonly Converter<string, string> Convert = new StringMapper(new CharacterMapper(HalfWidthPunctuations, FullWidthPunctuations).Map).Convert;
		}
		static class FWL2HWL
		{
			public static readonly Converter<string, string> Convert = new StringMapper(FullWidthLetterToHalfWidth).Convert;
		}
		static class HWL2FWL
		{
			public static readonly Converter<string, string> Convert = new StringMapper(HalfWidthLetterToFullWidth).Convert;
		}
		static class FWN2HWN
		{
			public static readonly Converter<string, string> Convert = new StringMapper(FullWidthNumberToHalfWidth).Convert;
		}
		static class HWN2FWN
		{
			public static readonly Converter<string, string> Convert = new StringMapper(HalfWidthNumberToFullWidth).Convert;
		}

		sealed class StringMapper(Converter<char, char> converter)
		{
			readonly Converter<char, char> _converter = converter;

			public unsafe string Convert(string value) {
				if (String.IsNullOrEmpty(value)) {
					return value;
				}
				int i = 0;
				foreach (var ch in value) {
					if (ch != _converter(ch)) {
						break;
					}
					++i;
				}
				if (i == value.Length) {
					return value;
				}
				var r = String.Copy(value);
				fixed (char* s = r) {
					char* c = s + i;
					char* end = c + value.Length;
					do {
						*c = _converter(*c);
					} while (++c < end);
				}
				return r;
			}
		}

		sealed class CharacterMapper
		{
			readonly Dictionary<char, char> _Mapper;

			public CharacterMapper(string from, string to) {
				var i = 0;
				_Mapper = new Dictionary<char, char>(from.Length);
				foreach (var item in from) {
					_Mapper[item] = to[i++];
				}
			}

			public char Map(char value) {
				return _Mapper.TryGetValue(value, out var r) ? r : value;
			}
		}

		sealed class SequentialCharacterMapper(char from, char to, int count)
		{
			readonly char _From = from, _To = to;
			readonly int _Count = count;

			public char Map(char value) {
				var d = value - _From;
				return d >= 0 && d < _Count ? (char)(_To + d) : value;
			}
		}
	}
}
