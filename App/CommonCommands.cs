using System;
using System.Collections.Generic;
using System.Text;
using PDFPatcher.Common;

namespace PDFPatcher
{
	static class CommonCommands
	{
		internal static void CreateShortcut() {
			var p = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
			var s = new ShortcutFile(FileHelper.CombinePath(p, Constants.AppEngName + ".exe")) {
				WorkingDirectory = p, Description = Constants.AppName
			};
			var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			s.Save(FileHelper.CombinePath(desktopPath, Constants.AppName + ".lnk"));

			FormHelper.InfoBox("已在桌面创建" + Constants.AppName + "的快捷方式。");
		}

		internal static void VisitHomePage() {
			System.Diagnostics.Process.Start(Constants.AppHomePage);
		}
	}
}