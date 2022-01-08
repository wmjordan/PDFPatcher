using System;
using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	public partial class ShiftPageNumberEntryForm : Form
	{
		internal int ShiftNumber => (int)_ShiftNumberBox.Value;

		public ShiftPageNumberEntryForm() {
			InitializeComponent();
		}

		private void ShiftPageNumberEntryForm_Load(object sender, EventArgs e) {

		}

		protected void _OkButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.OK;
			Close();
		}

		protected void _CancelButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
