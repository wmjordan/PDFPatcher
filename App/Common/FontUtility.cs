using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PDFPatcher.Common
{
	static class FontUtility
	{
		static readonly Regex _italic =
			new Regex(" (?:Italic|Oblique)$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

		static readonly Regex _bold = new Regex(" Bold$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

		static readonly Regex _boldItalic = new Regex(" Bold (?:Italic|Oblique)$",
			RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

		static FriendlyFontName[] _Fonts;

		public static FriendlyFontName[] InstalledFonts {
			get {
				if (_Fonts == null) {
					ListInstalledFonts();
				}

				return _Fonts;
			}
		}

		private static void ListInstalledFonts() {
			var uf = new List<FriendlyFontName>(); // 可能包含中文的字体
			var of = new List<FriendlyFontName>(); // 其他字体
			var fs = FontHelper.GetInstalledFonts(false);
			string dn /*显示名称*/, fn /*字体名称*/;
			foreach (var item in fs.Keys) {
				fn = item;
				dn = _boldItalic.Replace(fn, "(粗斜体)");
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
				return OriginalName.CompareTo(other.OriginalName);
			}

			#endregion
		}
	}
}