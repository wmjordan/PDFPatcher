using System;
using System.Drawing;

namespace MuPdfSharp
{
	public sealed class MuPage : IDisposable
	{
		#region 非托管资源成员
		private readonly ContextHandle _context;
		private DocumentHandle _document;
		private readonly PageHandle _page;
		private DisplayListHandle _displayList;
		#endregion

		#region 托管资源成员
		static readonly ImageRendererOptions __defaultOptions = new ImageRendererOptions();
		MuCookie _cookie;
		MuTextPage _TextPage;
		bool _flattened;

		/// <summary>获取当前页面的页码。</summary>
		public int PageNumber { get; private set; }
		/// <summary>获取当前页面的尺寸（左下角坐标置为“0,0”）。如需获取页面字典中的原始可视区域，请使用 <see cref="VisualBound"/> 属性。</summary>
		public Rectangle Bound => NativeMethods.BoundPage(_context, _page);
		/// <summary>获取当前页面可视区域的坐标及尺寸。</summary>
		public Rectangle VisualBound => Matrix.Identity.RotateTo(Rotation).Transform(VisualBox);

		public Rectangle ArtBox => LookupPageBox("ArtBox");
		public Rectangle BleedBox => LookupPageBox("BleedBox");
		public Rectangle CropBox => LookupPageBox("CropBox");
		public Rectangle TrimBox => LookupPageBox("TrimBox");
		public Rectangle MediaBox => LookupPageBox("MediaBox");
		public Rectangle VisualBox { get { var b = LookupPageBox("CropBox"); return b.IsEmpty ? LookupPageBox("MediaBox") : b; } }
		public int Rotation => LookupPage("Rotate").IntegerValue;

		public MuTextPage TextPage {
			get {
				PopulateTextPage();
				return _TextPage;
			}
		}

		private unsafe Rectangle LookupPageBox(string name) {
			if (_flattened == false) {
				var d = _page.PageDictionary;
				NativeMethods.FlattenInheritablePageItems(_context, d);
				_flattened = true;
			}
			var a = new MuPdfDictionary(_context, _page.PageDictionary);
			var ra = a[name].AsArray();
			return ra.Count == 4 ? Rectangle.FromArray(a[name]) : Rectangle.Empty;
		}
		private MuPdfObject LookupPage(string name) {
			var a = new MuPdfDictionary(_context, _page.PageDictionary);
			return a[name];
		}
		#endregion

		internal MuPage(ContextHandle context, DocumentHandle document, int pageNumber, ref MuCookie cookie) {
			try {
				_page = new PageHandle(document, pageNumber - 1);
				_document = document;
				_context = context;
				_cookie = cookie;
				PageNumber = pageNumber;
			}
			catch (AccessViolationException) {
				_page.DisposeHandle();
				throw new MuPdfException("无法加载第 " + pageNumber + " 页。");
			}
		}

		///// <summary>
		///// 获取指定区域的文本。
		///// </summary>
		///// <param name="selection">区域。</param>
		///// <returns>区域内的文本。</returns>
		//public string GetSelection (Rectangle selection) {
		//    return Interop.DecodeUtf8String (NativeMethods.CopySelection (_context, GetTextPage (), selection));
		//}

		///// <summary>
		///// 获取指定区域的文本。
		///// </summary>
		///// <param name="selection">区域。</param>
		///// <returns>区域内的文本。</returns>
		//public List<Rectangle> HighlightSelection (Rectangle selection) {
		//	var l = 
		//	return Interop.DecodeUtf8String (NativeMethods.HighlightSelection (_context, _page, selection));
		//}

		/// <summary>
		/// 使用默认的配置渲染页面。
		/// </summary>
		/// <param name="width">页面的宽度。</param>
		/// <param name="height">页面的高度。</param>
		/// <returns>渲染后生成的 <see cref="Bitmap"/>。</returns>
		public FreeImageAPI.FreeImageBitmap RenderPage(int width, int height) {
			return RenderPage(width, height, __defaultOptions);
		}

		/// <summary>
		/// 使用指定的配置渲染页面。
		/// </summary>
		/// <param name="width">页面的宽度。</param>
		/// <param name="height">页面的高度。</param>
		/// <param name="options">渲染选项。</param>
		/// <returns>渲染后生成的 <see cref="FreeImageAPI.FreeImageBitmap"/>。</returns>
		public FreeImageAPI.FreeImageBitmap RenderPage(int width, int height, ImageRendererOptions options) {
			using (var pix = InternalRenderPage(width, height, options)) {
				if (pix != null) {
					return pix.ToFreeImageBitmap(options);
				}
			}
			return null;
		}

		/// <summary>
		/// 使用指定的配置渲染页面。
		/// </summary>
		/// <param name="width">页面的宽度。</param>
		/// <param name="height">页面的高度。</param>
		/// <param name="options">渲染选项。</param>
		/// <returns>渲染后生成的 <see cref="Bitmap"/>。</returns>
		public Bitmap RenderBitmapPage(int width, int height, ImageRendererOptions options) {
			using (var pix = InternalRenderPage(width, height, options)) {
				if (pix != null) {
					return pix.ToBitmap(options);
				}
			}
			return null;
		}

		public MuFont GetFont(MuTextChar character) {
			return new MuFont(_context, character.FontID);
		}
		public MuFont GetFont(MuTextSpan span) {
			return new MuFont(_context, span.FontID);
		}
		private DisplayListHandle GetDisplayList() {
			if (_displayList.IsValid()) {
				return _displayList;
			}
			_displayList = _context.CreateDisplayList(Bound);
			using (var d = new DeviceHandle(_context, _displayList)) {
				NativeMethods.RunPage(_context, _page, d, Matrix.Identity, ref _cookie);
				d.EndOperations();
			}
			if (_cookie.ErrorCount > 0) {
				System.Diagnostics.Debug.WriteLine("在第 " + PageNumber + " 页有 " + _cookie.ErrorCount + " 个错误。");
			}
			return _displayList;
		}

