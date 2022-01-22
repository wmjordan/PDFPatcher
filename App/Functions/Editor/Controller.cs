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
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using MuPdfSharp;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;
using PageLabel = MuPdfSharp.PageLabel;
using Point = System.Drawing.Point;
using Rectangle = MuPdfSharp.Rectangle;

namespace PDFPatcher.Functions.Editor
{
	sealed class Controller
	{
		BackgroundWorker _loader;

		public Controller(IEditView view) {
			Model = new EditModel();
			View = view;
			Model.Undo.OnAddUndo += (UndoManager u, IUndoAction a) => View.UndoButton.Enabled = true;
			View.Bookmark.Undo = Model.Undo;
		}

		public EditModel Model { get; }
		public IEditView View { get; }

		internal IEnumerable<XmlNode> ProcessBookmarks(IPdfInfoXmlProcessor processor) {
			return ProcessBookmarks(View.AffectsDescendantBookmarks, true, processor);
		}

		/// <summary>
		///     逐个处理选中的书签。
		/// </summary>
		/// <param name="includeDescendant">处理操作是否包含选中书签的内层书签。</param>
		/// <param name="selectChildren">处理时是否遍历选中的内层书签。</param>
		/// <param name="processor">用于处理书签的 <see cref="IPdfInfoXmlProcessor" />。</param>
		/// <returns>处理后的书签。</returns>
		internal IEnumerable<XmlNode> ProcessBookmarks(bool includeDescendant, bool selectChildren,
			IPdfInfoXmlProcessor processor) {
			BookmarkEditorView b = View.Bookmark;
			b.Freeze();
			List<BookmarkElement> si = b.GetSelectedElements(selectChildren);
			HashSet<XmlElement> pi = new HashSet<XmlElement>();
			HashSet<XmlNode> r = ProcessBookmarks(si, pi, includeDescendant, processor);
			if (r != null) {
				foreach (var i in r.Select(item => item as XmlElement).Where(i => i.ParentNode.Name == Constants.DocumentBookmark)) {
					b.Roots = i.ParentNode.SelectNodes(Constants.Bookmark).ToXmlNodeArray();
					break;
				}

				b.RefreshObjects(r.ToArray());
			}

			b.Unfreeze();
			return r;
		}

		HashSet<XmlNode> ProcessBookmarks(ICollection si, ISet<XmlElement> processedItems, bool includeDescendant,
			IPdfInfoXmlProcessor processor) {
			if (si == null || si.Count == 0) {
				return null;
			}

			UndoActionGroup undo = new UndoActionGroup();
			foreach (BookmarkElement item in si) {
				ProcessItem(includeDescendant, processor, processedItems, undo, item);
			}

			if (undo.Count > 0) {
				Model.Undo.AddUndo(processor.Name, undo);
			}

			return new HashSet<XmlNode>(undo.AffectedElements);
		}

