using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Processor
{
	interface IProcessor
	{
		/// <summary>
		/// 返回处理器的名称。
		/// </summary>
		string Name { get; }
	}
}