using System.Drawing;
using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	/// <summary>
	/// Class used to preserve / restore / maximize state of the form
	/// </summary>
	public sealed class FormState
	{
		FormBorderStyle brdStyle;
		bool topMost;
		Rectangle bounds;

		bool IsMaximized;

		public void Maximize(Form targetForm) {
			if (!IsMaximized) {
				IsMaximized = true;
				if (targetForm.WindowState == FormWindowState.Maximized) {
					targetForm.WindowState = FormWindowState.Normal;
				}
				Save(targetForm);
				targetForm.FormBorderStyle = FormBorderStyle.None;
				targetForm.TopMost = true;
				targetForm.Bounds = Screen.FromControl(targetForm).Bounds;
			}
		}

		void Save(Form targetForm) {
			brdStyle = targetForm.FormBorderStyle;
			topMost = targetForm.TopMost;
			bounds = targetForm.Bounds;
		}

		public void Restore(Form targetForm) {
			targetForm.FormBorderStyle = brdStyle;
			targetForm.TopMost = topMost;
			targetForm.Bounds = bounds;
			IsMaximized = false;
		}
	}
}
