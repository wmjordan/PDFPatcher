using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions.Editor
{
	sealed class InsertBookmarkCommand : IEditorCommand
	{
		static readonly Regex __RemoveOcrWhiteSpace = new Regex(@"\s{2,}", RegexOptions.Compiled);
		static InsertBookmarkForm _dialog;

		public void Process(Controller controller, params string[] parameters) {
			var v = controller.View.Viewer;
			BookmarkAtClientPoint(controller, v.TransposeVirtualImageToClient(v.PinPoint.X, v.PinPoint.Y));
		}

		private void BookmarkAtClientPoint(Controller controller, Point cp) {
			var v = controller.View.Viewer;
			var pp = v.TransposeClientToPagePosition(cp.X, cp.Y);
			if (pp.Page == 0) {
				return;
			}
			if (Control.ModifierKeys == Keys.Control) {
				v.PinPoint = v.PointToImage(cp);
				ShowInsertBookmarkDialog(controller, cp, new EditModel.Region(pp, null, EditModel.TextSource.Empty));
				return;
			}
			var r = controller.CopyText(cp, pp);
			ShowInsertBookmarkDialog(controller, cp, r);
		}

		private void ShowInsertBookmarkDialog(Controller controller, Point mousePoint, EditModel.Region region) {
			var p = region.Position;
			if (p.Page == 0) {
				return;
			}
			var f = GetDialog(controller);
			var v = controller.View.Viewer;
			var vp = v.GetImageViewPort();
			Point fp;
			var sr = v.SelectionRegion;
			if (sr != RectangleF.Empty) {
				fp = v.TransposeVirtualImageToClient(sr.Left, sr.Top);
				if (v.HorizontalFlow) {
					fp.X += sr.Width.ToInt32() + 20;
				}
				else {
					fp.Y -= f.Height + 20;
				}
			}
			else {
				fp = new Point(mousePoint.X + 20, mousePoint.Y - f.Height);
			}
			var l = v.PointToScreen(fp);
			if (l.Y < 0) {
				l.Y = l.Y + (int)sr.Height + f.Height + 40;
				if (l.Y + f.Height > Screen.PrimaryScreen.WorkingArea.Height) {
					l.Y = Screen.PrimaryScreen.WorkingArea.Height - f.Height;
				}
			}
			if (l.X < v.PointToScreen(Point.Empty).X) {
				l.X = v.PointToScreen(Point.Empty).X;
			}
			f.Location = l;
			f.TargetPosition = p.PageY;
			if (String.IsNullOrEmpty(region.Text) == false) {
				f.Title = __RemoveOcrWhiteSpace.Replace(region.Text, " ").Trim();
			}
			f.Comment = region.LiteralTextSource;
			f.Show();
			f.TargetPageNumber = p.Page;
		}

		private InsertBookmarkForm GetDialog(Controller controller) {
			if (_dialog != null && _dialog.IsDisposed == false) {
				_dialog.Controller = controller;
				return _dialog;
			}
			_dialog = new InsertBookmarkForm {
				Controller = controller
			};
			_dialog.OkClicked += (object sender, EventArgs e) => {
				var f = (InsertBookmarkForm)sender;
				var c = f.Controller;
				var t = f.Title;
				if (string.IsNullOrEmpty(t)) {
					FormHelper.ErrorBox("书签标题不能为空。");
					return;
				}
				c.Model.LockDownViewer = true;
				c.InsertBookmark(t, f.TargetPageNumber, f.TargetPosition, (InsertBookmarkPositionType)f.InsertMode);
				c.Model.LockDownViewer = false;
			};
			_dialog.Deactivate += (s, args) => {
				(s as Form).Visible = false;
			};
			_dialog.VisibleChanged += (s, args) => {
				var f = (InsertBookmarkForm)s;
				var c = f.Controller;
				c.View.Viewer.ShowPinPoint = f.Visible;
			};
			return _dialog;
		}

	}
}
