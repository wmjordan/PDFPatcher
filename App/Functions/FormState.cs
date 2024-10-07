using System.Drawing;
using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	/// <summary>
	/// 用于保存窗体状态的类。
	/// </summary>
	public sealed class FormState
	{
		FormBorderStyle _borderStyle;
		bool _topMost;
		Rectangle _bounds;

		bool _isMaximized;

		public void Maximize(Form targetForm) {
			if (_isMaximized) {
				return;
			}

			_isMaximized = true;
			if (targetForm.WindowState == FormWindowState.Maximized) {
				targetForm.WindowState = FormWindowState.Normal;
			}
			Save(targetForm);
			targetForm.FormBorderStyle = FormBorderStyle.None;
			targetForm.TopMost = true;
			targetForm.Bounds = Screen.FromControl(targetForm).Bounds;
		}

		void Save(Form targetForm) {
			_borderStyle = targetForm.FormBorderStyle;
			_topMost = targetForm.TopMost;
			_bounds = targetForm.Bounds;
		}

		public void Restore(Form targetForm) {
			targetForm.FormBorderStyle = _borderStyle;
			targetForm.TopMost = _topMost;
			targetForm.Bounds = _bounds;
			_isMaximized = false;
		}
	}
}
