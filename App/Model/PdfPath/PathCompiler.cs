using System;
using System.Collections.Generic;
using System.Linq;

namespace PDFPatcher.Model.PdfPath
{
	static class PathCompiler
	{
		const char PathSeparator = '/';
		const char SelfChar = '.';
		const char UniversalName = '*';
		const char StartPredicate = '[';
		const char EndPredicate = ']';

		static readonly char[] __PredicateChars = new char[] { StartPredicate, EndPredicate };

		sealed class Context
		{
			internal bool CanBeRoot { get; set; }
			internal PathAxisType Axis { get; set; }
		}

		public static IEnumerable<IPathExpression> Compile(string path) {
			var r = new Queue<IPathExpression>();
			if (String.IsNullOrEmpty(path)) {
				return r.ToArray();
			}
			var l = path.Length;
			var i = 0;
			string n;
			var ctx = new Context();
			while (i < l) {
				ctx.Axis = ExtractAxis(path, l, ctx.CanBeRoot, ref i);
				if (ctx.Axis == PathAxisType.Root) {
					ctx.CanBeRoot = false;
					r.Enqueue(new PathExpression(ctx.Axis));
					continue;
				}
				n = ExtractName(path, l, ref i);
				r.Enqueue(new PathExpression(ctx.Axis, n));
			}
			return r.ToArray();
		}

		private static PathAxisType ExtractAxis(string path, int length, bool canBeRoot, ref int index) {
			char c = path[index];
			if (__PredicateChars.Contains(c)) {
				throw new FormatException("“[]”筛选表达式前缺少节点轴及节点名称标识。");
			}

			if (c == PathSeparator) {
				if (MatchNextChar(path, length, index, PathSeparator)) {
					++index;
					return PathAxisType.Descendants;
				}
				else if (canBeRoot) {
					return PathAxisType.Root;
				}
				else {
					return PathAxisType.Children;
				}
			}
			else if (c == SelfChar) {
				if (MatchNextChar(path, length, index, SelfChar)) {
					++index;
					return PathAxisType.Parent;
				}
				else {
					return PathAxisType.None;
				}
			}
			else {
				return PathAxisType.Children;
			}
		}

		private static string ExtractName(string path, int length, ref int index) {
			char c = path[index];
			if (__PredicateChars.Contains(c)) {
				throw new FormatException("“[]”筛选表达式前缺少节点名称。");
			}
			if (c == UniversalName) {
				return null;
			}
			var n = new List<char>();
			while (Char.IsLetter(c) || n.Count > 0 && Char.IsLetterOrDigit(c)) {
				n.Add(c);
				++index;
				if (index < length) {
					c = path[index];
				}
				else {
					break;
				}
			}
			return n.Count > 0 ? new String(n.ToArray()) : null;
		}

		private static bool MatchNextChar(string path, int length, int index, char ch) {
			return index + 1 < length && path[index + 1] == ch;
		}
	}
}
