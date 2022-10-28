using System.Drawing.Imaging;
using FreeImageAPI;

namespace PDFPatcher.Processor.Imaging
{
	static class TiffHelper
	{
		static readonly ImageCodecInfo _tiffCodec = BitmapHelper.GetCodec("image/tiff");
		static readonly EncoderParameters _encoderParameters = new EncoderParameters(1) {
			Param = new EncoderParameter[] { new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionCCITT4) }
		};

		internal static void Save(FreeImageBitmap bmp, string fileName) {
			// 使用 .NET 的 TIFF 保存方式，文件尺寸较小
			if (_tiffCodec != null) {
				if (bmp.ColorType == FREE_IMAGE_COLOR_TYPE.FIC_MINISWHITE) {
					// HACK: TIFF编码黑色为1，解决 .NET TIFF 编码器无法正常保存双色图片的问题
					bmp.Invert();
				}
				using (var b = bmp.ToBitmap()) {
					b.Save(fileName, _tiffCodec, _encoderParameters);
				}
			}
			else {
				bmp.Save(fileName, FREE_IMAGE_FORMAT.FIF_TIFF, FREE_IMAGE_SAVE_FLAGS.TIFF_CCITTFAX4);
			}
		}

		/// <summary>
		/// 将图片保存为黑白双色图片。如图片的 <see cref="PixelFormat"/> 不为 <see cref="PixelFormat.Format1bppIndexed"/>，则按默认格式保存。
		/// </summary>
		/// <param name="bmp">要保存的图片。</param>
		/// <param name="fileName">保存路径。</param>
		internal static void SaveBinaryImage(this System.Drawing.Image bmp, string fileName) {
			if (bmp.PixelFormat == PixelFormat.Format1bppIndexed) {
				bmp.Save(fileName, _tiffCodec, _encoderParameters);
			}
			else {
				bmp.Save(fileName, ImageFormat.Tiff);
			}
		}
		internal static void SaveBinaryImage(this System.Drawing.Image bmp, System.IO.Stream stream) {
			if (bmp.PixelFormat == PixelFormat.Format1bppIndexed) {
				bmp.Save(stream, _tiffCodec, _encoderParameters);
			}
			else {
				bmp.Save(stream, ImageFormat.Tiff);
			}
		}

		internal static byte[] Decode(ImageInfo info, byte[] bytes, int k, bool endOfLine, bool encodedByteAlign, bool endOfBlock, bool blackIs1) {
			using (var s = new MuPdfSharp.MuStream(bytes))
			using (var img = s.DecodeTiffFax(info.Width, info.Height, k, endOfLine, encodedByteAlign, endOfBlock, blackIs1)) {
				return img.ReadAll(bytes.Length);
			}
			//var outBuf = new byte[(info.Width + 7) / 8 * info.Height];
			//var decoder = new TIFFFaxDecoder (1, info.Width, info.Height);
			//if (k < 0) {
			//    // CCITT Fax Group 4
			//    decoder.DecodeT6 (outBuf, bytes, 0, info.Height, 0L);
			//}
			//else if (k == 0) {
			//    // CCITT Fax Group 3 (1-D)
			//    decoder.Decode1D (outBuf, bytes, 0, info.Height);
			//}
			//else {
			//    // CCITT Fax Group 3 (2-D)
			//    decoder.Decode2D (outBuf, bytes, 0, info.Height, 0L);
			//}
			//return outBuf;
		}
	}
}
