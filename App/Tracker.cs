using System;
using System.ComponentModel;
using System.Diagnostics;

namespace PDFPatcher;

internal static class Tracker
{
	private static BackgroundWorker __Worker;

	///<summary>指定后台工作进程。</summary>
	internal static void SetWorker(BackgroundWorker value) {
		__Worker = value;
	}

	internal static void TrackProgress(int progress) {
		ReportProgress(progress, null);
	}

	internal static void IncrementProgress(int progress) {
		ReportProgress(progress, "INC");
	}

	internal static void IncrementTotalProgress() {
		ReportProgress(1, "TINC");
	}

	internal static void SetProgressGoal(int goalNumber) {
		ReportProgress(goalNumber, "GOAL");
	}

	internal static void SetTotalProgressGoal(int goalNumber) {
		ReportProgress(goalNumber, "TGOAL");
	}

	internal static void DebugMessage(string message) {
		Debug.Write(DateTime.Now.ToString("HH:mm:ss.fff "));
		Debug.WriteLine(message);
	}

	internal static void TraceMessage(Category level, string message) {
		Trace.Write(DateTime.Now.ToString("HH:mm:ss.fff "));
		Trace.WriteLine(message);
		BackgroundWorker worker = __Worker;
		if (worker is { IsBusy: true }) {
			worker.ReportProgress((int)level, message);
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

	private static void ReportProgress(int progress, string category) {
		BackgroundWorker worker = __Worker;
		if (worker == null) {
			return;
		}

		if (worker.CancellationPending) {
			throw new OperationCanceledException();
		}

		worker.ReportProgress(progress, category);
	}

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
}