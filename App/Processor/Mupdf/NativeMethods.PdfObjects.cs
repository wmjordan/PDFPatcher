using System;
using System.IO;
using System.Runtime.InteropServices;
using CC = System.Runtime.InteropServices.CallingConvention;

namespace MuPdfSharp
{
	partial class NativeMethods
	{
		#region Dictionary
		/// <summary>Returns [0, len) for key found. Returns (-1-len, -1] for key not found, but with insertion point -1-i.</summary>
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_finds")]
		public static extern int FindIndex(ContextHandle ctx, IntPtr dict, [MarshalAs(UnmanagedType.LPStr)] string key);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_len")]
		public static extern int GetDictLength(ContextHandle context, IntPtr dict);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_get_key")]
		public static extern IntPtr GetKey(ContextHandle context, IntPtr dict, int index);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_get_val")]
		public static extern IntPtr GetValue(ContextHandle context, IntPtr dict, int index);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_get")]
		public static extern IntPtr Get(ContextHandle context, IntPtr dict, IntPtr key);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_gets", BestFitMapping = false)]
		public static extern IntPtr Get(ContextHandle context, IntPtr dict, [MarshalAs(UnmanagedType.LPStr)] string key);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_getsa", BestFitMapping = false)]
		public static extern IntPtr GetOrAbbrev(ContextHandle context, IntPtr dict, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string abbrev);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_getp", BestFitMapping = false)]
		public static extern IntPtr Locate(ContextHandle context, IntPtr dict, [MarshalAs(UnmanagedType.LPStr)] string path);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_del")]
		public static extern void Delete(ContextHandle context, IntPtr dict, IntPtr key);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_dels", BestFitMapping = false)]
		public static extern void Delete(ContextHandle context, IntPtr dict, [MarshalAs(UnmanagedType.LPStr)] string key);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_put")]
		public static extern void Put(ContextHandle context, IntPtr dict, IntPtr key, IntPtr value);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_puts", BestFitMapping = false)]
		public static extern void Put(ContextHandle context, IntPtr dict, [MarshalAs(UnmanagedType.LPStr)] string key, IntPtr value);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_puts_drop", BestFitMapping = false)]
		public static extern void PutAndDrop(ContextHandle context, IntPtr dict, [MarshalAs(UnmanagedType.LPStr)] string key, IntPtr value);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_putp", BestFitMapping = false)]
		public static extern void LocatePut(ContextHandle context, IntPtr dict, [MarshalAs(UnmanagedType.LPStr)] string path, IntPtr value);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_putp_drop", BestFitMapping = false)]
		public static extern void LocatePutAndDrop(ContextHandle context, IntPtr dict, [MarshalAs(UnmanagedType.LPStr)] string path, IntPtr value);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_get_rect")]
		public static extern Rectangle DictGetRect(ContextHandle context, IntPtr dict, IntPtr key);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_dict_get_matrix")]
		public static extern Matrix DictGetMatrix(ContextHandle context, IntPtr dict, IntPtr key);
		#endregion

