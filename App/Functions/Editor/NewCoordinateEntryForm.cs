using System;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	sealed partial class NewCoordinateEntryForm : Form
	{
		public string CoordinateName => _CoordinateBox.Text;
		public float AdjustmentValue => (float)_AdjustmentAmountBox.Value;
		public bool IsAbsolute => _AbsoluteBox.Checked;
		public bool IsProportional => _ProportionBox.Checked;

		public NewCoordinateEntryForm() {
			InitializeComponent();
			_CoordinateBox.SelectedIndex = 0;
			this.OnFirstLoad(OnLoad);
		}

		void OnLoad() {
			_AdjustmentAmountBox.Select();
			_AdjustmentAmountBox.Focus();
		}

		void _OkButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}

		void _CancelButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
