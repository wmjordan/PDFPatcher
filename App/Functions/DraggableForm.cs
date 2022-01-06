using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	public class DraggableForm : Form
	{
		protected override void OnMouseDown(MouseEventArgs args) {
			if (args.Button == MouseButtons.Left) {
				NativeMethods.ReleaseCapture();
				NativeMethods.SendMessage(Handle, 0xa1, (IntPtr)0x2, (IntPtr)0);
			}
			base.OnMouseMove(args);
		}

		static class NativeMethods
		{
			#region Form Dragging API Support
			//The SendMessage function sends a message to a window or windows.
			[DllImport("user32.dll", SetLastError = false)]
			internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

			//ReleaseCapture releases a mouse capture
			[DllImport("user32.dll", SetLastError = false)]
			internal static extern bool ReleaseCapture();
			#endregion
		}
	}
}
