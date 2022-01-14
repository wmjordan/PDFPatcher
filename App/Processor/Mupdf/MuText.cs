using System;
using System.Collections.Generic;
using System.Diagnostics;

#pragma warning disable 649, 169
namespace MuPdfSharp
{
	[DebuggerDisplay("Name={Name}")]
	public sealed class MuFont
	{
		readonly ContextHandle _context;
		readonly IntPtr _Font;

		public unsafe string Name => new string(NativeMethods.GetFontName(_context, _Font));
		public MuFontFlags Attributes => NativeMethods.GetFontFlags(_Font);
		public BBox BBox => NativeMethods.GetFontBBox(_context, _Font);

		internal MuFont(ContextHandle handle, IntPtr font) {
			_context = handle;
			_Font = font;
		}
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
		readonly TextPageHandle _handle;
		NativeTextPage _TextPage;
		IEnumerable<MuTextBlock> _Blocks;

		public Rectangle BBox => _TextPage.MediaBox;
		public IEnumerable<MuTextBlock> Blocks => _Blocks ?? (_Blocks = MuContentBlock.GetTextBlocks(_TextPage._FirstBlock));

		internal MuTextPage(TextPageHandle nativePage) {
			_handle = nativePage;
			_TextPage = _handle.MarshalAs<NativeTextPage>();
		}

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

		struct NativeTextPage
		{
			readonly IntPtr /*fz_pool*/ _Pool;
			readonly Rectangle _MediaBox;
			/* fz_text_block */
			internal IntPtr _FirstBlock, _LastBlock;

			internal Rectangle MediaBox => _MediaBox;
		}

		#region IDisposable Support
		private bool disposedValue = false; // 要检测冗余调用

