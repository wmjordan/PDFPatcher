using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using MuPDF;
using MuPDF.Extensions;
using PDFPatcher.Common;
using DrawingPoint = System.Drawing.Point;
using MuPoint = MuPDF.Point;
using MuRectangle = MuPDF.Box;

namespace PDFPatcher.Functions.Editor
{
	public enum ContentDirection
	{
		TopToDown,
		RightToLeft
	}

	public enum MouseMode
	{
		Move, Selection
	}

	public readonly struct PagePoint : IEquatable<PagePoint>
	{
		public static readonly PagePoint Empty;

		public readonly int Page;
		public readonly float ImageX, ImageY;
		public PagePoint(int pageNumber, float imageX, float imageY) {
			Page = pageNumber;
			ImageX = imageX;
			ImageY = imageY;
		}

		public override bool Equals(object obj) {
			return obj is PagePoint point && Equals(point);
		}

		public bool Equals(PagePoint other) {
			return Page == other.Page &&
				   ImageX == other.ImageX &&
				   ImageY == other.ImageY;
		}

		public override int GetHashCode() {
			int hashCode = -1954381243;
			hashCode = hashCode * -1521134295 + Page.GetHashCode();
			hashCode = hashCode * -1521134295 + ImageX.GetHashCode();
			hashCode = hashCode * -1521134295 + ImageY.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(PagePoint left, PagePoint right) {
			return left.Equals(right);
		}

		public static bool operator !=(PagePoint left, PagePoint right) {
			return !(left == right);
		}
	}

	public readonly struct PagePosition
	{
		public static readonly PagePosition Empty;
		/// <summary>
		/// 所在页码。
		/// </summary>
		public readonly int Page;
		/// <summary>
		/// 在 PDF 页面空间上的位置。
		/// </summary>
		public readonly float PageX, PageY;
		/// <summary>
		/// 在渲染页面上的位置。
		/// </summary>
		public readonly int ImageX, ImageY;
		/// <summary>
		/// 当前点是否在页面上。
		/// </summary>
		public readonly bool IsInPage;
		public MuPDF.Point Location => new MuPDF.Point(PageX, PageY);
		internal PagePosition(int page, PointF position, DrawingPoint imagePosition, bool isInPage)
			: this(page, position.X, position.Y, imagePosition.X, imagePosition.Y, isInPage) { }

		internal PagePosition(int page, float x, float y, int imageX, int imageY, bool isInPage) {
			Page = page;
			PageX = x;
			PageY = y;
			ImageX = imageX;
			ImageY = imageY;
			IsInPage = isInPage;
		}

		public MuPoint ToPageCoordinate(Page page) {
			var pb = page.Bound;
			return new MuPoint(PageX - pb.Left, pb.Bottom - PageY);
		}
	}

	public readonly struct PageRegion
	{
		public static readonly PageRegion Empty = new PageRegion();

		public readonly int Page;
		public readonly MuRectangle Region;

		internal PageRegion(PagePosition p1, PagePosition p2) {
			if (p1.Page != p2.Page) {
				Page = 0;
				Region = new MuRectangle();
			}
			else {
				Page = p1.Page;
				Region = new MuRectangle(p1.PageX, p1.PageY, p2.PageX, p2.PageY);
			}
		}

		public MuRectangle ToPageCoordinate(Page page) {
			var r = Region;
			var pb = page.Bound;
			return new MuRectangle(r.Left,
				pb.Bottom - r.Top,
				r.Right,
				pb.Bottom - r.Bottom);
		}
	}

	public readonly struct TextInfo
	{
		public readonly Page Page;

		/// <summary>获取文本字符的位置边框。</summary>
		public readonly MuRectangle TextBBox;
		/// <summary>获取文本位置以下的文本行。</summary>
		public readonly List<TextLine> Lines;
		public readonly List<TextSpan> Spans;

		public TextInfo(Page page, MuRectangle bbox, List<TextLine> textLines, List<TextSpan> spans) {
			Page = page;
			TextBBox = bbox;
			Lines = textLines;
			Spans = spans;
		}

		public MuRectangle BBox => TextBBox;

		public IEnumerable<TextFont> GetFonts() {
			if (Spans.HasContent() == false) {
				yield break;
			}
			var fonts = new HashSet<TextFont>();
			foreach (var span in Spans) {
				if (fonts.Add(span.Font)) {
					yield return span.Font;
				}
			}
		}

		public IEnumerable<string> GetFontNames() {
			if (Spans.HasContent() == false) {
				yield break;
			}
			HashSet<string> fonts = new HashSet<string>();
			foreach (var span in Spans) {
				var f = span.Font;
				if (fonts.Add(f.Name)) {
					yield return f.Name;
				}
			}
		}

		public override string ToString() {
			if (Lines == null) {
				return null;
			}
			var c = Lines.Count;
			if (c == 1) {
				return Lines[0].GetText();
			}
			var sb = new StringBuilder();
			var b = Lines[0].Bound;
			foreach (var line in Lines) {
				if (line.Bound.IsHorizontalNeighbor(b)) {
					sb.Append(line.GetText());
					b = b.Union(line.Bound);
				}
				else {
					b = line.Bound;
					sb.AppendLine();
					sb.Append(line.GetText());
				}
			}
			return sb.ToString();
		}
	}

	[DebuggerDisplay("Point={Point}; Font={Font}; Size={Size}; Color={Color}; Text={Text}")]
	public sealed class TextSpan
	{
		public TextSpan(TextLine line, MuPoint point, string text, float size, TextFont font, MuRectangle box, int color) {
			Line = line;
			Point = point;
			Text = text;
			Size = size;
			Font = font;
			Box = box;
			Color = color;
		}

		public TextLine Line { get; }
		public MuPoint Point { get; }
		public string Text { get; }
		public float Size { get; }
		public TextFont Font { get; }
		public MuRectangle Box { get; }
		public int Color { get; }

		public static IEnumerable<TextSpan> GetTextSpans(TextLine line) {
			var r = new List<TextSpan>(2);
			var ch = line.FirstCharacter;
			var start = ch;
			var end = line.LastCharacter;
			var size = ch.Size;
			var font = ch.Font;
			var color = ch.Color;
			var t = new System.Text.StringBuilder(100);
			t.Append((char)ch.Character);
			do {
				ch = ch.Next;
				if (ch is null) {
					break;
				}
				if (ch.Size == size && ch.Font == font && ch.Color == color) {
					t.Append((char)ch.Character);
					continue;
				}
				r.Add(new TextSpan(line, start.Origin, t.ToString(), size, font, start.Quad.Union(ch.Quad).ToBox(), color));
				t.Length = 0;
				size = ch.Size;
				font = ch.Font;
				color = ch.Color;
				start = ch;
				t.Append((char)ch.Character);
			} while (ch != end);
			if (t.Length > 0) {
				var s = t.ToString().TrimEnd();
				if (s.Length > 0) {
					r.Add(new TextSpan(line, start.Origin, s, size, font, start.Quad.Union(end.Quad).ToBox(), color));
				}
			}
			return r;
		}
	}

	public readonly struct Selection
	{
		readonly RenderResultCache _cache;
		public static readonly Selection Empty;

		/// <summary>
		/// 获取选中区域的页码。
		/// </summary>
		public readonly int Page;
		/// <summary>
		/// 获取选中区域在页面上的矩形区域（屏幕左下角点坐标为0，0）。
		/// </summary>
		public readonly MuRectangle PageRegion;
		/// <summary>
		/// 获取选中区域在显示图片上的矩形区域。
		/// </summary>
		public readonly RectangleF ImageRegion;

		public Bitmap GetSelectedBitmap() {
			_cache.LoadPage(Page);
			var p = _cache.GetBitmap(Page);
			var clip = new MuRectangle(
					ImageRegion.Left < 0 ? 0 : ImageRegion.Left,
					ImageRegion.Top < 0 ? 0 : ImageRegion.Top,
					ImageRegion.Right > p.Width ? p.Width : ImageRegion.Right,
					ImageRegion.Bottom > p.Height ? p.Height : ImageRegion.Bottom
				);
			return p.Clone(clip.ToRectangleF(), p.PixelFormat);
		}

		public Selection(RenderResultCache cache, int page, Box region, RectangleF imageRegion) {
			Page = page;
			PageRegion = region;
			ImageRegion = imageRegion;
			_cache = cache;
		}
	}
}
