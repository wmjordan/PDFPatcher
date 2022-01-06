using System;
using System.Runtime.InteropServices;

namespace PDFPatcher.Processor.Imaging
{
	internal static class JBig2Decoder
	{
		internal const string DLL = "jbig2.dll";
		const int JBIG2_OPTIONS_EMBEDDED = 1;

		internal static byte[] Decode(byte[] data, byte[] globals) {
			IntPtr ctxptr = IntPtr.Zero, globalptr = IntPtr.Zero;
			IntPtr imageptr;
			byte[] decodedData = null;
			int c;

			try {
				ctxptr = NativeMethods.New(IntPtr.Zero);
				if (globals != null && globals.Length > 0) {
					c = NativeMethods.ReadData(ctxptr, globals, (uint)globals.Length);
					globalptr = NativeMethods.MakeGlobal(ctxptr);
					ctxptr = NativeMethods.New(globalptr);
				}
				c = NativeMethods.ReadData(ctxptr, data, (uint)data.Length);
				c = NativeMethods.CompletePage(ctxptr);
				if ((imageptr = NativeMethods.Decode(ctxptr)) != IntPtr.Zero) {
					var image = Common.PInvokeHelper.Unwrap<JBig2Image>(imageptr);
					decodedData = image.GetData();
					NativeMethods.ReleasePage(ctxptr, imageptr);
				}
				return decodedData;
			}
			finally {
				if (globalptr != IntPtr.Zero) {
					NativeMethods.Free(globalptr);
				}
				if (ctxptr != IntPtr.Zero) {
					NativeMethods.Free(ctxptr);
				}
			}
		}

		class NativeMethods
		{
			internal static IntPtr New(IntPtr globalCtx) {
				return New(IntPtr.Zero, JBIG2_OPTIONS_EMBEDDED, globalCtx, null, IntPtr.Zero, 0, 19);
			}
			[DllImport(DLL, EntryPoint = "jbig2_ctx_new_imp", CallingConvention = CallingConvention.Cdecl)]
			extern static IntPtr New(IntPtr allocator, int options, IntPtr globalCtx, Jbig2ErrorCallback error_callback, IntPtr error_callback_data, int major, int minor);

			[DllImport(DLL, EntryPoint = "jbig2_ctx_free", CallingConvention = CallingConvention.Cdecl)]
			internal extern static IntPtr Free(IntPtr ctx);

			[DllImport(DLL, EntryPoint = "jbig2_complete_page", CallingConvention = CallingConvention.Cdecl)]
			internal extern static int CompletePage(IntPtr ctx);

			[DllImport(DLL, EntryPoint = "jbig2_data_in", CallingConvention = CallingConvention.Cdecl)]
			internal extern static int ReadData(IntPtr ctx, [InAttribute()] byte[] bytes, uint length);

			[DllImport(DLL, EntryPoint = "jbig2_make_global_ctx", CallingConvention = CallingConvention.Cdecl)]
			internal extern static IntPtr MakeGlobal(IntPtr ctx);

			[DllImport(DLL, EntryPoint = "jbig2_page_out", CallingConvention = CallingConvention.Cdecl)]
			internal extern static IntPtr Decode(IntPtr ctx);

			[DllImport(DLL, EntryPoint = "jbig2_release_page", CallingConvention = CallingConvention.Cdecl)]
			internal extern static void ReleasePage(IntPtr ctx, IntPtr image);

			static int ErrorCallback(IntPtr data, [InAttribute()] string msg, Jbig2Severity severity, int seg_idx) {
				System.Diagnostics.Debug.WriteLine(msg);
				return 0;
			}
		}

	}
}
