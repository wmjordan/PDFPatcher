using System;
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
				try {
					return MuPDF.JBig2Codec.LosslessEncode(bmp.Width, bmp.Height, bits.Stride, zeroIsWhite, bits.Scan0);
				}
				finally {
					bmp.UnlockBits(bits);
				}
			}
		}
	}
}
