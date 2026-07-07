using System.ComponentModel;
using System.Drawing;
using CLR;
using MuPDF;
using PDFPatcher.Common;
using Point = System.Drawing.Point;

namespace PDFPatcher.Functions.Editor.Parts;

abstract class PageLayoutProvider
{
	protected float ZoomFactor { get; private set; }
	protected Size VisualBound { get; private set; }
	protected RectangleF[] ContentRects { get; private set; }
	protected RectangleF[] FullRects { get; private set; }
	public SizeF VirtualSize { get; protected set; }
	public SizeF MaxPageDimension { get; protected set; }
	public virtual int ScrollEdgeOnClient => 0;

	public abstract ContentDirection ContentDirection { get; }
	public abstract bool IsReverse { get; }
	public abstract bool IsHorizontal { get; }
	public float Margin {
		get => field;
		set { field = value; DoubleMargin = value * 2; }
	}
	protected float DoubleMargin { get; private set; }
	/// <summary>
	/// 获取向下翻页的变量。
	/// </summary>
	public abstract int PageDelta { get; }

	public void PerformLayout(Box[] pageBounds, float zoomFactor, Size visualBound) {
		ZoomFactor = zoomFactor;
		VisualBound = visualBound;
		int pageCount = pageBounds.Length - 1;
		ContentRects = new RectangleF[pageCount + 1];
		FullRects = new RectangleF[pageCount + 1];
		StartLayout(pageCount);

		MaxDimension max = new();
		if (IsReverse) {
			for (int i = pageCount; i >= 1; i--) {
				ProcessPage(pageBounds, i, ref max);
			}
		}
		else {
			for (int i = 1; i <= pageCount; i++) {
				ProcessPage(pageBounds, i, ref max);
			}
		}
		MaxPageDimension = new SizeF(max.Width, max.Height);
		VirtualSize = EndLayout(max.Right + Margin, max.Bottom + Margin);
	}

	void ProcessPage(Box[] pageBounds, int pageNumber, ref MaxDimension max) {
		var bound = pageBounds[pageNumber];
		float width = bound.Width * ZoomFactor;
		float height = bound.Height * ZoomFactor;

		bound.Width.SetMax(ref max.Width);
		bound.Height.SetMax(ref max.Height);

		var contentRect = AddPage(pageNumber, width, height);
		FullRects[pageNumber] = new RectangleF(contentRect.Left - Margin, contentRect.Top - Margin,
				contentRect.Width + DoubleMargin, contentRect.Height + DoubleMargin);
		contentRect.Right.SetMax(ref max.Right);
		contentRect.Bottom.SetMax(ref max.Bottom);
	}

	protected abstract void StartLayout(int pageCount);
	/// <summary>
	/// 添加一个页面，返回内容矩形（不含边距）。调用此方法后，内部会自动设置整体矩形。
	/// </summary>
	/// <param name="pageNumber">页码</param>
	/// <param name="contentWidth">内容宽度（不含边距）</param>
	/// <param name="contentHeight">内容高度（不含边距）</param>
	/// <returns>内容矩形</returns>
	protected abstract RectangleF AddPage(int pageNumber, float contentWidth, float contentHeight);
	protected virtual SizeF EndLayout(float maxRight, float maxBottom) => new SizeF(maxRight, maxBottom);

	public RectangleF GetPageRect(int pageNumber) => ContentRects[pageNumber];
	public RectangleF GetPageRectWithMargin(int pageNumber) => FullRects[pageNumber];
	/// <summary>
	/// 获取指定页面起始阅读方向的坐标。
	/// </summary>
	public abstract float GetPageStartEdgeOffset(int pageNumber);
	public abstract int GetPageNumberFromOffset(float offsetX, float offsetY);
	public virtual Model.PageRange GetVisiblePageRange(RectangleF viewRect) {
		if (ContentRects == null || ContentRects.Length <= 1) {
			return new Model.PageRange(0, 0);
		}

		// 首先找到可视区域中心点所在的页面（作为起始点）
		float centerX = viewRect.Left + viewRect.Width / 2;
		float centerY = viewRect.Top + viewRect.Height / 2;
		int centerPage = GetPageNumberFromOffset(centerX, centerY);
		if (centerPage == 0) {
			return new Model.PageRange(0, 0);
		}

		int first = centerPage, last = centerPage;

		// 向左扩展（减小页码）
		for (int i = centerPage - 1; i >= 1; i--) {
			if (ContentRects[i].IntersectsWith(viewRect)) {
				first = i;
			}
			else {
				break; // 一旦不再相交，停止继续向左
			}
		}

		// 向右扩展（增加页码）
		for (int i = centerPage + 1; i < ContentRects.Length; i++) {
			if (ContentRects[i].IntersectsWith(viewRect)) {
				last = i;
			}
			else {
				break;
			}
		}

		return new Model.PageRange(first, last);
	}

	public static PageLayoutProvider Get(ContentDirection direction) {
		return direction switch {
			ContentDirection.TopToDown => new TopDownLayoutProvider(),
			ContentDirection.RightToLeft => new RightToLeftLayoutProvider(),
			ContentDirection.LeftToRight => new LeftToRightLayoutProvider(),
			_ =>
#if DEBUG
			throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(ContentDirection))
#else
			new TopDownLayoutProvider()
#endif
		};
	}

	struct MaxDimension
	{
		public float Width, Height, Right, Bottom;
	}
}

