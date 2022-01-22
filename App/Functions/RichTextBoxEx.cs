using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RichTextBoxLinks;

public class RichTextBoxEx : RichTextBox
{
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
	///     Insert a given text as a link into the RichTextBox at the current insert position.
	/// </summary>
	/// <param name="text">Text to be inserted</param>
	public void InsertLink(string text) {
		InsertLink(text, SelectionStart);
	}

	/// <summary>
	///     Insert a given text at a given position as a link.
	/// </summary>
	/// <param name="text">Text to be inserted</param>
	/// <param name="position">Insert position</param>
	public void InsertLink(string text, int position) {
		if (position < 0 || position > Text.Length) {
			throw new ArgumentOutOfRangeException(nameof(position));
		}

		SelectionStart = position;
		SelectedText = text;
		Select(position, text.Length);
		SetSelectionLink(true);
		Select(position + text.Length, 0);
	}

	/// <summary>
	///     Set the current selection's link style
	/// </summary>
	/// <param name="link">true: set link style, false: clear link style</param>
	public void SetSelectionLink(bool link) {
		SetSelectionStyle(CFM_LINK, link ? CFE_LINK : 0);
	}

	public void SetSelectionFontSize(float size) {
		CHARFORMAT2_STRUCT cf = new() { dwMask = CFM_SIZE };
		cf.cbSize = (uint)Marshal.SizeOf(cf);
		cf.yHeight = (int)(size * 20f);

		IntPtr wpar = new(SCF_SELECTION);
		IntPtr lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
		Marshal.StructureToPtr(cf, lpar, false);
		IntPtr unused = SendMessage(Handle, EM_SETCHARFORMAT, wpar, lpar);
		Marshal.FreeCoTaskMem(lpar);
	}

	public void SetSelectionBold(bool bold) {
		SetSelectionStyle(CFM_BOLD, bold ? CFE_BOLD : 0);
	}

	private void SetSelectionStyle(uint mask, uint effect) {
		CHARFORMAT2_STRUCT cf = new();
		cf.cbSize = (uint)Marshal.SizeOf(cf);
		cf.dwMask = mask;
		cf.dwEffects = effect;

		IntPtr wpar = new(SCF_SELECTION);
		IntPtr lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
		Marshal.StructureToPtr(cf, lpar, false);

		SendMessage(Handle, EM_SETCHARFORMAT, wpar, lpar);

		Marshal.FreeCoTaskMem(lpar);
	}

	#region Interop-Defines

	[StructLayout(LayoutKind.Sequential)]
	private struct CHARFORMAT2_STRUCT
	{
		public uint cbSize;
		public uint dwMask;
		public uint dwEffects;
		public int yHeight;
		private readonly int yOffset;
		private readonly int crTextColor;
		private readonly byte bCharSet;
		private readonly byte bPitchAndFamily;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public char[] szFaceName;

		private readonly ushort wWeight;
		private readonly ushort sSpacing;
		private readonly int crBackColor; // Color.ToArgb() -> int
		private readonly int lcid;
		private readonly int dwReserved;
		private readonly short sStyle;
		private readonly short wKerning;
		private readonly byte bUnderlineType;
		private readonly byte bAnimation;
		private readonly byte bRevAuthor;
		private readonly byte bReserved1;
	}

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

	private const int WM_USER = 0x0400;
	private const int EM_SETCHARFORMAT = WM_USER + 68;

	private const int SCF_SELECTION = 0x0001;

	#region CHARFORMAT2 Flags

	private const uint CFE_BOLD = 0x0001;
	private const uint CFE_LINK = 0x0020;

	private const uint CFM_BOLD = 0x00000001;
	private const uint CFM_LINK = 0x00000020;
	private const uint CFM_SIZE = 0x80000000;

	#endregion

	#endregion
}