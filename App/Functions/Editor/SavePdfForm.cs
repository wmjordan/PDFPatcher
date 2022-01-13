﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions;

public partial class SavePdfForm : Form
{
	private readonly PdfInfoXmlDocument _bookmarkDocument;
	public EventHandler DoWork;
	public EventHandler Finished;

	public SavePdfForm(string sourcePath, string targePath, PdfInfoXmlDocument bookmarkDocument) {
		InitializeComponent();
		if (string.IsNullOrEmpty(sourcePath) == false) {
			_SourceFileBox.Text = sourcePath;
		}

		if (string.IsNullOrEmpty(targePath) == false) {
			_TargetFileBox.Text = targePath;
		}

		_bookmarkDocument = new PdfInfoXmlDocument();
		using (XmlReader r = bookmarkDocument.CreateNavigator().ReadSubtree()) {
			_bookmarkDocument.Load(r);
		}

		_OverwriteBox.CheckedChanged += (s, args) => _TargetFileBox.Enabled = !_OverwriteBox.Checked;
	}

	public string SourceFilePath => _SourceFileBox.Text;
	public string TargetFilePath => _TargetFileBox.Text;

	private void ImportBookmarkForm_Load(object sender, EventArgs e) {
		_TargetFileBox.FileMacroMenu.LoadStandardSourceFileMacros();
		_ConfigButton.Click += (s, args) => {
			AppContext.MainForm.SelectFunctionList(Function.EditorOptions);
		};
	}

	protected void _OkButton_Click(object source, EventArgs args) {
		AppContext.MainForm.ResetWorker();
		PdfInfoXmlDocument doc = _bookmarkDocument;
		string s = _SourceFileBox.Text;
		string t = _OverwriteBox.Checked ? _SourceFileBox.Text : _TargetFileBox.Text;
		if (string.IsNullOrEmpty(s)) {
			FormHelper.ErrorBox(Messages.SourceFileNotFound);
			return;
		}

		if (string.IsNullOrEmpty(t)) {
			FormHelper.ErrorBox(Messages.TargetFileNotSpecified);
			return;
		}

		_SourceFileBox.FileList.AddHistoryItem();
		_TargetFileBox.FileList.AddHistoryItem();

		BackgroundWorker worker = AppContext.MainForm.GetWorker();
		worker.DoWork += (dummy, arg) => {
			DoWork?.Invoke(this, null);
			Worker.PatchDocument(new SourceItem.Pdf(s), t, _bookmarkDocument, AppContext.Importer,
				AppContext.Editor);
		};
		worker.RunWorkerCompleted += (dummy, arg) => {
			if (Finished != null) {
				Finished(this, arg);
			}
		};
		worker.RunWorkerAsync();

		DialogResult = DialogResult.OK;
		Close();
	}

	protected void _CancelButton_Click(object source, EventArgs args) {
		DialogResult = DialogResult.Cancel;
		Close();
	}
}