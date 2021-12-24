using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Cyotek.Windows.Forms.Demo
{
    // Cyotek ImageBox
    // Copyright (c) 2010-2015 Cyotek Ltd.
    // http://cyotek.com
    // http://cyotek.com/blog/tag/imagebox

    // Licensed under the MIT License. See license.txt for the full text.

    // If you use this control in your applications, attribution, donations or contributions are welcome.

    internal class ImageBoxEx : ImageBox
    {
        #region Constants

        private readonly DragHandleCollection _dragHandles;

        private static readonly object _eventDragHandleSizeChanged = new object();

        private static readonly object _eventMaximumSelectionSizeChanged = new object();

        private static readonly object _eventMinimumSelectionSizeChanged = new object();

        private static readonly object _eventSelectionMoved = new object();

        private static readonly object _eventSelectionMoving = new object();

        private static readonly object _eventSelectionResized = new object();

        private static readonly object _eventSelectionResizing = new object();

        #endregion

        #region Fields

        private int _dragHandleSize;

        private Point _dragOrigin;

        private Point _dragOriginOffset;

        private bool _isMoving;

        private bool _isResizing;

        private Size _maximumSelectionSize;

        private Size _minimumSelectionSize;

        private RectangleF _previousSelectionRegion;

        private DragHandleAnchor _resizeAnchor;

        #endregion

        #region Constructors

        public ImageBoxEx() {
            _dragHandles = new DragHandleCollection();
            _dragHandleSize = 8;
            _maximumSelectionSize = Size.Empty;
            this.PositionDragHandles();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the DragHandleSize property value changes
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler DragHandleSizeChanged {
            add { this.Events.AddHandler(_eventDragHandleSizeChanged, value); }
            remove { this.Events.RemoveHandler(_eventDragHandleSizeChanged, value); }
        }

        /// <summary>
        /// Occurs when the MaximumSelectionSize property value changes
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler MaximumSelectionSizeChanged {
            add { this.Events.AddHandler(_eventMaximumSelectionSizeChanged, value); }
            remove { this.Events.RemoveHandler(_eventMaximumSelectionSizeChanged, value); }
        }

        /// <summary>
        /// Occurs when the MinimumSelectionSize property value changes
        /// </summary>
        [Category("Property Changed")]
        public event EventHandler MinimumSelectionSizeChanged {
            add { this.Events.AddHandler(_eventMinimumSelectionSizeChanged, value); }
            remove { this.Events.RemoveHandler(_eventMinimumSelectionSizeChanged, value); }
        }

        [Category("Action")]
        public event EventHandler SelectionMoved {
            add { this.Events.AddHandler(_eventSelectionMoved, value); }
            remove { this.Events.RemoveHandler(_eventSelectionMoved, value); }
        }

        [Category("Action")]
        public event CancelEventHandler SelectionMoving {
            add { this.Events.AddHandler(_eventSelectionMoving, value); }
            remove { this.Events.RemoveHandler(_eventSelectionMoving, value); }
        }

        [Category("Action")]
        public event EventHandler SelectionResized {
            add { this.Events.AddHandler(_eventSelectionResized, value); }
            remove { this.Events.RemoveHandler(_eventSelectionResized, value); }
        }

        [Category("Action")]
        public event CancelEventHandler SelectionResizing {
            add { this.Events.AddHandler(_eventSelectionResizing, value); }
            remove { this.Events.RemoveHandler(_eventSelectionResizing, value); }
        }

        #endregion

        #region Properties

        [Browsable(false)]
        public DragHandleCollection DragHandles {
            get { return _dragHandles; }
        }

        [Category("Appearance")]
        [DefaultValue(8)]
        public virtual int DragHandleSize {
            get { return _dragHandleSize; }
            set {
                if (_dragHandleSize != value) {
                    _dragHandleSize = value;

                    this.OnDragHandleSizeChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMoving {
            get { return _isMoving; }
            protected set { _isMoving = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsResizing {
            get { return _isResizing; }
            protected set { _isResizing = value; }
        }

        [Category("Behavior")]
        [DefaultValue(typeof(Size), "0, 0")]
        public virtual Size MaximumSelectionSize {
            get { return _maximumSelectionSize; }
            set {
                if (this.MaximumSelectionSize != value) {
                    _maximumSelectionSize = value;

                    this.OnMaximumSelectionSizeChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Behavior")]
        [DefaultValue(typeof(Size), "0, 0")]
        public virtual Size MinimumSelectionSize {
            get { return _minimumSelectionSize; }
            set {
                if (this.MinimumSelectionSize != value) {
                    _minimumSelectionSize = value;

                    this.OnMinimumSelectionSizeChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        public RectangleF PreviousSelectionRegion {
            get { return _previousSelectionRegion; }
            protected set { _previousSelectionRegion = value; }
        }

        protected Point DragOrigin {
            get { return _dragOrigin; }
            set { _dragOrigin = value; }
        }

        protected Point DragOriginOffset {
            get { return _dragOriginOffset; }
            set { _dragOriginOffset = value; }
        }

        protected DragHandleAnchor ResizeAnchor {
            get { return _resizeAnchor; }
            set { _resizeAnchor = value; }
        }

        #endregion

        #region Methods

        public void CancelResize() {
            this.SelectionRegion = _previousSelectionRegion;
            this.CompleteResize();
        }

        public void StartMove() {
            CancelEventArgs e;

            if (_isMoving || _isResizing) {
                throw new InvalidOperationException("A move or resize action is currently being performed.");
            }

            e = new CancelEventArgs();

            this.OnSelectionMoving(e);

            if (!e.Cancel) {
                _previousSelectionRegion = this.SelectionRegion;
                _isMoving = true;
            }
        }

        protected virtual void DrawDragHandle(Graphics graphics, DragHandle handle) {
            int left;
            int top;
            int width;
            int height;
            Pen outerPen;
            Brush innerBrush;

            left = handle.Bounds.Left;
            top = handle.Bounds.Top;
            width = handle.Bounds.Width;
            height = handle.Bounds.Height;

            if (handle.Enabled) {
                outerPen = SystemPens.WindowFrame;
                innerBrush = SystemBrushes.Window;
            }
            else {
                outerPen = SystemPens.ControlDark;
                innerBrush = SystemBrushes.Control;
            }

            graphics.FillRectangle(innerBrush, left + 1, top + 1, width - 2, height - 2);
            graphics.DrawLine(outerPen, left + 1, top, left + width - 2, top);
            graphics.DrawLine(outerPen, left, top + 1, left, top + height - 2);
            graphics.DrawLine(outerPen, left + 1, top + height - 1, left + width - 2, top + height - 1);
            graphics.DrawLine(outerPen, left + width - 1, top + 1, left + width - 1, top + height - 2);
        }

        /// <summary>
        /// Raises the <see cref="DragHandleSizeChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnDragHandleSizeChanged(EventArgs e) {
            EventHandler handler;

            this.PositionDragHandles();
            this.Invalidate();

            handler = (EventHandler)this.Events[_eventDragHandleSizeChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="MaximumSelectionSizeChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnMaximumSelectionSizeChanged(EventArgs e) {
            EventHandler handler;

            handler = (EventHandler)this.Events[_eventMaximumSelectionSizeChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="MinimumSelectionSizeChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnMinimumSelectionSizeChanged(EventArgs e) {
            EventHandler handler;

            handler = (EventHandler)this.Events[_eventMinimumSelectionSizeChanged];

            handler?.Invoke(this, e);
        }

        /// <summary>
        ///   Raises the <see cref="System.Windows.Forms.Control.MouseDown" /> event.
        /// </summary>
        /// <param name="e">
        ///   A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.
        /// </param>
        protected override void OnMouseDown(MouseEventArgs e) {
            Point imagePoint;
            RectangleF selectionRegion;

            imagePoint = this.PointToImage(e.Location);
            selectionRegion = this.SelectionRegion;

            if (e.Button == MouseButtons.Left && (selectionRegion.Contains(imagePoint) || this.HitTest(e.Location) != DragHandleAnchor.None)) {
                _dragOrigin = e.Location;
                _dragOriginOffset = new Point(imagePoint.X - (int)selectionRegion.X, imagePoint.Y - (int)selectionRegion.Y);
            }
            else {
                _dragOriginOffset = Point.Empty;
                _dragOrigin = Point.Empty;
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        ///   Raises the <see cref="System.Windows.Forms.Control.MouseMove" /> event.
        /// </summary>
        /// <param name="e">
        ///   A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.
        /// </param>
        protected override void OnMouseMove(MouseEventArgs e) {
            // start either a move or a resize operation
            if (!this.IsSelecting && !_isMoving && !_isResizing && e.Button == MouseButtons.Left && !_dragOrigin.IsEmpty && this.IsOutsideDragZone(e.Location)) {
                DragHandleAnchor anchor;

                anchor = this.HitTest(_dragOrigin);

                if (anchor == DragHandleAnchor.None) {
                    // move
                    this.StartMove();
                }
                else if (_dragHandles[anchor].
                           Enabled && _dragHandles[anchor].
                           Visible) {
                    // resize
                    this.StartResize(anchor);
                }
            }

            // set the cursor
            this.SetCursor(e.Location);

            // perform operations
            this.ProcessSelectionMove(e.Location);
            this.ProcessSelectionResize(e.Location);

            base.OnMouseMove(e);
        }

        /// <summary>
        ///   Raises the <see cref="System.Windows.Forms.Control.MouseUp" /> event.
        /// </summary>
        /// <param name="e">
        ///   A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.
        /// </param>
        protected override void OnMouseUp(MouseEventArgs e) {
            if (_isMoving) {
                this.CompleteMove();
            }
            else if (_isResizing) {
                this.CompleteResize();
            }

            base.OnMouseUp(e);
        }

        /// <summary>
        ///   Raises the <see cref="System.Windows.Forms.Control.Paint" /> event.
        /// </summary>
        /// <param name="e">
        ///   A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.
        /// </param>
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            if (this.AllowPainting && !this.SelectionRegion.IsEmpty) {
                foreach (DragHandle handle in _dragHandles) {
                    if (handle.Visible) {
                        this.DrawDragHandle(e.Graphics, handle);
                    }
                }
            }
        }

        /// <summary>
        ///   Raises the <see cref="ImageBox.PanStart" /> event.
        /// </summary>
        /// <param name="e">
        ///   The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.
        /// </param>
        protected override void OnPanStart(CancelEventArgs e) {
            if (_isMoving || _isResizing || !_dragOrigin.IsEmpty) {
                e.Cancel = true;
            }

            base.OnPanStart(e);
        }

        /// <summary>
        ///   Raises the <see cref="System.Windows.Forms.Control.Resize" /> event.
        /// </summary>
        /// <param name="e">
        ///   An <see cref="T:System.EventArgs" /> that contains the event data.
        /// </param>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            this.PositionDragHandles();
        }

        /// <summary>
        ///   Raises the <see cref="System.Windows.Forms.ScrollableControl.Scroll" /> event.
        /// </summary>
        /// <param name="se">
        ///   A <see cref="T:System.Windows.Forms.ScrollEventArgs" /> that contains the event data.
        /// </param>
        protected override void OnScroll(ScrollEventArgs se) {
            base.OnScroll(se);

            this.PositionDragHandles();
        }

        /// <summary>
        ///   Raises the <see cref="ImageBox.Selecting" /> event.
        /// </summary>
        /// <param name="e">
        ///   The <see cref="System.EventArgs" /> instance containing the event data.
        /// </param>
        protected override void OnSelecting(ImageBoxCancelEventArgs e) {
            e.Cancel = _isMoving || _isResizing || this.SelectionRegion.Contains(this.PointToImage(e.Location)) || this.HitTest(e.Location) != DragHandleAnchor.None;

            base.OnSelecting(e);
        }

        /// <summary>
        /// Raises the <see cref="SelectionMoved" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnSelectionMoved(EventArgs e) {
            EventHandler handler;

            handler = (EventHandler)this.Events[_eventSelectionMoved];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="SelectionMoving" /> event.
        /// </summary>
        /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
        protected virtual void OnSelectionMoving(CancelEventArgs e) {
            CancelEventHandler handler;

            handler = (CancelEventHandler)this.Events[_eventSelectionMoving];

            handler?.Invoke(this, e);
        }

        /// <summary>
        ///   Raises the <see cref="ImageBox.SelectionRegionChanged" /> event.
        /// </summary>
        /// <param name="e">
        ///   The <see cref="System.EventArgs" /> instance containing the event data.
        /// </param>
        protected override void OnSelectionRegionChanged(EventArgs e) {
            base.OnSelectionRegionChanged(e);

            this.PositionDragHandles();
        }

        /// <summary>
        /// Raises the <see cref="SelectionResized" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnSelectionResized(EventArgs e) {
            EventHandler handler;

            handler = (EventHandler)this.Events[_eventSelectionResized];

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="SelectionResizing" /> event.
        /// </summary>
        /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
        protected virtual void OnSelectionResizing(CancelEventArgs e) {
            CancelEventHandler handler;

            handler = (CancelEventHandler)this.Events[_eventSelectionResizing];

            handler?.Invoke(this, e);
        }

        /// <summary>
        ///   Raises the <see cref="ImageBox.ZoomChanged" /> event.
        /// </summary>
        /// <param name="e">
        ///   The <see cref="System.EventArgs" /> instance containing the event data.
        /// </param>
        protected override void OnZoomChanged(EventArgs e) {
            base.OnZoomChanged(e);

            this.PositionDragHandles();
        }

        /// <summary>
        /// Processes a dialog key.
        /// </summary>
        /// <returns>
        /// true if the key was processed by the control; otherwise, false.
        /// </returns>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values that represents the key to process. </param>
        protected override bool ProcessDialogKey(Keys keyData) {
            bool result;

            if (keyData == Keys.Escape && (_isResizing || _isMoving)) {
                if (_isResizing) {
                    this.CancelResize();
                }
                else {
                    this.CancelMove();
                }

                result = true;
            }
            else {
                result = base.ProcessDialogKey(keyData);
            }

            return result;
        }

        protected virtual void SetCursor(Point point) {
            // http://forums.cyotek.com/imagebox/cursor-issue-in-imageboxex/msg92/#msg92

            if (!this.IsPanning) {
                Cursor cursor;

                if (this.IsSelecting) {
                    cursor = Cursors.Default;
                }
                else {
                    DragHandleAnchor handleAnchor;

                    handleAnchor = _isResizing ? _resizeAnchor : this.HitTest(point);
                    if (handleAnchor != DragHandleAnchor.None && _dragHandles[handleAnchor].
                          Enabled) {
                        switch (handleAnchor) {
                            case DragHandleAnchor.TopLeft:
                            case DragHandleAnchor.BottomRight:
                                cursor = Cursors.SizeNWSE;
                                break;
                            case DragHandleAnchor.TopCenter:
                            case DragHandleAnchor.BottomCenter:
                                cursor = Cursors.SizeNS;
                                break;
                            case DragHandleAnchor.TopRight:
                            case DragHandleAnchor.BottomLeft:
                                cursor = Cursors.SizeNESW;
                                break;
                            case DragHandleAnchor.MiddleLeft:
                            case DragHandleAnchor.MiddleRight:
                                cursor = Cursors.SizeWE;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else if (_isMoving || this.SelectionRegion.Contains(this.PointToImage(point))) {
                        cursor = Cursors.SizeAll;
                    }
                    else {
                        cursor = Cursors.Default;
                    }
                }

                this.Cursor = cursor;
            }
        }

        private void CancelMove() {
            this.SelectionRegion = _previousSelectionRegion;
            this.CompleteMove();
        }

        private void CompleteMove() {
            this.ResetDrag();
            this.OnSelectionMoved(EventArgs.Empty);
        }

        private void CompleteResize() {
            this.ResetDrag();
            this.OnSelectionResized(EventArgs.Empty);
        }

        private DragHandleAnchor HitTest(Point cursorPosition) {
            return _dragHandles.HitTest(cursorPosition);
        }

        private bool IsOutsideDragZone(Point location) {
            Rectangle dragZone;
            int dragWidth;
            int dragHeight;

            dragWidth = SystemInformation.DragSize.Width;
            dragHeight = SystemInformation.DragSize.Height;
            dragZone = new Rectangle(_dragOrigin.X - (dragWidth / 2), _dragOrigin.Y - (dragHeight / 2), dragWidth, dragHeight);

            return !dragZone.Contains(location);
        }

        private void PositionDragHandles() {
            if (_dragHandles != null && _dragHandleSize > 0) {
                RectangleF selectionRegion;

                selectionRegion = this.SelectionRegion;

                if (selectionRegion.IsEmpty) {
                    foreach (DragHandle handle in _dragHandles) {
                        handle.Bounds = Rectangle.Empty;
                    }
                }
                else {
                    int left;
                    int top;
                    int right;
                    int bottom;
                    int halfWidth;
                    int halfHeight;
                    int halfDragHandleSize;
                    Rectangle viewport;
                    int offsetX;
                    int offsetY;

                    viewport = this.GetImageViewPort();
                    offsetX = viewport.Left + this.Padding.Left + this.AutoScrollPosition.X;
                    offsetY = viewport.Top + this.Padding.Top + this.AutoScrollPosition.Y;
                    halfDragHandleSize = _dragHandleSize / 2;
                    left = Convert.ToInt32((selectionRegion.Left * this.ZoomFactor) + offsetX);
                    top = Convert.ToInt32((selectionRegion.Top * this.ZoomFactor) + offsetY);
                    right = left + Convert.ToInt32(selectionRegion.Width * this.ZoomFactor);
                    bottom = top + Convert.ToInt32(selectionRegion.Height * this.ZoomFactor);
                    halfWidth = Convert.ToInt32(selectionRegion.Width * this.ZoomFactor) / 2;
                    halfHeight = Convert.ToInt32(selectionRegion.Height * this.ZoomFactor) / 2;

                    _dragHandles[DragHandleAnchor.TopLeft].
                      Bounds = new Rectangle(left - _dragHandleSize, top - _dragHandleSize, _dragHandleSize, _dragHandleSize);
                    _dragHandles[DragHandleAnchor.TopCenter].
                      Bounds = new Rectangle(left + halfWidth - halfDragHandleSize, top - _dragHandleSize, _dragHandleSize, _dragHandleSize);
                    _dragHandles[DragHandleAnchor.TopRight].
                      Bounds = new Rectangle(right, top - _dragHandleSize, _dragHandleSize, _dragHandleSize);
                    _dragHandles[DragHandleAnchor.MiddleLeft].
                      Bounds = new Rectangle(left - _dragHandleSize, top + halfHeight - halfDragHandleSize, _dragHandleSize, _dragHandleSize);
                    _dragHandles[DragHandleAnchor.MiddleRight].
                      Bounds = new Rectangle(right, top + halfHeight - halfDragHandleSize, _dragHandleSize, _dragHandleSize);
                    _dragHandles[DragHandleAnchor.BottomLeft].
                      Bounds = new Rectangle(left - _dragHandleSize, bottom, _dragHandleSize, _dragHandleSize);
                    _dragHandles[DragHandleAnchor.BottomCenter].
                      Bounds = new Rectangle(left + halfWidth - halfDragHandleSize, bottom, _dragHandleSize, _dragHandleSize);
                    _dragHandles[DragHandleAnchor.BottomRight].
                      Bounds = new Rectangle(right, bottom, _dragHandleSize, _dragHandleSize);
                }
            }
        }

        private void ProcessSelectionMove(Point cursorPosition) {
            if (_isMoving) {
                int x;
                int y;
                Point imagePoint;
                Size viewSize;
                RectangleF selectionRegion;

                imagePoint = this.PointToImage(cursorPosition, false);
                viewSize = this.ViewSize;
                selectionRegion = this.SelectionRegion;

                x = Math.Max(0, imagePoint.X - _dragOriginOffset.X);
                if (x + selectionRegion.Width >= viewSize.Width) {
                    x = viewSize.Width - (int)selectionRegion.Width;
                }

                y = Math.Max(0, imagePoint.Y - _dragOriginOffset.Y);
                if (y + selectionRegion.Height >= viewSize.Height) {
                    y = viewSize.Height - (int)selectionRegion.Height;
                }

                this.SelectionRegion = new RectangleF(x, y, selectionRegion.Width, selectionRegion.Height);
            }
        }

        private void ProcessSelectionResize(Point cursorPosition) {
            if (_isResizing) {
                Point imagePosition;
                float left;
                float top;
                float right;
                float bottom;
                bool resizingTopEdge;
                bool resizingBottomEdge;
                bool resizingLeftEdge;
                bool resizingRightEdge;
                RectangleF selectionRegion;
                Size viewSize;

                imagePosition = this.PointToImage(cursorPosition);
                viewSize = this.ViewSize;

                // get the current selection
                selectionRegion = this.SelectionRegion;
                left = selectionRegion.Left;
                top = selectionRegion.Top;
                right = selectionRegion.Right;
                bottom = selectionRegion.Bottom;

                // decide which edges we're resizing
                resizingTopEdge = _resizeAnchor >= DragHandleAnchor.TopLeft && _resizeAnchor <= DragHandleAnchor.TopRight;
                resizingBottomEdge = _resizeAnchor >= DragHandleAnchor.BottomLeft && _resizeAnchor <= DragHandleAnchor.BottomRight;
                resizingLeftEdge = _resizeAnchor == DragHandleAnchor.TopLeft || _resizeAnchor == DragHandleAnchor.MiddleLeft || _resizeAnchor == DragHandleAnchor.BottomLeft;
                resizingRightEdge = _resizeAnchor == DragHandleAnchor.TopRight || _resizeAnchor == DragHandleAnchor.MiddleRight || _resizeAnchor == DragHandleAnchor.BottomRight;

                // and resize!
                if (resizingTopEdge) {
                    top = imagePosition.Y > 0 ? imagePosition.Y : 0;

                    if (bottom - top < this.MinimumSelectionSize.Height) {
                        top = bottom - this.MinimumSelectionSize.Height;
                    }
                    else if (this.MaximumSelectionSize.Height > 0 && bottom - top > this.MaximumSelectionSize.Height) {
                        top = bottom - this.MaximumSelectionSize.Height;
                    }
                }
                else if (resizingBottomEdge) {
                    bottom = imagePosition.Y < viewSize.Height ? imagePosition.Y : viewSize.Height;

                    if (bottom - top < this.MinimumSelectionSize.Height) {
                        bottom = top + this.MinimumSelectionSize.Height;
                    }
                    else if (this.MaximumSelectionSize.Height > 0 && bottom - top > this.MaximumSelectionSize.Height) {
                        bottom = top + this.MaximumSelectionSize.Height;
                    }
                }

                if (resizingLeftEdge) {
                    left = imagePosition.X > 0 ? imagePosition.X : 0;

                    if (right - left < this.MinimumSelectionSize.Width) {
                        left = right - this.MinimumSelectionSize.Width;
                    }
                    else if (this.MaximumSelectionSize.Width > 0 && right - left > this.MaximumSelectionSize.Width) {
                        left = right - this.MaximumSelectionSize.Width;
                    }
                }
                else if (resizingRightEdge) {
                    right = imagePosition.X < viewSize.Width ? imagePosition.X : viewSize.Width;

                    if (right - left < this.MinimumSelectionSize.Width) {
                        right = left + this.MinimumSelectionSize.Width;
                    }
                    else if (this.MaximumSelectionSize.Width > 0 && right - left > this.MaximumSelectionSize.Width) {
                        right = left + this.MaximumSelectionSize.Width;
                    }
                }

                this.SelectionRegion = new RectangleF(left, top, right - left, bottom - top);
            }
        }

        private void ResetDrag() {
            _isResizing = false;
            _isMoving = false;
            _dragOrigin = Point.Empty;
            _dragOriginOffset = Point.Empty;
        }

        private void StartResize(DragHandleAnchor anchor) {
            CancelEventArgs e;

            if (_isMoving || _isResizing) {
                throw new InvalidOperationException("A move or resize action is currently being performed.");
            }

            e = new CancelEventArgs();

            this.OnSelectionResizing(e);

            if (!e.Cancel) {
                _resizeAnchor = anchor;
                _previousSelectionRegion = this.SelectionRegion;
                _isResizing = true;
            }
        }

        #endregion
    }
}
