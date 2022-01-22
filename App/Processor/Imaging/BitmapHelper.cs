using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace PDFPatcher.Processor.Imaging;

internal static class BitmapHelper
{
	private const int threshold = 255 * 3 / 2;

	public static ImageCodecInfo GetCodec(string codecName) {
		ImageCodecInfo[] ie = ImageCodecInfo.GetImageEncoders();

		foreach (var t in ie) {
			if (t.MimeType == codecName) {
				return t;
			}
		}

		return null;
	}

	/// <summary>
	///     获取指定图片的不重复颜色集合。
	/// </summary>
	/// <param name="bmp">需要获取颜色集合的 <see cref="Bitmap" />。</param>
	/// <returns>包含不重复颜色集合的列表。</returns>
	public static unsafe Color[] GetPalette(this Bitmap bmp) {
		HashSet<int> hs = new();
		if (bmp == null) {
			return null;
		}

		if (bmp.IsIndexed()) {
			return Array.ConvertAll(bmp.Palette.Entries, c => c); //duplicates the array
		}

		if (bmp.PixelFormat != PixelFormat.Format24bppRgb && bmp.PixelFormat != PixelFormat.Format32bppArgb) {
			throw new InvalidOperationException("仅支持 Format24bppRgb 和 Format32bppArgb。");
		}

		int bw = bmp.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;
		BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
			bmp.PixelFormat);
		byte* pl = (byte*)bmpData.Scan0;
		int w = bmp.Width, h = bmp.Height;
		for (int y = 0; y < h; y++) {
			byte* ps = pl;
			switch (bw) {
				case 3: {
						for (int x = 0; x < w; x++) {
							hs.Add(*ps + (*++ps << 8) + (*++ps << 16) + (0xFF << 24));
							++ps;
						}

						break;
					}
				case 4: {
						for (int x = 0; x < w; x++) {
							hs.Add(*ps + (*++ps << 8) + (*++ps << 16) + (*++ps << 24));
							++ps;
						}

						break;
					}
			}

			pl += bmpData.Stride;
		}

		bmp.UnlockBits(bmpData);
		Color[] r = new Color[hs.Count];
		int i = 0;
		foreach (Color item in hs.Select(Color.FromArgb)) {
			r[i++] = item;
		}

