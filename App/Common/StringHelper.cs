using System;
using System.Diagnostics;

namespace PDFPatcher.Common;

internal static class StringHelper
{
	[DebuggerStepThrough]
	public static bool IsNullOrWhiteSpace(this string text) {
		return string.IsNullOrWhiteSpace(text);
	}

	public static string ReplaceControlAndBomCharacters(string source) {
		if (string.IsNullOrEmpty(source)) {
			return string.Empty;
		}

		char[] p = source.ToCharArray();
		bool m = false;
		for (int i = 0; i < source.Length; i++) {
			ref char c = ref p[i];
			if ((!char.IsControl(c) || c == '\t' || c == '\r' || c == '\n') &&
				(c <= 0xFFFD || (c != 0xFFFF && c != 0xFFFE && c != 0xFFEF))) {
				continue;
			}

			c = ' ';
			m = true;
		}

		return m ? new string(p) : source;
	}

	public static string Take(this string text, int startIndex, int count) {
		if (string.IsNullOrEmpty(text) || startIndex >= text.Length) {
			return string.Empty;
		}

		if (startIndex >= 0) {
			return count <= 0
				? string.Empty
				: text.Substring(startIndex, startIndex + count > text.Length ? text.Length - startIndex : count);
		}

		startIndex = text.Length + startIndex;
		if (startIndex < 0) {
			startIndex = 0;
		}

		return count <= 0
			? string.Empty
			: text.Substring(startIndex, startIndex + count > text.Length ? text.Length - startIndex : count);
	}

	public static string ToDescription<TEnum>(this TEnum value) where TEnum : Enum {
		return value.ToString();
	}
}