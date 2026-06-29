using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using CLR;
using PDFPatcher.Common;
using PDFPatcher.Processor.Imaging;

namespace MuPDF.Extensions;

public static class MuPDFExtensions
{
	#region 文档基础结构
	public static bool IsContainer(this PdfObject obj) {
		return obj.TypeKind.CeqAny(Kind.Dictionary, Kind.Array, Kind.Stream);
	}
	public static TObj Get<TObj>(this PdfArray array, int index) where TObj : PdfObject {
		return array[index].UnderlyingObject as TObj;
	}
	public static TObj Get<TObj>(this PdfDictionary dict, int index) where TObj : PdfObject {
		return dict[index].Value.UnderlyingObject as TObj;
	}
	public static TObj Get<TObj>(this PdfDictionary dict, PdfNames key) where TObj : PdfObject {
		return dict[key].UnderlyingObject as TObj;
	}
	public static bool TryGet<TObj>(this PdfDictionary dict, PdfNames key, out TObj value) where TObj : PdfObject {
		return (value = dict[key].UnderlyingObject as TObj) is not null;
	}
	public static bool HasNameValue(this PdfDictionary dict, PdfNames key, PdfNames valueToCompare) {
		return (dict[key].UnderlyingObject as PdfName)?.Equals(valueToCompare) == true;
	}
	public static TObj Get<TObj>(this PdfDictionary dict, PdfNames key, PdfNames alias) where TObj : PdfObject {
		return dict.GetValue(key, alias).UnderlyingObject as TObj;
	}

	public static string GetName(this Kind kind) {
		return kind switch {
			Kind.Array => "array",
			Kind.Boolean => "bool",
			Kind.Dictionary => "dictionary",
			Kind.Reference => "reference",
			Kind.Name => "name",
			Kind.Null => "null",
			Kind.Integer or Kind.Float => "number",
			Kind.Stream => "stream",
			Kind.String => "string",
			_ => String.Empty,
		};
	}

	internal static string GetArrayString(this IEnumerable<PdfObject> array) {
		var sb = StringBuilderCache.Acquire();
		int k = 0;
		foreach (var item in array) {
			if (++k > 1) {
				sb.Append(' ');
			}
			if (item.TypeKind == Kind.Array) {
				sb.Append('[');
				sb.Append(GetArrayString(item as PdfArray));
				sb.Append(']');
			}
			else if (item.TypeKind.CeqAny(Kind.Dictionary, Kind.Stream)) {
				sb.Append("<<...>>");
			}
			else {
				sb.Append(item);
			}
		}
		return StringBuilderCache.GetStringAndRelease(sb);
	}

	#endregion

	#region 几何尺寸
	public static RectangleF ToRectangleF(this Box box) {
		return RectangleF.FromLTRB(box.X0, box.Y0, box.X1, box.Y1);
	}
	public static Rectangle ToRectangle(this Box box) {
		return Rectangle.FromLTRB((int)box.X0, (int)box.Y0, (int)box.X1, (int)box.Y1);
	}
	public static void Deconstruct(this Point point, out float x, out float y) {
		x = point.X; y = point.Y;
	}

	public static bool IsHorizontalNeighbor(this Box me, Box other) {
		if (me.IsEmpty || other.IsInfinite || other.IsEmpty || me.IsInfinite || other.Y0 > me.Y1 || other.Y1 < me.Y0) {
			return false;
		}
		var h = me.Height / other.Height;
		return h > 0.4 && h < 2.5;
	}
	#endregion

	/// <summary>
	/// 使用 <see cref="Encoding"/> 强制解码 <see cref="PdfString"/>。
	/// </summary>
	/// <param name="text">要解码的字符串。</param>
	/// <param name="encoding">使用的文本编码。指定 <see langword="null"/> 则自动选择文本编码。</param>
	public static string Decode(this PdfString text, Encoding encoding) {
		if (encoding is null) {
			return text.Value;
		}
		var bytes = text.GetBytes();
		int offset = 0;
		int length = bytes.Length;
		ushort h0;
		if (length >= 2) {
			// 尝试跳过字节顺序标记
			if ((h0 = Op.Cast<byte, ushort>(ref bytes[0])).CeqAny(0xFEFF, 0xFFFE)) {
				offset = 2;
			}
			else if (length >= 3) {
				// UTF-8 BOM: EFBBBF
				if (h0 == 0xBBEF && bytes[2] == 0xBF) {
					offset = 3;
				}
				// BOM: 0000FEFF
				else if (length >= 4 && Op.Cast<byte, uint>(ref bytes[0]) == 0xFFFE0000) {
					offset += 4;
				}
			}
		}
		return encoding.GetString(bytes, offset, length - offset);
	}

