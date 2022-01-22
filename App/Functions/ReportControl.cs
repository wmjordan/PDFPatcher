using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Properties;

namespace PDFPatcher.Functions;

public partial class ReportControl : UserControl
{
	public ReportControl() {
		InitializeComponent();
	}

	protected override void OnVisibleChanged(EventArgs e) {
		base.OnVisibleChanged(e);
		if (!Visible) {
			return;
		}

		FindForm().AcceptButton = _CancelButton;
		_CancelButton.Focus();
	}

	private void _CancelButton_Click(object sender, EventArgs e) {
		if (AppContext.MainForm.IsWorkerBusy == false) {
			Hide();
		}
		else {
			if (FormHelper.YesNoBox("程序正在工作，是否终止执行？") != DialogResult.Yes) {
				return;
			}

			AppContext.MainForm.GetWorker().CancelAsync();
			AppContext.Abort = true;
		}
	}

	internal void SetGoal(int goalValue) {
		_ProgressBar.Value = 0;
		_ProgressBar.Maximum = goalValue;
	}

	internal void SetTotalGoal(int goalValue) {
		_TotalProgressBar.Value = 0;
		_TotalProgressBar.Maximum = goalValue;
	}

	internal void SetProgress(int p) {
		_ProgressBar.Value = p > _ProgressBar.Maximum ? _ProgressBar.Maximum : p;
	}

	internal void IncrementProgress(int progress) {
		_ProgressBar.Increment(progress);
	}

	internal void IncrementTotalProgress() {
		try {
			_TotalProgressBar.Value++;
		}
		catch (ArgumentException) {
			Debug.WriteLine("Total Progress too big: " + _TotalProgressBar.Value);
		}
	}

	internal void PrintMessage(string text, Tracker.Category category) {
		switch (category) {
			case Tracker.Category.Message:
				goto default;
			case Tracker.Category.ImportantMessage:
				_LogBox.SelectionColor = Color.DarkBlue;
				FormHelper.InsertLinkedText(_LogBox, text);
				_LogBox.AppendText(Environment.NewLine);
				break;
			case Tracker.Category.Alert:
				_LogBox.SelectionFont = new Font(_LogBox.Font, FontStyle.Bold);
				_LogBox.SelectionColor = Color.Blue;
				FormHelper.InsertLinkedText(_LogBox, text);
				_LogBox.AppendText(Environment.NewLine);
				break;
			case Tracker.Category.Error:
				_LogBox.SelectionFont = new Font(_LogBox.Font, FontStyle.Bold);
				_LogBox.SelectionColor = Color.Red;
				goto default;
			case Tracker.Category.Notice:
				_LogBox.SelectionColor = Color.DarkMagenta;
				goto default;
			case Tracker.Category.InputFile:
				_InputFileBox.Text = text;
				break;
			case Tracker.Category.OutputFile:
				_OutputFileBox.Text = text;
				break;
			default:
				_LogBox.AppendText(text);
				_LogBox.AppendText(Environment.NewLine);
				break;
		}
	}

	internal void Reset() {
		_LogBox.Clear();
		_ProgressBar.Value = 0;
		_TotalProgressBar.Value = 0;
		_InputFileBox.Text = string.Empty;
		_OutputFileBox.Text = string.Empty;
		_CancelButton.Text = "取消";
		_CancelButton.Image = Resources.Reset;
	}

	internal void Complete() {
		_ProgressBar.Value = _ProgressBar.Maximum;
		_TotalProgressBar.Value = _TotalProgressBar.Maximum;
		_CancelButton.Text = "返回";
		_CancelButton.Image = Resources.Return;
	}

	private void _LogBox_LinkClicked(object sender, LinkClickedEventArgs e) {
		string f = e.LinkText;
		if (!File.Exists(f) && !Directory.Exists(f)) {
			return;
		}

		try {
			Process.Start(f);
		}
		catch (Exception) {
			FormHelper.ErrorBox("无法打开文件：" + f);
		}
	}


	#region IDefaultButtonControl 成员

	//public override Button DefaultButton {
	//	get { return _CancelButton; }
	//}

	#endregion
}