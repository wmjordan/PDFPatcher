using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MuPdfSharp;
using DrawingPoint = System.Drawing.Point;

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

	public struct PagePoint
	{
		public static readonly PagePoint Empty;

		public readonly int Page;
		public readonly float ImageX, ImageY;

		public PagePoint(int pageNumber, float imageX, float imageY) {
			Page = pageNumber;
			ImageX = imageX;
			ImageY = imageY;
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

		public MuPdfSharp.Point Location => new MuPdfSharp.Point(PageX, PageY);

		internal PagePosition(int page, PointF position, DrawingPoint imagePosition, bool isInPage)
			: this(page, position.X, position.Y, imagePosition.X, imagePosition.Y, isInPage) {
		}

		internal PagePosition(int page, float x, float y, int imageX, int imageY, bool isInPage) {
			Page = page;
			PageX = x;
			PageY = y;
			ImageX = imageX;
			ImageY = imageY;
			IsInPage = isInPage;
		}
	}

	public struct PageRegion
	{
		public static readonly PageRegion Empty = new PageRegion();

		public readonly int Page;
		public readonly MuPdfSharp.Rectangle Region;

		internal PageRegion(PagePosition p1, PagePosition p2) {
			if (p1.Page != p2.Page) {
				Page = 0;
				Region = new MuPdfSharp.Rectangle();
			}
			else {
				Page = p1.Page;
				Region = new MuPdfSharp.Rectangle(p1.PageX, p1.PageY, p2.PageX, p2.PageY);
			}
		}
	}

	public struct TextInfo
	{
		public readonly MuPage Page;

		/// <summary>获取文本字符的位置边框。</summary>
		public readonly MuPdfSharp.Rectangle CharBBox;

		/// <summary>获取文本位置以下的文本行。</summary>
		public readonly List<MuTextLine> TextLines;

		public readonly List<MuTextSpan> Spans;

		public TextInfo(MuPage page, MuPdfSharp.Rectangle bbox, List<MuTextLine> textLines, List<MuTextSpan> spans) {
			Page = page;
			CharBBox = bbox;
			TextLines = textLines;
			Spans = spans;
		}
	}

	public struct Selection
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
		public readonly MuPdfSharp.Rectangle PageRegion;

		/// <summary>
		/// 获取选中区域在显示图片上的矩形区域。
		/// </summary>
		public readonly RectangleF ImageRegion;

		///// <summary>
		///// 获取选中区域的文本。
		///// </summary>
		//public string SelectedText {
		//	get {
		//		var p = _cache.LoadPage (Page);
		//		return p.GetSelection (PageRegion);
		//	}
		//}
		public Bitmap GetSelectedBitmap() {
			_cache.LoadPage(Page);
			var p = _cache.GetBitmap(Page);
			var clip = new MuPdfSharp.Rectangle(
				ImageRegion.Left < 0 ? 0 : ImageRegion.Left,
				ImageRegion.Top < 0 ? 0 : ImageRegion.Top,
				ImageRegion.Right > p.Width ? p.Width : ImageRegion.Right,
				ImageRegion.Bottom > p.Height ? p.Height : ImageRegion.Bottom
			);
			return p.Clone(clip, p.PixelFormat);
		}

		public Selection(RenderResultCache cache, int page, MuPdfSharp.Rectangle region, RectangleF imageRegion) {
			Page = page;
			PageRegion = region;
			ImageRegion = imageRegion;
			_cache = cache;
		}
	}
}