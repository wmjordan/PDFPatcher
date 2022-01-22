using System;
using System.Windows.Forms;
using MuPdfSharp;
using PDFPatcher.Common;

namespace PDFPatcher.Functions;

public partial class InsertPageLabelForm : DraggableForm
{
	public InsertPageLabelForm() {
		InitializeComponent();
		_NumericStyleBox.AddRange(Constants.PageLabelStyles.Names).Select(0);
		_RemoveLabelButton.Enabled = false;
	}

	public int PageNumber {
		get => _PageNumberBox.Text.ToInt32();
		set => _PageNumberBox.Text = value.ToText();
	}

	internal PageLabel PageLabel => new(PageNumber - 1, (int)_StartAtBox.Value, _PrefixBox.Text,
		(PageLabelStyle)Constants.PageLabelStyles.PdfValues[_NumericStyleBox.SelectedIndex]);

	private void InsertPageLabelForm_Load(object sender, EventArgs e) {
		_CancelButton.Click += (_, _) => {
			DialogResult = DialogResult.Cancel;
			Close();
		};
		_OkButton.Click += (_, _) => {
			DialogResult = DialogResult.OK;
			Close();
		};
		_RemoveLabelButton.Click += (_, _) => {
			DialogResult = DialogResult.Abort;
			Close();
		};
	}

	internal void SetValues(PageLabel label) {
		int s = Array.IndexOf(Constants.PageLabelStyles.PdfValues, (char)label.NumericStyle);
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