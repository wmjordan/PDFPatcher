﻿using System.Collections.Generic;
using System.IO;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor
{
	internal sealed class EditModel
	{
		public EditModel() {
			Undo = new UndoManager();
			TitleStyles = [];
			Document = new PdfInfoXmlDocument();
		}

		internal bool IsLoadingDocument { get; set; }
		internal PdfInfoXmlDocument Document { get; set; }
		internal bool LockDownViewer { get; set; }
		internal bool InsertBookmarkWithOcrOnly { get; set; }
		internal UndoManager Undo { get; }
		internal string DocumentPath { get; set; }
		internal string LastSavedPdfPath { get; set; }
		MuPDF.Document _Document;
		internal MuPDF.Document PdfDocument {
			get => _Document;
			set {
				if (_Document != value) {
					_Document = value;
					_PageLabels = null;
				}
			}
		}
		MuPDF.PageLabelCollection _PageLabels;
		internal MuPDF.PageLabelCollection PageLabels {
			get {
				if (_PageLabels == null && PdfDocument != null) {
					_PageLabels = new MuPDF.PageLabelCollection(PdfDocument);
				}
				return _PageLabels;
			}
		}
		internal List<AutoBookmarkSettings> TitleStyles { get; }
		internal string GetPdfFilePath() {
			if (DocumentPath == null) {
				return null;
			}
			var s = FileHelper.HasExtension(DocumentPath, Constants.FileExtensions.Pdf) ? DocumentPath : null;
			if (string.IsNullOrEmpty(s)) {
				s = Document.PdfDocumentPath;
				if (!Path.IsPathRooted(s)) {
					s = Path.Combine(Path.GetDirectoryName(DocumentPath), s);
				}
			}
			if (!File.Exists(s)) {
				s = null;
			}
			return s;
		}

		internal sealed class Region
		{
			internal PagePosition Position { get; }
			internal string Text { get; }
			internal TextSource TextSource { get; }
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
		internal sealed class AutoBookmarkSettings
		{
			internal readonly string FontName;
			internal readonly int FontSize;
			internal readonly BookmarkSettings Bookmark;
			internal MatchPattern MatchPattern;

			internal int Level;

			public AutoBookmarkSettings(int level, string fontName, int fontSize) {
				Level = level;
				FontName = fontName;
				FontSize = fontSize;
				Bookmark = new BookmarkSettings();
			}
		}
	}

}