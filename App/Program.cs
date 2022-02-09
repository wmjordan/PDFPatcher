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
		[STAThread]
		static void Main(string[] args) {
			using (var m = new Mutex(true, Constants.AppEngName)) {
				if (FormHelper.IsCtrlKeyDown || m.WaitOne(100)) {
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
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
