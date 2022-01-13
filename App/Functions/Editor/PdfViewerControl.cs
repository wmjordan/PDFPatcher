using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Cyotek.Windows.Forms;
using Cyotek.Windows.Forms.Demo;
using MuPdfSharp;
using PDFPatcher.Common;
using PDFPatcher.Functions.Editor;
using PDFPatcher.Model;
using PDFPatcher.Processor;
using PDFPatcher.Processor.Imaging;
using PDFPatcher.Properties;
using DrawingPoint = System.Drawing.Point;
using DrawingRectangle = System.Drawing.Rectangle;
using Matrix = System.Drawing.Drawing2D.Matrix;
using MuPoint = MuPdfSharp.Point;
using MuRectangle = MuPdfSharp.Rectangle;
using TextInfo = PDFPatcher.Functions.Editor.TextInfo;

namespace PDFPatcher.Functions;

internal sealed class PdfViewerControl : ImageBoxEx
{
	private static readonly IComparer<int> __horizontalComparer = ValueHelper.GetReverseComparer<int>();

	private static readonly int __pageMargin =
		(int)(TextRenderer.MeasureText("国", SystemFonts.MessageBoxFont).Height * 1.2d);

	private readonly Timer _refreshTimer;
	private readonly ImageRendererOptions _renderOptions;

	private readonly BackgroundWorker _renderWorker;

	/// <summary>
	///     缓存页面渲染结果的缓冲区。
	/// </summary>
	private RenderResultCache _cache;

	private bool _cancelRendering;
	private ContentDirection _contentFlow;

	private PageRange _DisplayRange;

	private string _LiteralZoom;
	private bool _lockDown;

	private SizeF _maxDimension;
	private MuDocument _mupdf;

	private Dictionary<int, List<TextLine>> _ocrResults;

	/// <summary>
	///     页面的尺寸信息。
	/// </summary>
	private MuRectangle[] _pageBounds;

	/// <summary>
	///     页面的滚动位置。
	/// </summary>
	private int[] _pageOffsets;

	private DrawingPoint _PinPoint;

	private bool _ShowPinPoint;

	private bool _ShowTextBorders;
	private float _zoomFactor;

	private ZoomMode _zoomMode;

	public PdfViewerControl() {
		VirtualMode = true;
		VirtualSize = Size.Empty;
		PanMode = ImageBoxPanMode.Both;
		AllowUnfocusedMouseWheel = true;
		_renderOptions = new ImageRendererOptions();
		//_ViewBox.SelectionMode = ImageBoxSelectionMode.Rectangle;

		_refreshTimer = new Timer {Interval = 200};
		_refreshTimer.Tick += (s, args) => {
			for (int i = _DisplayRange.StartValue; i <= _DisplayRange.EndValue; i++) {
				bool v;
				lock (_cache.SyncObj) {
					v = _cache.GetBitmap(i) != null;
				}

				if (v == false && _renderWorker.IsBusy == false) {
					_renderWorker.RunWorkerAsync();
					return;
				}
			}
		};

		_renderWorker = new BackgroundWorker {WorkerSupportsCancellation = true};
		_renderWorker.DoWork += (s, args) => {
			Tracker.DebugMessage("started prerender job: " + _DisplayRange);
			_refreshTimer.Stop();
			for (int i = _DisplayRange.StartValue;
			     i >= _DisplayRange.StartValue && i < _DisplayRange.EndValue + 2;
			     i++) {
				if (i < 1 || i > _mupdf.PageCount) {
					continue;
				}

				if (_cancelRendering || _renderWorker.CancellationPending || _mupdf.IsDocumentOpened == false) {
					_cancelRendering = false;
					args.Cancel = true;
					return;
				}

				if (_cache.GetBitmap(i) == null) {
					lock (_cache.SyncObj) {
						MuRectangle pb = _pageBounds[i];
						Tracker.DebugMessage("load page " + i);
						float z = GetZoomFactorForPage(pb);
						RenderPage(i, (pb.Width * z).ToInt32(), (pb.Height * z).ToInt32());
						if (_DisplayRange.Contains(i)) {
							Invalidate();
						}
					}
				}
			}
		};
		_renderWorker.RunWorkerCompleted += (s, args) => {
			if (_cancelRendering == false) {
				_refreshTimer.Start();
			}
		};
	}

	/// <summary>
	///     获取或设置显示的焦点页面。
	/// </summary>
	[DefaultValue(0)]
	public int CurrentPageNumber {
		get => HorizontalFlow ? _DisplayRange.EndValue : _DisplayRange.StartValue;
		set {
			if (value == CurrentPageNumber) {
				return;
			}

			ShowPage(value);
		}
	}

	/// <summary>
	///     获取当前可见的第一个页面。
	/// </summary>
	[Browsable(false)]
	public int FirstPage => _DisplayRange.StartValue;

	/// <summary>
	///     获取当前可见的最后一个页面。
	/// </summary>
	[Browsable(false)]
	public int LastPage => _DisplayRange.EndValue;

	/// <summary>
	///     获取文本识别选项。
	/// </summary>
	[Browsable(false)]
	public OcrOptions OcrOptions { get; } = new();

