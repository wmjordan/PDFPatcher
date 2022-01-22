using System;
using System.Collections.Generic;
using System.Linq;

namespace PDFPatcher.Model.PdfPath;

internal class PathCompiler
{
	private const char PathSeparator = '/';
	private const char SelfChar = '.';
	private const char UniversalName = '*';
	private const char StartPredicate = '[';
	private const char EndPredicate = ']';

	private static readonly char[] __PredicateChars = { StartPredicate, EndPredicate };

	public static IEnumerable<IPathExpression> Compile(string path) {
		Queue<IPathExpression> r = new();
		if (string.IsNullOrEmpty(path)) {
			return r.ToArray();
		}

		int l = path.Length;
		int i = 0;
		Context ctx = new();
		while (i < l) {
			ctx.Axis = ExtractAxis(path, l, ctx.CanBeRoot, ref i);
			if (ctx.Axis == PathAxisType.Root) {
				ctx.CanBeRoot = false;
				r.Enqueue(new PathExpression(ctx.Axis));
				continue;
			}

			string n = ExtractName(path, l, ref i);
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

			if (canBeRoot) {
				return PathAxisType.Root;
			}

			return PathAxisType.Children;
		}

		if (c == SelfChar) {
			if (MatchNextChar(path, length, index, SelfChar)) {
				++index;
				return PathAxisType.Parent;
			}

			return PathAxisType.None;
		}

		return PathAxisType.Children;
	}

	private static string ExtractName(string path, int length, ref int index) {
		char c = path[index];
		if (__PredicateChars.Contains(c)) {
			throw new FormatException("“[]”筛选表达式前缺少节点名称。");
		}

		if (c == UniversalName) {
			return null;
		}

		List<char> n = new();
		while (char.IsLetter(c) || (n.Count > 0 && char.IsLetterOrDigit(c))) {
			n.Add(c);
			++index;
			if (index < length) {
				c = path[index];
			}
			else {
				break;
			}
		}

		return n.Count > 0 ? new string(n.ToArray()) : null;
	}

	private static bool MatchNextChar(string path, int length, int index, char ch) {
		return index + 1 < length && path[index + 1] == ch;
	}

	private sealed class Context
	{
		internal bool CanBeRoot { get; set; }
		internal PathAxisType Axis { get; set; }
	}
}