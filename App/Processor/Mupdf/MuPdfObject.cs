using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MuPdfSharp;

[DebuggerDisplay("({Kind})")]
public unsafe class MuPdfObject
{
	internal MuPdfObject(ContextHandle context, IntPtr obj) {
		Pointer = obj;
		Context = context;
	}

	internal DocumentHandle Document {
		get {
			IntPtr d = NativeMethods.GetDocument(Context, Pointer);
			return d == IntPtr.Zero ? null : new DocumentHandle(Context, d);
		}
	}

	/// <summary>返回对象的类型。</summary>
	public MuPdfObjectKind Kind =>
		Pointer == IntPtr.Zero ? MuPdfObjectKind.PDF_NULL : (MuPdfObjectKind)NativePointer->_kind;

	/// <summary>返回对象的引用数量。</summary>
	internal int ReferenceCount => Pointer == IntPtr.Zero ? 0 : NativePointer->_referenceCount;

	public int IntegerValue => NativeMethods.ToInteger(Context, Pointer);
	public float FloatValue => NativeMethods.ToSingle(Context, Pointer);
	public string NameValue => NativeMethods.ToName(Context, Pointer);
	public string StringValue => NativeMethods.ToString(Context, Pointer);
	public bool IsNull => NativeMethods.IsNull(Context, Pointer) != 0;

	internal MuPdfDictionary AsDictionary() {
		return new MuPdfDictionary(Context, NativeMethods.ResolveIndirect(Context, Pointer));
	}

	internal MuPdfArray AsArray() {
		return new MuPdfArray(Context, NativeMethods.ResolveIndirect(Context, Pointer));
	}

	protected struct NativeObject
	{
#pragma warning disable 649, 169
		internal short _referenceCount;
		internal byte _kind;
		internal byte _flags;
#pragma warning restore 649, 169
	}

	#region 非托管资源成员

	private NativeObject* NativePointer => (NativeObject*)Pointer;
	internal IntPtr Pointer { get; }

	internal ContextHandle Context { get; }

	#endregion
}

public struct MuIndirectReference
{
	public readonly int Number, Generation;
	public static readonly MuIndirectReference Empty;

	public MuIndirectReference(int number, int generation) {
		Number = number;
		Generation = generation;
	}
}

public enum MuPdfObjectKind : byte
{
	PDF_NULL = 0,
	PDF_ARRAY = (byte)'a'
}

public sealed unsafe class MuPdfDictionary : MuPdfObject
{
	internal MuPdfDictionary(ContextHandle context, IntPtr obj)
		: base(context, obj) {
	}

	private NativeDict* NativePointer => (NativeDict*)Pointer;

	/// <summary>
	///     获取字典中的项目数量。
	/// </summary>
	public int Count => Pointer == IntPtr.Zero ? 0 : NativePointer->_length;

	public KeyValuePair<string, MuPdfObject> this[int index] => new(
		new MuPdfObject(Context, NativeMethods.GetKey(Context, Pointer, index)).NameValue,
		new MuPdfObject(Context, NativeMethods.GetValue(Context, Pointer, index))
	);

	public MuPdfObject this[string key] {
		get => new(Context, NativeMethods.Get(Context, Pointer, key));
		set => NativeMethods.Put(Context, Pointer, key, value.Pointer);
	}

	public MuPdfObject GetValue(int index) {
		return new MuPdfObject(Context, NativeMethods.GetValue(Context, Pointer, index));
	}

	public MuPdfObject Locate(string path) {
		return new MuPdfObject(Context, NativeMethods.Locate(Context, Pointer, path));
	}

	public void Delete(string key) {
		NativeMethods.Delete(Context, Pointer, key);
	}

	public void Delete(int index) {
		NativeMethods.Delete(Context, Pointer, NativeMethods.GetKey(Context, Pointer, index));
	}

	private struct NativeDict
	{
#pragma warning disable 649
		internal NativeObject _nativeObject;
		internal IntPtr _document;
		internal int _parentNum;
		internal int _length;
		internal int _capacity;
		internal IntPtr _items;
#pragma warning restore 649
	}
}

public sealed unsafe class MuPdfArray : MuPdfObject
{
	internal MuPdfArray(ContextHandle context, IntPtr obj)
		: base(context, obj) {
	}

	private NativeArray* NativePointer => (NativeArray*)Pointer;

	/// <summary>
	///     获取数组中的项目数量。
	/// </summary>
	public int Count => Pointer == IntPtr.Zero ? 0 : NativePointer->_length;

	public MuPdfObject this[int index] {
		get => new(Context, NativeMethods.GetArrayItem(Context, Pointer, index));
		set => NativeMethods.SetArrayItem(Context, Pointer, index, value.Pointer);
	}

	public void Add(MuPdfObject obj) {
		NativeMethods.Push(Context, Pointer, obj.Pointer);
	}

	private struct NativeArray
	{
#pragma warning disable 649
		internal NativeObject _nativeObject;
		internal IntPtr _document;
		internal int _parentNum;
		internal int _length;
		internal int _capacity;
		internal IntPtr _items;
#pragma warning restore 649
	}
}