		static void ProcessItem(bool includeDescendant, IPdfInfoXmlProcessor processor, ISet<XmlElement> processedItems,
			UndoActionGroup undo, BookmarkContainer item) {
			if (item == null || processedItems.Contains(item)) {
				return;
			}

			undo.Add(processor.Process(item));
			processedItems.Add(item);
			if (!includeDescendant) {
				return;
			}

			foreach (BookmarkElement d in item.SubBookmarks) {
				ProcessItem(includeDescendant, processor, processedItems, undo, d);
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
			string s = Model.GetPdfFilePath();
			PdfViewerControl v = View.Viewer;
			if (s != null) {
				try {
					MuDocument d = v.Document;
					Model.PdfDocument = v.Document = new MuDocument(s);
					d.TryDispose();
					View.AutoBookmark.TryDispose();
					v.Enabled = true;
					View.ViewerToolbar.Enabled = true;
				}
				catch (Exception ex) {
					FormHelper.ErrorBox(ex.Message);
					s = null;
				}
			}

			if (s != null) {
				return;
			}

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

			string ext = Path.GetExtension(path).ToLowerInvariant();
			PdfInfoXmlDocument infoDoc = new PdfInfoXmlDocument();
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
						LoadPdfDocument();
					}

					if (Model.PdfDocument == null) {
						return;
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

		static void _LoadBookmarkWorker_DoWork(object sender, DoWorkEventArgs e) {
			object[] args = e.Argument as object[];
			string path = args[0] as string;
			bool importMode = (bool)args[1];
			Tracker.DebugMessage("open file");
			using PdfReader reader = PdfHelper.OpenPdfFile(path, AppContext.LoadPartialPdfFile, false);
			try {
				Tracker.DebugMessage("consolidate");
				reader.ConsolidateNamedDestinations();
				Tracker.DebugMessage("get bookmark");
				e.Result = new object[] {
					OutlineManager.GetBookmark(reader, new UnitConverter() {Unit = Constants.Units.Point}),
					importMode, path
				};
				Tracker.DebugMessage("finished loading");
			}
			catch (BadPasswordException) {
				FormHelper.ErrorBox(Messages.PasswordInvalid);
				Tracker.TraceMessage(Tracker.Category.Error, Messages.PasswordInvalid);
			}
			catch (Exception ex) {
				FormHelper.ErrorBox("在打开 PDF 文件时遇到错误：\n" + ex.Message);
			}
		}

		void _LoadBookmarkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			((BackgroundWorker)sender).Dispose();
			Model.IsLoadingDocument = false;
			View.MainPanel.Enabled = View.BookmarkToolbar.Enabled = true;
			if (e.Result is not object[] r) {
				// 异常终止
				ClearBookmarks();
				InitBookmarkEditor();
				return;
			}

			bool importMode = (bool)r[1];
			if (importMode == false) {
				View.DocumentPath = r[2] as string;
			}

			if (r[0] is XmlElement b) {
				PdfInfoXmlDocument infoDoc = new PdfInfoXmlDocument();
				XmlElement root = infoDoc.DocumentElement;
				(root.AppendChild(infoDoc.CreateElement(Constants.Units.ThisName)) as XmlElement).SetAttribute(
					Constants.Units.Unit, Constants.Units.Point);
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
			BookmarkEditorView b = View.Bookmark;
			XmlNodeList m = document.Bookmarks;
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
			if (Model.PdfDocument == null || !document.PageLabelRoot.HasChildNodes) {
				return;
			}

			PageLabelCollection pl = Model.PdfDocument.PageLabels;
			pl.Clear();
			foreach (PageLabelElement item in document.PageLabels) {
				pl.Add(item.ToPageLabel());
			}
		}

		void LoadBookmarks(BookmarkEditorView view, XmlNodeList bookmarks) {
			InitBookmarkEditor();
			view.LoadBookmarks(bookmarks);
		}

		void ImportBookmarks(TreeListView editView, XmlNodeList bookmarks) {
			if (Model.Document == null) {
				Model.Document = new PdfInfoXmlDocument();
			}

			PdfInfoXmlDocument d = Model.Document;
			UndoActionGroup g = new UndoActionGroup();
			BookmarkElement s = editView.GetFirstSelectedModel<BookmarkElement>();
			List<XmlNode> il = new List<XmlNode>();
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
				BookmarkRootElement r = d.BookmarkRoot;
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
			PdfViewerControl v = View.Viewer;
			bool ps = v.IsClientPointInSelection(cp);
			List<MuTextLine> lines = ps ? v.FindTextLines(v.GetSelectionPageRegion()) : v.FindTextLines(pp).TextLines;
			string t = null;
			EditModel.TextSource ts;
			if (Model.InsertBookmarkWithOcrOnly == false && lines.HasContent()) {
				StringBuilder sb = new StringBuilder();
				Rectangle r = lines[0].BBox;
				foreach (var line in lines.TakeWhile(line => sb.Length <= 100)) {
					t = line.Text.TrimEnd();
					if (sb.Length > 0 && t.Length > 0) {
						char c = t[0];
						sb.Append(' ');
						r = r.Union(line.BBox);
					}

					sb.Append(t);
				}

				t = sb.ToString();
				RectangleF b = v.MuRectangleToImageRegion(pp.Page, r);
				v.SelectionRegion = b;
				v.PinPoint = b.Location.Round();
				b.Offset(v.GetVirtualImageOffset(pp.Page));
				pp = v.TransposeVirtualImageToPagePosition(pp.Page, v.PinPoint.X, v.PinPoint.Y);
				ts = EditModel.TextSource.Text;
			}
			else if (t == null && ModiOcr.ModiInstalled && v.OcrLanguage != 0) {
				v.UseWaitCursor = true;
				List<TextLine> r = v.OcrPage(pp.Page, true);
				v.UseWaitCursor = false;
				if (r.HasContent()) {
					RectangleF ir = v.GetSelection().ImageRegion;
					Bound ib = new Bound(ir.Left, ir.Bottom, ir.Right, ir.Top);
					RectangleF b = RectangleF.Empty;
					if (ps) {
						List<TextLine> mr = r.FindAll((i) => i.Region.IntersectWith(ib));
						if (mr.HasContent()) {
							StringBuilder sb = new StringBuilder();
							b = mr[0].Region;
							foreach (TextLine line in mr) {
								t = OcrProcessor.CleanUpText(line.Text, v.OcrOptions);
								if (sb.Length > 0 && t.Length > 0) {
									char c = t[0];
									sb.Append(' ');
									b = b.Union(line.Region);
								}

								sb.Append(t);
							}

							t = sb.ToString();
						}
					}
					else {
						PagePoint l = v.TransposeClientToPageImage(cp.X, cp.Y);
						TextLine tl = r.Find((i) => i.Region.Contains(l.ImageX, l.ImageY));
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

		internal void LabelAtPage(PagePosition position) {
			if (position.Page == 0) {
				return;
			}

			PageLabelCollection l = Model.PdfDocument.PageLabels;
			if (l == null) {
				return;
			}

			PdfViewerControl v = View.Viewer;
			InsertPageLabelForm f = new InsertPageLabelForm {
				Location = Cursor.Position.Transpose(-16, -16),
				PageNumber = position.Page
			};
			PageLabel pl = l.Find(position.Page);
			if (pl.IsEmpty == false) {
				f.SetValues(pl);
			}

			f.FormClosed += InsertPageLabelForm_Closed;
			f.Show();
		}

		void InsertPageLabelForm_Closed(object sender, EventArgs e) {
			InsertPageLabelForm form = sender as InsertPageLabelForm;
			if (form.DialogResult == DialogResult.Cancel) {
				return;
			}

			PageLabelCollection l = Model.PdfDocument.PageLabels;
			switch (form.DialogResult) {
				case DialogResult.OK when l == null:
					return;
				case DialogResult.OK:
					l.Add(form.PageLabel);
					break;
				case DialogResult.Abort:
					Model.PdfDocument.PageLabels.Remove(form.PageLabel);
					break;
			}

			XmlElement pl = Model.Document.PageLabelRoot;
			pl.InnerText = String.Empty;
			foreach (PageLabel item in l) {
				pl.AppendChild(Model.Document.CreatePageLabel(item));
			}

			View.Viewer.Invalidate();
		}

		internal void InsertBookmark() {
			if (Model.PdfDocument != null) {
				int pn = View.Viewer.CurrentPageNumber;
				float p = View.Viewer.TransposeClientToPagePosition(0, 0).PageY;
				float pt = View.Viewer.GetPageBound(pn).Bottom;
				if (pt < p) {
					p = pt;
				}

				InsertBookmark(null, pn, p, InsertBookmarkPositionType.AfterCurrent);
			}
			else {
				InsertBookmark(null, 0, 0, InsertBookmarkPositionType.AfterCurrent);
			}
		}

		internal void InsertBookmark(string title, int pageNumber, float position, InsertBookmarkPositionType type) {
			BookmarkEditorView b = View.Bookmark;
			PdfInfoXmlDocument d = Model.Document;
			int i = b.SelectedIndex;
			BookmarkElement c = null;
			if (i == -1 && b.Items.Count == 0) {
				c = d.BookmarkRoot.AppendBookmark();
				Model.Undo.AddUndo("插入书签", new RemoveElementAction(c));
				string s = title ?? Path.GetFileNameWithoutExtension(Model.DocumentPath);
				c.SetTitleAndGotoPagePosition(
					string.IsNullOrEmpty(s) ? Constants.Bookmark : s,
					pageNumber > 0 ? pageNumber : 1,
					position);
				b.Roots = new XmlElement[] { c };
			}
			else {
				BookmarkElement o = b.GetModelObject(i != -1 ? i : b.GetItemCount() - 1) as BookmarkElement;
				string t = title ?? Constants.Bookmark;
				int p = pageNumber > 0 ? pageNumber : o.Page;
				if (type == InsertBookmarkPositionType.NoDefined) {
					UndoActionGroup g = new UndoActionGroup();
					g.SetAttribute(o, Constants.BookmarkAttributes.Title, t);
					g.SetAttribute(o, Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
					g.SetAttribute(o, Constants.DestinationAttributes.Page, p.ToText());
					g.SetAttribute(o, Constants.DestinationAttributes.View,
						Constants.DestinationAttributes.ViewType.XYZ);
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

			if (c == null) {
				return;
			}

			if (c.ParentNode.Name == Constants.Bookmark) {
				b.Expand(c.ParentNode);
			}

			b.Expand(c);
			b.EnsureItemsVisible(new BookmarkElement[] { c });
			b.SelectedObjects = new BookmarkElement[] { c };
			b.FocusedObject = c;
			//_BookmarkBox.ModelToItem (c).BeginEdit ();
		}

		internal void Undo(int step) {
			if (Model.Undo.CanUndo == false) {
				return;
			}

			Model.LockDownViewer = true;
			IList sl = View.Bookmark.SelectedObjects;
			bool r = false; // 是否需要刷新根节点
			HashSet<XmlNode> rl = new HashSet<XmlNode>();
			while (step-- > 0) {
				IEnumerable<XmlNode> a = Model.Undo.Undo();
				foreach (XmlNode item in a) {
					XmlElement e = item as XmlElement;
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
			int l = es.Count;
			if (l < 2) {
				return;
			}

			XmlNode p = es[0].ParentNode;
			for (int i = 1; i < l; i++) {
				//if (es[i].SelectSingleNode (Constants.Bookmark) != null) {
				//    Common.Form.ErrorBox ("合并的书签不能有子书签。");
				//    return;
				//}
				if (es[i].ParentNode == p || es[i].ParentNode == es[0]) {
					continue;
				}

				FormHelper.ErrorBox("合并的书签必须有相同的上级书签。");
				return;
			}

			UndoActionGroup undo = new UndoActionGroup();
			string[] ts = new string[l];
			BookmarkElement dest = es[0];
			ts[0] = dest.Title;
			XmlDocumentFragment ct = dest.OwnerDocument.CreateDocumentFragment();
			for (int i = 1; i < l; i++) {
				ts[i] = es[i].GetAttribute(Constants.BookmarkAttributes.Title);
				if (ts[i].Length > 0) {
					char c = ts[i][0];
					if (c is >= 'a' and <= 'z' || c is >= 'A' and <= 'Z') {
						ts[i] = " " + ts[i];
					}
				}

				while (es[i].HasChildNodes) {
					if (es[i].FirstChild is not XmlElement c) {
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
			BookmarkEditorView b = View.Bookmark;
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


		internal void ConfigAutoBookmarkTextStyles(int level, TextInfo textInfo) {
			if (textInfo.Spans == null) {
				return;
			}

			foreach (MuTextSpan span in textInfo.Spans) {
				MuFont s = textInfo.Page.GetFont(span);
				if (s != null) {
					bool m = false;
					int fs = span.Size.ToInt32();
					foreach (var item in Model.TitleStyles.Where(item => item.InternalFontName == s.Name && item.FontSize == fs)) {
						m = true;
						goto NEXT;
					}

					if (m == false) {
						Model.TitleStyles.Add(new EditModel.AutoBookmarkStyle(level, s.Name, fs));
					}
				}

			NEXT:;
			}

			ShowAutoBookmarkForm();
		}

		internal void ShowAutoBookmarkForm() {
			AutoBookmarkForm f = View.AutoBookmark;
			if (f.Visible == false) {
				f.Location = Cursor.Position.Transpose(-16, -16);
				f.Show(View.Viewer);
			}

			f.SetValues(Model.TitleStyles);
		}

		internal void AutoBookmark(IEnumerable<EditModel.AutoBookmarkStyle> list, bool mergeAdjacentTitle) {
			View.Bookmark.CancelCellEdit();
			MuDocument pdf = Model.PdfDocument;
			BookmarkContainer bm = Model.Document.BookmarkRoot;
			int c = pdf.PageCount;
			List<EditModel.AutoBookmarkStyle> bs = new List<EditModel.AutoBookmarkStyle>(list);
			if (bs.Count == 0) {
				return;
			}

			bs.Sort((x, y) => x.Level - y.Level);
			MatchPattern.IMatcher[] mp = new MatchPattern.IMatcher[bs.Count];
			for (int i = 0; i < bs.Count; i++) {
				mp[i] = bs[i].MatchPattern?.CreateMatcher();
			}

			UndoActionGroup ug = new UndoActionGroup();
			Model.Undo.AddUndo("自动生成书签", ug);
			foreach (XmlElement item in bm.SubBookmarks) {
				ug.Add(new AddElementAction(item));
			}

			bm.RemoveAll();
			List<MuTextSpan> spans = new List<MuTextSpan>(3);
			int bl = 0;
			for (int i = 0; i < c;) {
				using MuPage p = pdf.LoadPage(++i);
				float h = p.VisualBound.Height;
				float dh = p.VisualBound.Bottom - h;
				foreach (MuTextBlock block in p.TextPage.Blocks) {
					foreach (MuTextLine line in block.Lines) {
						foreach (MuTextSpan span in line.Spans) {
							for (int si = 0; si < bs.Count; si++) {
								EditModel.AutoBookmarkStyle style = bs[si];
								MatchPattern.IMatcher matcher = mp[si];
								if (style.FontName != PdfDocumentFont.RemoveSubsetPrefix(p.GetFont(span).Name)
									|| style.FontSize != span.Size.ToInt32()) {
									continue;
								}

								string t = span.Text;
								Rectangle b = span.Box;
								if (t.Length == 0) {
									continue;
								}

								if (bl < style.Level) {
									if (matcher?.Matches(line.Text) == false) {
										continue;
									}

									bm = CreateNewSiblingBookmark(bm, spans);
									++bl;
								}
								else if (bl == style.Level) {
									// todo+ 删除重复的文本
									BookmarkElement cb = bm as BookmarkElement;
									float bb = h - cb.Bottom + dh;
									float bt = h - cb.Top;
									float lt = b.Top - b.Height * 2 + dh;
									float lb = b.Bottom;
									if (cb.Page == p.PageNumber
										&& (bb >= lt && bb <= lb || bt >= lt && bt <= lb || bt < lt && bb > lb)
										&& (mergeAdjacentTitle || spans.Count == 1 ||
											spans[spans.Count - 1].Point.Y == span.Point.Y)) {
										//var m = false;
										//var bs = b.Size.Area();
										//foreach (var ss in spans) {
										//	var a = ss.Box.Intersect(b).Size.Area();
										//	var ov = a / bs;
										//	if (0.5 < ov && ov <= 1) {
										//		m = true;
										//		break;
										//	}
										//	ov = a / ss.Box.Size.Area();
										//	if (0.5 < ov && ov <= 1) {
										//		m = true;
										//		break;
										//	}
										//}
										if ( /*m == false &&*/ t.Length > 0) {
											// 保留英文和数字文本之间的空格
											string ct = cb.Title;
											if (ct.Length > 0) {
												char lc = ct[ct.Length - 1];
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

									if (matcher?.Matches(line.Text) == false) {
										continue;
									}

									bm = CreateNewSiblingBookmarkForParent(bm, spans);
								}
								else {
									while (bl > style.Level) {
										bm = bm.ParentBookmark;
										--bl;
									}

									if (matcher?.Matches(line.Text) == false) {
										continue;
									}

									bm = CreateNewSiblingBookmarkForParent(bm, spans);
								}

								BookmarkElement be = bm as BookmarkElement;
								be.Title = t;
								be.Top = h - b.Top + b.Height + dh;
								be.Bottom = h - b.Bottom + dh;
								be.Action = Constants.ActionType.Goto;
								be.Page = p.PageNumber;
								BookmarkSettings s = style.Style;
								if (s.IsBold || s.IsItalic) {
									be.TextStyle = s.IsBold && s.IsItalic ? FontStyle.Bold | FontStyle.Italic
										: s.IsBold ? FontStyle.Bold
										: s.IsItalic ? FontStyle.Italic
										: FontStyle.Regular;
								}

								if (s.IsOpened) {
									be.IsOpen = true;
								}

								be.ForeColor = s.ForeColor;
								//todo+ 删除尾随的空格
								ug.Add(new RemoveElementAction(bm));
								spans.Add(span);
								break;
							}
						}
					}
				}
			}

			View.Bookmark.Roots = Model.Document.Bookmarks;
			View.Bookmark.RebuildAll(false);
		}

		static BookmarkContainer CreateNewSiblingBookmarkForParent(BookmarkContainer bm, IList spans) {
			TrimBookmarkText(bm);
			bm = bm.Parent.AppendBookmark();
			spans.Clear();
			return bm;
		}

		static void TrimBookmarkText(BookmarkContainer bm) {
			if (bm is not BookmarkElement b) {
				return;
			}

			string t = b.Title;
			string t2 = b.Title.Trim();
			if (t2 != t) {
				b.Title = t2;
			}
		}

		static BookmarkContainer CreateNewSiblingBookmark(BookmarkContainer bm, IList spans) {
			TrimBookmarkText(bm);
			bm = bm.AppendBookmark();
			spans.Clear();
			return bm;
		}
	}
}