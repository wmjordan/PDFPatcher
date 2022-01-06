using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PDFPatcher.Common
{
	static class PInvokeHelper
	{
		/// <summary>
		/// 将 <paramref name="ptr"/> 指针对应的数据转换为 <typeparamref name="T"/> 类型实例。
		/// </summary>
		/// <typeparam name="T">传出类型实例。</typeparam>
		/// <param name="ptr">指向数据的指针。</param>
		/// <returns>指针封装后的托管实例。</returns>
		internal static T Unwrap<T>(this IntPtr ptr) where T : class, new() {
			var t = new T();
			Marshal.PtrToStructure(ptr, t);
			return t;
		}


	}
}
