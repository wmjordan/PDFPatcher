using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PDFPatcher.Functions;

internal sealed class DoubleClickableRadioButton : RadioButton
{
	public DoubleClickableRadioButton() {
		SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);
	}

	public new event EventHandler DoubleClick;

	protected override void OnMouseDoubleClick(MouseEventArgs e) {
		base.OnMouseDoubleClick(e);

		if (DoubleClick != null) {
			DoubleClick(this, e);
		}
	}
}