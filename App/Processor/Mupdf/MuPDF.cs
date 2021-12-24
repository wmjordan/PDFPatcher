using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace MuPDFLib
{
	public class MuPDF : IDisposable
	{
		private const string MuDLL = "MuPDFLib.dll";
		private IntPtr _MuPdfObject;
		private string _FileName;
		private byte[] _Image;
		private GCHandle _ImagePin;
		private string _PdfPassword;
		private int _CurrentPage;
		private int _LoadType;
		private int _AliasBits;

		public int PageCount { get; set; }
		public int Page {
			get { return _CurrentPage; }
			set {
				_CurrentPage = NativeMethods.LoadPage (this._MuPdfObject, value);
			}
		}

		public double Width {
			get {
				if (_CurrentPage > 0)
					return NativeMethods.GetWidth (this._MuPdfObject);
				else
					return 0;
			}
		}

		public double Height {
			get {
				if (_CurrentPage > 0)
					return NativeMethods.GetHeight (this._MuPdfObject);
				else
					return 0;
			}
		}

		public int AntiAliasLevel {
			get {
				return NativeMethods.GetAntiAliasLevel (this._MuPdfObject);
			}
			set {
				if (_AliasBits > 8) {
					_AliasBits = 8;
				}
				else if (_AliasBits < 0) {
					_AliasBits = 0;
				}
				NativeMethods.SetAntiAliasLevel (this._MuPdfObject, _AliasBits);
			}
		}

		public MuPDF (byte[] image, string pdfPassword) {
			_LoadType = 1;
			_Image = image;
			_PdfPassword = pdfPassword;

			if (image == null)
				throw new ArgumentNullException ();
			Initialize ();
		}

		public MuPDF (string fileName, string pdfPassword) {
			_LoadType = 0;
			_FileName = fileName;
			_PdfPassword = pdfPassword;

			if (!File.Exists (_FileName))
				throw new FileNotFoundException ("Cannot find file to open!", _FileName);
			Initialize ();
		}

		public void Initialize () {
			this._MuPdfObject = NativeMethods.CreateMuPDFClass ();
			if (_LoadType == 0)
				PageCount = NativeMethods.LoadPdf (this._MuPdfObject, _FileName, _PdfPassword);
			else if (_LoadType == 1) {
				_ImagePin = GCHandle.Alloc (_Image, GCHandleType.Pinned);
				PageCount = NativeMethods.LoadPdfFromStream (this._MuPdfObject, _Image, (int)_Image.Length, _PdfPassword);
			}

			if (PageCount == -5)
				throw new Exception ("PDF password needed!");
			else if (PageCount == -6)
				throw new Exception ("Invalid PDF password supplied!");
			else if (PageCount < 1)
				throw new Exception ("Unable to open pdf document!");
			_CurrentPage = 1;
			_AliasBits = NativeMethods.GetAntiAliasLevel (_MuPdfObject);
		}

		private static bool Is64BitProcess () {
			return IntPtr.Size == 8;
		}

		public void Dispose () {
			Dispose (true);
		}

		protected virtual void Dispose (bool bDisposing) {
			if (this._MuPdfObject != IntPtr.Zero) {
				// Call the DLL Export to dispose this class
				NativeMethods.DisposeMuPDFClass (this._MuPdfObject);
				this._MuPdfObject = IntPtr.Zero;
				if (_ImagePin.IsAllocated)
					_ImagePin.Free ();
			}

			if (bDisposing) {
				// No need to call the finalizer since we've now cleaned
				// up the unmanaged memory
				GC.SuppressFinalize (this);
			}
		}

		~MuPDF () {
			Dispose (false);
		}

		public unsafe Bitmap GetBitmap (int width, int height, float dpix, float dpiy, int rotation, RenderType type, bool rotateLandscapePages, int maxSize) {
			Bitmap bitmap2 = null;
			int nLength = 0;
			IntPtr data = NativeMethods.GetBitmap (this._MuPdfObject, out width, out height, dpix, dpiy, rotation, (int)type, rotateLandscapePages, out nLength, maxSize);
			if (data == null || data == IntPtr.Zero)
				throw new Exception ("Unable to render pdf page to bitmap!");

			if (type == RenderType.RGB) {
				bitmap2 = new Bitmap (width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				BitmapData imageData = bitmap2.LockBits (new Rectangle (0, 0, width, height), ImageLockMode.ReadWrite, bitmap2.PixelFormat);
				byte* ptrSrc = (byte*)data;
				byte* ptrDest = (byte*)imageData.Scan0;
				for (int y = 0; y < height; y++) {
					byte* pl = ptrDest;
					byte* sl = ptrSrc;
					for (int x = 0; x < width; x++) {
						//Swap these here instead of in MuPDF because most pdf images will be rgb or cmyk.
						//Since we are going through the pixels one by one anyway swap here to save a conversion from rgb to bgr.
						pl[2] = sl[0]; //b-r
						pl[1] = sl[1]; //g-g
						pl[0] = sl[2]; //r-b
						//pl[3] = sl[3]; //alpha
						pl += 3;
						sl += 4;
					}
					ptrDest += imageData.Stride;
					ptrSrc += width * 4;
				}
				bitmap2.UnlockBits (imageData);
			}
			else if (type == RenderType.Grayscale) {
				bitmap2 = new Bitmap (width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
				ColorPalette palette = bitmap2.Palette;
				for (int i = 0; i < 256; ++i)
					palette.Entries[i] = System.Drawing.Color.FromArgb (i, i, i);
				bitmap2.Palette = palette;
				BitmapData imageData = bitmap2.LockBits (new Rectangle (0, 0, width, height), ImageLockMode.ReadWrite, bitmap2.PixelFormat);

				byte* ptrSrc = (byte*)data;
				byte* ptrDest = (byte*)imageData.Scan0;
				for (int y = 0; y < height; y++) {
					byte* pl = ptrDest;
					byte* sl = ptrSrc;
					for (int x = 0; x < width; x++) {
						pl[0] = sl[0];
						//pl[1] = sl[1]; //alpha
						pl += 1;
						sl += 2;
					}
					ptrDest += imageData.Stride;
					ptrSrc += width * 2;
				}
				bitmap2.UnlockBits (imageData);
			}
			else//RenderType.Monochrome
            {
				//bitmap2 = new Bitmap(width, height, bmpstride, PixelFormat.Format1bppIndexed, data);//Doesn't free memory
				bitmap2 = new Bitmap (width, height, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
				ColorPalette palette = bitmap2.Palette;
				palette.Entries[0] = System.Drawing.Color.FromArgb (0, 0, 0);
				palette.Entries[1] = System.Drawing.Color.FromArgb (255, 255, 255);
				bitmap2.Palette = palette;
				BitmapData imageData = bitmap2.LockBits (new Rectangle (0, 0, width, height), ImageLockMode.ReadWrite, bitmap2.PixelFormat);

				byte* ptrSrc = (byte*)data;
				byte* ptrDest = (byte*)imageData.Scan0;
				for (int i = 0; i < nLength; i++) {
					ptrDest[i] = ptrSrc[i];
				}
				bitmap2.UnlockBits (imageData);
			}
			bitmap2.SetResolution (dpix, dpiy);
			NativeMethods.FreeRenderedPage (this._MuPdfObject);//Free unmanaged array

			return bitmap2;
		}


		public unsafe byte[] GetPixels (ref int width, ref int height, float dpix, float dpiy, int rotation, RenderType type, bool rotateLandscapePages, out uint cbStride, int maxSize) {
			byte[] output = null;
			int nLength = 0;
			IntPtr data = NativeMethods.GetBitmap (this._MuPdfObject, out width, out height, dpix, dpiy, rotation, (int)type, rotateLandscapePages, out nLength, maxSize);
			if (data == null || data == IntPtr.Zero)
				throw new Exception ("Unable to render pdf page to bitmap!");

			if (type == RenderType.RGB) {
				const int depth = 24;
				int bmpstride = ((width * depth + 31) & ~31) >> 3;
				int newSize = bmpstride * height;

				output = new byte[newSize];
				cbStride = (uint)bmpstride;
				IntPtr DestPointer = Marshal.UnsafeAddrOfPinnedArrayElement (output, 0);

				byte* ptrSrc = (byte*)data;
				byte* ptrDest = (byte*)DestPointer;
				for (int y = 0; y < height; y++) {
					byte* pl = ptrDest;
					byte* sl = ptrSrc;
					for (int x = 0; x < width; x++) {
						//Swap these here instead of in MuPDF because most pdf images will be rgb or cmyk.
						//Since we are going through the pixels one by one anyway swap here to save a conversion from rgb to bgr.
						pl[2] = sl[0]; //b-r
						pl[1] = sl[1]; //g-g
						pl[0] = sl[2]; //r-b
						//pl[3] = sl[3]; //alpha
						pl += 3;
						sl += 4;
					}
					ptrDest += cbStride;
					ptrSrc += width * 4;
				}
			}
			else if (type == RenderType.Grayscale) {
				const int depth = 8;//(n * 8)
				int bmpstride = ((width * depth + 31) & ~31) >> 3;
				int newSize = bmpstride * height;

				output = new byte[newSize];
				cbStride = (uint)bmpstride;
				IntPtr DestPointer = Marshal.UnsafeAddrOfPinnedArrayElement (output, 0);

				byte* ptrSrc = (byte*)data;
				byte* ptrDest = (byte*)DestPointer;
				for (int y = 0; y < height; y++) {
					byte* pl = ptrDest;
					byte* sl = ptrSrc;
					for (int x = 0; x < width; x++) {
						pl[0] = sl[0]; //g
						//pl[1] = sl[1]; //alpha
						pl += 1;
						sl += 2;
					}
					ptrDest += cbStride;
					ptrSrc += width * 2;
				}
			}
			else//RenderType.Monochrome
            {
				const int depth = 1;
				int bmpstride = ((width * depth + 31) & ~31) >> 3;

				cbStride = (uint)bmpstride;
				output = new byte[nLength];
				Marshal.Copy (data, output, 0, nLength);
			}
			NativeMethods.FreeRenderedPage (this._MuPdfObject);//Free unmanaged array
			return output;
		}

		class NativeMethods
		{
			[DllImport (MuDLL, EntryPoint = "CreateMuPDFClass")]
			static internal extern IntPtr CreateMuPDFClass ();

			[DllImport (MuDLL, EntryPoint = "DisposeMuPDFClass")]
			static internal extern void DisposeMuPDFClass (IntPtr mupdf);

			[DllImport (MuDLL, EntryPoint = "CallGetBitmap")]
			internal static extern IntPtr GetBitmap (IntPtr mupdf, out int width, out int height, float dpix, float dpiy, int rotation, int colorspace, bool rotateLandscapePages, out int nLength, int maxSize);

			[DllImport (MuDLL, EntryPoint = "CallLoadPdf", CharSet=CharSet.Unicode)]
			internal static extern int LoadPdf (IntPtr mupdf, string filename, string password);

			[DllImport (MuDLL, EntryPoint = "CallLoadPdfFromStream")]
			internal static extern int LoadPdfFromStream (IntPtr mupdf, byte[] buffer, int bufferSize, string password);

			[DllImport (MuDLL, EntryPoint = "CallGetWidth")]
			internal static extern float GetWidth (IntPtr mupdf);

			[DllImport (MuDLL, EntryPoint = "CallLoadPage")]
			internal static extern int LoadPage (IntPtr mupdf, int pageNumber);

			[DllImport (MuDLL, EntryPoint = "CallGetHeight")]
			internal static extern float GetHeight (IntPtr mupdf);

			[DllImport (MuDLL, EntryPoint = "CallGetAntiAliasLevel")]
			internal static extern int GetAntiAliasLevel (IntPtr mupdf);

			[DllImport (MuDLL, EntryPoint = "CallSetAntiAliasLevel")]
			internal static extern void SetAntiAliasLevel (IntPtr mupdf, int antiAliasLevel);

			[DllImport (MuDLL, EntryPoint = "CallFreeRenderedPage")]
			internal static extern void FreeRenderedPage (IntPtr mupdf);
		}
	}

	public enum RenderType
	{
		/// <summary>24-bit Color RGB</summary>
		RGB = 0,
		/// <summary>8-bit Grayscale</summary>
		Grayscale = 1,
		/// <summary>1-bit Monochrome</summary>
		Monochrome = 2
	}
}
