using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace PDFPatcher.Processor.Imaging
{
	static class BitmapHelper
	{
		public static ImageCodecInfo GetCodec(string codecName) {
			var ie = ImageCodecInfo.GetImageEncoders();
			for (int i = 0; i < ie.Length; i++) {
				var ic = ie[i];
				if (ic.MimeType == codecName) {
					return ic;
				}
			}
			return null;
		}

		/// <summary>
		/// 获取指定图片的不重复颜色集合。
		/// </summary>
		/// <param name="bmp">需要获取颜色集合的 <see cref="Bitmap"/>。</param>
		/// <returns>包含不重复颜色集合的列表。</returns>
		unsafe public static Color[] GetPalette(this Bitmap bmp) {
			var hs = new HashSet<int>();
			if (bmp == null) {
				return null;
			}
			if (bmp.IsIndexed()) {
				return Array.ConvertAll(bmp.Palette.Entries, c => c); //duplicates the array
			}
			if (bmp.PixelFormat != PixelFormat.Format24bppRgb && bmp.PixelFormat != PixelFormat.Format32bppArgb) {
				throw new InvalidOperationException("仅支持 Format24bppRgb 和 Format32bppArgb。");
			}
			BitmapData bmpData;
			int bw = bmp.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;
			byte* ps, pl;
			bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
			pl = (byte*)bmpData.Scan0;
			int w = bmp.Width, h = bmp.Height;
			for (int y = 0; y < h; y++) {
				ps = pl;
				if (bw == 3) {
					for (int x = 0; x < w; x++) {
						hs.Add((*ps) + (*(++ps) << 8) + (*(++ps) << 16) + (0xFF << 24));
						++ps;
					}
				}
				else if (bw == 4) {
					for (int x = 0; x < w; x++) {
						hs.Add((*ps) + ((*++ps) << 8) + (*(++ps) << 16) + (*(++ps) << 24));
						++ps;
					}
				}
				pl += bmpData.Stride;
			}
			bmp.UnlockBits(bmpData);
			var r = new Color[hs.Count];
			var i = 0;
			foreach (var item in hs.Select(Color.FromArgb)) {
				r[i++] = item;
			}
			return r;
		}

		/// <summary>
		/// 检查 <see cref="Image"/> 是否为索引调色板图像。
		/// </summary>
		/// <param name="image">需要检查的图像。</param>
		/// <returns>如为索引调色板图像，则返回 true，否则返回 false。</returns>
		public static bool IsIndexed(this Image image) {
			return (image.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed;
		}

		/// <summary>
		/// 锁定 <see cref="Bitmap"/> 的内容，用于读写。
		/// </summary>
		/// <param name="bmp">需要锁定的内容。</param>
		/// <param name="writable">是否可写入。</param>
		/// <returns>锁定后的 <see cref="BitmapData"/>。</returns>
		public static BitmapData LockBits(this Bitmap bmp, bool writable) {
			return bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), writable ? ImageLockMode.ReadWrite : ImageLockMode.ReadOnly, bmp.PixelFormat);
		}

		/// <summary>
		/// 调整 <paramref name="source"/> 的尺寸。
		/// </summary>
		/// <param name="source">需要调整尺寸的 <see cref="Image"/>。</param>
		/// <param name="size">新尺寸。</param>
		/// <param name="highQuality">是否采用插值方式调整尺寸。</param>
		/// <returns>调整后的新 <see cref="Bitmap"/>。</returns>
		public static Bitmap ResizeImage(this Image source, Size size, bool highQuality) {
			var b = new Bitmap(size.Width, size.Height);
			using (var g = Graphics.FromImage(b)) {
				if (highQuality) {
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				}
				g.DrawImage(source, 0, 0, size.Width, size.Height);
			}
			return b;
		}
		/// <summary>
		/// 按文件名的扩展名保存图像文件为对应的格式。
		/// </summary>
		/// <param name="image">需要保存的 <see cref="Image"/>。</param>
		/// <param name="fileName">保存的文件路径。</param>
		public static void SaveAs(this Image image, string fileName) {
			var ext = System.IO.Path.GetExtension(fileName);
			switch (ext.ToUpperInvariant()) {
				case ".PNG":
					image.Save(fileName, ImageFormat.Png); return;
				case ".BMP":
					image.Save(fileName, ImageFormat.Bmp); return;
				case ".JPG":
				case ".JPEG":
					image.Save(fileName, 75); return;
				case ".TIF":
				case ".TIFF":
					TiffHelper.SaveBinaryImage(image, fileName); return;
				case ".GIF":
					image.Save(fileName, ImageFormat.Gif); return;
				default:
					goto case ".PNG";
			}
		}
		public static void SaveAs(this Image image, string extension, System.IO.Stream stream) {
			switch (extension) {
				case ".PNG":
					image.Save(stream, ImageFormat.Png); return;
				case ".BMP":
					image.Save(stream, ImageFormat.Bmp); return;
				case ".JPG":
				case ".JPEG":
					image.Save(stream, 75); return;
				case ".TIF":
				case ".TIFF":
					TiffHelper.SaveBinaryImage(image, stream); return;
				case ".GIF":
					image.Save(stream, ImageFormat.Gif); return;
				default:
					goto case ".PNG";
			}
		}
		/// <summary>
		/// 将 <paramref name="tint"/> 颜色染色到 <paramref name="color"/> 上。
		/// </summary>
		/// <param name="color">基色。</param>
		/// <param name="tint">染色颜色。</param>
		/// <returns>染色后的新颜色。</returns>
		public static Color Tint(this Color color, Color tint) {
			return Color.FromArgb(color.A, mul255(color.R, tint.R), mul255(color.G, tint.G), mul255(color.B, tint.B));

			// MuPDF: pixmap.c
			static int mul255(int a, int b) {
				/* see Jim Blinn's book "Dirty Pixels" for how this works */
				int x = a * b + 128;
				x += x >> 8;
				return x >> 8;
			}
		}

		unsafe public static Bitmap ToIndexImage(this Bitmap source, Color[] pallette) {
			if (source == null) {
				return null;
			}
			if (source.PixelFormat != PixelFormat.Format24bppRgb && source.PixelFormat != PixelFormat.Format32bppArgb) {
				throw new InvalidOperationException("仅支持 Format24bppRgb 和 Format32bppArgb。");
			}
			var pi = new Dictionary<int, byte>(pallette.Length);
			for (int i = pallette.Length - 1; i >= 0; i--) {
				pi[pallette[i].ToArgb()] = (byte)i;
			}
			var result = new Bitmap(source.Width, source.Height, PixelFormat.Format8bppIndexed);
			var sourceData = source.LockBits(false);
			var targetData = result.LockBits(true);
			int bw = source.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;
			byte* ps, pr, src, res;
			src = (byte*)sourceData.Scan0;
			res = (byte*)targetData.Scan0;
			var rp = result.Palette;
			for (int i = 0; i < pallette.Length; i++) {
				rp.Entries[i] = pallette[i];
			}
			result.Palette = rp;
			int w = source.Width, h = source.Height;
			for (int y = 0; y < h; y++) {
				ps = src;
				pr = res;
				if (bw == 3) {
					for (int x = 0; x < w; x++) {
						*pr = pi[(*ps) + (*(++ps) << 8) + (*(++ps) << 16) + (0xFF << 24)];
						++pr;
						++ps;
					}
				}
				else if (bw == 4) {
					for (int x = 0; x < w; x++) {
						*pr = pi[(*ps) + ((*++ps) << 8) + (*(++ps) << 16) + (*(++ps) << 24)];
						++pr;
						++ps;
					}
				}
				src += sourceData.Stride;
				res += targetData.Stride;
			}
			source.UnlockBits(sourceData);
			result.UnlockBits(targetData);
			return result;
		}

		/// <summary>将图像转换为黑白图像。</summary>
		/// <param name="original">需要转换的图像。</param>
		/// <returns>转换后的图像。</returns>
		/// <remarks>http://www.wischik.com/lu/programmer/1bpp.html</remarks>
		public static Bitmap ToBitonal(this Bitmap original) {
			Bitmap source;

			if (original.PixelFormat == PixelFormat.Format1bppIndexed) {
				return (Bitmap)original.Clone();
			}
			else if (original.PixelFormat != PixelFormat.Format24bppRgb) {
				// If original bitmap is not already in 24 BPP, ARGB format, then convert
				// unfortunately Clone doesn't do this for us but returns a bitmap with the same pixel format
				// source = original.Clone( new Rectangle( Point.Empty, original.Size ), PixelFormat.Format24bppRgb );
				source = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
				source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
				using (Graphics g = Graphics.FromImage(source)) {
					//g.CompositingQuality = Drawing2D.CompositingQuality.GammaCorrected;
					//g.InterpolationMode = Drawing2D.InterpolationMode.Low;
					//g.SmoothingMode = Drawing2D.SmoothingMode.None;
					g.DrawImageUnscaled(original, 0, 0);
				}
			}
			else {
				source = original;
			}

			// Lock source bitmap in memory
			BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			// Copy image data to binary array
			int imageSize = sourceData.Stride * sourceData.Height;
			byte[] sourceBuffer = new byte[imageSize];
			Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);

			// Unlock source bitmap
			source.UnlockBits(sourceData);

			// Dispose of source if not originally supplied bitmap
			if (source != original) {
				source.Dispose();
			}

			// Create destination bitmap
			Bitmap destination = new Bitmap(sourceData.Width, sourceData.Height, PixelFormat.Format1bppIndexed);
			destination.SetResolution(original.HorizontalResolution, original.VerticalResolution);

			// Lock destination bitmap in memory
			BitmapData destinationData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

			// Create destination buffer
			byte[] destinationBuffer = SimpleThresholdBW(
				sourceBuffer,
				sourceData.Width,
				sourceData.Height,
				sourceData.Stride,
				destinationData.Stride);

			// Copy binary image data to destination bitmap
			Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, destinationData.Stride * sourceData.Height);

			// Unlock destination bitmap
			destination.UnlockBits(destinationData);

			// Return
			return destination;
		}

		public static byte[] SimpleThresholdBW(byte[] sourceBuffer, int width, int height, int srcStride, int dstStride) {
			const int THRESHOLD = 255 * 3 / 2;

			byte[] destinationBuffer = new byte[dstStride * height];
			int srcIx = 0;
			int dstIx = 0;
			byte bit;
			byte pix8;

			int newPixel, i, j;

			// Iterate lines
			for (int y = 0; y < height; y++, srcIx += srcStride, dstIx += dstStride) {
				bit = 128;
				i = srcIx;
				j = dstIx;
				pix8 = 0;
				// Iterate pixels
				for (int x = 0; x < width; x++, i += 3) {
					// Compute pixel brightness (i.e. total of Red, Green, and Blue values)
					newPixel = sourceBuffer[i] + sourceBuffer[i + 1] + sourceBuffer[i + 2];

					if (newPixel > THRESHOLD) {
						pix8 |= bit;
					}
					if (bit == 1) {
						destinationBuffer[j++] = pix8;
						bit = 128;
						pix8 = 0; // init next value with 0
					}
					else {
						bit >>= 1;
					}
				} // line finished
				if (bit != 128) {
					destinationBuffer[j] = pix8;
				}
			} // all lines finished
			return destinationBuffer;
		}
	}
}
