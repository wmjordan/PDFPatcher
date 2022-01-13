using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MuPdfSharp
{
	[DebuggerDisplay("({Kind})")]
	public unsafe class MuPdfObject
	{
		#region 非托管资源成员

		readonly IntPtr _object;
		readonly ContextHandle _context;
		NativeObject* NativePointer => (NativeObject*)_object;
		internal IntPtr Pointer => _object;
		internal ContextHandle Context => _context;

		#endregion

		internal DocumentHandle Document {
			get {
				var d = NativeMethods.GetDocument(_context, _object);
				return d == IntPtr.Zero ? null : new DocumentHandle(_context, d);
			}
		}

		/// <summary>返回对象的类型。</summary>
		public MuPdfObjectKind Kind =>
			Pointer == IntPtr.Zero ? MuPdfObjectKind.PDF_NULL : (MuPdfObjectKind)NativePointer->_kind;

		/// <summary>返回对象的引用数量。</summary>
		internal int ReferenceCount => Pointer == IntPtr.Zero ? 0 : NativePointer->_referenceCount;

		/// <summary>
		/// 返回对象的类型（如对象为引用，则返回其解除引用后的类型）。
		/// </summary>
		public MuPdfObjectKind UnderlyingKind {
			get {
				var obj = new MuPdfObject(_context, NativeMethods.ResolveIndirect(_context, _object));
				return obj.Kind;
			}
		}

		public bool BooleanValue => NativeMethods.ToBoolean(_context, _object) != 0;
		public int IntegerValue => NativeMethods.ToInteger(_context, _object);
		public float FloatValue => NativeMethods.ToSingle(_context, _object);
		public string NameValue => NativeMethods.ToName(_context, _object);
		public string StringValue => NativeMethods.ToString(_context, _object);
		public bool IsNull => NativeMethods.IsNull(_context, _object) != 0;
		public bool IsArray => NativeMethods.IsArray(_context, _object) != 0;
		public bool IsDictionary => NativeMethods.IsDictionary(_context, _object) != 0;

		internal MuPdfObject(ContextHandle context, IntPtr obj) {
			_object = obj;
			_context = context;
		}

		internal MuPdfDictionary AsDictionary() {
			return new MuPdfDictionary(_context, NativeMethods.ResolveIndirect(_context, _object));
		}

		internal MuPdfArray AsArray() {
			return new MuPdfArray(_context, NativeMethods.ResolveIndirect(_context, _object));
		}

		protected struct NativeObject
		{
#pragma warning disable 649, 169
			internal short _referenceCount;
			internal byte _kind;
			internal byte _flags;
#pragma warning restore 649, 169
		}
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
		NativeDict* NativePointer => (NativeDict*)Pointer;

		internal MuPdfDictionary(ContextHandle context, IntPtr obj)
			: base(context, obj) {
		}

		public int Capacity => Pointer == IntPtr.Zero ? 0 : NativePointer->_capacity;

		/// <summary>
		/// 获取字典中的项目数量。
		/// </summary>
		public int Count => Pointer == IntPtr.Zero ? 0 : NativePointer->_length;

		public KeyValuePair<string, MuPdfObject> this[int index] => new KeyValuePair<string, MuPdfObject>(
			new MuPdfObject(Context, NativeMethods.GetKey(Context, Pointer, index)).NameValue,
			new MuPdfObject(Context, NativeMethods.GetValue(Context, Pointer, index))
		);

		public MuPdfObject this[string key] {
			get => new MuPdfObject(Context, NativeMethods.Get(Context, Pointer, key));
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

		struct NativeDict
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
		NativeArray* NativePointer => (NativeArray*)Pointer;

		public int Capacity => Pointer == IntPtr.Zero ? 0 : NativePointer->_capacity;

		/// <summary>
		/// 获取数组中的项目数量。
		/// </summary>
		public int Count => Pointer == IntPtr.Zero ? 0 : NativePointer->_length;

		public MuPdfObject this[int index] {
			get => new MuPdfObject(Context, NativeMethods.GetArrayItem(Context, Pointer, index));
			set => NativeMethods.SetArrayItem(Context, Pointer, index, value.Pointer);
		}

		internal MuPdfArray(ContextHandle context, IntPtr obj)
			: base(context, obj) {
		}

		public void Add(MuPdfObject obj) {
			NativeMethods.Push(Context, Pointer, obj.Pointer);
		}

		public void AddAndDrop(MuPdfObject obj) {
			NativeMethods.PushAndDrop(Context, Pointer, obj.Pointer);
		}

		struct NativeArray
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
		NativeRef* NativePointer => (NativeRef*)Pointer;

		public int Number => NativePointer->_Num;
		public int Generation => NativePointer->_Generation;

		internal MuPdfRef(ContextHandle context, IntPtr obj) : base(context, obj) {
		}

		struct NativeRef
		{
#pragma warning disable 649
			internal NativeObject _nativeObject;
			internal IntPtr _document; // Only needed for arrays, dicts and indirects
			internal int _Num;
			internal int _Generation;
#pragma warning restore 649
		}
	}
}