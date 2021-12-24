using System.Drawing.Imaging;

namespace PDFPatcher.Processor.Imaging
{
	static class JpgHelper
	{
		static ImageCodecInfo _jpgCodec = BitmapHelper.GetCodec ("image/jpeg");
		static EncoderParameters GetEncoderParameters (int quality) {
			return new EncoderParameters (2) {
				Param = new EncoderParameter[] {
					new EncoderParameter (Encoder.Compression, (long)EncoderValue.RenderProgressive),
					new EncoderParameter (Encoder.Quality, quality)
				}
			};
		}
		// JPEG 编码器不支持 8 位图像输出
		static EncoderParameters GetEncoderParameters (int quality, int colorDepth) {
			return new EncoderParameters (3) {
				Param = new EncoderParameter[] {
					new EncoderParameter (Encoder.Compression, (long)EncoderValue.RenderProgressive),
					new EncoderParameter (Encoder.Quality, quality),
					new EncoderParameter (Encoder.ColorDepth, colorDepth)
				}
			};
		}

		internal static void Save (this System.Drawing.Image bmp, string fileName, int quality) {
			//if (bmp.IsIndexed ()) {
			//    bmp.Save (fileName, _jpgCodec, GetEncoderParameters (quality, 8));
			//}
			//else {
			using (var p = GetEncoderParameters (quality)) {
				bmp.Save (fileName, _jpgCodec, p);
				foreach (var item in p.Param) {
					item.Dispose ();
				}
			}
			//}
		}
	}
}
