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