	/// <summary>
	///     获取或设置显示放大比率。
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
	///     获取或设置阅读器是否使用右到左的水平滚动模式。
	/// </summary>
	[DefaultValue(ContentDirection.TopToDown)]
	public ContentDirection ContentDirection {
		get => _contentFlow;
		set {
			if (value == _contentFlow) {
				return;
			}

			PagePosition pp = PagePosition.Empty;
			if (HorizontalScroll.Value != 0 || VerticalScroll.Value != 0) {
				pp = TransposeVirtualImageToPagePosition(HorizontalScroll.Value, VerticalScroll.Value);
			}

			Selection s = GetSelection();
			_contentFlow = value;
			UpdateDisplay(true);
			if (s.ImageRegion.IsEmpty == false) {
				RectangleF r = s.ImageRegion;
				DrawingPoint p = GetVirtualImageOffset(s.Page);
				r = new RectangleF(p.X + r.Left, p.Y + r.Top, r.Width, r.Height);
				SelectionRegion = r;
			}

			if (pp.Page > 0) {
				if (_zoomMode == ZoomMode.FitPage) {
					ShowPage(pp.Page);
				}
				else {
					ScrollToPosition(pp);
				}
			}
		}
	}

	public bool HorizontalFlow => _contentFlow != ContentDirection.TopToDown;

	/// <summary>
	///     获取或设置阅读器是否将页面渲染为灰度图像。
	/// </summary>
	[DefaultValue(false)]
	public bool GrayScale {
		get => _renderOptions.ColorSpace == ColorSpace.Gray;
		set {
			ColorSpace v = value ? ColorSpace.Gray : ColorSpace.Rgb;
			if (_renderOptions.ColorSpace != v) {
				_renderOptions.ColorSpace = v;
				UpdateDisplay();
			}
		}
	}

	/// <summary>
	///     获取或设置阅读器是否将页面渲染为反转颜色的效果。
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
	///     获取或设置阅读器的鼠标操作模式。
	/// </summary>
	[DefaultValue(MouseMode.Move)]
	public MouseMode MouseMode {
		get => PanMode != ImageBoxPanMode.None ? MouseMode.Move : MouseMode.Selection;
		set {
			if (value == MouseMode.Move) {
				PanMode = ImageBoxPanMode.Both;
				AllowZoom = false;
				SelectionMode = ImageBoxSelectionMode.None;
				SelectionRegion = RectangleF.Empty;
			}
			else {
				PanMode = ImageBoxPanMode.Both;
				AllowZoom = false;
				SelectionMode = ImageBoxSelectionMode.Rectangle;
			}
		}
	}

	[DefaultValue(false)] public bool FullPageScroll { get; set; }

	[Description("指定鼠标定位点")]
	public DrawingPoint PinPoint {
		get => _PinPoint;
		set {
			if (_PinPoint != value) {
				_PinPoint = value;
				if (IsPinPointVisible && DesignMode == false) {
					Invalidate();
				}
			}
		}
	}

	[DefaultValue(false)]
	[Description("指定是否显示鼠标定位点")]
	public bool ShowPinPoint {
		get => _ShowPinPoint;
		set {
			if (_ShowPinPoint != value) {
				_ShowPinPoint = value;
				if (IsPinPointVisible && DesignMode == false) {
					Invalidate();
				}
			}
		}
	}

	private bool IsPinPointVisible {
		get {
			if (PinPoint != DrawingPoint.Empty) {
				DrawingPoint op = GetOffsetPoint(0, 0);
				DrawingRectangle vp = GetImageViewPort();
				DrawingPoint pp = PinPoint;
				pp.Offset(op);
				if (vp.Contains(pp)) {
					return true;
				}
			}

			return false;
		}
	}

	[DefaultValue(false)]
	[Description("显示文本层的边框")]
	public bool ShowTextBorders {
		get => _ShowTextBorders;
		set {
			if (_ShowTextBorders != value) {
				_ShowTextBorders = value;
				if (DesignMode == false) {
					Invalidate();
				}
			}
		}
	}

	[DefaultValue(0)]
	[Description("指定用于识别文本的语言")]
	public int OcrLanguage {
		get => OcrOptions.OcrLangID;
		set {
			if (OcrOptions.OcrLangID == value) {
				return;
			}

			OcrOptions.OcrLangID = value;
			_ocrResults.Clear();
		}
	}

	[Description("指定需要显示的 PDF 文档")]
	[Browsable(false)]
	[DefaultValue(null)]
	public MuDocument Document {
		get => _mupdf;
		set {
			Enabled = false;
			InitViewer();
			_mupdf = value;
			if (value != null) {
				Tracker.DebugMessage("Load document.");
				int l = _mupdf.PageCount + 1;
				_pageOffsets = new int[l];
				_pageBounds = new MuRectangle[l];
				LoadPageBounds();
				_cache = new RenderResultCache(_mupdf);
				Tracker.DebugMessage("Calculating document virtual size.");
				CalculateZoomFactor(_LiteralZoom);
				CalculateDocumentVirtualSize();
				ShowPage(1);
				_refreshTimer.Start();
				if (_renderWorker.IsBusy == false) {
					_renderWorker.RunWorkerAsync();
				}

				DocumentLoaded?.Invoke(this, EventArgs.Empty);
				Enabled = true;
			}
		}
	}

