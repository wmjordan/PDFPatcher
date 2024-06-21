using System;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	sealed partial class ShiftPageNumberEntryForm : Form
	{
		internal int ShiftNumber => (int)_ShiftNumberBox.Value;

		public ShiftPageNumberEntryForm() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
		}

		void OnLoad() {
			_ShiftNumberBox.Select();
			_ShiftNumberBox.Focus();
			_ShiftNumberBox.Maximum = 299999;
			_ShiftNumberBox.Minimum = -299999;
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
