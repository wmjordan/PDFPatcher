using System;
using System.IO;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor
{
	sealed class SaveDocumentCommand : IEditorCommand
	{
		readonly bool _showDialog, _saveAsBookmark;
		public SaveDocumentCommand(bool showDialog, bool saveAsBookmark) {
			_saveAsBookmark = saveAsBookmark;
			_showDialog = showDialog;
		}
		public void Process(Controller controller, params string[] parameters) {
			if (_saveAsBookmark) {
				SaveBookmark(controller, _showDialog);
			}
			else {
				SavePdf(controller);
			}
		}

		static void SaveBookmark(Controller controller, bool showDialog) {
			var idoc = controller.Model.Document;
			var mudoc = controller.Model.PdfDocument;
			var t = new FilePath(controller.Model.DocumentPath);
			if (idoc == null || idoc.DocumentElement == null || t == null) {
				return;
			}

			if (t.HasExtension(Constants.FileExtensions.Xml) == false
				|| showDialog) {
				using (var d = new SaveFileDialog() {
					DefaultExt = Constants.FileExtensions.Xml,
					Title = "指定保存文件的路径",
					Filter = Constants.FileExtensions.XmlFilter + "|" + Constants.FileExtensions.TxtFilter
				}) {
					if (t.ExistsFile) {
						d.InitialDirectory = t.Directory;
						d.FileName = t.FileNameWithoutExtension;
					}
					if (d.ShowDialog() != DialogResult.OK) {
						return;
					}
					t = d.FileName;
				}
			}

			if (t.HasExtension(Constants.FileExtensions.Txt)) {
				using (var writer = new StreamWriter(t)) {
					const string indentString = "\t";
					writer.WriteLine("#版本=" + Constants.InfoDocVersion);
					if (mudoc != null) {
						writer.WriteLine("#" + Constants.Info.DocumentPath + "=" + mudoc.FilePath);
					}
					writer.WriteLine("#缩进标记=" + indentString);
					writer.WriteLine("#首页页码=1");
					writer.WriteLine();
					OutlineManager.WriteSimpleBookmark(writer, idoc.BookmarkRoot, 0, indentString);
				}
			}
			else {
				t = t.EnsureExtension(Constants.FileExtensions.Xml);
				using (var writer = System.Xml.XmlWriter.Create(t, DocInfoExporter.GetWriterSettings())) {
					if (mudoc != null) {
						idoc.PdfDocumentPath = t.GetRelativePath(mudoc.FilePath);
					}
					idoc.WriteContentTo(writer);
				}
				controller.View.DocumentPath = t;

				RecentFileMenuHelper.AddRecentHistoryFile(t);
			}
			controller.Model.Undo.MarkClean();
		}

		static void SavePdf(Controller controller) {
			var m = controller.Model;
			var vv = controller.View.Viewer;
			if (m.Document == null) {
				FormHelper.ErrorBox("尚未加载书签文档。");
				return;
			}
			using (var f = new SavePdfForm(m.GetPdfFilePath(), m.LastSavedPdfPath, m.Document)) {
				f.DoWork = (s, args) => vv.CloseFile();
				f.Finished = (success) => {
					vv.Reopen();
					vv.Enabled = true;
					if (success) { m.Undo.MarkClean(); }
				};

				if (f.ShowDialog() == DialogResult.OK) {
					vv.Enabled = false;
					m.Document.PdfDocumentPath = f.SourceFilePath;
					m.LastSavedPdfPath = f.TargetFilePath;
				}
			}
		}

	}
}
