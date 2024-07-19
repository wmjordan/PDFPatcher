using System;
using System.Runtime.InteropServices;
using System.Security;
using CC = System.Runtime.InteropServices.CallingConvention;

namespace MuPdfSharp
{
	[SuppressUnmanagedCodeSecurity]
	static partial class NativeMethods
	{
		const string DLL = "mupdflib.dll";
		const string FZ_VERSION = "1.24.3";

		const uint FZ_STORE_DEFAULT = 256 << 20;

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_context_imp", BestFitMapping = false)]
		static extern ContextHandle NewContext(IntPtr alloc, IntPtr locks, uint max_store, [MarshalAs(UnmanagedType.LPStr)] string fz_version);

		internal static ContextHandle NewContext() {
			var c = NewContext(IntPtr.Zero, IntPtr.Zero, FZ_STORE_DEFAULT, FZ_VERSION);
			if (c.IsInvalid) {
				throw new MuPdfException("MuPDF 引擎版本不匹配。");
			}
			return c;
		}

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_drop_context")]
		internal static extern void DropContext(IntPtr ctx);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_convert_error")]
		internal static unsafe extern sbyte* ConvertError(IntPtr ctx, out int code);

		#region Colorspace
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_device_gray")]
		internal static extern IntPtr GetGrayColorSpace(ContextHandle ctx);
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_device_rgb")]
		internal static extern IntPtr GetRgbColorSpace(ContextHandle ctx);
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_device_bgr")]
		internal static extern IntPtr GetBgrColorSpace(ContextHandle ctx);
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_device_cmyk")]
		internal static extern IntPtr GetCmykColorSpace(ContextHandle ctx);
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_device_lab")]
		internal static extern IntPtr GetLabColorSpace(ContextHandle ctx);
		#endregion

