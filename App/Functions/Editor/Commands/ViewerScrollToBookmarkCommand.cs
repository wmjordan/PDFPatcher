using System;
using BrightIdeasSoftware;
using CLR;
using PDFPatcher.Model;

namespace PDFPatcher.Functions.Editor;

/// <summary>
/// 将阅读器的视图滚动到和书签编辑器选中书签对应的位置。
/// </summary>
sealed class ViewerScrollToBookmarkCommand : IEditorCommand
{
	public void Process(Controller context, params string[] parameters) {
		var b = context.View.Bookmark;
		var i = b.GetFirstSelectedIndex();
		if (i == -1
			|| context.Model.LockDownViewer
			|| b.GetModelObject(i) is not BookmarkElement el
			|| b.SelectedIndices.Count != 1
			|| el.Page <= 0
			|| el.Action != Constants.ActionType.Goto) {
			return;
		}
		var v = context.View.Viewer;
		if (context.Model.PdfDocument is not null
			&& el.Page.IsBetween(1, v.Document.PageCount)) {
			var pb = v.GetPageBound(el.Page);
			v.ScrollToPosition(new PagePosition(el.Page,
				v.HorizontalFlow ? Math.Min(el.Left, pb.Width) : 0,
				v.HorizontalFlow || el.Top == 0 ? 0 : el.Top.Clamp(pb.Top, pb.Bottom),
				0, 0, true)
			);
		}
	}
}
