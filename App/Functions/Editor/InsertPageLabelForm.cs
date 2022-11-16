using System;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	sealed partial class InsertPageLabelForm : DraggableForm
	{
		public int PageNumber {
			get => _PageNumberBox.Text.ToInt32();
			set => _PageNumberBox.Text = value.ToText();
		}

		internal MuPdfSharp.PageLabel PageLabel => new MuPdfSharp.PageLabel(PageNumber - 1, (int)_StartAtBox.Value, _PrefixBox.Text, (MuPdfSharp.PageLabelStyle)Constants.PageLabelStyles.PdfValues[_NumericStyleBox.SelectedIndex]);

		public InsertPageLabelForm() {
			InitializeComponent();
			_NumericStyleBox.AddRange(Constants.PageLabelStyles.Names).Select(0);
			_RemoveLabelButton.Enabled = false;
		}

		void InsertPageLabelForm_Load(object sender, EventArgs e) {
			_CancelButton.Click += (s, args) => {
				DialogResult = DialogResult.Cancel;
				Close();
			};
			_OkButton.Click += (s, args) => {
				DialogResult = DialogResult.OK;
				Close();
			};
			_RemoveLabelButton.Click += (s, args) => {
				DialogResult = DialogResult.Abort;
				Close();
			};
		}

		internal void SetValues(MuPdfSharp.PageLabel label) {
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
