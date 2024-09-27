using System;
using System.Drawing;
using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	sealed class AutoResizingTextBox : TextBox
	{
		readonly int _MinHeight, _MaxHeight, _Width;

		public AutoResizingTextBox(Rectangle bound, string text) {
			Bounds = bound;
			_Width = bound.Width;
			_MinHeight = bound.Height;
			_MaxHeight = bound.Height * 8;
			MaximumSize = new Size(_Width, _MaxHeight);
			Multiline = true;
			Text = text;
			AcceptsReturn = text.IndexOf('\n') >= 0;
		}

		void ResizeForText() {
			Size = new Size(_Width, Math.Min(Math.Max(PreferredSize.Height, _MinHeight), _MaxHeight));
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
