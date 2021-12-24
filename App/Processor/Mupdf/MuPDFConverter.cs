using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MuPDFLib
{
	public static class MuPdfConverter
	{
		public static byte[] ConvertPdfToTiff (byte[] image, float dpi, RenderType type, bool rotateLandscapePages, int maxSizeInPdfPixels, string pdfPassword) {
			byte[] output = null;

			if (image == null)
				throw new ArgumentNullException ("image");

			using (MuPDF pdfDoc = new MuPDF (image, pdfPassword)) {
				using (MemoryStream outputStream = new MemoryStream ()) {
					ImageCodecInfo info = null;
					foreach (ImageCodecInfo ice in ImageCodecInfo.GetImageEncoders ())
						if (ice.MimeType == "image/tiff")
							info = ice;

					Bitmap saveTif = null;
					for (int i = 1; i <= pdfDoc.PageCount; i++) {
						int Width = 0;//Zero for no resize.
						int Height = 0;//Zero for autofit height to width.

						pdfDoc.Page = i;

						Bitmap FirstImage = pdfDoc.GetBitmap (Width, Height, dpi, dpi, 0, type, rotateLandscapePages, maxSizeInPdfPixels);
						if (FirstImage == null)
							throw new Exception ("Unable to convert pdf to tiff!");
						using (EncoderParameters ep = new EncoderParameters (2)) {
							ep.Param[0] = new EncoderParameter (Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
							ep.Param[1] = new EncoderParameter (Encoder.Compression, (long)EncoderValue.CompressionLZW);
							if (type == RenderType.Monochrome) {
								ep.Param[1] = new EncoderParameter (Encoder.Compression, (long)EncoderValue.CompressionCCITT4);
							}

							if (i == 1) {
								saveTif = FirstImage;
								saveTif.Save (outputStream, info, ep);
							}
							else {
								ep.Param[0] = new EncoderParameter (Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
								saveTif.SaveAdd (FirstImage, ep);
								FirstImage.Dispose ();
							}
							if (i == pdfDoc.PageCount) {
								ep.Param[0] = new EncoderParameter (Encoder.SaveFlag, (long)EncoderValue.Flush);
								saveTif.SaveAdd (ep);
								saveTif.Dispose ();
							}
						}
					}
					output = outputStream.ToArray ();
				}
			}
			return output;
		}

		public static bool ConvertPdfToTiff (string sourceFile, string outputFile, float dpi, RenderType type, bool rotateLandscapePages, int maxSizeInPdfPixels, string pdfPassword) {
			bool output = false;

			if (string.IsNullOrEmpty (sourceFile) || string.IsNullOrEmpty (outputFile))
				throw new ArgumentNullException ();

			using (MuPDF pdfDoc = new MuPDF (sourceFile, pdfPassword)) {
				using (FileStream outputStream = new FileStream (outputFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
					ImageCodecInfo info = null;
					foreach (ImageCodecInfo ice in ImageCodecInfo.GetImageEncoders ())
						if (ice.MimeType == "image/tiff")
							info = ice;

					Bitmap saveTif = null;
					for (int i = 1; i <= pdfDoc.PageCount; i++) {
						pdfDoc.Page = i;

						Bitmap FirstImage = pdfDoc.GetBitmap (0, 0, dpi, dpi, 0, type, rotateLandscapePages, maxSizeInPdfPixels);
						if (FirstImage == null)
							throw new Exception ("Unable to convert pdf to tiff!");

						using (EncoderParameters ep = new EncoderParameters (2)) {
							ep.Param[0] = new EncoderParameter (Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
							ep.Param[1] = new EncoderParameter (Encoder.Compression, (long)EncoderValue.CompressionLZW);
							if (type == RenderType.Monochrome) {
								ep.Param[1] = new EncoderParameter (Encoder.Compression, (long)EncoderValue.CompressionCCITT4);
							}

							if (i == 1) {
								saveTif = FirstImage;
								saveTif.Save (outputStream, info, ep);
							}
							else {
								ep.Param[0] = new EncoderParameter (Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
								saveTif.SaveAdd (FirstImage, ep);
								FirstImage.Dispose ();
							}
							if (i == pdfDoc.PageCount) {
								ep.Param[0] = new EncoderParameter (Encoder.SaveFlag, (long)EncoderValue.Flush);
								saveTif.SaveAdd (ep);
								saveTif.Dispose ();
							}
						}
					}
				}
				if (File.Exists (outputFile))
					output = true;
			}
			return output;
		}

		public static byte[] ConvertPdfToFaxTiff (byte[] image, float dpi, string pdfPassword) {
			byte[] output = null;
			const long Compression = (long)EncoderValue.CompressionCCITT4;

			using (MuPDF pdfDoc = new MuPDF (image, pdfPassword)) {
				using (MemoryStream outputStream = new MemoryStream ()) {
					ImageCodecInfo info = null;
					foreach (ImageCodecInfo ice in ImageCodecInfo.GetImageEncoders ())
						if (ice.MimeType == "image/tiff")
							info = ice;

					Bitmap saveTif = null;
					for (int i = 1; i <= pdfDoc.PageCount; i++) {
						int Width = 0;//Zero for no resize.
						//int Height = 0;//Zero for autofit height to width.
						float DpiX = dpi;
						float DpiY = dpi;

						pdfDoc.Page = i;

						if (dpi == 200) {
							Width = 1728;
							DpiX = 204;
							DpiY = 196;
						}
						else if (dpi == 300)
							Width = 2592;
						else if (dpi == 400)
							Width = 3456;

						Bitmap FirstImage = pdfDoc.GetBitmap (Width, 0, DpiX, DpiY, 0, RenderType.Monochrome, true, 0);
						if (FirstImage == null)
							throw new Exception ("Unable to convert pdf to tiff!");

						using (EncoderParameters ep = new EncoderParameters (2)) {
							ep.Param[0] = new EncoderParameter (Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
							ep.Param[1] = new EncoderParameter (Encoder.Compression, Compression);

							if (i == 1) {
								saveTif = FirstImage;
								saveTif.Save (outputStream, info, ep);
							}
							else {
								ep.Param[0] = new EncoderParameter (Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
								saveTif.SaveAdd (FirstImage, ep);
								FirstImage.Dispose ();
							}
							if (i == pdfDoc.PageCount) {
								ep.Param[0] = new EncoderParameter (Encoder.SaveFlag, (long)EncoderValue.Flush);
								saveTif.SaveAdd (ep);
								saveTif.Dispose ();
							}
						}
					}
					output = outputStream.ToArray ();
				}
			}
			return output;
		}

		public static bool ConvertPdfToFaxTiff (string sourceFile, string outputFile, float dpi, string pdfPassword) {
			bool output = false;
			const long Compression = (long)EncoderValue.CompressionCCITT4;

			using (MuPDF pdfDoc = new MuPDF (sourceFile, pdfPassword)) {
				using (FileStream outputStream = new FileStream (outputFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
					ImageCodecInfo info = null;
					foreach (ImageCodecInfo ice in ImageCodecInfo.GetImageEncoders ())
						if (ice.MimeType == "image/tiff")
							info = ice;

					Bitmap saveTif = null;
					for (int i = 1; i <= pdfDoc.PageCount; i++) {
						int Width = 0;//Zero for no resize.
						//int Height = 0;//Zero for autofit height to width.
						float DpiX = dpi;
						float DpiY = dpi;

						pdfDoc.Page = i;

						if (dpi == 200) {
							Width = 1728;
							DpiX = 204;
							DpiY = 196;
						}
						else if (dpi == 300)
							Width = 2592;
						else if (dpi == 400)
							Width = 3456;

						Bitmap FirstImage = pdfDoc.GetBitmap (Width, 0, DpiX, DpiY, 0, RenderType.Monochrome, true, 0);
						if (FirstImage == null)
							throw new Exception ("Unable to convert pdf to tiff!");
						using (EncoderParameters ep = new EncoderParameters (2)) {
							ep.Param[0] = new EncoderParameter (Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
							ep.Param[1] = new EncoderParameter (Encoder.Compression, Compression);

							if (i == 1) {
								saveTif = FirstImage;
								saveTif.Save (outputStream, info, ep);
							}
							else {
								ep.Param[0] = new EncoderParameter (Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
								saveTif.SaveAdd (FirstImage, ep);
								FirstImage.Dispose ();
							}
							if (i == pdfDoc.PageCount) {
								ep.Param[0] = new EncoderParameter (Encoder.SaveFlag, (long)EncoderValue.Flush);
								saveTif.SaveAdd (ep);
								saveTif.Dispose ();
							}
						}
					}
				}
				if (File.Exists (outputFile))
					output = true;
			}
			return output;
		}
	}
}
