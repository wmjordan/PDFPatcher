using System;
using System.Collections.Generic;

namespace PDFPatcher.Processor
{
	sealed class SetCaseProcessor : IPdfInfoXmlProcessor
	{
		static readonly System.Globalization.TextInfo __currentTextInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
		static readonly char[] FullWidthNumbers = "０１２３４５６７８９０".ToCharArray();
		static readonly char[] HalfWidthNumbers = "01234567890".ToCharArray();
		static readonly char[] ChineseNumbers = "○一二三四五六七八九〇".ToCharArray();
		static readonly char[] TraditionalChineseNumbers = "零壹贰叁肆伍陆柒捌玖零".ToCharArray();

		static readonly char[] FullWidthAlphabetics = "ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ".ToCharArray();
		static readonly char[] HalfWidthAlphabetics = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
		const string FullWidthPunctuations = "！＂＃＄％＆＇（）＊＋，－．／：；＜＝＞？＠［＼］＾＿｀｛｜｝～";
		const string HalfWidthPunctuations = "!\"#$%&'()*+,-./;:<=>?@[\\]^_`{|}~";

		internal static string[] CaseNames = new string[]{
			"首字母大写", "英文大写", "英文小写",
			"全角数字", "全角字母", "全角标点",
			"半角数字", "半角字母", "半角标点",
			"中文数字", "大写中文数字",
			"繁体汉字转简体", "简体汉字转繁体"
		};

		public enum LetterCase
		{
			Title, Upper, Lower,
			FullWidthNumber, FullWidthAlphabetic, FullWidthPunctuation,
			HalfWidthNumber, HalfWidthAlphabetic, HalfWidthPunctuation,
			ChineseNumber, TraditionalChineseNumbers,
			TraditionalToSimplifiedCjk, SimplifiedToTraditionalCjk
		}

		public LetterCase Case { get; }

		public SetCaseProcessor(LetterCase letterCase) {
			Case = letterCase;
		}

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
					return Translate(source, HalfWidthAlphabetics, FullWidthAlphabetics);
				case LetterCase.FullWidthNumber:
					value = Translate(source, HalfWidthNumbers, FullWidthNumbers);
					return Translate(value, ChineseNumbers, FullWidthNumbers);
				case LetterCase.FullWidthPunctuation:
					return HWP2FWP.Convert(source);
				case LetterCase.HalfWidthAlphabetic:
					return Translate(source, FullWidthAlphabetics, HalfWidthAlphabetics);
				case LetterCase.HalfWidthNumber:
					value = Translate(source, FullWidthNumbers, HalfWidthNumbers);
					return Translate(value, ChineseNumbers, HalfWidthNumbers);
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

		static string Translate(string s, char[] source, char[] target) {
			var cs = s.ToCharArray();
			int p;
			for (int i = 0; i < cs.Length; i++) {
				p = Array.IndexOf(source, cs[i]);
				if (p != -1) {
					cs[i] = target[p];
				}
			}
			return new string(cs);
		}

		static class SC2TC
		{
			public static readonly Converter<string, string> Convert = new StringMapper(new CharacterMapper(ChineseCharMap.Simplified, ChineseCharMap.Traditional).Map).Convert;
		}
		static class TC2SC
		{
			public static readonly Converter<string, string> Convert = new StringMapper(new CharacterMapper(ChineseCharMap.Traditional, ChineseCharMap.Simplified).Map).Convert;
		}
		static class FWP2HWP
		{
			public static readonly Converter<string, string> Convert = new StringMapper(new CharacterMapper(FullWidthPunctuations, HalfWidthPunctuations).Map).Convert;
		}
		static class HWP2FWP
		{
			public static readonly Converter<string, string> Convert = new StringMapper(new CharacterMapper(HalfWidthPunctuations, FullWidthPunctuations).Map).Convert;
		}

		sealed class StringMapper
		{
			readonly Converter<char, char> Converter;
			public StringMapper(Converter<char, char> converter) {
				Converter = converter;
			}

			public unsafe string Convert(string value) {
				if (String.IsNullOrEmpty(value)) {
					return value;
				}
				var r = String.Copy(value);
				fixed (char* s = r) {
					char* c = s;
					char* end = c + value.Length;
					do {
						*c = Converter(*c);
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

		sealed class SequentialCharacterMapper
		{
			readonly char _From, _To;
			readonly int _Count;

			public SequentialCharacterMapper(char from, char to, int count) {
				_From = from;
				_To = to;
				_Count = count;
			}

			public char Map(char value) {
				var d = value - _From;
				return d >= 0 && d < _Count ? (char)(_To + d) : value;
			}
		}
	}
}
