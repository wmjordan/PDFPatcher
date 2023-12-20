using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor
{
	public partial class CustomPatternForm : Form
	{
		public CustomPatternForm() {
			InitializeComponent();
		}

		public string Pattern { get => _PatternBox.Text; set => _PatternBox.Text = value; }
		public bool MatchCase { get => _MatchCaseBox.Checked; set => _MatchCaseBox.Checked = value; }
		public bool FullMatch { get => _FullMatchBox.Checked; set => _FullMatchBox.Checked = value; }

		void _OkButton_Click(object sender, EventArgs e) {
			try {
				new Regex(Pattern);
			}
			catch (Exception ex) {
				this.ErrorBox("正则表达式格式错误", ex);
				return;
			}
			DialogResult = DialogResult.OK;
			Close();
		}

		void _CancelButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
