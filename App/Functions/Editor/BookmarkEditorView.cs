using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions;

public partial class BookmarkEditorView : TreeListView
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<BookmarkElement> _copiedBookmarks;

	private readonly Dictionary<BookmarkElement, Color> _markers = new();

	public BookmarkEditorView() {
		InitializeComponent();
		InitEditorBox();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal UndoManager Undo { get; set; }

	public bool OperationAffectsDescendants { get; set; }
	public OLVColumn BookmarkOpenColumn { get; private set; }

	public OLVColumn BookmarkNameColumn { get; private set; }

	public OLVColumn BookmarkPageColumn { get; private set; }

	public bool HasMarker => _markers.Count > 0;
	public bool IsLabelEditing { get; private set; }

	private void InitEditorBox() {
		if (IsDesignMode) {
			return;
		}

		UseOverlays = false;

		#region 修复树视图无法正确选择节点的问题

		SmallImageList = new ImageList();

		#endregion

		this.SetTreeViewLine();
		this.FixEditControlWidth();
		CanExpandGetter = x => x is BookmarkElement e && e.HasSubBookmarks;
		ChildrenGetter = x => ((BookmarkElement)x).SubBookmarks;
		BookmarkNameColumn.AutoCompleteEditorMode = AutoCompleteMode.Suggest;
		//this.SelectedRowDecoration = new RowBorderDecoration ()
		//{
		//    FillBrush = new SolidBrush (Color.FromArgb (64, SystemColors.Highlight)),
		//    BoundsPadding = new Size (0, 0),
		//    CornerRounding = 2,
		//    BorderPen = new Pen (Color.FromArgb (216, SystemColors.Highlight))
		//};
		new TypedColumn<BookmarkElement>(BookmarkNameColumn) {
			AspectGetter = e => e.Title,
			AspectPutter = (e, newValue) => {
				string s = newValue as string;
				if (e.Title == s) {
					return;
				}

				ReplaceTitleTextProcessor p = new(s);
				Undo?.AddUndo("编辑书签文本", p.Process(e));
			}
		};
		new TypedColumn<BookmarkElement>(BookmarkOpenColumn) {
			AspectGetter = e => e == null ? false : (object)e.IsOpen,
			AspectPutter = (e, newValue) => {
				if (e == null || e.HasSubBookmarks == false) {
					return;
				}

				BookmarkOpenStatusProcessor p = new((bool)newValue);
				Undo.AddUndo(p.Name, p.Process(e));
			}
		};
		new TypedColumn<XmlElement>(BookmarkPageColumn) {
			AspectGetter = e => {
				if (e == null) {
					return 0;
				}

				int p = e.GetValue(Constants.DestinationAttributes.Page, 0);
				if (e.HasAttribute(Constants.DestinationAttributes.FirstPageNumber)) {
					int o = e.GetValue(Constants.DestinationAttributes.FirstPageNumber, 0);
					if (o > 0) {
						p += o;
						e.RemoveAttribute(Constants.DestinationAttributes.FirstPageNumber);
					}
				}

				return p;
			},
			AspectPutter = (e, value) => {
				if (e == null) {
					return;
				}

				if (value.ToString().TryParse(out int n)) {
					ChangePageNumberProcessor p = new(n, true, false);
					Undo.AddUndo(p.Name, p.Process(e));
				}
			}
		};
		_ActionColumn.AspectGetter = x => {
			if (x is not XmlElement e) {
				return string.Empty;
			}

			string a = e.GetAttribute(Constants.DestinationAttributes.Action);
			if (string.IsNullOrEmpty(a)) {
				return e.HasAttribute(Constants.DestinationAttributes.Page) ? Constants.ActionType.Goto : "无";
			}

			return a;
		};
	}

	protected override void OnBeforeSorting(BeforeSortingEventArgs e) {
		e.Canceled = true; // 禁止排序
		base.OnBeforeSorting(e);
	}

	protected override void OnItemActivate(EventArgs e) {
		base.OnItemActivate(e);
		EditSubItem(SelectedItem, 0);
	}

	internal void LoadBookmarks(XmlNodeList bookmarks) {
		Roots = bookmarks.ToXmlNodeArray();
		foreach (BookmarkElement item in Roots) {
			if (item?.IsOpen == true) {
				Expand(item);
			}
		}

		_markers.Clear();
		Mark(bookmarks);
	}

	private void Mark(XmlNodeList bookmarks) {
		foreach (BookmarkElement item in bookmarks) {
			if (item == null || item.MarkerColor == 0) {
				continue;
			}

			_markers.Add(item, Color.FromArgb(item.MarkerColor));
			Mark(item.ChildNodes);
		}
	}

	/// <summary>
	///     复制或移动书签。
	/// </summary>
	/// <param name="source">需要复制或移动的源书签。</param>
	/// <param name="target">目标书签。</param>
	/// <param name="child">是否复制为子节点。</param>
	/// <param name="after">是否复制到后面。</param>
	/// <param name="copy">是否复制书签。</param>
	/// <param name="deepCopy">是否深度复制书签。</param>
	internal void CopyOrMoveElement(List<BookmarkElement> source, XmlElement target, bool child, bool after, bool copy,
		bool deepCopy) {
		UndoActionGroup undo = new();
		bool spr = false; // source parent is root
		bool tpr = false; // target parent is root
		List<XmlNode> pn = new();
		if (copy) {
			List<BookmarkElement> clones = new(source.Count);
			XmlDocument td = target.OwnerDocument;
			foreach (BookmarkElement item in source) {
				if (item.OwnerDocument == td) {
					clones.Add((BookmarkElement)item.CloneNode(deepCopy));
				}
				else {
					clones.Add(td.ImportNode(item, deepCopy) as BookmarkElement);
				}
			}

			source = clones;
		}
		else {
			foreach (BookmarkElement item in source) {
				XmlElement e = item.ParentNode as XmlElement;
				if (e.Name == Constants.DocumentBookmark) {
					spr = true;
					pn = null;
					break;
				}

				pn.Add(e);
			}
		}

		//else {
		//	foreach (var item in source) {
		//		this.Collapse (item);
		//	}
		//	this.RemoveObjects (source);
		//}
		if (child) {
			if (after) {
				tpr = target.Name == Constants.DocumentBookmark;
				foreach (BookmarkElement item in source) {
					if (!copy) {
						undo.Add(new AddElementAction(item));
					}

					target.AppendChild(item);
					undo.Add(new RemoveElementAction(item));
				}
			}
			else {
				source.Reverse();
				foreach (BookmarkElement item in source) {
					if (!copy) {
						undo.Add(new AddElementAction(item));
					}

					target.PrependChild(item);
					undo.Add(new RemoveElementAction(item));
				}
			}

			Expand(target);
		}
		else {
			XmlNode p = target.ParentNode;
			if (after) {
				tpr = p.Name == Constants.DocumentBookmark;
				source.Reverse();
				foreach (BookmarkElement item in source) {
					if (!copy) {
						undo.Add(new AddElementAction(item));
					}

					p.InsertAfter(item, target);
					undo.Add(new RemoveElementAction(item));
				}
			}
			else {
				foreach (BookmarkElement item in source) {
					if (!copy) {
						undo.Add(new AddElementAction(item));
					}

					p.InsertBefore(item, target);
					undo.Add(new RemoveElementAction(item));
				}
			}
		}

		Undo?.AddUndo(copy ? "复制书签" : "移动书签", undo);
		if ((copy == false && spr) || tpr) {
			Roots = (target.OwnerDocument as PdfInfoXmlDocument).BookmarkRoot.SubBookmarks;
		}

		if (pn != null) {
			RefreshObjects(pn);
		}

		RefreshObject(target);
		SelectedObjects = source;
	}

	/// <summary>
	///     检查 <paramref name="source" /> 是否为 <paramref name="target" /> 的先代元素。
	/// </summary>
	private static bool IsAncestorOrSelf(XmlElement source, XmlElement target) {
		do {
			if (source == target) {
				return true;
			}
		} while ((target = target.ParentNode as XmlElement) != null);

		return false;
	}

	internal void MarkItems(List<BookmarkElement> items, Color color) {
		foreach (BookmarkElement item in items) {
			_markers[item] = color;
			item.MarkerColor = color.ToArgb();
		}

		RefreshObjects(items);
	}

	internal List<BookmarkElement> SelectMarkedItems(Color color) {
		Freeze();
		List<BookmarkElement> items = new();
		int c = color.ToArgb();
		List<BookmarkElement> r = new();
		foreach (KeyValuePair<BookmarkElement, Color> item in _markers) {
			if (item.Value.ToArgb() == c) {
				BookmarkElement k = item.Key;
				Debug.Assert((k.ParentNode == null || k.OwnerDocument == null) == false);
				if (k.ParentNode == null || k.OwnerDocument == null) {
					r.Add(k);
					continue;
				}

				items.Add(k);
				MakeItemVisible(k);
			}
		}

		foreach (BookmarkElement item in r) {
			_markers.Remove(item);
		}

		SelectObjects(items);
		EnsureItemsVisible(items);
		Unfreeze();
		return items;
	}

	internal void UnmarkItems(List<BookmarkElement> items) {
		foreach (BookmarkElement item in items) {
			_markers.Remove(item);
			item.MarkerColor = 0;
		}

		RefreshObjects(items);
	}

	internal void ClearMarks(bool refresh) {
		if (refresh) {
			List<XmlElement> items = new(_markers.Count);
			foreach (KeyValuePair<BookmarkElement, Color> item in _markers) {
				items.Add(item.Key);
				item.Key.MarkerColor = 0;
			}

			_markers.Clear();
			RefreshObjects(items);
		}
		else {
			_markers.Clear();
		}
	}

	internal void MakeItemVisible(XmlElement item) {
		XmlNode p = item.ParentNode;
		Stack<XmlNode> a = new(); //ancestorsToExpand
		a.Push(null);
		a.Push(p);
		while (p.Name != Constants.DocumentBookmark) {
			p = p.ParentNode;
			a.Push(p);
		}

		while (a.Peek() != null) {
			Expand(a.Pop());
		}
	}

	internal void EnsureItemsVisible(ICollection<BookmarkElement> items) {
		if (items.Count == 0) {
			return;
		}

		Rectangle cr = ClientRectangle;
		OLVListItem fi = null, li = null;
		foreach (BookmarkElement item in items) {
			OLVListItem i = ModelToItem(item);
			if (i != null) {
				Rectangle r = GetItemRect(i.Index);
				if (r.Top >= cr.Top && r.Bottom <= cr.Bottom) {
					return;
				}

				li = i;
				if (fi == null) {
					fi = i;
				}
			}
		}

		if ((fi ?? li) != null) {
			EnsureVisible(fi.Index);
		}
	}

	internal void CopySelectedBookmark() {
		_copiedBookmarks = GetSelectedElements(false);
		Clipboard.Clear();
	}

	internal void PasteBookmarks(XmlElement target, bool asChild) {
		try {
			string t = Clipboard.GetText();
			bool c = false;
			if (t.IsNullOrWhiteSpace() == false) {
				PdfInfoXmlDocument doc = new();
				using (StringReader s = new(t)) {
					OutlineManager.ImportSimpleBookmarks(s, doc);
				}

				_copiedBookmarks = doc.Bookmarks.ToNodeList<BookmarkElement>() as List<BookmarkElement>;
				c = true;
			}

			if (_copiedBookmarks == null || _copiedBookmarks.Count == 0) {
				return;
			}

			CopyOrMoveElement(_copiedBookmarks, target, asChild, true, true, c || OperationAffectsDescendants);
		}
		catch (Exception) {
			// ignore
		}
	}

	internal List<BookmarkElement> GetSelectedElements() { return GetSelectedElements(this, true); }

	internal List<BookmarkElement> GetSelectedElements(bool selectChildren) {
		return GetSelectedElements(this, selectChildren);
	}

	private static List<BookmarkElement> GetSelectedElements(VirtualObjectListView treeList, bool selectChildren) {
		if (treeList == null) {
			return null;
		}

		SelectedIndexCollection si = treeList.SelectedIndices;
		int[] il = new int[si.Count];
		si.CopyTo(il, 0);
		Array.Sort(il);
		List<BookmarkElement> el = new();
		int l = -1;
		foreach (int item in il) {
			BookmarkElement e = treeList.GetModelObject(item) as BookmarkElement;
			if (selectChildren) {
				el.Add(e);
			}
			else if (item > l) {
				l = item + (treeList.VirtualListDataSource as Tree).GetVisibleDescendentCount(e);
				el.Add(e);
			}
		}

		return el;
	}

	private void BookmarkEditorView_BeforeLabelEdit(object sender, LabelEditEventArgs e) {
		IsLabelEditing = true;
	}

	private void _BookmarkBox_AfterLabelEdit(object sender, LabelEditEventArgs e) {
		IsLabelEditing = false;
		if (GetModelObject(e.Item) is not XmlElement o || string.IsNullOrEmpty(e.Label)) {
			e.CancelEdit = true;
			return;
		}

		ReplaceTitleTextProcessor p = new(e.Label);
		Undo?.AddUndo("编辑书签文本", p.Process(o));
		OLVListItem i = GetItem(e.Item);
		if (o.HasChildNodes && FormHelper.IsCtrlKeyDown == false) {
			Expand(o);
		}

		if (i.Index < Items.Count - 1) {
			GetItem(i.Index + 1).BeginEdit();
		}

		RefreshItem(i);
	}

	private void BookmarkEditor_CellClick(object sender, HyperlinkClickedEventArgs e) {
		if (e.Column != _ActionColumn) {
			return;
		}

		e.Handled = true;
		ShowBookmarkProperties(e.Model as BookmarkElement);
	}

	public void ShowBookmarkProperties(BookmarkElement bookmark) {
		if (bookmark == null) {
			return;
		}

		using (ActionEditorForm form = new(bookmark)) {
			if (form.ShowDialog() == DialogResult.OK && form.UndoActions.Count > 0) {
				Undo?.AddUndo("更改书签动作属性", form.UndoActions);
				RefreshObject(bookmark);
			}
		}
	}

	private void BookmarkEditor_HotItemChanged(object sender, HotItemChangedEventArgs e) {
		if (e.HotColumnIndex == _ActionColumn.Index || e.OldHotColumnIndex == _ActionColumn.Index
		   //&& (e.HotRowIndex != e.OldHotRowIndex || e.HotColumnIndex != e.OldHotColumnIndex)
		   ) {
			// e.handled = false;
			return;
		}

		e.Handled = true;
	}

	private void _BookmarkBox_FormatRow(object sender, FormatRowEventArgs e) {
		if (e.Model is not BookmarkElement b) {
			return;
		}

		e.Item.UseItemStyleForSubItems = false;
		e.UseCellFormatEvents = false;
		if (b.MarkerColor != 0) {
			e.Item.BackColor = Color.FromArgb(b.MarkerColor);
		}

		Color c = b.ForeColor;
		if (c != Color.Transparent) {
			e.Item.ForeColor = c;
		}

		FontStyle ts = b.TextStyle;
		if (ts != FontStyle.Regular) {
			e.Item.Font = new Font(e.Item.Font, ts);
		}

		if (_ActionColumn.Index != -1) {
			e.Item.SubItems[_ActionColumn.Index].ForeColor = Color.Blue;
		}
	}

	internal BookmarkElement SearchBookmark(BookmarkMatcher matcher) {
		BookmarkElement s = this.GetFirstSelectedModel<BookmarkElement>();
		if (s == null) {
			s = GetModelObject(0) as BookmarkElement;
			if (s == null) {
				return null;
			}
		}

		XPathNavigator n = s.CreateNavigator();
		while (n.MoveToFollowing(Constants.Bookmark, string.Empty)) {
			if (n.UnderlyingObject is BookmarkElement e && matcher.Match(e)) {
				MakeItemVisible(e);
				EnsureModelVisible(e);
				SelectedObject = e;
				return e;
			}
		}

		return null;
	}

	internal List<BookmarkElement> SearchBookmarks(BookmarkMatcher matcher) {
		List<BookmarkElement> matches = new();
		Freeze();
		try {
			foreach (BookmarkElement item in Roots) {
				SearchBookmarks(matcher, matches, item);
			}
		}
		catch (Exception ex) {
			FormHelper.ErrorBox("在匹配文本时出现错误：" + ex.Message);
		}

		Unfreeze();
		if (matches.Count > 0) {
			EnsureItemsVisible(matches);
			SelectedObjects = matches;
		}

		return matches;
	}

	private void SearchBookmarks(BookmarkMatcher matcher, ICollection<BookmarkElement> matches, BookmarkElement item) {
		if (item.HasChildNodes) {
			foreach (BookmarkElement c in item.SelectNodes(Constants.Bookmark)) {
				SearchBookmarks(matcher, matches, c);
			}
		}

		if (matcher.Match(item)) {
			matches.Add(item);
			MakeItemVisible(item);
		}
	}

	#region 拖放操作

	protected override void OnCanDrop(OlvDropEventArgs args) {
		if (args.DataObject is not DataObject o) {
			return;
		}

		StringCollection f = o.GetFileDropList();
		foreach (string item in f) {
			if (FileHelper.HasExtension(item, Constants.FileExtensions.Xml)
				|| FileHelper.HasExtension(item, Constants.FileExtensions.Pdf)) {
				args.Handled = true;
				args.DropTargetLocation = DropTargetLocation.Background;
				args.Effect = DragDropEffects.Copy;
				args.InfoMessage = "打开文件" + item;
				return;
			}
		}

		base.OnCanDrop(args);
	}

	protected override void OnModelCanDrop(ModelDropEventArgs e) {
		IList si = e.SourceModels;
		XmlElement ti = e.TargetModel as XmlElement;
		if (si == null || si.Count == 0 || e.TargetModel == null) {
			e.Effect = DragDropEffects.None;
			return;
		}

		bool copy = (ModifierKeys & Keys.Control) != Keys.None ||
					(e.SourceModels[0] as XmlElement).OwnerDocument != ti.OwnerDocument;
		if (copy == false) {
			if (e.DropTargetItem.Selected) {
				e.Effect = DragDropEffects.None;
				return;
			}

			if (si.Cast<XmlElement>().Any(item => IsAncestorOrSelf(item, ti))) {
				e.Effect = DragDropEffects.None;
				e.InfoMessage = "目标书签不能是源书签的子书签。";
				return;
			}
		}

		OLVListItem d = e.DropTargetItem;
		Point ml = e.MouseLocation;
		bool child = ml.X > d.Position.X + (d.GetBounds(ItemBoundsPortion.ItemOnly).Width / 2);
		bool append = ml.Y > d.Position.Y + (d.Bounds.Height / 2);
		if (child == false && copy == false) {
			int xi = e.DropTargetIndex + (append ? 1 : -1);
			if (xi > -1 && xi < e.ListView.Items.Count
						&& e.ListView.Items[xi].Selected
						&& ti.ParentNode == (e.ListView.GetModelObject(xi) as XmlElement).ParentNode) {
				e.Effect = DragDropEffects.None;
				return;
			}
		}

		e.Effect = copy ? DragDropEffects.Copy : DragDropEffects.Move;
		e.InfoMessage = string.Concat(copy ? "复制" : "移动", "到", child ? "所有子书签" : string.Empty, append ? "后面" : "前面");
		base.OnModelCanDrop(e);
	}

	protected override void OnModelDropped(ModelDropEventArgs args) {
		base.OnModelDropped(args);
		BookmarkElement t = args.TargetModel as BookmarkElement;
		List<BookmarkElement> se = GetSelectedElements(args.SourceListView as TreeListView, false);
		if (se == null) {
			return;
		}

		BookmarkElement ti = args.TargetModel as BookmarkElement;
		OLVListItem d = args.DropTargetItem;
		Point ml = args.MouseLocation;
		Freeze();
		bool child = ml.X > d.Position.X + (d.GetBounds(ItemBoundsPortion.ItemOnly).Width / 2);
		bool append = ml.Y > d.Position.Y + (d.Bounds.Height / 2);
		bool copy = (ModifierKeys & Keys.Control) != Keys.None ||
					(args.SourceModels[0] as BookmarkElement).OwnerDocument != ti.OwnerDocument;
		bool deepCopy = copy && (OperationAffectsDescendants || (ModifierKeys & Keys.Shift) != Keys.None);
		int tii = TopItemIndex;
		CopyOrMoveElement(se, ti, child, append, copy, deepCopy);
		//e.RefreshObjects ();
		TopItemIndex = tii;
		Unfreeze();
		args.Handled = true;
	}

	#endregion
}