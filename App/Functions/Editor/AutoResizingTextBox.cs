using System;
using System.Drawing;
using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	sealed class AutoResizingTextBox : TextBox
	{
		readonly int _MinHeight, _MaxHeight, _Width;

		public AutoResizingTextBox(Rectangle bound, string text, Control parent = null) {
			Bounds = bound;
			_Width = bound.Width;
			_MinHeight = bound.Height;
			_MaxHeight = bound.Height * 8;
			MaximumSize = new Size(_Width, _MaxHeight);
			Multiline = true;
			Parent = parent;
			AcceptsReturn = text?.IndexOf('\n') >= 0;
			Text = text;
		}

		void ResizeForText() {
			using (var g = CreateGraphics()) {
				// NOTE: 当文本无处换行但又溢出文本框时，PreferredSize 的计算有误，故改用 MeasureString
				var height = g.MeasureString(Text, Font, _Width, StringFormat.GenericDefault).Height + Margin.Vertical;
				Size = new Size(_Width, Math.Min(Math.Max((int)height, _MinHeight), _MaxHeight));
				if (Parent?.ClientRectangle.Height < Bottom) {
					Location = new Point(Location.X, Location.Y - (Bottom - Parent.ClientRectangle.Height));
				}
				// NOTE：如果解除下面的注释，可能会在编辑过程中抛出 ObjectDisposedException
				//if (height > _MaxHeight) {
				//	ScrollBars = ScrollBars.Vertical;
				//}
			}
		}

		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			ResizeForText();
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			if (e.KeyData == (Keys.Shift | Keys.Enter)) {
				AcceptsReturn = true;
			}
			else if (e.KeyData == (Keys.Control | Keys.Enter)) {
				e.Handled = true;
				e.SuppressKeyPress = true;
				Parent.Focus();
			}
			base.OnKeyDown(e);
		}
	}
}
