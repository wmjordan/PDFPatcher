using System;
using System.Runtime.InteropServices;

namespace PDFPatcher.Processor.Imaging
{
	[UnmanagedFunctionPointer (CallingConvention.Cdecl, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = false)]
	public delegate int Jbig2ErrorCallback (System.IntPtr data, [InAttribute ()] [MarshalAsAttribute (UnmanagedType.LPStr)] string msg, Jbig2Severity severity, int seg_idx);

	public enum Jbig2Severity
	{
		JBIG2_SEVERITY_DEBUG,
		JBIG2_SEVERITY_INFO,
		JBIG2_SEVERITY_WARNING,
		JBIG2_SEVERITY_FATAL,
	}

	[StructLayout (LayoutKind.Sequential)]
	internal sealed class JBig2Ctx
	{
		IntPtr /*Jbig2Allocator **/allocator;
		int options;
		IntPtr /*const Jbig2Ctx **/global_ctx;
		Jbig2ErrorCallback error_callback;
		IntPtr /*void **/error_callback_data;
		IntPtr /*byte **/buf;
		int buf_size;
		int buf_rd_ix;
		int buf_wr_ix;

		int/*Jbig2FileState*/ state;

		byte file_header_flags;
		int n_pages;

		int n_segments_max;
		IntPtr /*Jbig2Segment ***/segments;
		int n_segments;	/* index of last segment header parsed */
		int segment_index;    /* index of last segment body parsed */

		/* list of decoded pages, including the one in progress,
		   currently stored as a contiguous, 0-indexed array. */
		int current_page;
		int max_page_index;
		IntPtr /*Jbig2Page **/pages;
	}

	[StructLayout (LayoutKind.Sequential)]
	internal sealed class JBig2Image
	{
		int Width;
		int Height;
		int Stride;
		IntPtr Data;
		int RefCount;
		public byte[] GetData () {
			var result = new byte[this.Height * this.Stride];
			Marshal.Copy (this.Data, result, 0, result.Length);
			return result;
		}
	}
}
