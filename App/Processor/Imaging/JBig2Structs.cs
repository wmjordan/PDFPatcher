using System;
using System.Runtime.InteropServices;

namespace PDFPatcher.Processor.Imaging
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = false)]
	public delegate int Jbig2ErrorCallback(System.IntPtr data,
		[InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string msg, Jbig2Severity severity, int seg_idx);

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
		readonly IntPtr /*Jbig2Allocator **/
			allocator;

		readonly int options;

		readonly IntPtr /*const Jbig2Ctx **/
			global_ctx;

		readonly Jbig2ErrorCallback error_callback;

		readonly IntPtr /*void **/
			error_callback_data;

		readonly IntPtr /*byte **/
			buf;

		readonly int buf_size;
		readonly int buf_rd_ix;
		readonly int buf_wr_ix;

		readonly int /*Jbig2FileState*/
			state;

		readonly byte file_header_flags;
		readonly int n_pages;
		readonly int n_segments_max;

		readonly IntPtr /*Jbig2Segment ***/
			segments;

		readonly int n_segments; /* index of last segment header parsed */
		readonly int segment_index; /* index of last segment body parsed */

		/* list of decoded pages, including the one in progress,
		   currently stored as a contiguous, 0-indexed array. */
		readonly int current_page;
		readonly int max_page_index;

		readonly IntPtr /*Jbig2Page **/
			pages;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal sealed class JBig2Image
	{
		readonly int Width;
		readonly int Height;
		readonly int Stride;
		readonly IntPtr Data;
		readonly int RefCount;

		public byte[] GetData() {
			var result = new byte[Height * Stride];
			Marshal.Copy(Data, result, 0, result.Length);
			return result;
		}
	}
}