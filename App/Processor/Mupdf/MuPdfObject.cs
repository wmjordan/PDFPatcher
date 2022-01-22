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

	/// <summary>
	///     返回对象的类型（如对象为引用，则返回其解除引用后的类型）。
	/// </summary>
	public MuPdfObjectKind UnderlyingKind {
		get {
			MuPdfObject obj = new(Context, NativeMethods.ResolveIndirect(Context, Pointer));
			return obj.Kind;
		}
	}

	public bool BooleanValue => NativeMethods.ToBoolean(Context, Pointer) != 0;
	public int IntegerValue => NativeMethods.ToInteger(Context, Pointer);
	public float FloatValue => NativeMethods.ToSingle(Context, Pointer);
	public string NameValue => NativeMethods.ToName(Context, Pointer);
	public string StringValue => NativeMethods.ToString(Context, Pointer);
	public bool IsNull => NativeMethods.IsNull(Context, Pointer) != 0;
	public bool IsArray => NativeMethods.IsArray(Context, Pointer) != 0;
	public bool IsDictionary => NativeMethods.IsDictionary(Context, Pointer) != 0;

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
	PDF_BOOL = (byte)'b',
	PDF_INT = (byte)'i',
	PDF_REAL = (byte)'f',
	PDF_STRING = (byte)'s',
	PDF_NAME = (byte)'n',
	PDF_ARRAY = (byte)'a',
	PDF_DICT = (byte)'d',
	PDF_INDIRECT = (byte)'r'
}

public sealed unsafe class MuPdfDictionary : MuPdfObject
{
	internal MuPdfDictionary(ContextHandle context, IntPtr obj)
		: base(context, obj) {
	}

	private NativeDict* NativePointer => (NativeDict*)Pointer;

	public int Capacity => Pointer == IntPtr.Zero ? 0 : NativePointer->_capacity;

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

	public MuPdfObject GetKey(int index) {
		return new MuPdfObject(Context, NativeMethods.GetKey(Context, Pointer, index));
	}

	public MuPdfObject GetValue(int index) {
		return new MuPdfObject(Context, NativeMethods.GetValue(Context, Pointer, index));
	}

	public MuPdfObject Locate(string path) {
		return new MuPdfObject(Context, NativeMethods.Locate(Context, Pointer, path));
	}

	public void LocatePut(string path, MuPdfObject value) {
		NativeMethods.LocatePut(Context, Pointer, path, value.Pointer);
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

	public int Capacity => Pointer == IntPtr.Zero ? 0 : NativePointer->_capacity;

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

	public void AddAndDrop(MuPdfObject obj) {
		NativeMethods.PushAndDrop(Context, Pointer, obj.Pointer);
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

public sealed unsafe class MuPdfRef : MuPdfObject
{
	internal MuPdfRef(ContextHandle context, IntPtr obj) : base(context, obj) {
	}

	private NativeRef* NativePointer => (NativeRef*)Pointer;

	public int Number => NativePointer->_Num;
	public int Generation => NativePointer->_Generation;

	private struct NativeRef
	{
#pragma warning disable 649
		internal NativeObject _nativeObject;
		internal IntPtr _document; // Only needed for arrays, dicts and indirects
		internal int _Num;
		internal int _Generation;
#pragma warning restore 649
	}
}