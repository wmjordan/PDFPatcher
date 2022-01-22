using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MuPdfSharp
{
	public sealed class MuDocument : IDisposable
	{
		#region 非托管资源成员
		StreamHandle _sourceStream;
		DocumentHandle _document;
		readonly ContextHandle _context;
		#endregion

		#region 托管资源成员
		/// <summary>获取所加载文档的路径。</summary>
		public string FilePath { get; private set; }
		/// <summary>获取文档的页数。</summary>
		public int PageCount { get; private set; }
		/// <summary>获取或设置抗锯齿显示级别。</summary>
		public int AntiAlias {
			get => NativeMethods.GetAntiAliasLevel(_context);
			set => NativeMethods.SetAntiAliasLevel(_context, value);
		}
		/// <summary>获取文件句柄是否打开。</summary>
		public bool IsDocumentOpened => _document.IsValid() && _sourceStream.IsValid();
		/// <summary>获取文档是否设置了打开密码。</summary>
		public bool NeedsPassword => NativeMethods.NeedsPdfPassword(_context, _document);
		public bool IsCancellationPending => _cookie.IsCancellationPending;
		object _SyncObj = new object();
		public object SyncObj => _SyncObj;

		internal ContextHandle Context => _context;
		MuPdfDictionary _trailer;
		internal MuPdfDictionary Trailer {
			get {
				if (_trailer == null) {
					_trailer = new MuPdfDictionary(_context, NativeMethods.GetTrailer(_context, _document));
				}
				return _trailer;
			}
		}
		internal MuPdfDictionary Root => Trailer["Root"].AsDictionary();
		internal MuDocumentInfo Info => new MuDocumentInfo(Trailer["Info"].AsDictionary());
		PageLabelCollection _PageLabels;
		/// <summary>
		/// 返回文档的页码标签。
		/// </summary>
		public PageLabelCollection PageLabels {
			get {
				if (_PageLabels == null) {
					_PageLabels = new PageLabelCollection(this);
				}
				return _PageLabels;
			}
		}
		MuCookie _cookie;
		#endregion

		public MuDocument(string fileName) : this() {
			LoadPdf(fileName, null);
		}
		public MuDocument(string fileName, string password) : this() {
			LoadPdf(fileName, password);
		}
		private MuDocument() {
			_context = ContextHandle.Create();
			_cookie = new MuCookie();

			NativeMethods.LoadSystemFontFuncs(_context, NativeMethods.LoadSystemFont, NativeMethods.LoadSystemCjkFont, NativeMethods.LoadSystemFallbackFont);
			PageCount = -1;
		}

		private int LoadPdf(string fileName, string password) {
			if (File.Exists(fileName) == false) {
				throw new FileNotFoundException("找不到 PDF 文件：" + fileName);
			}
			try {
				_sourceStream = new StreamHandle(_context, fileName);
				_document = new DocumentHandle(_context, _sourceStream);
				FilePath = fileName;
				return InitPdf(password);
			}
			catch (AccessViolationException) {
				_sourceStream.DisposeHandle();
				_document.DisposeHandle();
				throw new IOException("PDF 文件无效：" + fileName);
			}
		}

		/// <summary>
		/// 释放文档对应的句柄，不再占用 PDF 文件。
		/// </summary>
		public void ReleaseFile() {
			lock (_SyncObj) {
				_document.DisposeHandle();
				_sourceStream.DisposeHandle();
			}
		}

		/// <summary>
		/// 重新打开文件。
		/// </summary>
		public void Reopen() {
			lock (_SyncObj) {
				ReleaseFile();
				LoadPdf(FilePath, null);
			}
		}

		public void AbortAsync() {
			_cookie.CancelAsync();
		}
		/// <summary>
		/// 使用指定的尺寸渲染页面。
		/// </summary>
		/// <param name="pageNumber">需要渲染的页码。</param>
		/// <param name="size">图片尺寸。</param>
		/// <returns>渲染的图片。</returns>
		public FreeImageAPI.FreeImageBitmap RenderPage(int pageNumber, System.Drawing.Size size) {
			return RenderPage(pageNumber, size.Width, size.Height, null);
		}
		/// <summary>
		/// 使用指定的尺寸渲染页面。
		/// </summary>
		/// <param name="pageNumber">需要渲染的页码。</param>
		/// <param name="width">页面宽度。</param>
		/// <param name="height">页面高度。</param>
		/// <returns>渲染的图片。</returns>
		public FreeImageAPI.FreeImageBitmap RenderPage(int pageNumber, int width, int height) {
			return RenderPage(pageNumber, width, height, null);
		}
		/// <summary>
		/// 使用渲染配置渲染指定的页面。
		/// </summary>
		/// <param name="pageNumber">要渲染的页码。</param>
		/// <param name="width">页面宽度。</param>
		/// <param name="height">页面高度。</param>
		/// <param name="options">渲染选项。</param>
		/// <returns>成功渲染后的 <see cref="FreeImageAPI.FreeImageBitmap"/> 实例。如传入的页码在有效页码范围内，则返回空引用。</returns>
		public FreeImageAPI.FreeImageBitmap RenderPage(int pageNumber, int width, int height, ImageRendererOptions options) {
			if (pageNumber < 1 || pageNumber > PageCount) {
				return null;
			}
			using (var p = LoadPage(pageNumber)) {
				return options != null ? p.RenderPage(width, height, options) : p.RenderPage(width, height);
			}
		}

		public MuPage LoadPage(int pageNumber) {
			return new MuPage(_context, _document, pageNumber, ref _cookie);
		}

		private int InitPdf(string password) {
			if (NativeMethods.NeedsPdfPassword(_context, _document)) {
				if (String.IsNullOrEmpty(password) == false) {
					if (NativeMethods.AuthenticatePassword(_context, _document, password) == false) {
						throw new iTextSharp.text.exceptions.BadPasswordException("密码无效。");
					}
				}
				else {
					throw new iTextSharp.text.exceptions.BadPasswordException("需要提供打开 PDF 文档的密码。");
				}
			}

			PageCount = NativeMethods.CountPages(_context, _document);
			return PageCount;
		}

		#region 实现 IDisposable 接口的属性和方法
		private bool disposed;
		public bool IsDisposed => disposed;

		/// <summary>释放由 MuDocument 占用的资源。</summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>释放由 MuDocument 占用的资源。</summary>
		/// <param name="disposing">是否手动释放托管资源。</param>
		void Dispose(bool disposing) {
			if (!disposed) {
				if (disposing) {
					#region 释放托管资源
					_trailer = null;
					_SyncObj = null;
					if (_PageLabels != null) {
						_PageLabels.Clear();
						_PageLabels = null;
					}
					#endregion
				}

				#region 释放非托管资源
				// 注意这里不是线程安全的
				_document.DisposeHandle();
				_sourceStream.DisposeHandle();
				_context.DisposeHandle();
				#endregion
			}
			disposed = true;
		}

		// 析构函数只在未调用 Dispose 方法时调用
		// 派生类中不必再提供析构函数
		~MuDocument() {
			Dispose(false);
		}
		#endregion

		#region 生成对象
		public MuPdfObject Create(bool value) {
			return new MuPdfObject(_context, NativeMethods.NewBoolean(_context, _document, value ? 1 : 0));
		}
		public MuPdfObject Create(int value) {
			return new MuPdfObject(_context, NativeMethods.NewInteger(_context, _document, value));
		}
		public MuPdfObject Create(float value) {
			return new MuPdfObject(_context, NativeMethods.NewFloat(_context, _document, value));
		}
		public MuPdfObject Create(string value) {
			return new MuPdfObject(_context, NativeMethods.NewString(_context, _document, value, value.Length));
		}
		public MuPdfObject CreateName(string value) {
			return new MuPdfObject(_context, NativeMethods.NewName(_context, _document, value));
		}
		public MuPdfObject Create(int number, int generation) {
			return new MuPdfObject(_context, NativeMethods.NewIndirectReference(_context, _document, number, generation));
		}
		public MuPdfArray Create(Rectangle rect) {
			return new MuPdfArray(_context, NativeMethods.NewRect(_context, _document, rect));
		}
		public MuPdfArray Create(Matrix matrix) {
			return new MuPdfArray(_context, NativeMethods.NewMatrix(_context, _document, matrix));
		}
		public MuPdfArray CreateArray() {
			return new MuPdfArray(_context, NativeMethods.NewArray(_context, _document, 4));
		}
		public MuPdfDictionary CreateDictionary() {
			return new MuPdfDictionary(_context, NativeMethods.NewDictionary(_context, _document, 4));
		}
		#endregion

		unsafe struct FzFont
		{
#pragma warning disable 649, 169
			readonly int refs;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] readonly byte[] name;
			internal FzBuffer* Buffer;
			internal FzFontFlags Flags;
#pragma warning restore 649, 169
			public string Name => System.Text.Encoding.Default.GetString(name, 0, Array.IndexOf(name, 0));
		}

		struct FzFontFlags
		{
#pragma warning disable 649
			private uint flag;
#pragma warning restore 649
			bool IsMono => (flag & 1) > 0;
			bool IsSerif => (flag & 2) > 0;
			bool IsBold => (flag & 4) > 0;
			bool IsItalic => (flag & 8) > 0;
			bool IsSubstitute => (flag & 16) > 0; /* use substitute metrics */
			bool IsStretch => (flag & 32) > 0; /* stretch to match PDF metrics */
			bool IsFakeBold => (flag & 64) > 0; /* synthesize bold */
			bool IsFakeItalic => (flag & 128) > 0; /* synthesize italic */
			bool IsForcedHinting => (flag & 256) > 0; /* force hinting for DynaLab fonts */
			bool HasOpenType => (flag & 512) > 0; /* has opentype shaping tables */
			bool InvalidBBox => (flag & 1024) > 0;
		}

		struct FzBuffer
		{
#pragma warning disable 649, 169
			readonly int refs;
			readonly IntPtr data;
			internal uint cap, len;
			readonly int unused_bits;
			readonly int shared;
#pragma warning restore 649, 169
		}
	}
}
