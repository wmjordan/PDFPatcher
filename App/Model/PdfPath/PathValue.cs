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
			switch (value.ValueType) {
				case PathValueType.Expression:
					return (value as IPathExpression).SelectObject(source) != null;
				case PathValueType.String:
					return (value as PathStringValue).Value.Length > 0;
				case PathValueType.Number:
					return (value as PathNumberValue).Value != 0;
				case PathValueType.Boolean:
					return (value as PathBooleanValue).Value;
				default:
					return false;
			}
		}


		sealed class PathStringValue : IConstantPathValue
		{
			public PathValueType ValueType => PathValueType.String;

			public string Value { get; private set; }

			public string LiteralValue => Value;

			public PathStringValue(string value) {
				Value = value;
			}
		}

		sealed class PathNumberValue : IConstantPathValue
		{
			public PathValueType ValueType => PathValueType.Number;

			public double Value { get; private set; }

			public string LiteralValue => Value.ToText();

			public PathNumberValue(double value) {
				Value = value;
			}
		}

		sealed class PathBooleanValue : IConstantPathValue
		{
			public PathValueType ValueType => PathValueType.Boolean;

			public bool Value { get; private set; }

			public string LiteralValue => Value.ToString();

			public PathBooleanValue(bool value) {
				Value = value;
			}
		}

	}
}
