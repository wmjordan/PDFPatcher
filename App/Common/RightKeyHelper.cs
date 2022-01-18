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
		/// 添加右键菜单需要在注册表的HKEY_CLASSES_ROO节点下操作
		/// 原本打算使用System.Security进行提权，但是尝试多种方法均失败，所以采取了修改app.manifest来实现
		/// <requestedExecutionLevel level="asInvoker" uiAccess="false" /> -> <requestedExecutionLevel level="highestAvailable" uiAccess="false" />
		/// 有关注册表的描述：https://www.c-sharpcorner.com/UploadFile/f9f215/windows-registry/
		/// 注册表的提权操作见：https://docs.microsoft.com/zh-cn/dotnet/api/system.security.accesscontrol.registrysecurity?view=net-6.0#methods
		/// </summary>
		private static RegistryKey root = Registry.ClassesRoot;

		public static bool Add(string appName, string appPath) {
			try {
				RegistryKey key = root;
				RegistryKey shell = key.OpenSubKey(@"*\shell",true);
				RegistryKey pdfPather = shell.CreateSubKey(appName);
				RegistryKey command = shell.CreateSubKey(appName+@"\command");
				command.SetValue("", appPath);
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
				RegistryKey key = root;
				RegistryKey shell = key.OpenSubKey(@"*\shell",true);
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
				RegistryKey key = root;
				RegistryKey shell = key.OpenSubKey(@"*\shell", true);
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
