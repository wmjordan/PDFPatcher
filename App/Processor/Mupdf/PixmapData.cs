using System;
using System.Drawing;
using System.Drawing.Imaging;
using FreeImageAPI;
using CC = System.Runtime.InteropServices.CallingConvention;
using DllImport = System.Runtime.InteropServices.DllImportAttribute;

namespace MuPdfSharp
{
	internal sealed class PixmapData : IDisposable
	{
		readonly ContextHandle _context;
		readonly PixmapHandle _pixmap;
		public PixmapData(ContextHandle context, PixmapHandle pixmap) {
			Width = NativeMethods.GetWidth(context, pixmap);
			Height = NativeMethods.GetHeight(context, pixmap);
			Components = NativeMethods.GetComponents(context, pixmap);
			Samples = NativeMethods.GetSamples(context, pixmap);
			_context = context;
			_pixmap = pixmap;
		}
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int Components { get; private set; }
		/// <summary>获取指向 Pixmap 数据内容的指针。</summary>
		public IntPtr Samples { get; private set; }
		/// <summary>获取 Pixmap 的边框。</summary>
		public BBox BBox => NativeMethods.GetBBox(_context, _pixmap);
		/// <summary>number of colorants (components, less any spots and alpha)。</summary>
		public int Colorants => NativeMethods.GetColorants(_context, _pixmap);
		/// <summary>number of spots (components, less colorants and alpha). Does not throw exceptions.。</summary>
		public int Spots => NativeMethods.GetColorants(_context, _pixmap);
		/// <summary>获取 Pixmap 一行像素的字节数。</summary>
		public int Stride => NativeMethods.GetStride(_context, _pixmap);

		/// <summary>
		/// 将 Pixmap 的数据转换为 <see cref="Bitmap"/>。
		/// </summary>
		public unsafe Bitmap ToBitmap(ImageRendererOptions options) {
			int width = Width;
			int height = Height;
			bool grayscale = options.ColorSpace == ColorSpace.Gray;
			bool invert = options.InvertColor;
			var bmp = new Bitmap(width, height, grayscale ? PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb);
			var imageData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bmp.PixelFormat);
			var ptrSrc = (byte*)Samples;
			var ptrDest = (byte*)imageData.Scan0;
			if (grayscale) {
				var palette = bmp.Palette;
				for (int i = 0; i < 256; ++i)
					palette.Entries[i] = Color.FromArgb(i, i, i);
				bmp.Palette = palette;
				for (int y = 0; y < height; y++) {
					var pl = ptrDest;
					var sl = ptrSrc;
					for (int x = 0; x < width; x++) {
						*pl = invert ? (byte)(*sl ^ 0xFF) : *sl;
						pl++;
						sl++;
					}
					ptrDest += imageData.Stride;
					ptrSrc = sl;
				}
			}
			else { // DeviceBGR
				for (int y = 0; y < height; y++) {
					var pl = ptrDest;
					var sl = ptrSrc;
					if (invert) {
						for (int x = 0; x < width; x++) {
							// 在这里进行 RGB 到 DIB BGR 的转换（省去 Mupdf 内部的转换工作）
							pl[2] = (byte)(*sl ^ 0xFF); sl++; // R
							pl[1] = (byte)(*sl ^ 0xFF); sl++; // G
							pl[0] = (byte)(*sl ^ 0xFF); sl++; // B
							pl += 3;
						}
					}
					else {
						for (int x = 0; x < width; x++) {
							// 在这里进行 RGB 到 DIB BGR 的转换（省去 Mupdf 内部的转换工作）
							pl[2] = *sl; sl++; // R
							pl[1] = *sl; sl++; // G
							pl[0] = *sl; sl++; // B
							pl += 3;
						}
					}
					ptrDest += imageData.Stride;
					ptrSrc = sl;
				}
			}
			bmp.UnlockBits(imageData);
			if (options.Dpi > 0) {
				bmp.SetResolution(options.Dpi, options.Dpi);
			}
			return bmp;
		}

