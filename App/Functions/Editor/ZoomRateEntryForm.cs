using System;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	sealed partial class ZoomRateEntryForm : Form
	{
		internal string ZoomRate => _ZoomRateBox.Text;

		public ZoomRateEntryForm() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
		}

		void OnLoad() {
			_ZoomRateBox.Select();
			_ZoomRateBox.Focus();
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
