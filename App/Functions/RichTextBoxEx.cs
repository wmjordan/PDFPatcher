using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RichTextBoxLinks
{
	public class RichTextBoxEx : RichTextBox
	{
		#region Interop-Definitions
		[StructLayout(LayoutKind.Sequential)]
		struct CHARFORMAT2_STRUCT
		{
			public UInt32 cbSize;
			public UInt32 dwMask;
			public UInt32 dwEffects;
			public Int32 yHeight;
			public Int32 yOffset;
			public Int32 crTextColor;
			public byte bCharSet;
			public byte bPitchAndFamily;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szFaceName;
			public UInt16 wWeight;
			public UInt16 sSpacing;
			public int crBackColor; // Color.ToArgb() -> int
			public int lcid;
			public int dwReserved;
			public Int16 sStyle;
			public Int16 wKerning;
			public byte bUnderlineType;
			public byte bAnimation;
			public byte bRevAuthor;
			public byte bReserved1;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

		const int WM_USER = 0x0400;
		const int EM_GETCHARFORMAT = WM_USER + 58;
		const int EM_SETCHARFORMAT = WM_USER + 68;

		const int SCF_SELECTION = 0x0001;
		const int SCF_WORD = 0x0002;
		const int SCF_ALL = 0x0004;

		#region CHARFORMAT2 Flags
		const UInt32 CFE_BOLD = 0x0001;
		const UInt32 CFE_ITALIC = 0x0002;
		const UInt32 CFE_UNDERLINE = 0x0004;
		const UInt32 CFE_STRIKEOUT = 0x0008;
		const UInt32 CFE_PROTECTED = 0x0010;
		const UInt32 CFE_LINK = 0x0020;
		const UInt32 CFE_AUTOCOLOR = 0x40000000;
		const UInt32 CFE_SUBSCRIPT = 0x00010000;        /* Superscript and subscript are */
		const UInt32 CFE_SUPERSCRIPT = 0x00020000;      /*  mutually exclusive			 */

		const int CFM_SMALLCAPS = 0x0040;           /* (*)	*/
		const int CFM_ALLCAPS = 0x0080;         /* Displayed by 3.0	*/
		const int CFM_HIDDEN = 0x0100;          /* Hidden by 3.0 */
		const int CFM_OUTLINE = 0x0200;         /* (*)	*/
		const int CFM_SHADOW = 0x0400;          /* (*)	*/
		const int CFM_EMBOSS = 0x0800;          /* (*)	*/
		const int CFM_IMPRINT = 0x1000;         /* (*)	*/
		const int CFM_DISABLED = 0x2000;
		const int CFM_REVISED = 0x4000;

		const int CFM_BACKCOLOR = 0x04000000;
		const int CFM_LCID = 0x02000000;
		const int CFM_UNDERLINETYPE = 0x00800000;       /* Many displayed by 3.0 */
		const int CFM_WEIGHT = 0x00400000;
		const int CFM_SPACING = 0x00200000;     /* Displayed by 3.0	*/
		const int CFM_KERNING = 0x00100000;     /* (*)	*/
		const int CFM_STYLE = 0x00080000;       /* (*)	*/
		const int CFM_ANIMATION = 0x00040000;       /* (*)	*/
		const int CFM_REVAUTHOR = 0x00008000;


		const UInt32 CFM_BOLD = 0x00000001;
		const UInt32 CFM_ITALIC = 0x00000002;
		const UInt32 CFM_UNDERLINE = 0x00000004;
		const UInt32 CFM_STRIKEOUT = 0x00000008;
		const UInt32 CFM_PROTECTED = 0x00000010;
		const UInt32 CFM_LINK = 0x00000020;
		const UInt32 CFM_SIZE = 0x80000000;
		const UInt32 CFM_COLOR = 0x40000000;
		const UInt32 CFM_FACE = 0x20000000;
		const UInt32 CFM_OFFSET = 0x10000000;
		const UInt32 CFM_CHARSET = 0x08000000;
		const UInt32 CFM_SUBSCRIPT = CFE_SUBSCRIPT | CFE_SUPERSCRIPT;
		const UInt32 CFM_SUPERSCRIPT = CFM_SUBSCRIPT;

		const byte CFU_UNDERLINENONE = 0x00000000;
		const byte CFU_UNDERLINE = 0x00000001;
		const byte CFU_UNDERLINEWORD = 0x00000002; /* (*) displayed as ordinary underline	*/
		const byte CFU_UNDERLINEDOUBLE = 0x00000003; /* (*) displayed as ordinary underline	*/
		const byte CFU_UNDERLINEDOTTED = 0x00000004;
		const byte CFU_UNDERLINEDASH = 0x00000005;
		const byte CFU_UNDERLINEDASHDOT = 0x00000006;
		const byte CFU_UNDERLINEDASHDOTDOT = 0x00000007;
		const byte CFU_UNDERLINEWAVE = 0x00000008;
		const byte CFU_UNDERLINETHICK = 0x00000009;
		const byte CFU_UNDERLINEHAIRLINE = 0x0000000A; /* (*) displayed as ordinary underline	*/

		#endregion

		#endregion

		public RichTextBoxEx() {
			// Otherwise, non-standard links get lost when user starts typing
			// next to a non-standard link
			DetectUrls = false;
		}

		[DefaultValue(false)]
		public new bool DetectUrls {
			get => base.DetectUrls;
			set => base.DetectUrls = value;
		}

		/// <summary>
		/// Insert a given text as a link into the RichTextBox at the current insert position.
		/// </summary>
		/// <param name="text">Text to be inserted</param>
		public void InsertLink(string text) {
			InsertLink(text, SelectionStart);
		}

		/// <summary>
		/// Insert a given text at a given position as a link. 
		/// </summary>
		/// <param name="text">Text to be inserted</param>
		/// <param name="position">Insert position</param>
		public void InsertLink(string text, int position) {
			if (position < 0 || position > Text.Length)
				throw new ArgumentOutOfRangeException("position");

			SelectionStart = position;
			SelectedText = text;
			Select(position, text.Length);
			SetSelectionLink(true);
			Select(position + text.Length, 0);
		}

		/// <summary>
		/// Insert a given text at at the current input position as a link.
		/// The link text is followed by a hash (#) and the given hyperlink text, both of
		/// them invisible.
		/// When clicked on, the whole link text and hyperlink string are given in the
		/// LinkClickedEventArgs.
		/// </summary>
		/// <param name="text">Text to be inserted</param>
		/// <param name="hyperlink">Invisible hyperlink string to be inserted</param>
		public void InsertLink(string text, string hyperlink) {
			InsertLink(text, hyperlink, SelectionStart);
		}

		/// <summary>
		/// Insert a given text at a given position as a link. The link text is followed by
		/// a hash (#) and the given hyperlink text, both of them invisible.
		/// When clicked on, the whole link text and hyperlink string are given in the
		/// LinkClickedEventArgs.
		/// </summary>
		/// <param name="text">Text to be inserted</param>
		/// <param name="hyperlink">Invisible hyperlink string to be inserted</param>
		/// <param name="position">Insert position</param>
		public void InsertLink(string text, string hyperlink, int position) {
			if (position < 0 || position > Text.Length)
				throw new ArgumentOutOfRangeException("position");

			SelectionStart = position;
			SelectedRtf = @"{\rtf1\ansi " + text + @"\v #" + hyperlink + @"\v0}";
			Select(position, text.Length + hyperlink.Length + 1);
			SetSelectionLink(true);
			Select(position + text.Length + hyperlink.Length + 1, 0);
		}

		/// <summary>
		/// Set the current selection's link style
		/// </summary>
		/// <param name="link">true: set link style, false: clear link style</param>
		public void SetSelectionLink(bool link) {
			SetSelectionStyle(CFM_LINK, link ? CFE_LINK : 0);
		}
		/// <summary>
		/// Get the link style for the current selection
		/// </summary>
		/// <returns>0: link style not set, 1: link style set, -1: mixed</returns>
		public int GetSelectionLink() {
			return GetSelectionStyle(CFM_LINK, CFE_LINK);
		}

		public void SetSelectionFontSize(float size) {
			var cf = new CHARFORMAT2_STRUCT {
				dwMask = CFM_SIZE
			};
			cf.cbSize = (UInt32)Marshal.SizeOf(cf);
			cf.yHeight = (int)(size * 20f);

			IntPtr wpar = new IntPtr(SCF_SELECTION);
			IntPtr lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
			Marshal.StructureToPtr(cf, lpar, false);
			IntPtr res = SendMessage(Handle, EM_SETCHARFORMAT, wpar, lpar);
			Marshal.FreeCoTaskMem(lpar);
		}

		public void SetSelectionBold(bool bold) {
			SetSelectionStyle(CFM_BOLD, bold ? CFE_BOLD : 0);
		}
		public void SetSelectionUnderline(bool underline) {
			SetSelectionStyle(CFM_UNDERLINE, underline ? CFE_UNDERLINE : 0);
		}

		void SetSelectionStyle(UInt32 mask, UInt32 effect) {
			var cf = new CHARFORMAT2_STRUCT();
			cf.cbSize = (UInt32)Marshal.SizeOf(cf);
			cf.dwMask = mask;
			cf.dwEffects = effect;

			IntPtr wpar = new IntPtr(SCF_SELECTION);
			IntPtr lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
			Marshal.StructureToPtr(cf, lpar, false);

			IntPtr res = SendMessage(Handle, EM_SETCHARFORMAT, wpar, lpar);

			Marshal.FreeCoTaskMem(lpar);
		}

		int GetSelectionStyle(UInt32 mask, UInt32 effect) {
			var cf = new CHARFORMAT2_STRUCT();
			cf.cbSize = (UInt32)Marshal.SizeOf(cf);
			cf.szFaceName = new char[32];

			IntPtr wpar = new IntPtr(SCF_SELECTION);
			IntPtr lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
			Marshal.StructureToPtr(cf, lpar, false);

			IntPtr res = SendMessage(Handle, EM_GETCHARFORMAT, wpar, lpar);

			cf = (CHARFORMAT2_STRUCT)Marshal.PtrToStructure(lpar, typeof(CHARFORMAT2_STRUCT));

			int state;
			// dwMask holds the information which properties are consistent throughout the selection:
			if ((cf.dwMask & mask) == mask) {
				if ((cf.dwEffects & effect) == effect)
					state = 1;
				else
					state = 0;
			}
			else {
				state = -1;
			}

			Marshal.FreeCoTaskMem(lpar);
			return state;
		}
	}
}
