﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using BrightIdeasSoftware;
using CLR;
using MuPDF.Extensions;
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

		public void ExecuteCommand(string command, params string[] parameters) {
			EditorCommands.Execute(command, this, parameters);
		}

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
				b.FireBookmarkChanged();
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

		internal void Uninitialize(ViewerControl v) {
			v.Document.TryDispose();
			View.AutoBookmark.TryDispose();
			Model.PdfDocument = v.Document = null;
			v.Enabled = false;
			View.ViewerToolbar.Enabled = false;
		}

		internal void Destroy() {
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
			if (!File.Exists(path)) {
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
					if (!importMode) {
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
		}

		void _LoadBookmarkWorker_DoWork(object sender, DoWorkEventArgs e) {
			var args = e.Argument as object[];
			var path = args[0] as string;
			bool importMode = (bool)args[1];
			Tracker.DebugMessage("open file");
			try {
				Tracker.DebugMessage("get bookmark");
				using (var d = MuPDF.Document.Open(path)) {
					e.Result = new object[] {
						OutlineManager.GetBookmark(d, new UnitConverter { Unit = Constants.Units.Point }),
						importMode,
						path
					};
				}
				Tracker.DebugMessage("finished loading bookmark");
			}
			catch (Exception ex) {
				AppContext.MainForm.ErrorBox("在打开 PDF 文件时遇到错误", ex);
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
			if (!importMode) {
				View.DocumentPath = r[2] as string;
			}
			if (r[0] is XmlElement b) {
				var infoDoc = new PdfInfoXmlDocument();
				var root = infoDoc.DocumentElement;
				(root.AppendChild(infoDoc.CreateElement(Constants.Units.ThisName)) as XmlElement).SetAttribute(Constants.Units.Unit, Constants.Units.Point);
				root.AppendChild(infoDoc.ImportNode(b, true));
				LoadInfoDocument(infoDoc, importMode);
				if (!importMode) {
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
			}
		}

		void DecryptDocument(string fileName) {
			try {
				FilePath decryptedFilePath = fileName;
				FilePath p = fileName + ".tmp";
				var t = new FilePath(View.DocumentPath).ToFileInfo().CreationTime;
				using (var mupdf = PdfHelper.OpenMuDocument(View.DocumentPath)) {
					mupdf.Save(p, new MuPDF.WriterOptions { Clean = true, Encrypt = MuPDF.EncryptionMode.Remove });
				}
				if (decryptedFilePath.ExistsFile) {
					File.Delete(decryptedFilePath);
				}
				File.Move(p, decryptedFilePath);
				if (decryptedFilePath.Equals(View.DocumentPath)) {
					decryptedFilePath.ToFileInfo().CreationTime = t;
				}
				FormHelper.InfoBox("处理完毕，请尝试重新打开文档。");
			}
			catch (Exception ex) {
				AppContext.MainForm.ErrorBox("解密文档时出错", ex);
			}
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
				return;
			}
			Model.Document = document;
			LoadBookmarks(b, m);
			if (Model.PdfDocument != null && document.PageLabelRoot.HasChildNodes) {
				var pl = Model.PageLabels;
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

		internal EditModel.Region CopyText(Point cp, PagePosition pp) {
			var v = View.Viewer;
			var ps = v.IsClientPointInSelection(cp);
			var lines = ps ? v.FindTextLines(v.GetSelectionPageRegion()) : v.FindTextLines(pp).Lines;
			string t = null;
			EditModel.TextSource ts;
			if (!Model.InsertBookmarkWithOcrOnly && lines.HasContent()) {
				var sb = StringBuilderCache.Acquire();
				var r = lines[0].Bound;
				foreach (var line in lines) {
					if (sb.Length > 100) {
						break;
					}
					t = line.GetText().TrimEnd();
					if (sb.Length > 0 && t.Length > 0) {
						var c = t[0];
						sb.Append(' ');
						r = r.Union(line.Bound);
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

		internal void InsertBookmark(InsertBookmarkPositionType position = InsertBookmarkPositionType.Undefined) {
			if (position == InsertBookmarkPositionType.Undefined) {
				position = (Control.ModifierKeys & Keys.Shift) > 0 ? InsertBookmarkPositionType.BeforeCurrent : InsertBookmarkPositionType.AfterCurrent;
			}
			if (Model.PdfDocument != null) {
				var pn = View.Viewer.CurrentPageNumber;
				var p = View.Viewer.TransposeClientToPagePosition(0, 0).PageY;
				var pt = View.Viewer.GetPageBound(pn).Y1;
				if (pt < p) {
					p = pt;
				}
				InsertBookmark(null, pn, p, position);
			}
			else {
				InsertBookmark(null, 0, 0, position);
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
				if (type == InsertBookmarkPositionType.Undefined) {
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
						case InsertBookmarkPositionType.AfterGrandParent:
							if (o.ParentBookmark?.ParentBookmark != null) {
								o.ParentBookmark.ParentBookmark.ParentNode.InsertAfter(c, o.ParentBookmark.ParentBookmark);
							}
							else {
								goto case InsertBookmarkPositionType.AfterParent;
							}
							break;
						case InsertBookmarkPositionType.LastRoot:
							o.BookmarkRoot.AppendChild(c);
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
			if (!Model.Undo.CanUndo) {
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
					if (!r && e.Name == Constants.DocumentBookmark) {
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
			View.Bookmark.FireBookmarkChanged();
		}

		internal void MergeBookmark(IList<BookmarkElement> es) {
			var l = es.Count;
			if (l < 2) {
				return;
			}
			var p = es[0].ParentNode;
			for (int i = 1; i < l; i++) {
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
				ref var title = ref ts[i];
				var be = es[i];
				title = be.GetAttribute(Constants.BookmarkAttributes.Title);
				if (title.Length > 0) {
					var c = title[0];
					if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z') {
						title = " " + title;
					}
				}
				while (be.HasChildNodes) {
					if (be.FirstChild is not XmlElement c) {
						continue;
					}
					undo.Add(new AddElementAction(c));
					ct.AppendChild(c);
				}
				if (be.ParentNode == p) {
					undo.Add(new AddElementAction(be));
					p.RemoveChild(be);
				}
				else /*be.ParentNode = es[0]*/ {
					undo.Add(new AddElementAction(be));
					dest.RemoveChild(be);
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
				var s = span.Font;
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
				if (!m) {
					Model.TitleStyles.Add(new EditModel.AutoBookmarkSettings(level, fn, fs));
				}
			NEXT:;
			}
			ShowAutoBookmarkForm();
		}

		internal void ShowAutoBookmarkForm() {
			var f = View.AutoBookmark;
			if (!f.Visible) {
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
			if (!keepExisting) {
				bm.RemoveAll();
			}
			var spans = new List<MuPDF.TextSpan>(3);
			var bl = 0;
			for (int i = 0; i < c;) {
				using (var p = pdf.LoadPage(i++)) {
					var pb = p.Bound;
					var h = pb.Height;
					var dh = pb.Y1 - h;
					var pt = pb.Y0;
					foreach (var block in p.TextPage) {
						string jointBlockText = null;
						bool hasJointText = false;
						foreach (var line in block) {
							var matchLine = false;
							foreach (var span in line.GetSpans()) {
								if (matchLine) {
									break;
								}
								for (var si = 0; si < bs.Count; si++) {
									var style = bs[si];
									var matcher = mp[si];
									if (style.FontName != PdfDocumentFont.RemoveSubsetPrefix(span.Font.Name)
										|| style.FontSize != span.Size.ToInt32()) {
										continue;
									}
									var t = span.ToString();
									if (t.Length == 0) {
										continue;
									}
									var b = span.Bound;
									if (bl < style.Level) {
										if (matcher != null) {
											jointBlockText ??= GetLineText(line, block, out hasJointText);
											if ((!(matchLine = matcher(jointBlockText)))) {
												continue;
											}
										}
										bm = NewBookmark(bm, bm, spans);
										++bl;
									}
									else if (bl == style.Level) {
										// todo 删除重复的文本
										var cb = bm as BookmarkElement;
										var bb = h - cb.Bottom + dh;
										var bt = h - cb.Top;
										var lt = b.Y0 - b.Height * 2 + dh;
										var lb = b.Y1;
										if (cb.Page == p.PageNumber + 1
											&& (bb.IsBetween(lt, lb) || bt.IsBetween(lt, lb) || bt < lt && bb > lb)
											&& (mergeAdjacentTitle || spans[spans.Count - 1].Bound.IsHorizontalNeighbor(b))) {
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
												cb.Bottom = h - lb + pt;
												spans.Add(span);
											}
											continue;
										}
										if (matcher != null) {
											jointBlockText ??= GetLineText(line, block, out hasJointText);
											if ((!(matchLine = matcher(jointBlockText)))) {
												continue;
											}
										}
										bm = NewBookmark(bm, bm.Parent, spans);
									}
									else {
										while (bl > style.Level) {
											bm = bm.ParentBookmark;
											--bl;
										}
										if (matcher != null) {
											jointBlockText ??= GetLineText(line, block, out hasJointText);
											if ((!(matchLine = matcher(jointBlockText)))) {
												continue;
											}
										}
										bm = NewBookmark(bm, bm.Parent, spans);
									}
									var be = bm as BookmarkElement;
									var s = style.Bookmark;
									if (s.IsBold || s.IsItalic) {
										be.TextStyle = s.IsBold && s.IsItalic ? FontStyle.Bold | FontStyle.Italic
											: s.IsBold ? FontStyle.Bold
											: s.IsItalic ? FontStyle.Italic
											: FontStyle.Regular;
									}
									be.Title = matchLine ? jointBlockText : t;
									be.Top = (s.GoToTop ? h + dh : h - b.Y1 + b.Height + dh) + pt;
									be.Bottom = h - b.Y0 + dh + pt;
									be.Action = Constants.ActionType.Goto;
									be.Page = p.PageNumber + 1;
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
							if (matchLine) {
								if (hasJointText) {
									goto NEXT_BLOCK;
								}
								break;
							}
						}
					NEXT_BLOCK:;
					}
				}
			}
			View.Bookmark.Roots = Model.Document.Bookmarks;
			View.Bookmark.RebuildAll(false);
		}

		static BookmarkContainer NewBookmark(BookmarkContainer bm, BookmarkContainer container, List<MuPDF.TextSpan> spans) {
			if (TrimBookmarkText(bm)) {
				bm = container.AppendBookmark();
			}
			spans.Clear();
			return bm;
		}

		static string GetLineText(MuPDF.TextLine line, MuPDF.TextBlock block, out bool jointLines) {
			string t = null;
			jointLines = false;
			foreach (var item in block) {
				if (line == item) {
					t += item.GetText();
				}
				else if (line.Bound.IsHorizontalNeighbor(item.Bound)) {
					jointLines = true;
					t += item.GetText();
				}
			}
			return t;
		}

		static bool TrimBookmarkText(BookmarkContainer bm) {
			if (bm is BookmarkElement b) {
				var t = b.Title;
				var t2 = t.Trim();
				if (t2 != t) {
					b.Title = t2;
				}
				return t2.Length != 0;
			}
			return true;
		}
	}
}
