using System;
using Microsoft.Win32;
using PDFPatcher.Common;

namespace PDFPatcher
{
	static class ShellHelper
	{
		internal static void CreateShortcut() {
			var p = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
			var s = new ShortcutFile(FileHelper.CombinePath(p, $"{Constants.AppEngName}.exe")) {
				WorkingDirectory = p,
				Description = Constants.AppName
			};
			var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			s.Save(FileHelper.CombinePath(desktopPath, $"{Constants.AppName}.lnk"));

			FormHelper.InfoBox($"已在桌面创建{Constants.AppName}的快捷方式。");
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
			const string MRUList = "MRUList";
			const string ApplicationKey = @"SOFTWARE\Classes\Applications\PDFPatcher.exe";
			const string ShellOpenCommandKey = @"\shell\open\command";
			const string OpenWithList = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.pdf\OpenWithList";
			static readonly string AppName = FilePath.AppPath.FileName;

			public static bool HasAssociation() {
				using (var command = Registry.CurrentUser.OpenOrCreateSubKey(ApplicationKey + ShellOpenCommandKey, false)) {
					if (command?.GetValue(String.Empty) is string != true) {
						return false;
					}
				}
				using (var openWithList = Registry.CurrentUser.OpenOrCreateSubKey(OpenWithList, false)) {
					if (openWithList == null) {
						return false;
					}
					var mruList = openWithList.GetValue(MRUList) as string;
					if (mruList == null) {
						return false;
					}
					foreach (var k in mruList) {
						var appName = openWithList.GetValue(k.ToString()) as string;
						if (String.Equals(appName, AppName, StringComparison.OrdinalIgnoreCase)) {
							return true;
						}
					}
				}
				return false;
			}

			public static void Create() {
				using (var command = Registry.CurrentUser.OpenOrCreateSubKey(ApplicationKey + ShellOpenCommandKey, true)) {
					command.SetValue(String.Empty, $"\"{FilePath.AppPath}\" \"%1\"");
				}
				using (var openWithList = Registry.CurrentUser.OpenOrCreateSubKey(OpenWithList, true)) {
					var mruList = openWithList.GetValue(MRUList) as string;
					if (mruList == null) {
						openWithList.SetValue(MRUList, "a");
						openWithList.SetValue("a", AppName);
						return;
					}
					var slots = new bool[26];
					foreach (var k in mruList) {
						var appName = openWithList.GetValue(k.ToString()) as string;
						if (String.Equals(appName, AppName, StringComparison.OrdinalIgnoreCase)) {
							return;
						}
						if ((uint)(k - 'a') <= ('z' - 'a')) {
							slots[k - 'a'] = true;
						}
					}
					for (char i = 'a'; i <= 'z'; i++) {
						if (slots[i - 'a'] == false) {
							openWithList.SetValue(i.ToString(), AppName);
							if (mruList.IndexOf(i) < 0) {
								openWithList.SetValue(MRUList, i + mruList);
							}
							return;
						}
					}
				}
			}

			public static void Delete() {
				Registry.CurrentUser.DeleteSubKeyTree(ApplicationKey);
				using (var openWithList = Registry.CurrentUser.OpenOrCreateSubKey(OpenWithList, false)) {
					if (openWithList == null) {
						return;
					}
					var mruList = openWithList.GetValue(MRUList) as string;
					if (mruList == null) {
						return;
					}
					foreach (var k in mruList) {
						var keyName = k.ToString();
						var appName = openWithList.GetValue(keyName) as string;
						if (String.Equals(appName, AppName, StringComparison.OrdinalIgnoreCase)) {
							openWithList.DeleteValue(keyName);
							openWithList.SetValue(MRUList, mruList.Replace(keyName, String.Empty));
							return;
						}
					}
				}
			}
		}
	}
}
