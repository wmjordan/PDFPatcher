using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PDFPatcher.Common;
using TheArtOfDev.HtmlRenderer.Core.Entities;

namespace PDFPatcher
{
	[ToolboxItem(false)]
	sealed partial class FrontPageControl : Functions.HtmlPageControl
	{
		readonly Regex __FrontPagePattern = new Regex ("<div>(.+);(.+);(.+);(.+)</div>", RegexOptions.CultureInvariant);

		public override string FunctionName => "主页";

		public override System.Drawing.Bitmap IconImage => Properties.Resources.HomePage;

		public override void ExecuteCommand (string commandName, params string[] parameters) {
			if (commandName == Commands.Open) {
				var n = AppContext.MainForm.ShowPdfFileDialog ();
				if (n != null) {
					AppContext.MainForm.OpenFileWithEditor (n);
				}
				return;
			}
			else if (commandName == Commands.CleanUpInexistentFiles) {
				AppContext.CleanUpInexistentFiles (AppContext.Recent.SourcePdfFiles);
				AppContext.CleanUpInexistentFiles (AppContext.Recent.InfoDocuments);
				AppContext.CleanUpInexistentFolders (AppContext.Recent.Folders);
				RefreshContent ();
				return;
			}
			base.ExecuteCommand (commandName, parameters);
		}

		public FrontPageControl () {
			InitializeComponent ();
			Text = "主页";
			RefreshContent ();
			RecentFileItemClicked = (s, args) => { AppContext.MainForm.OpenFileWithEditor (args.ClickedItem.ToolTipText); };
			AllowDrop = true;
		}

		protected override void OnDragEnter (DragEventArgs drgevent) {
			base.OnDragEnter (drgevent);
			FormHelper.FeedbackDragFileOver (drgevent, Constants.FileExtensions.PdfAndAllBookmarkExtension);
		}
		protected override void OnDragDrop (DragEventArgs drgevent) {
			base.OnDragDrop (drgevent);
			var files = FormHelper.DropFileOver (drgevent, Constants.FileExtensions.PdfAndAllBookmarkExtension);
			foreach (var item in files) {
				AppContext.MainForm.OpenFileWithEditor (item);
			}
		}
		internal override void OnSelected () {
			base.OnSelected ();
			RefreshContent ();
		}
		public override void SetupCommand (ToolStripItem item) {
			switch (item.Name) {
				case Commands.Close:
				case Commands.Action:
					EnableCommand (item, false, true);
					break;
			}
			base.SetupCommand (item);
		}

		private void RefreshContent () {
			_FrontPageBox.Text = __FrontPagePattern
						 .Replace (Properties.Resources.FrontPage, @"<div><a href=""func:$2""><img src=""res:$1"" />$3</a></div>")
						 .Replace ("$appName", Constants.AppName)
						 .Replace ("$AppHomePage", Constants.AppHomePage)
						 .Replace ("<li></li>", GetLastFileList ());
		}

		private string GetLastFileList () {
			var i = 0;
			return String.Concat (AppContext.Recent.SourcePdfFiles.ConvertAll ((s) => FileHelper.IsPathValid(s) && System.IO.File.Exists(s)
					? String.Concat(@"<li><a href=""recent:", i++, "\">", SubstringAfter(s, '\\'), "</a></li>")
					: String.Concat(@"<li id=""", i++, "\">", SubstringAfter(s, '\\'), "</li>")));
		}

		private void _FrontPageBox_LinkClicked (object sender, HtmlLinkClickedEventArgs e) {
			HandleLinkClicked (e.Link);
			e.Handled = true;
		}

		private void _FrontPageBox_ImageLoad (object sender, HtmlImageLoadEventArgs e) {
			LoadResourceImage (e);
		}

	}
}
