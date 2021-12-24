using System;
using System.Runtime.InteropServices;

namespace PDFPatcher.Processor.Mupdf
{
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_matrix_s
	{
		// float
		public float a;
		// float
		public float b;
		// float
		public float c;
		// float
		public float d;
		// float
		public float e;
		// float
		public float f;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_point_s
	{
		// float
		public float x;
		// float
		public float y;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_rect_s
	{
		// float
		public float x0;
		// float
		public float y0;
		// float
		public float x1;
		// float
		public float y1;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_bbox_s
	{
		// int
		public int x0;
		// int
		public int y0;
		// int
		public int x1;
		// int
		public int y1;
	}
	[StructLayoutAttribute (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct fz_md5_s
	{
		// unsigned int[4]
		[MarshalAsAttribute (UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] state;
		// unsigned int[2]
		[MarshalAsAttribute (UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
		public uint[] count;
		// unsigned char[64]
		[MarshalAsAttribute (UnmanagedType.ByValTStr, SizeConst = 64)]
		public string buffer;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_sha256_s
	{
		// unsigned int[8]
		[MarshalAsAttribute (UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U4)]
		public uint[] state;
		// unsigned int[2]
		[MarshalAsAttribute (UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
		public uint[] count;
		// Anonymous_070cc860_42fb_4449_9338_672f82fd50b4
		public fz_sha256_s.Buffer buffer;

		[StructLayoutAttribute (LayoutKind.Explicit)]
		public struct Buffer
		{
			// unsigned char[64]
			[MarshalAsAttribute (UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
			[FieldOffsetAttribute (0)]
			public byte[] u8;
			// unsigned int[16]
			[MarshalAsAttribute (UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U4)]
			[FieldOffsetAttribute (0)]
			public uint[] u32;
		}
	}
	[StructLayoutAttribute (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct fz_arc4_s
	{
		// unsigned int
		public uint x;
		// unsigned int
		public uint y;
		// unsigned char[256]
		[MarshalAsAttribute (UnmanagedType.ByValTStr, SizeConst = 256)]
		public string state;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_aes_s
	{
		// int
		public int nr;
		// unsigned int*
		public IntPtr rk;
		// unsigned int[68]
		[MarshalAsAttribute (UnmanagedType.ByValArray, SizeConst = 68, ArraySubType = UnmanagedType.U4)]
		public uint[] buf;
	}
	public enum fz_objkind_e
	{
		FZ_NULL,
		FZ_BOOL,
		FZ_INT,
		FZ_REAL,
		FZ_STRING,
		FZ_NAME,
		FZ_ARRAY,
		FZ_DICT,
		FZ_INDIRECT,
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_buffer_s
	{
		// int
		public int refs;
		// unsigned char*
		[MarshalAsAttribute (UnmanagedType.LPStr)]
		public string data;
		// int
		public int cap;
		// int
		public int len;
	}
	public enum fz_blendmode_e
	{
		FZ_BNORMAL,
		FZ_BMULTIPLY,
		FZ_BSCREEN,
		FZ_BOVERLAY,
		FZ_BDARKEN,
		FZ_BLIGHTEN,
		FZ_BCOLORDODGE,
		FZ_BCOLORBURN,
		FZ_BHARDLIGHT,
		FZ_BSOFTLIGHT,
		FZ_BDIFFERENCE,
		FZ_BEXCLUSION,
		FZ_BHUE,
		FZ_BSATURATION,
		FZ_BCOLOR,
		FZ_BLUMINOSITY,
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_pixmap_s
	{
		// int
		public int refs;
		// int
		public int x;
		// int
		public int y;
		// int
		public int w;
		// int
		public int h;
		// int
		public int n;
		// fz_pixmap*
		public IntPtr mask;
		// int
		public int interpolate;
		// fz_colorspace*
		public IntPtr colorspace;
		// unsigned char*
		[MarshalAsAttribute (UnmanagedType.LPStr)]
		public string samples;
		// int
		public int freesamples;
	}
	[StructLayoutAttribute (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct fz_colorspace_s
	{
		// Return Type: void
		//param0: fz_colorspace*
		//src: float*
		//xyz: float*
		public delegate void ToXyzDelegate (ref fz_colorspace_s param0, ref float src, ref float xyz);
		// Return Type: void
		//param0: fz_colorspace*
		//xyz: float*
		//dst: float*
		public delegate void FromXyzDelegate (ref fz_colorspace_s param0, ref float xyz, ref float dst);
		// Return Type: void
		//param0: fz_colorspace*
		public delegate void FreeDataDelegate (ref fz_colorspace_s param0);

		// int
		public int refs;
		// char[16]
		[MarshalAsAttribute (UnmanagedType.ByValTStr, SizeConst = 16)]
		public string name;
		// int
		public int n;
		// fz_colorspace_s_toxyz
		public ToXyzDelegate toxyz;
		// fz_colorspace_s_fromxyz
		public FromXyzDelegate fromxyz;
		// fz_colorspace_s_freedata
		public FreeDataDelegate freedata;
		// void*
		public IntPtr data;
	}

	public enum fz_pathelkind_e
	{
		FZ_MOVETO,
		FZ_LINETO,
		FZ_CURVETO,
		FZ_CLOSEPATH,
	}
	[StructLayoutAttribute (LayoutKind.Explicit)]
	public struct fz_pathel_s
	{
		// fz_pathelkind->fz_pathelkind_e
		[FieldOffsetAttribute (0)]
		public fz_pathelkind_e k;
		// float
		[FieldOffsetAttribute (0)]
		public float v;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_path_s
	{
		// int
		public int len;
		// int
		public int cap;
		// fz_pathel*
		public IntPtr els;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_strokestate_s
	{
		// int
		public int linecap;
		// int
		public int linejoin;
		// float
		public float linewidth;
		// float
		public float miterlimit;
		// float
		public float dashphase;
		// int
		public int dashlen;
		// float[32]
		[MarshalAsAttribute (UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.R4)]
		public float[] dashlist;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_textel_s
	{
		// float
		public float x;
		// float
		public float y;
		// int
		public int gid;
		// int
		public int ucs;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_shade_s
	{
		// int
		public int refs;
		// fz_rect->fz_rect_s
		public fz_rect_s bbox;
		// fz_colorspace*
		public IntPtr cs;
		// fz_matrix->fz_matrix_s
		public fz_matrix_s matrix;
		// int
		public int usebackground;
		// float[]
		public float[] background;
		// int
		public int usefunction;
		// float[]
		public float[] function;
		// int
		public int type;
		// int[2]
		[MarshalAsAttribute (UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I4)]
		public int[] extend;
		// int
		public int meshlen;
		// int
		public int meshcap;
		// float*
		public IntPtr mesh;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_edge_s
	{
		// int
		public int x;
		// int
		public int e;
		// int
		public int h;
		// int
		public int y;
		// int
		public int adjup;
		// int
		public int adjdown;
		// int
		public int xmove;
		// int
		public int xdir;
		// int
		public int ydir;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_gel_s
	{
		// fz_bbox->fz_bbox_s
		public fz_bbox_s clip;
		// fz_bbox->fz_bbox_s
		public fz_bbox_s bbox;
		// int
		public int cap;
		// int
		public int len;
		// fz_edge*
		public IntPtr edges;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_ael_s
	{
		// int
		public int cap;
		// int
		public int len;
		// fz_edge**
		public IntPtr edges;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct fz_textchar_s
	{
		// int
		public int c;
		// fz_bbox->fz_bbox_s
		public fz_bbox_s bbox;
	}
	public enum fz_displaycommand_e
	{
		FZ_CMDFILLPATH,
		FZ_CMDSTROKEPATH,
		FZ_CMDCLIPPATH,
		FZ_CMDCLIPSTROKEPATH,
		FZ_CMDFILLTEXT,
		FZ_CMDSTROKETEXT,
		FZ_CMDCLIPTEXT,
		FZ_CMDCLIPSTROKETEXT,
		FZ_CMDIGNORETEXT,
		FZ_CMDFILLSHADE,
		FZ_CMDFILLIMAGE,
		FZ_CMDFILLIMAGEMASK,
		FZ_CMDCLIPIMAGEMASK,
		FZ_CMDPOPCLIP,
		FZ_CMDBEGINMASK,
		FZ_CMDENDMASK,
		FZ_CMDBEGINGROUP,
		FZ_CMDENDGROUP,
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct pdf_cryptfilter_s
	{
		// int
		public int method;
		// int
		public int length;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct pdf_range_s
	{
		// unsigned short
		public ushort low;
		// unsigned short
		public ushort extentflags;
		// unsigned int
		public uint offset;
	}
	[StructLayoutAttribute (LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct pdf_cmap_s
	{
		[StructLayoutAttribute (LayoutKind.Sequential)]
		public struct Cspace
		{
			// unsigned short
			public ushort n;
			// unsigned short
			public ushort low;
			// unsigned short
			public ushort high;
		}

		// int
		public int refs;
		// char[32]
		[MarshalAsAttribute (UnmanagedType.ByValTStr, SizeConst = 32)]
		public string cmapname;
		// char[32]
		[MarshalAsAttribute (UnmanagedType.ByValTStr, SizeConst = 32)]
		public string usecmapname;
		// pdf_cmap*
		public IntPtr usecmap;
		// int
		public int wmode;
		// int
		public int ncspace;
		// Anonymous_f39df5ad_2fc5_4167_911a_7b31839b08a0[40]
		[MarshalAsAttribute (UnmanagedType.ByValArray, SizeConst = 40, ArraySubType = UnmanagedType.Struct)]
		public pdf_cmap_s.Cspace[] cspace;
		// int
		public int rlen;
		// int
		public int rcap;
		// pdf_range*
		public IntPtr ranges;
		// int
		public int tlen;
		// int
		public int tcap;
		// unsigned short*
		public IntPtr table;

	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct pdf_hmtx_s
	{
		// unsigned short
		public ushort lo;
		// unsigned short
		public ushort hi;
		// int
		public int w;
	}
	[StructLayoutAttribute (LayoutKind.Sequential)]
	public struct pdf_vmtx_s
	{
		// unsigned short
		public ushort lo;
		// unsigned short
		public ushort hi;
		// short
		public short x;
		// short
		public short y;
		// short
		public short w;
	}
	public enum pdf_linkkind_e
	{
		// PDF_LGOTO -> 0
		PDF_LGOTO = 0,
		PDF_LURI,
		PDF_LLAUNCH,
		PDF_LNAMED,
		PDF_LACTION,
	}
	
	public partial class NativeMethods
	{
		const string DLL = "libmupdf.dll";
		// Return Type: void*
		//size: int
		[DllImportAttribute (DLL, EntryPoint = "fz_malloc")]
		public static extern IntPtr fz_malloc (int size);
		// Return Type: void*
		//count: int
		//size: int
		[DllImportAttribute (DLL, EntryPoint = "fz_calloc")]
		public static extern IntPtr fz_calloc (int count, int size);
		// Return Type: void*
		//p: void*
		//count: int
		//size: int
		[DllImportAttribute (DLL, EntryPoint = "fz_realloc")]
		public static extern IntPtr fz_realloc (IntPtr p, int count, int size);
		// Return Type: void
		//p: void*
		[DllImportAttribute (DLL, EntryPoint = "fz_free")]
		public static extern void fz_free (IntPtr p);
		// Return Type: char*
		//s: char*
		[DllImportAttribute (DLL, EntryPoint = "fz_strdup")]
		public static extern IntPtr fz_strdup (IntPtr s);
		// Return Type: int
		[DllImportAttribute (DLL, EntryPoint = "fz_isbigendian")]
		public static extern int fz_isbigendian ();
		// Return Type: char*
		//stringp: char**
		//delim: char*
		[DllImportAttribute (DLL, EntryPoint = "fz_strsep")]
		public static extern IntPtr fz_strsep (ref IntPtr stringp, [InAttribute ()] [MarshalAsAttribute (UnmanagedType.LPStr)] string delim);
		// Return Type: int
		//dst: char*
		//src: char*
		//n: int
		[DllImportAttribute (DLL, EntryPoint = "fz_strlcpy")]
		public static extern int fz_strlcpy (IntPtr dst, [InAttribute ()] [MarshalAsAttribute (UnmanagedType.LPStr)] string src, int n);
		// Return Type: int
		//dst: char*
		//src: char*
		//n: int
		[DllImportAttribute (DLL, EntryPoint = "fz_strlcat")]
		public static extern int fz_strlcat (IntPtr dst, [InAttribute ()] [MarshalAsAttribute (UnmanagedType.LPStr)] string src, int n);
		// Return Type: fz_matrix->fz_matrix_s
		//one: fz_matrix->fz_matrix_s
		//two: fz_matrix->fz_matrix_s
		[DllImportAttribute (DLL, EntryPoint = "fz_concat")]
		public static extern fz_matrix_s fz_concat (fz_matrix_s one, fz_matrix_s two);
		// Return Type: fz_matrix->fz_matrix_s
		//sx: float
		//sy: float
		[DllImportAttribute (DLL, EntryPoint = "fz_scale")]
		public static extern fz_matrix_s fz_scale (float sx, float sy);
		// Return Type: fz_matrix->fz_matrix_s
		//theta: float
		[DllImportAttribute (DLL, EntryPoint = "fz_rotate")]
		public static extern fz_matrix_s fz_rotate (float theta);
		// Return Type: fz_matrix->fz_matrix_s
		//tx: float
		//ty: float
		[DllImportAttribute (DLL, EntryPoint = "fz_translate")]
		public static extern fz_matrix_s fz_translate (float tx, float ty);
		// Return Type: fz_matrix->fz_matrix_s
		//m: fz_matrix->fz_matrix_s
		[DllImportAttribute (DLL, EntryPoint = "fz_invertmatrix")]
		public static extern fz_matrix_s fz_invertmatrix (fz_matrix_s m);
		// Return Type: int
		//m: fz_matrix->fz_matrix_s
		[DllImportAttribute (DLL, EntryPoint = "fz_isrectilinear")]
		public static extern int fz_isrectilinear (fz_matrix_s m);
		// Return Type: float
		//m: fz_matrix->fz_matrix_s
		[DllImportAttribute (DLL, EntryPoint = "fz_matrixexpansion")]
		public static extern float fz_matrixexpansion (fz_matrix_s m);
		// Return Type: fz_bbox->fz_bbox_s
		//r: fz_rect->fz_rect_s
		[DllImportAttribute (DLL, EntryPoint = "fz_roundrect")]
		public static extern fz_bbox_s fz_roundrect (fz_rect_s r);
		// Return Type: fz_bbox->fz_bbox_s
		//a: fz_bbox->fz_bbox_s
		//b: fz_bbox->fz_bbox_s
		[DllImportAttribute (DLL, EntryPoint = "fz_intersectbbox")]
		public static extern fz_bbox_s fz_intersectbbox (fz_bbox_s a, fz_bbox_s b);
		// Return Type: fz_bbox->fz_bbox_s
		//a: fz_bbox->fz_bbox_s
		//b: fz_bbox->fz_bbox_s
		[DllImportAttribute (DLL, EntryPoint = "fz_unionbbox")]
		public static extern fz_bbox_s fz_unionbbox (fz_bbox_s a, fz_bbox_s b);
		// Return Type: fz_point->fz_point_s
		//m: fz_matrix->fz_matrix_s
		//p: fz_point->fz_point_s
		[DllImportAttribute (DLL, EntryPoint = "fz_transformpoint")]
		public static extern fz_point_s fz_transformpoint (fz_matrix_s m, fz_point_s p);
		// Return Type: fz_point->fz_point_s
		//m: fz_matrix->fz_matrix_s
		//p: fz_point->fz_point_s
		[DllImportAttribute (DLL, EntryPoint = "fz_transformvector")]
		public static extern fz_point_s fz_transformvector (fz_matrix_s m, fz_point_s p);
		// Return Type: fz_rect->fz_rect_s
		//m: fz_matrix->fz_matrix_s
		//r: fz_rect->fz_rect_s
		[DllImportAttribute (DLL, EntryPoint = "fz_transformrect")]
		public static extern fz_rect_s fz_transformrect (fz_matrix_s m, fz_rect_s r);
		// Return Type: fz_bbox->fz_bbox_s
		//m: fz_matrix->fz_matrix_s
		//b: fz_bbox->fz_bbox_s
		[DllImportAttribute (DLL, EntryPoint = "fz_transformbbox")]
		public static extern fz_bbox_s fz_transformbbox (fz_matrix_s m, fz_bbox_s b);
		// Return Type: void
		//state: fz_md5*
		[DllImportAttribute (DLL, EntryPoint = "fz_md5init")]
		public static extern void fz_md5init (ref fz_md5_s state);
		// Return Type: void
		//state: fz_md5*
		//input: char*
		//inlen: int
		[DllImportAttribute (DLL, EntryPoint = "fz_md5update")]
		public static extern void fz_md5update (ref fz_md5_s state, [InAttribute ()] [MarshalAsAttribute (UnmanagedType.LPStr)] string input, int inlen);
		// Return Type: void
		//state: fz_sha256*
		[DllImportAttribute (DLL, EntryPoint = "fz_sha256init")]
		public static extern void fz_sha256init (ref fz_sha256_s state);
		// Return Type: void
		//state: fz_sha256*
		//input: char*
		//inlen: unsigned int
		[DllImportAttribute (DLL, EntryPoint = "fz_sha256update")]
		public static extern void fz_sha256update (ref fz_sha256_s state, [InAttribute ()] [MarshalAsAttribute (UnmanagedType.LPStr)] string input, uint inlen);
		// Return Type: void
		//state: fz_arc4*
		//key: char*
		//len: int
		[DllImportAttribute (DLL, EntryPoint = "fz_arc4init")]
		public static extern void fz_arc4init (ref fz_arc4_s state, [InAttribute ()] [MarshalAsAttribute (UnmanagedType.LPStr)] string key, int len);
		// Return Type: void
		//state: fz_arc4*
		//dest: unsigned char*
		//src: char*
		//len: int
		[DllImportAttribute (DLL, EntryPoint = "fz_arc4encrypt")]
		public static extern void fz_arc4encrypt (ref fz_arc4_s state, IntPtr dest, [InAttribute ()] [MarshalAsAttribute (UnmanagedType.LPStr)] string src, int len);
		// Return Type: void
		//ctx: fz_aes*
		//key: char*
		//keysize: int
		[DllImportAttribute (DLL, EntryPoint = "aes_setkey_enc")]
		public static extern void aes_setkey_enc (ref fz_aes_s ctx, [InAttribute ()] [MarshalAsAttribute (UnmanagedType.LPStr)] string key, int keysize);
		// Return Type: void
		//ctx: fz_aes*
		//key: char*
		//keysize: int
		[DllImportAttribute (DLL, EntryPoint = "aes_setkey_dec")]
		public static extern void aes_setkey_dec (ref fz_aes_s ctx, [InAttribute ()] [MarshalAsAttribute (UnmanagedType.LPStr)] string key, int keysize);
		// Return Type: fz_buffer*
		//size: int
		[DllImportAttribute (DLL, EntryPoint = "fz_newbuffer")]
		public static extern IntPtr fz_newbuffer (int size);
		// Return Type: fz_buffer*
		//buf: fz_buffer*
		[DllImportAttribute (DLL, EntryPoint = "fz_keepbuffer")]
		public static extern IntPtr fz_keepbuffer (ref fz_buffer_s buf);
		// Return Type: void
		//buf: fz_buffer*
		[DllImportAttribute (DLL, EntryPoint = "fz_dropbuffer")]
		public static extern void fz_dropbuffer (ref fz_buffer_s buf);
		// Return Type: void
		//buf: fz_buffer*
		//size: int
		[DllImportAttribute (DLL, EntryPoint = "fz_resizebuffer")]
		public static extern void fz_resizebuffer (ref fz_buffer_s buf, int size);
		// Return Type: void
		//buf: fz_buffer*
		[DllImportAttribute (DLL, EntryPoint = "fz_growbuffer")]
		public static extern void fz_growbuffer (ref fz_buffer_s buf);
		// Return Type: fz_pixmap*
		//colorspace: fz_colorspace*
		//x: int
		//y: int
		//w: int
		//h: int
		//samples: unsigned char*
		[DllImportAttribute (DLL, EntryPoint = "fz_newpixmapwithdata")]
		public static extern IntPtr fz_newpixmapwithdata (ref fz_colorspace_s colorspace, int x, int y, int w, int h, IntPtr samples);
		// Return Type: fz_pixmap*
		//param0: fz_colorspace*
		//bbox: fz_bbox->fz_bbox_s
		[DllImportAttribute (DLL, EntryPoint = "fz_newpixmapwithrect")]
		public static extern IntPtr fz_newpixmapwithrect (ref fz_colorspace_s param0, fz_bbox_s bbox);
		// Return Type: fz_pixmap*
		//param0: fz_colorspace*
		//x: int
		//y: int
		//w: int
		//h: int
		[DllImportAttribute (DLL, EntryPoint = "fz_newpixmap")]
		public static extern IntPtr fz_newpixmap (ref fz_colorspace_s param0, int x, int y, int w, int h);
		// Return Type: fz_pixmap*
		//pix: fz_pixmap*
		[DllImportAttribute (DLL, EntryPoint = "fz_keeppixmap")]
		public static extern IntPtr fz_keeppixmap (ref fz_pixmap_s pix);
		// Return Type: void
		//pix: fz_pixmap*
		[DllImportAttribute (DLL, EntryPoint = "fz_droppixmap")]
		public static extern void fz_droppixmap (ref fz_pixmap_s pix);
		// Return Type: void
		//pix: fz_pixmap*
		[DllImportAttribute (DLL, EntryPoint = "fz_clearpixmap")]
		public static extern void fz_clearpixmap (ref fz_pixmap_s pix);
		// Return Type: void
		//pix: fz_pixmap*
		//value: int
		[DllImportAttribute (DLL, EntryPoint = "fz_clearpixmapwithcolor")]
		public static extern void fz_clearpixmapwithcolor (ref fz_pixmap_s pix, int value);
		// Return Type: fz_pixmap*
		//gray: fz_pixmap*
		//luminosity: int
		[DllImportAttribute (DLL, EntryPoint = "fz_alphafromgray")]
		public static extern IntPtr fz_alphafromgray (ref fz_pixmap_s gray, int luminosity);
		// Return Type: fz_bbox->fz_bbox_s
		//pix: fz_pixmap*
		[DllImportAttribute (DLL, EntryPoint = "fz_boundpixmap")]
		public static extern fz_bbox_s fz_boundpixmap (ref fz_pixmap_s pix);
		// Return Type: fz_pixmap*
		//src: fz_pixmap*
		//xdenom: int
		//ydenom: int
		[DllImportAttribute (DLL, EntryPoint = "fz_scalepixmap")]
		public static extern IntPtr fz_scalepixmap (ref fz_pixmap_s src, int xdenom, int ydenom);
		// Return Type: fz_pixmap*
		//src: fz_pixmap*
		//x: float
		//y: float
		//w: float
		//h: float
		[DllImportAttribute (DLL, EntryPoint = "fz_smoothscalepixmap")]
		public static extern IntPtr fz_smoothscalepixmap (ref fz_pixmap_s src, float x, float y, float w, float h);
		// Return Type: fz_error->int
		//pixmap: fz_pixmap*
		//filename: char*
		[DllImportAttribute (DLL, EntryPoint = "fz_writepnm")]
		public static extern int fz_writepnm (ref fz_pixmap_s pixmap, IntPtr filename);
		// Return Type: fz_error->int
		//pixmap: fz_pixmap*
		//filename: char*
		//savealpha: int
		[DllImportAttribute (DLL, EntryPoint = "fz_writepam")]
		public static extern int fz_writepam (ref fz_pixmap_s pixmap, IntPtr filename, int savealpha);
		// Return Type: fz_error->int
		//pixmap: fz_pixmap*
		//filename: char*
		//savealpha: int
		[DllImportAttribute (DLL, EntryPoint = "fz_writepng")]
		public static extern int fz_writepng (ref fz_pixmap_s pixmap, IntPtr filename, int savealpha);
		// Return Type: fz_error->int
		//imgp: fz_pixmap**
		//data: unsigned char*
		//size: int
		[DllImportAttribute (DLL, EntryPoint = "fz_loadjpximage")]
		public static extern int fz_loadjpximage (ref IntPtr imgp, IntPtr data, int size);
		// Return Type: fz_colorspace*
		//name: char*
		//n: int
		[DllImportAttribute (DLL, EntryPoint = "fz_newcolorspace")]
		public static extern IntPtr fz_newcolorspace (IntPtr name, int n);
		// Return Type: fz_colorspace*
		//cs: fz_colorspace*
		[DllImportAttribute (DLL, EntryPoint = "fz_keepcolorspace")]
		public static extern IntPtr fz_keepcolorspace (ref fz_colorspace_s cs);
		// Return Type: void
		//cs: fz_colorspace*
		[DllImportAttribute (DLL, EntryPoint = "fz_dropcolorspace")]
		public static extern void fz_dropcolorspace (ref fz_colorspace_s cs);
		// Return Type: fz_colorspace*
		//name: char*
		[DllImportAttribute (DLL, EntryPoint = "fz_getstaticcolorspace")]
		public static extern IntPtr fz_getstaticcolorspace (IntPtr name);
		// Return Type: void
		//srcs: fz_colorspace*
		//srcv: float*
		//dsts: fz_colorspace*
		//dstv: float*
		[DllImportAttribute (DLL, EntryPoint = "fz_convertcolor")]
		public static extern void fz_convertcolor (ref fz_colorspace_s srcs, ref float srcv, ref fz_colorspace_s dsts, ref float dstv);
		// Return Type: void
		//src: fz_pixmap*
		//dst: fz_pixmap*
		[DllImportAttribute (DLL, EntryPoint = "fz_convertpixmap")]
		public static extern void fz_convertpixmap (ref fz_pixmap_s src, ref fz_pixmap_s dst);
		// Return Type: char*
		//err: int
		[DllImportAttribute (DLL, EntryPoint = "ft_errorstring")]
		public static extern IntPtr ft_errorstring (int err);
		// Return Type: fz_path*
		[DllImportAttribute (DLL, EntryPoint = "fz_newpath")]
		public static extern IntPtr fz_newpath ();
		// Return Type: void
		//param0: fz_path*
		//x: float
		//y: float
		[DllImportAttribute (DLL, EntryPoint = "fz_moveto")]
		public static extern void fz_moveto (ref fz_path_s param0, float x, float y);
		// Return Type: void
		//param0: fz_path*
		//x: float
		//y: float
		[DllImportAttribute (DLL, EntryPoint = "fz_lineto")]
		public static extern void fz_lineto (ref fz_path_s param0, float x, float y);
		// Return Type: void
		//param0: fz_path*
		//param1: float
		//param2: float
		//param3: float
		//param4: float
		//param5: float
		//param6: float
		[DllImportAttribute (DLL, EntryPoint = "fz_curveto")]
		public static extern void fz_curveto (ref fz_path_s param0, float param1, float param2, float param3, float param4, float param5, float param6);
		// Return Type: void
		//param0: fz_path*
		//param1: float
		//param2: float
		//param3: float
		//param4: float
		[DllImportAttribute (DLL, EntryPoint = "fz_curvetov")]
		public static extern void fz_curvetov (ref fz_path_s param0, float param1, float param2, float param3, float param4);
		// Return Type: void
		//param0: fz_path*
		//param1: float
		//param2: float
		//param3: float
		//param4: float
		[DllImportAttribute (DLL, EntryPoint = "fz_curvetoy")]
		public static extern void fz_curvetoy (ref fz_path_s param0, float param1, float param2, float param3, float param4);
		// Return Type: void
		//param0: fz_path*
		[DllImportAttribute (DLL, EntryPoint = "fz_closepath")]
		public static extern void fz_closepath (ref fz_path_s param0);
		// Return Type: void
		//path: fz_path*
		[DllImportAttribute (DLL, EntryPoint = "fz_freepath")]
		public static extern void fz_freepath (ref fz_path_s path);
		// Return Type: fz_path*
		//old: fz_path*
		[DllImportAttribute (DLL, EntryPoint = "fz_clonepath")]
		public static extern IntPtr fz_clonepath (ref fz_path_s old);
		// Return Type: fz_rect->fz_rect_s
		//path: fz_path*
		//stroke: fz_strokestate*
		//ctm: fz_matrix->fz_matrix_s
		[DllImportAttribute (DLL, EntryPoint = "fz_boundpath")]
		public static extern fz_rect_s fz_boundpath (ref fz_path_s path, ref fz_strokestate_s stroke, fz_matrix_s ctm);
		// Return Type: void
		//param0: fz_path*
		//indent: int
		[DllImportAttribute (DLL, EntryPoint = "fz_debugpath")]
		public static extern void fz_debugpath (ref fz_path_s param0, int indent);
		// Return Type: fz_shade*
		//shade: fz_shade*
		[DllImportAttribute (DLL, EntryPoint = "fz_keepshade")]
		public static extern IntPtr fz_keepshade (ref fz_shade_s shade);
		// Return Type: void
		//shade: fz_shade*
		[DllImportAttribute (DLL, EntryPoint = "fz_dropshade")]
		public static extern void fz_dropshade (ref fz_shade_s shade);
		// Return Type: void
		//shade: fz_shade*
		[DllImportAttribute (DLL, EntryPoint = "fz_debugshade")]
		public static extern void fz_debugshade (ref fz_shade_s shade);
		// Return Type: fz_rect->fz_rect_s
		//shade: fz_shade*
		//ctm: fz_matrix->fz_matrix_s
		[DllImportAttribute (DLL, EntryPoint = "fz_boundshade")]
		public static extern fz_rect_s fz_boundshade (ref fz_shade_s shade, fz_matrix_s ctm);
		// Return Type: void
		//shade: fz_shade*
		//ctm: fz_matrix->fz_matrix_s
		//dest: fz_pixmap*
		//bbox: fz_bbox->fz_bbox_s
		[DllImportAttribute (DLL, EntryPoint = "fz_paintshade")]
		public static extern void fz_paintshade (ref fz_shade_s shade, fz_matrix_s ctm, ref fz_pixmap_s dest, fz_bbox_s bbox);
		// Return Type: fz_gel*
		[DllImportAttribute (DLL, EntryPoint = "fz_newgel")]
		public static extern IntPtr fz_newgel ();
		// Return Type: void
		//gel: fz_gel*
		//x0: float
		//y0: float
		//x1: float
		//y1: float
		[DllImportAttribute (DLL, EntryPoint = "fz_insertgel")]
		public static extern void fz_insertgel (ref fz_gel_s gel, float x0, float y0, float x1, float y1);
		// Return Type: fz_bbox->fz_bbox_s
		//gel: fz_gel*
		[DllImportAttribute (DLL, EntryPoint = "fz_boundgel")]
		public static extern fz_bbox_s fz_boundgel (ref fz_gel_s gel);
		// Return Type: void
		//gel: fz_gel*
		//clip: fz_bbox->fz_bbox_s
		[DllImportAttribute (DLL, EntryPoint = "fz_resetgel")]
		public static extern void fz_resetgel (ref fz_gel_s gel, fz_bbox_s clip);
		// Return Type: void
		//gel: fz_gel*
		[DllImportAttribute (DLL, EntryPoint = "fz_sortgel")]
		public static extern void fz_sortgel (ref fz_gel_s gel);
		// Return Type: void
		//gel: fz_gel*
		[DllImportAttribute (DLL, EntryPoint = "fz_freegel")]
		public static extern void fz_freegel (ref fz_gel_s gel);
		// Return Type: int
		//gel: fz_gel*
		[DllImportAttribute (DLL, EntryPoint = "fz_isrectgel")]
		public static extern int fz_isrectgel (ref fz_gel_s gel);
		// Return Type: fz_ael*
		[DllImportAttribute (DLL, EntryPoint = "fz_newael")]
		public static extern IntPtr fz_newael ();
		// Return Type: void
		//ael: fz_ael*
		[DllImportAttribute (DLL, EntryPoint = "fz_freeael")]
		public static extern void fz_freeael (ref fz_ael_s ael);
		// Return Type: fz_error->int
		//gel: fz_gel*
		//ael: fz_ael*
		//eofill: int
		//clip: fz_bbox->fz_bbox_s
		//pix: fz_pixmap*
		//colorbv: unsigned char*
		[DllImportAttribute (DLL, EntryPoint = "fz_scanconvert")]
		public static extern int fz_scanconvert (ref fz_gel_s gel, ref fz_ael_s ael, int eofill, fz_bbox_s clip, ref fz_pixmap_s pix, IntPtr colorbv);
		// Return Type: void
		//gel: fz_gel*
		//path: fz_path*
		//ctm: fz_matrix->fz_matrix_s
		//flatness: float
		[DllImportAttribute (DLL, EntryPoint = "fz_fillpath")]
		public static extern void fz_fillpath (ref fz_gel_s gel, ref fz_path_s path, fz_matrix_s ctm, float flatness);
		// Return Type: void
		//gel: fz_gel*
		//path: fz_path*
		//stroke: fz_strokestate*
		//ctm: fz_matrix->fz_matrix_s
		//flatness: float
		//linewidth: float
		[DllImportAttribute (DLL, EntryPoint = "fz_strokepath")]
		public static extern void fz_strokepath (ref fz_gel_s gel, ref fz_path_s path, ref fz_strokestate_s stroke, fz_matrix_s ctm, float flatness, float linewidth);
		// Return Type: void
		//gel: fz_gel*
		//path: fz_path*
		//stroke: fz_strokestate*
		//ctm: fz_matrix->fz_matrix_s
		//flatness: float
		//linewidth: float
		[DllImportAttribute (DLL, EntryPoint = "fz_dashpath")]
		public static extern void fz_dashpath (ref fz_gel_s gel, ref fz_path_s path, ref fz_strokestate_s stroke, fz_matrix_s ctm, float flatness, float linewidth);
		// Return Type: void
		[DllImportAttribute (DLL, EntryPoint = "fz_accelerate")]
		public static extern void fz_accelerate ();
		// Return Type: void
		//pix: fz_pixmap*
		//decode: float*
		[DllImportAttribute (DLL, EntryPoint = "fz_decodetile")]
		public static extern void fz_decodetile (ref fz_pixmap_s pix, ref float decode);
		// Return Type: void
		//pix: fz_pixmap*
		//decode: float*
		//maxval: int
		[DllImportAttribute (DLL, EntryPoint = "fz_decodeindexedtile")]
		public static extern void fz_decodeindexedtile (ref fz_pixmap_s pix, ref float decode, int maxval);
		// Return Type: void
		//dst: fz_pixmap*
		//scissor: fz_bbox->fz_bbox_s
		//img: fz_pixmap*
		//ctm: fz_matrix->fz_matrix_s
		//alpha: int
		[DllImportAttribute (DLL, EntryPoint = "fz_paintimage")]
		public static extern void fz_paintimage (ref fz_pixmap_s dst, fz_bbox_s scissor, ref fz_pixmap_s img, fz_matrix_s ctm, int alpha);
		// Return Type: void
		//dst: fz_pixmap*
		//scissor: fz_bbox->fz_bbox_s
		//img: fz_pixmap*
		//ctm: fz_matrix->fz_matrix_s
		//colorbv: unsigned char*
		[DllImportAttribute (DLL, EntryPoint = "fz_paintimagecolor")]
		public static extern void fz_paintimagecolor (ref fz_pixmap_s dst, fz_bbox_s scissor, ref fz_pixmap_s img, fz_matrix_s ctm, IntPtr colorbv);
		// Return Type: void
		//dst: fz_pixmap*
		//src: fz_pixmap*
		//alpha: int
		[DllImportAttribute (DLL, EntryPoint = "fz_paintpixmap")]
		public static extern void fz_paintpixmap (ref fz_pixmap_s dst, ref fz_pixmap_s src, int alpha);
		// Return Type: void
		//dst: fz_pixmap*
		//src: fz_pixmap*
		//msk: fz_pixmap*
		[DllImportAttribute (DLL, EntryPoint = "fz_paintpixmapmask")]
		public static extern void fz_paintpixmapmask (ref fz_pixmap_s dst, ref fz_pixmap_s src, ref fz_pixmap_s msk);
		// Return Type: void
		//dst: fz_pixmap*
		//src: fz_pixmap*
		//alpha: int
		//blendmode: fz_blendmode->fz_blendmode_e
		[DllImportAttribute (DLL, EntryPoint = "fz_blendpixmap")]
		public static extern void fz_blendpixmap (ref fz_pixmap_s dst, ref fz_pixmap_s src, int alpha, fz_blendmode_e blendmode);
		// Return Type: char*
		//src: unsigned short*
		[DllImportAttribute (DLL, EntryPoint = "pdf_fromucs2")]
		public static extern IntPtr pdf_fromucs2 (ref ushort src);
		// Return Type: fz_pixmap*
		//src: fz_pixmap*
		[DllImportAttribute (DLL, EntryPoint = "pdf_expandindexedpixmap")]
		public static extern IntPtr pdf_expandindexedpixmap (ref fz_pixmap_s src);
		// Return Type: pdf_cmap*
		[DllImportAttribute (DLL, EntryPoint = "pdf_newcmap")]
		public static extern IntPtr pdf_newcmap ();
		// Return Type: pdf_cmap*
		//cmap: pdf_cmap*
		[DllImportAttribute (DLL, EntryPoint = "pdf_keepcmap")]
		public static extern IntPtr pdf_keepcmap (ref pdf_cmap_s cmap);
		// Return Type: void
		//cmap: pdf_cmap*
		[DllImportAttribute (DLL, EntryPoint = "pdf_dropcmap")]
		public static extern void pdf_dropcmap (ref pdf_cmap_s cmap);
		// Return Type: void
		//cmap: pdf_cmap*
		[DllImportAttribute (DLL, EntryPoint = "pdf_debugcmap")]
		public static extern void pdf_debugcmap (ref pdf_cmap_s cmap);
		// Return Type: int
		//cmap: pdf_cmap*
		[DllImportAttribute (DLL, EntryPoint = "pdf_getwmode")]
		public static extern int pdf_getwmode (ref pdf_cmap_s cmap);
		// Return Type: void
		//cmap: pdf_cmap*
		//wmode: int
		[DllImportAttribute (DLL, EntryPoint = "pdf_setwmode")]
		public static extern void pdf_setwmode (ref pdf_cmap_s cmap, int wmode);
		// Return Type: void
		//cmap: pdf_cmap*
		//usecmap: pdf_cmap*
		[DllImportAttribute (DLL, EntryPoint = "pdf_setusecmap")]
		public static extern void pdf_setusecmap (ref pdf_cmap_s cmap, ref pdf_cmap_s usecmap);
		// Return Type: void
		//cmap: pdf_cmap*
		//low: int
		//high: int
		//n: int
		[DllImportAttribute (DLL, EntryPoint = "pdf_addcodespace")]
		public static extern void pdf_addcodespace (ref pdf_cmap_s cmap, int low, int high, int n);
		// Return Type: void
		//cmap: pdf_cmap*
		//low: int
		//map: int*
		//len: int
		[DllImportAttribute (DLL, EntryPoint = "pdf_maprangetotable")]
		public static extern void pdf_maprangetotable (ref pdf_cmap_s cmap, int low, ref int map, int len);
		// Return Type: void
		//cmap: pdf_cmap*
		//srclo: int
		//srchi: int
		//dstlo: int
		[DllImportAttribute (DLL, EntryPoint = "pdf_maprangetorange")]
		public static extern void pdf_maprangetorange (ref pdf_cmap_s cmap, int srclo, int srchi, int dstlo);
		// Return Type: void
		//cmap: pdf_cmap*
		//one: int
		//many: int*
		//len: int
		[DllImportAttribute (DLL, EntryPoint = "pdf_maponetomany")]
		public static extern void pdf_maponetomany (ref pdf_cmap_s cmap, int one, ref int many, int len);
		// Return Type: void
		//cmap: pdf_cmap*
		[DllImportAttribute (DLL, EntryPoint = "pdf_sortcmap")]
		public static extern void pdf_sortcmap (ref pdf_cmap_s cmap);
		// Return Type: int
		//cmap: pdf_cmap*
		//cpt: int
		[DllImportAttribute (DLL, EntryPoint = "pdf_lookupcmap")]
		public static extern int pdf_lookupcmap (ref pdf_cmap_s cmap, int cpt);
		// Return Type: int
		//cmap: pdf_cmap*
		//cpt: int
		//out: int*
		[DllImportAttribute (DLL, EntryPoint = "pdf_lookupcmapfull")]
		public static extern int pdf_lookupcmapfull (ref pdf_cmap_s cmap, int cpt, ref int @out);
		// Return Type: unsigned char*
		//cmap: pdf_cmap*
		//s: unsigned char*
		//cpt: int*
		[DllImportAttribute (DLL, EntryPoint = "pdf_decodecmap")]
		public static extern IntPtr pdf_decodecmap (ref pdf_cmap_s cmap, IntPtr s, ref int cpt);
		// Return Type: pdf_cmap*
		//wmode: int
		//bytes: int
		[DllImportAttribute (DLL, EntryPoint = "pdf_newidentitycmap")]
		public static extern IntPtr pdf_newidentitycmap (int wmode, int bytes);
		// Return Type: fz_error->int
		//cmapp: pdf_cmap**
		//name: char*
		[DllImportAttribute (DLL, EntryPoint = "pdf_loadsystemcmap")]
		public static extern int pdf_loadsystemcmap (ref IntPtr cmapp, IntPtr name);
		// Return Type: void
		//estrings: char**
		//encoding: char*
		[DllImportAttribute (DLL, EntryPoint = "pdf_loadencoding")]
		public static extern void pdf_loadencoding (ref IntPtr estrings, IntPtr encoding);
		// Return Type: int
		//name: char*
		[DllImportAttribute (DLL, EntryPoint = "pdf_lookupagl")]
		public static extern int pdf_lookupagl (IntPtr name);
	}

}
