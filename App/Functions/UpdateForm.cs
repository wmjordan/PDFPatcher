using System;
using System.Drawing;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	public partial class UpdateForm : Form
	{
		System.Net.WebClient _UpdateChecker;

		public UpdateForm() {
			InitializeComponent();
			this.SetIcon(Properties.Resources.CheckUpdate);
			Load += (s, args) => {
				CheckNewVersion();
				var i = AppContext.CheckUpdateInterval;
				_CheckUpdateIntervalBox.Select(i == 7 ? 0 : i == 14 ? 1 : i == 30 ? 2 : 3);
			};
			FormClosed += (s, args) => {
				if (_UpdateChecker != null) {
					_UpdateChecker.Dispose();
				}
			};
			_HomePageButton.Click += (s, args) => {
				CommonCommands.VisitHomePage();
			};
			_DownloadButton.Click += (s, args) => {
				System.Diagnostics.Process.Start(_DownloadButton.Tag.ToString());
			};
			_CheckUpdateIntervalBox.SelectedIndexChanged += (s, args) => {
				switch (_CheckUpdateIntervalBox.SelectedIndex) {
					case 0: AppContext.CheckUpdateInterval = 7; break;
					case 1: AppContext.CheckUpdateInterval = 14; break;
					case 2: AppContext.CheckUpdateInterval = 30; break;
					default: AppContext.CheckUpdateInterval = Int32.MaxValue; break;
				}
				if (AppContext.CheckUpdateInterval != Int32.MaxValue) {
					AppContext.CheckUpdateDate = DateTime.Today + TimeSpan.FromDays(AppContext.CheckUpdateInterval);
				}
			};
		}

		private void CheckNewVersion() {
			_UpdateChecker = new System.Net.WebClient();
			_InfoBox.AppendLine("正在检查新版本，请稍候……");
			_UpdateChecker.DownloadDataCompleted += (s, args) => {
				_InfoBox.Clear();
				if (args.Error != null) {
					_InfoBox.AppendText("检查新版本失败：" + args.Error.Message);
					goto Exit;
				}
				try {
					var x = new System.Xml.XmlDocument();
					x.Load(new System.IO.MemoryStream(args.Result));
					CheckResult(x);
				}
				catch (Exception) {
					Common.FormHelper.ErrorBox("版本信息文件格式错误，请稍候重试。");
				}
			Exit:
				_UpdateChecker.Dispose();
				_UpdateChecker = null;
			};
			_UpdateChecker.DownloadDataAsync(new Uri(Constants.AppUpdateFile));
		}

		private void CheckResult(System.Xml.XmlDocument x) {
			var r = x.DocumentElement;
			if (r == null || r.Name != Constants.AppEngName) {
				_InfoBox.SelectionColor = Color.Red;
				_InfoBox.AppendLine("版本信息文件格式错误，请稍候重试。");
			}
			var v = r.GetAttribute("version");
			var d = r.GetAttribute("date");
			var u = r.GetAttribute("url");
			var c = r.SelectSingleNode("content");
			if (new Version(ProductVersion) < new Version(r.GetAttribute("version"))) {
				_InfoBox.SelectionColor = Color.Blue;
				_InfoBox.AppendLine(String.Concat("发现新版本：", v, " ", d));
				_InfoBox.AppendLine(c.InnerText);
				_InfoBox.SelectionStart = 0;
				if (u.Length > 0) {
					_DownloadButton.Enabled = true;
					_DownloadButton.Tag = u;
				}
			}
			else {
				_InfoBox.AppendLine(String.Join("\n", new string[] {
							"未发现新版本。", "服务器上发布的版本是：", v + " " + d
						}));
			}
		}
	}
}
