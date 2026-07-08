using System;
using PDFPatcher.Common;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor;

static class EditorCommands
{
	static readonly CommandRegistry<Controller> __Commands = InitCommands();

	// 在此注册编辑器的命令和对应的命令标识符
	static CommandRegistry<Controller> InitCommands() {
		var d = new CommandRegistry<Controller>();
		d.Register(new LoadDocumentCommand(true, false), Commands.Open);
		d.Register(new LoadDocumentCommand(true, true), Commands.ImportBookmark);
		d.Register(new LoadDocumentCommand(false, false), Commands.OpenFile);
		d.Register(new InsertBookmarkCommand(), Commands.EditorInsertBookmark);
		d.Register(new SaveDocumentCommand(false, true), "_SaveButton", Commands.SaveBookmark);
		d.Register(new SaveDocumentCommand(true, true), Commands.SaveAsInfoFile);
		d.Register(new SaveDocumentCommand(true, false), Commands.Action, Commands.EditorSavePdf);
		d.Register(new BookmarkLevelCommand(true), Commands.EditorBookmarkLevelUp);
		d.Register(new BookmarkLevelCommand(false), Commands.EditorBookmarkLevelDown);
		d.Register(new DocumentPropertyCommand(), Commands.DocumentProperties);
		d.Register(new CopyBookmarkItemCommand(), Commands.Copy);
		d.Register(new PasteBookmarkItemCommand(), Commands.Paste);
		d.Register(new DeleteBookmarkItemCommand(), Commands.EditorBookmarkDelete, Commands.Delete);
		d.Register(new BookmarkStyleCommand(SetTextStyleProcessor.Style.SetBold), Commands.EditorBookmarkBold);
		d.Register(new BookmarkStyleCommand(SetTextStyleProcessor.Style.SetItalic), Commands.EditorBookmarkItalic);
		d.Register(new BookmarkPageCommand(1), Commands.EditorBookmarkPageNumberIncrement);
		d.Register(new BookmarkPageCommand(-1), Commands.EditorBookmarkPageNumberDecrement);
		d.Register(new BookmarkPageCommand(0), Commands.EditorBookmarkPageNumberShift);
		d.Register(new BookmarkPageCommand(0, true), Commands.EditorBookmarkPageNumberShiftTakeFollowing);
		d.Register(new SimpleBookmarkCommand<ClearDestinationOffsetProcessor, ClearDestinationOffsetProcessor.PositionType>(ClearDestinationOffsetProcessor.PositionType.XY), "_ClearPositionXY");
		d.Register(new SimpleBookmarkCommand<ClearDestinationOffsetProcessor, ClearDestinationOffsetProcessor.PositionType>(ClearDestinationOffsetProcessor.PositionType.X), "_ClearPositionX");
		d.Register(new SimpleBookmarkCommand<ClearDestinationOffsetProcessor, ClearDestinationOffsetProcessor.PositionType>(ClearDestinationOffsetProcessor.PositionType.Y), "_ClearPositionY");
		d.Register(new SimpleBookmarkCommand<BookmarkOpenStatusProcessor, bool>(true), "_SetOpenStatusTrue");
		d.Register(new SimpleBookmarkCommand<BookmarkOpenStatusProcessor, bool>(false), "_SetOpenStatusFalse");
		foreach (var item in Constants.DestinationAttributes.ViewType.Names) {
			d.Register(new BookmarkActionCommand(item), item);
		}
		d.Register(new BookmarkActionCommand(Constants.Coordinates.Unchanged), Constants.Coordinates.Unchanged);
		d.Register(new BookmarkActionCommand("_ChangeCoordinates"), "_ChangeCoordinates");
		d.Register(new BookmarkActionCommand(Commands.EditorBookmarkSetCurrentCoordinates), Commands.EditorBookmarkSetCurrentCoordinates);
		d.Register(new BookmarkActionCommand("_BookmarkAction"), "_BookmarkAction");
		d.Register(new SimpleBookmarkCommand<DestinationGotoTopProcessor>(), "_SetGotoTop");
		d.Register(new SimpleBookmarkCommand<ForceInternalLinkProcessor>(), "_ForceInternalLink");
		d.Register(new BookmarkSelectionCommand(Commands.SelectAllItems), Commands.SelectAllItems);
		d.Register(new BookmarkSelectionCommand(Commands.SelectNone), Commands.SelectNone);
		d.Register(new BookmarkSelectionCommand(Commands.SelectChildren), Commands.SelectChildren);
		d.Register(new BookmarkSelectionCommand(Commands.InvertSelection), Commands.InvertSelection);
		d.Register(new BookmarkSelectionCommand(Commands.ExpandAll), Commands.ExpandAll);
		d.Register(new BookmarkSelectionCommand(Commands.CollapseAll), Commands.CollapseAll);
		d.Register(new BookmarkSelectionCommand(Commands.ExpandSelection), Commands.ExpandSelection);
		d.Register(new BookmarkSelectionCommand(Commands.CollapseSelection), Commands.CollapseSelection);
		d.Register(new BookmarkSelectionCommand(Commands.CollapseChildren), Commands.CollapseChildren);
		d.Register(new OcrPageCommand(), Commands.EditorOcrPage);
		d.Register(new PagePropertiesCommand(), Commands.EditorPageProperties);
		d.Register(new SavePageImageCommand(), Commands.EditorSavePageImage);
		d.Register(new ViewerScrollToBookmarkCommand(), Commands.EditorViewerScrollToBookmark);
		d.Register(new InsertPageLabelCommand(), Commands.EditorInsertPageLabel);
		d.Register(new ApplyBookmarkOptionCommand(), EditorCommands.ApplyOptions);
		BookmarkMarkerCommand.RegisterCommands(d);
		ViewerCommand.RegisterCommands(d);
		QuickSelectCommand.RegisterCommands(d);
		return d;
	}

	public const string FirstPage = "_FirstPage";
	public const string PreviousPage = "_PreviousPage";
	public const string NextPage = "_NextPage";
	public const string LastPage = "_LastPage";
	public const string ScrollVertical = "_ScrollVertical";
	public const string ScrollHorizontal = "_ScrollHorizontal";
	public const string ScrollHorizontalLeftToRight = "_ScrollHorizontalLeftToRight";
	public const string TrueColorSpace = "_TrueColorSpace";
	public const string GrayColorSpace = "_GrayColorSpace";
	public const string InvertColor = "_InvertColor";
	public const string MoveMode = "_MoveMode";
	public const string SelectionMode = "_SelectionMode";
	public const string FullPageScroll = "_FullPageScroll";
	public const string ShowTextBorders = "_ShowTextBorders";
	public const string DarkMode = "_DarkMode";
	public const string GreenMode = "_GreenMode";
	public const string ShowBookmarks = "_ShowBookmarks";
	public const string ShowAnnotations = "_ShowAnnotations";
	public const string OcrDetectPunctuation = "_OcrDetectPunctuation";
	public const string FullScreen = "_FullScreen";
	public const string OcrPage = "_OcrPage";
	public const string Options = "_Option";
	public const string ApplyOptions = "_ApplyOptions";

	public static void Execute(string command, Controller controller, params string[] parameters) {
		__Commands.Process(command, controller, parameters);
	}
}
