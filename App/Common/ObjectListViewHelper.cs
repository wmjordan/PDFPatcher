using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BrightIdeasSoftware
{
	static class ObjectListViewHelper
	{
		/// <summary>修复编辑控件太窄小的问题。</summary>
		public static void FixEditControlWidth(this ObjectListView view) {
			view.CellEditStarting += View_CellEditStarting;
			view.Disposed += View_Disposed;
		}

		static void View_Disposed(object sender, EventArgs e) {
			var view = (ObjectListView)sender;
			view.CellEditStarting -= View_CellEditStarting;
			view.Disposed -= View_Disposed;
		}

		static void View_CellEditStarting(object sender, CellEditEventArgs e) {
			var b = e.CellBounds;
			if (b.Width < 60) {
				b.Width = 60;
			}
			if (e.Control is System.Windows.Forms.Control c) {
				c.Bounds = b;
				c.Location = b.Location;
			}
		}

		public static void RedrawItem(this ObjectListView view, int index) {
			var c = view.GetItemCount();
			if (c > 0) {
				view.RedrawItems(Math.Max(0, index - 1), Math.Min(index + 1, c - 1), true);
			}
		}

		public static void SetTreeViewLine(this TreeListView view) {
			var tcr = view.TreeColumnRenderer as TreeListView.TreeRenderer;
			tcr.LinePen = new Pen(SystemColors.ControlDark) {
				DashCap = DashCap.Round,
				DashStyle = DashStyle.Dash
			};
		}

		public static void ExpandSelected(this TreeListView view, bool moveToFirstChild = false) {
			var so = view.SelectedObjects;
			if (so.Count == 0) {
				return;
			}
			var expanded = false;
			foreach (var item in so) {
				if (view.IsExpanded(item) == false) {
					view.Expand(item);
					expanded = true;
				}
			}
			if (moveToFirstChild && expanded == false) {
				foreach (var ch in view.GetChildren(so[0])) {
					view.SelectedObject = view.FocusedObject = ch;
					return;
				}
			}
		}
		public static void Expand(this TreeListView view, System.Collections.IEnumerable items, bool recursive) {
			foreach (var item in items) {
				view.Expand(item);
				if (recursive) {
					view.Expand(view.GetChildren(item), true);
				}
			}
		}
		public static void Collapse(this TreeListView view, System.Collections.IEnumerable items, bool recursive) {
			foreach (var item in items) {
				if (recursive && view.IsExpanded(item)) {
					view.Collapse(view.GetChildren(item), true);
				}
				view.Collapse(item);
			}
		}

		public static TypedObjectListView<T> AsTyped<T>(this ObjectListView view) where T : class {
			return view.AsTyped<T>(null);
		}

		public static TypedObjectListView<T> AsTyped<T>(this ObjectListView view, Action<TypedObjectListView<T>> configurator) where T : class {
			var v = new TypedObjectListView<T>(view);
			configurator?.Invoke(v);
			return v;
		}
		public static TypedObjectListView<T> ConfigColumn<T>(this TypedObjectListView<T> view, OLVColumn column, Action<TypedColumn<T>> configurator) where T : class {
			var t = new TypedColumn<T>(column);
			configurator(t);
			return view;
		}
		public static TypedColumn<T> AsTyped<T>(this OLVColumn column, Action<TypedColumn<T>> configurator) where T : class {
			var t = new TypedColumn<T>(column);
			configurator(t);
			return t;
		}
		public static T GetParentModel<T>(this TreeListView view, T model) where T : class {
			return view.GetParent(model) as T;
		}

		public static List<T> GetAncestorsOrSelf<T>(this TreeListView view, T model) where T : class {
			var r = new List<T>();
			do {
				r.Add(model);
			} while ((model = view.GetParent(model) as T) != null);
			return r;
		}

		public static void CollapseSelected(this TreeListView view, bool moveToParent = false) {
			var so = view.SelectedObjects;
			if (so.Count == 0) {
				return;
			}
			var collapsed = false;
			foreach (var item in so) {
				if (view.IsExpanded(item)) {
					view.Collapse(item);
					collapsed = true;
				}
			}
			if (moveToParent && collapsed == false) {
				var parent = view.GetParent(so[0]);
				if (parent != null) {
					view.SelectedObject = view.FocusedObject = parent;
				}
			}
		}

		public static void MoveUpSelection(this ObjectListView view) {
			var si = view.GetFirstSelectedIndex();
			if (si < 1) {
				return;
			}
			var so = view.SelectedObjects;
			view.MoveObjects(--si, so);
			view.SelectedObjects = so;
		}

		public static void MoveDownSelection(this ObjectListView view) {
			var ls = view.GetLastItemInDisplayOrder();
			if (ls == null || ls.Selected == true) {
				return;
			}
			var si = view.GetFirstSelectedIndex();
			if (si < 0) {
				return;
			}
			var so = view.SelectedObjects;
			view.MoveObjects(si + 2, so);
			view.SelectedObjects = so;
		}

		public static T GetFirstSelectedModel<T>(this ObjectListView view) where T : class {
			return view.GetModelObject(view.GetFirstSelectedIndex()) as T;
		}

		public static OLVListItem GetFocusedOrFirstSelectedItem(this ObjectListView view) {
			return view.GetItemCount() == 0 ? null
				: (view.FocusedItem
				?? view.SelectedItem
				?? (view.SelectedIndices.Count > 0 ? view.GetItem(view.SelectedIndices[0]) : null)) as OLVListItem;
		}

		// 树视图存在子节点且多选节点时，在 SelectedIndexChanged 事件中，SelectedIndices属性可能返回无内容的集合。
		public static int GetFirstSelectedIndex(this ObjectListView view) {
			var c = view.GetItemCount();
			int i = c;
			foreach (int item in view.SelectedIndices) {
				if (item < i) {
					i = item;
				}
			}
			return i == c ? -1 : i;
		}

		public static int GetLastSelectedIndex(this ObjectListView view) {
			int i = -1;
			foreach (int item in view.SelectedIndices) {
				if (item > i) {
					i = item;
				}
			}
			return i;
		}

		public static List<T> GetSelectedModels<T>(this ObjectListView view) where T : class {
			var s = view.SelectedObjects;
			var r = new List<T>(s.Count);
			foreach (T item in s) {
				if (item != null) {
					r.Add(item);
				}
			}
			return r;
		}

		/// <summary>测试坐标点属于哪个单元格。</summary>
		public static GridTestResult GetGridAt(this ObjectListView view, int x, int y) {
			var cr = view.ContentRectangle;
			var ic = view.GetItemCount();
			var ob = false;
			if (x < cr.Left) {
				x = cr.Left;
				ob = true;
			}
			else if (x >= cr.Right) {
				x = cr.Right - 1;
				ob = true;
			}
			var cb = cr.Top + ic * view.RowHeightEffective;
			if (y < cr.Top) {
				y = cr.Top;
				ob = true;
			}
			else if (y >= cb) {
				y = cb;
				ob = true;
			}
			var r = view.GetItemAt(x, y, out OLVColumn c);
			if (r != null) {
				return new GridTestResult(c.DisplayIndex, r.Index, ob);
			}
			// 当列表框滚动时，上述方法失效，使用此替补方法
			r = view.GetNthItemInDisplayOrder((y - 1 - cr.Top) / view.RowHeightEffective);
			var w = cr.Left;
			var cl = view.ColumnsInDisplayOrder;
			for (int i = 0; i < cl.Count; i++) {
				if (x >= w && x <= (w += cl[i].Width)) {
					c = cl[i];
					break;
				}
			}
			if (c == null) {
				c = cl[cl.Count - 1];
				ob = true;
			}
			y = r.Index + view.TopItemIndex;
			if (y >= view.GetItemCount()) {
				y = view.GetItemCount() - 1;
			}
			return new GridTestResult(c.DisplayIndex, y, ob);
		}

		public static void InvertSelect(this ObjectListView view) {
			view.Freeze();
			var l = view.GetItemCount();
			for (int i = 0; i < l; i++) {
				var oi = view.GetItem(i);
				oi.Selected = !oi.Selected;
			}
			view.Unfreeze();
		}

	}

	struct GridTestResult(int columnIndex, int rowIndex, bool isOutOfRange)
	{
		public int ColumnIndex { get; } = columnIndex;
		public int RowIndex { get; } = rowIndex;
		public bool IsOutOfRange { get; } = isOutOfRange;
	}
}
