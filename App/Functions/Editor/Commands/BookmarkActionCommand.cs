using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor;

internal sealed class BookmarkActionCommand : IEditorCommand
{
	private readonly string _viewType;

	public BookmarkActionCommand(string viewType) {
		_viewType = viewType;
	}

	public void Process(Controller controller, params string[] parameters) {
		BookmarkEditorView b = controller.View.Bookmark;
		if (b.FocusedItem == null) {
			return;
		}

		switch (_viewType) {
			case Constants.DestinationAttributes.ViewType.XYZ:
				using (ZoomRateEntryForm form = new()) {
					if (form.ShowDialog() != DialogResult.OK) {
						return;
					}

					string z = form.ZoomRate;
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
				using (NewCoordinateEntryForm f = new()) {
					if (f.ShowDialog() == DialogResult.OK) {
						controller.ProcessBookmarks(new ChangeCoordinateProcessor(f.CoordinateName,
							f.AdjustmentValue, f.IsAbsolute, f.IsProportional));
					}
				}

				break;
			case "_BookmarkAction":
				b.ShowBookmarkProperties(b.GetFirstSelectedModel<BookmarkElement>());
				break;
			default:
				controller.ProcessBookmarks(new ChangeZoomRateProcessor(_viewType));
				break;
		}
	}
}