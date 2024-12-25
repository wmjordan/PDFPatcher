using System;
using System.Drawing;
using BrightIdeasSoftware;
using PDFPatcher.Model;

namespace PDFPatcher.Functions.Editor.Parts
{
	internal sealed class BookmarkInViewSynchronizer
	{
		readonly BookmarkEditorView _BookmarkBox;
		readonly ViewerControl _ViewerBox;
		readonly RowBorderDecoration _BookmarkInViewDecoration = new RowBorderDecoration() {
			FillBrush = new SolidBrush(Color.FromArgb(40, Color.Yellow)),
			BorderPen = new Pen(Color.FromArgb(216, Color.Yellow))
		};
		BookmarkElement _BookmarkInView = null;

		public BookmarkInViewSynchronizer(BookmarkEditorView bookmark, ViewerControl viewer) {
			_BookmarkBox = bookmark;
			_BookmarkBox.FormatRow += HighlightInViewRow;
			_BookmarkBox.ItemsChanged += HandleBookmarkChange;
			_BookmarkBox.BookmarkChanged += HandleBookmarkChange;
			_ViewerBox = viewer;
			_ViewerBox.PageChanged += SyncBookmarkPosition;
		}

		void HandleBookmarkChange(object sender, EventArgs e) {
			SyncView();
		}

		public BookmarkElement BookmarkInView {
			get => _BookmarkInView;
			internal set {
				if (value != _BookmarkInView) {
					var old = _BookmarkInView;
					_BookmarkInView = value;
					var i = _BookmarkBox.IndexOf(old);
					if (i >= 0) {
						_BookmarkBox.RedrawItem(i);
					}
					i = _BookmarkBox.IndexOf(value);
					if (i >= 0) {
						_BookmarkBox.RedrawItem(i);
					}
				}
			}
		}

		void SyncBookmarkPosition(object sender, ViewerControl.PageChangedEventArgs e) {
			SyncView();
		}

		void SyncView() {
			int l = _BookmarkBox.GetItemCount();
			if (l == 0) {
				return;
			}
			var p = _ViewerBox.GetCurrentScrollPosition();
			if (p.Page == 0) {
				return;
			}
			BookmarkInView = FindNearestBookmark(l, p);
		}

		BookmarkElement FindNearestBookmark(int itemCount, PagePosition p) {
			BookmarkElement e = null;
			float t = float.NaN;
			var h = _ViewerBox.HorizontalFlow;
			int start = 0, end = itemCount, i = itemCount >> 1, bp;
			BookmarkElement b;
			// 使用二分法查找最近的书签，这里假设书签指向的页面从上到下是顺序的
			while (i != start && i != end) {
				b = _BookmarkBox.GetBookmark(i);
				bp = b.Page;
				if (bp == 0) {
					start = i;
					if (i < end) {
						++i;
					}
				}
				if (bp > p.Page) {
					end = i;
					i = start + ((i - start) >> 1);
				}
				else if (bp < p.Page) {
					start = i;
					i = end - ((end - i) >> 1);
					if (Single.IsNaN(t)) {
						e = b;
					}
				}
				else {
					if (h) {
						e = b;
						break;
					}
					if (Single.IsNaN(t)) {
						t = b.Top;
						e = b;
						if (t > p.PageY) {
							start = i;
							++i;
						}
						else if (t < p.PageY) {
							end = i;
							--i;
						}
						else {
							break;
						}
					}
					else {
						if (t > p.PageY) {
							if (t > b.Top && t - p.PageY >= Math.Abs(b.Top - p.PageY)) {
								t = b.Top;
								e = b;
								start = i;
								++i;
								continue;
							}
						}
						else if (t < p.PageY) {
							if (t < b.Top && p.PageY - t >= Math.Abs(b.Top - p.PageY)) {
								t = b.Top;
								e = b;
								end = i;
								--i;
								continue;
							}
						}
						else {
							e = b;
						}
						break;
					}
				}
			}
			return e ?? _BookmarkBox.GetBookmark(start);
		}

		void HighlightInViewRow(object sender, FormatRowEventArgs e) {
			if (e.Model == _BookmarkInView) {
				e.Item.Decorations.Add(_BookmarkInViewDecoration);
			}
		}
	}
}
