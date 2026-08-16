using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CLR;
using Cyotek.Windows.Forms;
using Cyotek.Windows.Forms.Demo;
using MuPDF;
using MuPDF.Extensions;
using PDFPatcher.Common;
using PDFPatcher.Functions.Editor;
using DrawingPoint = System.Drawing.Point;
using DrawingRectangle = System.Drawing.Rectangle;

namespace PDFPatcher.Functions;

internal sealed class ViewerControl : ImageBoxEx
{
	enum ZoomMode
	{
		Custom, FitPage = -1, FitHorizontal = -2, FitVertical = -3
	}

	public event EventHandler DocumentLoaded;
	public new event EventHandler ZoomChanged;
	public event EventHandler ContentDirectionChanged;
	public event EventHandler PageScrollModeChanged;
	public event EventHandler<PageChangedEventArgs> PageChanged;
	public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

	internal sealed class PageChangedEventArgs(int pageNumber) : EventArgs
	{
		public int PageNumber { get; } = pageNumber;
	}
	internal sealed class SelectionChangedEventArgs(Editor.Selection selection) : EventArgs
	{
		public Editor.Selection Selection { get; } = selection;
	}

	static readonly int __pageMargin = (int)(TextRenderer.MeasureText("国", SystemFonts.MessageBoxFont).Height * 1.2d),
		__doubleMargin = __pageMargin << 1;

	readonly BackgroundWorker _renderWorker;
	readonly Timer _refreshTimer;
	bool _cancelRendering, _disposed;
	int _lockDown;
	Document _mupdf;
	Cookie _cookie = new Cookie();
	PageLabelCollection _pageLabels;
	readonly object _syncObj = new object();
	readonly ImageRendererOptions _renderOptions;

	Editor.Parts.PageLayoutProvider _LayoutProvider;
	ZoomMode _zoomMode;
	float _zoomFactor;
	ContentDirection _ContentFlow;
	int _totalPageCount;
	Box[] _pageBounds;
	SizeF _maxPageDimension;
	RenderResultCache _cache; // 缓存页面渲染结果的缓冲区
	Dictionary<int, List<Model.TextLine>> _ocrResults;

	Model.PageRange _DisplayRange;
	/// <summary>
	/// 获取或设置显示的焦点页面。
	/// </summary>
	[DefaultValue(0)]
	public int CurrentPageNumber {
		get => _DisplayRange.StartValue;
		set {
			if (value == CurrentPageNumber) {
				return;
			}

			ScrollToPage(value);
		}
	}
	/// <summary>
	/// 获取当前可见的第一个页面。
	/// </summary>
	[Browsable(false)]
	public int FirstPage => _DisplayRange.StartValue;
	/// <summary>
	/// 获取当前可见的最后一个页面。
	/// </summary>
	[Browsable(false)]
	public int LastPage => _DisplayRange.EndValue;

	[Browsable(false)]
	public PageLabelCollection PageLabels { get => _pageLabels; set => _pageLabels = value; }

	readonly OcrOptions _OcrOptions = new OcrOptions();
	/// <summary>
	/// 获取文本识别选项。
	/// </summary>
	[Browsable(false)]
	public OcrOptions OcrOptions => _OcrOptions;

