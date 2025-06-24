using System;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	sealed partial class SavePdfForm : Form
	{
		readonly PdfInfoXmlDocument _bookmarkDocument;
		public EventHandler DoWork;
		public Action<bool> Finished;

		public string SourceFilePath => _SourceFileBox.Text;
		public string TargetFilePath => _TargetFileBox.Text;

		SavePdfForm() {
			InitializeComponent();
			_OptionsBox.Options = AppContext.Editor;
			_OptionsBox.ForEditor = true;
		}

		public SavePdfForm(string sourcePath, string targetPath, PdfInfoXmlDocument bookmarkDocument) : this() {
			if (!String.IsNullOrEmpty(sourcePath)) {
				_SourceFileBox.Text = sourcePath;
				_SourceFileBox.Enabled = false;
			}
			if (!String.IsNullOrEmpty(targetPath)) {
				_TargetFileBox.Text = targetPath;
			}
			else {
				targetPath = sourcePath;
			}
			if (!targetPath.IsNullOrWhiteSpace()) {
				var p = new FilePath(targetPath);
				_TargetFileBox.FileDialog.FileName = p.FileName;
				_TargetFileBox.FileDialog.InitialDirectory = p.Directory;
			}
			_bookmarkDocument = new PdfInfoXmlDocument();
			using (var r = bookmarkDocument.CreateNavigator().ReadSubtree()) {
				_bookmarkDocument.Load(r);
			}

			_OverwriteBox.CheckedChanged += (s, args) => _TargetFileBox.Enabled = !_OverwriteBox.Checked;
			if (AppContext.Editor.DefaultOverwriteDocument) {
				_OverwriteBox.Checked = true;
			}
		}

		void ImportBookmarkForm_Load(object sender, EventArgs e) {
			_TargetFileBox.FileMacroMenu.LoadStandardSourceFileMacros();
			_OptionsBox.OnLoad();
		}

		void _OkButton_Click(object source, EventArgs args) {
			_OptionsBox.Apply();
			AppContext.MainForm.ResetWorker();
			var doc = _bookmarkDocument;
			var s = _SourceFileBox.Text;
			var t = _OverwriteBox.Checked ? _SourceFileBox.Text : _TargetFileBox.Text;
			if (String.IsNullOrEmpty(s)) {
				FormHelper.ErrorBox(Messages.SourceFileNotFound);
				return;
			}
			if (String.IsNullOrEmpty(t)) {
				FormHelper.ErrorBox(Messages.TargetFileNotSpecified);
				return;
			}
			_SourceFileBox.FileList.AddHistoryItem();
			_TargetFileBox.FileList.AddHistoryItem();

			var worker = AppContext.MainForm.GetWorker();
			worker.DoWork += (dummy, arg) => {
				DoWork?.Invoke(this, null);
				arg.Result = Processor.Worker.PatchDocument(new SourceItem.Pdf(s), t, _bookmarkDocument, AppContext.Importer, AppContext.Editor, true);
			};
			worker.RunWorkerCompleted += (dummy, arg) => Finished?.Invoke(arg.Result.CastOrDefault<bool>());
			worker.RunWorkerAsync();

			DialogResult = DialogResult.OK;
			Close();
		}

		void _CancelButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.Cancel;
			_OptionsBox.Apply();
			Close();
		}
	}
}
