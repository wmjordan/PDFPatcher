using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using FreeImageAPI;
using iTextSharp.text.exceptions;

namespace MuPdfSharp;

public sealed class MuDocument : IDisposable
{
	public MuDocument(string fileName) : this() {
		LoadPdf(fileName, null);
	}

	public MuDocument(string fileName, string password) : this() {
		LoadPdf(fileName, password);
	}

	private MuDocument() {
		Context = ContextHandle.Create();
		_cookie = new MuCookie();

		NativeMethods.LoadSystemFontFuncs(Context, NativeMethods.LoadSystemFont, NativeMethods.LoadSystemCjkFont,
			NativeMethods.LoadSystemFallbackFont);
		PageCount = -1;
	}

	private int LoadPdf(string fileName, string password) {
		if (File.Exists(fileName) == false) {
			throw new FileNotFoundException("找不到 PDF 文件：" + fileName);
		}

		try {
			_sourceStream = new StreamHandle(Context, fileName);
			_document = new DocumentHandle(Context, _sourceStream);
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
	///     释放文档对应的句柄，不再占用 PDF 文件。
	/// </summary>
	public void ReleaseFile() {
		lock (SyncObj) {
			_document.DisposeHandle();
			_sourceStream.DisposeHandle();
		}
	}

	/// <summary>
	///     重新打开文件。
	/// </summary>
	public void Reopen() {
		lock (SyncObj) {
			ReleaseFile();
			LoadPdf(FilePath, null);
		}
	}

	public void AbortAsync() {
		_cookie.CancelAsync();
	}

	/// <summary>
	///     使用指定的尺寸渲染页面。
	/// </summary>
	/// <param name="pageNumber">需要渲染的页码。</param>
	/// <param name="size">图片尺寸。</param>
	/// <returns>渲染的图片。</returns>
	public FreeImageBitmap RenderPage(int pageNumber, Size size) {
		return RenderPage(pageNumber, size.Width, size.Height, null);
	}

	/// <summary>
	///     使用指定的尺寸渲染页面。
	/// </summary>
	/// <param name="pageNumber">需要渲染的页码。</param>
	/// <param name="width">页面宽度。</param>
	/// <param name="height">页面高度。</param>
	/// <returns>渲染的图片。</returns>
	public FreeImageBitmap RenderPage(int pageNumber, int width, int height) {
		return RenderPage(pageNumber, width, height, null);
	}

	/// <summary>
	///     使用渲染配置渲染指定的页面。
	/// </summary>
	/// <param name="pageNumber">要渲染的页码。</param>
	/// <param name="width">页面宽度。</param>
	/// <param name="height">页面高度。</param>
	/// <param name="options">渲染选项。</param>
	/// <returns>成功渲染后的 <see cref="FreeImageAPI.FreeImageBitmap" /> 实例。如传入的页码在有效页码范围内，则返回空引用。</returns>
	public FreeImageBitmap RenderPage(int pageNumber, int width, int height, ImageRendererOptions options) {
		if (pageNumber < 1 || pageNumber > PageCount) {
			return null;
		}

		using MuPage p = LoadPage(pageNumber);
		return options != null ? p.RenderPage(width, height, options) : p.RenderPage(width, height);
	}

	public MuPage LoadPage(int pageNumber) {
		return new MuPage(Context, _document, pageNumber, ref _cookie);
	}

	private int InitPdf(string password) {
		if (NativeMethods.NeedsPdfPassword(Context, _document)) {
			if (string.IsNullOrEmpty(password) == false) {
				if (NativeMethods.AuthenticatePassword(Context, _document, password) == false) {
					throw new BadPasswordException("密码无效。");
				}
			}
			else {
				throw new BadPasswordException("需要提供打开 PDF 文档的密码。");
			}
		}

		PageCount = NativeMethods.CountPages(Context, _document);
		return PageCount;
	}

	private unsafe struct FzFont
	{
#pragma warning disable 649, 169
		private readonly int refs;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		private readonly byte[] name;

		internal FzBuffer* Buffer;
		internal FzFontFlags Flags;
#pragma warning restore 649, 169
		public string Name => Encoding.Default.GetString(name, 0, Array.IndexOf(name, 0));
	}

	private struct FzFontFlags
	{
#pragma warning disable 649
		private uint flag;
#pragma warning restore 649
		private bool IsMono => (flag & 1) > 0;
		private bool IsSerif => (flag & 2) > 0;
		private bool IsBold => (flag & 4) > 0;
		private bool IsItalic => (flag & 8) > 0;
		private bool IsSubstitute => (flag & 16) > 0; /* use substitute metrics */
		private bool IsStretch => (flag & 32) > 0; /* stretch to match PDF metrics */
		private bool IsFakeBold => (flag & 64) > 0; /* synthesize bold */
		private bool IsFakeItalic => (flag & 128) > 0; /* synthesize italic */
		private bool IsForcedHinting => (flag & 256) > 0; /* force hinting for DynaLab fonts */
		private bool HasOpenType => (flag & 512) > 0; /* has opentype shaping tables */
		private bool InvalidBBox => (flag & 1024) > 0;
	}

	private struct FzBuffer
	{
#pragma warning disable 649, 169
		private readonly int refs;
		private readonly IntPtr data;
		internal uint cap, len;
		private readonly int unused_bits;
		private readonly int shared;
#pragma warning restore 649, 169
	}

	#region 非托管资源成员

	private StreamHandle _sourceStream;
	private DocumentHandle _document;

	#endregion

	#region 托管资源成员

	/// <summary>获取所加载文档的路径。</summary>
	public string FilePath { get; private set; }

	/// <summary>获取文档的页数。</summary>
	public int PageCount { get; private set; }

	/// <summary>获取或设置抗锯齿显示级别。</summary>
	public int AntiAlias {
		get => NativeMethods.GetAntiAliasLevel(Context);
		set => NativeMethods.SetAntiAliasLevel(Context, value);
	}

	/// <summary>获取文件句柄是否打开。</summary>
	public bool IsDocumentOpened => _document.IsValid() && _sourceStream.IsValid();

	/// <summary>获取文档是否设置了打开密码。</summary>
	public bool NeedsPassword => NativeMethods.NeedsPdfPassword(Context, _document);

	public bool IsCancellationPending => _cookie.IsCancellationPending;
	public object SyncObj { get; private set; } = new();

	internal ContextHandle Context { get; }

	private MuPdfDictionary _trailer;

	internal MuPdfDictionary Trailer {
		get {
			return _trailer ?? (_trailer = new MuPdfDictionary(Context, NativeMethods.GetTrailer(Context, _document)));
		}
	}

	internal MuPdfDictionary Root => Trailer["Root"].AsDictionary();
	internal MuDocumentInfo Info => new(Trailer["Info"].AsDictionary());
	private PageLabelCollection _PageLabels;

	/// <summary>
	///     返回文档的页码标签。
	/// </summary>
	public PageLabelCollection PageLabels {
		get {
			if (_PageLabels == null) {
				_PageLabels = new PageLabelCollection(this);
			}

			return _PageLabels;
		}
	}

	private MuCookie _cookie;

	#endregion

	#region 实现 IDisposable 接口的属性和方法

	public bool IsDisposed { get; private set; }

	/// <summary>释放由 MuDocument 占用的资源。</summary>
	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>释放由 MuDocument 占用的资源。</summary>
	/// <param name="disposing">是否手动释放托管资源。</param>
	private void Dispose(bool disposing) {
		if (!IsDisposed) {
			if (disposing) {
				#region 释放托管资源

				_trailer = null;
				SyncObj = null;
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
			Context.DisposeHandle();

			#endregion
		}

		IsDisposed = true;
	}

	// 析构函数只在未调用 Dispose 方法时调用
	// 派生类中不必再提供析构函数
	~MuDocument() {
		Dispose(false);
	}

	#endregion

	#region 生成对象

	public MuPdfObject Create(bool value) {
		return new MuPdfObject(Context, NativeMethods.NewBoolean(Context, _document, value ? 1 : 0));
	}

	public MuPdfObject Create(int value) {
		return new MuPdfObject(Context, NativeMethods.NewInteger(Context, _document, value));
	}

	public MuPdfObject Create(float value) {
		return new MuPdfObject(Context, NativeMethods.NewFloat(Context, _document, value));
	}

	public MuPdfObject Create(string value) {
		return new MuPdfObject(Context, NativeMethods.NewString(Context, _document, value, value.Length));
	}

	public MuPdfObject CreateName(string value) {
		return new MuPdfObject(Context, NativeMethods.NewName(Context, _document, value));
	}

	public MuPdfObject Create(int number, int generation) {
		return new MuPdfObject(Context, NativeMethods.NewIndirectReference(Context, _document, number, generation));
	}

	public MuPdfArray Create(Rectangle rect) {
		return new MuPdfArray(Context, NativeMethods.NewRect(Context, _document, rect));
	}

	public MuPdfArray Create(Matrix matrix) {
		return new MuPdfArray(Context, NativeMethods.NewMatrix(Context, _document, matrix));
	}

	public MuPdfArray CreateArray() {
		return new MuPdfArray(Context, NativeMethods.NewArray(Context, _document, 4));
	}

	public MuPdfDictionary CreateDictionary() {
		return new MuPdfDictionary(Context, NativeMethods.NewDictionary(Context, _document, 4));
	}

	#endregion
}