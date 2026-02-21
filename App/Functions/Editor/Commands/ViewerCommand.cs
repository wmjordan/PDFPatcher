using System.Drawing;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor;

sealed class ViewerCommand : IEditorCommand
{
	static readonly Color __DarkModeColor = Color.DarkGray;
	static readonly Color __GreenModeColor = Color.FromArgb(0xCC, 0xFF, 0xCC);
	static readonly string[] __commands = [
		EditorCommands.FirstPage,
		EditorCommands.PreviousPage,
		EditorCommands.NextPage,
		EditorCommands.LastPage,
		EditorCommands.ScrollVertical,
		EditorCommands.ScrollHorizontal,
		EditorCommands.ScrollHorizontalLeftToRight,
		EditorCommands.TrueColorSpace,
		EditorCommands.GrayColorSpace,
		EditorCommands.InvertColor,
		EditorCommands.MoveMode,
		EditorCommands.SelectionMode,
		EditorCommands.FullPageScroll,
		EditorCommands.ShowTextBorders,
		EditorCommands.DarkMode,
		EditorCommands.GreenMode,
		EditorCommands.ShowBookmarks,
		EditorCommands.ShowAnnotations,
		EditorCommands.OcrDetectPunctuation,
		EditorCommands.FullScreen
	];
	internal static void RegisterCommands(CommandRegistry<Controller> registry) {
		foreach (var item in __commands) {
			registry.Register(new ViewerCommand(item), item);
		}
	}

	readonly string _command;

	public ViewerCommand(string command) {
		_command = command;
	}

	public void Process(Controller controller, params string[] parameters) {
		var v = controller.View.Viewer;
		switch (_command) {
			case EditorCommands.FirstPage:
			case EditorCommands.PreviousPage:
			case EditorCommands.NextPage:
			case EditorCommands.LastPage: v.ExecuteCommand(_command); break;
			case EditorCommands.ScrollVertical: v.ContentDirection = ContentDirection.TopToDown; break;
			case EditorCommands.ScrollHorizontal: v.ContentDirection = ContentDirection.RightToLeft; break;
			case EditorCommands.ScrollHorizontalLeftToRight: v.ContentDirection = ContentDirection.LeftToRight; break;
			case EditorCommands.TrueColorSpace: v.GrayScale = false; break;
			case EditorCommands.GrayColorSpace: v.GrayScale = true; break;
			case EditorCommands.InvertColor: v.InvertColor = !v.InvertColor; break;
			case EditorCommands.MoveMode: v.MouseMode = MouseMode.Move; break;
			case EditorCommands.SelectionMode: v.MouseMode = MouseMode.Selection; break;
			case EditorCommands.FullPageScroll: v.FullPageScroll = !v.FullPageScroll; break;
			case EditorCommands.ShowTextBorders: v.ShowTextBorders = !v.ShowTextBorders; break;
			case EditorCommands.DarkMode: v.TintColor = v.TintColor != __DarkModeColor ? __DarkModeColor : Color.Transparent; break;
			case EditorCommands.GreenMode: v.TintColor = v.TintColor != __GreenModeColor ? __GreenModeColor : Color.Transparent; break;
			case EditorCommands.ShowAnnotations: v.HideAnnotations = !v.HideAnnotations; break;
			case EditorCommands.ShowBookmarks: controller.View.MainPanel.Panel1Collapsed = !controller.View.MainPanel.Panel1Collapsed; break;
			case EditorCommands.OcrDetectPunctuation: v.OcrOptions.DetectContentPunctuations = !v.OcrOptions.DetectContentPunctuations; break;
			case EditorCommands.FullScreen: AppContext.MainForm.FullScreen = !AppContext.MainForm.FullScreen; break;
		}
	}

}