		#region Array
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_array_len")]
		public static extern int GetArrayLength(ContextHandle context, IntPtr pdfArray);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_array_get")]
		public static extern IntPtr GetArrayItem(ContextHandle context, IntPtr pdfArray, int index);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_array_contains")]
		public static extern int Contains(ContextHandle context, IntPtr pdfArray, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_array_put")]
		public static extern void SetArrayItem(ContextHandle context, IntPtr pdfArray, int index, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_array_push")]
		public static extern void Push(ContextHandle context, IntPtr pdfArray, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_array_push_drop")]
		public static extern void PushAndDrop(ContextHandle context, IntPtr pdfArray, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_array_get_rect")]
		public static extern Rectangle ArrayGetRect(ContextHandle context, IntPtr pdfArray, int index);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_array_get_matrix")]
		public static extern Matrix ArrayGetMatrix(ContextHandle context, IntPtr pdfArray, int index);
		#endregion

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_get_bound_document")]
		public static extern IntPtr GetDocument(ContextHandle context, IntPtr obj);

		#region Object type
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_is_null")]
		public static extern int IsNull(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_is_bool")]
		public static extern int IsBoolean(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_is_int")]
		public static extern int IsInteger(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_is_real")]
		public static extern int IsFloat(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_is_number")]
		public static extern int IsNumber(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_is_string")]
		public static extern int IsString(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_is_name")]
		public static extern int IsName(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_is_array")]
		public static extern int IsArray(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_is_dict")]
		public static extern int IsDictionary(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_is_indirect")]
		public static extern int IsIndirectReference(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_is_stream")]
		public static extern int IsStream(ContextHandle context, IntPtr document, int number, int generation);
		#endregion

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_objcmp")]
		public static extern int Compare(ContextHandle context, IntPtr obj, IntPtr obj2);

		#region Object conversion
		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_to_bool")]
		public static extern int ToBoolean(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_to_real")]
		public static extern float ToSingle(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_to_int")]
		public static extern int ToInteger(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_to_name")]
		public static extern string ToName(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_to_str_buf")]
		static extern IntPtr ToStringBytes(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_to_str_len")]
		static extern int ToStringLength(ContextHandle context, IntPtr obj);

		public static string ToString(ContextHandle context, IntPtr obj) {
			var b = ToStringBytes(context, obj);
			var l = ToStringLength(context, obj);
			var bytes = new byte[l];
			Marshal.Copy(b, bytes, 0, l);
			if (bytes.Length >= 2 && (bytes[0] == 255 && bytes[1] == 254 || bytes[0] == 254 && bytes[1] == 255)) {
				using (var ms = new MemoryStream(bytes)) {
					using (var r = new StreamReader(ms, true)) {
						return r.ReadToEnd();
					}
				}
			}
			else {
				var c = new char[l];
				for (int i = 0; i < l; i++) {
					c[i] = (char)pdf_doc_encoding[bytes[i]];
				}
				return new string(c);
			}
		}

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_to_num")]
		static extern int ToIndirectNum(ContextHandle context, IntPtr obj);

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_to_gen")]
		static extern int ToIndirectGen(ContextHandle context, IntPtr obj);

		public static MuIndirectReference ToReference(ContextHandle context, IntPtr obj) {
			return new MuIndirectReference(ToIndirectNum(context, obj), ToIndirectGen(context, obj));
		}

		[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_resolve_indirect")]
		public static extern IntPtr ResolveIndirect(ContextHandle context, IntPtr obj);

		static readonly ushort[] pdf_doc_encoding = {
				/* 0x0 to 0x17 except \t, \n and \r are really undefined */
				0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
				0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
				0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17,
				0x02d8, 0x02c7, 0x02c6, 0x02d9, 0x02dd, 0x02db, 0x02da, 0x02dc,
				0x0020, 0x0021, 0x0022, 0x0023, 0x0024, 0x0025, 0x0026, 0x0027,
				0x0028, 0x0029, 0x002a, 0x002b, 0x002c, 0x002d, 0x002e, 0x002f,
				0x0030, 0x0031, 0x0032, 0x0033, 0x0034, 0x0035, 0x0036, 0x0037,
				0x0038, 0x0039, 0x003a, 0x003b, 0x003c, 0x003d, 0x003e, 0x003f,
				0x0040, 0x0041, 0x0042, 0x0043, 0x0044, 0x0045, 0x0046, 0x0047,
				0x0048, 0x0049, 0x004a, 0x004b, 0x004c, 0x004d, 0x004e, 0x004f,
				0x0050, 0x0051, 0x0052, 0x0053, 0x0054, 0x0055, 0x0056, 0x0057,
				0x0058, 0x0059, 0x005a, 0x005b, 0x005c, 0x005d, 0x005e, 0x005f,
				0x0060, 0x0061, 0x0062, 0x0063, 0x0064, 0x0065, 0x0066, 0x0067,
				0x0068, 0x0069, 0x006a, 0x006b, 0x006c, 0x006d, 0x006e, 0x006f,
				0x0070, 0x0071, 0x0072, 0x0073, 0x0074, 0x0075, 0x0076, 0x0077,
				0x0078, 0x0079, 0x007a, 0x007b, 0x007c, 0x007d, 0x007e, 0x0000,
				0x2022, 0x2020, 0x2021, 0x2026, 0x2014, 0x2013, 0x0192, 0x2044,
				0x2039, 0x203a, 0x2212, 0x2030, 0x201e, 0x201c, 0x201d, 0x2018,
				0x2019, 0x201a, 0x2122, 0xfb01, 0xfb02, 0x0141, 0x0152, 0x0160,
				0x0178, 0x017d, 0x0131, 0x0142, 0x0153, 0x0161, 0x017e, 0x0000,
				0x20ac, 0x00a1, 0x00a2, 0x00a3, 0x00a4, 0x00a5, 0x00a6, 0x00a7,
				0x00a8, 0x00a9, 0x00aa, 0x00ab, 0x00ac, 0x0000, 0x00ae, 0x00af,
				0x00b0, 0x00b1, 0x00b2, 0x00b3, 0x00b4, 0x00b5, 0x00b6, 0x00b7,
				0x00b8, 0x00b9, 0x00ba, 0x00bb, 0x00bc, 0x00bd, 0x00be, 0x00bf,
				0x00c0, 0x00c1, 0x00c2, 0x00c3, 0x00c4, 0x00c5, 0x00c6, 0x00c7,
				0x00c8, 0x00c9, 0x00ca, 0x00cb, 0x00cc, 0x00cd, 0x00ce, 0x00cf,
				0x00d0, 0x00d1, 0x00d2, 0x00d3, 0x00d4, 0x00d5, 0x00d6, 0x00d7,
				0x00d8, 0x00d9, 0x00da, 0x00db, 0x00dc, 0x00dd, 0x00de, 0x00df,
				0x00e0, 0x00e1, 0x00e2, 0x00e3, 0x00e4, 0x00e5, 0x00e6, 0x00e7,
				0x00e8, 0x00e9, 0x00ea, 0x00eb, 0x00ec, 0x00ed, 0x00ee, 0x00ef,
				0x00f0, 0x00f1, 0x00f2, 0x00f3, 0x00f4, 0x00f5, 0x00f6, 0x00f7,
				0x00f8, 0x00f9, 0x00fa, 0x00fb, 0x00fc, 0x00fd, 0x00fe, 0x00ff
			};
		#endregion
	}
}
