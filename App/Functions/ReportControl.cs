using System;
using System.IO;
using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	public partial class ReportControl : UserControl
	{
		public ReportControl () {
			InitializeComponent ();
		}

		protected override void OnVisibleChanged (EventArgs e) {
			base.OnVisibleChanged (e);
			if (Visible) {
				FindForm ().AcceptButton = _CancelButton;
				_CancelButton.Focus ();
			}
		}

		private void _CancelButton_Click (object sender, EventArgs e) {
			if (AppContext.MainForm.IsWorkerBusy == false) {
				this.Hide ();
			}
			else {
				if (Common.FormHelper.YesNoBox ("程序正在工作，是否终止执行？") == DialogResult.Yes) {
					AppContext.MainForm.GetWorker ().CancelAsync ();
					AppContext.Abort = true;
				}
			}
		}


		#region IDefaultButtonControl 成员

		//public override Button DefaultButton {
		//	get { return _CancelButton; }
		//}

		#endregion

		internal void SetGoal (int goalValue) {
			_ProgressBar.Value = 0;
			_ProgressBar.Maximum = goalValue;
		}
		internal void SetTotalGoal (int goalValue) {
			_TotalProgressBar.Value = 0;
			_TotalProgressBar.Maximum = goalValue;
		}
		internal void SetProgress (int p) {
			this._ProgressBar.Value = p > _ProgressBar.Maximum ? _ProgressBar.Maximum : p;
		}
		internal void IncrementProgress (int progress) {
			_ProgressBar.Increment (progress);
		}
		internal void IncrementTotalProgress () {
			try {
				_TotalProgressBar.Value++;
			}
			catch (ArgumentException) {
				System.Diagnostics.Debug.WriteLine ("Total Progress too big: " + _TotalProgressBar.Value);
			}
		}
		internal void PrintMessage (string text, Tracker.Category category) {
			switch (category) {
				case Tracker.Category.Message:
					goto default;
				case Tracker.Category.ImportantMessage:
					this._LogBox.SelectionColor = System.Drawing.Color.DarkBlue;
					Common.FormHelper.InsertLinkedText (this._LogBox, text);
					this._LogBox.AppendText (Environment.NewLine);
					break;
				case Tracker.Category.Alert:
					this._LogBox.SelectionFont = new System.Drawing.Font (this._LogBox.Font, System.Drawing.FontStyle.Bold);
					this._LogBox.SelectionColor = System.Drawing.Color.Blue;
					Common.FormHelper.InsertLinkedText (this._LogBox, text);
					this._LogBox.AppendText (Environment.NewLine);
					break;
				case Tracker.Category.Error:
					this._LogBox.SelectionFont = new System.Drawing.Font (this._LogBox.Font, System.Drawing.FontStyle.Bold);
					this._LogBox.SelectionColor = System.Drawing.Color.Red;
					goto default;
				case Tracker.Category.Notice:
					this._LogBox.SelectionColor = System.Drawing.Color.DarkMagenta;
					goto default;
				case Tracker.Category.InputFile:
					this._InputFileBox.Text = text;
					break;
				case Tracker.Category.OutputFile:
					this._OutputFileBox.Text = text;
					break;
				default:
					this._LogBox.AppendText (text);
					this._LogBox.AppendText (Environment.NewLine);
					break;
			}
		}
		internal void Reset () {
			_LogBox.Clear ();
			_ProgressBar.Value = 0;
			_TotalProgressBar.Value = 0;
			_InputFileBox.Text = String.Empty;
			_OutputFileBox.Text = String.Empty;
			_CancelButton.Text = "取消";
			_CancelButton.Image = Properties.Resources.Reset;
		}
		internal void Complete () {
			_ProgressBar.Value = _ProgressBar.Maximum;
			_TotalProgressBar.Value = _TotalProgressBar.Maximum;
			_CancelButton.Text = "返回";
			_CancelButton.Image = Properties.Resources.Return;
		}

		private void _LogBox_LinkClicked (object sender, LinkClickedEventArgs e) {
			var f = e.LinkText;
			if (File.Exists (f) || Directory.Exists (f)) {
				try {
					System.Diagnostics.Process.Start (f);
				}
				catch (Exception) {
					Common.FormHelper.ErrorBox ("无法打开文件：" + f);
				}
			}
		}

	}
}
