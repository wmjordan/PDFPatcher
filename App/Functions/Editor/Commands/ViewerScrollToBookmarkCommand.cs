using System;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions.Editor
{
	/// <summary>
	/// 将阅读器的视图滚动到和书签编辑器选中书签对应的位置。
	/// </summary>
	sealed class ViewerScrollToBookmarkCommand : IEditorCommand
	{
		public void Process(Controller context, params string[] parameters) {
			BookmarkElement el;
			var b = context.View.Bookmark;
			var i = b.GetFirstSelectedIndex();
			if (i == -1) {
				return;
			}
			el = b.GetModelObject(i) as BookmarkElement;
			if (context.Model.LockDownViewer == false
				&& b.SelectedIndices.Count == 1
				&& (i = el.Page) > 0
				&& el.Action == Constants.ActionType.Goto) {
				var v = context.View.Viewer;
				if (context.Model.PdfDocument != null && el.Page > 0 && el.Page <= v.Document.PageCount) {
					var pb = v.GetPageBound(el.Page);
					v.ScrollToPosition(new Editor.PagePosition(el.Page,
						v.HorizontalFlow ? el.Left > pb.Width ? pb.Width : el.Left : 0,
						v.HorizontalFlow || el.Top == 0 ? 0 : el.Top.LimitInRange(pb.Top, pb.Bottom),
						0, 0, true)
					);
				}
			}
		}
	}
}
