using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions
{
	public partial class BookmarkEditorView : TreeListView
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static List<BookmarkElement> _copiedBookmarks;

		[EditorBrowsable(EditorBrowsableState.Never)]
		internal UndoManager Undo { get; set; }

		public bool OperationAffectsDecendants { get; set; }
		public OLVColumn BookmarkOpenColumn => _BookmarkOpenColumn;
		public OLVColumn BookmarkNameColumn => _BookmarkNameColumn;
		public OLVColumn BookmarkPageColumn => _BookmarkPageColumn;
		public bool HasMarker => _markers.Count > 0;
		public bool IsLabelEditing { get; private set; }

		readonly Dictionary<BookmarkElement, Color> _markers = new Dictionary<BookmarkElement, Color>();

		public BookmarkEditorView() {
			InitializeComponent();
			InitEditerBox();
		}

		private void InitEditerBox() {
			if (IsDesignMode) {
				return;
			}
			UseOverlays = false;
			#region 修复树视图无法正确选择节点的问题
			SmallImageList = new ImageList();
			#endregion
			this.SetTreeViewLine();
			CanExpandGetter = (object x) => x is BookmarkElement e && e.HasSubBookmarks;
			ChildrenGetter = (object x) => ((BookmarkElement)x).SubBookmarks;
			_BookmarkNameColumn.AutoCompleteEditorMode = AutoCompleteMode.Suggest;
			//this.SelectedRowDecoration = new RowBorderDecoration ()
			//{
			//    FillBrush = new SolidBrush (Color.FromArgb (64, SystemColors.Highlight)),
			//    BoundsPadding = new Size (0, 0),
			//    CornerRounding = 2,
			//    BorderPen = new Pen (Color.FromArgb (216, SystemColors.Highlight))
			//};
			new TypedColumn<BookmarkElement>(_BookmarkNameColumn) {
				AspectGetter = (e) => e.Title,
				AspectPutter = (e, newValue) => {
					var s = newValue as string;
					if (e.Title == s) {
						return;
					}
					var p = new ReplaceTitleTextProcessor(s);
					Undo?.AddUndo("编辑书签文本", p.Process(e));
				}
			};
			new TypedColumn<BookmarkElement>(_BookmarkOpenColumn) {
				AspectGetter = (e) => e == null ? false : (object)e.IsOpen,
				AspectPutter = (e, newValue) => {
					if (e == null || e.HasSubBookmarks == false) {
						return;
					}
					var p = new BookmarkOpenStatusProcessor((bool)newValue);
					Undo.AddUndo(p.Name, p.Process(e));
				}
			};
			new TypedColumn<XmlElement>(_BookmarkPageColumn) {
				AspectGetter = (e) => {
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
						var p = new ChangePageNumberProcessor(n, true, false);
						Undo.AddUndo(p.Name, p.Process(e));
					}
				}
			};
			_ActionColumn.AspectGetter = (object x) => {
				var e = x as XmlElement;
				if (e == null) {
					return String.Empty;
				}
				var a = e.GetAttribute(Constants.DestinationAttributes.Action);
				if (String.IsNullOrEmpty(a)) {
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
		protected override void OnCellEditStarting(CellEditEventArgs e) {
			if (e.Column == _BookmarkPageColumn) {
				var b = e.CellBounds;
				if (b.Width < 60) {
					b.Width = 60;
				}
				e.Control = new NumericUpDown {
					Location = b.Location,
					Bounds = b,
					Value = Convert.ToDecimal(e.Value)
				};
			}
			base.OnCellEditStarting(e);
		}
		protected override void OnCellEditFinished(CellEditEventArgs e) {
			if (e.Column == _BookmarkPageColumn) {
				e.NewValue = ((NumericUpDown)e.Control).Value;
			}
			base.OnCellEditFinished(e);
		}
		#region 拖放操作
		protected override void OnCanDrop(OlvDropEventArgs args) {
			var o = args.DataObject as DataObject;
			if (o == null) {
				return;
			}
			var f = o.GetFileDropList();
			foreach (var item in f) {
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
			var si = e.SourceModels;
			var ti = e.TargetModel as XmlElement;
			if (si == null || si.Count == 0 || e.TargetModel == null) {
				e.Effect = DragDropEffects.None;
				return;
			}
			var copy = (Control.ModifierKeys & Keys.Control) != Keys.None || (e.SourceModels[0] as XmlElement).OwnerDocument != ti.OwnerDocument;
			if (copy == false) {
				if (e.DropTargetItem.Selected) {
					e.Effect = DragDropEffects.None;
					return;
				}
				foreach (XmlElement item in si) {
					if (IsAncestorOrSelf(item, ti)) {
						e.Effect = DragDropEffects.None;
						e.InfoMessage = "目标书签不能是源书签的子书签。";
						return;
					}
				}
			}
			var d = e.DropTargetItem;
			var ml = e.MouseLocation;
			var child = ml.X > d.Position.X + d.GetBounds(ItemBoundsPortion.ItemOnly).Width / 2;
			var append = ml.Y > d.Position.Y + d.Bounds.Height / 2;
			if (child == false && copy == false) {
				var xi = e.DropTargetIndex + (append ? 1 : -1);
				if (xi > -1 && xi < e.ListView.Items.Count
					&& e.ListView.Items[xi].Selected
					&& ti.ParentNode == (e.ListView.GetModelObject(xi) as XmlElement).ParentNode) {
					e.Effect = DragDropEffects.None;
					return;
				}
			}
			e.Effect = copy ? DragDropEffects.Copy : DragDropEffects.Move;
			e.InfoMessage = String.Concat((copy ? "复制" : "移动"), "到", (child ? "所有子书签" : String.Empty), (append ? "后面" : "前面"));
			base.OnModelCanDrop(e);
		}

		protected override void OnModelDropped(ModelDropEventArgs args) {
			base.OnModelDropped(args);
			var t = args.TargetModel as BookmarkElement;
			var se = GetSelectedElements(args.SourceListView as BrightIdeasSoftware.TreeListView, false);
			if (se == null) {
				return;
			}
			var ti = args.TargetModel as BookmarkElement;
			var d = args.DropTargetItem;
			var ml = args.MouseLocation;
			Freeze();
			var child = ml.X > d.Position.X + d.GetBounds(ItemBoundsPortion.ItemOnly).Width / 2;
			var append = ml.Y > d.Position.Y + d.Bounds.Height / 2;
			var copy = (Control.ModifierKeys & Keys.Control) != Keys.None || (args.SourceModels[0] as BookmarkElement).OwnerDocument != ti.OwnerDocument;
			var deepCopy = copy && (OperationAffectsDecendants || (Control.ModifierKeys & Keys.Shift) != Keys.None);
			var tii = TopItemIndex;
			CopyOrMoveElement(se, ti, child, append, copy, deepCopy);
			//e.RefreshObjects ();
			TopItemIndex = tii;
			Unfreeze();
			args.Handled = true;
		}
		#endregion

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

		void Mark(XmlNodeList bookmarks) {
			foreach (BookmarkElement item in bookmarks) {
				if (item == null || item.MarkerColor == 0) {
					continue;
				}
				_markers.Add(item, Color.FromArgb(item.MarkerColor));
				Mark(item.ChildNodes);
			}
		}

		/// <summary>
		/// 复制或移动书签。
		/// </summary>
		/// <param name="source">需要复制或移动的源书签。</param>
		/// <param name="target">目标书签。</param>
		/// <param name="child">是否复制为子节点。</param>
		/// <param name="after">是否复制到后面。</param>
		/// <param name="copy">是否复制书签。</param>
		/// <param name="deepCopy">是否深度复制书签。</param>
		internal void CopyOrMoveElement(List<BookmarkElement> source, XmlElement target, bool child, bool after, bool copy, bool deepCopy) {
			var undo = new UndoActionGroup();
			bool spr = false; // source parent is root
			bool tpr = false; // target parent is root
			var pn = new List<XmlNode>();
			if (copy) {
				var clones = new List<BookmarkElement>(source.Count);
				var td = target.OwnerDocument;
				foreach (XmlElement item in source) {
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
				foreach (var item in source) {
					var e = item.ParentNode as XmlElement;
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
					foreach (XmlElement item in source) {
						if (!copy) {
							undo.Add(new AddElementAction(item));
						}
						target.AppendChild(item);
						undo.Add(new RemoveElementAction(item));
					}
				}
				else {
					source.Reverse();
					foreach (XmlElement item in source) {
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
				var p = target.ParentNode;
				if (after) {
					tpr = p.Name == Constants.DocumentBookmark;
					source.Reverse();
					foreach (XmlElement item in source) {
						if (!copy) {
							undo.Add(new AddElementAction(item));
						}
						p.InsertAfter(item, target);
						undo.Add(new RemoveElementAction(item));
					}
				}
				else {
					foreach (XmlElement item in source) {
						if (!copy) {
							undo.Add(new AddElementAction(item));
						}
						p.InsertBefore(item, target);
						undo.Add(new RemoveElementAction(item));
					}
				}
			}
			Undo?.AddUndo(copy ? "复制书签" : "移动书签", undo);
			if (copy == false && spr || tpr) {
				Roots = (target.OwnerDocument as PdfInfoXmlDocument).BookmarkRoot.SubBookmarks;
			}
			if (pn != null) {
				RefreshObjects(pn);
			}
			RefreshObject(target);
			SelectedObjects = source;
		}

		/// <summary>
		/// 检查 <paramref name="source"/> 是否为 <paramref name="target"/> 的先代元素。
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
			foreach (var item in items) {
				_markers[item] = color;
				item.MarkerColor = color.ToArgb();
			}
			RefreshObjects(items);
		}
		internal List<BookmarkElement> SelectMarkedItems(Color color) {
			Freeze();
			var items = new List<BookmarkElement>();
			var c = color.ToArgb();
			var r = new List<BookmarkElement>();
			foreach (var item in _markers) {
				if (item.Value.ToArgb() == c) {
					var k = item.Key;
					Debug.Assert((k.ParentNode == null || k.OwnerDocument == null) == false);
					if (k.ParentNode == null || k.OwnerDocument == null) {
						r.Add(k);
						continue;
					}
					items.Add(k);
					MakeItemVisible(k);
				}
			}
			foreach (var item in r) {
				_markers.Remove(item);
			}
			SelectObjects(items);
			EnsureItemsVisible(items);
			Unfreeze();
			return items;
		}
		internal void UnmarkItems(List<BookmarkElement> items) {
			foreach (var item in items) {
				_markers.Remove(item);
				item.MarkerColor = 0;
			}
			RefreshObjects(items);
		}
		internal void ClearMarks(bool refresh) {
			if (refresh) {
				var items = new List<XmlElement>(_markers.Count);
				foreach (var item in _markers) {
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
			var p = item.ParentNode;
			var a = new Stack<XmlNode>(); //ancestorsToExpand
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
			var cr = ClientRectangle;
			OLVListItem fi = null, li = null;
			foreach (var item in items) {
				var i = ModelToItem(item);
				if (i != null) {
					var r = GetItemRect(i.Index);
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
				var t = Clipboard.GetText();
				bool c = false;
				if (t.IsNullOrWhiteSpace() == false) {
					var doc = new PdfInfoXmlDocument();
					using (var s = new System.IO.StringReader(t)) {
						OutlineManager.ImportSimpleBookmarks(s, doc);
					}
					_copiedBookmarks = doc.Bookmarks.ToNodeList<BookmarkElement>() as List<BookmarkElement>;
					c = true;
				}
				if (_copiedBookmarks == null || _copiedBookmarks.Count == 0) {
					return;
				}
				CopyOrMoveElement(_copiedBookmarks, target, asChild, true, true, c || OperationAffectsDecendants);
			}
			catch (Exception ex) {
				// ignore
			}
		}

		internal List<BookmarkElement> GetSelectedElements() { return GetSelectedElements(this, true); }
		internal List<BookmarkElement> GetSelectedElements(bool selectChildren) { return GetSelectedElements(this, selectChildren); }
		private static List<BookmarkElement> GetSelectedElements(TreeListView treeList, bool selectChildren) {
			if (treeList == null) {
				return null;
			}
			var si = treeList.SelectedIndices;
			var il = new int[si.Count];
			si.CopyTo(il, 0);
			Array.Sort(il);
			var el = new List<BookmarkElement>();
			var l = -1;
			BookmarkElement e;
			foreach (var item in il) {
				e = treeList.GetModelObject(item) as BookmarkElement;
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
			var o = GetModelObject(e.Item) as XmlElement;
			if (o == null || String.IsNullOrEmpty(e.Label)) {
				e.CancelEdit = true;
				return;
			}
			var p = new ReplaceTitleTextProcessor(e.Label);
			Undo?.AddUndo("编辑书签文本", p.Process(o));
			var i = GetItem(e.Item);
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
			using (var form = new ActionEditorForm(bookmark)) {
				if (form.ShowDialog() == DialogResult.OK && form.UndoActions.Count > 0) {
					Undo?.AddUndo("更改书签动作属性", form.UndoActions);
					RefreshObject(bookmark);
				}
			}
		}

		void BookmarkEditor_HotItemChanged(object sender, BrightIdeasSoftware.HotItemChangedEventArgs e) {
			if ((e.HotColumnIndex == _ActionColumn.Index || e.OldHotColumnIndex == _ActionColumn.Index)
				//&& (e.HotRowIndex != e.OldHotRowIndex || e.HotColumnIndex != e.OldHotColumnIndex)
				) {
				// e.handled = false;
				return;
			}
			e.Handled = true;
		}

		private void _BookmarkBox_FormatRow(object sender, FormatRowEventArgs e) {
			var b = e.Model as BookmarkElement;
			if (b == null) {
				return;
			}
			e.Item.UseItemStyleForSubItems = false;
			e.UseCellFormatEvents = false;
			Color c;
			if (b.MarkerColor != 0) {
				e.Item.BackColor = Color.FromArgb(b.MarkerColor);
			}
			c = b.ForeColor;
			if (c != Color.Transparent) {
				e.Item.ForeColor = c;
			}
			var ts = b.TextStyle;
			if (ts != FontStyle.Regular) {
				e.Item.Font = new Font(e.Item.Font, ts);
			}
			if (_ActionColumn.Index != -1) {
				e.Item.SubItems[_ActionColumn.Index].ForeColor = Color.Blue;
			}
		}

		internal BookmarkElement SearchBookmark(BookmarkMatcher matcher) {
			var s = this.GetFirstSelectedModel<BookmarkElement>();
			if (s == null) {
				s = GetModelObject(0) as BookmarkElement;
				if (s == null) {
					return null;
				}
			}
			var n = s.CreateNavigator();
			BookmarkElement e;
			while (n.MoveToFollowing(Constants.Bookmark, String.Empty)) {
				e = n.UnderlyingObject as BookmarkElement;
				if (e != null && matcher.Match(e)) {
					MakeItemVisible(e);
					EnsureModelVisible(e);
					SelectedObject = e;
					return e;
				}
			}
			return null;
		}

		internal List<BookmarkElement> SearchBookmarks(BookmarkMatcher matcher) {
			var matches = new List<BookmarkElement>();
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

		private void SearchBookmarks(BookmarkMatcher matcher, List<BookmarkElement> matches, BookmarkElement item) {
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


	}
}
