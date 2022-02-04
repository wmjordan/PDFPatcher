using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using PDFPatcher.Common;

namespace PDFPatcher
{
	static class Program
	{
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			using (var m = new Mutex(true, Constants.AppEngName)) {
				if (FormHelper.IsCtrlKeyDown || m.WaitOne(100)) {
					Application.Run(AppContext.MainForm = new MainForm());
				}
				else {
					Process.GetProcesses()
						.FirstOrDefault(p => p.MainWindowTitle.StartsWith(Constants.AppName + " [", StringComparison.Ordinal))
						?.SendCopyDataMessage(String.Join("\n", args));
				}
			}
		}
	}
}
