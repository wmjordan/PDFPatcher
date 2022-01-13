using System.Drawing;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor;

internal sealed class BookmarkMarkerCommand : IEditorCommand
{
	private static readonly string[] __commands = {
		"_MarkBookmarkRed", "_MarkBookmarkYellow", "_MarkBookmarkGreen", "_MarkBookmarkBlue", "_MarkBookmarkCyan",
		"_MarkBookmarkPurple", "_UnmarkBookmark", "_ClearBookmarkMarks", "_SelectRedMarks", "_SelectYellowMarks",
		"_SelectGreenMarks", "_SelectBlueMarks", "_SelectCyanMarks", "_SelectPurpleMarks"
	};

	internal static void RegisterCommands(CommandRegistry<Controller> registry) {
		foreach (string item in __commands) {
			registry.Register(new BookmarkMarkerCommand(item), item);
		}
	}

	private readonly string _command;

	public BookmarkMarkerCommand(string command) {
		_command = command;
	}

	public void Process(Controller controller, params string[] parameters) {
		BookmarkEditorView b = controller.View.Bookmark;
		switch (_command) {
			case "_MarkBookmarkRed":
				b.MarkItems(b.GetSelectedElements(true), Color.PeachPuff);
				break;
			case "_MarkBookmarkYellow":
				b.MarkItems(b.GetSelectedElements(true), Color.Yellow);
				break;
			case "_MarkBookmarkGreen":
				b.MarkItems(b.GetSelectedElements(true), Color.GreenYellow);
				break;
			case "_MarkBookmarkBlue":
				b.MarkItems(b.GetSelectedElements(true), Color.LightSkyBlue);
				break;
			case "_MarkBookmarkCyan":
				b.MarkItems(b.GetSelectedElements(true), Color.Aqua);
				break;
			case "_MarkBookmarkPurple":
				b.MarkItems(b.GetSelectedElements(true), Color.Violet);
				break;
			case "_UnmarkBookmark":
				b.UnmarkItems(b.GetSelectedElements(true));
				break;
			case "_ClearBookmarkMarks":
				if (b.HasMarker && FormHelper.YesNoBox("是否确定清除书签标记？") == DialogResult.Yes) {
					b.ClearMarks(true);
				}

				break;
			case "_SelectRedMarks":
				b.SelectMarkedItems(Color.PeachPuff);
				break;
			case "_SelectYellowMarks":
				b.SelectMarkedItems(Color.Yellow);
				break;
			case "_SelectGreenMarks":
				b.SelectMarkedItems(Color.GreenYellow);
				break;
			case "_SelectBlueMarks":
				b.SelectMarkedItems(Color.LightSkyBlue);
				break;
			case "_SelectCyanMarks":
				b.SelectMarkedItems(Color.Aqua);
				break;
			case "_SelectPurpleMarks":
				b.SelectMarkedItems(Color.Violet);
				break;
		}
	}
}