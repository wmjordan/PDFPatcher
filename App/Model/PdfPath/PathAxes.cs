namespace PDFPatcher.Model.PdfPath;

internal static class PathAxes
{
	private static readonly SelfAxis __Self = new();
	private static readonly ParentAxis __Parent = new();
	private static readonly ChildrenAxis __Children = new();
	private static readonly RootAxis __Root = new();
	private static readonly AncestorsAxis __Ancestors = new();
	private static readonly DescendantsAxis __Descendants = new();

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

		#endregion
	}

	private sealed class ParentAxis : IPathAxis
	{
		#region IPathAxis 成员

		public PathAxisType Type => PathAxisType.Parent;

		#endregion
	}

	private sealed class ChildrenAxis : IPathAxis
	{
		#region IPathAxis 成员

		public PathAxisType Type => PathAxisType.Children;

		#endregion
	}

	private sealed class RootAxis : IPathAxis
	{
		#region IPathAxis 成员

		public PathAxisType Type => PathAxisType.Root;

		#endregion
	}

	private sealed class AncestorsAxis : IPathAxis
	{
		#region IPathAxis 成员

		public PathAxisType Type => PathAxisType.Ancestors;

		#endregion
	}

	private sealed class DescendantsAxis : IPathAxis
	{
		#region IPathAxis 成员

		public PathAxisType Type => PathAxisType.Descendants;

		#endregion
	}
}