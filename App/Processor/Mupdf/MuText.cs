using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#pragma warning disable 649, 169
namespace MuPdfSharp;

[DebuggerDisplay("Name={Name}")]
public sealed class MuFont
{
	private readonly ContextHandle _context;
	private readonly IntPtr _Font;

	internal MuFont(ContextHandle handle, IntPtr font) {
		_context = handle;
		_Font = font;
	}

	public unsafe string Name => new(NativeMethods.GetFontName(_context, _Font));
	public MuFontFlags Attributes => NativeMethods.GetFontFlags(_Font);
	public BBox BBox => NativeMethods.GetFontBBox(_context, _Font);
}

[Flags]
public enum MuFontFlags : uint
{
	None = 0,
	IsMono = 1,
	IsSerif = 1 << 1,
	IsBold = 1 << 2,
	IsItalic = 1 << 3,
	IsSubstitute = 1 << 4, /* use substitute metrics */
	IsStretch = 1 << 5, /* stretch to match PDF metrics */
	IsFakeBold = 1 << 6, /* synthesize bold */
	IsFakeItalic = 1 << 7, /* synthesize italic */
	IsForcedHinting = 1 << 8, /* force hinting for DynaLab fonts */
	HasOpenType = 1 << 9, /* has opentype shaping tables */
	InvalidBBox = 1 << 10
}

[DebuggerDisplay("BBox={BBox}")]
public sealed class MuTextPage : IMuBoundedElement, IDisposable
{
	private readonly TextPageHandle _handle;
	private IEnumerable<MuTextBlock> _Blocks;
	private NativeTextPage _TextPage;

	internal MuTextPage(TextPageHandle nativePage) {
		_handle = nativePage;
		_TextPage = _handle.MarshalAs<NativeTextPage>();
	}

	public IEnumerable<MuTextBlock> Blocks =>
		_Blocks ?? (_Blocks = MuContentBlock.GetTextBlocks(_TextPage._FirstBlock));

	public Rectangle BBox => _TextPage.MediaBox;

	///// <summary>
	///// 获取指针指向的所有文本集合。
	///// </summary>
	///// <param name="firstPage">第一个 fz_text_page 指针。</param>
	///// <returns>包含所有文本页的集合。</returns>
	//internal static List<MuTextPage> GetTextPages (TextPageHandle firstPage) {
	//	var l = new List<MuTextPage> ();
	//	foreach (var p in firstPage.EnumerateLinkedList<NativeTextPage> ()) {
	//		l.Add (new MuTextPage (p.Data));
	//	}
	//	return l;
	//}

	private struct NativeTextPage
	{
		private readonly IntPtr /*fz_pool*/
			_Pool;

		/* fz_text_block */
		internal IntPtr _FirstBlock, _LastBlock;

		internal Rectangle MediaBox { get; }
	}

	#region IDisposable Support

	private bool disposedValue; // 要检测冗余调用

	private void Dispose(bool disposing) {
		if (!disposedValue) {
			if (disposing) {
				_Blocks = null;
			}

			_handle.DisposeHandle();
			disposedValue = true;
		}
	}

	~MuTextPage() {
		Dispose(false);
	}

	// 添加此代码以正确实现可处置模式。
	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	#endregion
}

public enum ContentBlockType { Text = 0, Image = 1 }

public abstract class MuContentBlock : IMuBoundedElement
{
	public abstract ContentBlockType Type { get; }
	public abstract Rectangle BBox { get; }

	internal static IEnumerable<MuContentBlock> GetBlocks(IntPtr firstBlock) {
		foreach (NativeObject<NativeContentBlock> item in firstBlock.EnumerateLinkedList<NativeContentBlock>()) {
			switch (item.Data.Type) {
				case ContentBlockType.Text:
					yield return item.Data.ToMuBlock();
					break;
				case ContentBlockType.Image:
					yield return item.Ptr.MarshalAs<NativeImageBlock>().ToMuBlock();
					break;
			}
		}
	}

	internal static IEnumerable<MuTextBlock> GetTextBlocks(IntPtr firstBlock) {
		foreach (NativeObject<NativeContentBlock> item in firstBlock.EnumerateLinkedList<NativeContentBlock>()) {
			if (item.Data.Type == ContentBlockType.Text) {
				yield return item.Data.ToMuBlock() as MuTextBlock;
			}
		}
	}

