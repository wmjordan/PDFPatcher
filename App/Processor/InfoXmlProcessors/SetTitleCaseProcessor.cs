using System;

namespace PDFPatcher.Processor
{
	sealed class SetTitleCaseProcessor : IPdfInfoXmlProcessor
	{
		static readonly System.Globalization.TextInfo __currentTextInfo =
			System.Globalization.CultureInfo.CurrentCulture.TextInfo;

		static readonly char[] SimplifiedChars = ChineseCharMap.Simplified.ToCharArray();
		static readonly char[] TraditionalChars = ChineseCharMap.Traditional.ToCharArray();
		static readonly char[] FullWidthNumbers = "０１２３４５６７８９０".ToCharArray();
		static readonly char[] HalfWidthNumbers = "01234567890".ToCharArray();
		static readonly char[] ChineseNumbers = "○一二三四五六七八九〇".ToCharArray();
		static readonly char[] TraditionalChineseNumbers = "零壹贰叁肆伍陆柒捌玖零".ToCharArray();

		static readonly char[] FullWidthAlphabetics =
			"ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ".ToCharArray();

		static readonly char[] HalfWidthAlphabetics =
			"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

		static readonly char[] FullWidthPunctuations = "！＂＃＄％＆＇（）＊＋，－．／：；＜＝＞？＠［＼］＾＿｀｛｜｝～".ToCharArray();
		static readonly char[] HalfWidthPunctuations = "!\"#$%&'()*+,-./;:<=>?@[\\]^_`{|}~".ToCharArray();

		internal static string[] CaseNames = new string[] {
			"首字母大写", "英文大写", "英文小写", "全角数字", "全角字母", "全角标点", "半角数字", "半角字母", "半角标点", "中文数字", "大写中文数字", "繁体汉字转简体",
			"简体汉字转繁体"
		};

		public enum LetterCase
		{
			Title, Upper, Lower,
			FullWidthNumber, FullWidthAlphabetic, FullWidthPunctuation,
			HalfWidthNumber, HalfWidthAlphabetic, HalfWidthPunctuation,
			ChineseNumber, TraditionalChineseNumbers,
			TraditionalToSimplifiedCjk, SimplifiedToTraditionalCjk
		}

		public LetterCase Case { get; private set; }

		public SetTitleCaseProcessor(LetterCase letterCase) {
			Case = letterCase;
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

		#region IInfoDocProcessor 成员

		public string Name => "设置书签文本为" + CaseNames[(int)Case];

		public IUndoAction Process(System.Xml.XmlElement item) {
			var a = item.GetAttributeNode(Constants.BookmarkAttributes.Title);
			if (a == null) {
				return null;
			}

			string value;
			switch (Case) {
				case LetterCase.Lower:
					value = a.Value.ToLowerInvariant();
					break;
				case LetterCase.Upper:
					value = a.Value.ToUpperInvariant();
					break;
				case LetterCase.Title:
					value = __currentTextInfo.ToTitleCase(a.Value.ToLowerInvariant());
					break;
				case LetterCase.FullWidthAlphabetic:
					value = Translate(a.Value, HalfWidthAlphabetics, FullWidthAlphabetics);
					break;
				case LetterCase.FullWidthNumber:
					value = Translate(a.Value, HalfWidthNumbers, FullWidthNumbers);
					value = Translate(value, ChineseNumbers, FullWidthNumbers);
					break;
				case LetterCase.FullWidthPunctuation:
					value = Translate(a.Value, HalfWidthNumbers, FullWidthPunctuations);
					break;
				case LetterCase.HalfWidthAlphabetic:
					value = Translate(a.Value, FullWidthAlphabetics, HalfWidthAlphabetics);
					break;
				case LetterCase.HalfWidthNumber:
					value = Translate(a.Value, FullWidthNumbers, HalfWidthNumbers);
					value = Translate(value, ChineseNumbers, HalfWidthNumbers);
					break;
				case LetterCase.HalfWidthPunctuation:
					value = Translate(a.Value, FullWidthPunctuations, HalfWidthPunctuations);
					break;
				case LetterCase.ChineseNumber:
					value = Translate(a.Value, FullWidthNumbers, ChineseNumbers);
					value = Translate(value, HalfWidthNumbers, ChineseNumbers);
					break;
				case LetterCase.TraditionalChineseNumbers:
					value = Translate(a.Value, FullWidthNumbers, TraditionalChineseNumbers);
					value = Translate(value, HalfWidthNumbers, TraditionalChineseNumbers);
					value = Translate(value, ChineseNumbers, TraditionalChineseNumbers);
					break;
				case LetterCase.SimplifiedToTraditionalCjk:
					value = Translate(a.Value, SimplifiedChars, TraditionalChars);
					break;
				case LetterCase.TraditionalToSimplifiedCjk:
					value = Translate(a.Value, TraditionalChars, SimplifiedChars);
					break;
				default: throw new System.ArgumentOutOfRangeException("LetterCase");
			}

			if (a.Value == value) {
				return null;
			}

			return UndoAttributeAction.GetUndoAction(item, Constants.BookmarkAttributes.Title, value);
		}

		#endregion
	}
}