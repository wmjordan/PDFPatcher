using System;
using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	sealed partial class ZoomRateEntryForm : Form
	{
		internal string ZoomRate => _ZoomRateBox.Text;

		public ZoomRateEntryForm() {
			InitializeComponent();
		}

		void _OkButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.OK;
			Close();
		}

		void _CancelButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