	public static string GetText(this TextLine textLine) {
		var sb = StringBuilderCache.Acquire(10);
		foreach (var ch in textLine) {
			sb.Append(Char.IsSurrogate((char)ch.Character) ? "?" : char.ConvertFromUtf32(ch.Character));
		}
		return StringBuilderCache.GetStringAndRelease(sb);
	}

	#region 渲染页面
	public static Bitmap RenderBitmapPage(this Page page, int width, int height, ImageRendererOptions options, Cookie cookie) {
		using var pix = InternalRenderPage(page, width, height, options, cookie);
		return pix?.ToBitmap(options);
	}

	static Pixmap InternalRenderPage(Page page, int width, int height, ImageRendererOptions options, Cookie cookie) {
		var b = page.Bound;
		if (b.Width == 0 || b.Height == 0) {
			return null;
		}
		var ctm = CalculateMatrix(page, width, height, options);
		var bbox = width > 0 && height > 0 ? new BBox(0, 0, width, height) : b.Transform(ctm).Round();

		var pix = Pixmap.Create(((ColorspaceKind)options.ColorSpace).SubstituteDefault(ColorspaceKind.RGB), bbox)
			?? throw new MuException($"无法渲染页面：{(page.PageNumber + 1).ToText()}");
		pix.Clear(0xFF);
		try {
			using var dev = Device.NewDraw(pix, Matrix.Identity);
			if (options.LowQuality) {
				dev.EnableDeviceHints(DeviceHints.DontInterpolateImages | DeviceHints.NoCache);
			}
			if (cookie.IsCancellationPending) {
				goto CANCEL;
			}
			page.RunContents(dev, ctm, cookie);
			if (!options.HideAnnotations) {
				page.RunAnnotations(dev, ctm, cookie);
				page.RunWidgets(dev, ctm, cookie);
			}
			dev.Close();

			if (cookie.IsCancellationPending) {
				goto CANCEL;
			}
			if (options.TintColor != Color.Transparent) {
				pix.Tint(options.TintColor.ToArgb());
			}
			if (options.Gamma != 1.0f) {
				pix.Gamma(options.Gamma);
			}
			return pix;
		}
		catch {
			pix.Dispose();
			throw;
		}
	CANCEL:
		pix.Dispose();
		return null;
	}

	static Matrix CalculateMatrix(Page page, int width, int height, ImageRendererOptions options) {
		float w = width, h = height;
		var b = page.Bound;
		if (options.UseSpecificWidth) {
			if (w < 0) {
				w = -w;
			}
			if (h < 0) {
				h = -h;
			}
			if (options.FitArea && w != 0 && h != 0) {
				var rw = w / b.Width;
				var rh = h / b.Height;
				if (rw < rh) {
					h = 0;
				}
				else {
					w = 0;
				}
			}
			if (w == 0 && h == 0) { // No resize
				w = b.Width;
				h = b.Height;
			}
			else if (h == 0) {
				h = width * b.Height / b.Width;
			}
			else if (w == 0) {
				w = height * b.Width / b.Height;
			}
		}
		else if (w == 0 || h == 0) {
			w = b.Width * options.ScaleRatio * options.Dpi / 72;
			h = b.Height * options.ScaleRatio * options.Dpi / 72;
		}

		var ctm = Matrix.Scale(w / b.Width, h / b.Height).RotateTo(options.Rotation);
		if (options.VerticalFlipImages) {
			ctm = ctm.Concat(Matrix.VerticalFlip);
		}
		if (options.HorizontalFlipImages) {
			ctm = ctm.Concat(Matrix.HorizontalFlip);
		}
		return ctm;
	}
	/// <summary>
	/// 将 MuPDF Pixmap 转换为 .NET Bitmap。
	/// 假设 pixmap.Samples 返回 IntPtr，像素数据为 BGR 排列。
	/// </summary>
	public static unsafe Bitmap ToBitmap(this Pixmap pix) {
		int w = pix.Width;
		int h = pix.Height;
		int n = pix.Components;       // 总分量数 = colorants + spots + alpha
		int colorants = pix.Colorants; // 过程色分量数
		int spots = pix.Spots;         // 专色通道数
		bool hasAlpha = pix.Alpha == 1;

		// 存在 spot 通道时，应先通过 MuPDF 的 fz_convert_pixmap
		// 转为 DeviceRGB/BGR，否则屏幕无法正确显示专色

		// ── 1. 灰度 → 8bpp Indexed ──
		if ((colorants == 1 && !hasAlpha || colorants == 0 && hasAlpha) && spots == 0) {
			var bmp = new Bitmap(w, h, PixelFormat.Format8bppIndexed);
			bmp.CreateStandardGrayscalePalette();
			var data = bmp.LockBits(true);
			Copy8bppImage(pix, w, h, hasAlpha, data);
			bmp.UnlockBits(data);
			return bmp;
		}

		// ── 2. BGR 24bpp → Format24bppRgb（直接拷贝，无需换序） ──
		if (colorants == 3 && !hasAlpha && spots == 0) {
			var bmp = new Bitmap(w, h, PixelFormat.Format24bppRgb);
			var data = bmp.LockBits(true);
			Copy24bppImage(pix, w, h, false, data);
			bmp.UnlockBits(data);
			return bmp;
		}

		// ── 3. BGR+Alpha 32bpp → Format32bppArgb（需反预乘） ──
		if (colorants == 3 && hasAlpha && spots == 0) {
			var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
			var data = bmp.LockBits(true);
			Copy32bppBgraUnpremultiply(pix, w, h, data);
			bmp.UnlockBits(data);
			return bmp;
		}

		if (colorants == 4 && !hasAlpha && spots == 0) {
			var bmp = new Bitmap(w, h, PixelFormat.Format24bppRgb);
			var data = bmp.LockBits(true);
			using (var rgbPix = pix.ConvertColorspace(ColorspaceKind.RGB, null)) {
				Copy24bppImage(rgbPix, w, h, false, data);
			}
			bmp.UnlockBits(data);
			return bmp;
		}

		throw new NotSupportedException(
			$"不支持的像素格式: colorants={colorants}, spots={spots}, alpha={hasAlpha}");
	}

