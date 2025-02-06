using System;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor
{
	sealed partial class InsertPageLabelForm : DraggableForm
	{
		public int PageNumber {
			get => _PageNumberBox.Text.ToInt32();
			set => _PageNumberBox.Text = value.ToText();
		}

		internal MuPDF.PageLabel PageLabel => new MuPDF.PageLabel(PageNumber - 1, (int)_StartAtBox.Value, _PrefixBox.Text, (MuPDF.PageLabelStyle)Constants.PageLabelStyles.PdfValues[_NumericStyleBox.SelectedIndex]);

		internal EditModel Model { get; set; }
		internal ViewerControl Viewer { get; set; }

		public InsertPageLabelForm() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
		}

		void OnLoad() {
			_NumericStyleBox.AddRange(Constants.PageLabelStyles.Names).Select(0);
			_RemoveLabelButton.Enabled = false;
			_CancelButton.Click += HandleCommandButtonClick;
			_OkButton.Click += HandleCommandButtonClick;
			_RemoveLabelButton.Click += HandleCommandButtonClick;
		}

		protected override void OnFormClosed(FormClosedEventArgs e) {
			base.OnFormClosed(e);
			Model = null;
			Viewer = null;
		}

		void HandleCommandButtonClick(object sender, EventArgs e) {
			DialogResult = sender == _CancelButton ? DialogResult.Cancel
				: sender == _OkButton ? DialogResult.OK
				: sender == _RemoveLabelButton ? DialogResult.Abort
				: DialogResult.None;
			Close();
		}

		internal void SetValues(MuPDF.PageLabel label) {
			var s = Array.IndexOf(Constants.PageLabelStyles.PdfValues, (char)label.NumericStyle);
			_NumericStyleBox.Select(s);
			_PrefixBox.Text = label.Prefix;
			_StartAtBox.SetValue(label.StartAt);
			_RemoveLabelButton.Enabled = true;
		}

		protected override void OnDeactivate(EventArgs e) {
			base.OnDeactivate(e);
			Close();
		}

	}
}
