using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PDFPatcher.Common;

internal static class FontUtility
{
	private static readonly Regex _italic = new(" (?:Italic|Oblique)$",
		RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	private static readonly Regex _bold = new(" Bold$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	private static readonly Regex _boldItalic =
		new(" Bold (?:Italic|Oblique)$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	private static FriendlyFontName[] _Fonts;

	public static FriendlyFontName[] InstalledFonts {
		get {
			if (_Fonts == null) {
				ListInstalledFonts();
			}

			return _Fonts;
		}
	}

	private static void ListInstalledFonts() {
		List<FriendlyFontName> uf = new(); // 可能包含中文的字体
		List<FriendlyFontName> of = new(); // 其他字体
		Dictionary<string, string> fs = FontHelper.GetInstalledFonts(false);
		foreach (string item in fs.Keys) {
			string fn = item /*字体名称*/;
			string dn = _boldItalic.Replace(fn, "(粗斜体)") /*字体名称*/;
			dn = _italic.Replace(dn, "(斜体)");
			dn = _bold.Replace(dn, "(粗体)");
			if (dn[0] > 0xFF) {
				uf.Add(new FriendlyFontName(fn, dn));
			}
			else {
				of.Add(new FriendlyFontName(fn, dn));
			}
		}

		uf.Sort();
		of.Sort();
		_Fonts = new FriendlyFontName[uf.Count + of.Count];
		uf.CopyTo(_Fonts);
		of.CopyTo(_Fonts, uf.Count);
	}

	internal struct FriendlyFontName : IComparable<FriendlyFontName>
	{
		public string OriginalName;
		public string DisplayName;

		public FriendlyFontName(string originalName, string displayName) {
			OriginalName = originalName;
			DisplayName = displayName != originalName ? displayName : null;
		}

		public override string ToString() {
			return DisplayName ?? OriginalName;
		}

		#region IComparable<FriendlyFontName> 成员

		int IComparable<FriendlyFontName>.CompareTo(FriendlyFontName other) {
			return string.Compare(OriginalName, other.OriginalName, StringComparison.Ordinal);
		}

		#endregion
	}
}