	public event EventHandler DocumentLoaded;
	public new event EventHandler ZoomChanged;
	public event EventHandler<PageChangedEventArgs> PageChanged;
	public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

	protected override void OnCreateControl() {
		base.OnCreateControl();
		using (Graphics g = CreateGraphics()) {
			_renderOptions.Dpi = g.DpiX;
		}
	}

	protected override void OnMouseMove(MouseEventArgs e) {
		base.OnMouseMove(e);
		if (SelectionRegion.IsEmpty == false && (IsResizing || IsSelecting || IsMoving) &&
		    e.Button == MouseButtons.Left) {
			LimitSelectionInPage(e.Location);
		}
	}

	//        protected override void SetCursor (DrawingPoint point) {
	//            if (IsPanning) {
	//                return;
	//            }
	//#if DEBUG
	//            if (!IsResizing || !IsSelecting) {
	//                var p = TransposeClientToPageImage (point.X, point.Y);
	//                if (p.ImageY < 0) {
	//                    this.Cursor = Cursors.Hand;
	//                    return;
	//                }
	//            }
	//#endif
	//            base.SetCursor (point);
	//        }

	protected override void OnSelectionRegionChanged(EventArgs e) {
		base.OnSelectionRegionChanged(e);
		if (_mupdf == null || MouseMode == MouseMode.Move || SelectionChanged == null) {
			return;
		}

		SelectionChanged(this, new SelectionChangedEventArgs(GetSelection()));
	}

	protected override void OnClientSizeChanged(EventArgs e) {
		base.OnClientSizeChanged(e);
		if (_zoomMode != ZoomMode.Custom && _lockDown == false) {
			if (ChangeZoom(LiteralZoom) && ZoomChanged != null) {
				ZoomChanged(this, EventArgs.Empty);
			}

			//CalculateDocumentVirtualSize ();
			Invalidate();
		}
	}

	private void LimitSelectionInPage(DrawingPoint location) {
		RectangleF r = SelectionRegion;
		PagePosition pp = TransposeClientToPagePosition(location.X, location.Y);
		DrawingPoint p = GetVirtualImageOffset(pp.Page);
		Tracker.DebugMessage(pp.Location.ToString());
		r.Offset(-p.X, -p.Y);
		MuRectangle b = _pageBounds[pp.Page];
		float z = GetZoomFactorForPage(b);

		float x1 = r.Left, y1 = r.Top, x2 = r.Right, y2 = r.Bottom;
		bool c = false;
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
			x1 -= r.Right - (b.Width * z);
			if (x1 < 0) {
				x1 = 0;
			}

			c = true;
		}

		if (r.Bottom > b.Height * z) {
			y2 = b.Height * z;
			y1 -= r.Bottom - (b.Height * z);
			if (y1 < 0) {
				y1 = 0;
			}

			c = true;
		}

		if (c) {
			SelectionRegion = new MuRectangle(p.X + x1, p.Y + y1, p.X + x2, p.Y + y2);
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
					ScrollTo(HorizontalScroll.Value - (GetInsideViewPort().Width * 0.95).ToInt32(),
						VerticalScroll.Value);
				}
				else {
					ScrollTo(HorizontalScroll.Value,
						VerticalScroll.Value + (GetInsideViewPort().Height * 0.95).ToInt32());
				}

				return true;
			case Keys.PageUp:
				if (FullPageScroll) {
					ExecuteCommand("_PreviousPage");
					return true;
				}

				if (HorizontalFlow) {
					ScrollTo(HorizontalScroll.Value + (GetInsideViewPort().Width * 0.95).ToInt32(),
						VerticalScroll.Value);
				}
				else {
					ScrollTo(HorizontalScroll.Value,
						VerticalScroll.Value - (GetInsideViewPort().Height * 0.95).ToInt32());
				}

				return true;
			case Keys.Home:
				ShowPage(1);
				return true;
			case Keys.End:
				if (_mupdf != null) {
					ShowPage(_mupdf.PageCount);
				}

