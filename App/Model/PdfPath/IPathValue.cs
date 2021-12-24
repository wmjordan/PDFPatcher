using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model.PdfPath
{
	public interface IPathValue
	{
		PathValueType ValueType { get; }
	}

	public interface IConstantPathValue : IPathValue
	{
		string LiteralValue { get; }
	}

}
