using System;
using System.Collections.Generic;
using System.Text;
using PDFPatcher.Model;

namespace PDFPatcher.Model.PdfPath
{
	public interface IPathExpression : IPathValue
	{
		IList<IPathPredicate> Predicates { get; }
		IPathAxis Axis { get; }
		string Name { get; }
		DocumentObject SelectObject(DocumentObject source);
		IList<DocumentObject> SelectObjects(DocumentObject source);
	}

	public class PathExpression : IPathExpression
	{
		internal static readonly IList<DocumentObject> EmptyMatchResult = new DocumentObject[0];

		#region IPathExpression 成员

		public PathValueType ValueType => PathValueType.Expression;

		public IPathAxis Axis { get; private set; }

		public string Name { get; private set; }

		private IList<IPathPredicate> _Predicates;

		///<summary>获取匹配条件列表。</summary>
		public IList<IPathPredicate> Predicates {
			get {
				if (_Predicates == null)
					_Predicates = new List<IPathPredicate>();
				return _Predicates;
			}
		}

		public DocumentObject SelectObject(DocumentObject source) {
			return Axis.SelectObject(source, Name, Predicates);
		}

		public IList<DocumentObject> SelectObjects(DocumentObject source) {
			return Axis.SelectObjects(source, Name, _Predicates);
		}

		#endregion

		public PathExpression(PathAxisType axis) {
			Axis = PathAxes.Create(axis);
		}

		public PathExpression(PathAxisType axis, string name) {
			Axis = PathAxes.Create(axis);
			Name = name;
		}
	}
}