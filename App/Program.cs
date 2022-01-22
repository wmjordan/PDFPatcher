using System;
using System.Windows.Forms;

namespace PDFPatcher;

internal static class Program
{
    /// <summary>
    ///     应用程序的主入口点。
    /// </summary>
    [STAThread]
    private static void Main() {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(AppContext.MainForm = new MainForm());
    }
}