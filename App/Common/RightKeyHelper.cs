using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security;
using Microsoft.Win32;

namespace PDFPatcher.Common
{
	class RightKeyHelper
	{
		/// <summary>
		/// 为程序添加鼠标右键菜单
		/// 首次提交PR后Owner很快就回复，感谢他的建议现在采用向HKEY_CURRENT_USER\SOFTWARE\Classes下添加项，完美避开提权问题。不需要修改app.manifest
		/// 有关注册表的描述：https://www.c-sharpcorner.com/UploadFile/f9f215/windows-registry/
		/// 注册表的提权操作见：https://docs.microsoft.com/zh-cn/dotnet/api/system.security.accesscontrol.registrysecurity?view=net-6.0#methods
		/// </summary>

		public static bool Add(string appName, string appPath) {
			try {
				RegistryKey software = Registry.CurrentUser.OpenSubKey(@"SOFTWARE");
				RegistryKey key = software;
				RegistryKey shell = key.OpenSubKey(@"Classes\*\shell", true);
				RegistryKey pdfPather = shell.CreateSubKey(appName);
				RegistryKey command = shell.CreateSubKey(appName+@"\command");
				command.SetValue("", appPath+@" %1");
				pdfPather.SetValue("", "使用PDF补丁丁打开");
				pdfPather.SetValue("Icon",appPath);
				return true;
			}
			catch (Exception e){
				FormHelper.InfoBox(e.ToString());
				return false;
			}
		}

		public static bool Delete(string appName) {
			RightKeyHelper reg = new();
			try {
				RegistryKey software = Registry.CurrentUser.OpenSubKey(@"SOFTWARE");
				RegistryKey key = software;
				RegistryKey shell = key.OpenSubKey(@"Classes\*\shell", true);
				RegistryKey pdfPather = shell.OpenSubKey(appName, true);
				pdfPather.DeleteSubKey(@"command");
				shell.DeleteSubKey(appName);
				return true;
			}
			catch (Exception e) {
				FormHelper.InfoBox(e.ToString());
				return false;
			}
		}


		public static bool IsExist(string appName) {
			RightKeyHelper reg = new();
			string[] subkeyNames;
			try {
				RegistryKey software = Registry.CurrentUser.OpenSubKey(@"SOFTWARE");
				RegistryKey key = software;
				RegistryKey shell = key.OpenSubKey(@"Classes\*\shell", true);
				subkeyNames=shell.GetSubKeyNames();
				foreach(string keyName in subkeyNames) {
					if(keyName == appName) {
						key.Close();
						return true;
					}
				}
				return false;
			}
			catch(Exception e) {
				FormHelper.InfoBox(e.ToString());
				return false;
			}
		}

	}
}
