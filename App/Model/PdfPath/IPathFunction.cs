namespace PDFPatcher.Model.PdfPath;

internal interface IPathFunction
{
	object Evaluate(DocumentObject source);
}

internal sealed class CurrentPosition : IPathFunction
{
	#region IPathFunction 成员

	public object Evaluate(DocumentObject source) {
		if (source == null || source.Parent == null) {
			return 0;
		}

		int i = 0;
		foreach (DocumentObject item in source.Parent.Children) {
			++i;
			if (item == source) {
				return i;
			}
		}

		return i;
	}

	#endregion
}