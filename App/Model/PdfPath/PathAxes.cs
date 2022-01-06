using System;
using System.Collections.Generic;
using System.Text;
using PDFPatcher.Common;

namespace PDFPatcher.Model.PdfPath
{
	static class PathAxes
	{
		static bool MatchesPredicate(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
			if (source == null) {
				return false;
			}
			if (name != null && (source.FriendlyName ?? source.Name) != name) {
				return false;
			}
			if (predicates == null) {
				return true;
			}
			foreach (var p in predicates) {
				if (p.Match(source, p.Operand1, p.Operand2) == false) {
					return false;
				}
			}
			return true;
		}
		static IList<DocumentObject> CompriseSingleObjectCollection(DocumentObject source) {
			return source == null ? PathExpression.EmptyMatchResult : new DocumentObject[] { source };
		}

		private static string GetLiteralValue(object operand) {
			if (operand == null) {
				return String.Empty;
			}
			if (operand is string t) {
				return t;
			}
			if (operand is DocumentObject o) {
				return o.FriendlyValue ?? o.LiteralValue;
			}
			if (operand is IList<DocumentObject> l) {
				if (l.Count > 0) {
					return l[0].FriendlyValue ?? l[0].LiteralValue;
				}
				else {
					return String.Empty;
				}
			}
			return ((double)operand).ToText();
		}

		static readonly SelfAxis __Self = new SelfAxis();
		static readonly ParentAxis __Parent = new ParentAxis();
		static readonly ChildrenAxis __Children = new ChildrenAxis();
		static readonly RootAxis __Root = new RootAxis();
		static readonly AncestorsAxis __Ancestors = new AncestorsAxis();
		static readonly DecendantsAxis __Decendants = new DecendantsAxis();

		sealed class SelfAxis : IPathAxis
		{
			#region IPathAxis 成员

			public PathAxisType Type => PathAxisType.None;

			public DocumentObject SelectObject(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
				return source != null && MatchesPredicate(source, name, predicates) ? source : null;
			}

			public IList<DocumentObject> SelectObjects(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
				return CompriseSingleObjectCollection(SelectObject(source, name, predicates));
			}

			#endregion
		}

		sealed class ParentAxis : IPathAxis
		{
			#region IPathAxis 成员

			public PathAxisType Type => PathAxisType.Parent;

			public DocumentObject SelectObject(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
				return source != null && MatchesPredicate(source, name, predicates) ? source.Parent : null;
			}

			public IList<DocumentObject> SelectObjects(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
				return CompriseSingleObjectCollection(SelectObject(source, name, predicates));
			}

			#endregion
		}

		sealed class ChildrenAxis : IPathAxis
		{
			#region IPathAxis 成员

			public PathAxisType Type => PathAxisType.Children;

			public DocumentObject SelectObject(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
				if (source != null && source.HasChildren) {
					var r = new List<DocumentObject>();
					foreach (var item in source.Children) {
						if (MatchesPredicate(item, name, predicates)) {
							return item;
						}
					}
				}
				return null;
			}

			public IList<DocumentObject> SelectObjects(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
				if (source != null && source.HasChildren) {
					var r = new List<DocumentObject>();
					foreach (var item in source.Children) {
						if (MatchesPredicate(item, name, predicates)) {
							r.Add(item);
						}
					}
					return r.ToArray();
				}
				else {
					return PathExpression.EmptyMatchResult;
				}
			}

			#endregion
		}

		sealed class RootAxis : IPathAxis
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
				if (MatchesPredicate(source, name, predicates)) {
					return source;
				}
				return null;
			}

			public IList<DocumentObject> SelectObjects(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
				return CompriseSingleObjectCollection(SelectObject(source, name, predicates));
			}

			#endregion
		}

		sealed class AncestorsAxis : IPathAxis
		{
			#region IPathAxis 成员

			public PathAxisType Type => PathAxisType.Ancestors;

			public DocumentObject SelectObject(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
				if (source == null) {
					return null;
				}
				var r = new List<DocumentObject>();
				while (source.Parent != null) {
					source = source.Parent;
					if (MatchesPredicate(source, name, predicates)) {
						return source;
					}
				}
				return null;
			}

			public IList<DocumentObject> SelectObjects(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
				if (source == null) {
					return PathExpression.EmptyMatchResult;
				}
				var r = new List<DocumentObject>();
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

		sealed class DecendantsAxis : IPathAxis
		{
			void SelectObjects(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates, List<DocumentObject> list) {
				if (source == null || source.HasChildren == false) {
					return;
				}
				foreach (var item in source.Children) {
					if (MatchesPredicate(item, name, predicates)) {
						list.Add(item);
					}
					SelectObjects(item, name, predicates, list);
				}
			}

			#region IPathAxis 成员

			public PathAxisType Type => PathAxisType.Decendants;

			public DocumentObject SelectObject(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
				if (source == null || source.HasChildren == false) {
					return null;
				}
				foreach (var item in source.Children) {
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

			public IList<DocumentObject> SelectObjects(DocumentObject source, string name, IEnumerable<IPathPredicate> predicates) {
				if (source == null || source.HasChildren == false) {
					return PathExpression.EmptyMatchResult;
				}
				var r = new List<DocumentObject>();
				foreach (var item in source.Children) {
					if (MatchesPredicate(item, name, predicates)) {
						r.Add(item);
						SelectObjects(item, name, predicates, r);
					}
				}
				return r.ToArray();
			}

			#endregion
		}

		public static IPathAxis Create(PathAxisType axisType) {
			switch (axisType) {
				case PathAxisType.None: return __Self;
				case PathAxisType.Children: return __Children;
				case PathAxisType.Parent: return __Parent;
				case PathAxisType.Ancestors: return __Ancestors;
				case PathAxisType.Decendants: return __Decendants;
				case PathAxisType.Root: return __Root;
				case PathAxisType.Previous:
				case PathAxisType.Next:
				default: break;
			}
			return __Children;
		}
	}

}
