using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Functions;
using PDFPatcher.Properties;
using TheArtOfDev.HtmlRenderer.Core.Entities;

namespace PDFPatcher;

[ToolboxItem(false)]
internal sealed partial class FrontPageControl : HtmlPageControl
{
	private readonly Regex __FrontPagePattern = new("<div>(.+);(.+);(.+);(.+)</div>", RegexOptions.CultureInvariant);

	public FrontPageControl() {
		InitializeComponent();
		Text = "主页";
		RefreshContent();
		RecentFileItemClicked = (s, args) => { AppContext.MainForm.OpenFileWithEditor(args.ClickedItem.ToolTipText); };
		AllowDrop = true;
	}

	public override string FunctionName => "主页";

	public override Bitmap IconImage => Resources.HomePage;

	public override void ExecuteCommand(string commandName, params string[] parameters) {
		if (commandName == Commands.Open) {
			string n = AppContext.MainForm.ShowPdfFileDialog();
			if (n != null) {
				AppContext.MainForm.OpenFileWithEditor(n);
			}

			return;
		}

		if (commandName == Commands.CleanUpInexistentFiles) {
			AppContext.CleanUpInexistentFiles(AppContext.Recent.SourcePdfFiles);
			AppContext.CleanUpInexistentFiles(AppContext.Recent.InfoDocuments);
			AppContext.CleanUpInexistentFolders(AppContext.Recent.Folders);
			RefreshContent();
			return;
		}

		base.ExecuteCommand(commandName, parameters);
	}

	protected override void OnDragEnter(DragEventArgs drgevent) {
		base.OnDragEnter(drgevent);
		drgevent.FeedbackDragFileOver(Constants.FileExtensions.PdfAndAllBookmarkExtension);
	}

	protected override void OnDragDrop(DragEventArgs drgevent) {
		base.OnDragDrop(drgevent);
		foreach (string item in drgevent.DropFileOver(Constants.FileExtensions.PdfAndAllBookmarkExtension)) {
			AppContext.MainForm.OpenFileWithEditor(item);
		}
	}

	internal override void OnSelected() {
		base.OnSelected();
		RefreshContent();
	}

	public override void SetupCommand(ToolStripItem item) {
		switch (item.Name) {
			case Commands.Close:
			case Commands.Action:
				EnableCommand(item, false, true);
				break;
		}

		base.SetupCommand(item);
	}

	private void RefreshContent() {
		_FrontPageBox.Text = __FrontPagePattern
			.Replace(Resources.FrontPage, @"<div><a href=""func:$2""><img src=""res:$1"" />$3</a></div>")
			.Replace("$appName", Constants.AppName)
			.Replace("$AppHomePage", Constants.AppHomePage)
			.Replace("<li></li>", GetLastFileList());
	}

	private static string GetLastFileList() {
		int i = 0;
		return string.Concat(AppContext.Recent.SourcePdfFiles.ConvertAll(s =>
			FileHelper.IsPathValid(s) && File.Exists(s)
				? string.Concat(@"<li><a href=""recent:", i++, "\">", SubstringAfter(s, '\\'), "</a></li>")
				: string.Concat(@"<li id=""", i++, "\">", SubstringAfter(s, '\\'), "</li>")));
	}

	private void _FrontPageBox_LinkClicked(object sender, HtmlLinkClickedEventArgs e) {
		HandleLinkClicked(e.Link);
		e.Handled = true;
	}

	private void _FrontPageBox_ImageLoad(object sender, HtmlImageLoadEventArgs e) {
		LoadResourceImage(e);
	}
}