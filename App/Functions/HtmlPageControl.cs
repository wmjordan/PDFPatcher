using System;
using PDFPatcher.Common;

namespace PDFPatcher.Functions;

internal class HtmlPageControl : FunctionControl
{
	protected void HandleLinkClicked(string link) {
		int i = link.IndexOf(':');
		if (i == -1) {
			return;
		}

		string p = link.Substring(0, i);
		switch (p) {
			case "func":
				Function func = (Function)Enum.Parse(typeof(Function), link.Substring(i + 1));
				AppContext.MainForm.SelectFunctionList(func);
				break;
			case "recent":
				AppContext.MainForm.OpenFileWithEditor(
					AppContext.Recent.SourcePdfFiles[link.Substring(i + 1).ToInt32()]);
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
		e.Callback(
			Properties.Resources.ResourceManager.GetObject(e.Src.Substring("res:".Length)) as System.Drawing.Image);
		e.Handled = true;
	}

	/// <summary>返回字符串中包含指定字符串之后的子字符串。</summary>
	/// <remarks>如果找不到指定字符串，则返回空字符串。</remarks>
	protected static string SubstringAfter(string source, char value) {
		int index = source.LastIndexOf(value);
		if (index != -1) {
			return source.Substring(index + 1);
		}

		return string.Empty;
	}
}