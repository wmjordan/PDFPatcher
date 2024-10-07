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

		public static bool HasCaseInsensitivePrefix(this string text, string prefix) {
			return text?.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) == true;
		}
		public static bool HasPrefix(this string text, string prefix) {
			return text?.StartsWith(prefix, StringComparison.Ordinal) == true;
		}

		/// <summary>返回字符串中包含指定字符串之后的子字符串。</summary>
		/// <remarks>如果找不到指定字符串，则返回空字符串。</remarks>
		public static string SubstringAfter(this string source, char value) {
			int index = source.LastIndexOf(value);
			return index != -1
				? source.Substring(index + 1)
				: String.Empty;
		}
	}
}
