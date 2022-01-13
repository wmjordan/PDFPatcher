﻿using System;
using System.Windows.Forms;

namespace PDFPatcher.Functions;

public partial class ZoomRateEntryForm : Form
{
	public ZoomRateEntryForm() {
		InitializeComponent();
	}

	internal string ZoomRate => _ZoomRateBox.Text;

	private void ZoomRateEntryForm_Load(object sender, EventArgs e) {
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