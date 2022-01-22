using System.Collections.Generic;
using System.Linq;
using PDFPatcher.Common;

namespace PDFPatcher.Model.PdfPath;

internal static class PathAxes
{
	private static readonly SelfAxis __Self = new();
	private static readonly ParentAxis __Parent = new();
	private static readonly ChildrenAxis __Children = new();
	private static readonly RootAxis __Root = new();
	private static readonly AncestorsAxis __Ancestors = new();
	private static readonly DescendantsAxis __Descendants = new();

	private static bool MatchesPredicate(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
		if (source == null) {
			return false;
		}

		if (name != null && (source.FriendlyName ?? source.Name) != name) {
			return false;
		}

		return predicates == null || predicates.All(p => p.Match(source, p.Operand1, p.Operand2) != false);
	}

	private static IList<DocumentObject> CompriseSingleObjectCollection(DocumentObject source) {
		return source == null ? PathExpression.EmptyMatchResult : new[] { source };
	}

	private static string GetLiteralValue(object operand) {
		return operand switch {
			null => string.Empty,
			string t => t,
			DocumentObject o => o.FriendlyValue ?? o.LiteralValue,
			IList<DocumentObject> { Count: > 0 } l => l[0].FriendlyValue ?? l[0].LiteralValue,
			IList<DocumentObject> l => string.Empty,
			_ => ((double)operand).ToText()
		};
	}

	public static IPathAxis Create(PathAxisType axisType) {
		switch (axisType) {
			case PathAxisType.None: return __Self;
			case PathAxisType.Children: return __Children;
			case PathAxisType.Parent: return __Parent;
			case PathAxisType.Ancestors: return __Ancestors;
			case PathAxisType.Descendants: return __Descendants;
			case PathAxisType.Root: return __Root;
			case PathAxisType.Previous:
			case PathAxisType.Next:
			default: break;
		}

		return __Children;
	}

	private sealed class SelfAxis : IPathAxis
	{
		#region IPathAxis 成员

		public PathAxisType Type => PathAxisType.None;

		public DocumentObject SelectObject(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
			return source != null && MatchesPredicate(source, name, predicates) ? source : null;
		}

		public IList<DocumentObject> SelectObjects(DocumentObject source, string name,
			IEnumerable<IPathPredicate> predicates) {
			return CompriseSingleObjectCollection(SelectObject(source, name, predicates));
		}

		#endregion
	}

	private sealed class ParentAxis : IPathAxis
	{
		#region IPathAxis 成员

		public PathAxisType Type => PathAxisType.Parent;

		public DocumentObject SelectObject(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
			return source != null && MatchesPredicate(source, name, predicates) ? source.Parent : null;
		}

		public IList<DocumentObject> SelectObjects(DocumentObject source, string name,
			IEnumerable<IPathPredicate> predicates) {
			return CompriseSingleObjectCollection(SelectObject(source, name, predicates));
		}

		#endregion
	}

	private sealed class ChildrenAxis : IPathAxis
	{
		#region IPathAxis 成员

		public PathAxisType Type => PathAxisType.Children;

		public DocumentObject SelectObject(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
			if (source is not { HasChildren: true }) {
				return null;
			}

			List<DocumentObject> r = new();
			return source.Children.FirstOrDefault(item => MatchesPredicate(item, name, predicates));
		}

		public IList<DocumentObject> SelectObjects(DocumentObject source, string name,
			IEnumerable<IPathPredicate> predicates) {
			return source is { HasChildren: true } ? source.Children.Where(item => MatchesPredicate(item, name, predicates)).ToArray() : PathExpression.EmptyMatchResult;
		}

		#endregion
	}

	private sealed class RootAxis : IPathAxis
	{
		#region IPathAxis 成员

		public PathAxisType Type => PathAxisType.Root;

		public DocumentObject SelectObject(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
			if (source == null) {
				return null;
			}

			while (source.Parent != null) {
				source = source.Parent;
			}

			return MatchesPredicate(source, name, predicates) ? source : null;
		}

		public IList<DocumentObject> SelectObjects(DocumentObject source, string name,
			IEnumerable<IPathPredicate> predicates) {
			return CompriseSingleObjectCollection(SelectObject(source, name, predicates));
		}

		#endregion
	}

	private sealed class AncestorsAxis : IPathAxis
	{
		#region IPathAxis 成员

		public PathAxisType Type => PathAxisType.Ancestors;

		public DocumentObject SelectObject(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
			if (source == null) {
				return null;
			}

			List<DocumentObject> r = new();
			while (source.Parent != null) {
				source = source.Parent;
				if (MatchesPredicate(source, name, predicates)) {
					return source;
				}
			}

			return null;
		}

		public IList<DocumentObject> SelectObjects(DocumentObject source, string name,
			IEnumerable<IPathPredicate> predicates) {
			if (source == null) {
				return PathExpression.EmptyMatchResult;
			}

			List<DocumentObject> r = new();
			while (source.Parent != null) {
				source = source.Parent;
				if (MatchesPredicate(source, name, predicates)) {
					r.Add(source);
				}
			}

			return r.ToArray();
		}

		#endregion
	}

	private sealed class DescendantsAxis : IPathAxis
	{
		private static void SelectObjects(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates,
			ICollection<DocumentObject> list) {
			if (source == null || source.HasChildren == false) {
				return;
			}

			foreach (DocumentObject item in source.Children) {
				if (MatchesPredicate(item, name, predicates)) {
					list.Add(item);
				}

				SelectObjects(item, name, predicates, list);
			}
		}

		#region IPathAxis 成员

		public PathAxisType Type => PathAxisType.Descendants;

		public DocumentObject SelectObject(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
			if (source == null || source.HasChildren == false) {
				return null;
			}

			foreach (DocumentObject item in source.Children) {
				if (MatchesPredicate(item, name, predicates)) {
					return item;
				}

				DocumentObject o;
				if ((o = SelectObject(item, name, predicates)) != null) {
					return o;
				}
			}

			return null;
		}

		public IList<DocumentObject> SelectObjects(DocumentObject source, string name,
			IEnumerable<IPathPredicate> predicates) {
			if (source == null || source.HasChildren == false) {
				return PathExpression.EmptyMatchResult;
			}

			List<DocumentObject> r = new();
			foreach (DocumentObject item in source.Children) {
				if (!MatchesPredicate(item, name, predicates)) {
					continue;
				}

				r.Add(item);
				SelectObjects(item, name, predicates, r);
			}

			return r.ToArray();
		}

		#endregion
	}
}