sealed class TopDownLayoutProvider : PageLayoutProvider
{
	private float _currentY;

	public override ContentDirection ContentDirection => ContentDirection.TopToDown;
	public override bool IsReverse => false;
	public override bool IsHorizontal => false;
	public override int PageDelta => (VisualBound.Height * 0.95).ToInt32();

	protected override void StartLayout(int pageCount) => _currentY = 0;
	protected override SizeF EndLayout(float maxRight, float maxBottom) {
		var width = maxRight;
		var height = maxBottom;
		float n;
		if (ContentRects.Length > 1 && (n = ContentRects[ContentRects.Length - 1].Height) < VisualBound.Height) {
			height += VisualBound.Height - n;
		}
		return new SizeF(width, height);
	}

	protected override RectangleF AddPage(int pageNumber, float contentWidth, float contentHeight) {
		var margin = Margin;
		var contentRect = new RectangleF(margin, _currentY + margin, contentWidth, contentHeight);
		ContentRects[pageNumber] = contentRect;
		_currentY += contentHeight + DoubleMargin;
		return contentRect;
	}

	public override float GetPageStartEdgeOffset(int pageNumber) {
		return FullRects[pageNumber].Top; // 在构造页面矩形区域时已包含顶端的空白
	}
	public override int GetPageNumberFromOffset(float offsetX, float offsetY) {
		int lo = 1, hi = ContentRects.Length - 1;
		while (lo <= hi) {
			int mid = (lo + hi) / 2;
			var rect = ContentRects[mid];
			if (offsetY < rect.Top) {
				hi = mid - 1;
			}
			else if (offsetY >= rect.Bottom) {
				lo = mid + 1;
			}
			else {
				return mid;
			}
		}
		return lo > ContentRects.Length - 1 ? ContentRects.Length - 1 : lo;
	}
}

abstract class HorizontalLayoutProvider : PageLayoutProvider
{
	float _currentX;
	public override bool IsHorizontal => true;

	protected override void StartLayout(int pageCount) => _currentX = 0;

	protected override RectangleF AddPage(int pageNumber, float contentWidth, float contentHeight) {
		var margin = Margin;
		var contentRect = new RectangleF(_currentX + margin, margin, contentWidth, contentHeight);
		ContentRects[pageNumber] = contentRect;
		_currentX += contentWidth + DoubleMargin;
		return contentRect;
	}
}

sealed class LeftToRightLayoutProvider : HorizontalLayoutProvider
{
	public override ContentDirection ContentDirection => ContentDirection.LeftToRight;
	public override bool IsReverse => false;
	public override int PageDelta => (VisualBound.Width * 0.95).ToInt32();

	protected override SizeF EndLayout(float maxRight, float maxBottom) {
		var width = maxRight;
		var height = maxBottom;
		float n;
		if (ContentRects.Length > 1 && (n = ContentRects[ContentRects.Length - 1].Width + DoubleMargin) < VisualBound.Width) {
			width += VisualBound.Width - n;
		}
		return new SizeF(width, height);
	}

	public override float GetPageStartEdgeOffset(int pageNumber) {
		return FullRects[pageNumber].Left;
	}
	public override int GetPageNumberFromOffset(float offsetX, float offsetY) {
		int lo = 1, hi = ContentRects.Length - 1;
		while (lo <= hi) {
			int mid = (lo + hi) / 2;
			var rect = ContentRects[mid];
			if (offsetX < rect.Left) {
				hi = mid - 1;
			}
			else if (offsetX >= rect.Right) {
				lo = mid + 1;
			}
			else {
				return mid;
			}
		}
		return lo > ContentRects.Length - 1 ? ContentRects.Length - 1 : lo;
	}
}

sealed class RightToLeftLayoutProvider : HorizontalLayoutProvider
{
	public override ContentDirection ContentDirection => ContentDirection.RightToLeft;
	public override bool IsReverse => true;
	public override int PageDelta => (VisualBound.Width * -0.95).ToInt32();
	public override int ScrollEdgeOnClient => VisualBound.Width;

	protected override SizeF EndLayout(float maxRight, float maxBottom) {
		var width = maxRight;
		var height = maxBottom;
		float n;
		if (ContentRects.Length > 1 && (n = ContentRects[ContentRects.Length - 1].Width + DoubleMargin) < VisualBound.Width) {
			n = VisualBound.Width - n;
			width += n;
			for (var i = 1; i < ContentRects.Length; i++) {
				var rect = ContentRects[i];
				rect.Offset(n, 0);
				ContentRects[i] = rect;
			}
		}
		return new SizeF(width, height);
	}

	public override float GetPageStartEdgeOffset(int pageNumber) {
		return FullRects[pageNumber].Left;
	}
	public override int GetPageNumberFromOffset(float offsetX, float offsetY) {
		int lo = 1, hi = ContentRects.Length - 1;
		while (lo <= hi) {
			int mid = (lo + hi) / 2;
			var rect = ContentRects[mid];
			if (offsetX < rect.Left) {
				lo = mid + 1;
			}
			else if (offsetX >= rect.Right + DoubleMargin) { // 将页面右侧的空白归为下一页
				hi = mid - 1;
			}
			else {
				return mid;
			}
		}
		return hi < 1 ? 1 : hi;
	}
}

