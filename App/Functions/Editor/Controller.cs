using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor
{
	sealed class Controller
	{
		public EditModel Model { get; }
		public IEditView View { get; }

		BackgroundWorker _loader;

		public Controller(IEditView view) {
			Model = new EditModel();
			View = view;
			Model.Undo.OnAddUndo += (UndoManager u, IUndoAction a) => View.UndoButton.Enabled = true;
			View.Bookmark.Undo = Model.Undo;
		}
		public bool IsBusy => _loader?.IsBusy == true;

		internal IEnumerable<XmlNode> ProcessBookmarks(IPdfInfoXmlProcessor processor) {
			return ProcessBookmarks(View.AffectsDescendantBookmarks, true, processor);
		}
		/// <summary>
		/// 逐个处理选中的书签。
		/// </summary>
		/// <param name="includeDescendant">处理操作是否包含选中书签的内层书签。</param>
		/// <param name="selectChildren">处理时是否遍历选中的内层书签。</param>
		/// <param name="processor">用于处理书签的 <see cref="IPdfInfoXmlProcessor"/>。</param>
		/// <returns>处理后的书签。</returns>
		internal IEnumerable<XmlNode> ProcessBookmarks(bool includeDescendant, bool selectChildren, IPdfInfoXmlProcessor processor) {
			var b = View.Bookmark;
			b.Freeze();
			var si = b.GetSelectedElements(selectChildren);
			var pi = new HashSet<XmlElement>();
			var r = ProcessBookmarks(si, pi, includeDescendant, processor);
			if (r != null) {
				foreach (var item in r) {
					var i = item as XmlElement;
					if (i.ParentNode.Name == Constants.DocumentBookmark) {
						b.Roots = i.ParentNode.SelectNodes(Constants.Bookmark).ToXmlNodeArray();
						break;
					}
				}
				b.RefreshObjects(r.ToArray());
			}
			b.Unfreeze();
			return r;
		}
		HashSet<XmlNode> ProcessBookmarks(IList si, HashSet<XmlElement> processedItems, bool includeDescendant, IPdfInfoXmlProcessor processor) {
			if (si == null || si.Count == 0) {
				return null;
			}
			var undo = new UndoActionGroup();
			foreach (BookmarkElement item in si) {
				ProcessItem(includeDescendant, processor, processedItems, undo, item);
			}
			if (undo.Count > 0) {
				Model.Undo.AddUndo(processor.Name, undo);
			}
			return new HashSet<XmlNode>(undo.AffectedElements);
		}

		static void ProcessItem(bool includeDescendant, IPdfInfoXmlProcessor processor, HashSet<XmlElement> processedItems, UndoActionGroup undo, BookmarkElement item) {
			if (item == null || processedItems.Contains(item)) {
				return;
			}
			undo.Add(processor.Process(item));
			processedItems.Add(item);
			if (includeDescendant) {
				foreach (BookmarkElement d in item.SubBookmarks) {
					ProcessItem(includeDescendant, processor, processedItems, undo, d);
				}
			}
		}

		internal XmlElement PrepareBookmarkDocument() {
			if (Model.Document == null) {
				Model.Document = new PdfInfoXmlDocument();
			}
			return Model.Document.BookmarkRoot;
		}

		internal void ClearBookmarks() {
			Model.Document.BookmarkRoot.RemoveAll();
			View.Bookmark.ClearObjects();
		}

		void LoadPdfDocument() {
			var s = Model.GetPdfFilePath();
			var v = View.Viewer;
			if (s != null) {
				try {
					var d = v.Document;
					Model.PdfDocument = v.Document = PdfHelper.OpenMuDocument(s);
					d.TryDispose();
					View.AutoBookmark.TryDispose();
					v.Enabled = true;
					View.ViewerToolbar.Enabled = true;
					View.Viewer.Invalidate();
				}
				catch (Exception ex) {
					AppContext.MainForm.ErrorBox("加载文件时出错", ex);
					s = null;
				}
			}
			if (s == null) {
				Uninitialize(v);
			}
		}

		internal void Uninitialize(PdfViewerControl v) {
			v.Document.TryDispose();
			View.AutoBookmark.TryDispose();
			Model.PdfDocument = v.Document = null;
			v.Enabled = false;
			View.ViewerToolbar.Enabled = false;
		}

		internal void Destroy() {
			//if (_view.InsertBookmarkForm != null) {
			//	_view.InsertBookmarkForm.Dispose ();
			//	_view.InsertBookmarkForm = null;
			//}
			if (_loader != null) {
				_loader.RunWorkerCompleted -= _LoadBookmarkWorker_RunWorkerCompleted;
				_loader.Dispose();
			}
			Model.PdfDocument.TryDispose();
			View.AutoBookmark.TryDispose();
		}

		internal void InitBookmarkEditor() {
			Model.Undo.Clear();
			View.Bookmark.DeselectAll();
			View.UndoButton.DropDown.Items.Clear();
			View.UndoButton.Enabled = false;
			View.Bookmark.ClearMarks(false);
		}

		internal void LoadDocument(string path, bool importMode) {
			if (File.Exists(path) == false) {
				FormHelper.ErrorBox("找不到文件：" + path);
				return;
			}
			var ext = Path.GetExtension(path).ToLowerInvariant();
			var infoDoc = new PdfInfoXmlDocument();
			switch (ext) {
				case Constants.FileExtensions.Txt:
					OutlineManager.ImportSimpleBookmarks(path, infoDoc);
					goto case "<load>";
				case Constants.FileExtensions.Xml:
					infoDoc.Load(path);
					goto case "<load>";
				case Constants.FileExtensions.Pdf:
					View.MainPanel.Enabled = View.BookmarkToolbar.Enabled = false;
					if (importMode == false) {
						View.DocumentPath = path;
					}
					_loader = new BackgroundWorker();
					_loader.RunWorkerCompleted += _LoadBookmarkWorker_RunWorkerCompleted;
					_loader.DoWork += _LoadBookmarkWorker_DoWork;
					Model.IsLoadingDocument = true;
					_loader.RunWorkerAsync(new object[] { path, importMode });
					break;
				case "<load>":
					if (importMode) {
						LoadInfoDocument(infoDoc, importMode);
						break;
					}
					View.Bookmark.ClearObjects();
					View.DocumentPath = path;
					LoadInfoDocument(infoDoc, importMode);
					LoadPdfDocument();
					break;
				default:
					return;
			}
			RecentFileMenuHelper.AddRecentHistoryFile(path);
			//// 书签编辑器窗口需要重画表头
			//this._BookmarkBox.HeaderControl.Invalidate ();
		}

		void _LoadBookmarkWorker_DoWork(object sender, DoWorkEventArgs e) {
			var args = e.Argument as object[];
			var path = args[0] as string;
			bool importMode = (bool)args[1];
			Tracker.DebugMessage("open file");
			using (var reader = PdfHelper.OpenPdfFile(path, AppContext.LoadPartialPdfFile, false)) {
				try {
					Tracker.DebugMessage("consolidate");
					try {
						reader.ConsolidateNamedDestinations();
					}
					catch (Exception ex) {
						Tracker.TraceMessage(ex);
					}
					Tracker.DebugMessage("get bookmark");
					e.Result = new object[] {
					OutlineManager.GetBookmark (reader, new UnitConverter () { Unit = Constants.Units.Point }),
						importMode,
						path
					};
					Tracker.DebugMessage("finished loading");
				}
				catch (iTextSharp.text.exceptions.BadPasswordException) {
					FormHelper.ErrorBox(Messages.PasswordInvalid);
					Tracker.TraceMessage(Tracker.Category.Error, Messages.PasswordInvalid);
				}
				catch (Exception ex) {
					AppContext.MainForm.ErrorBox("在打开 PDF 文件时遇到错误", ex);
				}
			}
		}

		void _LoadBookmarkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			((BackgroundWorker)sender).Dispose();
			Model.IsLoadingDocument = false;
			View.MainPanel.Enabled = View.BookmarkToolbar.Enabled = true;
			var r = e.Error == null ? e.Result as object[] : null;
			if (r == null) {
				// 异常终止
				ClearBookmarks();
				InitBookmarkEditor();
				Uninitialize(View.Viewer);
				return;
			}
			LoadPdfDocument();
			var importMode = (bool)r[1];
			if (importMode == false) {
				View.DocumentPath = r[2] as string;
			}
			if (r[0] is XmlElement b) {
				var infoDoc = new PdfInfoXmlDocument();
				var root = infoDoc.DocumentElement;
				(root.AppendChild(infoDoc.CreateElement(Constants.Units.ThisName)) as XmlElement).SetAttribute(Constants.Units.Unit, Constants.Units.Point);
				root.AppendChild(infoDoc.ImportNode(b, true));
				LoadInfoDocument(infoDoc, importMode);
				if (importMode == false) {
					infoDoc.PdfDocumentPath = Model.DocumentPath;
				}
			}
			else {
				if (importMode) {
					FormHelper.InfoBox("文档不包含书签。");
					return;
				}
				ClearBookmarks();
				InitBookmarkEditor();
				//if (_currentFilePath != null) {
				//	Common.FormHelper.InfoBox ("文档不包含书签。");
				//}
			}
			//_viewer = new PdfViewer (this._FilePathBox.Text, _doc.DocumentElement);
		}

		internal void LoadInfoDocument(PdfInfoXmlDocument document, bool importMode) {
			var b = View.Bookmark;
			var m = document.Bookmarks;
			if (importMode) {
				if (m.Count > 0) {
					ImportBookmarks(b, m);
				}
				return;
			}
			// 文档不包含书签
			if (m.Count == 0) {
				ClearBookmarks();
				InitBookmarkEditor();
				Model.Document = new PdfInfoXmlDocument();
				PrepareBookmarkDocument();
				return;
			}
			Model.Document = document;
			LoadBookmarks(b, m);
			if (Model.PdfDocument != null && document.PageLabelRoot.HasChildNodes) {
				var pl = Model.PdfDocument.PageLabels;
				pl.Clear();
				foreach (PageLabelElement item in document.PageLabels) {
					pl.Add(item.ToPageLabel());
				}
			}
		}

		void LoadBookmarks(BookmarkEditorView view, XmlNodeList bookmarks) {
			InitBookmarkEditor();
			view.LoadBookmarks(bookmarks);
		}

		void ImportBookmarks(BookmarkEditorView editView, XmlNodeList bookmarks) {
			if (Model.Document == null) {
				Model.Document = new PdfInfoXmlDocument();
			}
			var d = Model.Document;
			var g = new UndoActionGroup();
			var s = editView.GetFirstSelectedModel<BookmarkElement>();
			var il = new List<XmlNode>();
			XmlElement n;
			if (s != null) {
				foreach (XmlNode item in bookmarks) {
					n = s.AppendChild(d.ImportNode(item, true)) as XmlElement;
					g.Add(new RemoveElementAction(n));
					il.Add(n);
				}
				editView.RefreshObject(s);
				editView.Expand(s);
			}
			else {
				var r = d.BookmarkRoot;
				foreach (XmlNode item in bookmarks) {
					n = r.AppendChild(d.ImportNode(item, true)) as XmlElement;
					g.Add(new RemoveElementAction(n));
					il.Add(n);
				}
				editView.Roots = d.Bookmarks;
			}
			editView.SelectedObjects = il;
			Model.Undo.AddUndo("导入书签", g);
		}

		//internal void HideInsertBookmarkForm () {
		//	if (_view.InsertBookmarkForm != null) {
		//		_view.InsertBookmarkForm.Visible = false;
		//	}
		//}

		//internal void BookmarkAtClientPoint (Point cp) {
		//	var v = _view.Viewer;
		//	var pp = v.TransposeClientToPagePosition (cp.X, cp.Y);
		//	if (pp.Page == 0) {
		//		return;
		//	}
		//	if (Control.ModifierKeys == Keys.Control) {
		//		v.PinPoint = v.PointToImage (cp);
		//		ShowInsertBookmarkDialog (cp, new EditModel.Region (pp, null, EditModel.TextSource.Empty));
		//		return;
		//	}
		//	var r = CopyText (cp, pp);
		//	ShowInsertBookmarkDialog (cp, r);
		//}

		internal EditModel.Region CopyText(Point cp, PagePosition pp) {
			var v = View.Viewer;
			var ps = v.IsClientPointInSelection(cp);
			var lines = ps ? v.FindTextLines(v.GetSelectionPageRegion()) : v.FindTextLines(pp).Lines;
			string t = null;
			EditModel.TextSource ts;
			if (Model.InsertBookmarkWithOcrOnly == false && lines.HasContent()) {
				var sb = StringBuilderCache.Acquire();
				var r = lines[0].BBox;
				foreach (var line in lines) {
					if (sb.Length > 100) {
						break;
					}
					t = line.Text.TrimEnd();
					if (sb.Length > 0 && t.Length > 0) {
						var c = t[0];
						sb.Append(' ');
						r = r.Union(line.BBox);
					}
					sb.Append(t);
				}
				t = StringBuilderCache.GetStringAndRelease(sb);
				var b = v.MuRectangleToImageRegion(pp.Page, r);
				v.SelectionRegion = b;
				v.PinPoint = b.Location.Round();
				b.Offset(v.GetVirtualImageOffset(pp.Page));
				pp = v.TransposeVirtualImageToPagePosition(pp.Page, v.PinPoint.X, v.PinPoint.Y);
				ts = EditModel.TextSource.Text;
			}
			else if (t == null && ModiOcr.ModiInstalled && v.OcrLanguage != 0) {
				v.UseWaitCursor = true;
				var r = v.OcrPage(pp.Page, true);
				v.UseWaitCursor = false;
				if (r.HasContent()) {
					var ir = v.GetSelection().ImageRegion;
					var ib = new Bound(ir.Left, ir.Bottom, ir.Right, ir.Top);
					var b = RectangleF.Empty;
					if (ps) {
						var mr = r.FindAll((i) => i.Region.IntersectWith(ib));
						if (mr.HasContent()) {
							var sb = StringBuilderCache.Acquire();
							b = mr[0].Region;
							foreach (var line in mr) {
								t = OcrProcessor.CleanUpText(line.Text, v.OcrOptions);
								if (sb.Length > 0 && t.Length > 0) {
									var c = t[0];
									sb.Append(' ');
									b = b.Union(line.Region);
								}
								sb.Append(t);
							}
							t = StringBuilderCache.GetStringAndRelease(sb);
						}
					}
					else {
						var l = v.TransposeClientToPageImage(cp.X, cp.Y);
						var tl = r.Find((i) => i.Region.Contains(l.ImageX, l.ImageY));
						if (tl != null) {
							t = tl.Text;
							b = (RectangleF)tl.Region;
						}
					}
					if (b != RectangleF.Empty) {
						b.Offset(v.GetVirtualImageOffset(pp.Page));
						v.SelectionRegion = b;
						v.PinPoint = b.Location.Round();
						pp = v.TransposeVirtualImageToPagePosition(pp.Page, b.Left.ToInt32(), b.Top.ToInt32());
						ts = EditModel.TextSource.OcrText;
					}
					else {
						ts = EditModel.TextSource.Empty;
					}
				}
				else {
					ts = EditModel.TextSource.OcrError;
				}
			}
			else {
				v.PinPoint = v.PointToImage(cp);
				ts = EditModel.TextSource.Empty;
			}
			return new EditModel.Region(pp, t, ts);
		}

		//void ShowInsertBookmarkDialog (Point mousePoint, EditModel.Region region) {
		//	var p = region.Position;
		//	if (p.Page == 0) {
		//		return;
		//	}
		//	if (_view.InsertBookmarkForm == null) {
		//		_view.InsertBookmarkForm = new InsertBookmarkForm ();
		//		_view.InsertBookmarkForm.OkClicked += _insertBookmarkForm_OkClicked;
		//		_view.InsertBookmarkForm.VisibleChanged += (s, args) => {
		//			_view.Viewer.ShowPinPoint = (s as Form).Visible;
		//		};
		//	}
		//	var f = _view.InsertBookmarkForm;
		//	var v = _view.Viewer;
		//	var vp = v.GetImageViewPort ();
		//	Point fp;
		//	var sr = v.SelectionRegion;
		//	if (sr != RectangleF.Empty) {
		//		fp = v.TransposeVirtualImageToClient (sr.Left, sr.Top);
		//		if (v.HorizontalFlow) {
		//			fp.X += sr.Width.ToInt32 () + 20;
		//		}
		//		else {
		//			fp.Y -= f.Height + 20;
		//		}
		//	}
		//	else {
		//		fp = new Point (mousePoint.X + 20, mousePoint.Y - f.Height);
		//	}
		//	var l = v.PointToScreen (fp);
		//	if (l.Y < 0) {
		//		l.Y = l.Y + (int)sr.Height + f.Height + 40;
		//		if (l.Y + f.Height > Screen.PrimaryScreen.WorkingArea.Height) {
		//			l.Y = Screen.PrimaryScreen.WorkingArea.Height - f.Height;
		//		}
		//	}
		//	if (l.X < v.PointToScreen (Point.Empty).X) {
		//		l.X = v.PointToScreen (Point.Empty).X;
		//	}
		//	f.Location = l;
		//	f.TargetPosition = p.PageY;
		//	if (String.IsNullOrEmpty (region.Text) == false) {
		//		f.Title = __RemoveOcrWhiteSpace.Replace (region.Text, " ").Trim ();
		//	}
		//	f.Comment = region.LiteralTextSource;
		//	f.Show ();
		//	f.TargetPageNumber = p.Page;
		//	f.FormClosed += (s1, a1) => { _view.InsertBookmarkForm = null; };
		//}

		internal void LabelAtPage(Editor.PagePosition position) {
			if (position.Page == 0) {
				return;
			}
			var l = Model.PdfDocument.PageLabels;
			if (l == null) {
				return;
			}
			var v = View.Viewer;
			var f = new InsertPageLabelForm {
				Location = Cursor.Position.Transpose(-16, -16),
				PageNumber = position.Page
			};
			var pl = l.Find(position.Page);
			if (pl.IsEmpty == false) {
				f.SetValues(pl);
			}
			f.FormClosed += InsertPageLabelForm_Closed;
			f.Show();
		}

		void InsertPageLabelForm_Closed(object sender, EventArgs e) {
			var form = sender as InsertPageLabelForm;
			if (form.DialogResult == DialogResult.Cancel) {
				return;
			}
			var l = Model.PdfDocument.PageLabels;
			if (form.DialogResult == DialogResult.OK) {
				if (l == null) {
					return;
				}
				l.Add(form.PageLabel);
			}
			else if (form.DialogResult == DialogResult.Abort) {
				Model.PdfDocument.PageLabels.Remove(form.PageLabel);
			}
			var pl = Model.Document.PageLabelRoot;
			pl.InnerText = String.Empty;
			foreach (var item in l) {
				pl.AppendChild(Model.Document.CreatePageLabel(item));
			}
			View.Viewer.Invalidate();
		}

		internal void InsertBookmark() {
			if (Model.PdfDocument != null) {
				var pn = View.Viewer.CurrentPageNumber;
				var p = View.Viewer.TransposeClientToPagePosition(0, 0).PageY;
				var pt = View.Viewer.GetPageBound(pn).Bottom;
				if (pt < p) {
					p = pt;
				}
				InsertBookmark(null, pn, p, (Control.ModifierKeys & Keys.Shift) > 0 ? InsertBookmarkPositionType.BeforeCurrent : InsertBookmarkPositionType.AfterCurrent);
			}
			else {
				InsertBookmark(null, 0, 0, InsertBookmarkPositionType.AfterCurrent);
			}
		}

		internal void InsertBookmark(string title, int pageNumber, float position, InsertBookmarkPositionType type) {
			var b = View.Bookmark;
			var d = Model.Document;
			int i = b.SelectedIndex;
			BookmarkElement c = null;
			if (i == -1 && b.Items.Count == 0) {
				c = d.BookmarkRoot.AppendBookmark();
				Model.Undo.AddUndo("插入书签", new RemoveElementAction(c));
				var s = title ?? Path.GetFileNameWithoutExtension(Model.DocumentPath);
				c.SetTitleAndGotoPagePosition(
					string.IsNullOrEmpty(s) ? Constants.Bookmark : s,
					pageNumber > 0 ? pageNumber : 1,
					position);
				b.Roots = new XmlElement[] { c };
			}
			else {
				var o = b.GetModelObject(i != -1 ? i : b.GetItemCount() - 1) as BookmarkElement;
				var t = title ?? Constants.Bookmark;
				var p = pageNumber > 0 ? pageNumber : o.Page;
				if (type == InsertBookmarkPositionType.NoDefined) {
					var g = new UndoActionGroup();
					if (t.Length > 0) {
						g.SetAttribute(o, Constants.BookmarkAttributes.Title, t);
					}
					g.SetAttribute(o, Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
					g.SetAttribute(o, Constants.DestinationAttributes.Page, p.ToText());
					g.SetAttribute(o, Constants.DestinationAttributes.View, Constants.DestinationAttributes.ViewType.XYZ);
					g.SetAttribute(o, Constants.Coordinates.Top, position.ToText());
					Model.Undo.AddUndo("替换书签", g);
					c = o;
				}
				else {
					c = d.CreateBookmark();
					c.SetTitleAndGotoPagePosition(t, p, position);
					switch (type) {
						case InsertBookmarkPositionType.AfterCurrent:
							goto default;
						case InsertBookmarkPositionType.AsChild:
							o.AppendChild(c);
							break;
						case InsertBookmarkPositionType.AfterParent:
							if (o.ParentBookmark != null) {
								o.ParentBookmark.ParentNode.InsertAfter(c, o.ParentBookmark);
							}
							else {
								goto default;
							}
							break;
						case InsertBookmarkPositionType.BeforeCurrent:
							o.ParentNode.InsertBefore(c, o);
							break;
						default:
							o.ParentNode.InsertAfter(c, o);
							break;
					}
					Model.Undo.AddUndo("插入书签", new RemoveElementAction(c));
				}
				if (c.ParentNode.Name == Constants.DocumentBookmark) {
					b.SetObjects(c.Parent.SubBookmarks);
				}
				else {
					b.RefreshObject(c.ParentNode);
					b.RefreshObject(c);
				}
			}
			if (c != null) {
				if (c.ParentNode.Name == Constants.Bookmark) {
					b.Expand(c.ParentNode);
				}
				b.Expand(c);
				b.EnsureItemsVisible(new BookmarkElement[] { c });
				b.SelectedObjects = new BookmarkElement[] { c };
				b.FocusedObject = c;
				b.ModelToItem (c).BeginEdit ();
			}
		}

		internal void Undo(int step) {
			if (Model.Undo.CanUndo == false) {
				return;
			}
			Model.LockDownViewer = true;
			var sl = View.Bookmark.SelectedObjects;
			XmlElement e;
			bool r = false; // 是否需要刷新根节点
			var rl = new HashSet<XmlNode>();
			while (step-- > 0) {
				var a = Model.Undo.Undo();
				foreach (var item in a) {
					e = item as XmlElement;
					if (r == false && e.Name == Constants.DocumentBookmark) {
						r = true;
					}
					else {
						rl.Add(item);
					}
				}
			}

			View.Bookmark.RefreshObjects(rl.ToArray());
			if (r) {
				View.Bookmark.Roots = Model.Document.Bookmarks;
			}
			View.Bookmark.SelectedObjects = sl;
			View.UndoButton.Enabled = Model.Undo.CanUndo;
			Model.LockDownViewer = false;
		}

		internal void MergeBookmark(IList<BookmarkElement> es) {
			var l = es.Count;
			if (l < 2) {
				return;
			}
			var p = es[0].ParentNode;
			for (int i = 1; i < l; i++) {
				//if (es[i].SelectSingleNode (Constants.Bookmark) != null) {
				//    Common.Form.ErrorBox ("合并的书签不能有子书签。");
				//    return;
				//}
				if (es[i].ParentNode != p && es[i].ParentNode != es[0]) {
					FormHelper.ErrorBox("合并的书签必须有相同的上级书签。");
					return;
				}
			}
			var undo = new UndoActionGroup();
			var ts = new string[l];
			var dest = es[0];
			ts[0] = dest.Title;
			var ct = dest.OwnerDocument.CreateDocumentFragment();
			for (int i = 1; i < l; i++) {
				ts[i] = es[i].GetAttribute(Constants.BookmarkAttributes.Title);
				if (ts[i].Length > 0) {
					var c = ts[i][0];
					if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z') {
						ts[i] = " " + ts[i];
					}
				}
				while (es[i].HasChildNodes) {
					var c = es[i].FirstChild as XmlElement;
					if (c == null) {
						continue;
					}
					undo.Add(new AddElementAction(c));
					ct.AppendChild(c);
				}
				if (es[i].ParentNode == p) {
					undo.Add(new AddElementAction(es[i]));
					p.RemoveChild(es[i]);
				}
				else /*es[i].ParentNode = es[0]*/ {
					undo.Add(new AddElementAction(es[i]));
					dest.RemoveChild(es[i]);
				}
			}
			while (ct.HasChildNodes) {
				dest.AppendChild(ct.FirstChild);
			}
			undo.Add(UndoAttributeAction.GetUndoAction(dest, Constants.BookmarkAttributes.Title, String.Concat(ts)));
			var b = View.Bookmark;
			if (p.Name != Constants.DocumentBookmark) {
				b.RefreshObject(p);
			}
			else {
				b.SetObjects(p.SelectNodes(Constants.Bookmark));
			}
			Model.Undo.AddUndo("合并书签", undo);
			b.RefreshObject(dest);
			b.SelectObject(dest);
		}


		internal void ConfigAutoBookmarkTextStyles(int level, Editor.TextInfo textInfo) {
			if (textInfo.Spans == null) {
				return;
			}
			foreach (var span in textInfo.Spans) {
				var s = textInfo.Page.GetFont(span);
				if (s == null) {
					continue;
				}
				var fn = PdfDocumentFont.RemoveSubsetPrefix(s.Name);
				bool m = false;
				int fs = span.Size.ToInt32();
				foreach (var item in Model.TitleStyles) {
					if (item.FontSize == fs && item.FontName == fn && item.MatchPattern == null) {
						m = true;
						goto NEXT;
					}
				}
				if (m == false) {
					Model.TitleStyles.Add(new EditModel.AutoBookmarkSettings(level, fn, fs));
				}
			NEXT:;
			}
			ShowAutoBookmarkForm();
		}

		internal void ShowAutoBookmarkForm() {
			var f = View.AutoBookmark;
			if (f.Visible == false) {
				f.Location = Cursor.Position.Transpose(-16, -16);
				f.Show(View.Viewer);
			}
			f.SetValues(Model.TitleStyles);
		}

		internal void AutoBookmark(IEnumerable<EditModel.AutoBookmarkSettings> list, bool mergeAdjacentTitle, bool keepExisting) {
			View.Bookmark.CancelCellEdit();
			var pdf = Model.PdfDocument;
			BookmarkContainer bm = Model.Document.BookmarkRoot;
			var c = pdf.PageCount;
			var bs = new List<EditModel.AutoBookmarkSettings>(list);
			if (bs.Count == 0) {
				return;
			}
			bs.Sort((x, y) => x.Level - y.Level);
			var mp = new Func<string, bool>[bs.Count];
			for (var i = 0; i < bs.Count; i++) {
				var m = bs[i].MatchPattern?.CreateMatcher();
				if (m != null) {
					mp[i] = m.Matches;
				}
			}
			var ug = new UndoActionGroup();
			Model.Undo.AddUndo("自动生成书签", ug);
			foreach (XmlElement item in bm.SubBookmarks) {
				ug.Add(new AddElementAction(item));
			}
			if (keepExisting == false) {
				bm.RemoveAll();
			}
			var spans = new List<MuPdfSharp.MuTextSpan>(3);
			var bl = 0;
			for (int i = 0; i < c;) {
				using (var p = pdf.LoadPage(++i)) {
					var h = p.VisualBound.Height;
					var dh = p.VisualBound.Bottom - h;
					foreach (var block in p.TextPage.Blocks) {
						foreach (var line in block.Lines) {
							foreach (var span in line.Spans) {
								for (var si = 0; si < bs.Count; si++) {
									var style = bs[si];
									var matcher = mp[si];
									if (style.FontName != PdfDocumentFont.RemoveSubsetPrefix(p.GetFont(span).Name)
										|| style.FontSize != span.Size.ToInt32()) {
										continue;
									}
									var t = span.Text;
									if (t.Length == 0) {
										continue;
									}
									var b = span.Box;
									if (bl < style.Level) {
										if (matcher?.Invoke(line.Text) == false) {
											continue;
										}
										bm = CreateNewSiblingBookmark(bm, spans);
										++bl;
									}
									else if (bl == style.Level) {
										// todo 删除重复的文本
										var cb = bm as BookmarkElement;
										var bb = h - cb.Bottom + dh;
										var bt = h - cb.Top;
										var lt = b.Top - b.Height * 2 + dh;
										var lb = b.Bottom;
										if (cb.Page == p.PageNumber
											&& (bb >= lt && bb <= lb || bt >= lt && bt <= lb || bt < lt && bb > lb)
											&& (mergeAdjacentTitle || spans[spans.Count - 1].Point.Y == span.Point.Y)) {
											if (/*m == false &&*/ t.Length > 0) {
												// 保留英文和数字文本之间的空格
												var ct = cb.Title;
												if (ct.Length > 0) {
													var lc = ct[ct.Length - 1];
													cb.Title = (Char.IsLetterOrDigit(lc) || Char.IsPunctuation(lc)
															&& lc != '-') && t[0] != ' '
														? ct + ' ' + t
														: ct + t;
												}
												cb.Bottom = h - lb;
												spans.Add(span);
											}
											continue;
										}
										if (matcher?.Invoke(line.Text) == false) {
											continue;
										}
										bm = CreateNewSiblingBookmarkForParent(bm, spans);
									}
									else {
										while (bl > style.Level) {
											bm = bm.ParentBookmark;
											--bl;
										}
										if (matcher?.Invoke(line.Text) == false) {
											continue;
										}
										bm = CreateNewSiblingBookmarkForParent(bm, spans);
									}
									var be = bm as BookmarkElement;
									var s = style.Bookmark;
									if (s.IsBold || s.IsItalic) {
										be.TextStyle = s.IsBold && s.IsItalic ? FontStyle.Bold | FontStyle.Italic
											: s.IsBold ? FontStyle.Bold
											: s.IsItalic ? FontStyle.Italic
											: FontStyle.Regular;
									}
									be.Title = t;
									be.Top = s.GoToTop ? h + dh : h - b.Top + b.Height + dh;
									be.Bottom = h - b.Bottom + dh;
									be.Action = Constants.ActionType.Goto;
									be.Page = p.PageNumber;
									if (s.IsOpened) {
										be.IsOpen = true;
									}
									be.ForeColor = s.ForeColor;
									//todo 删除尾随的空格
									ug.Add(new RemoveElementAction(bm));
									spans.Add(span);
									break;
								}
							}
						}
					}
				}
			}
			View.Bookmark.Roots = Model.Document.Bookmarks;
			View.Bookmark.RebuildAll(false);
		}

		static BookmarkContainer CreateNewSiblingBookmarkForParent(BookmarkContainer bm, List<MuPdfSharp.MuTextSpan> spans) {
			TrimBookmarkText(bm);
			bm = bm.Parent.AppendBookmark();
			spans.Clear();
			return bm;
		}

		static void TrimBookmarkText(BookmarkContainer bm) {
			if (bm is BookmarkElement b) {
				var t = b.Title;
				var t2 = b.Title.Trim();
				if (t2 != t) {
					b.Title = t2;
				}
			}
		}

		static BookmarkContainer CreateNewSiblingBookmark(BookmarkContainer bm, List<MuPdfSharp.MuTextSpan> spans) {
			TrimBookmarkText(bm);
			bm = bm.AppendBookmark();
			spans.Clear();
			return bm;
		}
	}
}
