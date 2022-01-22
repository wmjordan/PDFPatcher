using System.IO;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions;

internal static class RecentFileMenuHelper
{
	public static void AddRecentHistoryFile(string path) {
		switch (Path.GetExtension(path).ToLowerInvariant()) {
			case Constants.FileExtensions.Pdf:
				AppContext.RecentItems.AddHistoryItem(AppContext.Recent.SourcePdfFiles, path);
				break;
			//case Constants.FileExtensions.Txt:
			case Constants.FileExtensions.Xml:
				AppContext.RecentItems.AddHistoryItem(AppContext.Recent.InfoDocuments, path);
				break;
		}
	}


	public static void AddSourcePdfFiles(this ToolStripItemCollection list) {
		foreach (string item in AppContext.Recent.SourcePdfFiles) {
			ToolStripItem i = list.Add(FileHelper.GetEllipticPath(item, 50));
			i.ToolTipText = item;
			if (File.Exists(item) == false) {
				i.Enabled = false;
			}
		}
	}

	public static void AddInfoFiles(this ToolStripItemCollection list) {
		foreach (string item in AppContext.Recent.InfoDocuments) {
			if (FileHelper.IsPathValid(item) && Path.IsPathRooted(item)) {
				ToolStripItem i = list.Add(FileHelper.GetEllipticPath(item, 50));
				i.ToolTipText = item;
				if (File.Exists(item) == false) {
					i.Enabled = false;
				}
			}
		}
	}
}