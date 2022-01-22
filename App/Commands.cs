using System.Collections.Generic;

namespace PDFPatcher;

internal static class Commands
{
    internal const string File = "_File";

    internal const string Tools = "_ToolBox";

    internal const string Selection = "_Select";

    internal const string Help = "_Help";
    internal static readonly string[] TopMenuItems = { File, Tools, Selection };
    internal static readonly string[] CommonSelectionCommands = { SelectAllItems, SelectNone, InvertSelectItem };

    internal static readonly HashSet<string> DefaultDisabledItems = new(
        new[] { SelectAllItems, InvertSelectItem, SelectNone, Copy, Delete, Options });

    internal static readonly HashSet<string> DefaultHiddenItems = new(
        new[] {
            ImportBookmark, SaveBookmark, SaveAsInfoFile, DocumentProperties, SelectAllPdf, SelectAllImages,
            SelectAllFolders, ItemTypeSeparator
        });

    #region File menu

    internal const string Open = "_Open";
    internal const string OpenFile = "OpenFile";
    internal const string LoadList = "_LoadList";
    internal const string RecentFiles = "_RecentFiles";
    internal const string ImportBookmark = "_ImportBookmark";
    internal const string Close = "_Close";
    internal const string Action = "_Action";
    internal const string SaveBookmark = "_SaveBookmark";
    internal const string SaveAsInfoFile = "_SaveAsInfoFile";
    internal const string DocumentProperties = "_Properties";
    internal const string Options = "_Options";
    internal const string ResetOptions = "_ResetOptions";
    internal const string AppOptions = "_AppOptions";
    internal const string RestoreOptions = "_RestoreOptions";
    internal const string SaveOptions = "_SaveOptions";
    internal const string SaveList = "_SaveList";
    internal const string CleanUpInexistentFiles = "_CleanUpInexistentFiles";
    internal const string Exit = "_Exit";

    #endregion

    #region Tools menu

    internal const string LogWindow = "_LogWindow";
    internal const string ShowGeneralToolbar = "_ShowGeneralToolbar";
    internal const string CustomizeToolbar = "_CustomizeToolbar";

    #endregion

    #region Selection menu

    internal const string Copy = "_Copy";
    internal const string Delete = "_Delete";
    internal const string Paste = "_Paste";
    internal const string SelectAllPdf = "_SelectAllPdfFiles";
    internal const string SelectAllImages = "_SelectAllImages";
    internal const string SelectAllFolders = "_SelectAllFolders";
    internal const string SelectAllItems = "_SelectAll";
    internal const string InvertSelectItem = "_InvertSelect";
    internal const string SelectNone = "_SelectNone";
    internal const string ItemTypeSeparator = "_ItemTypeSeparator";

    #endregion

    #region Options menu

    internal const string PatcherOptions = "_PatcherOptions";
    internal const string MergerOptions = "_MergerOptions";
    internal const string InfoFileOptions = "_InfoFileOptions";

    #endregion

    #region Help menu

    internal const string CreateShortcut = "_CreateShortcut";
    internal const string VisitHomePage = "_VisitHomePage";
    internal const string CheckUpdate = "_CheckUpdate";

    #endregion

    #region Editor

    internal const string EditorInsertBookmark = "_BookmarkHere";
    internal const string EditorSavePdf = "_SavePDF";
    internal const string EditorBookmarkLevelUp = "_LevelUp";
    internal const string EditorBookmarkLevelDown = "_LevelDown";
    internal const string EditorBookmarkDelete = "_DeleteBookmark";
    internal const string EditorBookmarkBold = "_BookmarkBoldButton";
    internal const string EditorBookmarkItalic = "_BookmarkItalicButton";
    internal const string EditorBookmarkPageNumberIncrement = "_IncrementPageNumber";
    internal const string EditorBookmarkPageNumberDecrement = "_DecrementPageNumber";
    internal const string EditorBookmarkPageNumberShift = "_ShiftMultiPageNumber";
    internal const string EditorOcrPage = "_OcrPage";
    internal const string EditorPageProperties = "_PageProperties";
    internal const string EditorSavePageImage = "_SavePageImage";

    #endregion
}