using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace PDFPatcher
{
	[ToolboxItem(false)]
	sealed partial class AboutControl : Functions.HtmlPageControl
	{
		public override string FunctionName => "关于 " + AssemblyTitle;

		public override Bitmap IconImage => Properties.Resources.About;

		public AboutControl() {
			InitializeComponent();
			Text = $"关于 {AssemblyTitle}";
			_FrontPageBox.Text = Properties.Resources.AboutPage
				.Replace("$AppName", Constants.AppName)
				.Replace("$AssemblyCopyright", AssemblyCopyright)
				.Replace("$AppHomePage", Constants.AppHomePage)
				.Replace("$AppHubPage", Constants.AppHubPage)
				.Replace("$AssemblyCompany", AssemblyCompany)
				.Replace("$AssemblyVersion", AssemblyVersion);
		}

		#region 程序集属性访问器

		public string AssemblyTitle {
			get {
				var attributes = Assembly.GetExecutingAssembly()
					.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0) {
					var titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "") {
						return titleAttribute.Title;
					}
				}

				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion => Application.ProductVersion;

		public string AssemblyDescription {
			get {
				var attributes = Assembly.GetExecutingAssembly()
					.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				return attributes.Length == 0
					? String.Empty
					: ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct {
			get {
				var attributes = Assembly.GetExecutingAssembly()
					.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				return attributes.Length == 0 ? String.Empty : ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright {
			get {
				var attributes = Assembly.GetExecutingAssembly()
					.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				return attributes.Length == 0 ? String.Empty : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany {
			get {
				var attributes = Assembly.GetExecutingAssembly()
					.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				return attributes.Length == 0 ? String.Empty : ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}

		#endregion

		public override void ExecuteCommand(string commandName, params string[] parameters) {
			if (commandName == Commands.CheckUpdate) {
				AppContext.MainForm.ExecuteCommand(commandName);
			}
			else {
				base.ExecuteCommand(commandName, parameters);
			}
		}

		private void _FrontPageBox_ImageLoad(object sender,
			TheArtOfDev.HtmlRenderer.Core.Entities.HtmlImageLoadEventArgs e) {
			LoadResourceImage(e);
		}

		private void _FrontPageBox_LinkClicked(object sender,
			TheArtOfDev.HtmlRenderer.Core.Entities.HtmlLinkClickedEventArgs e) {
			HandleLinkClicked(e.Link);
			e.Handled = true;
		}
	}
}