		return r;
	}

	/// <summary>
	///     检查 <see cref="Image" /> 是否为索引调色板图像。
	/// </summary>
	/// <param name="image">需要检查的图像。</param>
	/// <returns>如为索引调色板图像，则返回 true，否则返回 false。</returns>
	public static bool IsIndexed(this Image image) {
		return (image.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed;
	}

	/// <summary>
	///     锁定 <see cref="Bitmap" /> 的内容，用于读写。
	/// </summary>
	/// <param name="bmp">需要锁定的内容。</param>
	/// <param name="writable">是否可写入。</param>
	/// <returns>锁定后的 <see cref="BitmapData" />。</returns>
	public static BitmapData LockBits(this Bitmap bmp, bool writable) {
		return bmp.LockBits(new Rectangle(Point.Empty, bmp.Size),
			writable ? ImageLockMode.ReadWrite : ImageLockMode.ReadOnly, bmp.PixelFormat);
	}

	/// <summary>
	///     调整 <paramref name="source" /> 的尺寸。
	/// </summary>
	/// <param name="source">需要调整尺寸的 <see cref="Image" />。</param>
	/// <param name="size">新尺寸。</param>
	/// <param name="highQuality">是否采用插值方式调整尺寸。</param>
	/// <returns>调整后的新 <see cref="Bitmap" />。</returns>
	public static Bitmap ResizeImage(this Image source, Size size, bool highQuality) {
		Bitmap b = new(size.Width, size.Height);
		using Graphics g = Graphics.FromImage(b);
		if (highQuality) {
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		}

		g.DrawImage(source, 0, 0, size.Width, size.Height);

		return b;
	}

	/// <summary>
	///     按文件名的扩展名保存图像文件为对应的格式。
	/// </summary>
	/// <param name="image">需要保存的 <see cref="Image" />。</param>
	/// <param name="fileName">保存的文件路径。</param>
	public static void SaveAs(this Image image, string fileName) {
		string ext = Path.GetExtension(fileName);
		switch (ext.ToUpperInvariant()) {
			case ".PNG":
				image.Save(fileName, ImageFormat.Png);
				return;
			case ".BMP":
				image.Save(fileName, ImageFormat.Bmp);
				return;
			case ".JPG":
			case ".JPEG":
				image.Save(fileName, 75);
				return;
			case ".TIF":
			case ".TIFF":
				image.SaveBinaryImage(fileName);
				return;
			case ".GIF":
				image.Save(fileName, ImageFormat.Gif);
				return;
			default:
				goto case ".PNG";
		}
	}

	/// <summary>
	///     将 <paramref name="tint" /> 颜色染色到 <paramref name="color" /> 上。
	/// </summary>
	/// <param name="color">基色。</param>
	/// <param name="tint">染色颜色。</param>
	/// <returns>染色后的新颜色。</returns>
	public static Color Tint(this Color color, Color tint) {
		return Color.FromArgb(color.A, mul255(color.R, tint.R), mul255(color.G, tint.G), mul255(color.B, tint.B));
	}

	public static unsafe Bitmap ToIndexImage(this Bitmap source, Color[] pallette) {
		if (source == null) {
			return null;
		}

		if (source.PixelFormat != PixelFormat.Format24bppRgb && source.PixelFormat != PixelFormat.Format32bppArgb) {
			throw new InvalidOperationException("仅支持 Format24bppRgb 和 Format32bppArgb。");
		}

		Dictionary<int, byte> pi = new(pallette.Length);
		for (int i = pallette.Length - 1; i >= 0; i--) {
			pi[pallette[i].ToArgb()] = (byte)i;
		}

		Bitmap result = new(source.Width, source.Height, PixelFormat.Format8bppIndexed);
		BitmapData sourceData = source.LockBits(false);
		BitmapData targetData = result.LockBits(true);
		int bw = source.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;
		byte* src = (byte*)sourceData.Scan0;
		byte* res = (byte*)targetData.Scan0;
		ColorPalette rp = result.Palette;
		for (int i = 0; i < pallette.Length; i++) {
			rp.Entries[i] = pallette[i];
		}

		result.Palette = rp;
		int w = source.Width, h = source.Height;
		for (int y = 0; y < h; y++) {
			byte* ps = src;
			byte* pr = res;
			switch (bw) {
				case 3: {
						for (int x = 0; x < w; x++) {
							*pr = pi[*ps + (*++ps << 8) + (*++ps << 16) + (0xFF << 24)];
							++pr;
							++ps;
						}

						break;
					}
				case 4: {
						for (int x = 0; x < w; x++) {
							*pr = pi[*ps + (*++ps << 8) + (*++ps << 16) + (*++ps << 24)];
							++pr;
							++ps;
						}

						break;
					}
			}

			src += sourceData.Stride;
			res += targetData.Stride;
		}

		source.UnlockBits(sourceData);
		result.UnlockBits(targetData);
		return result;
	}

	public static Bitmap ToMonochrome(this Bitmap bitmap) {
		return CopyToBpp(bitmap, 1);
	}

	/// <summary>将图像转换为黑白图像。</summary>
	/// <param name="original">需要转换的图像。</param>
	/// <returns>转换后的图像。</returns>
	/// <remarks>http://www.wischik.com/lu/programmer/1bpp.html</remarks>
	public static Bitmap ToBitonal(this Bitmap original) {
		Bitmap source = null;

		if (original.PixelFormat == PixelFormat.Format1bppIndexed) {
			return (Bitmap)original.Clone();
		}

		if (original.PixelFormat != PixelFormat.Format24bppRgb) {
			// If original bitmap is not already in 24 BPP, ARGB format, then convert
			// unfortunately Clone doesn't do this for us but returns a bitmap with the same pixel format
			// source = original.Clone( new Rectangle( Point.Empty, original.Size ), PixelFormat.Format24bppRgb );
			source = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
			source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
			using Graphics g = Graphics.FromImage(source);
			//g.CompositingQuality = Drawing2D.CompositingQuality.GammaCorrected;
			//g.InterpolationMode = Drawing2D.InterpolationMode.Low;
			//g.SmoothingMode = Drawing2D.SmoothingMode.None;
			g.DrawImageUnscaled(original, 0, 0);
		}
		else {
			source = original;
		}

		// Lock source bitmap in memory
		BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height),
			ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

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
		Bitmap destination = new(sourceData.Width, sourceData.Height, PixelFormat.Format1bppIndexed);
		destination.SetResolution(original.HorizontalResolution, original.VerticalResolution);

		// Lock destination bitmap in memory
		BitmapData destinationData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height),
			ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

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
		byte[] destinationBuffer = new byte[dstStride * height];
		int srcIx = 0;
		int dstIx = 0;

		// Iterate lines
		for (int y = 0; y < height; y++, srcIx += srcStride, dstIx += dstStride) {
			byte bit = 128;
			int i = srcIx;
			int j = dstIx;
			byte pix8 = 0;
			// Iterate pixels
			for (int x = 0; x < width; x++, i += 3) {
				// Compute pixel brightness (i.e. total of Red, Green, and Blue values)
				int newPixel = sourceBuffer[i] + sourceBuffer[i + 1] + sourceBuffer[i + 2];

				if (newPixel > threshold) {
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

	/// <summary>
	///     Copies a bitmap into a 1bpp/8bpp bitmap of the same dimensions, fast
	/// </summary>
	/// <param name="b">original bitmap</param>
	/// <param name="bpp">1 or 8, target bpp</param>
	/// <returns>a 1bpp copy of the bitmap</returns>
	/// <remarks>http://www.wischik.com/lu/programmer/1bpp.html</remarks>
	private static Bitmap CopyToBpp(Bitmap b, int bpp) {
		if (bpp != 1 && bpp != 8) {
			throw new ArgumentException("1 or 8", nameof(bpp));
		}

		// Plan: built into Windows GDI is the ability to convert
		// bitmaps from one format to another. Most of the time, this
		// job is actually done by the graphics hardware accelerator card
		// and so is extremely fast. The rest of the time, the job is done by
		// very fast native code.
		// We will call into this GDI functionality from C#. Our plan:
		// (1) Convert our Bitmap into a GDI hbitmap (ie. copy unmanaged->managed)
		// (2) Create a GDI monochrome hbitmap
		// (3) Use GDI "BitBlt" function to copy from hbitmap into monochrome (as above)
		// (4) Convert the monochrone hbitmap into a Bitmap (ie. copy unmanaged->managed)

		int w = b.Width, h = b.Height;
		IntPtr hbm = b.GetHbitmap(); // this is step (1)
									 //
									 // Step (2): create the monochrome bitmap.
									 // "BITMAPINFO" is an interop-struct which we define below.
									 // In GDI terms, it's a BITMAPHEADERINFO followed by an array of two RGBQUADs
		NativeMethods.BITMAPINFO bmi = new() {
			biSize = 40, // the size of the BITMAPHEADERINFO struct
			biWidth = w,
			biHeight = h,
			biPlanes = 1, // "planes" are confusing. We always use just 1. Read MSDN for more info.
			biBitCount = (short)bpp, // ie. 1bpp or 8bpp
			biCompression =
				NativeMethods.BI_RGB, // ie. the pixels in our RGBQUAD table are stored as RGBs, not palette indexes
			biSizeImage = (uint)(((w + 7) & 0xFFFFFFF8) * h / 8),
			biXPelsPerMeter = 1000000, // not really important
			biYPelsPerMeter = 1000000 // not really important
		};
		// Now for the colour table.
		uint ncols = (uint)1 << bpp; // 2 colours for 1bpp; 256 colours for 8bpp
		bmi.biClrUsed = ncols;
		bmi.biClrImportant = ncols;
		bmi.cols = new uint[256]; // The structure always has fixed size 256, even if we end up using fewer colours
		if (bpp == 1) {
			bmi.cols[0] = MakeRgb(0, 0, 0);
			bmi.cols[1] = MakeRgb(255, 255, 255);
		}
		else {
			for (int i = 0; i < ncols; i++) {
				bmi.cols[i] = MakeRgb(i, i, i);
			}
		}

		// For 8bpp we've created an palette with just greyscale colours.
		// You can set up any palette you want here. Here are some possibilities:
		// greyscale: for (int i=0; i<256; i++) bmi.cols[i]=MAKERGB(i,i,i);
		// rainbow: bmi.biClrUsed=216; bmi.biClrImportant=216; int[] colv=new int[6]{0,51,102,153,204,255};
		//          for (int i=0; i<216; i++) bmi.cols[i]=MAKERGB(colv[i/36],colv[(i/6)%6],colv[i%6]);
		// optimal: a difficult topic: http://en.wikipedia.org/wiki/Color_quantization
		// 
		// Now create the indexed bitmap "hbm0"
		IntPtr hbm0 = NativeMethods.CreateDIBSection(IntPtr.Zero, ref bmi, NativeMethods.DIB_RGB_COLORS, out IntPtr bits0,
			IntPtr.Zero, 0);
		//
		// Step (3): use GDI's BitBlt function to copy from original hbitmap into monocrhome bitmap
		// GDI programming is kind of confusing... nb. The GDI equivalent of "Graphics" is called a "DC".
		IntPtr sdc = NativeMethods.GetDC(IntPtr.Zero); // First we obtain the DC for the screen
													   // Next, create a DC for the original hbitmap
		IntPtr hdc = NativeMethods.CreateCompatibleDC(sdc);
		NativeMethods.SelectObject(hdc, hbm);
		// and create a DC for the monochrome hbitmap
		IntPtr hdc0 = NativeMethods.CreateCompatibleDC(sdc);
		NativeMethods.SelectObject(hdc0, hbm0);
		// Now we can do the BitBlt:
		NativeMethods.BitBlt(hdc0, 0, 0, w, h, hdc, 0, 0, NativeMethods.SRCCOPY);
		// Step (4): convert this monochrome hbitmap back into a Bitmap:
		Bitmap b0 = Image.FromHbitmap(hbm0);
		//
		// Finally some cleanup.
		NativeMethods.DeleteDC(hdc);
		NativeMethods.DeleteDC(hdc0);
		NativeMethods.ReleaseDC(IntPtr.Zero, sdc);
		NativeMethods.DeleteObject(hbm);
		NativeMethods.DeleteObject(hbm0);
		//
		return b0;
	}

	private static uint MakeRgb(int r, int g, int b) {
		return (uint)(b & 255) | (uint)((g & 255) << 8) | (uint)((r & 255) << 16);
	}

	// MuPDF: pixmap.c
	private static int mul255(int a, int b) {
		/* see Jim Blinn's book "Dirty Pixels" for how this works */
		int x = (a * b) + 128;
		x += x >> 8;
		return x >> 8;
	}

	private static class NativeMethods
	{
		public const uint BI_RGB = 0;
		public const uint DIB_RGB_COLORS = 0;
		public const int SRCCOPY = 0x00CC0020;

		[DllImport("gdi32.dll")]
		public static extern int BitBlt(IntPtr hdcDst, int xDst, int yDst, int w, int h, IntPtr hdcSrc, int xSrc,
			int ySrc, int rop);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO bmi, uint Usage, out IntPtr bits,
			IntPtr hSection, uint dwOffset);

		[DllImport("gdi32.dll")]
		public static extern int DeleteDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[StructLayout(LayoutKind.Sequential)]
		public struct BITMAPINFO
		{
			public uint biSize;
			public int biWidth, biHeight;
			public short biPlanes, biBitCount;
			public uint biCompression, biSizeImage;
			public int biXPelsPerMeter, biYPelsPerMeter;
			public uint biClrUsed, biClrImportant;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public uint[] cols;
		}
	}
}