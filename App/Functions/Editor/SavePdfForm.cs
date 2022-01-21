using System;
using System.Windows.Forms;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	public partial class SavePdfForm : Form
	{
		readonly PdfInfoXmlDocument _bookmarkDocument;
		public EventHandler DoWork;
		public EventHandler Finished;

		public string SourceFilePath => _SourceFileBox.Text;
		public string TargetFilePath => _TargetFileBox.Text;

		public SavePdfForm(string sourcePath, string targetPath, PdfInfoXmlDocument bookmarkDocument) {
			InitializeComponent();
			if (String.IsNullOrEmpty(sourcePath) == false) {
				_SourceFileBox.Text = sourcePath;
			}
			if (String.IsNullOrEmpty(targetPath) == false) {
				_TargetFileBox.Text = targetPath;
			}
			_bookmarkDocument = new PdfInfoXmlDocument();
			using (var r = bookmarkDocument.CreateNavigator().ReadSubtree()) {
				_bookmarkDocument.Load(r);
			}

			_OverwriteBox.CheckedChanged += (s, args) => _TargetFileBox.Enabled = !_OverwriteBox.Checked;
		}

		private void ImportBookmarkForm_Load(object sender, EventArgs e) {
			_TargetFileBox.FileMacroMenu.LoadStandardSourceFileMacros();
			_ConfigButton.Click += (s, args) => {
				AppContext.MainForm.SelectFunctionList(Function.EditorOptions);
			};
		}

		protected void _OkButton_Click(Object source, EventArgs args) {
			AppContext.MainForm.ResetWorker();
			var doc = _bookmarkDocument;
			var s = _SourceFileBox.Text;
			var t = _OverwriteBox.Checked ? _SourceFileBox.Text : _TargetFileBox.Text;
			if (String.IsNullOrEmpty(s)) {
				Common.FormHelper.ErrorBox(Messages.SourceFileNotFound);
				return;
			}
			if (String.IsNullOrEmpty(t)) {
				Common.FormHelper.ErrorBox(Messages.TargetFileNotSpecified);
				return;
			}
			_SourceFileBox.FileList.AddHistoryItem();
			_TargetFileBox.FileList.AddHistoryItem();

			var worker = AppContext.MainForm.GetWorker();
			worker.DoWork += (dummy, arg) => {
				DoWork?.Invoke(this, null);
				Processor.Worker.PatchDocument(new SourceItem.Pdf(s), t, _bookmarkDocument, AppContext.Importer, AppContext.Editor);
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

		protected void _CancelButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
