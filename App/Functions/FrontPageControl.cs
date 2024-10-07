using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PDFPatcher.Common;
using TheArtOfDev.HtmlRenderer.Core.Entities;

namespace PDFPatcher
{
	[ToolboxItem(false)]
	sealed partial class FrontPageControl : Functions.HtmlPageControl, ITabContent
	{
		readonly Regex __FrontPagePattern = new Regex("<div>(.+);(.+);(.+);(.+)</div>", RegexOptions.CultureInvariant);

		public override string FunctionName => "主页";

		public override System.Drawing.Bitmap IconImage => Properties.Resources.HomePage;

		bool ITabContent.CanClose => false;

		public override void ExecuteCommand(string commandName, params string[] parameters) {
			switch (commandName) {
				case Commands.Open: 
					var n = AppContext.MainForm.ShowPdfFileDialog();
					if (n != null) {
						AppContext.MainForm.OpenFileWithEditor(n);
					}
					return;
				case Commands.CleanUpInexistentFiles:
					AppContext.CleanUpInexistentFiles(AppContext.Recent.SourcePdfFiles);
					AppContext.CleanUpInexistentFiles(AppContext.Recent.InfoDocuments);
					AppContext.CleanUpInexistentFolders(AppContext.Recent.Folders);
					RefreshContent();
					return;
			}
			base.ExecuteCommand(commandName, parameters);
		}

		public FrontPageControl() {
			InitializeComponent();
			Text = "主页";
			RefreshContent();
			RecentFileItemClicked = (s, args) => AppContext.MainForm.OpenFileWithEditor(args.ClickedItem.ToolTipText);
			AllowDrop = true;
		}

		protected override void OnDragEnter(DragEventArgs drgevent) {
			base.OnDragEnter(drgevent);
			drgevent.FeedbackDragFileOver(Constants.FileExtensions.PdfAndAllBookmarkExtension);
		}
		protected override void OnDragDrop(DragEventArgs drgevent) {
			base.OnDragDrop(drgevent);
			foreach (var item in drgevent.DropFileOver(Constants.FileExtensions.PdfAndAllBookmarkExtension)) {
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

		void RefreshContent() {
			var s = FormHelper.GetDpiScale(this);
			_FrontPageBox.Text = __FrontPagePattern
						 .Replace(Properties.Resources.FrontPage, $@"<div><a href=""func:$2""><img src=""res:$1"" width=""{s * 16}px"" />$3</a></div>")
						 .Replace("$sideBarWidth", (s * 180).ToText() + "px")
						 .Replace("$appName", Constants.AppName)
						 .Replace("$appHomePage", Constants.AppHomePage)
						 .Replace("<li></li>", GetLastFileList());
		}

		string GetLastFileList() {
			var i = 0;
			return String.Concat(AppContext.Recent.SourcePdfFiles.ConvertAll((s) => FileHelper.IsPathValid(s) && new FilePath(s).ExistsFile
				  ? $"<li><a href=\"recent:{i++}\">{s.SubstringAfter('\\')}</a></li>"
				  : $"<li id=\"{i++}\">{s.SubstringAfter('\\')}</li>"));
		}

		void _FrontPageBox_LinkClicked(object sender, HtmlLinkClickedEventArgs e) {
			HandleLinkClicked(e.Link);
			e.Handled = true;
		}

		void _FrontPageBox_ImageLoad(object sender, HtmlImageLoadEventArgs e) {
			LoadResourceImage(e);
		}

	}
}
