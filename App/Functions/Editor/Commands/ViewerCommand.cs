using System.Drawing;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor;

internal sealed class ViewerCommand : IEditorCommand
{
	private static readonly Color __DarkModeColor = Color.DarkGray;
	private static readonly Color __GreenModeColor = Color.FromArgb(0xCC, 0xFF, 0xCC);

	private static readonly string[] __commands = {
		"_FirstPage", "_PreviousPage", "_NextPage", "_LastPage", "_ScrollVertical", "_ScrollHorizontal",
		"_TrueColorSpace", "_GrayColorSpace", "_InvertColor", "_MoveMode", "_SelectionMode", "_FullPageScroll",
		"_ShowTextBorders", "_DarkMode", "_GreenMode", "_ShowBookmarks", "_ShowAnnotations", "_OcrDetectPunctuation",
		"_FullScreen", "_EditorOptions"
	};

	private readonly string _command;

	public ViewerCommand(string command) {
		_command = command;
	}

	public void Process(Controller controller, params string[] parameters) {
		PdfViewerControl v = controller.View.Viewer;
		switch (_command) {
			case "_FirstPage":
			case "_PreviousPage":
			case "_NextPage":
			case "_LastPage":
				v.ExecuteCommand(_command);
				break;
			case "_ScrollVertical":
				v.ContentDirection = ContentDirection.TopToDown;
				break;
			case "_ScrollHorizontal":
				v.ContentDirection = ContentDirection.RightToLeft;
				break;
			case "_TrueColorSpace":
				v.GrayScale = false;
				break;
			case "_GrayColorSpace":
				v.GrayScale = true;
				break;
			case "_InvertColor":
				v.InvertColor = !v.InvertColor;
				break;
			case "_MoveMode":
				v.MouseMode = MouseMode.Move;
				break;
			case "_SelectionMode":
				v.MouseMode = MouseMode.Selection;
				break;
			case "_FullPageScroll":
				v.FullPageScroll = !v.FullPageScroll;
				break;
			case "_ShowTextBorders":
				v.ShowTextBorders = !v.ShowTextBorders;
				break;
			case "_DarkMode":
				v.TintColor = v.TintColor != __DarkModeColor ? __DarkModeColor : Color.Transparent;
				break;
			case "_GreenMode":
				v.TintColor = v.TintColor != __GreenModeColor ? __GreenModeColor : Color.Transparent;
				break;
			case "_ShowAnnotations":
				v.HideAnnotations = !v.HideAnnotations;
				break;
			case "_ShowBookmarks":
				controller.View.MainPanel.Panel1Collapsed = !controller.View.MainPanel.Panel1Collapsed;
				break;
			case "_OcrDetectPunctuation":
				v.OcrOptions.DetectContentPunctuations = !v.OcrOptions.DetectContentPunctuations;
				break;
			case "_FullScreen":
				AppContext.MainForm.FullScreen = !AppContext.MainForm.FullScreen;
				break;
			case "_EditorOptions":
				AppContext.MainForm.SelectFunctionList(Function.EditorOptions);
				break;
		}
	}

	internal static void RegisterCommands(CommandRegistry<Controller> registry) {
		foreach (string item in __commands) {
			registry.Register(new ViewerCommand(item), item);
		}
	}
}