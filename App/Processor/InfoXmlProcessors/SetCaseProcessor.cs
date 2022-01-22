using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace PDFPatcher.Processor;

internal sealed class SetCaseProcessor : IPdfInfoXmlProcessor
{
	public enum LetterCase
	{
		Title, Upper, Lower,
		FullWidthNumber, FullWidthAlphabetic, FullWidthPunctuation,
		HalfWidthNumber, HalfWidthAlphabetic, HalfWidthPunctuation,
		ChineseNumber, TraditionalChineseNumbers,
		TraditionalToSimplifiedCjk, SimplifiedToTraditionalCjk
	}

	internal const string FullWidthPunctuations = "！＂＃＄％＆＇（）＊＋，－．／：；＜＝＞？＠［＼］＾＿｀｛｜｝～";
	internal const string HalfWidthPunctuations = "!\"#$%&'()*+,-./;:<=>?@[\\]^_`{|}~";
	private static readonly TextInfo __currentTextInfo = CultureInfo.CurrentCulture.TextInfo;
	private static readonly char[] FullWidthNumbers = "０１２３４５６７８９０".ToCharArray();
	private static readonly char[] HalfWidthNumbers = "01234567890".ToCharArray();
	private static readonly char[] ChineseNumbers = "○一二三四五六七八九〇".ToCharArray();
	private static readonly char[] TraditionalChineseNumbers = "零壹贰叁肆伍陆柒捌玖零".ToCharArray();

	internal static string[] CaseNames = {
		"首字母大写", "英文大写", "英文小写", "全角数字", "全角字母", "全角标点", "半角数字", "半角字母", "半角标点", "中文数字", "大写中文数字", "繁体汉字转简体", "简体汉字转繁体"
	};

	public SetCaseProcessor(LetterCase letterCase) {
		Case = letterCase;
	}

	public LetterCase Case { get; }

	private static string ConvertCase(string source, LetterCase targetCase) {
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

	private static string Translate(string s, char[] source, IList<char> target) {
		char[] cs = s.ToCharArray();
		for (int i = 0; i < cs.Length; i++) {
			int p = Array.IndexOf(source, cs[i]);
			if (p != -1) {
				cs[i] = target[p];
			}
		}

		return new string(cs);
	}

	private static char FullWidthLetterToHalfWidth(char ch) {
		return ch switch {
			>= 'ａ' and <= 'ｚ' => (char)(ch - 'ａ' + 'a'),
			>= 'Ａ' and <= 'Ｚ' => (char)(ch - 'Ａ' + 'A'),
			_ => ch
		};
	}

	private static char HalfWidthLetterToFullWidth(char ch) {
		return ch switch {
			>= 'a' and <= 'z' => (char)(ch + 'ａ' - 'a'),
			>= 'A' and <= 'Z' => (char)(ch + 'Ａ' - 'A'),
			_ => ch
		};
	}

	private static char FullWidthNumberToHalfWidth(char ch) {
		return ch is >= '０' and <= '９' ? (char)(ch - '０' + '0') : ch;
	}

	private static char HalfWidthNumberToFullWidth(char ch) {
		return ch is >= '0' and <= '9' ? (char)(ch + '０' - '0') : ch;
	}

	private static class SC2TC
	{
		public static readonly Converter<string, string> Convert =
			new StringMapper(new CharacterMapper(ChineseCharMap.Simplified, ChineseCharMap.Traditional).Map).Convert;
	}

	private static class TC2SC
	{
		public static readonly Converter<string, string> Convert =
			new StringMapper(new CharacterMapper(ChineseCharMap.Traditional, ChineseCharMap.Simplified).Map).Convert;
	}

	private static class FWP2HWP
	{
		public static readonly Converter<string, string> Convert =
			new StringMapper(new CharacterMapper(FullWidthPunctuations, HalfWidthPunctuations).Map).Convert;
	}

	private static class HWP2FWP
	{
		public static readonly Converter<string, string> Convert =
			new StringMapper(new CharacterMapper(HalfWidthPunctuations, FullWidthPunctuations).Map).Convert;
	}

	private static class FWL2HWL
	{
		public static readonly Converter<string, string> Convert = new StringMapper(FullWidthLetterToHalfWidth).Convert;
	}

	private static class HWL2FWL
	{
		public static readonly Converter<string, string> Convert = new StringMapper(HalfWidthLetterToFullWidth).Convert;
	}

	private static class FWN2HWN
	{
		public static readonly Converter<string, string> Convert = new StringMapper(FullWidthNumberToHalfWidth).Convert;
	}

	private static class HWN2FWN
	{
		public static readonly Converter<string, string> Convert = new StringMapper(HalfWidthNumberToFullWidth).Convert;
	}

	private sealed class StringMapper
	{
		private readonly Converter<char, char> Converter;

		public StringMapper(Converter<char, char> converter) {
			Converter = converter;
		}

		public unsafe string Convert(string value) {
			if (string.IsNullOrEmpty(value)) {
				return value;
			}

			int i = value.TakeWhile(ch => ch == Converter(ch)).Count();

			if (i == value.Length) {
				return value;
			}

			string r = string.Copy(value);
			fixed (char* s = r) {
				char* c = s + i;
				char* end = c + value.Length;
				do {
					*c = Converter(*c);
				} while (++c < end);
			}

			return r;
		}
	}

	private sealed class CharacterMapper
	{
		private readonly Dictionary<char, char> _Mapper;

		public CharacterMapper(string from, string to) {
			int i = 0;
			_Mapper = new Dictionary<char, char>(from.Length);
			foreach (char item in from) {
				_Mapper[item] = to[i++];
			}
		}

		public char Map(char value) {
			return _Mapper.TryGetValue(value, out char r) ? r : value;
		}
	}

	private sealed class SequentialCharacterMapper
	{
		private readonly int _Count;
		private readonly char _From, _To;

		public SequentialCharacterMapper(char from, char to, int count) {
			_From = from;
			_To = to;
			_Count = count;
		}

		public char Map(char value) {
			int d = value - _From;
			return d >= 0 && d < _Count ? (char)(_To + d) : value;
		}
	}

	#region IInfoDocProcessor 成员

	public string Name => "设置书签文本为" + CaseNames[(int)Case];

	public IUndoAction Process(XmlElement item) {
		XmlAttribute a = item.GetAttributeNode(Constants.BookmarkAttributes.Title);
		if (a == null) {
			return null;
		}

		string source = a.Value;
		string value = ConvertCase(source, Case);
		return source == value ? null : UndoAttributeAction.GetUndoAction(item, Constants.BookmarkAttributes.Title, value);
	}

	#endregion
}