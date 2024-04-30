using System;
using System.ComponentModel;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	[ToolboxItem(false)]
	public partial class PatcherOptionForm : Form, IResettableControl
	{
		public PatcherOptionForm() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
		}

		void OnLoad() {
			this.SetIcon(Properties.Resources.PdfOptions);
			_OptionsBox.Options = AppContext.Patcher;
			_OptionsBox.ForEditor = false;
			_OptionsBox.OnLoad();
		}

		public void Reset() {
			Reload();
		}

		public void Reload() {
			_OptionsBox.Reload();
		}

		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			_OptionsBox.Apply();
		}
	}
}