		void Dispose(bool disposing) {
			if (disposedValue) {
				return;
			}

			if (disposing) {
				_Blocks = null;
			}

			_handle.DisposeHandle();
			disposedValue = true;
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
		public abstract Rectangle BBox { get; }
		public abstract ContentBlockType Type { get; }
		internal static IEnumerable<MuContentBlock> GetBlocks(IntPtr firstBlock) {
			foreach (var item in firstBlock.EnumerateLinkedList<NativeContentBlock>()) {
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
			foreach (var item in firstBlock.EnumerateLinkedList<NativeContentBlock>()) {
				if (item.Data.Type == ContentBlockType.Text) {
					yield return item.Data.ToMuBlock() as MuTextBlock;
				}
			}
		}

		struct NativeImageBlock
		{
			readonly ContentBlockType _Type;
			readonly Rectangle _BBox;

			/* union {
				struct { fz_stext_line *first_line, *last_line; } t;
				struct { fz_matrix transform; fz_image *image; } i;
			} u; */
			readonly Matrix _Transform;
			readonly IntPtr _Image;
			readonly IntPtr _PreviousBlock, _NextBlock;
			internal MuContentBlock ToMuBlock() {
				return new MuImageBlock(_BBox, _Transform, _Image);
			}
		}
		struct NativeContentBlock : Interop.ILinkedList
		{
			readonly ContentBlockType _Type;
			readonly Rectangle _BBox;

			/* union {
				struct { fz_stext_line *first_line, *last_line; } t;
				struct { fz_matrix transform; fz_image *image; } i;
			} u; */
			readonly IntPtr _Ptr1, _Ptr2, a, b, c, d, e;
			readonly IntPtr _PreviousBlock, _NextBlock;

			IntPtr Interop.ILinkedList.Next => _NextBlock;
			internal ContentBlockType Type => _Type;
			internal MuContentBlock ToMuBlock() {
				return new MuTextBlock(_BBox, _Ptr1, _Ptr2);
			}
		}
	}

	public sealed class MuImageBlock : MuContentBlock, IMuBoundedElement
	{
		readonly Rectangle _BBox;
		readonly Matrix _Matrix;
		readonly IntPtr _Image;

		internal MuImageBlock(Rectangle bbox, Matrix matrix, IntPtr image) {
			_BBox = bbox;
			_Matrix = matrix;
			_Image = image;
		}

		public override Rectangle BBox => _BBox;
		public override ContentBlockType Type => ContentBlockType.Image;
	}

	public sealed class MuTextBlock : MuContentBlock, IMuBoundedElement
	{
		readonly Rectangle _BBox;
		readonly IntPtr _FirstLine, _LastLine;

		IEnumerable<MuTextLine> _Lines;
		public override Rectangle BBox => _BBox;
		public override ContentBlockType Type => ContentBlockType.Text;
		public IEnumerable<MuTextLine> Lines => _Lines ?? (_Lines = MuTextLine.GetLines(_FirstLine));

		internal MuTextBlock(Rectangle BBox, IntPtr FirstLine, IntPtr LastLine) {
			_BBox = BBox;
			_FirstLine = FirstLine;
			_LastLine = LastLine;
		}
	}

	[DebuggerDisplay("Text={Text},BBox={BBox}")]
	public sealed class MuTextLine : IMuBoundedElement
	{
		NativeTextLine _textLine;
		string _Text;
		IEnumerable<MuTextChar> _Characters;

		public Rectangle BBox => _textLine.BBox;
		public IEnumerable<MuTextChar> Characters => _Characters ?? (_Characters = MuTextChar.GetCharacters(_textLine._FirstChar));
		public IList<MuTextSpan> Spans => MuTextChar.GetSpans(this, _textLine._FirstChar, _textLine._LastChar);
		public string Text => _Text ?? (_Text = GetText());
		public MuTextChar FirstCharacter => MuTextChar.GetChar(_textLine._FirstChar);

		MuTextLine(IntPtr textLine) {
			_textLine = textLine.MarshalAs<NativeTextLine>();
		}

		string GetText() {
			var sb = new System.Text.StringBuilder(50);
			foreach (var ch in Characters) {
				sb.Append(char.ConvertFromUtf32(ch.Unicode));
			}
			return sb.ToString();
		}

		internal static IEnumerable<MuTextLine> GetLines(IntPtr firstLine) {
			foreach (var item in firstLine.EnumerateLinkedList<NativeTextLine>()) {
				yield return new MuTextLine(item.Ptr);
			}
		}

		internal struct NativeTextLine : IMuBoundedElement, Interop.ILinkedList
		{
			//int wmode; /* 0 for horizontal, 1 for vertical */
			readonly int _WMode;

			//fz_point dir; /* normalized direction of baseline */
			readonly Point _Point;

			//fz_rect bbox;
			readonly Rectangle _BBox;
			//fz_stext_char *first_char, *last_char;
			internal IntPtr _FirstChar, _LastChar;

			//fz_stext_line *prev, *next;
			readonly IntPtr _PrevLine, _NextLine;

			IntPtr Interop.ILinkedList.Next => _NextLine;

			public Rectangle BBox => _BBox;
		}
	}

	[DebuggerDisplay("Point={Point}; Size={Size}, Char={System.Char.ConvertFromUtf32(Unicode)}({Unicode}); Font={FontID}")]
	public sealed class MuTextChar : IMuBoundedElement
	{
		NativeTextChar _textChar;
		readonly Rectangle _Box;

		public Point Point => _textChar._Point;
		public Rectangle BBox => _Box.IsEmpty ? Quad.ToRectangle() : _Box;
		public Quad Quad => _textChar._Quad;
		public int Unicode => _textChar._Unicode;
		public float Size => _textChar._Size;
		public IntPtr FontID => _textChar._Font;

		MuTextChar(NativeTextChar textChar) {
			_textChar = textChar;
		}
		internal static MuTextChar GetChar(IntPtr charPtr) {
			return new MuTextChar(charPtr.MarshalAs<NativeTextChar>());
		}
		internal static IEnumerable<MuTextChar> GetCharacters(IntPtr firstChar) {
			foreach (var item in firstChar.EnumerateLinkedList<NativeTextChar>()) {
				yield return new MuTextChar(item.Data);
			}
		}
		internal unsafe static IList<MuTextSpan> GetSpans(MuTextLine textLine, IntPtr firstChar, IntPtr lastChar) {
			if (firstChar == IntPtr.Zero) {
				return new MuTextSpan[0];
			}
			var r = new List<MuTextSpan>(2);
			var ch = (NativeTextChar*)firstChar;
			var start = ch;
			var end = (NativeTextChar*)lastChar;
			var size = ch->_Size;
			var font = ch->_Font;
			var color = ch->_Color;
			var t = new System.Text.StringBuilder(100);
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
				r.Add(new MuTextSpan(textLine, start->_Point, t.ToString(), size, font, start->_Quad.Union(ch->_Quad).ToRectangle(), color));
				t.Length = 0;
				size = ch->_Size;
				font = ch->_Font;
				color = ch->_Color;
				start = ch;
				t.Append((char)ch->_Unicode);
			} while (ch != end);
			if (t.Length > 0) {
				var s = t.ToString().TrimEnd();
				if (s.Length > 0) {
					r.Add(new MuTextSpan(textLine, start->_Point, s, size, font, start->_Quad.Union(end->_Quad).ToRectangle(), color));
				}
			}
			return r;
		}
		unsafe struct NativeTextChar : Interop.ILinkedList
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
		public MuTextSpan(MuTextLine line, Point point, string text, float size, IntPtr fontID, Rectangle box, int color) {
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

	sealed class TextOptions
	{
		internal int _Flags;
	}
}
