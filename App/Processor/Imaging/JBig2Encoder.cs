using System;
using System.Runtime.InteropServices;
using FreeImageAPI;
using System.Drawing.Imaging;

namespace PDFPatcher.Processor.Imaging
{
	static class JBig2Encoder
	{
		const uint White = 0x00FFFFFF;

		internal static byte[] Encode(FreeImageBitmap fi) {
			bool zeroIsWhite = fi.HasPalette && (fi.Palette.Data[0].uintValue & White) == White;
			using (var bmp = fi.ToBitmap()) {
				var bits = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
				var bytes = Encode(bmp.Width, bmp.Height, bits.Stride, zeroIsWhite, bits.Scan0);
				bmp.UnlockBits(bits);
				return bytes;
			}
		}

		static byte[] Encode(int width, int height, int stride, bool zeroIsWhite, IntPtr b) {
			int l = 0;
			var r = NativeMethods.Encode(width, height, stride, zeroIsWhite, b, ref l);
			var result = new byte[l];
			Marshal.Copy(r, result, 0, l);
			NativeMethods.Release(r);
			return result;
		}

		static class NativeMethods
		{
			[DllImport(JBig2Decoder.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "jbig2_encode")]
			internal static extern IntPtr Encode(int width, int height, int stride, bool zeroIsWhite, IntPtr data, ref int length);

			[DllImport(JBig2Decoder.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "jbig2_freemem")]
			internal static extern IntPtr Release(IntPtr data);
		}
	}
}
