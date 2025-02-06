using System;
using PDFPatcher.Common;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor
{
    static class EditorCommands
    {
		static readonly CommandRegistry<Controller> __Commands = InitCommands();

		// 在此注册编辑器的命令和对应的命令标识符
		static CommandRegistry<Controller> InitCommands() {
			var d = new CommandRegistry<Editor.Controller>();
			d.Register(new Editor.LoadDocumentCommand(true, false), Commands.Open);
			d.Register(new Editor.LoadDocumentCommand(true, true), Commands.ImportBookmark);
			d.Register(new Editor.LoadDocumentCommand(false, false), Commands.OpenFile);
			d.Register(new Editor.InsertBookmarkCommand(), Commands.EditorInsertBookmark);
			d.Register(new Editor.SaveDocumentCommand(false, true), "_SaveButton", Commands.SaveBookmark);
			d.Register(new Editor.SaveDocumentCommand(true, true), Commands.SaveAsInfoFile);
			d.Register(new Editor.SaveDocumentCommand(true, false), Commands.Action, Commands.EditorSavePdf);
			d.Register(new Editor.BookmarkLevelCommand(true), Commands.EditorBookmarkLevelUp);
			d.Register(new Editor.BookmarkLevelCommand(false), Commands.EditorBookmarkLevelDown);
			d.Register(new Editor.DocumentPropertyCommand(), Commands.DocumentProperties);
			d.Register(new Editor.CopyBookmarkItemCommand(), Commands.Copy);
			d.Register(new Editor.PasteBookmarkItemCommand(), Commands.Paste);
			d.Register(new Editor.DeleteBookmarkItemCommand(), Commands.EditorBookmarkDelete, Commands.Delete);
			d.Register(new Editor.BookmarkStyleCommand(SetTextStyleProcessor.Style.SetBold), Commands.EditorBookmarkBold);
			d.Register(new Editor.BookmarkStyleCommand(SetTextStyleProcessor.Style.SetItalic), Commands.EditorBookmarkItalic);
			d.Register(new Editor.BookmarkPageCommand(1), Commands.EditorBookmarkPageNumberIncrement);
			d.Register(new Editor.BookmarkPageCommand(-1), Commands.EditorBookmarkPageNumberDecrement);
			d.Register(new Editor.BookmarkPageCommand(0), Commands.EditorBookmarkPageNumberShift);
			d.Register(new Editor.BookmarkPageCommand(0, true), Commands.EditorBookmarkPageNumberShiftTakeFollowing);
			d.Register(new Editor.SimpleBookmarkCommand<ClearDestinationOffsetProcessor, ClearDestinationOffsetProcessor.PositionType>(ClearDestinationOffsetProcessor.PositionType.XY), "_ClearPositionXY");
			d.Register(new Editor.SimpleBookmarkCommand<ClearDestinationOffsetProcessor, ClearDestinationOffsetProcessor.PositionType>(ClearDestinationOffsetProcessor.PositionType.X), "_ClearPositionX");
			d.Register(new Editor.SimpleBookmarkCommand<ClearDestinationOffsetProcessor, ClearDestinationOffsetProcessor.PositionType>(ClearDestinationOffsetProcessor.PositionType.Y), "_ClearPositionY");
			d.Register(new Editor.SimpleBookmarkCommand<BookmarkOpenStatusProcessor, bool>(true), "_SetOpenStatusTrue");
			d.Register(new Editor.SimpleBookmarkCommand<BookmarkOpenStatusProcessor, bool>(false), "_SetOpenStatusFalse");
			foreach (var item in Constants.DestinationAttributes.ViewType.Names) {
				d.Register(new Editor.BookmarkActionCommand(item), item);
			}
			d.Register(new Editor.BookmarkActionCommand(Constants.Coordinates.Unchanged), Constants.Coordinates.Unchanged);
			d.Register(new Editor.BookmarkActionCommand("_ChangeCoordinates"), "_ChangeCoordinates");
			d.Register(new Editor.BookmarkActionCommand(Commands.EditorBookmarkSetCurrentCoordinates), Commands.EditorBookmarkSetCurrentCoordinates);
			d.Register(new Editor.BookmarkActionCommand("_BookmarkAction"), "_BookmarkAction");
			d.Register(new Editor.SimpleBookmarkCommand<DestinationGotoTopProcessor>(), "_SetGotoTop");
			d.Register(new Editor.SimpleBookmarkCommand<ForceInternalLinkProcessor>(), "_ForceInternalLink");
			d.Register(new Editor.BookmarkSelectionCommand(Commands.SelectAllItems), Commands.SelectAllItems);
			d.Register(new Editor.BookmarkSelectionCommand(Commands.SelectNone), Commands.SelectNone);
			d.Register(new Editor.BookmarkSelectionCommand(Commands.SelectChildren), Commands.SelectChildren);
			d.Register(new Editor.BookmarkSelectionCommand(Commands.InvertSelection), Commands.InvertSelection);
			d.Register(new Editor.BookmarkSelectionCommand(Commands.ExpandAll), Commands.ExpandAll);
			d.Register(new Editor.BookmarkSelectionCommand(Commands.CollapseAll), Commands.CollapseAll);
			d.Register(new Editor.BookmarkSelectionCommand(Commands.ExpandSelection), Commands.ExpandSelection);
			d.Register(new Editor.BookmarkSelectionCommand(Commands.CollapseSelection), Commands.CollapseSelection);
			d.Register(new Editor.BookmarkSelectionCommand(Commands.CollapseChildren), Commands.CollapseChildren);
			d.Register(new Editor.OcrPageCommand(), Commands.EditorOcrPage);
			d.Register(new Editor.PagePropertiesCommand(), Commands.EditorPageProperties);
			d.Register(new Editor.SavePageImageCommand(), Commands.EditorSavePageImage);
			d.Register(new Editor.ViewerScrollToBookmarkCommand(), Commands.EditorViewerScrollToBookmark);
			d.Register(new Editor.InsertPageLabelCommand(), Commands.EditorInsertPageLabel);
			Editor.BookmarkMarkerCommand.RegisterCommands(d);
			Editor.ViewerCommand.RegisterCommands(d);
			Editor.QuickSelectCommand.RegisterCommands(d);
			return d;
		}

		public static void Execute(string command, Controller controller, params string[] parameters) {
			__Commands.Process(command, controller, parameters);
		}
	}
}
