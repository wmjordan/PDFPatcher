using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
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
			var infoDoc = controller.Model.Document;
			var mupdf = controller.Model.PdfDocument;
			var t = new FilePath(controller.Model.DocumentPath);
			if (infoDoc == null || infoDoc.DocumentElement == null || t == null) {
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
					if (mupdf != null) {
						writer.WriteLine("#" + Constants.Info.DocumentPath + "=" + mupdf.FilePath);
					}
					writer.WriteLine("#缩进标记=" + indentString);
					writer.WriteLine("#首页页码=1");
					writer.WriteLine();
					OutlineManager.WriteSimpleBookmark(writer, infoDoc.BookmarkRoot, 0, indentString);
				}
			}
			else {
				t = t.EnsureExtension(Constants.FileExtensions.Xml);
				var ws = DocInfoExporter.GetWriterSettings();
				try {
					SaveBookmarkDocument(infoDoc, mupdf, t, ws);
				}
				catch (EncoderFallbackException) when (ws.Encoding != Encoding.UTF8) {
					ws.Encoding = Encoding.UTF8;
					SaveBookmarkDocument(infoDoc, mupdf, t, ws);
				}
				controller.View.DocumentPath = t;

				RecentFileMenuHelper.AddRecentHistoryFile(t);
			}
			controller.Model.Undo.MarkClean();
		}

		static void SaveBookmarkDocument(Model.PdfInfoXmlDocument infoDoc, MuPDF.Document mupdf, FilePath t, XmlWriterSettings ws) {
			using (var writer = XmlWriter.Create(t, ws)) {
				if (mupdf != null) {
					infoDoc.PdfDocumentPath = t.GetRelativePath(mupdf.FilePath);
				}
				infoDoc.WriteContentTo(writer);
			}
		}

		static void SavePdf(Controller controller) {
			var m = controller.Model;
			var vv = controller.View.Viewer;
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
