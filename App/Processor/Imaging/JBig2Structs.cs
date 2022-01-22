using System;
using System.Runtime.InteropServices;

namespace PDFPatcher.Processor.Imaging
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = false)]
	public delegate int Jbig2ErrorCallback(System.IntPtr data, [InAttribute()][MarshalAsAttribute(UnmanagedType.LPStr)] string msg, Jbig2Severity severity, int seg_idx);

	public enum Jbig2Severity
	{
		JBIG2_SEVERITY_DEBUG,
		JBIG2_SEVERITY_INFO,
		JBIG2_SEVERITY_WARNING,
		JBIG2_SEVERITY_FATAL,
	}

	[StructLayout(LayoutKind.Sequential)]
	internal sealed class JBig2Ctx
	{
		/* list of decoded pages, including the one in progress,
		   currently stored as a contiguous, 0-indexed array. */
	}

	[StructLayout(LayoutKind.Sequential)]
	internal sealed class JBig2Image
	{
		readonly int Height;
		readonly int Stride;
		readonly IntPtr Data;

		public byte[] GetData() {
			var result = new byte[Height * Stride];
			Marshal.Copy(Data, result, 0, result.Length);
			return result;
		}
	}
}