		#region Document and file stream
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "OpenPdfDocumentStream")]
		internal static extern IntPtr OpenPdfDocumentStream(ContextHandle ctx, StreamHandle stm);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_drop_document")]
		internal static extern void DropDocument(ContextHandle context, IntPtr doc);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_drop_stream")]
		internal static extern IntPtr DropStream(ContextHandle context, IntPtr stm);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_load_jpx")]
		internal static extern IntPtr LoadJpeg2000(ContextHandle ctx, byte[] data, int size, IntPtr colorspace);
		#endregion

		#region Device
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_bbox_device")]
		internal static extern IntPtr NewBBoxDevice(ContextHandle ctx, ref Rectangle bbox);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_draw_device")]
		internal static extern IntPtr NewDrawDevice(ContextHandle ctx, Matrix matrix, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_draw_device_with_bbox")]
		internal static extern IntPtr NewDrawDevice(ContextHandle ctx, Matrix matrix, PixmapHandle pix, ref BBox bbox);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_gdiplus_device")]
		internal static extern IntPtr NewGdiPlusDevice(ContextHandle ctx, IntPtr dc, BBox base_clip);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_list_device")]
		internal static extern IntPtr NewListDevice(ContextHandle ctx, DisplayListHandle list);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_stext_device")]
		internal static extern IntPtr NewTextDevice(ContextHandle ctx, TextPageHandle page, TextOptions options);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_close_device")]
		internal static extern void CloseDevice(ContextHandle context, IntPtr dev);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_drop_device")]
		internal static extern void DropDevice(ContextHandle context, IntPtr dev);
		#endregion

		#region Page
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_load_page")]
		internal static extern IntPtr LoadPage(ContextHandle context, DocumentHandle doc, int pageNumber);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_drop_page")]
		internal static extern void DropPage(ContextHandle context, IntPtr page);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_bound_page")]
		public static extern Rectangle BoundPage(ContextHandle context, PageHandle page);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_flatten_inheritable_page_items")]
		public static extern void FlattenInheritablePageItems(ContextHandle doc, IntPtr pageNode);
		#endregion

		#region Pixmap and display list
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_pixmap")]
		internal static extern IntPtr NewPixmap(ContextHandle ctx, IntPtr colorspace, int width, int height, IntPtr separations, int alpha);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "NewPixmapWithBBox")]
		internal static extern IntPtr NewPixmap(ContextHandle ctx, IntPtr colorspace, BBox bbox, IntPtr separations, int alpha);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_drop_pixmap")]
		internal static extern void DropPixmap(ContextHandle ctx, IntPtr pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_gamma_pixmap")]
		public static extern void GammaPixmap(ContextHandle ctx, PixmapHandle pix, float gamma);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_invert_pixmap")]
		public static extern void InvertPixmap(ContextHandle ctx, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_invert_pixmap_rect")]
		public static extern void InvertPixmap(ContextHandle ctx, PixmapHandle pix, BBox rect);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_tint_pixmap")]
		public static extern void TintPixmap(ContextHandle ctx, PixmapHandle pix, int black, int white);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_pixmap_bbox")]
		public static extern BBox GetBBox(ContextHandle ctx, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_pixmap_colorspace")]
		public static extern IntPtr GetColorSpace(ContextHandle ctx, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_pixmap_colorants")]
		public static extern int GetColorants(ContextHandle ctx, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_pixmap_spots")]
		public static extern int GetSpots(ContextHandle ctx, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_pixmap_components")]
		public static extern int GetComponents(ContextHandle ctx, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_pixmap_height")]
		public static extern int GetHeight(ContextHandle ctx, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_pixmap_width")]
		public static extern int GetWidth(ContextHandle ctx, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_pixmap_x")]
		public static extern int GetPixmapX(ContextHandle ctx, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_pixmap_y")]
		public static extern int GetPixmapY(ContextHandle ctx, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_pixmap_samples")]
		public static extern IntPtr GetSamples(ContextHandle ctx, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_pixmap_stride")]
		public static extern int GetStride(ContextHandle ctx, PixmapHandle pix);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_drop_display_list")]
		internal static extern void DropDisplayList(ContextHandle ctx, IntPtr list);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_display_list")]
		internal static extern IntPtr NewDisplayList(ContextHandle ctx, Rectangle mediaBox);
		#endregion

		#region Text page
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_stext_page")]
		internal static extern IntPtr NewTextPage(ContextHandle ctx, Rectangle mediaBox);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_drop_stext_page")]
		internal static extern void DropTextPage(ContextHandle ctx, IntPtr page);
		#endregion

		#region 图像渲染函数
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_clear_pixmap_with_value")]
		public static extern void ClearPixmap(ContextHandle ctx, PixmapHandle pix, int byteValue);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_disable_device_hints")]
		public static extern void DisableDeviceHints(ContextHandle ctx, DeviceHandle dev, DeviceHints hints);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_enable_device_hints")]
		public static extern void EnableDeviceHints(ContextHandle ctx, DeviceHandle dev, DeviceHints hints);

		#endregion

		#region 文本函数
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_stext_page")]
		public static extern TextPageHandle GetTextPage(ContextHandle ctx, Rectangle mediaBox);

		//[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_highlight_selection")]
		//public static extern int HighlightSelection (ContextHandle ctx, IntPtr page, Rectangle rect, Rectangle hitBBox, fz_quad* quads, int maxHit);

		//[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_copy_selection")]
		//public static extern IntPtr CopySelection (ContextHandle ctx, IntPtr page, Rectangle rect);
		#endregion

		#region Render page content and annotation
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_run_page")]
		public static extern void RunPage(ContextHandle context, PageHandle page, DeviceHandle dev, Matrix transform, ref MuCookie cookie);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_run_page_contents")]
		public static extern void RunPageContents(ContextHandle context, PageHandle page, DeviceHandle dev, Matrix transform, ref MuCookie cookie);

		/// <summary>
		/// (Re)-run a display list through a device.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="list">A display list, created by fz_new_display_list and populated with objects from a page by running fz_run_page on a device obtained from fz_new_list_device.</param>
		/// <param name="dev">The target device to render the display list.</param>
		/// <param name="ctm">Transform to apply to display list contents. May include for example scaling and rotation, see fz_scale, fz_rotate and fz_concat.Set to fz_identity if no transformation is desired.</param>
		/// <param name="scissor">Only the part of the contents of the display list visible within this area will be considered when the list is run through the device.This does not imply for tile objects contained in the display list.</param>
		/// <param name="cookie">Communication mechanism between caller and library running the page.Intended for multi-threaded applications, while single-threaded applications set cookie to NULL.The caller may abort an ongoing page run.Cookie also communicates progress information back to the caller.The fields inside cookie are continually updated while the page is being run.</param>
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_run_display_list")]
		public static extern void RunDisplayList(ContextHandle context, DisplayListHandle list, DeviceHandle dev, Matrix ctm, Rectangle scissor, ref MuCookie cookie);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_run_page_annots")]
		public static extern void RunPageAnnotations(ContextHandle context, PageHandle page, DeviceHandle dev, Matrix transform, ref MuCookie cookie);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_run_page_widgets")]
		public static extern void RunPageWidgets(ContextHandle context, PageHandle page, DeviceHandle dev, Matrix transform, ref MuCookie cookie);
		#endregion

		#region 文档写入器
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_pdf_writer", CharSet = CharSet.Unicode)]
		public static extern IntPtr NewPdfWriter(ContextHandle context, string path, string option);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "CloseDocumentWriter")]
		public static extern bool CloseDocumentWriter(ContextHandle context, IntPtr writer);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "PdfSaveDocument", CharSet = CharSet.Unicode)]
		public static extern bool PdfSaveDocument(ContextHandle context, DocumentHandle document, string filePath, MuPdfWriterOptions options);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_drop_document_writer")]
		public static extern IntPtr DropDocumentWriter(ContextHandle context, IntPtr writer);
		#endregion
	}
}
