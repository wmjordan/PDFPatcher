using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using PDFPatcher.Functions;
using PDFPatcher.Properties;
using TheArtOfDev.HtmlRenderer.Core.Entities;

namespace PDFPatcher;

[ToolboxItem(false)]
internal sealed partial class AboutControl : HtmlPageControl
{
	public AboutControl() {
		InitializeComponent();
		Text = $"关于 {AssemblyTitle}";
		_FrontPageBox.Text = Resources.AboutPage
			.Replace("$AppName", Constants.AppName)
			.Replace("$AssemblyCopyright", AssemblyCopyright)
			.Replace("$AppHomePage", Constants.AppHomePage)
			.Replace("$AppHubPage", Constants.AppHubPage)
			.Replace("$AssemblyCompany", AssemblyCompany)
			.Replace("$AssemblyVersion", AssemblyVersion);
	}

	public override string FunctionName => "关于 " + AssemblyTitle;

	public override Bitmap IconImage => Resources.About;

	public override void ExecuteCommand(string commandName, params string[] parameters) {
		if (commandName == Commands.CheckUpdate) {
			AppContext.MainForm.ExecuteCommand(commandName);
		}
		else {
			base.ExecuteCommand(commandName, parameters);
		}
	}

	private void _FrontPageBox_ImageLoad(object sender, HtmlImageLoadEventArgs e) {
		LoadResourceImage(e);
	}

	private void _FrontPageBox_LinkClicked(object sender, HtmlLinkClickedEventArgs e) {
		HandleLinkClicked(e.Link);
		e.Handled = true;
	}

	#region 程序集属性访问器

	public string AssemblyTitle {
		get {
			object[] attributes = Assembly.GetExecutingAssembly()
				.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
			if (attributes.Length > 0) {
				AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
				if (titleAttribute.Title != "") {
					return titleAttribute.Title;
				}
			}

			return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
		}
	}

	public string AssemblyVersion => Application.ProductVersion;

	public string AssemblyDescription {
		get {
			object[] attributes = Assembly.GetExecutingAssembly()
				.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
			return attributes.Length == 0 ? string.Empty : ((AssemblyDescriptionAttribute)attributes[0]).Description;
		}
	}

	public string AssemblyProduct {
		get {
			object[] attributes = Assembly.GetExecutingAssembly()
				.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
			return attributes.Length == 0 ? string.Empty : ((AssemblyProductAttribute)attributes[0]).Product;
		}
	}

	public string AssemblyCopyright {
		get {
			object[] attributes = Assembly.GetExecutingAssembly()
				.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			return attributes.Length == 0 ? string.Empty : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
		}
	}

	public string AssemblyCompany {
		get {
			object[] attributes = Assembly.GetExecutingAssembly()
				.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
			return attributes.Length == 0 ? string.Empty : ((AssemblyCompanyAttribute)attributes[0]).Company;
		}
	}

	#endregion
}