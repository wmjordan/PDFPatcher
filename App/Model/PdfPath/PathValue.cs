using PDFPatcher.Common;

namespace PDFPatcher.Model.PdfPath;

public static class PathValue
{
	public static double ToNumber(DocumentObject source, IPathValue value) {
		double d;
		switch (value.ValueType) {
			case PathValueType.Expression:
				DocumentObject p = (value as IPathExpression).SelectObject(source);
				if (p == null) {
					return double.NaN;
				}

				return double.TryParse(p.FriendlyValue ?? p.LiteralValue, out d) ? d : double.NaN;
			case PathValueType.String:
				return double.TryParse((value as PathStringValue).Value, out d) ? d : double.NaN;
			case PathValueType.Number:
				return (value as PathNumberValue).Value;
			case PathValueType.Boolean:
				return (value as PathBooleanValue).Value ? 1 : 0;
			default:
				return double.NaN;
		}
	}

	public static string ToString(DocumentObject source, IPathValue value) {
		switch (value.ValueType) {
			case PathValueType.Expression:
				DocumentObject p = (value as IPathExpression).SelectObject(source);
				if (p == null) {
					return string.Empty;
				}

				return p.FriendlyValue ?? p.LiteralValue ?? string.Empty;
			case PathValueType.String:
				return (value as PathStringValue).Value;
			case PathValueType.Number:
				return (value as PathNumberValue).Value.ToText();
			case PathValueType.Boolean:
				return (value as PathBooleanValue).ToString();
			default:
				return string.Empty;
		}
	}

	public static bool ToBoolean(DocumentObject source, IPathValue value) {
		return value.ValueType switch {
			PathValueType.Expression => (value as IPathExpression).SelectObject(source) != null,
			PathValueType.String => (value as PathStringValue).Value.Length > 0,
			PathValueType.Number => (value as PathNumberValue).Value != 0,
			PathValueType.Boolean => (value as PathBooleanValue).Value,
			_ => false
		};
	}


	private sealed class PathStringValue : IConstantPathValue
	{
		public PathStringValue(string value) {
			Value = value;
		}

		public string Value { get; }
		public PathValueType ValueType => PathValueType.String;

		public string LiteralValue => Value;
	}

	private sealed class PathNumberValue : IConstantPathValue
	{
		public PathNumberValue(double value) {
			Value = value;
		}

		public double Value { get; }
		public PathValueType ValueType => PathValueType.Number;

		public string LiteralValue => Value.ToText();
	}

	private sealed class PathBooleanValue : IConstantPathValue
	{
		public PathBooleanValue(bool value) {
			Value = value;
		}

		public bool Value { get; }
		public PathValueType ValueType => PathValueType.Boolean;

		public string LiteralValue => Value.ToString();
	}
}