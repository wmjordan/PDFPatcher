using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model.PdfPath
{
	public interface IPathPredicate
	{
		PredicateOperatorType Operator { get; }
		IPathExpression Operand1 { get; }
		IPathExpression Operand2 { get; }
		bool Match(DocumentObject source, IPathValue value1, IPathValue value2);
	}

	abstract class PathPredicate : IPathPredicate
	{
		#region IPathPredicate 成员

		public abstract PredicateOperatorType Operator { get; }

		public IPathExpression Operand1 { get; internal set; }

		public IPathExpression Operand2 { get; internal set; }

		public abstract bool Match(DocumentObject source, IPathValue value1, IPathValue value2);

		#endregion

		protected PathPredicate(IPathExpression operand1, IPathExpression operand2) {
			Operand1 = operand1;
			Operand2 = operand2;
		}

		sealed class ContainmentPredicate : PathPredicate
		{
			public ContainmentPredicate(IPathExpression operand1, IPathExpression operand2)
				: base(operand1, operand2) {
			}

			public override PredicateOperatorType Operator => PredicateOperatorType.Contains;

			public override bool Match(DocumentObject source, IPathValue value1, IPathValue value2) {
				if (value1.ValueType == PathValueType.Expression) {
					var exp = value1 as IPathExpression;
					return exp.SelectObjects(source).Count > 0;
				}

				return false;
			}
		}

		class EqualityPredicate : PathPredicate
		{
			public override PredicateOperatorType Operator => PredicateOperatorType.Equal;

			public EqualityPredicate(IPathExpression operand1, IPathExpression operand2)
				: base(operand1, operand2) {
			}

			public override bool Match(DocumentObject source, IPathValue value1, IPathValue value2) {
				if (value1.ValueType == PathValueType.Expression) {
					var exp = value1 as IPathExpression;
					var cv = value2 as IConstantPathValue;
					string v;
					if (cv != null) {
						v = cv.LiteralValue;
					}
					else {
						var o = (value2 as IPathExpression).SelectObject(source);
						v = o != null ? (o.FriendlyValue ?? o.LiteralValue) : String.Empty;
					}

					foreach (var item in exp.SelectObjects(source)) {
						if ((item.FriendlyValue ?? item.LiteralValue) == v) {
							return true;
						}
					}

					return false;
				}
				else if (value1.ValueType == PathValueType.Number || value2.ValueType == PathValueType.Number) {
					return PathValue.ToNumber(source, value1) == PathValue.ToNumber(source, value2);
				}
				else if (value1.ValueType == PathValueType.String || value2.ValueType == PathValueType.String) {
					return PathValue.ToString(source, value1) == PathValue.ToString(source, value2);
				}
				else {
					return PathValue.ToBoolean(source, value1) == PathValue.ToBoolean(source, value2);
				}
			}
		}

		sealed class InequalityPredicate : EqualityPredicate
		{
			public InequalityPredicate(IPathExpression operand1, IPathExpression operand2)
				: base(operand1, operand2) {
			}

			public override PredicateOperatorType Operator => PredicateOperatorType.NotEqual;

			public override bool Match(DocumentObject source, IPathValue value1, IPathValue value2) {
				return !base.Match(source, value1, value2);
			}
		}
	}
}