using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using CharSet = System.Runtime.InteropServices.CharSet;
using DllImport = System.Runtime.InteropServices.DllImportAttribute;

namespace PDFPatcher.Common
{
	static class FontHelper
	{
		public static string FontDirectory { get; } = System.IO.Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\..\\fonts\\");

		/// <summary>
		/// 列出已安装的字体及其路径。
		/// </summary>
		/// <param name="includeFamilyName">是否包含字体组名称</param>
		public static Dictionary<string, string> GetInstalledFonts(bool includeFamilyName) {
			var d = new Dictionary<string, string>(50);
			using (var k = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts")) {
				foreach (var name in k.GetValueNames()) {
					var p = k.GetValue(name) as string;
					if (String.IsNullOrEmpty(p)) {
						continue;
					}
					if (p.IndexOf('\\') == -1) {
						p = FontDirectory + p;
					}
					var fp = new FilePath(p);
					try {
						if (fp.HasExtension(Constants.FileExtensions.Ttf)
							|| fp.HasExtension(Constants.FileExtensions.Otf)) {
							AddFontNames(d, p, includeFamilyName);
						}
						else if (fp.HasExtension(Constants.FileExtensions.Ttc)) {
							var nl = BaseFont.EnumerateTTCNames(p).Length;
							//Tracker.DebugMessage (p);
							for (int i = 0; i < nl; i++) {
								AddFontNames(d, p + "," + i.ToText(), includeFamilyName);
							}
						}
					}
					catch (System.IO.IOException) {
						// ignore
					}
					catch (NullReferenceException) {
					}
					catch (iTextSharp.text.DocumentException) {
						// ignore
					}
				}
			}
			return d;
		}

		static void AddFontNames(IDictionary<string, string> fontNames, string fontPath, bool includeFamilyName) {
			var nl = BaseFont.GetAllFontNames(fontPath, "Cp936", null);
			//Tracker.DebugMessage (fontPath);
			if (includeFamilyName) {
				fontNames[nl[0] as string] = fontPath;
			}
			var ffn = nl[2] as string[][];
			string n = null;
			string nn = null, cn = null;
			foreach (var fn in ffn) {
				var enc = fn[2];
				n = fn[3];
				if ("2052" == enc) {
					cn = n;
					break;
				}
				if ("1033" == enc) {
					nn = n;
				}
				else if ("0" == enc && nn == null) {
					nn = n;
				}
			}
			if (n != null) {
				//Tracker.DebugMessage (cn ?? nn ?? n);
				fontNames[cn ?? nn ?? n] = fontPath;
			}
			//foreach (string[] item in nl[1] as string[][]) {
			//    fontNames[item] = fontPath;
			//    Tracker.DebugMessage (item);
			//}
		}

		static class NativeMethods
		{
			[DllImport("Gdi32.dll", CharSet = CharSet.Unicode)]
			private static extern int AddFontResourceEx(string fontPath, int flag, IntPtr preserved);
			[DllImport("Gdi32.dll", CharSet = CharSet.Unicode)]
			private static extern int RemoveFontResourceEx(string fontPath, int flag, IntPtr preserved);

			internal static int LoadFont(string path) {
				return AddFontResourceEx(path, 0x10, IntPtr.Zero);
			}
			internal static int RemoveFont(string path) {
				return RemoveFontResourceEx(path, 0x10, IntPtr.Zero);
			}
		}
	}
}


