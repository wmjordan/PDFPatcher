using System;
using System.Collections.Generic;
using System.Text;
using PDFPatcher.Common;

namespace PDFPatcher;

internal static class CommonCommands
{
	internal static void CreateShortcut() {
		string p = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
		ShortcutFile s = new(FileHelper.CombinePath(p, Constants.AppEngName + ".exe")) {
			WorkingDirectory = p, Description = Constants.AppName
		};
		string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
		s.Save(FileHelper.CombinePath(desktopPath, Constants.AppName + ".lnk"));

		FormHelper.InfoBox("已在桌面创建" + Constants.AppName + "的快捷方式。");
	}

	internal static void VisitHomePage() {
		System.Diagnostics.Process.Start(Constants.AppHomePage);
	}
}