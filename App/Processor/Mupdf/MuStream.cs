using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MuPdfSharp;

internal sealed class MuStream : IDisposable
{
	private const int __CompressionBomb = 100 << 20;

	internal MuStream(byte[] data) {
		ContextHandle ctx = ContextHandle.Create();
		_knownDataLength = data.Length;
		_data = GCHandle.Alloc(data, GCHandleType.Pinned);
		_stream = new StreamHandle(ctx,
			NativeMethods.OpenMemory(ctx, _data.AddrOfPinnedObject(), _knownDataLength));
		_context = ctx;
	}

	internal MuStream(ContextHandle ctx, string fileName) {
		_stream = new StreamHandle(ctx, fileName);
		_knownDataLength = -1;
		_sharedContext = true;
	}

	private MuStream(ContextHandle ctx, StreamHandle stream) {
		_context = ctx;
		_stream = stream;
		NativeMethods.Keep(ctx, stream);
		_sharedContext = true;
	}

	/// <summary>
	///     读取 <paramref name="length" /> 字节到缓冲数组 <see cref="buffer" />。（可能抛出异常）
	/// </summary>
	/// <param name="buffer">放置读取数据的数组。</param>
	/// <param name="length">要读取的数据长度。</param>
	/// <returns>实际读取的长度。</returns>
	public int Read(byte[] buffer, int length) {
		return NativeMethods.Read(_context, _stream, buffer, length);
	}

	/// <summary>
	///     读取流的所有内容到字节数组。（可能抛出异常）
	/// </summary>
	/// <returns>包含流中所有内容的数组。</returns>
	public byte[] ReadAll(int initialSize) {
		if (_knownDataLength > 0) {
			byte[] b = new byte[_knownDataLength];
			NativeMethods.Read(_context, _stream, b, _knownDataLength);
			return b;
		}
		else {
			byte[] b = new byte[initialSize];
			int l;
			using (MemoryStream ms = new(initialSize))
			using (BinaryWriter mw = new(ms)) {
				while ((l = NativeMethods.Read(_context, _stream, b, initialSize)) > 0) {
					mw.Write(b, 0, l);
					if (ms.Length >= __CompressionBomb && ms.Length / 200 > initialSize) {
						throw new IOException("Compression bomb detected.");
					}
				}

				ms.Flush();
				return ms.ToArray();
			}
		}
	}

	/// <summary>
	///     跳转到流的指定位置。
	/// </summary>
	/// <param name="offset">偏移位置。</param>
	/// <param name="origin">跳转方式。</param>
	public void Seek(int offset, SeekOrigin origin) {
		NativeMethods.Seek(_context, _stream, offset,
			origin == SeekOrigin.Begin ? 0 : origin == SeekOrigin.Current ? 1 : 2);
	}

	/// <summary>
	///     将当前流视为以 CCITT Fax 压缩的图像来解压缩。
	/// </summary>
	/// <param name="width">图像宽度。</param>
	/// <param name="height">图像高度。</param>
	/// <param name="k"></param>
	/// <param name="endOfLine"></param>
	/// <param name="encodedByteAlign"></param>
	/// <param name="endOfBlock"></param>
	/// <param name="blackIs1"></param>
	/// <returns>解压缩后的图像数据。</returns>
	public MuStream DecodeTiffFax(int width, int height, int k, bool endOfLine, bool encodedByteAlign,
		bool endOfBlock, bool blackIs1) {
		return new MuStream(
			_context,
			new StreamHandle(_context,
				NativeMethods.DecodeCcittFax(_context, _stream, k, endOfLine ? 1 : 0, encodedByteAlign ? 1 : 0,
					width, height, endOfBlock ? 1 : 0, blackIs1 ? 1 : 0))
		);
	}

	#region 非托管资源成员

	private readonly StreamHandle _stream;
	private readonly ContextHandle _context;
	private GCHandle _data;

	#endregion

	#region 托管资源成员

	private readonly int _knownDataLength;
	private readonly bool _sharedContext;

	/// <summary>获取或设置游标位置。</summary>
	public int Position {
		get => NativeMethods.GetPosition(_context, _stream);
		set => Seek(value, SeekOrigin.Begin);
	}

	#endregion

	#region 实现 IDisposable 接口的属性和方法

	private bool disposed;

	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this); // 抑制析构函数
	}

	/// <summary>释放由 MuPdfPage 占用的资源。</summary>
	/// <param name="disposing">是否手动释放托管资源。</param>
	private void Dispose(bool disposing) {
		if (!disposed) {
			if (disposing) {
				#region 释放托管资源

				//_components.Dispose ();

				#endregion
			}

			#region 释放非托管资源

			// 注意这里不是线程安全的
			if (_stream.IsValid()) {
				_stream.Dispose();
			}

			if (_sharedContext == false && _context.IsValid()) {
				_context.Dispose();
			}

			if (_data.IsAllocated) {
				_data.Free();
			}

			#endregion
		}

		disposed = true;
	}

	// 析构函数只在未调用 Dispose 方法时调用
	// 派生类中不必再提供析构函数
	~MuStream() {
		Dispose(false);
	}

	#endregion
}