		void PopulateTextPage() {
			if (_TextPage != null) {
				return;
			}
			var b = Bound;
			var text = new TextPageHandle(_context, b);
			try {
				using (var dev = new DeviceHandle(_context, text)) {
					NativeMethods.RunDisplayList(_context, GetDisplayList(), dev, Matrix.Identity, b, ref _cookie);
					dev.EndOperations();
				}
				_TextPage = new MuTextPage(text);
			}
			catch (AccessViolationException) {
				text.DisposeHandle();
				throw;
			}
		}

		private PixmapData InternalRenderPage(int width, int height, ImageRendererOptions options) {
			var b = Bound;
			if (b.Width == 0 || b.Height == 0) {
				return null;
			}
			var ctm = CalculateMatrix(width, height, options);
			var bbox = width > 0 && height > 0 ? new BBox(0, 0, width, height) : ctm.Transform(b).Round;

			PixmapHandle pix = _context.CreatePixmap(options.ColorSpace, bbox);
			if (pix.IsInvalid) {
				_context.ThrowExceptionIfError();
				throw new MuPdfException("无法渲染页面：" + PageNumber);
			}
			try {
				NativeMethods.ClearPixmap(_context, pix, 0xFF);
				using (var dev = new DeviceHandle(_context, pix, Matrix.Identity)) {
					if (options.LowQuality) {
						NativeMethods.EnableDeviceHints(_context, dev, DeviceHints.IgnoreShade | DeviceHints.DontInterperateImages | DeviceHints.NoCache);
					}
					if (_cookie.IsCancellationPending) {
						return null;
					}
					NativeMethods.RunPageContents(_context, _page, dev, ctm, ref _cookie);
					if (options.HideAnnotations == false) {
						NativeMethods.RunPageAnnotations(_context, _page, dev, ctm, ref _cookie);
						NativeMethods.RunPageWidgets(_context, _page, dev, ctm, ref _cookie);
					}
					dev.EndOperations();

					if (_cookie.IsCancellationPending) {
						return null;
					}
					var pd = new PixmapData(_context, pix);
					if (options.TintColor != Color.Transparent) {
						pd.Tint(options.TintColor);
					}
					if (options.Gamma != 1.0f) {
						pd.Gamma(options.Gamma);
					}
					return pd;
				}
			}
			catch (AccessViolationException) {
				pix.DisposeHandle();
				throw new MuPdfException("无法渲染页面：" + PageNumber);
			}
		}

		private Matrix CalculateMatrix(int width, int height, ImageRendererOptions options) {
			float w = width, h = height;
			var b = Bound;
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
					h = (float)width * b.Height / b.Width;
				}
				else if (w == 0) {
					w = (float)height * b.Width / b.Height;
				}
			}
			else if (w == 0 || h == 0) {
				w = b.Width * options.ScaleRatio * options.Dpi / 72;
				h = b.Height * options.ScaleRatio * options.Dpi / 72;
			}

			var ctm = Matrix.Translate(-b.Left, -b.Top).ScaleTo(w / b.Width, h / b.Height).RotateTo(options.Rotation);
			if (options.VerticalFlipImages) {
				ctm = Matrix.Concat(ctm, Matrix.VerticalFlip);
			}
			if (options.HorizontalFlipImages) {
				ctm = Matrix.Concat(ctm, Matrix.HorizontalFlip);
			}
			return ctm;
		}

		/// <summary>
		/// 获取页面内容的实际覆盖范围。
		/// </summary>
		/// <returns>包含页面内容的最小 <see cref="BBox"/>。</returns>
		public Rectangle GetContentBoundary() {
			var b = Bound;
			var o = b;
			using (var dev = new DeviceHandle(_context, ref o)) {
				try {
					var im = Matrix.Identity;
					//NativeMethods.BeginPage (dev, ref b, ref im);
					NativeMethods.RunDisplayList(_context, GetDisplayList(), dev, Matrix.Identity, b, ref _cookie);
					dev.EndOperations();
					//NativeMethods.EndPage (dev);
					return o;
				}
				catch (AccessViolationException) {
					throw new MuPdfException("无法获取页面内容边框：" + PageNumber);
				}
			}
		}

		#region 实现 IDisposable 接口的属性和方法
		private bool disposed;
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);  // 抑制析构函数
		}

		/// <summary>释放由 MuPdfPage 占用的资源。</summary>
		/// <param name="disposing">是否手动释放托管资源。</param>
		void Dispose(bool disposing) {
			if (!disposed) {
				if (disposing) {
					#region 释放托管资源
					_TextPage?.Dispose();
					_TextPage = null;
					#endregion
				}

				#region 释放非托管资源
				// 注意这里不是线程安全的
				//int retry = 0;
				//_cookie.CancelAsync ();
				//while (_cookie.IsRunning && ++retry < 10) {
				//    System.Threading.Thread.Sleep (100);
				//}
				_page.DisposeHandle();
				_displayList.DisposeHandle();
				_document = null;
				#endregion
			}
			disposed = true;
		}

		// 析构函数只在未调用 Dispose 方法时调用
		// 派生类中不必再提供析构函数
		~MuPage() {
			Dispose(false);
		}
		#endregion

		//protected override bool ReleaseHandle () {
		//	NativeMethods.FreePage (_document, this.handle);
		//	return true;
		//}
	}
}
