using System;
using System.Runtime.InteropServices;

namespace PDFPatcher.Processor.Mupdf
{
	[StructLayout (LayoutKind.Sequential)]
	internal class GlyphCache
	{
		/*fz_hashtable**/ IntPtr hash;
		int total;
	}

	internal class PdfXref
	{
		IntPtr ptr = IntPtr.Zero;
		PdfXrefStruct data;

		internal int Length { get { return data.Length; } }

		public static PdfXref Open (string fileName, string password) {
			IntPtr p = IntPtr.Zero;
			Open (ref p, fileName, password);
			var r = LoadPageTree (p);
			var xref = new PdfXref ()
			{
				data = Common.PInvokeHelper.UnwrapPointer<PdfXrefStruct> (p),
				ptr = p
			};
			return xref;
		}

		public void Close () {
			Close (ptr);
		}

		#region Native Methods
		[DllImport (Constants.LibMupdf, EntryPoint = "pdf_open_xref", CharSet=CharSet.Ansi)]
		extern static int Open (ref IntPtr /*pdf_xref ***/xrefp, string /*char **/filename, string /*char **/password);

		[DllImport (Constants.LibMupdf, EntryPoint = "pdf_free_xref")]
		extern static void Close (IntPtr /*pdf_xref **/ xref);

		[DllImport (Constants.LibMupdf, EntryPoint = "pdf_load_page_tree")]
		extern static int LoadPageTree (IntPtr /*pdf_xref **/ xref);
		#endregion

		[StructLayout (LayoutKind.Sequential)]
		class PdfXrefStruct
		{
			#region Fields
			IntPtr /*fz_stream **/file;
			int version;
			int startxref;
			int filesize;
			IntPtr /*pdf_crypt **/crypt;
			IntPtr /*fz_obj **/trailer;

			int len;
			IntPtr /*pdf_xrefentry **/table;

			int pagelen;
			int pagecap;
			IntPtr /*fz_obj ***/pageobjs;
			IntPtr /*fz_obj ***/pagerefs;

			IntPtr /*struct pdf_store_s **/store;
			[MarshalAsAttribute (UnmanagedType.ByValArray, SizeConst = 65536, ArraySubType = UnmanagedType.U1)]
			byte[] /*char*/ scratch/*[65536]*/;
			#endregion

			#region Properties
			internal int Version { get { return version; } }
			internal int Length { get { return len; } }
			internal int PageCount { get { return pagelen; } }
			#endregion

		}
	}

}
