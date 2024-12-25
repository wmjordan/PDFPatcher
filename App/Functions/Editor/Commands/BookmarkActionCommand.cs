using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor
{
	sealed class BookmarkActionCommand : IEditorCommand
	{
		readonly string _viewType;

		public BookmarkActionCommand(string viewType) {
			_viewType = viewType;
		}

		public void Process(Controller controller, params string[] parameters) {
			var b = controller.View.Bookmark;
			if (b.FocusedItem == null) {
				return;
			}
			switch (_viewType) {
				case Constants.DestinationAttributes.ViewType.XYZ:
					using (var form = new ZoomRateEntryForm()) {
						if (form.ShowDialog() != DialogResult.OK) {
							return;
						}
						var z = form.ZoomRate;
						float r;
						if (z == Constants.Coordinates.Unchanged) {
							controller.ProcessBookmarks(new ChangeZoomRateProcessor(null));
						}
						else if (z.TryParse(out r)) {
							controller.ProcessBookmarks(new ChangeZoomRateProcessor(r));
						}
					}
					break;
				case Constants.Coordinates.Unchanged:
					controller.ProcessBookmarks(new ChangeZoomRateProcessor(null));
					break;
				case "_ChangeCoordinates":
					using (var f = new NewCoordinateEntryForm()) {
						if (f.ShowDialog() == DialogResult.OK) {
							controller.ProcessBookmarks(new ChangeCoordinateProcessor(f.CoordinateName, f.AdjustmentValue, f.IsAbsolute, f.IsProportional));
						}
					}
					break;
				case Commands.EditorBookmarkSetCurrentCoordinates:
					var pp = controller.View.Viewer.GetCurrentScrollPosition();
					if (pp.Page == 0) {
						return;
					}
					controller.ProcessBookmarks(new ChangePageCoordinateProcessor(Constants.Coordinates.Top, pp.Page, pp.PageX, pp.PageY));
					break;
				case "_BookmarkAction":
					b.ShowBookmarkProperties(b.GetFirstSelectedModel<Model.BookmarkElement>());
					break;
				default:
					controller.ProcessBookmarks(new ChangeZoomRateProcessor(_viewType));
					break;
			}
		}

	}
}
