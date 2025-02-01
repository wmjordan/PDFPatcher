using System;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	sealed partial class ShiftPageNumberEntryForm : Form
	{
		internal int ShiftNumber => (int)_ShiftNumberBox.Value;
		internal bool TakeFollowing {
			get => _TakeFollowingBox.Checked;
			set => _TakeFollowingBox.Checked = value;
		}

		public ShiftPageNumberEntryForm() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
		}

		void OnLoad() {
			_ShiftNumberBox.Select();
			_ShiftNumberBox.Select(0, 1);
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
