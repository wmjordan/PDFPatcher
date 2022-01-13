using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using CC = System.Runtime.InteropServices.CallingConvention;

namespace MuPdfSharp;

internal static partial class NativeMethods
{
	#region Object creation

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_new_null")]
	internal static extern IntPtr NewNull(ContextHandle ctx, DocumentHandle doc);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_new_bool")]
	internal static extern IntPtr NewBoolean(ContextHandle ctx, DocumentHandle doc, int boolean);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_new_int")]
	internal static extern IntPtr NewInteger(ContextHandle ctx, DocumentHandle doc, int value);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_new_real")]
	internal static extern IntPtr NewFloat(ContextHandle ctx, DocumentHandle doc, float value);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_new_string", BestFitMapping = false)]
	internal static extern IntPtr NewString(ContextHandle ctx, DocumentHandle doc,
		[MarshalAs(UnmanagedType.LPStr)] string value, int len);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_new_name", BestFitMapping = false)]
	internal static extern IntPtr NewName(ContextHandle ctx, DocumentHandle doc,
		[MarshalAs(UnmanagedType.LPStr)] string name);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_new_array")]
	internal static extern IntPtr NewArray(ContextHandle ctx, DocumentHandle doc, int initCap);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_new_dict")]
	internal static extern IntPtr NewDictionary(ContextHandle ctx, DocumentHandle doc, int initCap);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_new_indirect")]
	internal static extern IntPtr NewIndirectReference(ContextHandle ctx, DocumentHandle doc, int num, int gen);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_new_rect")]
	internal static extern IntPtr NewRect(ContextHandle ctx, DocumentHandle doc, Rectangle rect);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_new_matrix")]
	internal static extern IntPtr NewMatrix(ContextHandle ctx, DocumentHandle doc, Matrix matrix);

	#endregion

