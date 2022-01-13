using System;
using System.Runtime.InteropServices;

namespace PDFPatcher.Processor.Imaging;

[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = false)]
public delegate int Jbig2ErrorCallback(IntPtr data,
	[InAttribute] [MarshalAsAttribute(UnmanagedType.LPStr)]
	string msg, Jbig2Severity severity, int seg_idx);

public enum Jbig2Severity
{
	JBIG2_SEVERITY_DEBUG,
	JBIG2_SEVERITY_INFO,
	JBIG2_SEVERITY_WARNING,
	JBIG2_SEVERITY_FATAL
}

[StructLayout(LayoutKind.Sequential)]
internal sealed class JBig2Ctx
{
	private readonly IntPtr /*Jbig2Allocator **/
		allocator;

	private readonly IntPtr /*byte **/
		buf;

	private readonly int buf_rd_ix;

	private readonly int buf_size;
	private readonly int buf_wr_ix;

	/* list of decoded pages, including the one in progress,
	   currently stored as a contiguous, 0-indexed array. */
	private readonly int current_page;

	private readonly Jbig2ErrorCallback error_callback;

	private readonly IntPtr /*void **/
		error_callback_data;

	private readonly byte file_header_flags;

	private readonly IntPtr /*const Jbig2Ctx **/
		global_ctx;

	private readonly int max_page_index;
	private readonly int n_pages;

	private readonly int n_segments; /* index of last segment header parsed */
	private readonly int n_segments_max;

	private readonly int options;

	private readonly IntPtr /*Jbig2Page **/
		pages;

	private readonly int segment_index; /* index of last segment body parsed */

	private readonly IntPtr /*Jbig2Segment ***/
		segments;

	private readonly int /*Jbig2FileState*/
		state;
}

[StructLayout(LayoutKind.Sequential)]
internal sealed class JBig2Image
{
	private readonly IntPtr Data;
	private readonly int Height;
	private readonly int RefCount;
	private readonly int Stride;
	private readonly int Width;

	public byte[] GetData() {
		byte[] result = new byte[Height * Stride];
		Marshal.Copy(Data, result, 0, result.Length);
		return result;
	}
}