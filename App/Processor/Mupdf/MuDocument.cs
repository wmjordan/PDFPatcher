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
		public bool IsAuthenticated { get; private set; }
		public bool IsCancellationPending => _cookie.IsCancellationPending;
		object _SyncObj = new object();
		public object SyncObj => _SyncObj;

		internal ContextHandle Context => _context;
		MuPdfDictionary _trailer;
		internal MuPdfDictionary Trailer => _trailer ?? (_trailer = new MuPdfDictionary(_context, NativeMethods.GetTrailer(_context, _document)));
		internal MuPdfDictionary Root => Trailer["Root"].AsDictionary();
		internal MuDocumentInfo Info => new MuDocumentInfo(Trailer["Info"].AsDictionary());
		PageLabelCollection _PageLabels;
		/// <summary>
		/// 返回文档的页码标签。
		/// </summary>
		public PageLabelCollection PageLabels => _PageLabels ?? (_PageLabels = new PageLabelCollection(this));
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
				if (_sourceStream.IsInvalid) {
					_context.ThrowExceptionIfError();
				}
				_document = new DocumentHandle(_context, _sourceStream);
				if (_document.IsInvalid) {
					_context.ThrowExceptionIfError();
				}
				FilePath = fileName;
				return InitPdf(password);
			}
			catch (Exception) {
				_sourceStream.DisposeHandle();
				_document.DisposeHandle();
				throw new IOException("PDF 文件无效：" + fileName);
			}
		}

		public void SaveAs(string fileName, MuPdfWriterOptions options) {
			if (NativeMethods.PdfSaveDocument(_context, _document, fileName, options) == false) {
				_context.ThrowExceptionIfError();
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
				_trailer = null;
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

		public bool AuthenticatePassword(string password) {
			return IsAuthenticated = 
				NativeMethods.NeedsPdfPassword(_context, _document) == false
				|| String.IsNullOrEmpty(password) == false && NativeMethods.AuthenticatePassword(_context, _document, password);
		}

		int InitPdf(string password) {
			AuthenticatePassword(password);
			return PageCount = NativeMethods.CountPages(_context, _document);
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

	}
}
