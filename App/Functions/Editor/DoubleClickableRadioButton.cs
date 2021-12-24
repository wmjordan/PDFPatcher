using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	sealed class DoubleClickableRadioButton : RadioButton
	{
		public DoubleClickableRadioButton () {
			this.SetStyle (ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);
		}

		public new event EventHandler DoubleClick;

		protected override void OnMouseDoubleClick (MouseEventArgs e) {
			base.OnMouseDoubleClick (e);

			if (this.DoubleClick != null) {
				this.DoubleClick (this, e);
			}
		}
	}
}