	/// <summary>
	/// 将 Pixmap 的数据转换为 <see cref="Bitmap"/>。
	/// </summary>
	public static unsafe Bitmap ToBitmap(this Pixmap pix, ImageRendererOptions options) {
		int width = pix.Width;
		int height = pix.Height;
		bool grayscale = options.ColorSpace == (ColorSpace)ColorspaceKind.Gray;
		bool invert = options.InvertColor;
		var bmp = new Bitmap(width, height, grayscale ? PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb);
		var imageData = bmp.LockBits(true);
		try {
			if (grayscale) {
				bmp.CreateStandardGrayscalePalette();
				Copy8bppImage(pix, width, height, invert, imageData);
			}
			else { // DeviceBGR
				Copy24bppImage(pix, width, height, invert, imageData);
			}
		}
		finally {
			bmp.UnlockBits(imageData);
		}
		if (options.Dpi > 0) {
			bmp.SetResolution(options.Dpi, options.Dpi);
		}
		return bmp;
	}

	/// <summary>
	/// 灰度图像：1 byte/pixel，可选反转（用于 min-is-white 的图像）
	/// </summary>
	static unsafe void Copy8bppImage(Pixmap pix, int width, int height, bool invert, BitmapData imageData) {
		var ptrSrc = (byte*)pix.Samples;
		var ptrDest = (byte*)imageData.Scan0;
		int srcStride = pix.Stride;

		for (int y = 0; y < height; y++) {
			var sl = ptrSrc;
			var dl = ptrDest;
			if (invert) {
				for (int x = 0; x < width; x++)
					*dl++ = (byte)(*sl++ ^ 0xFF);
			}
			else {
				// 整行拷贝，比逐字节快得多
				Op.CopyUnaligned(sl, dl, width);
			}
			ptrSrc += srcStride;
			ptrDest += imageData.Stride;
		}
	}
	/// <summary>
	/// BGR+Alpha 32bpp → RGB+Alpha 32bpp，交换 R/B 并反预乘 alpha。
	/// MuPDF 渲染输出是 premultiplied alpha，.NET Bitmap 需要 straight alpha。
	/// </summary>
	static unsafe void Copy32bppBgraUnpremultiply(Pixmap pix, int width, int height,
												   BitmapData imageData) {
		var ptrSrc = (byte*)pix.Samples;
		var ptrDest = (byte*)imageData.Scan0;
		int srcStride = pix.Stride;

		for (int y = 0; y < height; y++) {
			byte* sl = ptrSrc;
			byte* dl = ptrDest;
			for (int x = 0; x < width; x++) {
				byte bSrc = sl[0]; // 源 B
				byte g = sl[1]; // 源 G
				byte rSrc = sl[2]; // 源 R
				byte a = sl[3]; // 源 A

				// 反预乘
				byte r, b;
				if (a > 0 && a < 255) {
					r = (byte)Math.Min(255, rSrc * 255 / a);
					g = (byte)Math.Min(255, g * 255 / a);
					b = (byte)Math.Min(255, bSrc * 255 / a);
				}
				else {
					r = rSrc;
					b = bSrc;
				}

				// 写入目标：RGB 顺序
				dl[0] = r;
				dl[1] = g;
				dl[2] = b;
				dl[3] = a;

				sl += 4;
				dl += 4;
			}
			ptrSrc += srcStride;
			ptrDest += imageData.Stride;
		}
	}

	static unsafe void Copy24bppImage(Pixmap pix, int width, int height, bool invert, BitmapData imageData) {
		var ptrSrc = (byte*)pix.Samples;
		var ptrDest = (byte*)imageData.Scan0;
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
	#endregion

}
