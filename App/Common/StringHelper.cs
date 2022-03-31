using System;
using System.Diagnostics;

namespace PDFPatcher.Common
{
	static class StringHelper
	{
		[DebuggerStepThrough]
		public static bool IsNullOrWhiteSpace(this string text) {
			return String.IsNullOrWhiteSpace(text);
		}

		public static string ReplaceControlAndBomCharacters(string source) {
			if (String.IsNullOrEmpty(source)) {
				return String.Empty;
			}
			var p = source.ToCharArray();
			var m = false;
			for (int i = 0; i < source.Length; i++) {
				ref var c = ref p[i];
				if ((Char.IsControl(c) && c != '\t' && c != '\r' && c != '\n')
					|| (c > 0xFFFD && (c == 0xFFFF || c == 0xFFFE || c == 0xFFEF))
					) {
					c = ' ';
					m = true;
				}
			}
			return m ? new String(p) : source;
		}

		public static string Take(this string text, int startIndex, int count) {
			if (String.IsNullOrEmpty(text) || startIndex >= text.Length) {
				return String.Empty;
			}
			if (startIndex < 0) {
				startIndex = text.Length + startIndex;
				if (startIndex < 0) {
					startIndex = 0;
				}
			}
			return count <= 0
				? String.Empty
				: text.Substring(startIndex, startIndex + count > text.Length ? text.Length - startIndex : count);
		}

		public static string ToDescription<TEnum>(this TEnum value) where TEnum : Enum {
			return value.ToString();
		}
	}
}