				return true;
		}

		return base.ProcessCmdKey(ref msg, keyData);
	}

	protected override void OnMouseWheel(MouseEventArgs e) {
		base.OnMouseWheel(e);
		if (HorizontalFlow) {
			ScrollTo(HorizontalScroll.Value + e.Delta, VerticalScroll.Value);
		}
		else {
			ScrollTo(HorizontalScroll.Value, VerticalScroll.Value - e.Delta);
		}
	}

	internal void CloseFile() {
		if (_mupdf != null) {
			_cache.Clear();
			_mupdf.ReleaseFile();
		}
	}

	internal void Reopen() {
		if (_mupdf != null && _mupdf.IsDocumentOpened == false) {
			_mupdf.Reopen();
			UpdateDisplay(true);
		}
	}

	protected override void OnVirtualDraw(PaintEventArgs e) {
		base.OnVirtualDraw(e);

		if (VirtualSize.IsEmpty || Enabled == false) {
			return;
		}

		_DisplayRange = GetDisplayingPageRange();
		int p = _DisplayRange.StartValue;
		PageChanged?.Invoke(this, new PageChangedEventArgs(p));
		Graphics g = e.Graphics;
		DrawingPoint op = GetOffsetPoint(0, 0); // 偏移位置点
		DrawingRectangle vp = GetImageViewPort();
		if (TintColor == Color.Transparent) {
			g.FillRectangle(Brushes.FloralWhite, vp);
		}
		else {
			using (SolidBrush b = new(BitmapHelper.Tint(Color.Gainsboro, TintColor))) {
				g.FillRectangle(b, vp);
			}
		}

		DrawingRectangle r = DrawingRectangle.Empty;
		do {
			Debug.Assert(p > 0 && p < _mupdf.PageCount + 1, p.ToString());
			MuRectangle pb = _pageBounds[p];
			float z = GetZoomFactorForPage(pb);
			int ox = HorizontalFlow ? _pageOffsets[p] : 0;
			int oy = HorizontalFlow ? 0 : _pageOffsets[p];
			r = new DrawingRectangle(
				ox + op.X + __pageMargin,
				oy + op.Y + __pageMargin,
				(pb.Width * z).ToInt32(),
				(pb.Height * z).ToInt32()
			);
			string pl = GetPageLabel(p);
			TextRenderer.DrawText(e.Graphics,
				string.Concat(pl, pl.Length > 0 ? " / 第 " : "第 ", p, " 页 (", pb.Width, " * ", pb.Height, ")"),
				SystemFonts.MessageBoxFont,
				new DrawingPoint(ox + op.X + __pageMargin, oy + op.Y),
				Color.Black);
			Bitmap bmp = _cache.GetBitmap(p);
			if (bmp == null) {
				g.FillRectangle(Brushes.White, r);
				if (_renderWorker.IsBusy == false) {
					_renderWorker.RunWorkerAsync();
				}
			}
			else {
				g.DrawImage(bmp, r.Location);
			}

			g.DrawRectangle(Pens.Black, r.Left - 1, r.Top - 1, r.Width + 1, r.Height + 1);
			if (ShowTextBorders) {
				DrawTextBorders(g, p, op);
			}
		} while ((HorizontalFlow ? r.Right > 0 : r.Bottom < vp.Height)
		         && ++p < _pageOffsets.Length);

		if (ShowPinPoint && PinPoint != DrawingPoint.Empty) {
			DrawingPoint pp = PinPoint.Transpose(op);
			if (vp.Contains(pp)) {
				g.DrawImage(Resources.Pin, pp.X, pp.Y - Resources.Pin.Height);
			}
		}

		if (_cache.GetBitmap(p + 1) == null && _renderWorker.IsBusy == false) {
			_renderWorker.RunWorkerAsync();
		}
	}

	private string GetPageLabel(int pageNumber) {
		if (_mupdf.IsDocumentOpened == false) {
			return string.Empty;
		}

		return _mupdf.PageLabels.Format(pageNumber);
	}

	private PageRange GetDisplayingPageRange() {
		DrawingRectangle b = GetOffsetRectangle(GetImageViewPort());
		return new PageRange(GetPageNumberFromOffset(-b.Left + b.Width, -b.Y),
			GetPageNumberFromOffset(-b.Left, -(b.Y - b.Height)));
	}

	private void DrawTextBorders(Graphics g, int pageNumber, DrawingPoint offset) {
		if (_mupdf.IsDocumentOpened == false) {
			return;
		}

		lock (_mupdf.SyncObj) {
			MuPage p = _cache.LoadPage(pageNumber);
			float z = GetZoomFactorForPage(p.VisualBound);
			DrawingPoint o = GetVirtualImageOffset(pageNumber);
			using (Pen spanPen = new(Color.LightGray, 1))
			using (Pen blockPen = new(Color.Gray, 1)) {
				blockPen.DashStyle = DashStyle.Dash;
				spanPen.DashStyle = DashStyle.Dash;
				using (Matrix m = new(z, 0, 0, z, offset.X + o.X, offset.Y + o.Y)) {
					g.MultiplyTransform(m);
				}

				foreach (MuTextBlock block in p.TextPage.Blocks) {
					g.DrawRectangle(blockPen, block.BBox);
					if (block == null) {
						continue;
					}

					foreach (MuTextLine line in block.Lines) {
						g.DrawRectangle(spanPen, line.BBox);
					}
				}
			}

			g.ResetTransform();
		}
	}

	/// <summary>
	///     返回选定区域。
	/// </summary>
	/// <returns>选定的矩形区域。</returns>
	internal Selection GetSelection() {
		PageRegion s = GetSelectionPageRegion();
		if (s.Page == 0 || _mupdf.IsDocumentOpened == false) {
			return Selection.Empty;
		}

		lock (_mupdf.SyncObj) {
			MuRectangle vb = _pageBounds[s.Page];
			MuRectangle sr = s.Region;
			MuRectangle pr = new(sr.Left - vb.Left, vb.Bottom - sr.Top, sr.Right - vb.Left,
				vb.Bottom - sr.Bottom);
			DrawingPoint o = GetVirtualImageOffset(s.Page);
			RectangleF area = SelectionRegion;
			area.Offset(-o.X, -o.Y);
			return new Selection(_cache, s.Page, pr, area);
		}
	}

	internal PageRegion GetSelectionPageRegion() {
		RectangleF area = SelectionRegion;
		if (area.IsEmpty) {
			return PageRegion.Empty;
		}

		DrawingRectangle b = GetOffsetRectangle(GetImageViewPort());
		PagePosition p1 = TransposeVirtualImageToPagePosition(area.Left.ToInt32(), area.Top.ToInt32());
		PagePosition p2 = TransposeVirtualImageToPagePosition(area.Right.ToInt32(), area.Bottom.ToInt32());
		return new PageRegion(p1, p2);
	}

	/// <summary>
	///     返回指定位置的文本行以及与该文本行具有相同样式的后续文本行。
	/// </summary>
	/// <param name="position">查找文本行的位置。</param>
	/// <returns>返回指定位置的文本行以及与该文本行具有相同样式的后续文本行。</returns>
	internal TextInfo FindTextLines(PagePosition position) {
		MuRectangle rect = new();
		TextInfo ti = new();
		if (_mupdf.IsDocumentOpened == false) {
			return ti;
		}

		lock (_mupdf.SyncObj) {
			MuPage page = _cache.LoadPage(position.Page);
			MuPoint point = position.Location.ToPageCoordinate(page.VisualBound);
			if (page.VisualBound.Contains(point) == false
			    || page.TextPage.BBox.Contains(point) == false) {
				return ti;
			}

			foreach (MuTextBlock block in page.TextPage.Blocks) {
				if (block.Type != ContentBlockType.Text || block.BBox.Contains(point) == false) {
					continue;
				}

				MuTextBlock tb = block;
				HashSet<IntPtr> s = null;
				MuTextLine l = null;
				List<MuTextLine> r = null;
				foreach (MuTextLine line in tb.Lines) {
					if (l == null) {
						if (line.BBox.Contains(point) == false) {
							continue;
						}

						s = new HashSet<IntPtr>(); // 获取选中文本行的文本样式集合
						r = new List<MuTextLine>();
						foreach (MuTextChar ch in line.Characters) {
							s.Add(ch.FontID);
						}

						rect = line.BBox;
						l = line;
						r.Add(l);
					}
					else {
						if (line.BBox.Top - l.BBox.Bottom > line.BBox.Height) {
							break;
						}

						// 获取具有相同样式的邻接文本行
						foreach (MuTextChar ch in line.Characters) {
							if (s.Contains(ch.FontID)) {
								r.Add(line);
								l = line;
								goto NEXT;
							}
						}
					}

					NEXT: ;
				}

				if (l != null) {
					List<MuTextSpan> spans = new(r.Count * 2);
					foreach (MuTextLine item in r) {
						spans.AddRange(item.Spans);
					}

					return new TextInfo(page, rect, r, spans);
				}
			}
		}

		return ti;
	}

	/// <summary>
	///     返回指定区域内的文本行。
	/// </summary>
	/// <param name="region">选择的区域。</param>
	/// <returns>区域内的文本行。</returns>
	internal List<MuTextLine> FindTextLines(PageRegion region) {
		if (_mupdf.IsDocumentOpened == false) {
			return null;
		}

		lock (_mupdf.SyncObj) {
			MuPage page = _cache.LoadPage(region.Page);
			MuRectangle pr = region.Region.ToPageCoordinate(page.VisualBound);
			if (pr.Intersect(page.VisualBound).IsEmpty) {
				return null;
			}

			if (pr.Intersect(page.TextPage.BBox).IsEmpty) {
				return null;
			}

			foreach (MuTextBlock block in page.TextPage.Blocks) {
				if (block.Type != ContentBlockType.Text || pr.Intersect(block.BBox).IsEmpty) {
					continue;
				}

				HashSet<int> s = new();
				List<MuTextLine> r = new();
				foreach (MuTextLine line in block.Lines) {
					if (pr.Intersect(line.BBox).IsEmpty == false) {
						r.Add(line);
					}
				}

				return r;
			}
		}

		return null;
	}

	private float GetZoomFactorForPage(MuRectangle bound) {
		return _zoomFactor;
	}

	public List<TextLine> OcrPage(int pageNumber, bool cached) {
		if (cached && _ocrResults.TryGetValue(pageNumber, out List<TextLine> r)) {
			return r;
		}

		r = Ocr(pageNumber);
		return _ocrResults[pageNumber] = r;
	}

	public string[] CleanUpOcrResult(List<TextLine> result) {
		return result.ConvertAll(t => OcrProcessor.CleanUpText(t.Text, OcrOptions)).ToArray();
	}

	private List<TextLine> Ocr(int pageNumber) {
		try {
			Bitmap bmp = GetPageImage(pageNumber);
			return OcrProcessor.OcrBitmap(bmp, OcrOptions);
		}
		catch (COMException ex) {
			switch (ex.ErrorCode) {
				case -959971327:
					FormHelper.InfoBox("识别引擎初始化时遇到错误。\n请尝试以管理员身份运行程序，或重新安装 Office 2007 的 MODI 组件。");
					return new List<TextLine>();
				case -959967087:
					FormHelper.ErrorBox("识别引擎无法识别本页文本。请尝试调整页面的显示比例，然后再执行识别。");
					return new List<TextLine>();
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
		MuRectangle b = _pageBounds[pageNumber];
		float z = GetZoomFactorForPage(b);
		return RenderPage(pageNumber, (z * b.Width).ToInt32(), (z * b.Height).ToInt32());
	}

	public MuPage LoadPage(int pageNumber) {
		return _cache.LoadPage(pageNumber);
	}

	public MuRectangle GetPageBound(int pageNumber) {
		return _pageBounds[pageNumber];
	}

	private Bitmap RenderPage(int pageNumber, int width, int height) {
		Bitmap bmp = _cache.GetBitmap(pageNumber);
		if (bmp != null) {
			return bmp;
		}

		if (_mupdf == null || _mupdf.IsDocumentOpened == false || Enabled == false) {
			return null;
		}

		lock (_mupdf.SyncObj) {
			MuPage p = _cache.LoadPage(pageNumber);
			if (pageNumber < _DisplayRange.StartValue - 1 || pageNumber > _DisplayRange.EndValue + 1) {
				return null;
			}

			Tracker.DebugMessage("render page " + pageNumber);
			bmp = p.RenderBitmapPage(width, height, _renderOptions);
			_cache.AddBitmap(pageNumber, bmp);
		}

		return bmp;
	}

	private int GetPageNumberFromOffset(int offsetX, int offsetY) {
		if (_mupdf == null) {
			return 0;
		}

		int p = HorizontalFlow
			? Array.BinarySearch(_pageOffsets, 1, _pageOffsets.Length - 1, offsetX, __horizontalComparer)
			: Array.BinarySearch(_pageOffsets, 1, _pageOffsets.Length - 1, offsetY);
		if (p < 0) {
			p = ~p;
			if (HorizontalFlow == false) {
				--p;
			}
		}

		if (p >= _pageOffsets.Length) {
			return _pageOffsets.Length - 1;
		}

		if (p < 1) {
			return 1;
		}

		return p;
	}

	private bool ChangeZoom(string zoomMode) {
		Selection s = GetSelection();
		PagePosition pp = PagePosition.Empty;
		float z = 0;
		if (s.Page > 0) {
			z = GetZoomFactorForPage(_pageBounds[s.Page]);
		}

		if (HorizontalScroll.Value != 0 || VerticalScroll.Value != 0) {
			pp = TransposeVirtualImageToPagePosition(HorizontalScroll.Value, VerticalScroll.Value);
		}

		if (CalculateZoomFactor(zoomMode) == false) {
			return false;
		}

		if (_mupdf == null) {
			return false;
		}

		UpdateDisplay(true);
		// 保持选区尺寸比例
		if (z > 0) {
			RectangleF r = s.ImageRegion;
			DrawingPoint p = GetVirtualImageOffset(s.Page);
			z = _zoomFactor / z;
			r = new RectangleF(p.X + (r.Left * z), p.Y + (r.Top * z), r.Width * z, r.Height * z);
			SelectionRegion = r;
		}

		if (pp.Page > 0) {
			if (_zoomMode == ZoomMode.FitPage) {
				ShowPage(pp.Page);
			}
			else {
				ScrollToPosition(pp);
			}
		}

		return true;
	}

	private bool CalculateZoomFactor(string zoomMode) {
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
				_zoomFactor = f / 100f * _renderOptions.Dpi / 72f;
				break;
		}

		return true;
	}

	private void UpdateDisplay() { UpdateDisplay(false); }

	private void UpdateDisplay(bool resized) {
		if (DesignMode) {
			return;
		}

		_refreshTimer.Stop();
		_renderWorker.CancelAsync();
		_cancelRendering = true;
		if (_cache != null) {
			lock (_mupdf.SyncObj) {
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

	private int GetPageFullWidth(float pageWidth) {
		return __pageMargin + __pageMargin + (pageWidth * _zoomFactor).ToInt32();
	}

	private int GetPageFullHeight(float pageHeight) {
		return __pageMargin + __pageMargin + (pageHeight * _zoomFactor).ToInt32();
	}

	private bool ShowPage(int pageNumber) {
		if (_mupdf == null || _pageOffsets == null) {
			return false;
		}

		if (pageNumber < 0) {
			pageNumber = _mupdf.PageCount + pageNumber + 1;
		}

		if (pageNumber <= 0 || pageNumber > _mupdf.PageCount) {
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
			FormHelper.ErrorBox(ex.Message);
			return false;
		}

		//if (PageChanged != null) {
		//	PageChanged (this, new PageChangedEventArgs (pageNumber));
		//}
		return true;
	}

	internal void ScrollToPosition(PagePosition position) {
		DrawingPoint op = GetVirtualImageOffset(position.Page);
		MuPoint l = position.Location.ToPageCoordinate(_pageBounds[position.Page]);
		float z = GetZoomFactorForPage(_pageBounds[position.Page]);
		bool h = HorizontalFlow;
		ScrollTo(
			position.PageX == 0 && h == false ? HorizontalScroll.Value : (l.X * z).ToInt32() + op.X,
			position.PageY == 0 && h
				? VerticalScroll.Value
				: (position.Location.Y == 0 ? 0 : (l.Y * z).ToInt32()) + op.Y
		);
	}

	private bool Next(int deltaPageNumber) {
		return ShowPage(CurrentPageNumber + deltaPageNumber);
	}

	private void LoadPageBounds() {
		float w = 0, h = 0;
		for (int i = _mupdf.PageCount; i > 0; i--) {
			using (MuPage p = _mupdf.LoadPage(i)) {
				MuRectangle b = p.VisualBound;
				_pageBounds[i] = b;
				if (b.Width > w) {
					w = b.Width;
				}

				if (b.Height > h) {
					h = b.Height;
				}
			}
		}

		_maxDimension = new SizeF(w, h);
	}

	private void CalculateDocumentVirtualSize() {
		if (_pageOffsets == null || _pageBounds == null) {
			return;
		}

		int h = 0, w = 0;
		int l = _mupdf.PageCount + 1;
		Size vs = GetInsideViewPort().Size;
		_lockDown = true;
		if (HorizontalFlow) {
			lock (_mupdf.SyncObj) {
				for (int i = l - 1; i >= 0; i--) {
					MuRectangle b = _pageBounds[i];
					_pageOffsets[i] = w;
					w += GetPageFullWidth(b.Width);
					if (b.Height > h) {
						h = b.Height.ToInt32();
					}
				}

				int w1 = GetPageFullWidth(_pageBounds[1].Width);
				if (w1 < vs.Width) {
					w += vs.Width - w1;
				}
			}

			VirtualSize = new Size(w, GetPageFullHeight(h));
		}
		else {
			lock (_mupdf.SyncObj) {
				for (int i = 1; i < l; i++) {
					MuRectangle b = _pageBounds[i];
					_pageOffsets[i] = h;
					h += GetPageFullHeight(_pageBounds[i].Height);
					if (b.Width > w) {
						w = b.Width.ToInt32();
					}
				}

				int h1 = GetPageFullHeight(_pageBounds[1].Height);
				if (h1 < vs.Height) {
					h += vs.Height - h1;
				}
			}

			VirtualSize = new Size(GetPageFullWidth(w), h);
		}

		_lockDown = false;
	}

	public void ExecuteCommand(string cmd) {
		switch (cmd) {
			case "_FirstPage":
				ShowPage(1);
				break;
			case "_PreviousPage":
				Next(-1);
				break;
			case "_NextPage":
				Next(1);
				break;
			case "_LastPage":
				ShowPage(-1);
				break;
			case "_ScrollVertical":
				ContentDirection = ContentDirection.TopToDown;
				break;
			case "_ScrollHorizontal":
				ContentDirection = ContentDirection.RightToLeft;
				break;
			case "_TrueColorSpace":
				GrayScale = false;
				break;
			case "_GrayColorSpace":
				GrayScale = true;
				break;
			case "_InvertColor":
				InvertColor = !InvertColor;
				break;
			case "_Refresh":
				UpdateDisplay();
				break;
		}
	}

	public void InitViewer() {
		_cancelRendering = true;
		_refreshTimer.Stop();
		SelectionRegion = DrawingRectangle.Empty;
		_DisplayRange = new PageRange();
		if (_LiteralZoom == null) {
			_zoomFactor = _renderOptions.Dpi / 72;
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

		_contentFlow = ContentDirection.TopToDown;
		OcrOptions.CompressWhiteSpaces = true;
		_ocrResults = new Dictionary<int, List<TextLine>>();
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		Tracker.DebugMessage("PDF Viewer control destroyed.");
		_cancelRendering = true;
		_mupdf?.AbortAsync();
		_refreshTimer.Stop();
		if (_cache != null) {
			_renderWorker.CancelAsync();
			lock (_cache.SyncObj) {
				_cache.Dispose();
			}
		}

		_renderWorker.Dispose();
		_refreshTimer.Dispose();
	}

	private enum ZoomMode
	{
		Custom, FitPage = -1, FitHorizontal = -2, FitVertical = -3
	}

	internal sealed class PageChangedEventArgs : EventArgs
	{
		public PageChangedEventArgs(int pageNumber) {
			PageNumber = pageNumber;
		}

		public int PageNumber { get; }
	}

	internal sealed class SelectionChangedEventArgs : EventArgs
	{
		public SelectionChangedEventArgs(Selection selection) {
			Selection = selection;
		}

		public Selection Selection { get; }
	}

	#region 坐标转换

	internal bool IsClientPointInSelection(DrawingPoint point) {
		return SelectionRegion.Contains(PointToImage(point));
	}

	internal RectangleF MuRectangleToImageRegion(int pageNumber, MuRectangle box) {
		bool rtl = HorizontalFlow;
		int o = _pageOffsets[pageNumber];
		float z = _zoomFactor;
		if (rtl) {
			return new RectangleF(o + __pageMargin + (box.Left * z), (box.Top * z) + __pageMargin, box.Width * z,
				box.Height * z);
		}

		return new RectangleF((box.Left * z) + __pageMargin, (box.Top * z) + __pageMargin + o, box.Width * z,
			box.Height * z);
	}

	/// <summary>
	///     将屏幕客户区域的位置转换为页面坐标。
	/// </summary>
	/// <param name="clientX">横坐标。</param>
	/// <param name="clientY">纵坐标。</param>
	/// <returns>页面坐标。</returns>
	internal PagePosition TransposeClientToPagePosition(int clientX, int clientY) {
		if (_DisplayRange.StartValue <= 0 || _pageBounds == null) {
			return PagePosition.Empty;
		}

		DrawingPoint p = PointToImage(clientX, clientY);
		return TransposeVirtualImageToPagePosition(p.X, p.Y);
	}

	/// <summary>
	///     将虚拟画布的坐标点转换为屏幕客户区域的位置。
	/// </summary>
	/// <param name="imageX">虚拟画布位置的横坐标。</param>
	/// <param name="imageY">虚拟画布位置的横坐标。</param>
	/// <returns>屏幕客户区域的位置。</returns>
	internal DrawingPoint TransposeVirtualImageToClient(float imageX, float imageY) {
		DrawingRectangle vp = GetImageViewPort();
		return new DrawingPoint(vp.Left + AutoScrollPosition.X + imageX.ToInt32(),
			vp.Top + AutoScrollPosition.Y + imageY.ToInt32());
	}

	/// <summary>
	///     获取指定页面在虚拟画布上的绘制坐标点。
	/// </summary>
	/// <param name="pageNumber">页面编号。</param>
	/// <returns>页面左上角在虚拟画布上的坐标点。</returns>
	internal DrawingPoint GetVirtualImageOffset(int pageNumber) {
		bool rtl = HorizontalFlow;
		int ox = rtl ? _pageOffsets[pageNumber] : 0;
		int oy = rtl ? 0 : _pageOffsets[pageNumber];
		return new DrawingPoint(ox + __pageMargin, oy + __pageMargin);
	}

	/// <summary>
	///     将虚拟画布的位置转换为页面坐标。
	/// </summary>
	/// <param name="imageX">虚拟画布位置的横坐标。</param>
	/// <param name="imageY">虚拟画布位置的纵坐标。</param>
	/// <returns>页面坐标。</returns>
	internal PagePosition TransposeVirtualImageToPagePosition(int imageX, int imageY) {
		int n = GetPageNumberFromOffset(imageX, imageY);
		return TransposeVirtualImageToPagePosition(n, imageX, imageY);
	}

	/// <summary>
	///     将屏幕客户区域的位置转换为渲染页面位置。
	/// </summary>
	/// <param name="clientX">屏幕区域的横坐标。</param>
	/// <param name="clientY">屏幕区域的纵坐标。</param>
	/// <returns>渲染页面的位置。</returns>
	internal PagePoint TransposeClientToPageImage(int clientX, int clientY) {
		if (_DisplayRange.StartValue <= 0 || _pageBounds == null || IsPointInImage(clientX, clientY) == false) {
			return PagePoint.Empty;
		}

		DrawingPoint p = PointToImage(clientX, clientY);
		int n = GetPageNumberFromOffset(p.X, p.Y);
		DrawingPoint o = GetVirtualImageOffset(n);
		return new PagePoint(n, p.X - o.X, p.Y - o.Y);
	}

	/// <summary>
	///     将虚拟页面的位置转换为PDF页面位置。
	/// </summary>
	/// <param name="pageNumber">页码。</param>
	/// <param name="imageX">虚拟图片的横坐标。</param>
	/// <param name="imageY">虚拟图片的纵坐标。</param>
	/// <returns>PDF 页面的位置。</returns>
	internal PagePosition TransposeVirtualImageToPagePosition(int pageNumber, int imageX, int imageY) {
		DrawingPoint o = GetVirtualImageOffset(pageNumber);
		MuRectangle b = _pageBounds[pageNumber];
		float z = GetZoomFactorForPage(b);
		float ox = (imageX - o.X) / z;
		float oy = (imageY - o.Y) / z;
		return new PagePosition(pageNumber,
			b.Left + ox, b.Top + b.Height - oy,
			imageX - o.X, imageY - o.Y,
			b.Contains(ox, oy));
	}

	internal PagePosition TransposePageImageToPagePosition(int pageNumber, float pageImageX,
		float pageImageY) {
		MuRectangle b = _pageBounds[pageNumber];
		float z = _zoomFactor;
		float ox = pageImageX / z;
		float oy = pageImageY / z;
		return new PagePosition(pageNumber,
			b.Left + ox, b.Top + b.Height - oy,
			pageImageX.ToInt32(), pageImageY.ToInt32(),
			b.Contains(ox, oy));
	}

	#endregion
}