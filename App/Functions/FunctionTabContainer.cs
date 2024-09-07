using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	class FunctionTabContainer : CustomTabControl
	{
		bool _MiddleClickCloseTab, _DoubleClickCloseTab;

		public CancelEventHandler BeforeCloseTab;

		public Control FirstControlInActiveTab {
			get {
				var t = SelectedTab;
				return t == null || t.HasChildren == false ? null : t.Controls[0];
			}
		}

		public bool MiddleClickCloseTab { get => _MiddleClickCloseTab; set => _MiddleClickCloseTab = value; }
		public bool DoubleClickCloseTab { get => _DoubleClickCloseTab; set => _DoubleClickCloseTab = value; }

		public bool SafeCloseTab(TabPage tabPage = null) {
			var t = tabPage ?? SelectedTab;
			if (t is null) {
				return false;
			}
			Control c;
			if (t.HasChildren == false
				|| (c = t.Controls[0]) is ITabContent tc && tc.CanClose == false
				|| (c is IDocumentEditor editor
					&& editor.IsDirty
					&& AppContext.MainForm.ConfirmYesBox(Messages.ConfirmCloseDirtyDocument) == false)) {
				return false;
			}
			var i = TabPages.IndexOf(tabPage);
			var n = TabCount;
			if (i == 0 && n > 1) {
				SelectedIndex = 1;
			}
			else if (i < n) {
				SelectedIndex = i - 1;
			}
			TabPages.RemoveAt(i);
			t.Dispose();
			return true;
		}

		public IEnumerable<TObject> GetPrimaryControlsInTabs<TObject>() {
			foreach (TabPage item in TabPages) {
				if (item.Controls.Count != 0 && item.Controls[0] is TObject obj) {
					yield return obj;
				}
			}
		}

		protected override void OnMouseClick(MouseEventArgs e) {
			base.OnMouseClick(e);
			if (e.Button == MouseButtons.Middle && _MiddleClickCloseTab) {
				CloseTabOnMouseEvent(e);
			}
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			if (_DoubleClickCloseTab) {
				CloseTabOnMouseEvent(e);
			}
		}

		void CloseTabOnMouseEvent(MouseEventArgs args) {
			for (int i = TabCount - 1; i >= 0; i--) {
				if (GetTabRect(i).Contains(args.Location) == false) {
					continue;
				}
				SafeCloseTab(TabPages[i]);
			}
		}

	}
}