		/// <summary>
		/// 将 Pixmap 的数据转换为 <see cref="FreeImageBitmap"/>。
		/// </summary>
		public unsafe FreeImageBitmap ToFreeImageBitmap(ImageRendererOptions options) {
			int width = Width;
			int height = Height;
			bool grayscale = options.ColorSpace == ColorSpace.Gray;
			bool invert = options.InvertColor;
			var bmp = new FreeImageBitmap(width, height, grayscale ? PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb);
			var ptrSrc = (byte*)Samples;
			if (grayscale) {
				bmp.Palette.CreateGrayscalePalette();
				for (int y = height - 1; y >= 0; y--) {
					var pDest = bmp.GetScanlinePointer(y);
					var pl = (byte*)pDest.ToPointer();
					var sl = ptrSrc;
					for (int x = 0; x < width; x++) {
						*pl = invert ? (byte)(*sl ^ 0xFF) : *sl;
						pl++;
						sl++;
					}
					ptrSrc = sl;
				}
			}
			else { // DeviceBGR
				for (int y = height - 1; y >= 0; y--) {
					var pDest = bmp.GetScanlinePointer(y);
					var pl = (byte*)pDest.ToPointer();
					var sl = ptrSrc;
					if (invert) {
						for (int x = 0; x < width; x++) {
							// 在这里进行 RGB 到 DIB BGR 的转换（省去 Mupdf 内部的转换工作）
							pl[2] = (byte)(*sl ^ 0xFF); sl++; // R
							pl[1] = (byte)(*sl ^ 0xFF); sl++; // G
							pl[0] = (byte)(*sl ^ 0xFF); sl++; // B
							pl += 3;
						}
					}
					else {
						for (int x = 0; x < width; x++) {
							// 在这里进行 RGB 到 DIB BGR 的转换（省去 Mupdf 内部的转换工作）
							pl[2] = *sl; sl++; // R
							pl[1] = *sl; sl++; // G
							pl[0] = *sl; sl++; // B
							pl += 3;
						}
					}
					ptrSrc = sl;
				}
			}
			bmp.SetResolution(options.Dpi, options.Dpi);
			return bmp;
		}

		/// <summary>
		/// 反转 Pixmap 的颜色。
		/// </summary>
		public void Invert() {
			NativeMethods.InvertPixmap(_context, _pixmap);
		}

		/// <summary>
		/// 为 Pixmap 蒙上色层。
		/// </summary>
		/// <param name="color">需要蒙上的颜色。</param>
		public void Tint(Color color) {
			NativeMethods.TintPixmap(_context, _pixmap, 0, color.ToArgb());
		}

		/// <summary>
		/// 对 Pixmap 执行 Gamma 校正。
		/// </summary>
		/// <param name="gamma">需要应用的 Gamma 值。1.0 表示不更改。</param>
		public void Gamma(float gamma) {
			if (gamma == 1) {
				return;
			}
			NativeMethods.GammaPixmap(_context, _pixmap, gamma);
		}

		/// <summary>
		/// 获取 Pixmap 内的数据。
		/// </summary>
		/// <returns>字节数组。</returns>
		public byte[] GetSampleBytes() {
			if (Samples == IntPtr.Zero) {
				return null;
			}
			var d = new byte[Width * Height * Components];
			System.Runtime.InteropServices.Marshal.Copy(Samples, d, 0, d.Length);
			return d;
		}

		#region 实现 IDisposable 接口的属性和方法
		private bool disposed = false;
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this); // 抑制析构函数
		}

		/// <summary>释放由 MuPdfPage 占用的资源。</summary>
		/// <param name="disposing">是否手动释放托管资源。</param>
		void Dispose(bool disposing) {
			if (!disposed) {
				if (disposing) {
					#region 释放托管资源
					//_components.Dispose ();
					#endregion
				}

				#region 释放非托管资源
				// 注意这里不是线程安全的
				_pixmap.DisposeHandle();
				#endregion
			}
			disposed = true;
		}

		// 析构函数只在未调用 Dispose 方法时调用
		// 派生类中不必再提供析构函数
		~PixmapData() {
			Dispose(false);
		}
		#endregion
	}


}
