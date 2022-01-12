using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace PDFPatcher.Common
{
	static class ValueHelper
	{
		[DebuggerStepThrough]
		public static TValue CastOrDefault<TValue>(this object value, TValue defaultValue) where TValue : struct {
			return value is TValue v ? v : defaultValue;
		}
		[DebuggerStepThrough]
		public static TValue CastOrDefault<TValue>(this object value) where TValue : struct {
			return value is TValue v ? v : default;
		}
		[DebuggerStepThrough]
		public static bool HasContent<T>(this ICollection<T> collection) {
			return collection?.Count > 0;
		}
		[DebuggerStepThrough]
		public static T SubstituteDefault<T>(this T value, T otherValue) {
			return EqualityComparer<T>.Default.Equals(value, default(T)) ? otherValue : value;
		}
		public static TDisposable TryDispose<TDisposable>(this TDisposable disposable)
			where TDisposable : IDisposable {
			if (disposable != null) {
				try {
					disposable.Dispose();
				}
				catch (Exception) {
					// ignore
				}
			}
			return disposable;
		}
		[DebuggerStepThrough]
		public static bool IsInCollection<T>(T input, params T[] values) {
			return values != null && input != null && values.Length != 0 && Array.IndexOf(values, input) != -1;
		}
		[DebuggerStepThrough]
		public static IComparer<TItem> GetReverseComparer<TItem>()
		where TItem : IComparable<TItem> {
			return new ReverseComparer<TItem>();
		}
		public static T LimitInRange<T>(this T value, T minValue, T maxValue)
			where T : IComparable<T> {
			return
				value.CompareTo(minValue) < 0 ? minValue
				: value.CompareTo(maxValue) > 0 ? maxValue
				: value;
		}
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
			TValue r;
			return dictionary == null ? default : dictionary.TryGetValue(key, out r) ? r : r;
		}
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) {
			TValue r;
			return dictionary != null && dictionary.TryGetValue(key, out r) ? r : defaultValue;
		}

		public static TMapped MapValue<TValue, TMapped>(TValue input, TValue[] fromValues, TMapped[] toValues) {
			return MapValue(input, fromValues, toValues, default(TMapped));
		}
		public static TMapped MapValue<TValue, TMapped>(TValue input, TValue[] fromValues, TMapped[] toValues, TMapped defaultValue) {
			if (fromValues == null) {
				return defaultValue;
			}
			if (toValues == null) {
				return defaultValue;
			}
			var i = Array.IndexOf(fromValues, input);
			if (i == -1 || i >= toValues.Length) {
				return defaultValue;
			}
			return toValues[i];
		}

		public static TMapped MapValue<TValue, TMapped>(TValue input, IEnumerable<TValue> fromValues, IEnumerable<TMapped> toValues, TMapped defaultValue) {
			if (fromValues == null) {
				return defaultValue;
			}
			if (toValues == null) {
				return defaultValue;
			}
			var i = 0;
			var j = 0;
			var c = EqualityComparer<TValue>.Default;
			foreach (var x in fromValues) {
				if (c.Equals(input, x)) {
					foreach (var y in toValues) {
						if (i == j) {
							return y;
						}
						j++;
					}
					return defaultValue;
				}
				i++;
			}
			return defaultValue;
		}
		public static IEnumerable ForEach<TItem>(this IEnumerable collection, Action<TItem> itemHandler) {
			if (collection == null || itemHandler == null) {
				return collection;
			}
			foreach (var item in collection) {
				if (item is TItem v) {
					itemHandler(v);
				}
			}
			return collection;
		}
		public static TCollection AddRange<TCollection, T>(this TCollection target, IEnumerable<T> source)
			where TCollection : ICollection<T> {
			if (source == null || target == null) {
				return target;
			}
			if (target is List<T> list) {
				list.AddRange(source);
				return target;
			}

			foreach (T item in source) {
				target.Add(item);
			}
			return target;
		}

		[DebuggerStepThrough]
		public static string ToText(this DateTimeOffset value) {
			return value.ToString(NumberFormatInfo.InvariantInfo);
		}

		[DebuggerStepThrough]
		public static string ToText(this int value) {
			return value.ToString(NumberFormatInfo.InvariantInfo);
		}

		[DebuggerStepThrough]
		public static string ToText(this float value) {
			return Math.Abs(value) < 0.00001 ? "0" : value.ToString(NumberFormatInfo.InvariantInfo);
		}

		[DebuggerStepThrough]
		public static string ToText(this double value) {
			return Math.Abs(value) < 0.000000000001 ? "0" : value.ToString(NumberFormatInfo.InvariantInfo);
		}

		[DebuggerStepThrough]
		public static string ToText(this long value) {
			return value.ToString(CultureInfo.InvariantCulture);
		}

		[DebuggerStepThrough]
		public static string ToText(this decimal value) {
			return value.ToString(NumberFormatInfo.InvariantInfo);
		}

		[DebuggerStepThrough]
		public static string ToText<TFormattable>(this TFormattable value)
			where TFormattable : IFormattable {
			return value.ToString(null, NumberFormatInfo.InvariantInfo);
		}
		[DebuggerStepThrough]
		public static string ToText<TFormattable>(this TFormattable value, string format)
			where TFormattable : IFormattable {
			return value.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		public static bool ToBoolean(this string value, bool defaultValue) {
			if (string.IsNullOrEmpty(value)) {
				return defaultValue;
			}
			switch (ParseBoolean(value)) {
				case 1: return true;
				case 0: return false;
				default: return defaultValue;
			}
		}

		static int ParseBoolean(string value) {
			const int True = 1, False = 0, Invalid = -1;
			var i = 0;
			var l = value.Length;
			do {
				var c = value[i];
				switch (c) {
					case 'T':
					case 't':
						if (i + 3 < l && ((c = value[++i]) == 'r' || c == 'R') && ((c = value[++i]) == 'u' || c == 'U') && ((c = value[++i]) == 'e' || c == 'E')) {
							goto EndsWithWhitespaceTrue;
						}
						return Invalid;
					case 'F':
					case 'f':
						if (i + 4 < l && ((c = value[++i]) == 'a' || c == 'A') && ((c = value[++i]) == 'l' || c == 'L') && ((c = value[++i]) == 's' || c == 'S') && ((c = value[++i]) == 'e' || c == 'E')) {
							goto EndsWithWhitespaceFalse;
						}
						return Invalid;
					case 'Y':
					case 'y':
						if (i + 2 < l && ((c = value[++i]) == 'e' || c == 'E') && ((c = value[++i]) == 's' || c == 'S')) {
							goto EndsWithWhitespaceTrue;
						}
						return Invalid;
					case 'N':
					case 'n':
						if (i + 1 < l && ((c = value[++i]) == 'o' || c == 'O')) {
							goto EndsWithWhitespaceFalse;
						}
						return Invalid;
					case 'O':
					case 'o':
						if (i + 2 < l && ((c = value[++i]) == 'f' || c == 'F') && ((c = value[++i]) == 'f' || c == 'F')) {
							goto EndsWithWhitespaceFalse;
						}
						if (i + 1 < l && ((c = value[++i]) == 'n' || c == 'N' || c == 'k' || c == 'K')) {
							goto EndsWithWhitespaceTrue;
						}
						return Invalid;
					case '是':
					case '对':
					case '开':
						goto EndsWithWhitespaceTrue;
					case '否':
					case '关':
						goto EndsWithWhitespaceFalse;
					case '正':
						if (i + 1 < l && value[++i] == '确') {
							goto EndsWithWhitespaceTrue;
						}
						goto EndsWithWhitespaceFalse;
					case '错':
						if (i + 1 < l && value[++i] == '误') {
							goto EndsWithWhitespaceFalse;
						}
						goto EndsWithWhitespaceFalse;
					default:
						if (Char.IsWhiteSpace(c)) {
							continue;
						}
						if (c >= '0' && c <= '9' || c == '-' || c == '+' || c == '.') {
							bool notZero = c > '0' && c <= '9';
							var hasDot = false;
							while (++i < l) {
								c = value[i];
								if (Char.IsNumber(c) == false && Char.IsWhiteSpace(c) == false) {
									if (c == '.') {
										if (hasDot == false) {
											hasDot = true;
											continue;
										}
										else {
											return Invalid;
										}
									}
									return Invalid;
								}
								if (notZero == false) {
									notZero = c > '0' && c <= '9';
								}
							}
							return notZero ? True : False;
						}
						return -1;
				}
			} while (++i < l);
		EndsWithWhitespaceTrue:
			while (++i < l && Char.IsWhiteSpace(value[i])) { }
			return i == l ? True : Invalid;
		EndsWithWhitespaceFalse:
			while (++i < l && Char.IsWhiteSpace(value[i])) { }
			return i == l ? False : Invalid;
		}

		[DebuggerStepThrough]
		public static int ToInt32(this float value) {
			return (int)(value > 0 ? value + 0.5f : value - 0.5f);
		}

		[DebuggerStepThrough]
		public static int ToInt32(this double value) {
			return (int)(value > 0 ? value + 0.5d : value - 0.5d);
		}

		[DebuggerStepThrough]
		public static long ToInt64(this float value) {
			return (long)(value > 0 ? value + 0.5f : value - 0.5f);
		}

		[DebuggerStepThrough]
		public static long ToInt64(this double value) {
			return (long)(value > 0 ? value + 0.5d : value - 0.5d);
		}

		[DebuggerStepThrough]
		public static int ToInt32(this string value) {
			int i;
			value.TryParse(out i);
			return i;
		}
		[DebuggerStepThrough]
		public static int ToInt32(this string value, int defaultValue) {
			int i;
			return value.TryParse(out i) ? i : defaultValue;
		}

		[DebuggerStepThrough]
		public static long ToInt64(this string value) {
			long i;
			value.TryParse(out i);
			return i;
		}

		[DebuggerStepThrough]
		public static long ToInt64(this string value, long defaultValue) {
			long i;
			return value.TryParse(out i) ? i : defaultValue;
		}

		[DebuggerStepThrough]
		public static float ToSingle(this string value) {
			float i;
			value.TryParse(out i);
			return i;
		}

		[DebuggerStepThrough]
		public static float ToSingle(this string value, float defaultValue) {
			float i;
			return value.TryParse(out i) ? i : defaultValue;
		}

		[DebuggerStepThrough]
		public static double ToDouble(this string value) {
			double i;
			value.TryParse(out i);
			return i;
		}

		[DebuggerStepThrough]
		public static double ToDouble(this string value, double defaultValue) {
			double i;
			return value.TryParse(out i) ? i : defaultValue;
		}

		[DebuggerStepThrough]
		public static string ToText(this byte value) {
			return value.ToString(CultureInfo.InvariantCulture);
		}
		public static bool TryParse(this string value, out int result) {
			return Int32.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result)
				|| ParseFloatStringToInt32(value, ref result);
		}

		static bool ParseFloatStringToInt32(string value, ref int result) {
			if (double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var d)) {
				result = d.ToInt32();
				return true;
			}
			return false;
		}

		public static bool TryParse(this string value, out long result) {
			return Int64.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result)
				|| ParseFloatStringToInt64(value, ref result);
		}

		static bool ParseFloatStringToInt64(string value, ref long result) {
			if (double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var d)) {
				result = d.ToInt64();
				return true;
			}
			return false;
		}

		[DebuggerStepThrough]
		public static bool TryParse(this string value, out float result) {
			return float.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result);
		}

		[DebuggerStepThrough]
		public static bool TryParse(this string value, out double result) {
			return double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result);
		}

		[DebuggerStepThrough]
		public static bool TryParse(this string value, out decimal result) {
			return decimal.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result);
		}

		public static string ToRoman(this int value) {
			if (value > 49999 || value < 1) {
				return string.Empty;
			}
			var sb = new StringBuilder();
			do {
				for (int i = value < 40 ? 5 : value < 400 ? 9 : Roman.Values.Length - 1; i >= 0; i--) {
					var n = Roman.Values[i];
					if (value >= n) {
						value -= n;
						sb.Append(Roman.Chars[i]);
						break;
					}
				}
			} while (value > 0);
			return sb.ToString();
		}
		public static string ToAlphabet(this int value, bool upper) {
			if (value <= 0) {
				return string.Empty;
			}
			var stack = new char[7];
			var c = (upper ? 'A' : 'a') - 1;
			var p = -1;
			while (value > 0) {
				var i = value % 26;
				stack[++p] = (char)(c + (i == 0 ? 26 : i));
				value = --value / 26;
			}
			return new string(stack, 0, p);
		}
		public static string ToHexBinString(this byte value, bool upperCaseHex) {
			return HexBinByteToString.ToString(value, upperCaseHex);
		}
		public static string ToHexBinString(this byte[] source) {
			return InternalToHexBinString(source, true, '\0', 0, Int32.MaxValue);
		}
		public static string ToHexBinString(this byte[] source, bool upperCaseHex, char separator, int offset, int count) {
			return InternalToHexBinString(source, upperCaseHex, separator, offset, count);
		}
		unsafe static string InternalToHexBinString(byte[] source, bool upperCaseHex, char separator, int offset, int count) {
			if (source == null || offset < 0 || count < 1) {
				return String.Empty;
			}
			var length = source.Length;
			if (length == 0 || offset >= length) {
				return String.Empty;
			}
			if (count > length - offset) {
				count = length - offset;
			}
			if (count == 1) {
				return source[offset].ToHexBinString(upperCaseHex);
			}
			var result = new string('0', (count << 1) + (separator > 0 ? count - 1 : 0));
			fixed (char* p = result)
			fixed (byte* bp = &source[offset]) {
				byte* b = bp;
				byte* end = bp + count;
				var mapper = HexBinByteValues.GetHexBinMapper(upperCaseHex);
				if (separator == 0) {
					int* h = (int*)p;
					while (b < end) {
						*(h++) = mapper[*(b++)];
					}
					return result;
				}
				else {
					char* c = p;
					*(int*)(c) = mapper[*bp];
					while (++b < end) {
						c += 2;
						*c = separator;
						*(int*)(++c) = mapper[*b];
					}
					return result;
				}
			}
		}

		static class HexBinByteToString
		{
			static readonly string[] __HexBins = InitHexBinStrings(true);
			static readonly string[] __HexBinLower = InitHexBinStrings(false);

			public static string ToString(byte value, bool upperCase) {
				return (upperCase ? __HexBins : __HexBinLower)[value];
			}
			static string[] InitHexBinStrings(bool upperCase) {
				var s = new string[Byte.MaxValue + 1];
				for (int i = 0; i < s.Length; i++) {
					s[i] = ToHexBinString((byte)i, upperCase);
				}
				return s;

				string ToHexBinString(byte value, bool upperCaseHex) {
					var h = (upperCaseHex ? 0x41 : 0x61) - 10;
					var a = new char[2];
					var t = (byte)(value >> 4);
					a[0] = (char)(t > 9 ? t + h : t + 0x30);
					t = (byte)(value & 0x0F);
					a[1] = (char)(t > 9 ? t + h : t + 0x30);
					return new string(a);
				}
			}
		}
		static class HexBinByteValues
		{
			static readonly int[] __HexBins = InitHexBin(true);
			static readonly int[] __HexBinLowerCase = InitHexBin(false);
			internal static readonly long QuadupleZero = ((long)__HexBins[0]) << 32 | __HexBins[0];

			unsafe static int[] InitHexBin(bool upperCase) {
				var v = new int[Byte.MaxValue + 1];
				var a = new char[2];
				var h = (upperCase ? 0x41 : 0x61) - 10;
				for (int i = 0; i <= Byte.MaxValue; i++) {
					var t = (byte)(i >> 4);
					a[0] = (char)(t > 9 ? t + h : t + 0x30);
					t = (byte)(i & 0x0F);
					a[1] = (char)(t > 9 ? t + h : t + 0x30);
					fixed (char* p = new string(a)) {
						v[i] = *(int*)p;
					}
				}
				return v;
			}
			public static int[] GetHexBinMapper(bool upperCase) {
				return upperCase ? __HexBins : __HexBinLowerCase;
			}
		}

		static class Roman
		{
			internal static readonly int[] Values = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000, 4000, 5000, 9000, 10000, 40000 };
			internal static readonly string[] Chars = { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M", "Mv", "v", "Mx", "x", "xl" };
		}

		sealed class ReverseComparer<T> : IComparer<T>
			where T : IComparable<T>
		{
			int IComparer<T>.Compare(T x, T y) {
				return y.CompareTo(x);
			}
		}
	}
}
