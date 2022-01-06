using System;
using System.ComponentModel;
using System.Diagnostics;

namespace PDFPatcher
{
	static class Tracker
	{
		internal enum Category
		{
			Message = -1,
			ImportantMessage = -2,
			Error = -3,
			Alert = -4,
			Notice = -5,
			InputFile = -10,
			OutputFile = -11
		}

		///<summary>获取或指定后台工作进程。</summary>
		internal static BackgroundWorker Worker { get; set; }

		internal static void TrackProgress(int progress) {
			if (Worker == null) {
				return;
			}
			if (Worker.CancellationPending) {
				throw new OperationCanceledException();
			}
			if (progress >= 0) {
				Worker.ReportProgress(progress);
			}
		}
		internal static void IncrementProgress(int progress) {
			if (Worker == null) {
				return;
			}
			if (Worker.CancellationPending) {
				throw new OperationCanceledException();
			}
			Worker.ReportProgress(progress, "INC");
		}
		internal static void IncrementTotalProgress() {
			if (Worker == null) {
				return;
			}
			Worker.ReportProgress(1, "TINC");
		}
		internal static void SetProgressGoal(int goalNumber) {
			if (Worker == null) {
				return;
			}
			Worker.ReportProgress(goalNumber, "GOAL");
		}
		internal static void SetTotalProgressGoal(int goalNumber) {
			if (Worker == null) {
				return;
			}
			Worker.ReportProgress(goalNumber, "TGOAL");
		}
		internal static void DebugMessage(string message) {
			Debug.Write(DateTime.Now.ToString("HH:mm:ss.fff "));
			Debug.WriteLine(message);
		}
		internal static void TraceMessage(Category level, string message) {
			Trace.Write(DateTime.Now.ToString("HH:mm:ss.fff "));
			Trace.WriteLine(message);
			if (Worker != null && Worker.IsBusy) {
				Worker.ReportProgress((int)level, message);
			}
		}
		internal static void TraceMessage(string message) {
			TraceMessage(Category.Message, message);
		}
		internal static void TraceMessage(Exception exception) {
			TraceMessage(Category.Error, exception.Message);
#if DEBUG
			TraceMessage(Category.Message, exception.StackTrace);
#endif
		}
	}

}