	string _LiteralZoom;
	/// <summary>
	/// 获取或设置显示放大比率。
	/// </summary>
	[Browsable(false)]
	public string LiteralZoom {
		get => _LiteralZoom;
		set {
			if (value != null && _LiteralZoom != value && ChangeZoom(value)) {
				_LiteralZoom = value;
				ZoomChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}
	[Browsable(false)]
	public new float ZoomFactor => _zoomFactor * 72f / _renderOptions.Dpi;
	[Browsable(false)]
	public bool IsAdaptiveZoomMode => _zoomMode.CeqAny(ZoomMode.FitPage, ZoomMode.FitHorizontal, ZoomMode.FitVertical);

	/// <summary>
	/// 获取或设置阅读器是否使用右到左的水平滚动模式。
	/// </summary>
	[DefaultValue(Editor.ContentDirection.TopToDown)]
	public Editor.ContentDirection ContentDirection {
		get => _ContentFlow;
		set {
			if (value == _ContentFlow) {
				return;
			}

			var viewPort = GetImageViewPort();
			var centerClient = new DrawingPoint(viewPort.Left + viewPort.Width / 2, viewPort.Top + viewPort.Height / 2);
			var centerImage = PointToImage(centerClient);
			var centerPagePos = TransposeVirtualImageToPagePosition(centerImage.X, centerImage.Y);

			var s = GetSelection();

			_ContentFlow = value;
			_LayoutProvider = Editor.Parts.PageLayoutProvider.Get(value);
			if (IsAdaptiveZoomMode) {
				CalculateZoomFactor(_LiteralZoom, ViewPortSize);
			}
			UpdateLayout(true);

			if (!s.ImageRegion.IsEmpty) {
				var r = s.ImageRegion;
				var p = GetVirtualImageOffset(s.Page);
				r = new RectangleF(p.X + r.Left, p.Y + r.Top, r.Width, r.Height);
				SelectionRegion = r;
			}

			if (centerPagePos.Page > 0) {
				// 计算新布局中该页面坐标对应的虚拟画布位置
				var newPos = TransposePageToVirtualImage(centerPagePos);
				// 计算滚动值使得该点位于视口中心
				var newScrollX = newPos.X - viewPort.Width / 2;
				var newScrollY = newPos.Y - viewPort.Height / 2;
				// 限制滚动范围
				newScrollX = Math.Max(0, Math.Min(VirtualSize.Width - viewPort.Width, newScrollX));
				newScrollY = Math.Max(0, Math.Min(VirtualSize.Height - viewPort.Height, newScrollY));
				ScrollTo(newScrollX, newScrollY);
			}
			else if (_zoomMode == ZoomMode.FitPage) {
				// 如果没有有效中心点，但缩放模式为适应页面，则保持页面编号
				ScrollToPage(centerPagePos.Page > 0 ? centerPagePos.Page : 1);
			}

			ContentDirectionChanged?.Invoke(this, EventArgs.Empty);
		}
	}
	public bool HorizontalFlow => _ContentFlow.CeqAny(ContentDirection.LeftToRight, ContentDirection.RightToLeft);
	public bool IsReversalLayout => _LayoutProvider?.IsReverse == true;
	/// <summary>获取剔除了滚动区域后的视图区域。</summary>
	public Size ViewPortSize {
		get {
			var size = ClientSize;
			if (!VScroll) {
				size.Width -= SystemInformation.VerticalScrollBarWidth;
			}
			if (!HScroll) {
				size.Height -= SystemInformation.HorizontalScrollBarHeight;
			}
			return size;
		}
	}

	/// <summary>
	/// 获取或设置阅读器是否将页面渲染为灰度图像。
	/// </summary>
	[DefaultValue(false)]
	public bool GrayScale {
		get => _renderOptions.ColorSpace == (ColorSpace)ColorspaceKind.Gray;
		set {
			var v = (ColorSpace)(value ? ColorspaceKind.Gray : ColorspaceKind.RGB);
			if (_renderOptions.ColorSpace != v) {
				_renderOptions.ColorSpace = v;
				UpdateDisplay();
			}
		}
	}

	/// <summary>
	/// 获取或设置阅读器是否将页面渲染为反转颜色的效果。
	/// </summary>
	[DefaultValue(false)]
	public bool InvertColor {
		get => _renderOptions.InvertColor;
		set {
			if (_renderOptions.InvertColor == value) {
				return;
			}

			_renderOptions.InvertColor = value;
			UpdateDisplay();
		}
	}

	public Color TintColor {
		get => _renderOptions.TintColor;
		set {
			if (_renderOptions.TintColor == value) {
				return;
			}

			_renderOptions.TintColor = value;
			UpdateDisplay();
		}
	}

	[DefaultValue(false)]
	public bool HideAnnotations {
		get => _renderOptions.HideAnnotations;
		set {
			if (_renderOptions.HideAnnotations == value) {
				return;
			}

			_renderOptions.HideAnnotations = value;
			UpdateDisplay();
		}
	}

	/// <summary>
	/// 获取或设置阅读器的鼠标操作模式。
	/// </summary>
	[DefaultValue(Editor.MouseMode.Move)]
	public Editor.MouseMode MouseMode {
		get => SelectionMode != ImageBoxSelectionMode.Rectangle ? Editor.MouseMode.Move : Editor.MouseMode.Selection;
		set {
			if (value == Editor.MouseMode.Move) {
				AllowZoom = false;
				SelectionMode = ImageBoxSelectionMode.None;
				SelectionRegion = RectangleF.Empty;
			}
			else {
				AllowZoom = false;
				SelectionMode = ImageBoxSelectionMode.Rectangle;
			}
		}
	}

	bool _FullPageScroll;
	[DefaultValue(false)]
	public bool FullPageScroll {
		get => _FullPageScroll;
		set {
			if (_FullPageScroll != value) {
				_FullPageScroll = value;
				PageScrollModeChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	DrawingPoint _PinPoint;
	[Description("指定鼠标定位点")]
	public DrawingPoint PinPoint {
		get => _PinPoint;
		set {
			if (_PinPoint != value) {
				_PinPoint = value;
				if (IsPinPointVisible && !DesignMode) {
					Invalidate();
				}
			}
		}
	}
	bool _ShowPinPoint;
	[DefaultValue(false)]
	[Description("指定是否显示鼠标定位点")]
	public bool ShowPinPoint {
		get => _ShowPinPoint;
		set {
			if (_ShowPinPoint != value) {
				_ShowPinPoint = value;
				if (IsPinPointVisible && !DesignMode) {
					Invalidate();
				}
			}
		}
	}

	bool IsPinPointVisible {
		get {
			if (PinPoint != DrawingPoint.Empty) {
				var op = GetOffsetPoint(0, 0);
				var vp = GetImageViewPort();
				var pp = PinPoint;
				pp.Offset(op);
				if (vp.Contains(pp)) {
					return true;
				}
			}
			return false;
		}
	}

	bool _ShowTextBorders;
	[DefaultValue(false)]
	[Description("显示文本层的边框")]
	public bool ShowTextBorders {
		get => _ShowTextBorders;
		set {
			if (_ShowTextBorders != value) {
				_ShowTextBorders = value;
				if (!DesignMode) {
					Invalidate();
				}
			}
		}
	}

	[DefaultValue(0)]
	[Description("指定用于识别文本的语言")]
	public int OcrLanguage {
		get => _OcrOptions.OcrLangID;
		set {
			if (_OcrOptions.OcrLangID == value) {
				return;
			}

			_OcrOptions.OcrLangID = value;
			_ocrResults.Clear();
		}
	}

	[Description("指定需要显示的 PDF 文档")]
	[Browsable(false)]
	[DefaultValue(null)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Document Document {
		get => _mupdf;
		set {
			Enabled = false;
			InitViewer();
			_mupdf = value;
			if (value == null) {
				return;
			}
			Tracker.DebugMessage("Load document.");
			_lockDown++;
			var l = (_totalPageCount = _mupdf.PageCount) + 1;
			_pageBounds = new Box[l];
			LoadPageBounds();
			_cache = new RenderResultCache(_mupdf);
			ApplyOptions(AppContext.Reader);
			CalculateZoomFactor(_LiteralZoom, ViewPortSize);
			UpdateLayout(true);
			_refreshTimer.Start();
			if (!_renderWorker.IsBusy) {
				_renderWorker.RunWorkerAsync();
			}
			_lockDown--;
			DocumentLoaded?.Invoke(this, EventArgs.Empty);
			Enabled = true;
		}
	}

	[Browsable(false)]
	public int TotalPageCount => _totalPageCount;

	public ViewerControl() {
		VirtualMode = true;
		VirtualSize = Size.Empty;
		AllowUnfocusedMouseWheel = true;
		_renderOptions = new ImageRendererOptions();

		_refreshTimer = new Timer { Interval = 200 };
		_refreshTimer.Tick += (s, args) => {
			var r = _DisplayRange;
			for (int i = r.StartValue; i <= r.EndValue; i++) {
				bool v;
				lock (_cache.SyncObj) {
					v = _cache.GetBitmap(i) != null;
				}

				if (!v && !_disposed && !_renderWorker.IsBusy) {
					_renderWorker.RunWorkerAsync();
					return;
				}
			}
		};

		_renderWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
		_renderWorker.DoWork += (s, args) => {
			var r = _DisplayRange;
			Tracker.DebugMessage("started prerender job: " + r);
			_refreshTimer.Stop();
			if (_disposed) {
				return;
			}

			bool invalidate = false;
			for (int i = r.StartValue; i >= r.StartValue && i < r.EndValue + 2; i++) {
				if (i < 1 || i > _mupdf.PageCount) {
					continue;
				}

				if (_cancelRendering || _renderWorker.CancellationPending || _mupdf.IsDisposed) {
					_cancelRendering = false;
					args.Cancel = true;
					return;
				}
				if (_cache.GetBitmap(i) == null) {
					lock (_cache.SyncObj) {
						var pb = _pageBounds[i];
						Tracker.DebugMessage("load page " + i);
						var z = GetZoomFactorForPage(pb);
						RenderPage(i, (pb.Width * z).ToInt32(), (pb.Height * z).ToInt32());
						if (r.Contains(i)) {
							invalidate = true;
						}
					}
				}
			}
			if (invalidate) {
				Invalidate();
			}
		};
		_renderWorker.RunWorkerCompleted += (s, args) => {
			if (!_cancelRendering && !_disposed) {
				_refreshTimer.Start();
			}
		};
	}

	protected override void OnCreateControl() {
		base.OnCreateControl();
		using (var g = CreateGraphics()) {
			_renderOptions.Dpi = g.DpiX;
		}
	}

	protected override void OnMouseMove(MouseEventArgs e) {
		base.OnMouseMove(e);
		if (!SelectionRegion.IsEmpty && (IsResizing || IsSelecting || IsMoving) && e.Button == MouseButtons.Left) {
			LimitSelectionInPage(e.Location);
		}
	}

	protected override void OnSelectionRegionChanged(EventArgs e) {
		base.OnSelectionRegionChanged(e);
		if (_mupdf == null || MouseMode == Editor.MouseMode.Move || SelectionChanged == null) {
			return;
		}

		SelectionChanged(this, new SelectionChangedEventArgs(GetSelection()));
	}

	protected override void OnClientSizeChanged(EventArgs e) {
		base.OnClientSizeChanged(e);
		if (_zoomMode != ZoomMode.Custom && _lockDown == 0) {
			++_lockDown;
			try {
				if (ChangeZoom(LiteralZoom) && ZoomChanged != null) {
					ZoomChanged(this, EventArgs.Empty);
				}

				Invalidate();
			}
			finally {
				--_lockDown;
			}
		}
	}

	void LimitSelectionInPage(DrawingPoint location) {
		var r = SelectionRegion;
		var pp = TransposeClientToPagePosition(location.X, location.Y);
		var p = GetVirtualImageOffset(pp.Page);
		Tracker.DebugMessage(pp.Location.ToString());
		r.Offset(-p.X, -p.Y);
		var b = _pageBounds[pp.Page];
		var z = GetZoomFactorForPage(b);

		float x1 = r.Left, y1 = r.Top, x2 = r.Right, y2 = r.Bottom;
		var c = false;
		if (r.Left < 0) { x1 = 0; x2 -= r.Left; c = true; }
		if (r.Top < 0) { y1 = 0; y2 -= r.Top; c = true; }
		if (r.Right > b.Width * z) { x2 = b.Width * z; x1 -= r.Right - b.Width * z; if (x1 < 0) { x1 = 0; } c = true; }
		if (r.Bottom > b.Height * z) { y2 = b.Height * z; y1 -= r.Bottom - b.Height * z; if (y1 < 0) { y1 = 0; } c = true; }
		if (c) {
			SelectionRegion = RectangleF.FromLTRB(p.X + x1, p.Y + y1, p.X + x2, p.Y + y2);
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
		switch (keyData) {
			case Keys.Space:
			case Keys.PageDown:
				if (FullPageScroll) {
					ExecuteCommand(EditorCommands.NextPage);
					return true;
				}
				if (_LayoutProvider is null) {
					return false;
				}
				if (_LayoutProvider.IsHorizontal) {
					ScrollTo(HorizontalScroll.Value + _LayoutProvider.PageDelta, VerticalScroll.Value);
				}
				else {
					ScrollTo(HorizontalScroll.Value, VerticalScroll.Value + _LayoutProvider.PageDelta);
				}
				return true;
			case Keys.PageUp:
				if (FullPageScroll) {
					ExecuteCommand(EditorCommands.PreviousPage);
					return true;
				}
				if (_LayoutProvider is null) {
					return false;
				}
				if (_LayoutProvider.IsHorizontal) {
					ScrollTo(HorizontalScroll.Value - _LayoutProvider.PageDelta, VerticalScroll.Value);
				}
				else {
					ScrollTo(HorizontalScroll.Value, VerticalScroll.Value - _LayoutProvider.PageDelta);
				}
				return true;
			case Keys.Home:
				ScrollToPage(1);
				return true;
			case Keys.End:
				if (_mupdf != null) {
					ScrollToPage(_mupdf.PageCount);
				}

				return true;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	protected override void OnMouseWheel(MouseEventArgs e) {
		base.OnMouseWheel(e);
		if (ModifierKeys == Keys.Control) {
			var zoom = _zoomFactor * 100 / _renderOptions.Dpi * 72f;
			if (e.Delta < 0) {
				if (zoom > 10) {
					zoom -= 10;
				}
			}
			else {
				zoom += 10;
				if (zoom > 400) {
					zoom = 400;
				}
			}
			LiteralZoom = zoom.ToInt32().ToText() + "%";
		}
		else if (_LayoutProvider != null) {
			if (_LayoutProvider.IsHorizontal) {
				ScrollTo(HorizontalScroll.Value + (_LayoutProvider.IsReverse ? e.Delta : -e.Delta), VerticalScroll.Value);
			}
			else {
				ScrollTo(HorizontalScroll.Value, VerticalScroll.Value - e.Delta);
			}
		}
	}

	internal void CloseFile() {
		if (_mupdf != null) { _cache.Clear(); _mupdf.CloseFile(); }
	}

	internal void Reopen() {
		if (_mupdf != null && _mupdf.IsDisposed) { _mupdf.Reopen(); UpdateDisplay(true); }
	}

	protected override void OnVirtualDraw(PaintEventArgs e) {
		base.OnVirtualDraw(e);

		if (VirtualSize.IsEmpty || !Enabled) {
			return;
		}

		var range = _DisplayRange = GetDisplayingPageRange();
		int p = range.StartValue;
		PageChanged?.Invoke(this, new PageChangedEventArgs(p));
		var g = e.Graphics;
		var op = GetOffsetPoint(0, 0);
		var vp = GetImageViewPort();
		if (TintColor == Color.Transparent) {
			g.FillRectangle(Brushes.FloralWhite, vp);
		}
		else {
			using (var b = new SolidBrush(Processor.Imaging.BitmapHelper.Tint(Color.Gainsboro, TintColor))) {
				g.FillRectangle(b, vp);
			}
		}

		for (; p <= range.EndValue; p++) {
			Debug.Assert(p > 0 && p < _mupdf.PageCount + 1, p.ToString());
			var pb = _pageBounds[p];
			var z = GetZoomFactorForPage(pb);
			// 从布局提供者获取页面矩形
			var pageRect = _LayoutProvider.GetPageRect(p);
			var cx = (int)pageRect.Left;
			var cy = (int)pageRect.Top;
			var fullRect = _LayoutProvider.GetPageRectWithMargin(p);
			var r = new DrawingRectangle(
				cx + op.X,
				cy + op.Y,
				(pb.Width * z).ToInt32(),
				(pb.Height * z).ToInt32()
			);
			var pl = GetPageLabel(p);
			TextRenderer.DrawText(e.Graphics,
				$"{pl}{(pl.Length > 0 ? " / 第 " : "第 ")}{p} 页 ({pb.Width} * {pb.Height})",
				SystemFonts.MessageBoxFont,
				new Rectangle(cx + op.X, (int)fullRect.Y + op.Y, fullRect.Width.ToInt32(), __pageMargin),
				Color.Black,
				TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);
			var bmp = _cache.GetBitmap(p);
			if (bmp == null) {
				g.FillRectangle(Brushes.White, r);
				if (!_renderWorker.IsBusy) {
					_renderWorker.RunWorkerAsync();
				}
			}
			else {
				g.DrawImage(bmp, r.Location);
			}
			g.DrawRectangle(Pens.Black, r.Left - 1, r.Top - 1, r.Width + 1, r.Height + 1);
			if (ShowTextBorders) {
				var textPage = _cache.GetTextPage(p);
				if (textPage != null) {
					DrawTextBorders(g, p, op, textPage);
				}
			}
		}

		if (ShowPinPoint && PinPoint != DrawingPoint.Empty) {
			var pp = PinPoint.Transpose(op);
			if (vp.Contains(pp)) {
				g.DrawImage(Properties.Resources.Pin, pp.X, pp.Y - Properties.Resources.Pin.Height);
			}
		}
		if (_cache.GetBitmap(p + 1) == null && !_renderWorker.IsBusy) {
			_renderWorker.RunWorkerAsync();
		}
	}

	string GetPageLabel(int pageNumber) {
		return _mupdf.IsDisposed
			? String.Empty
			: (_pageLabels != null
				? _pageLabels
				: (_pageLabels = new PageLabelCollection(_mupdf)))
				.Format(pageNumber);
	}

	Model.PageRange GetDisplayingPageRange() {
		return _LayoutProvider.GetVisiblePageRange(GetSourceImageRegion());
	}

	void DrawTextBorders(Graphics g, int pageNumber, DrawingPoint offset, TextPage textPage) {
		if (_mupdf.IsDisposed) {
			return;
		}

		var p = _cache.LoadPage(pageNumber);
		var b = p.Bound;
		var z = GetZoomFactorForPage(b);
		var o = GetVirtualImageOffset(pageNumber);
		using (var spanPen = new Pen(Color.LightGray, 1))
		using (var blockPen = new Pen(Color.DimGray, 1)) {
			blockPen.DashStyle
				= spanPen.DashStyle
				= System.Drawing.Drawing2D.DashStyle.Dash;
			using (var m = new System.Drawing.Drawing2D.Matrix(z, 0, 0, z, offset.X + o.X, offset.Y + o.Y)) {
				g.MultiplyTransform(m);
			}

			foreach (var block in textPage) {
				g.DrawRectangle(blockPen, block.Bound.ToRectangle());
				if (block == null) {
					continue;
				}

				foreach (var line in block) {
					g.DrawRectangle(spanPen, line.Bound.ToRectangle());
				}
			}
		}
		g.ResetTransform();
	}

	internal Editor.Selection GetSelection() {
		var s = GetSelectionPageRegion();
		if (s.Page == 0 || _mupdf.IsDisposed) {
			return Editor.Selection.Empty;
		}
		else {
			var vb = _pageBounds[s.Page];
			var sr = s.Region;
			var pr = new Box(sr.X0 - vb.X0, vb.X1 - sr.Y0, sr.X1 - vb.Y0, vb.Y1 - sr.Y1);
			var o = GetVirtualImageOffset(s.Page);
			var area = SelectionRegion;
			area.Offset(-o.X, -o.Y);
			return new Editor.Selection(_cache, s.Page, pr, area);
		}
	}

	internal Editor.PageRegion GetSelectionPageRegion() {
		var area = SelectionRegion;
		if (area.IsEmpty) {
			return Editor.PageRegion.Empty;
		}

		var p1 = TransposeVirtualImageToPagePosition(area.Left.ToInt32(), area.Top.ToInt32());
		var p2 = TransposeVirtualImageToPagePosition(area.Right.ToInt32(), area.Bottom.ToInt32());
		return new Editor.PageRegion(p1, p2);
	}

	/// <summary>
	/// 返回指定位置的文本行以及与该文本行具有相同样式的后续文本行。
	/// </summary>
	/// <param name="position">查找文本行的位置。</param>
	/// <returns>返回指定位置的文本行以及与该文本行具有相同样式的后续文本行。</returns>
	internal Editor.TextInfo FindTextLines(Editor.PagePosition position) {
		var rect = new Box();
		var ti = new Editor.TextInfo();
		if (_mupdf.IsDisposed) {
			return ti;
		}

		var page = _cache.LoadPage(position.Page);
		var point = position.ToPageCoordinate(page);
		if (!page.Bound.Contains(point)
			|| !page.TextPage.Bound.Contains(point)) {
			return ti;
		}

		foreach (var block in page.TextPage) {
			if (block.Type == BlockType.Image || !block.Bound.Contains(point)) {
				continue;
			}

			HashSet<TextFont> s = null;
			TextLine l = null;
			List<TextLine> r = null;
			foreach (var line in block) {
				if (l == null) {
					if (!line.Bound.Contains(point)) {
						continue;
					}
					s = []; // 获取选中文本行的文本样式集合
					r = [];
					foreach (var ch in line) {
						s.Add(ch.Font);
					}

					rect = line.Bound;
					l = line;
					r.Add(l);
				}
				else {
					if (!line.Bound.IsHorizontalNeighbor(rect)) {
						break;
					}
					// 获取具有相同样式的邻接文本行
					foreach (var ch in line) {
						if (s.Contains(ch.Font)) {
							r.Add(line);
							l = line;
							goto NEXT;
						}
					}
					rect = rect.Union(line.Bound);
				}
			NEXT:;
			}
			if (l != null) {
				var spans = new List<Editor.TextSpan>(r.Count * 2);
				foreach (var item in r) {
					spans.AddRange(Editor.TextSpan.GetTextSpans(item));
				}
				return new Editor.TextInfo(page, rect, r, spans);
			}
		}
		return ti;
	}

	/// <summary>
	/// 返回指定区域内的文本行。
	/// </summary>
	/// <param name="region">选择的区域。</param>
	/// <returns>区域内的文本行。</returns>
	internal List<TextLine> FindTextLines(Editor.PageRegion region) {
		if (_mupdf.IsDisposed) {
			return null;
		}
		List<TextLine> r = null;
		var page = _cache.LoadPage(region.Page);
		var pr = region.ToPageCoordinate(page);
		if (pr.Intersect(page.TextPage.Bound).IsEmpty) {
			return null;
		}
		foreach (var block in page.TextPage) {
			if (block.Type == BlockType.Image || pr.Intersect(block.Bound).IsEmpty) {
				continue;
			}
			var s = new HashSet<int>();
			r ??= [];
			foreach (var line in block) {
				if (pr.Intersect(line.Bound).Area > line.Bound.Area * 0.618f) {
					r.Add(line);
				}
			}
		}
		return r;
	}

	float GetZoomFactorForPage(Box bound) {
		return _zoomFactor;
	}

	public List<Model.TextLine> OcrPage(int pageNumber, bool cached) {
		if (cached && _ocrResults.TryGetValue(pageNumber, out var r)) {
			return r;
		}
		r = Ocr(pageNumber);
		return _ocrResults[pageNumber] = r;
	}
	public string[] CleanUpOcrResult(List<Model.TextLine> result) {
		return result.ConvertAll((t) => Processor.OcrProcessor.CleanUpText(t.Text, _OcrOptions)).ToArray();
	}

	List<Model.TextLine> Ocr(int pageNumber) {
		try {
			Bitmap bmp = GetPageImage(pageNumber);
			return Processor.OcrProcessor.OcrBitmap(bmp, _OcrOptions);
		}
		catch (System.Runtime.InteropServices.COMException ex) {
			switch (ex.ErrorCode) {
				case -959971327:
					FormHelper.InfoBox("识别引擎初始化时遇到错误。\n请尝试以管理员身份运行程序，或重新安装 Office 2007 的 MODI 组件。");
					return [];
				case -959967087:
					FormHelper.ErrorBox("识别引擎无法识别本页文本。请尝试调整页面的显示比例，然后再执行识别。");
					return [];
				default:
					throw;
			}
		}
		catch (Exception ex) {
			Tracker.DebugMessage("OCR error: " + ex.Message);
			return null;
		}
	}

	public Bitmap GetPageImage(int pageNumber) {
		var b = _pageBounds[pageNumber];
		var z = GetZoomFactorForPage(b);
		return RenderPage(pageNumber, (z * b.Width).ToInt32(), (z * b.Height).ToInt32());
	}

	public Page LoadPage(int pageNumber) {
		return _cache.LoadPage(pageNumber);
	}
	public Box GetPageBound(int pageNumber) {
		return _pageBounds[pageNumber];
	}

	Bitmap RenderPage(int pageNumber, int width, int height) {
		var bmp = _cache.GetBitmap(pageNumber);
		if (bmp != null) {
			return bmp;
		}
		if (_mupdf is null || _mupdf.IsDisposed || !Enabled) {
			return null;
		}
		lock (_syncObj) {
			lock (_cache.SyncObj) {
				var p = _cache.LoadPage(pageNumber);
				if (pageNumber < _DisplayRange.StartValue - 1 || pageNumber > _DisplayRange.EndValue + 1) {
					return null;
				}
				Tracker.DebugMessage("render page " + pageNumber);
				bmp = p.RenderBitmapPage(width, height, _renderOptions, _cookie);
				_cache.SetBitmap(pageNumber, bmp);
				_cache.SetTextPage(pageNumber, p.TextPage);
			}
		}
		return bmp;
	}

	int GetPageNumberFromOffset(int offsetX, int offsetY) {
		return _LayoutProvider?.GetPageNumberFromOffset(offsetX, offsetY) ?? 0;
	}

	bool ChangeZoom(string zoomMode) {
		if (_mupdf is null) {
			return false;
		}

		var s = GetSelection();
		var pp = Editor.PagePosition.Empty;
		float z = 0; // 旧的缩放比例
		if (s.Page > 0) {
			z = GetZoomFactorForPage(_pageBounds[s.Page]);
		}

		if (HorizontalScroll.Value != 0 || VerticalScroll.Value != 0) {
			pp = GetCurrentScrollPosition();
		}

		if (!CalculateZoomFactor(zoomMode, ViewPortSize)) {
			return false;
		}

		UpdateLayout(true);
		// 保持选区尺寸比例
		if (z > 0) {
			var r = s.ImageRegion;
			var p = GetVirtualImageOffset(s.Page);
			z = _zoomFactor / z; // 转换为新的缩放比例
			r = new RectangleF(p.X + r.Left * z, p.Y + r.Top * z, r.Width * z, r.Height * z);
			SelectionRegion = r;
		}
		if (pp.Page > 0) {
			if (_zoomMode == ZoomMode.FitPage) {
				ScrollToPage(pp.Page);
			}
			else {
				ScrollToPosition(pp);
			}
		}
		return true;
	}

	bool CalculateZoomFactor(string zoomMode, Size viewSize) {
		switch (zoomMode) {
			case Constants.DestinationAttributes.ViewType.Fit:
				_zoomMode = ZoomMode.FitPage;
				_zoomFactor = Math.Min(
						(viewSize.Width - __doubleMargin) / _maxPageDimension.Width,
						(viewSize.Height - __doubleMargin) / _maxPageDimension.Height
					);
				break;
			case Constants.DestinationAttributes.ViewType.FitH:
				_zoomMode = ZoomMode.FitHorizontal;
				_zoomFactor = (viewSize.Width - __doubleMargin) / _maxPageDimension.Width;
				break;
			case Constants.DestinationAttributes.ViewType.FitV:
				_zoomMode = ZoomMode.FitVertical;
				_zoomFactor = (viewSize.Height - __doubleMargin) / _maxPageDimension.Height;
				break;
			default:
				int f;
				if (zoomMode == null) {
					return false;
				}

				if (zoomMode.EndsWith("%", StringComparison.Ordinal) && zoomMode.Length > 2) {
					f = zoomMode.Substring(0, zoomMode.Length - 1).ToInt32();
				}
				else if (zoomMode.Length > 1) {
					f = zoomMode.ToInt32();
				}
				else {
					return false;
				}

				if (f == 0) {
					return false;
				}

				_zoomMode = ZoomMode.Custom;
				_zoomFactor = (float)f / 100f * _renderOptions.Dpi / 72f;
				break;
		}
		return true;
	}

	void UpdateDisplay() { UpdateDisplay(false); }
	void UpdateDisplay(bool resized) {
		if (DesignMode || _disposed || _mupdf == null) {
			return;
		}

		_refreshTimer.Stop();
		_renderWorker.CancelAsync();
		_cancelRendering = true;
		if (_cache != null) {
			lock (_syncObj) {
				lock (_cache.SyncObj) {
					_cache.Clear();
				}
			}
		}
		_ocrResults.Clear();
		_cancelRendering = false;
		_refreshTimer.Start();
		if (resized) {
			Invalidate();
		}
	}

	void UpdateLayout(bool resized = false) {
		if (_mupdf == null || _pageBounds == null || _LayoutProvider == null) {
			return;
		}

		_refreshTimer.Stop();
		_renderWorker.CancelAsync();
		_cancelRendering = true;
		lock (_syncObj) { _cache?.Clear(); }
		_ocrResults.Clear();
		_cancelRendering = false;

		// 执行布局计算
		var viewport = ViewPortSize;
		_LayoutProvider.Margin = __pageMargin;
		_LayoutProvider.PerformLayout(_pageBounds, _zoomFactor, viewport);
		++_lockDown;
		try {
			VirtualSize = Size.Ceiling(_LayoutProvider.VirtualSize);
			NativeMethods.SetWindowPos(this.Handle);
			_refreshTimer.Start();
			if (resized) {
				Invalidate();
			}
		}
		finally {
			--_lockDown;
		}
	}

	#region 坐标转换
	internal Editor.PagePosition GetCurrentScrollPosition() {
		return _LayoutProvider != null ? TransposeClientToPagePosition(_LayoutProvider.ScrollEdgeOnClient, 0) : default;
	}

	internal bool IsClientPointInSelection(DrawingPoint point) {
		return SelectionRegion.Contains(PointToImage(point));
	}

	internal RectangleF MuRectangleToImageRegion(int pageNumber, Box box) {
		var pageRect = _LayoutProvider.GetPageRect(pageNumber);
		var ox = (int)pageRect.Left;
		var oy = (int)pageRect.Top;
		var b = _pageBounds[pageNumber];
		var z = _zoomFactor;
		var l = box.X0 * z + ox;
		var t = box.Y0 * z + oy;
		return new RectangleF(l, t, box.Width * z, box.Height * z);
	}
	/// <summary>
	/// 将屏幕客户区域的位置转换为页面坐标。
	/// </summary>
	/// <param name="clientX">横坐标。</param>
	/// <param name="clientY">纵坐标。</param>
	/// <returns>页面坐标。</returns>
	internal Editor.PagePosition TransposeClientToPagePosition(int clientX, int clientY) {
		if (_DisplayRange.StartValue <= 0 || _pageBounds == null) {
			return Editor.PagePosition.Empty;
		}
		var p = PointToImage(clientX, clientY);
		return TransposeVirtualImageToPagePosition(p.X, p.Y);
	}

	/// <summary>
	/// 将虚拟画布的坐标点转换为屏幕客户区域的位置。
	/// </summary>
	/// <param name="imageX">虚拟画布位置的横坐标。</param>
	/// <param name="imageY">虚拟画布位置的横坐标。</param>
	/// <returns>屏幕客户区域的位置。</returns>
	internal DrawingPoint TransposeVirtualImageToClient(float imageX, float imageY) {
		var vp = GetImageViewPort();
		return new DrawingPoint(vp.Left + AutoScrollPosition.X + imageX.ToInt32(), vp.Top + AutoScrollPosition.Y + imageY.ToInt32());
	}
	/// <summary>
	/// 将页面坐标转换为虚拟画布上的像素坐标（内容区域左上角为基准）。
	/// </summary>
	internal DrawingPoint TransposePageToVirtualImage(Editor.PagePosition pos) {
		if (pos.Page <= 0 || pos.Page >= _pageBounds.Length) return DrawingPoint.Empty;

		var o = GetVirtualImageOffset(pos.Page); // 页面内容区域左上角在虚拟画布上的坐标
		var bound = _pageBounds[pos.Page];
		var z = GetZoomFactorForPage(bound);

		// pos.Location 是 PDF 坐标（原点在页面左下角），转换为图像坐标（原点在页面左上角）
		float imageX = (pos.Location.X - bound.X0) * z;
		float imageY = (bound.Height - (pos.Location.Y - bound.Y0)) * z;

		return new DrawingPoint(o.X + (int)imageX, o.Y + (int)imageY);
	}
	internal DrawingPoint GetVirtualImageOffset(int pageNumber) {
		if (_LayoutProvider is null) {
			return default;
		}
		var pageRect = _LayoutProvider.GetPageRect(pageNumber);
		return new DrawingPoint((int)pageRect.Left, (int)pageRect.Top);
	}

	/// <summary>
	/// 将虚拟画布的位置转换为页面坐标。
	/// </summary>
	/// <param name="imageX">虚拟画布位置的横坐标。</param>
	/// <param name="imageY">虚拟画布位置的纵坐标。</param>
	/// <returns>页面坐标。</returns>
	internal Editor.PagePosition TransposeVirtualImageToPagePosition(int imageX, int imageY) {
		var n = GetPageNumberFromOffset(imageX, imageY);
		return TransposeVirtualImageToPagePosition(n, imageX, imageY);
	}

	/// <summary>
	/// 将屏幕客户区域的位置转换为渲染页面位置。
	/// </summary>
	/// <param name="clientX">屏幕区域的横坐标。</param>
	/// <param name="clientY">屏幕区域的纵坐标。</param>
	/// <returns>渲染页面的位置。</returns>
	internal Editor.PagePoint TransposeClientToPageImage(int clientX, int clientY) {
		if (_DisplayRange.StartValue <= 0 || _pageBounds == null || !IsPointInImage(clientX, clientY)) {
			return Editor.PagePoint.Empty;
		}
		var p = PointToImage(clientX, clientY);
		var n = GetPageNumberFromOffset(p.X, p.Y);
		var o = GetVirtualImageOffset(n);
		return new Editor.PagePoint(n, p.X - o.X, p.Y - o.Y);
	}

	/// <summary>
	/// 将虚拟页面的位置转换为PDF页面位置。
	/// </summary>
	/// <param name="pageNumber">页码。</param>
	/// <param name="imageX">虚拟图片的横坐标。</param>
	/// <param name="imageY">虚拟图片的纵坐标。</param>
	/// <returns>PDF 页面的位置。</returns>
	internal Editor.PagePosition TransposeVirtualImageToPagePosition(int pageNumber, int imageX, int imageY) {
		var o = GetVirtualImageOffset(pageNumber);
		var b = _pageBounds[pageNumber];
		var z = GetZoomFactorForPage(b);
		var ox = (imageX - o.X) / z;
		var oy = (imageY - o.Y) / z;
		return new Editor.PagePosition(pageNumber,
			b.X0 + ox, Math.Min(b.Y1, b.Y0 + b.Height - oy),
			imageX - o.X, imageY - o.Y,
			b.Contains(new MuPDF.Point(ox, oy)));
	}

	internal Editor.PagePosition TransposePageImageToPagePosition(int pageNumber, float pageImageX, float pageImageY) {
		var b = _pageBounds[pageNumber];
		var z = _zoomFactor;
		var ox = pageImageX / z;
		var oy = pageImageY / z;
		return new Editor.PagePosition(pageNumber,
			b.X0 + ox, b.Y0 + b.Height - oy,
			pageImageX.ToInt32(), pageImageY.ToInt32(),
			b.Contains(new MuPDF.Point(ox, oy)));
	}
	#endregion

	public bool ScrollToPage(int pageNumber) {
		if (_mupdf == null || _LayoutProvider == null) {
			return false;
		}

		if (pageNumber < 0) {
			pageNumber = _mupdf.PageCount + pageNumber + 1;
		}

		if (!pageNumber.IsBetween(1, _mupdf.PageCount)) {
			return false;
		}

		_DisplayRange.StartValue = pageNumber;
		try {
			if (_LayoutProvider.IsHorizontal) {
				ScrollTo(_LayoutProvider.GetPageStartEdgeOffset(pageNumber).ToInt32(), VerticalScroll.Value);
			}
			else {
				ScrollTo(HorizontalScroll.Value, _LayoutProvider.GetPageStartEdgeOffset(pageNumber).ToInt32());
			}
		}
		catch (Exception ex) {
			AppContext.MainForm.ErrorBox($"显示页面 {pageNumber.ToText()} 时出错", ex);
			return false;
		}
		return true;
	}

	internal void ScrollToPosition(Editor.PagePosition position) {
		if (_mupdf is null || _LayoutProvider is null) {
			return;
		}

		var h = _LayoutProvider.IsHorizontal;
		var op = DrawingPoint.Round(_LayoutProvider.GetPageRectWithMargin(position.Page).Location);
		var bound = _pageBounds[position.Page];
		position.Location.Deconstruct(out var px, out var py);
		if (px != 0) {
			px -= bound.X0;
			if (h && Math.Abs(px) < 0.001f) {
				op.X -= _LayoutProvider.IsReverse ? __pageMargin : -__pageMargin;
			}
		}
		if (py != 0) {
			py = bound.Height - (py - bound.Y0);
		}
		var z = GetZoomFactorForPage(bound);
		ScrollTo(
			(position.PageX == 0 && !h) ? HorizontalScroll.Value : (px * z).ToInt32() + op.X,
			(position.PageY == 0 && h) ? VerticalScroll.Value : (position.Location.Y == 0 ? 0 : (py * z).ToInt32()) + op.Y
			);
	}

	bool Next(int deltaPageNumber) {
		if (_LayoutProvider == null) {
			return false;
		}

		return ScrollToPage(CurrentPageNumber + deltaPageNumber);
	}

	// 加载页面尺寸，计算页面最大的宽度和高度，用于“适合宽度”、“适合高度”和“适合页面”等缩放方式计算缩放比例
	void LoadPageBounds() {
		float w = 0, h = 0;
		for (int i = _mupdf.PageCount; i > 0; i--) {
			var b = _mupdf.BoundPage(i - 1);
			_pageBounds[i] = b;
			b.Width.SetMax(ref w);
			b.Height.SetMax(ref h);
		}
		_maxPageDimension = new SizeF(w, h);
	}

	public void ExecuteCommand(string cmd) {
		switch (cmd) {
			case EditorCommands.FirstPage: ScrollToPage(1); break;
			case EditorCommands.PreviousPage: Next(-1); break;
			case EditorCommands.NextPage: Next(1); break;
			case EditorCommands.LastPage: ScrollToPage(-1); break;
			case EditorCommands.ScrollVertical: ContentDirection = Editor.ContentDirection.TopToDown; break;
			case EditorCommands.ScrollHorizontal: ContentDirection = Editor.ContentDirection.RightToLeft; break;
			case EditorCommands.ScrollHorizontalLeftToRight: ContentDirection = Editor.ContentDirection.LeftToRight; break;
			case EditorCommands.TrueColorSpace: GrayScale = false; break;
			case EditorCommands.GrayColorSpace: GrayScale = true; break;
			case EditorCommands.InvertColor: InvertColor = !InvertColor; break;
			case "_Refresh": UpdateDisplay(); break;
		}
	}

	public void InitViewer() {
		_cancelRendering = true;
		_refreshTimer.Stop();
		SelectionRegion = DrawingRectangle.Empty;
		_DisplayRange = new Model.PageRange();
		if (_LiteralZoom == null) {
			_zoomFactor = (float)_renderOptions.Dpi / 72;
			_zoomMode = ZoomMode.FitHorizontal;
			_LiteralZoom = Constants.DestinationAttributes.ViewType.FitH;
			VirtualSize = new Size(1, 1);
		}
		ShowTextBorders = false;
		_pageBounds = null;
		_totalPageCount = 0;
		var oldDoc = _mupdf;
		if (oldDoc != null) {
			if (_cache != null) {
				lock (_cache.SyncObj) {
					oldDoc.TryDispose();
					_cache.Clear();
					_cache.Dispose();    // 释放资源
				}
				_cache = null;
			}
			_mupdf = null;
		}
		_OcrOptions.CompressWhiteSpaces = true;
		_ocrResults = [];
	}

	internal void ApplyOptions(ReaderOptions options) {
		_LiteralZoom = options.Zoom.SubstituteDefault(Constants.DestinationAttributes.ViewType.FitH);
		_FullPageScroll = options.FullPageScroll;
		_ShowTextBorders = options.ShowTextBoder;

		HideAnnotations = options.HideAnnotation;
		GrayScale = options.GrayScale;

		if (options.ContentDirection == ContentDirection.Auto
			&& Document is not null) {
			var layout = (Document.Trailer.Get<PdfDictionary>(PdfNames.Root)?[new PdfName("PageLayout")] as PdfName)?.Name;
			ContentDirection = layout == "TwoColumnRight" || layout == "TwoPageRight"
				? ContentDirection.RightToLeft
				: ContentDirection.TopToDown;
		}
	}


	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		Tracker.DebugMessage("PDF Viewer control destroyed.");
		_cancelRendering = true;
		_disposed = true;
		Cookie cookie = _cookie;
		if (cookie != null) {
			cookie.Cancel();
			cookie.Dispose();
			_cookie = null;
		}
		_mupdf?.Dispose();
		_refreshTimer.Stop();
		_renderWorker.CancelAsync();
		if (_cache != null) {
			lock (_cache.SyncObj) {
				_cache.Dispose();
			}
		}

		_renderWorker.Dispose();
		_refreshTimer.Dispose();
	}

	static class NativeMethods
	{
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		const uint SWP_NOMOVE = 0x0002;
		const uint SWP_NOSIZE = 0x0001;
		const uint SWP_NOZORDER = 0x0004;
		const uint SWP_FRAMECHANGED = 0x0020;

		// 在设置 VirtualSize 后调用，修复滚动条可能显示不出来的问题
		public static void SetWindowPos(IntPtr hWnd) {
			SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
		}
	}
}