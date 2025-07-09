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

namespace PDFPatcher.Functions
{
	internal sealed class ViewerControl : ImageBoxEx {
		enum ZoomMode {
			Custom, FitPage = -1, FitHorizontal = -2, FitVertical = -3
		}

		public event EventHandler DocumentLoaded;
		public new event EventHandler ZoomChanged;
		public event EventHandler ContentDirectionChanged;
		public event EventHandler PageScrollModeChanged;
		public event EventHandler<PageChangedEventArgs> PageChanged;
		public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

		internal sealed class PageChangedEventArgs(int pageNumber) : EventArgs {
			public int PageNumber { get; } = pageNumber;
		}
		internal sealed class SelectionChangedEventArgs(Editor.Selection selection) : EventArgs {
			public Editor.Selection Selection { get; } = selection;
		}

		readonly static IComparer<int> __horizontalComparer = ValueHelper.GetReverseComparer<int>();

		static readonly int __pageMargin = (int)(TextRenderer.MeasureText("国", SystemFonts.MessageBoxFont).Height * 1.2d);

		readonly BackgroundWorker _renderWorker;
		readonly Timer _refreshTimer;
		bool _cancelRendering, _disposed;
		bool _lockDown;
		Document _mupdf;
		Cookie _cookie = new Cookie();
		PageLabelCollection _pageLabels;
		readonly object _syncObj = new object();
		readonly ImageRendererOptions _renderOptions;

		ZoomMode _zoomMode;
		float _zoomFactor;
		ContentDirection _contentFlow;
		/// <summary>
		/// 页面的尺寸信息。
		/// </summary>
		Box[] _pageBounds;
		SizeF _maxDimension;
		/// <summary>
		/// 页面的滚动位置。
		/// </summary>
		int[] _pageOffsets;
		/// <summary>
		/// 缓存页面渲染结果的缓冲区。
		/// </summary>
		RenderResultCache _cache;
		Dictionary<int, List<Model.TextLine>> _ocrResults;

