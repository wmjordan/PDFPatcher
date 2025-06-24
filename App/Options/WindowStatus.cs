using System;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PDFPatcher
{
	public class WindowStatus
	{
		//[XmlAttribute("屏幕")]
		//public int ScreenId { get; set; }

		[XmlAttribute("状态")]
		public FormWindowState State { get; set; }

		[XmlAttribute("左")]
		public int Left { get; set; }

		[XmlAttribute("上")]
		public int Top { get; set; }

		[XmlAttribute("宽")]
		public int Width { get; set; }

		[XmlAttribute("高")]
		public int Height { get; set; }

		public int Right => Left + Width;
		public int Bottom => Top + Height;

		public WindowStatus() {}

		public WindowStatus(Form form) {
			//var s = Screen.FromControl(form);
			//ScreenId = Array.IndexOf(Screen.AllScreens, s);
			State = form.WindowState;
			Left = form.Left;
			Top = form.Top;
			Width = form.Width;
			Height = form.Height;
		}

		public void Position(Form form) {
			var a = Screen.FromControl(form).WorkingArea;
			if (!a.IntersectsWith(new System.Drawing.Rectangle(Left, Top, Width, Height))) {
				return;
			}
			form.StartPosition = FormStartPosition.Manual;
			form.WindowState = State == FormWindowState.Minimized ? FormWindowState.Normal : State;
			form.Left = Left;
			form.Top = Top;
			form.Width = Width;
			form.Height = Height;
		}
	}
}
