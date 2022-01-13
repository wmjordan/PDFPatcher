using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model.PdfPath
{
	interface IPathFunction
	{
		object Evaluate(DocumentObject source);
	}

	sealed class CurrentPosition : IPathFunction
	{
		#region IPathFunction 成员

		public object Evaluate(DocumentObject source) {
			if (source == null || source.Parent == null) {
				return 0;
			}

			int i = 0;
			foreach (var item in source.Parent.Children) {
				++i;
				if (item == source) {
					return i;
				}
			}

			return i;
		}

		#endregion
	}
}