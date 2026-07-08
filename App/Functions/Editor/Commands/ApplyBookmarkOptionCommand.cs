using System;
using CLR;
using MuPDF;
using MuPDF.Extensions;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor;

/// <summary>
/// 更改书签编辑栏的字体（第1个参数）和尺寸（第2个参数）
/// </summary>
sealed class ApplyBookmarkOptionCommand : IEditorCommand
{
	public void Process(Controller context, params string[] parameters) {
		var bookmark = context.View.Bookmark;
		var viewer = context.View.Viewer;
		var currFont = bookmark.Font;
		var option = AppContext.Reader;

		var font = option.BookmarkFont;
		var fontSize = option.BookmarkFontSize != 0 ? option.BookmarkFontSize.Clamp(7f, 72f) : 0;
		var keepFont = font.IsNullOrWhiteSpace() || bookmark.Font.FontFamily.Name == font;
		var keepSize = fontSize == 0 || Math.Abs(fontSize - currFont.Size) < 1f;
		if (!keepFont || !keepSize) {
			bookmark.Font = new System.Drawing.Font(keepFont ? currFont.FontFamily.Name : font,
				keepSize ? currFont.Size : fontSize);
		}

		var doc = context.Model.PdfDocument;
		if (doc is not null) {
			context.View.MainPanel.Panel1Collapsed = AppContext.Reader.BookmarkState switch {
				BookmarkState.Auto => !doc.Trailer.Locate(PdfNames.Root, PdfNames.Outlines).IsDictionary,
				BookmarkState.Show => false,
				BookmarkState.Hide => true,
				_ => false
			};
		}
	}
}
