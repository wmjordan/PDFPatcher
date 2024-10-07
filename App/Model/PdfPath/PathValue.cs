using System;
using PDFPatcher.Common;

namespace PDFPatcher.Model.PdfPath
{
	public static class PathValue
	{
		public static double ToNumber(DocumentObject source, IPathValue value) {
			double d;
			switch (value.ValueType) {
				case PathValueType.Expression:
					var p = (value as IPathExpression).SelectObject(source);
					if (p == null) {
						return Double.NaN;
					}
					return Double.TryParse(p.FriendlyValue ?? p.LiteralValue, out d) ? d : Double.NaN;
				case PathValueType.String:
					return Double.TryParse((value as PathStringValue).Value, out d) ? d : Double.NaN;
				case PathValueType.Number:
					return (value as PathNumberValue).Value;
				case PathValueType.Boolean:
					return (value as PathBooleanValue).Value ? 1 : 0;
				default:
					return Double.NaN;
			}
		}

		public static string ToString(DocumentObject source, IPathValue value) {
			switch (value.ValueType) {
				case PathValueType.Expression:
					var p = (value as IPathExpression).SelectObject(source);
					if (p == null) {
						return String.Empty;
					}
					return p.FriendlyValue ?? p.LiteralValue ?? String.Empty;
				case PathValueType.String:
					return (value as PathStringValue).Value;
				case PathValueType.Number:
					return (value as PathNumberValue).Value.ToText();
				case PathValueType.Boolean:
					return (value as PathBooleanValue).ToString();
				default:
					return String.Empty;
			}
		}

		public static bool ToBoolean(DocumentObject source, IPathValue value) {
			return value.ValueType switch {
				PathValueType.Expression => ((IPathExpression)value).SelectObject(source) != null,
				PathValueType.String => ((PathStringValue)value).Value.Length > 0,
				PathValueType.Number => ((PathNumberValue)value).Value != 0,
				PathValueType.Boolean => ((PathBooleanValue)value).Value,
				_ => false,
			};
		}

		sealed class PathStringValue(string value) : IConstantPathValue
		{
			public PathValueType ValueType => PathValueType.String;

			public string Value { get; } = value;

			public string LiteralValue => Value;
		}

		sealed class PathNumberValue(double value) : IConstantPathValue
		{
			public PathValueType ValueType => PathValueType.Number;

			public double Value { get; } = value;

			public string LiteralValue => Value.ToText();
		}

		sealed class PathBooleanValue(bool value) : IConstantPathValue
		{
			public PathValueType ValueType => PathValueType.Boolean;

			public bool Value { get; } = value;

			public string LiteralValue => Value.ToString();
		}
	}
}