	private struct NativeImageBlock
	{
		private readonly ContentBlockType _Type;
		private readonly Rectangle _BBox;

		/* union {
			struct { fz_stext_line *first_line, *last_line; } t;
			struct { fz_matrix transform; fz_image *image; } i;
		} u; */
		private readonly Matrix _Transform;
		private readonly IntPtr _Image;
		private readonly IntPtr _PreviousBlock, _NextBlock;

		internal MuContentBlock ToMuBlock() {
			return new MuImageBlock(_BBox, _Transform, _Image);
		}
	}

	private struct NativeContentBlock : Interop.ILinkedList
	{
		private readonly Rectangle _BBox;

		/* union {
			struct { fz_stext_line *first_line, *last_line; } t;
			struct { fz_matrix transform; fz_image *image; } i;
		} u; */
		private readonly IntPtr _Ptr1, _Ptr2, a, b, c, d, e;
		private readonly IntPtr _PreviousBlock;

		IntPtr Interop.ILinkedList.Next { get; }

		internal ContentBlockType Type { get; }

		internal MuContentBlock ToMuBlock() {
			return new MuTextBlock(_BBox, _Ptr1, _Ptr2);
		}
	}
}

public sealed class MuImageBlock : MuContentBlock, IMuBoundedElement
{
	private readonly IntPtr _Image;
	private readonly Matrix _Matrix;

	internal MuImageBlock(Rectangle bbox, Matrix matrix, IntPtr image) {
		BBox = bbox;
		_Matrix = matrix;
		_Image = image;
	}

	public override ContentBlockType Type => ContentBlockType.Image;

	public override Rectangle BBox { get; }
}

public sealed class MuTextBlock : MuContentBlock, IMuBoundedElement
{
	private readonly IntPtr _FirstLine, _LastLine;

	private IEnumerable<MuTextLine> _Lines;

	internal MuTextBlock(Rectangle BBox, IntPtr FirstLine, IntPtr LastLine) {
		this.BBox = BBox;
		_FirstLine = FirstLine;
		_LastLine = LastLine;
	}

	public override ContentBlockType Type => ContentBlockType.Text;
	public IEnumerable<MuTextLine> Lines => _Lines ?? (_Lines = MuTextLine.GetLines(_FirstLine));
	public override Rectangle BBox { get; }
}

[DebuggerDisplay("Text={Text},BBox={BBox}")]
public sealed class MuTextLine : IMuBoundedElement
{
	private IEnumerable<MuTextChar> _Characters;
	private string _Text;
	private NativeTextLine _textLine;

	private MuTextLine(IntPtr textLine) {
		_textLine = textLine.MarshalAs<NativeTextLine>();
	}

	public IEnumerable<MuTextChar> Characters =>
		_Characters ?? (_Characters = MuTextChar.GetCharacters(_textLine._FirstChar));

	public IList<MuTextSpan> Spans => MuTextChar.GetSpans(this, _textLine._FirstChar, _textLine._LastChar);
	public string Text => _Text ?? (_Text = GetText());
	public MuTextChar FirstCharacter => MuTextChar.GetChar(_textLine._FirstChar);

	public Rectangle BBox => _textLine.BBox;

	private string GetText() {
		StringBuilder sb = new(50);
		foreach (MuTextChar ch in Characters) {
			sb.Append(char.ConvertFromUtf32(ch.Unicode));
		}

		return sb.ToString();
	}

	internal static IEnumerable<MuTextLine> GetLines(IntPtr firstLine) {
		foreach (NativeObject<NativeTextLine> item in firstLine.EnumerateLinkedList<NativeTextLine>()) {
			yield return new MuTextLine(item.Ptr);
		}
	}

	internal struct NativeTextLine : IMuBoundedElement, Interop.ILinkedList
	{
		//int wmode; /* 0 for horizontal, 1 for vertical */
		private readonly int _WMode;

		//fz_point dir; /* normalized direction of baseline */
		private readonly Point _Point;

		//fz_rect bbox;

		//fz_stext_char *first_char, *last_char;
		internal IntPtr _FirstChar, _LastChar;

