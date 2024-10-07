using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PDFPatcher
{
	public class HistoryComboBox : ComboBox
	{
		public int MaxItemCount { get; set; }
		public IList<string> Contents { get; set; }

		public HistoryComboBox() {
			MaxItemCount = 16;
			DropDown += (s, args) => {
				Items.Clear();
				if (Contents == null) {
					return;
				}
				Items.AddRange(Contents.ToArray());
			};
		}

		internal void AddHistoryItem() {
			AddHistoryItem(Text);
		}

		internal void AddHistoryItem(string text) {
			if (text.Length == 0) {
				return;
			}
			var i = IndexOf(text);
			if (i == 0) {
				return;
			}
			if (i != -1) {
				RemoveAt(i);
			}
			Insert(0, text);
			while (Contents.Count > MaxItemCount) {
				RemoveAt(Contents.Count - 1);
			}
			Text = text;
		}

		int IndexOf(string o) {
			var l = Contents;
			if (l != null) {
				return l.IndexOf(o);
			}
			else {
				return Items.IndexOf(o);
			}
		}

		void RemoveAt(int i) {
			var l = Contents;
			if (l != null) {
				l.RemoveAt(i);
			}
			else {
				Items.RemoveAt(i);
			}
		}

		void Insert(int i, string o) {
			var l = Contents;
			if (l != null) {
				l.Insert(i, o);
			}
			else {
				Items.Insert(i, o);
			}
		}
	}
}
