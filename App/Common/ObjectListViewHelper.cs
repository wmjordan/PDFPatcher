using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace BrightIdeasSoftware;

internal static class ObjectListViewHelper
{
	/// <summary>修复编辑控件太窄小的问题。</summary>
	public static void FixEditControlWidth(this ObjectListView view) {
		view.CellEditStarting += View_CellEditStarting;
		view.Disposed += View_Disposed;
	}

	private static void View_Disposed(object sender, EventArgs e) {
		ObjectListView view = (ObjectListView)sender;
		view.CellEditStarting -= View_CellEditStarting;
		view.Disposed -= View_Disposed;
	}

	private static void View_CellEditStarting(object sender, CellEditEventArgs e) {
		Rectangle b = e.CellBounds;
		if (b.Width < 60) {
			b.Width = 60;
		}

		if (e.Control is not Control c) {
			return;
		}

		c.Bounds = b;
		c.Location = b.Location;
	}

	public static void SetTreeViewLine(this TreeListView view) {
		TreeListView.TreeRenderer tcr = view.TreeColumnRenderer;
		tcr.LinePen = new Pen(SystemColors.ControlDark) { DashCap = DashCap.Round, DashStyle = DashStyle.Dash };
	}

	public static void ExpandSelected(this TreeListView view) {
		IList so = view.SelectedObjects;
		foreach (object item in so) {
			view.Expand(item);
		}
	}

	public static TypedObjectListView<T> AsTyped<T>(this ObjectListView view) where T : class {
		return view.AsTyped<T>(null);
	}

	public static TypedObjectListView<T> AsTyped<T>(this ObjectListView view,
		Action<TypedObjectListView<T>> configurator) where T : class {
		TypedObjectListView<T> v = new(view);
		configurator?.Invoke(v);
		return v;
	}

	public static TypedObjectListView<T> ConfigColumn<T>(this TypedObjectListView<T> view, OLVColumn column,
		Action<TypedColumn<T>> configurator) where T : class {
		TypedColumn<T> t = new(column);
		configurator(t);
		return view;
	}

	public static TypedColumn<T> AsTyped<T>(this OLVColumn column, Action<TypedColumn<T>> configurator)
		where T : class {
		TypedColumn<T> t = new(column);
		configurator(t);
		return t;
	}

	public static T GetParentModel<T>(this TreeListView view, T model) where T : class {
		return view.GetParent(model) as T;
	}

	public static List<T> GetAncestorsOrSelf<T>(this TreeListView view, T model) where T : class {
		List<T> r = new();
		do {
			r.Add(model);
		} while ((model = view.GetParent(model) as T) != null);

		return r;
	}

	public static void CollapseSelected(this TreeListView view) {
		IList so = view.SelectedObjects;
		foreach (object item in so) {
			view.Collapse(item);
		}
	}

	public static void MoveUpSelection(this ObjectListView view) {
		int si = view.GetFirstSelectedIndex();
		if (si < 1) {
			return;
		}

		IList so = view.SelectedObjects;
		view.MoveObjects(--si, so);
		view.SelectedObjects = so;
	}

	public static void MoveDownSelection(this ObjectListView view) {
		OLVListItem ls = view.GetLastItemInDisplayOrder();
		if (ls == null || ls.Selected) {
			return;
		}

		int si = view.GetFirstSelectedIndex();
		if (si < 0) {
			return;
		}

		IList so = view.SelectedObjects;
		view.MoveObjects(si + 2, so);
		view.SelectedObjects = so;
	}

	public static T GetFirstSelectedModel<T>(this ObjectListView view) where T : class {
		return view.GetModelObject(view.GetFirstSelectedIndex()) as T;
	}

	/// <remarks>树视图存在子节点且多选节点时，在 SelectedIndexChanged 事件中，SelectedIndices属性可能返回无内容的集合。</remarks>
	public static int GetFirstSelectedIndex(this ObjectListView view) {
		int c = view.GetItemCount();
		int i = view.SelectedIndices.Cast<int>().Concat(new[] { c }).Min();

		return i == c ? -1 : i;
	}

	public static int GetLastSelectedIndex(this ObjectListView view) {
		return view.SelectedIndices.Cast<int>().Concat(new[] { -1 }).Max();
	}

	public static List<T> GetSelectedModels<T>(this ObjectListView view) where T : class {
		IList s = view.SelectedObjects;
		List<T> r = new(s.Count);
		r.AddRange(s.Cast<T>().Where(item => item != null));

		return r;
	}

	/// <summary>测试坐标点属于哪个单元格。</summary>
	public static GridTestResult GetGridAt(this ObjectListView view, int x, int y) {
		OLVColumn c;
		Rectangle cr = view.ContentRectangle;
		int ic = view.GetItemCount();
		bool ob = false;
		if (x < cr.Left) {
			x = cr.Left;
			ob = true;
		}
		else if (x >= cr.Right) {
			x = cr.Right - 1;
			ob = true;
		}

		int cb = cr.Top + (ic * view.RowHeightEffective);
		if (y < cr.Top) {
			y = cr.Top;
			ob = true;
		}
		else if (y >= cb) {
			y = cb;
			ob = true;
		}

		OLVListItem r = view.GetItemAt(x, y, out c);
		if (r != null) {
			return new GridTestResult(c.DisplayIndex, r.Index, ob);
		}

		// 当列表框滚动时，上述方法失效，使用此替补方法
		r = view.GetNthItemInDisplayOrder((y - 1 - cr.Top) / view.RowHeightEffective);
		int w = cr.Left;
		List<OLVColumn> cl = view.ColumnsInDisplayOrder;
		foreach (var t in cl.Where(t => x >= w && x <= (w += t.Width))) {
			c = t;
			break;
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
		int l = view.GetItemCount();
		for (int i = 0; i < l; i++) {
			OLVListItem oi = view.GetItem(i);
			oi.Selected = !oi.Selected;
		}

		view.Unfreeze();
	}
}

public struct GridTestResult
{
	public int ColumnIndex { get; }
	public int RowIndex { get; }
	public bool IsOutOfRange { get; }

	public GridTestResult(int columnIndex, int rowIndex, bool isOutOfRange) {
		ColumnIndex = columnIndex;
		RowIndex = rowIndex;
		IsOutOfRange = isOutOfRange;
	}
}