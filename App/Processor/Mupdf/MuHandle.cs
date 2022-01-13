using System;
using System.Runtime.ConstrainedExecution;
using System.Security.Permissions;

namespace MuPdfSharp
{
	[SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	abstract class MuHandle : System.Runtime.InteropServices.SafeHandle
	{
		protected MuHandle() : base(IntPtr.Zero, true) { }

		public override bool IsInvalid => handle == IntPtr.Zero;

		public T MarshalAs<T>() where T : struct {
			return handle.MarshalAs<T>();
		}
	}

	sealed class ContextHandle : MuHandle
	{
		private ContextHandle() { }

		/// <summary>
		/// 创建 MuPDF 的 Context 实例。
		/// </summary>
		/// <returns>指向 Context 的指针。</returns>
		internal static ContextHandle Create() {
			return NativeMethods.NewContext();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		protected override bool ReleaseHandle() {
			NativeMethods.DropContext(handle);
			SetHandleAsInvalid();
			return true;
		}

		internal PixmapHandle CreatePixmap(ColorSpace colorspace, int width, int height) {
			return new PixmapHandle(this, FindDeviceColorSpace(colorspace), width, height);
		}

		internal PixmapHandle CreatePixmap(ColorSpace colorspace, BBox box) {
			return new PixmapHandle(this, FindDeviceColorSpace(colorspace), box);
		}

		internal DisplayListHandle CreateDisplayList(Rectangle mediaBox) {
			return new DisplayListHandle(this, mediaBox);
		}

		internal PixmapHandle LoadJpeg2000(byte[] data) {
			var p = NativeMethods.LoadJpeg2000(this, data, data.Length, IntPtr.Zero);
			return new PixmapHandle(this, p);
		}

		IntPtr FindDeviceColorSpace(ColorSpace colorspace) {
			switch (colorspace) {
				case ColorSpace.Rgb: return NativeMethods.GetRgbColorSpace(this);
				case ColorSpace.Bgr: return NativeMethods.GetBgrColorSpace(this);
				case ColorSpace.Cmyk: return NativeMethods.GetCmykColorSpace(this);
				case ColorSpace.Gray: return NativeMethods.GetGrayColorSpace(this);
				default: throw new NotImplementedException(colorspace + " not supported.");
			}
		}
	}

	sealed class DocumentHandle : MuHandle
	{
		readonly ContextHandle _context;
		readonly bool _releaseContext;

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal DocumentHandle(ContextHandle context, StreamHandle stream) {
			handle = NativeMethods.OpenPdfDocumentStream(context, stream);
			_context = context;
			context.DangerousAddRef(ref _releaseContext);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal DocumentHandle(ContextHandle context, IntPtr documentHandle) {
			handle = documentHandle;
			_context = context;
			context.DangerousAddRef(ref _releaseContext);
		}

		internal ContextHandle Context => _context;

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		protected override bool ReleaseHandle() {
			NativeMethods.DropDocument(_context, handle);
			if (_releaseContext) {
				_context.DangerousRelease();
			}

			return true;
		}
	}

	sealed class StreamHandle : MuHandle
	{
		readonly bool _releaseContext;

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal StreamHandle(ContextHandle context, IntPtr handle) {
			this.handle = handle;
			Context = context;
			context.DangerousAddRef(ref _releaseContext);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal StreamHandle(ContextHandle context, string filePath)
			: this(context, NativeMethods.OpenFile(context, filePath)) {
		}

		internal ContextHandle Context { get; }

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		protected override bool ReleaseHandle() {
			NativeMethods.DropStream(Context, handle);
			if (_releaseContext) {
				Context.DangerousRelease();
			}

			return true;
		}

		internal void CloseStream() {
			NativeMethods.DropStream(Context, handle);
		}
	}

	sealed class DeviceHandle : MuHandle
	{
		readonly ContextHandle _context;
		readonly bool _releaseContext;

		DeviceHandle(ContextHandle context, IntPtr handle) : base() {
			this.handle = handle;
			_context = context;
			context.DangerousAddRef(ref _releaseContext);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal DeviceHandle(ContextHandle context, ref Rectangle rectangle)
			: this(context, NativeMethods.NewBBoxDevice(context, ref rectangle)) {
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal DeviceHandle(ContextHandle context, PixmapHandle pixmap, Matrix matrix)
			: this(context, NativeMethods.NewDrawDevice(context, matrix, pixmap)) {
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal DeviceHandle(ContextHandle context, PixmapHandle pixmap, Matrix matrix, ref BBox box)
			: this(context, NativeMethods.NewDrawDevice(context, matrix, pixmap, ref box)) {
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal DeviceHandle(ContextHandle context, DisplayListHandle displayList)
			: this(context, NativeMethods.NewListDevice(context, displayList)) {
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal DeviceHandle(ContextHandle context, TextPageHandle page)
			: this(context, NativeMethods.NewTextDevice(context, page, null)) {
		}

		internal void EndOperations() {
			NativeMethods.CloseDevice(_context, handle);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		protected override bool ReleaseHandle() {
			NativeMethods.DropDevice(_context, handle);
			if (_releaseContext) {
				_context.DangerousRelease();
			}

			return true;
		}
	}

	sealed class PageHandle : MuHandle
	{
		readonly DocumentHandle _document;
		readonly bool _releaseDocument;

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		public PageHandle(DocumentHandle document, int pageNumber) {
			handle = NativeMethods.LoadPage(document.Context, document, pageNumber);
			_document = document;
			document.DangerousAddRef(ref _releaseDocument);
		}

		internal unsafe IntPtr PageDictionary => ((NativePage*)handle)->PageDictionary;

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		protected override bool ReleaseHandle() {
			NativeMethods.DropPage(_document.Context, handle);
			if (_releaseDocument) {
				_document.DangerousRelease();
			}

			return true;
		}

#pragma warning disable 649, 169
		struct NativePage
		{
			readonly NativeFzPage FzPage;
			readonly IntPtr Document;
			internal IntPtr PageDictionary;
			readonly int Transparency;
			readonly int Overprint;
			readonly IntPtr Links;
			readonly IntPtr Annots, AnnotTailp;
			readonly IntPtr Widgets, WidgetTailp;
		}

		struct NativeFzPage
		{
			readonly int Refs;
			readonly IntPtr Document;
			readonly int Chapter;
			readonly int Number;
			readonly int Incomplete;

			readonly IntPtr /*fz_page_drop_page_fn*/
				DropPage;

			readonly IntPtr /*fz_page_bound_page_fn*/
				BoundPage;

			readonly IntPtr /*fz_page_run_page_fn*/
				RunPageContents;

			readonly IntPtr /*fz_page_run_page_fn*/
				RunPageAnnots;

			readonly IntPtr /*fz_page_run_page_fn*/
				RunPageWidgets;

			readonly IntPtr /*fz_page_load_links_fn*/
				LoadLinks;

			readonly IntPtr /*fz_page_page_presentation_fn*/
				PagePresentation;

			readonly IntPtr /*fz_page_control_separation_fn*/
				ControlSeparation;

			readonly IntPtr /*fz_page_separation_disabled_fn*/
				SeparationDisabled;

			readonly IntPtr /*fz_page_separations_fn*/
				GetSeparations;

			readonly IntPtr /*fz_page_uses_overprint_fn*/
				GetOverprint;

			readonly IntPtr /*fz_page_create_link_fn*/
				CreateLink;

			readonly IntPtr /*fz_page ** prev, *next*/
				Prev, Next;
		}
#pragma warning restore 649, 169
	}

	sealed class DisplayListHandle : MuHandle
	{
		readonly ContextHandle _context;
		readonly bool _releaseContext;

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal DisplayListHandle(ContextHandle context, Rectangle mediaBox) {
			handle = NativeMethods.NewDisplayList(context, mediaBox);
			_context = context;
			context.DangerousAddRef(ref _releaseContext);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		protected override bool ReleaseHandle() {
			NativeMethods.DropDisplayList(_context, handle);
			if (_releaseContext) {
				_context.DangerousRelease();
			}

			return true;
		}
	}

	sealed class PixmapHandle : MuHandle
	{
		readonly ContextHandle _context;
		readonly bool _releaseContext;

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal PixmapHandle(ContextHandle context, IntPtr pixmap) {
			handle = pixmap;
			_context = context;
			context.DangerousAddRef(ref _releaseContext);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal PixmapHandle(ContextHandle context, IntPtr colorspace, int width, int height) {
			handle = NativeMethods.NewPixmap(context, colorspace, width, height, IntPtr.Zero, 0);
			_context = context;
			context.DangerousAddRef(ref _releaseContext);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal PixmapHandle(ContextHandle context, IntPtr colorspace, BBox box) {
			handle = NativeMethods.NewPixmap(context, colorspace, box, IntPtr.Zero, 0);
			_context = context;
			context.DangerousAddRef(ref _releaseContext);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		protected override bool ReleaseHandle() {
			NativeMethods.DropPixmap(_context, handle);
			if (_releaseContext) {
				_context.DangerousRelease();
			}

			return true;
		}
	}

	sealed class TextPageHandle : MuHandle
	{
		readonly ContextHandle _context;
		readonly bool _releaseContext;

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		internal TextPageHandle(ContextHandle context, Rectangle mediaBox) {
			handle = NativeMethods.NewTextPage(context, mediaBox);
			_context = context;
			context.DangerousAddRef(ref _releaseContext);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]
		protected override bool ReleaseHandle() {
			NativeMethods.DropTextPage(_context, handle);
			if (_releaseContext) {
				_context.DangerousRelease();
			}

			return true;
		}
	}
}