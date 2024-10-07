using System;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	class HtmlPageControl : FunctionControl
	{
		protected void HandleLinkClicked(string link) {
			var i = link.IndexOf(':');
			if (i == -1) {
				return;
			}
			switch (link.Substring(0, i)) {
				case "func":
					var func = (Function)Enum.Parse(typeof(Function), link.Substring(i + 1));
					AppContext.MainForm.SelectFunctionList(func);
					break;
				case "recent":
					AppContext.MainForm.OpenFileWithEditor(AppContext.Recent.SourcePdfFiles[link.Substring(i + 1).ToInt32()]);
					break;
				case "exec":
					ExecuteCommand(link.Substring(i + 1));
					break;
				case "http":
				case "https":
					System.Diagnostics.Process.Start(link);
					break;
			}
		}

		protected void LoadResourceImage(TheArtOfDev.HtmlRenderer.Core.Entities.HtmlImageLoadEventArgs e) {
			e.Callback(Properties.Resources.ResourceManager.GetObject(e.Src.Substring("res:".Length)) as System.Drawing.Image);
			e.Handled = true;
		}
	}
}
