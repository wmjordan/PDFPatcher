using System;
using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	sealed partial class ShiftPageNumberEntryForm : Form
	{
		internal int ShiftNumber => (int)_ShiftNumberBox.Value;

		public ShiftPageNumberEntryForm() {
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