		Model.PageRange _DisplayRange;
		/// <summary>
		/// 获取或设置显示的焦点页面。
		/// </summary>
		[DefaultValue(0)]
		public int CurrentPageNumber {
			get => HorizontalFlow ? _DisplayRange.EndValue : _DisplayRange.StartValue;
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
				if (value != null && ChangeZoom(value)) {
					_LiteralZoom = value;
					ZoomChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}
		public new float ZoomFactor => _zoomFactor * 72f / _renderOptions.Dpi;
		/// <summary>
		/// 获取或设置阅读器是否使用右到左的水平滚动模式。
		/// </summary>
		[DefaultValue(Editor.ContentDirection.TopToDown)]
		public Editor.ContentDirection ContentDirection {
			get => _contentFlow;
			set {
				if (value == _contentFlow) {
					return;
				}
				Editor.PagePosition pp = _mupdf != null
					? TransposeVirtualImageToPagePosition(HorizontalScroll.Value, VerticalScroll.Value)
					: Editor.PagePosition.Empty;
				var s = GetSelection();
				_contentFlow = value;
				UpdateDisplay(true);
				if (!s.ImageRegion.IsEmpty) {
					var r = s.ImageRegion;
					var p = GetVirtualImageOffset(s.Page);
					r = new RectangleF(p.X + r.Left, p.Y + r.Top, r.Width, r.Height);
					SelectionRegion = r;
				}
				if (_zoomMode == ZoomMode.FitPage) {
					ScrollToPage(pp.Page);
				}
				else {
					ScrollToPosition(pp);
				}
				ContentDirectionChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		public bool HorizontalFlow => _contentFlow != Editor.ContentDirection.TopToDown;

		/// <summary>
		/// 获取或设置阅读器是否将页面渲染为灰度图像。
		/// </summary>
		[DefaultValue(false)]
		public bool GrayScale {
			get => _renderOptions.ColorSpace == ColorspaceKind.Gray;
			set {
				var v = value ? ColorspaceKind.Gray : ColorspaceKind.Rgb;
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
		public Document Document {
			get => _mupdf;
			set {
				Enabled = false;
				InitViewer();
				_mupdf = value;
				if (value != null) {
					Tracker.DebugMessage("Load document.");
					var l = _mupdf.PageCount + 1;
					_pageOffsets = new int[l];
					_pageBounds = new Box[l];
					LoadPageBounds();
					_cache = new RenderResultCache(_mupdf);
					Tracker.DebugMessage("Calculating document virtual size.");
					CalculateZoomFactor(_LiteralZoom);
					CalculateDocumentVirtualSize();
					ScrollToPage(1);
					_refreshTimer.Start();
					if (!_renderWorker.IsBusy) {
						_renderWorker.RunWorkerAsync();
					}
					DocumentLoaded?.Invoke(this, EventArgs.Empty);
					Enabled = true;
				}
			}
		}

		public ViewerControl() {
			VirtualMode = true;
			VirtualSize = Size.Empty;
			AllowUnfocusedMouseWheel = true;
			_renderOptions = new ImageRendererOptions();
			//_ViewBox.SelectionMode = ImageBoxSelectionMode.Rectangle;

			_refreshTimer = new Timer {
				Interval = 200
			};
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

			_renderWorker = new BackgroundWorker {
				WorkerSupportsCancellation = true
			};
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
					if (_cancelRendering
						|| _renderWorker.CancellationPending
						|| _mupdf.IsDisposed) {
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
			if (_zoomMode != ZoomMode.Custom && !_lockDown) {
				if (ChangeZoom(LiteralZoom) && ZoomChanged != null) {
					ZoomChanged(this, EventArgs.Empty);
				}
				//CalculateDocumentVirtualSize ();
				Invalidate();
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
			if (r.Left < 0) {
				x1 = 0;
				x2 -= r.Left;
				c = true;
			}
			if (r.Top < 0) {
				y1 = 0;
				y2 -= r.Top;
				c = true;
			}
			if (r.Right > b.Width * z) {
				x2 = b.Width * z;
				x1 -= r.Right - b.Width * z;
				if (x1 < 0) {
					x1 = 0;
				}
				c = true;
			}
			if (r.Bottom > b.Height * z) {
				y2 = b.Height * z;
				y1 -= r.Bottom - b.Height * z;
				if (y1 < 0) {
					y1 = 0;
				}
				c = true;
			}
			if (c) {
				SelectionRegion = RectangleF.FromLTRB(p.X + x1, p.Y + y1, p.X + x2, p.Y + y2);
			}
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			switch (keyData) {
				case Keys.Space:
				case Keys.PageDown:
					if (FullPageScroll) {
						ExecuteCommand("_NextPage");
						return true;
					}
					if (HorizontalFlow) {
						ScrollTo(HorizontalScroll.Value - (GetInsideViewPort().Width * 0.95).ToInt32(), VerticalScroll.Value);
					}
					else {
						ScrollTo(HorizontalScroll.Value, VerticalScroll.Value + (GetInsideViewPort().Height * 0.95).ToInt32());
					}
					return true;
				case Keys.PageUp:
					if (FullPageScroll) {
						ExecuteCommand("_PreviousPage");
						return true;
					}
					if (HorizontalFlow) {
						ScrollTo(HorizontalScroll.Value + (GetInsideViewPort().Width * 0.95).ToInt32(), VerticalScroll.Value);
					}
					else {
						ScrollTo(HorizontalScroll.Value, VerticalScroll.Value - (GetInsideViewPort().Height * 0.95).ToInt32());
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
			else {
				if (HorizontalFlow) {
					ScrollTo(HorizontalScroll.Value + e.Delta, VerticalScroll.Value);
				}
				else {
					ScrollTo(HorizontalScroll.Value, VerticalScroll.Value - e.Delta);
				}
			}
		}
		internal void CloseFile() {
			if (_mupdf != null) {
				_cache.Clear();
				_mupdf.CloseFile();
			}
		}
		internal void Reopen() {
			if (_mupdf != null && _mupdf.IsDisposed) {
				_mupdf.Reopen();
				UpdateDisplay(true);
			}
		}

		protected override void OnVirtualDraw(PaintEventArgs e) {
			base.OnVirtualDraw(e);

			if (VirtualSize.IsEmpty || !Enabled) {
				return;
			}
			_DisplayRange = GetDisplayingPageRange();
			var p = _DisplayRange.StartValue;
			PageChanged?.Invoke(this, new PageChangedEventArgs(p));
			var g = e.Graphics;
			var op = GetOffsetPoint(0, 0); // 偏移位置点
			var vp = GetImageViewPort();
			if (TintColor == Color.Transparent) {
				g.FillRectangle(Brushes.FloralWhite, vp);
			}
			else {
				using (var b = new SolidBrush(Processor.Imaging.BitmapHelper.Tint(Color.Gainsboro, TintColor))) {
					g.FillRectangle(b, vp);
				}
			}

			var r = DrawingRectangle.Empty;
			do {
				Debug.Assert(p > 0 && p < _mupdf.PageCount + 1, p.ToString());
				var pb = _pageBounds[p];
				var z = GetZoomFactorForPage(pb);
				var ox = HorizontalFlow ? _pageOffsets[p] : 0;
				var oy = HorizontalFlow ? 0 : _pageOffsets[p];
				r = new DrawingRectangle(
					ox + op.X + __pageMargin,
					oy + op.Y + __pageMargin,
					(pb.Width * z).ToInt32(),
					(pb.Height * z).ToInt32()
				);
				var pl = GetPageLabel(p);
				TextRenderer.DrawText(e.Graphics,
					$"{pl}{(pl.Length > 0 ? " / 第 " : "第 ")}{p} 页 ({pb.Width} * {pb.Height})",
					SystemFonts.MessageBoxFont,
					new DrawingPoint(ox + op.X + __pageMargin, oy + op.Y),
					Color.Black);
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
			} while ((HorizontalFlow ? (r.Right > 0) : (r.Bottom < vp.Height))
				&& ++p < _pageOffsets.Length);
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
			var b = GetOffsetRectangle(GetImageViewPort());
			int start = GetPageNumberFromOffset(-b.Left + b.Width, -b.Y);
			int end = GetPageNumberFromOffset(-b.Left, -(b.Y - b.Height));
			Debug.Assert(end >= start);
			if (end == 0) {
				end = start;
			}
			return new Model.PageRange(start, end);
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

		/// <summary>
		/// 返回选定区域。
		/// </summary>
		/// <returns>选定的矩形区域。</returns>
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
			#if DEBUG
			var b = GetOffsetRectangle(GetImageViewPort());
			#endif
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
			var offsets = _pageOffsets;
			if (offsets == null) {
				return 0;
			}
			var p = HorizontalFlow ?
				Array.BinarySearch(offsets, 1, offsets.Length - 1, offsetX, __horizontalComparer) :
				Array.BinarySearch(offsets, 1, offsets.Length - 1, offsetY);
			if (p < 0) {
				p = ~p;
				if (!HorizontalFlow) {
					--p;
				}
			}
			if (p >= offsets.Length) {
				return offsets.Length - 1;
			}
			else if (p < 1) {
				return 1;
			}
			return p;
		}

		bool ChangeZoom(string zoomMode) {
			var s = GetSelection();
			var pp = Editor.PagePosition.Empty;
			float z = 0;
			if (s.Page > 0) {
				z = GetZoomFactorForPage(_pageBounds[s.Page]);
			}
			if (HorizontalScroll.Value != 0 || VerticalScroll.Value != 0) {
				pp = TransposeVirtualImageToPagePosition(HorizontalScroll.Value, VerticalScroll.Value);
			}
			if (!CalculateZoomFactor(zoomMode)) {
				return false;
			}
			if (_mupdf == null) {
				return false;
			}
			UpdateDisplay(true);
			// 保持选区尺寸比例
			if (z > 0) {
				var r = s.ImageRegion;
				var p = GetVirtualImageOffset(s.Page);
				z = _zoomFactor / z;
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

		bool CalculateZoomFactor(string zoomMode) {
			switch (zoomMode) {
				case Constants.DestinationAttributes.ViewType.Fit:
					_zoomMode = ZoomMode.FitPage;
					_zoomFactor = Math.Min(
							(GetInsideViewPort().Width - __pageMargin - __pageMargin) / _maxDimension.Width,
							(GetInsideViewPort().Height - __pageMargin - __pageMargin) / _maxDimension.Height
						);
					break;
				case Constants.DestinationAttributes.ViewType.FitH:
					_zoomMode = ZoomMode.FitHorizontal;
					_zoomFactor = (GetInsideViewPort().Width - __pageMargin - __pageMargin) / _maxDimension.Width;
					break;
				case Constants.DestinationAttributes.ViewType.FitV:
					_zoomMode = ZoomMode.FitVertical;
					_zoomFactor = (GetInsideViewPort().Height - __pageMargin - __pageMargin) / _maxDimension.Height;
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
					else { return false; }
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
				//var p = FirstPage;
				CalculateDocumentVirtualSize();
				//ShowPage (p);
			}
		}

		#region 坐标转换
		internal Editor.PagePosition GetCurrentScrollPosition() {
			return TransposeClientToPagePosition(ClientRectangle.Width, 0);
		}

		internal bool IsClientPointInSelection(DrawingPoint point) {
			return SelectionRegion.Contains(PointToImage(point));
		}

		internal RectangleF MuRectangleToImageRegion(int pageNumber, Box box) {
			var rtl = HorizontalFlow;
			var o = _pageOffsets[pageNumber];
			var b = _pageBounds[pageNumber];
			var z = _zoomFactor;
			var l = box.X0 * z + __pageMargin;
			var t = box.Y0 * z + __pageMargin;
			if (rtl) {
				l += o;
			}
			else {
				t += o;
			}
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
		/// 获取指定页面在虚拟画布上的绘制坐标点。
		/// </summary>
		/// <param name="pageNumber">页面编号。</param>
		/// <returns>页面左上角在虚拟画布上的坐标点。</returns>
		internal DrawingPoint GetVirtualImageOffset(int pageNumber) {
			var rtl = HorizontalFlow;
			var ox = rtl ? _pageOffsets[pageNumber] : 0;
			var oy = rtl ? 0 : _pageOffsets[pageNumber];
			return new DrawingPoint(ox + __pageMargin, oy + __pageMargin);
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

		int GetPageFullWidth(float pageWidth) {
			return __pageMargin + __pageMargin + (pageWidth * _zoomFactor).ToInt32();
		}
		int GetPageFullHeight(float pageHeight) {
			return __pageMargin + __pageMargin + (pageHeight * _zoomFactor).ToInt32();
		}

		bool ScrollToPage(int pageNumber) {
			if (_mupdf == null || _pageOffsets == null) {
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
				if (HorizontalFlow) {
					ScrollTo(_pageOffsets[pageNumber], VerticalScroll.Value);
				}
				else {
					ScrollTo(HorizontalScroll.Value, _pageOffsets[pageNumber]);
				}
			}
			catch (Exception ex) {
				AppContext.MainForm.ErrorBox($"显示页面 {pageNumber.ToText()} 时出错", ex);
				return false;
			}
			return true;
		}

		internal void ScrollToPosition(Editor.PagePosition position) {
			if (_mupdf == null) {
				return;
			}
			var h = HorizontalFlow;
			var op = GetVirtualImageOffset(position.Page);
			var bound = _pageBounds[position.Page];
			position.Location.Deconstruct(out var px, out var py);
			if (px != 0) {
				px -= bound.X0;
				if (h && Math.Abs(px) < 0.001f) {
					op.X -= __pageMargin;
				}
			}
			if (py != 0) {
				py = bound.Height - (py - bound.Y0);
				if (!h && Math.Abs(py) < 0.001f) {
					op.Y -= __pageMargin;
				}
			}
			var z = GetZoomFactorForPage(bound);
			ScrollTo(
				(position.PageX == 0 && !h) ? HorizontalScroll.Value : (px * z).ToInt32() + op.X,
				(position.PageY == 0 && h) ? VerticalScroll.Value : (position.Location.Y == 0 ? 0 : (py * z).ToInt32()) + op.Y
				);
		}

		bool Next(int deltaPageNumber) {
			if (_pageOffsets == null) {
				return false;
			}
			return (HorizontalFlow && deltaPageNumber > 0 && HorizontalScroll.Value > _pageOffsets[CurrentPageNumber])
				? ScrollToPage(CurrentPageNumber + deltaPageNumber - 1)
				: ScrollToPage(CurrentPageNumber + deltaPageNumber);
		}

		void LoadPageBounds() {
			float w = 0, h = 0;
			for (int i = _mupdf.PageCount; i > 0; i--) {
				var b = _mupdf.BoundPage(i - 1);
				_pageBounds[i] = b;
				b.Width.SetMax(ref w);
				b.Height.SetMax(ref h);
			}
			_maxDimension = new SizeF(w, h);
		}

		void CalculateDocumentVirtualSize() {
			if (_pageOffsets == null || _pageBounds == null) {
				return;
			}
			int h = 0, w = 0;
			var l = _mupdf.PageCount + 1;
			var vs = GetInsideViewPort().Size;
			_lockDown = true;
			if (HorizontalFlow) {
				lock (_syncObj) {
					for (int i = l - 1; i >= 0; i--) {
						var b = _pageBounds[i];
						_pageOffsets[i] = w;
						w += GetPageFullWidth(b.Width);
						if (b.Height > h) {
							h = b.Height.ToInt32();
						}
					}
					var w1 = GetPageFullWidth(_pageBounds[1].Width);
					if (w1 < vs.Width) {
						w += vs.Width - w1;
					}
				}
				VirtualSize = new Size(w, h = GetPageFullHeight(h));
				HorizontalScroll.Visible = HorizontalScroll.Enabled = w > ClientSize.Width;
				VerticalScroll.Visible = VerticalScroll.Enabled = h > ClientSize.Height;
			}
			else {
				lock (_syncObj) {
					for (int i = 1; i < l; i++) {
						var b = _pageBounds[i];
						_pageOffsets[i] = h;
						h += GetPageFullHeight(_pageBounds[i].Height);
						if (b.Width > w) {
							w = b.Width.ToInt32();
						}
					}
					var h1 = GetPageFullHeight(_pageBounds[1].Height);
					if (h1 < vs.Height) {
						h += vs.Height - h1;
					}
				}
				VirtualSize = new Size(w = GetPageFullWidth(w), h);
				VerticalScroll.Visible = VerticalScroll.Enabled = h > ClientSize.Height;
				HorizontalScroll.Visible = HorizontalScroll.Enabled = w > ClientSize.Width;
			}
			_lockDown = false;
		}

		public void ExecuteCommand(string cmd) {
			switch (cmd) {
				case "_FirstPage": ScrollToPage(1); break;
				case "_PreviousPage": Next(-1); break;
				case "_NextPage": Next(1); break;
				case "_LastPage": ScrollToPage(-1); break;
				case "_ScrollVertical": ContentDirection = Editor.ContentDirection.TopToDown; break;
				case "_ScrollHorizontal": ContentDirection = Editor.ContentDirection.RightToLeft; break;
				case "_TrueColorSpace": GrayScale = false; break;
				case "_GrayColorSpace": GrayScale = true; break;
				case "_InvertColor": InvertColor = !InvertColor; break;
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
			_pageOffsets = null;
			if (_cache != null) {
				lock (_cache.SyncObj) {
					_cache.Clear();
				}
			}
			_contentFlow = Editor.ContentDirection.TopToDown;
			_OcrOptions.CompressWhiteSpaces = true;
			_ocrResults = [];
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
	}
}
