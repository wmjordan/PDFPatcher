using System;
using System.IO;
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

	private void LoadPdf(string fileName, string password) {
		if (File.Exists(fileName) == false) {
			throw new FileNotFoundException("找不到 PDF 文件：" + fileName);
		}

		try {
			_sourceStream = new StreamHandle(Context, fileName);
			_document = new DocumentHandle(Context, _sourceStream);
			FilePath = fileName;
			InitPdf(password);
			return;
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

	public MuPage LoadPage(int pageNumber) {
		return new MuPage(Context, _document, pageNumber, ref _cookie);
	}

	private void InitPdf(string password) {
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

	/// <summary>获取文件句柄是否打开。</summary>
	public bool IsDocumentOpened => _document.IsValid() && _sourceStream.IsValid();

	/// <summary>获取文档是否设置了打开密码。</summary>
	public bool NeedsPassword => NativeMethods.NeedsPdfPassword(Context, _document);

	public object SyncObj { get; private set; } = new();

	internal ContextHandle Context { get; }

	private MuPdfDictionary _trailer;

	internal MuPdfDictionary Trailer => _trailer ??= new MuPdfDictionary(Context, NativeMethods.GetTrailer(Context, _document));

	internal MuPdfDictionary Root => Trailer["Root"].AsDictionary();
	internal MuDocumentInfo Info => new(Trailer["Info"].AsDictionary());
	private PageLabelCollection _PageLabels;

	/// <summary>
	///     返回文档的页码标签。
	/// </summary>
	public PageLabelCollection PageLabels => _PageLabels ??= new PageLabelCollection(this);

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

	#endregion
}