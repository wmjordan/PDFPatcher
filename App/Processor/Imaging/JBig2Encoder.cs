using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using FreeImageAPI;

namespace PDFPatcher.Processor.Imaging;

internal class JBig2Encoder
{
	private static readonly uint White = 0x00FFFFFF;

	internal static byte[] Encode(FreeImageBitmap fi) {
		bool zeroIsWhite = fi.HasPalette && fi.Palette.Data[0].uintValue == White;
		using (Bitmap bmp = fi.ToBitmap()) {
			BitmapData bits = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
				ImageLockMode.ReadOnly,
				PixelFormat.Format1bppIndexed);
			byte[] bytes = Encode(bmp.Width, bmp.Height, bits.Stride, zeroIsWhite, bits.Scan0);
			bmp.UnlockBits(bits);
			return bytes;
		}
	}

	private static byte[] Encode(int width, int height, int stride, bool zeroIsWhite, IntPtr b) {
		int l = 0;
		IntPtr r = NativeMethods.Encode(width, height, stride, zeroIsWhite, b, ref l);
		byte[] result = new byte[l];
		Marshal.Copy(r, result, 0, l);
		NativeMethods.Release(r);
		return result;
	}

	private class NativeMethods
	{
		[DllImport(JBig2Decoder.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "jbig2_encode")]
		internal static extern IntPtr Encode(int width, int height, int stride, bool zeroIsWhite, IntPtr data,
			ref int length);

		[DllImport(JBig2Decoder.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "jbig2_freemem")]
		internal static extern IntPtr Release(IntPtr data);
	}
}