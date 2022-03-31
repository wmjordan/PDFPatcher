using System;
using System.Text;

namespace PDFPatcher.Common
{
	internal static class StringBuilderCache
	{
		internal const int MaxBuilderSize = 360;
		const int DefaultCapacity = 16; // == StringBuilder.DefaultCapacity

		[ThreadStatic]
		static StringBuilder __CachedInstance;

		public static StringBuilder Acquire(int capacity = DefaultCapacity) {
			if (capacity <= MaxBuilderSize) {
				StringBuilder sb = __CachedInstance;
				if (sb != null) {
					// Avoid stringbuilder block fragmentation by getting a new StringBuilder
					// when the requested size is larger than the current capacity
					if (capacity <= sb.Capacity) {
						__CachedInstance = null;
						sb.Length = 0;
						return sb;
					}
				}
			}

			return new StringBuilder(capacity);
		}

		public static void Release(StringBuilder sb) {
			if (sb.Capacity <= MaxBuilderSize) {
				__CachedInstance = sb;
			}
		}

		public static string GetStringAndRelease(StringBuilder sb) {
			string result = sb.ToString();
			Release(sb);
			return result;
		}
	}
}
