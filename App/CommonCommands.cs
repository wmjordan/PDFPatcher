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
				WorkingDirectory = p,
				Description = Constants.AppName
			};
			var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			s.Save(FileHelper.CombinePath(desktopPath, Constants.AppName + ".lnk"));

			FormHelper.InfoBox("已在桌面创建" + Constants.AppName + "的快捷方式。");
		}

		internal static void VisitHomePage() {
			System.Diagnostics.Process.Start(Constants.AppHomePage);
		}

		internal static void RightKey(Functions.AppOptionForm appOptionForm) {
			bool isExist = RightKeyHelper.IsExist(Constants.AppEngName);
			var path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
			var exeAbsPath = FileHelper.CombinePath(path, Constants.AppEngName + ".exe");

			if (isExist == true) {
				bool isDelete = RightKeyHelper.Delete(Constants.AppEngName);
				if (isDelete) {
					FormHelper.InfoBox(Constants.AppEngName + "右键菜单删除成功。");
					appOptionForm._RightKeyButton.Text = @"添加右键菜单";

				}
			}
			else {
				bool isAdd = RightKeyHelper.Add(Constants.AppEngName, exeAbsPath);
				if (isAdd) {
					FormHelper.InfoBox(Constants.AppEngName + "右键菜单创建成功。");
					appOptionForm._RightKeyButton.Text = @"删除右键菜单";

				}

			}
		}
		internal static void regButtonDisplay(Functions.AppOptionForm appOptionForm) {
			bool isExist = RightKeyHelper.IsExist(Constants.AppEngName);
			if(isExist == true) {
				appOptionForm._RightKeyButton.Text = @"删除右键菜单";
			}
		}
	}
}