		//fz_stext_line *prev, *next;
		private readonly IntPtr _PrevLine;

		IntPtr Interop.ILinkedList.Next { get; }

		public Rectangle BBox { get; }
	}
}

[DebuggerDisplay(
	"Point={Point}; Size={Size}, Char={System.Char.ConvertFromUtf32(Unicode)}({Unicode}); Font={FontID}")]
public sealed class MuTextChar : IMuBoundedElement
{
	private readonly Rectangle _Box;
	private readonly NativeTextChar _textChar;

	private MuTextChar(NativeTextChar textChar) {
		_textChar = textChar;
	}

	public Point Point => _textChar._Point;
	public Quad Quad => _textChar._Quad;
	public int Unicode => _textChar._Unicode;
	public float Size => _textChar._Size;
	public IntPtr FontID => _textChar._Font;
	public Rectangle BBox => _Box.IsEmpty ? Quad.ToRectangle() : _Box;

	internal static MuTextChar GetChar(IntPtr charPtr) {
		return new MuTextChar(charPtr.MarshalAs<NativeTextChar>());
	}

	internal static IEnumerable<MuTextChar> GetCharacters(IntPtr firstChar) {
		foreach (NativeObject<NativeTextChar> item in firstChar.EnumerateLinkedList<NativeTextChar>()) {
			yield return new MuTextChar(item.Data);
		}
	}

	internal static unsafe IList<MuTextSpan> GetSpans(MuTextLine textLine, IntPtr firstChar, IntPtr lastChar) {
		if (firstChar == IntPtr.Zero) {
			return new MuTextSpan[0];
		}

		List<MuTextSpan> r = new(2);
		NativeTextChar* ch = (NativeTextChar*)firstChar;
		NativeTextChar* start = ch;
		NativeTextChar* end = (NativeTextChar*)lastChar;
		float size = ch->_Size;
		IntPtr font = ch->_Font;
		int color = ch->_Color;
		StringBuilder t = new(100);
		t.Append((char)ch->_Unicode);
		do {
			ch = ch->_Next;
			if ((IntPtr)ch == IntPtr.Zero) {
				break;
			}

			if (ch->_Size == size && ch->_Font == font && ch->_Color == color) {
				t.Append((char)ch->_Unicode);
				continue;
			}

			r.Add(new MuTextSpan(textLine, start->_Point, t.ToString(), size, font,
				start->_Quad.Union(ch->_Quad).ToRectangle(), color));
			t.Length = 0;
			size = ch->_Size;
			font = ch->_Font;
			color = ch->_Color;
			start = ch;
			t.Append((char)ch->_Unicode);
		} while (ch != end);

		if (t.Length > 0) {
			string s = t.ToString().TrimEnd();
			if (s.Length > 0) {
				r.Add(new MuTextSpan(textLine, start->_Point, s, size, font,
					start->_Quad.Union(end->_Quad).ToRectangle(), color));
			}
		}

		return r;
	}

	private unsafe struct NativeTextChar : Interop.ILinkedList
	{
		//int c;
		internal int _Unicode;

		// color
		internal int _Color; // sRGB hex value

		//fz_point origin;
		internal Point _Point;

		//fz_quad quad;
		internal Quad _Quad;

		//float size;
		internal float _Size;

		//fz_font* font;
		internal IntPtr _Font;

		//fz_stext_char* next;
		internal NativeTextChar* _Next;

		IntPtr Interop.ILinkedList.Next => (IntPtr)_Next;
	}
}

[DebuggerDisplay("Point={Point}; FontID={FontID}; Size={Size}; Color={Color}; Text={Text}")]
public sealed class MuTextSpan
{
	public MuTextSpan(MuTextLine line, Point point, string text, float size, IntPtr fontID, Rectangle box,
		int color) {
		Line = line;
		Point = point;
		Text = text;
		Size = size;
		FontID = fontID;
		Box = box;
		Color = color;
	}

	public MuTextLine Line { get; }
	public Point Point { get; }
	public string Text { get; }
	public float Size { get; }
	public IntPtr FontID { get; }
	public Rectangle Box { get; }
	public int Color { get; }
}

internal sealed class TextOptions
{
	internal int _Flags;
}