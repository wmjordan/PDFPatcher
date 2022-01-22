using System;
using System.Runtime.InteropServices;

namespace PDFPatcher.Processor.Imaging;

[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = false)]
public delegate int Jbig2ErrorCallback(IntPtr data, [InAttribute][MarshalAsAttribute(UnmanagedType.LPStr)] string msg,
	Jbig2Severity severity, int seg_idx);

public enum Jbig2Severity
{
}

[StructLayout(LayoutKind.Sequential)]
internal sealed class JBig2Image
{
	private readonly IntPtr Data;
	private readonly int Height;
	private readonly int Stride;

	public byte[] GetData() {
		byte[] result = new byte[Height * Stride];
		Marshal.Copy(Data, result, 0, result.Length);
		return result;
	}
}