	#region Document level object and operation

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_authenticate_password", BestFitMapping = false)]
	internal static extern bool AuthenticatePassword(ContextHandle ctx, DocumentHandle doc,
		[MarshalAs(UnmanagedType.LPStr)] string password);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_trailer")]
	internal static extern IntPtr GetTrailer(ContextHandle ctx, DocumentHandle doc);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_count_pages")]
	internal static extern int CountPages(ContextHandle ctx, DocumentHandle doc);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_flush_warnings")]
	internal static extern void FlushWarnings(ContextHandle ctx);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_open_document", CharSet = CharSet.Unicode)]
	internal static extern DocumentHandle OpenPdfDocument(ContextHandle ctx, string fileName);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "pdf_needs_password")]
	internal static extern bool NeedsPdfPassword(ContextHandle ctx, DocumentHandle doc);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_aa_level")]
	internal static extern int GetAntiAliasLevel(ContextHandle ctx);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_set_aa_level")]
	internal static extern void SetAntiAliasLevel(ContextHandle ctx, int level);

	#endregion

	#region Fonts

	internal static readonly FzLoadSystemFont LoadSystemFont = RequestSystemFont;
	internal static readonly FzLoadSystemCjkFont LoadSystemCjkFont = RequestSystemCjkFont;
	internal static readonly FzLoadSystemFallbackFont LoadSystemFallbackFont = RequestSystemFallbackFont;

	[UnmanagedFunctionPointer(CC.Cdecl)]
	internal delegate IntPtr FzLoadSystemFont(IntPtr ctx, string name, int bold, int italic, int needExactMetrics);

	[UnmanagedFunctionPointer(CC.Cdecl)]
	internal delegate IntPtr FzLoadSystemCjkFont(IntPtr ctx, string name, int registry, int serifDesired);

	[UnmanagedFunctionPointer(CC.Cdecl)]
	internal delegate IntPtr FzLoadSystemFallbackFont(IntPtr ctx, int script, int language, int serif, int bold,
		int italic);

	/// <summary>打开系统内置汉字库功能，支持老的未正确嵌入汉字库的 PDF。</summary>
	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_install_load_system_font_funcs")]
	internal static extern void LoadSystemFontFuncs(ContextHandle ctx,
		FzLoadSystemFont fz_load_system_font_fn,
		FzLoadSystemCjkFont fz_load_system_cjk_font_fn,
		FzLoadSystemFallbackFont fz_load_system_fallback_font_fn
	);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_new_font_from_file", CharSet = CharSet.Ansi)]
	internal static extern IntPtr LoadFontFromFile(IntPtr ctx, [MarshalAs(UnmanagedType.LPStr)] string name,
		[MarshalAs(UnmanagedType.LPStr)] string path, int index, int useGlyphBox);

	private static IntPtr RequestSystemFont(IntPtr ctx, [MarshalAs(UnmanagedType.LPWStr)] string name, int bold,
		int italic,
		int needExactMetrics) {
		Debug.WriteLine("Requesting system font: " + name);
		string f = TryLoadCompatibleFont(name);
		if (f != null) {
			return LoadFontFromFile(ctx, null, f, 0, 0);
			//unsafe {
			//	MuFontFlags* flags = (MuFontFlags*)GetFontFlags(p);
			//	var n = GetFontName(ctx, p);
			//	var f = p.MarshalAs<FzFont>();
			//}
		}

		//var p = LoadFontFromFile(ctx, name, @"C:\Windows\Fonts\simsun.ttc", 1, 1);
		return IntPtr.Zero;
	}

	private static IntPtr RequestSystemCjkFont(IntPtr ctx, string name, int registry, int serifDesired) {
		Debug.WriteLine("Requesting system CJK font: " + name);
		string ff = TryLoadCompatibleFont(name);
		// todo: load fallback font
		return ff != null ? LoadFontFromFile(ctx, name, ff, 0, 1) : IntPtr.Zero;
	}

	private static string TryLoadCompatibleFont(string name) {
		string ff = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System)) + "\\Fonts\\";
		ff = name.StartsWith("SimKai", StringComparison.OrdinalIgnoreCase) ||
		     name.StartsWith("楷体_GB2312", StringComparison.OrdinalIgnoreCase) ||
		     name.StartsWith("Kaiti_GB2312", StringComparison.OrdinalIgnoreCase) ? ff + "simkai.ttf"
			: name.StartsWith("SimSun", StringComparison.OrdinalIgnoreCase) ||
			  name.StartsWith("宋体", StringComparison.OrdinalIgnoreCase) ||
			  name.StartsWith("STSong", StringComparison.OrdinalIgnoreCase) ? ff + "simsun.ttc"
			: name.StartsWith("SimHei", StringComparison.OrdinalIgnoreCase) ||
			  name.StartsWith("黑体", StringComparison.OrdinalIgnoreCase) ? ff + "simhei.ttf"
			: name.StartsWith("SimLi", StringComparison.OrdinalIgnoreCase) ||
			  name.StartsWith("隶书", StringComparison.OrdinalIgnoreCase) ? ff + "simli.ttf"
			: name.StartsWith("SimFang", StringComparison.OrdinalIgnoreCase) ||
			  name.StartsWith("仿宋_GB2312", StringComparison.OrdinalIgnoreCase) ||
			  name.StartsWith("Fangsong_GB2312", StringComparison.OrdinalIgnoreCase) ? ff + "simfang.ttf"
			: name.StartsWith("SimYou", StringComparison.OrdinalIgnoreCase) ||
			  name.StartsWith("幼圆", StringComparison.OrdinalIgnoreCase) ? ff + "simyou.ttf"
			: null;
		return ff;
	}

	private static IntPtr RequestSystemFallbackFont(IntPtr ctx, int script, int language, int serif, int bold,
		int italic) {
		Debug.WriteLine("Requesting fallback font: " + script);
		return IntPtr.Zero;
	}

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_font_ft_face")]
	internal static extern IntPtr GetFontFace(ContextHandle ctx, IntPtr font);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_font_flags")]
	internal static extern MuFontFlags GetFontFlags(IntPtr font);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_font_name")]
	internal static extern unsafe sbyte* GetFontName(ContextHandle ctx, IntPtr font);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_font_bbox")]
	internal static extern BBox GetFontBBox(ContextHandle ctx, IntPtr font);

	#endregion

	#region Stream and file

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_buffer_storage")]
	internal static extern int BufferStorage(ContextHandle ctx, IntPtr buffer, ref IntPtr data);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_open_faxd")]
	internal static extern IntPtr DecodeCcittFax(ContextHandle ctx, StreamHandle stmChain, int k, int endOfLine,
		int encodedByteAlign, int columns, int rows, int endOfBlock, int blackIs1);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_drop_buffer")]
	internal static extern void DropBuffer(ContextHandle ctx, IntPtr buffer);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_tell")]
	internal static extern int GetPosition(ContextHandle ctx, StreamHandle stm);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_keep_stream")]
	internal static extern IntPtr Keep(ContextHandle ctx, StreamHandle stm);

	/// <summary>打开文本流。</summary>
	/// <param name="ctx">MuPDF 上下文指针。</param>
	/// <param name="fileName">要打开的文件名。</param>
	/// <returns>指向 fz_stream 的指针</returns>
	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_open_file_w", CharSet = CharSet.Unicode)]
	internal static extern IntPtr OpenFile(ContextHandle ctx, string fileName);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_open_memory")]
	internal static extern IntPtr OpenMemory(ContextHandle ctx, byte[] data, int len);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_open_memory")]
	internal static extern IntPtr OpenMemory(ContextHandle ctx, IntPtr data, int len);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_read")]
	internal static extern int Read(ContextHandle ctx, StreamHandle stm, byte[] buffer, int len);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_read_all")]
	internal static extern IntPtr ReadAll(ContextHandle ctx, StreamHandle stm, int initial);

	[DllImport(DLL, CallingConvention = CC.Cdecl, EntryPoint = "fz_seek")]
	internal static extern void Seek(ContextHandle ctx, StreamHandle stm, int offset, int whence);

	#endregion
}