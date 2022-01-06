using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor
{
	internal sealed class EditModel
	{
		public EditModel() {
			Undo = new UndoManager();
			TitleStyles = new List<AutoBookmarkStyle>();
		}

		internal bool IsLoadingDocument { get; set; }
		internal PdfInfoXmlDocument Document { get; set; }
		internal bool LockDownViewer { get; set; }
		internal bool InsertBookmarkWithOcrOnly { get; set; }
		internal UndoManager Undo { get; }
		internal string DocumentPath { get; set; }
		internal string LastSavedPdfPath { get; set; }
		internal MuPdfSharp.MuDocument PdfDocument { get; set; }
		internal List<AutoBookmarkStyle> TitleStyles { get; }
		internal string GetPdfFilePath() {
			if (DocumentPath == null) {
				return null;
			}
			var s = FileHelper.HasExtension(DocumentPath, Constants.FileExtensions.Pdf) ? DocumentPath : null;
			if (string.IsNullOrEmpty(s)) {
				s = Document.PdfDocumentPath;
				if (Path.IsPathRooted(s) == false) {
					s = Path.Combine(Path.GetDirectoryName(DocumentPath), s);
				}
			}
			if (File.Exists(s) == false) {
				s = null;
			}
			return s;
		}

		internal sealed class Region
		{
			internal PagePosition Position { get; private set; }
			internal string Text { get; private set; }
			internal TextSource TextSource { get; private set; }
			internal string LiteralTextSource {
				get {
					switch (TextSource) {
						case TextSource.Empty: return "当前位置不包含文本";
						case TextSource.Text: return "已自动匹配文本层文本";
						case TextSource.OcrText: return "已自动识别图像文本";
						case TextSource.OcrError: return "当前页面不包含可识别文本，或识别过程出错";
						default:
							throw new System.IndexOutOfRangeException("TextSource");
					}
				}
			}

			public Region(PagePosition position, string text, TextSource source) {
				Position = position;
				Text = text;
				TextSource = source;
			}
		}

		internal enum TextSource
		{
			Empty, Text, OcrText, OcrError
		}
		internal sealed class AutoBookmarkStyle
		{
			internal readonly MuPdfSharp.MuFont Font;
			internal readonly string FontName;
			internal readonly int FontSize;
			internal readonly BookmarkSettings Style;
			internal MatchPattern MatchPattern;

			internal int Level;

			public AutoBookmarkStyle(int level, MuPdfSharp.MuFont font, int fontSize) {
				Level = level;
				Font = font;
				FontName = PdfDocumentFont.RemoveSubsetPrefix(font.Name);
				FontSize = fontSize;
				Style = new BookmarkSettings();
			}
		}
	}

	internal interface IEditView
	{
		bool AffectsDescendantBookmarks { get; }
		ToolStripSplitButton UndoButton { get; }
		AutoBookmarkForm AutoBookmark { get; }
		BookmarkEditorView Bookmark { get; }
		PdfViewerControl Viewer { get; }
		ToolStrip ViewerToolbar { get; }
		ToolStrip BookmarkToolbar { get; }
		SplitContainer MainPanel { get; }
		string DocumentPath { get; set; }
	}

}