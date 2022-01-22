using System;
using System.Windows.Forms;

namespace PDFPatcher.Functions;

public partial class ShiftPageNumberEntryForm : Form
{
    public ShiftPageNumberEntryForm() {
        InitializeComponent();
    }

    internal int ShiftNumber => (int)_ShiftNumberBox.Value;

    private void ShiftPageNumberEntryForm_Load(object sender, EventArgs e) {
    }

    protected void _OkButton_Click(object source, EventArgs args) {
        DialogResult = DialogResult.OK;
        Close();
    }

    protected void _CancelButton_Click(object source, EventArgs args) {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}