using System;
using System.Collections.Generic;
using System.Text;
using PDFPatcher.Common;
using Microsoft.Win32;

namespace PDFPatcher
{
	static class ShellHelper
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

		internal static bool CheckPdfOpenWithAssociation() {
			try {
				return OpenWithAssociation.HasAssociation();
			}
			catch (Exception) {
				// ignore
				return false;
			}
		}

		internal static void AddPdfOpenWithAssociation() {
			try {
				OpenWithAssociation.Create();
			}
			catch (Exception ex) {
				FormHelper.ErrorBox(ex.ToString());
			}
		}

		internal static void RemovePdfOpenWithAssociation() {
			try {
				OpenWithAssociation.Delete();
			}
			catch (Exception ex) {
				FormHelper.ErrorBox(ex.ToString());
			}
		}

		static RegistryKey OpenOrCreateSubKey(this RegistryKey baseKey, string keyPath, bool createIfInexistent) {
			var key = baseKey.OpenSubKey(keyPath, true);
			return key == null && createIfInexistent
				? baseKey.CreateSubKey(keyPath)
				: key;
		}

		sealed class OpenWithAssociation
		{
			const string DocumentType = Constants.AppEngName + ".Document";
			const string ShellCommandKey = "OpenWith" + Constants.AppEngName;
			const string ProgId = "PDF.Document";

			public static bool HasAssociation() {
				using (var classes = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes"))
				using (var fileType = classes.OpenSubKey(Constants.FileExtensions.Pdf, true)) {
					if (fileType == null) {
						return false;
					}
					var progIdName = fileType.GetValue(String.Empty, null) as string;
					if (progIdName == ProgId) {
						return true;
					}
					using (var command = classes.OpenSubKey($@"{progIdName}\shell\{ShellCommandKey}\command")) {
						return command != null;
					}
				}
			}

			public static void Create() {
				using (var classes = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes", true))
				using (var fileType = classes.OpenOrCreateSubKey(Constants.FileExtensions.Pdf, true)) {
					var progIdName = fileType.GetValue(String.Empty, null) as string;
					if (String.IsNullOrEmpty(progIdName)) {
						fileType.SetValue(String.Empty, progIdName = ProgId);
					}
					using (var progId = classes.OpenOrCreateSubKey(progIdName, true))
					using (var shell = progId.OpenOrCreateSubKey("shell", true))
					using (var openWith = shell.OpenOrCreateSubKey(ShellCommandKey, true))
					using (var command = openWith.OpenOrCreateSubKey("command", true)) {
						openWith.SetValue(String.Empty, "使用 " + Constants.AppName + " 打开");
						command.SetValue(String.Empty, $"\"{FilePath.AppPath}\" \"%1\"");
					}
				}
			}

			public static void Delete() {
				using (var classes = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes", true))
				using (var fileType = classes.OpenSubKey(Constants.FileExtensions.Pdf, true)) {
					if (fileType == null) {
						return;
					}
					var progIdName = fileType.GetValue(String.Empty, null) as string;
					if (String.IsNullOrEmpty(progIdName)) {
						return;
					}
					if (progIdName == ProgId) {
						classes.DeleteSubKeyTree(progIdName, false);
						return;
					}
					using (var shell = classes.OpenSubKey(progIdName + "\\shell", true))
					using (var openWith = shell.OpenOrCreateSubKey(ShellCommandKey, true)) {
						shell.DeleteSubKeyTree(ShellCommandKey, false);
					}
				}
			}
		}
	}
}
