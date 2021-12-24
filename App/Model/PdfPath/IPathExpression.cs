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
		DocumentObject SelectObject (DocumentObject source);
		IList<DocumentObject> SelectObjects (DocumentObject source);
	}

	public class PathExpression : IPathExpression
	{
		internal static readonly IList<DocumentObject> EmptyMatchResult = new DocumentObject[0];

		#region IPathExpression 成员
		public PathValueType ValueType { get { return PathValueType.Expression; } }

		public IPathAxis Axis { get; private set; }

		public string Name { get; private set; }

		private IList<IPathPredicate> _Predicates;
		///<summary>获取匹配条件列表。</summary>
		public IList<IPathPredicate> Predicates {
			get {
				if (_Predicates == null)
					_Predicates = new List<IPathPredicate> ();
				return _Predicates;
			}
		}

		public DocumentObject SelectObject (DocumentObject source) {
			return this.Axis.SelectObject (source, this.Name, this.Predicates);
		}

		public IList<DocumentObject> SelectObjects (DocumentObject source) {
			return this.Axis.SelectObjects (source, this.Name, this._Predicates);
		}

		#endregion

		public PathExpression (PathAxisType axis) {
			this.Axis = PathAxes.Create (axis);
		}
		public PathExpression (PathAxisType axis, string name) {
			this.Axis = PathAxes.Create (axis);
			this.Name = name;
		}

		
	}

}
