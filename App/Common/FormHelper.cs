﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PDFPatcher.Common
{
	static class FormHelper
	{
		public const int ProcMsg = NativeMethods.WM_COPYDATA;

		public static bool IsCtrlKeyDown => (Control.ModifierKeys & Keys.Control) != 0;

		public static bool IsShiftKeyDown => (Control.ModifierKeys & Keys.Shift) != 0;

		public static bool IsAltKeyDown => (Control.ModifierKeys & Keys.Alt) != 0;

		public static bool IsEmptyOrTransparent(this Color color) {
			return color.IsEmpty || color.A == 0;
		}
		public static Point Round(this PointF point) {
			return new Point(point.X.ToInt32(), point.Y.ToInt32());
		}
		public static RectangleF Union(this RectangleF rectangle, RectangleF other) {
			return RectangleF.FromLTRB(
				Math.Min(rectangle.Left, other.Left),
				Math.Min(rectangle.Top, other.Top),
				Math.Max(rectangle.Right, other.Right),
				Math.Max(rectangle.Bottom, other.Bottom)
				);
		}
		public static Point Transpose(this Point point, int x, int y) {
			return new Point(point.X + x, point.Y + y);
		}
		public static Point Transpose(this Point point, Point transpose) {
			return new Point(point.X + transpose.X, point.Y + transpose.Y);
		}
		public static Size Scale(this Size size, float scale) {
			return new Size((int)(size.Width * scale), (int)(size.Height * scale));
		}
		public static void OnFirstLoad(this Form form, Action handler) {
			new FormEventHandler(form, handler);
		}
		public static void SetIcon(this Form form, Bitmap bitmap) {
			form.Icon = Icon.FromHandle(bitmap.GetHicon());
		}
		public static void OnFirstLoad(this UserControl control, Action handler) {
			new UserControlLoadHandler(control, handler);
		}
		public static ProgressBar SetValue(this ProgressBar control, int value) {
			control.Value = value < control.Minimum ? control.Minimum
				: value > control.Maximum ? control.Maximum
				: value;
			return control;
		}
		public static NumericUpDown SetValue(this NumericUpDown box, int value) {
			return box.SetValue((decimal)value);
		}
		public static NumericUpDown SetValue(this NumericUpDown box, float value) {
			return box.SetValue((decimal)value);
		}
		public static NumericUpDown SetValue(this NumericUpDown box, double value) {
			return box.SetValue((decimal)value);
		}
		public static NumericUpDown SetValue(this NumericUpDown box, decimal value) {
			box.Value =
				value >= box.Minimum && value <= box.Maximum ? value
				: value > box.Maximum ? box.Maximum
				: box.Minimum;
			return box;
		}
		public static ListBox Select(this ListBox control, string item) {
			if (control.Items.Count == 0) {
				return control;
			}
			var i = control.FindString(item);
			if (i != -1) {
				control.SelectedIndex = i;
			}
			return control;
		}
		public static ComboBox Select(this ComboBox control, string item) {
			if (control.Items.Count == 0) {
				return control;
			}
			var i = control.FindString(item);
			if (i != -1) {
				control.SelectedIndex = i;
			}
			return control;
		}
		public static ListBox Select(this ListBox control, int index) {
			var items = control.Items;
			if (items.Count == 0) {
				return control;
			}
			control.SelectedIndex = index < 0 ? 0
				: index > items.Count - 1 ? items.Count - 1
				: index;
			return control;
		}
		public static ComboBox Select(this ComboBox control, int index) {
			var items = control.Items;
			if (items.Count == 0) {
				return control;
			}
			control.SelectedIndex = index < 0 ? 0
				: index > items.Count - 1 ? items.Count - 1
				: index;
			return control;
		}
		public static ComboBox AddRange(this ComboBox view, params object[] values) {
			view.Items.AddRange(values);
			return view;
		}
		public static TTextBox AppendLine<TTextBox>(this TTextBox box) where TTextBox : TextBoxBase {
			box.AppendText(Environment.NewLine);
			return box;
		}
		public static TTextBox AppendLine<TTextBox>(this TTextBox box, string text) where TTextBox : TextBoxBase {
			box.AppendText(text + Environment.NewLine);
			return box;
		}
		public static void SetLocation(this FileDialog dialog, string path) {
			if (!FileHelper.IsPathValid(path)) {
				return;
			}
			dialog.InitialDirectory = System.IO.Path.GetDirectoryName(path);
			dialog.FileName = System.IO.Path.GetFileName(path);
		}
		public static ToolStrip ToggleEnabled(this ToolStrip toolStrip, bool enabled, params string[] names) {
			foreach (ToolStripItem item in toolStrip.Items) {
				if (Array.IndexOf(names, item.Name) != -1) {
					item.Enabled = enabled;
				}
			}
			return toolStrip;
		}

		public static float GetDpiScale(this Control control) {
			using (var g = control.CreateGraphics()) {
				return g.DpiX / 96;
			}
		}

		public static void ScaleColumnWidths(this ListView listView, float scale) {
			foreach (ColumnHeader column in listView.Columns) {
				column.Width = (int)(column.Width * scale);
			}
		}
		public static void ScaleColumnWidths(this ListView listView) {
			float scale = GetDpiScale(listView);
			foreach (ColumnHeader column in listView.Columns) {
				column.Width = (int)(column.Width * scale);
			}
		}

		public static ToolStrip ScaleIcons(this ToolStrip toolStrip, int size) {
			size = (int)(toolStrip.GetDpiScale() * size);
			return toolStrip.ScaleIcons(new Size(size, size));
		}
		public static ToolStrip ScaleIcons(this ToolStrip toolStrip, Size size) {
			toolStrip.SuspendLayout();
			toolStrip.AutoSize = false;
			toolStrip.ImageScalingSize = size;
			toolStrip.ResumeLayout();
			toolStrip.AutoSize = true;
			return toolStrip;
		}

		internal static void InsertLinkedText(this RichTextBoxLinks.RichTextBoxEx textBox, string text) {
			const int TokenLength = 2;
			int p1 = text.IndexOf("<<");
			int p2 = text.IndexOf(">>");
			if (p1 != -1 && p2 != -1 && p2 > p1) {
				textBox.AppendText(text.Substring(0, p1));
				var c = textBox.SelectionColor;
				var f = textBox.SelectionFont;
				textBox.InsertLink(text.Substring(p1 + TokenLength, p2 - p1 - TokenLength));
				if (p2 < text.Length - TokenLength) {
					textBox.SelectionColor = c;
					textBox.SelectionFont = f;
					textBox.AppendText(text.Substring(p2 + TokenLength));
				}
			}
			else {
				textBox.AppendText(text);
			}
		}

		public static void FeedbackDragFileOver(this DragEventArgs args, params string[] allowedFileExtension) {
			if (args.Data.GetDataPresent(DataFormats.FileDrop)) {
				var files = args.Data.GetData(DataFormats.FileDrop) as string[];
				if (Array.Exists(files,
					f => {
						return Array.Exists(allowedFileExtension,
							ext => f.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase));
					})) {
					args.Effect = DragDropEffects.Copy;
				}
			}
		}
		public static string[] DropFileOver(this DragEventArgs args, params string[] allowedFileExtension) {
			if (args.Data.GetDataPresent(DataFormats.FileDrop)) {
				var files = (string[])args.Data.GetData(DataFormats.FileDrop);
				return Array.FindAll(files, f => {
					return Array.Exists(allowedFileExtension,
							ext => f.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase));
				});
			}
			return [];
		}
		public static bool DropFileOver(this Control control, DragEventArgs args, params string[] allowedFileExtension) {
			var files = DropFileOver(args, allowedFileExtension);
			if (files.Length > 0) {
				control.Text = files[0];
				return true;
			}
			return false;
		}

		public static void HidePopupMenu(this ToolStripItem item) {
			if (item is ToolStripDropDownItem mi && mi.HasDropDownItems) {
				return;
			}
			var oo = item.Owner as ToolStripDropDownMenu;
			oo?.Hide();
			var oi = item.OwnerItem as ToolStripDropDownItem;
			while (oi != null) {
				oi.DropDown.Close();
				oo = oi.Owner as ToolStripDropDownMenu;
				oo?.Hide();
				oi = oi.OwnerItem as ToolStripDropDownItem;
			}
		}
		public static void ClearDropDownItems(this ToolStripItemCollection items, int keepItems = 0) {
			if (items.Count == 0) {
				return;
			}
			keepItems--;
			for (var i = items.Count - 1; i > keepItems; i--) {
				items[i].Dispose();
			}
		}
		public static void ToggleVisibility(bool visible, params Control[] controls) {
			foreach (var item in controls) {
				item.Visible = visible;
			}
		}
		public static DialogResult ShowDialog<TForm>() where TForm : Form, new() {
			using (var f = new TForm()) {
				return f.ShowDialog();
			}
		}
		public static DialogResult ShowDialog<TForm>(this IWin32Window form) where TForm : Form, new() {
			using (var f = new TForm()) {
				f.StartPosition = FormStartPosition.CenterParent;
				return f.ShowDialog(form);
			}
		}
		public static DialogResult ShowDialog<TForm>(this IWin32Window form, object formParameter)
			where TForm : Form, new() {
			using (var f = new TForm()) {
				f.Tag = formParameter;
				return f.ShowDialog(form);
			}
		}
		public static DialogResult ShowDialog<TForm>(this IWin32Window form, Action<TForm> formConfigurator, Action<TForm> formConfirmationHandler) where TForm : Form, new() {
			using (var f = new TForm()) {
				formConfigurator?.Invoke(f);
				var r = f.ShowDialog(form);
				if (formConfirmationHandler != null && (r == DialogResult.OK || r == DialogResult.Yes)) {
					formConfirmationHandler(f);
				}
				return r;
			}
		}
		public static DialogResult ShowCommonDialog<TDialog>(this IWin32Window form, Action<TDialog> formConfigurator, Action<TDialog> formConfirmationHandler) where TDialog : CommonDialog, new() {
			using (var f = new TDialog()) {
				formConfigurator?.Invoke(f);
				var r = f.ShowDialog(form);
				if (formConfirmationHandler != null && (r == DialogResult.OK || r == DialogResult.Yes)) {
					formConfirmationHandler(f);
				}
				return r;
			}
		}

		internal static void ErrorBox(string text) {
			MessageBox.Show(text, Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		public static void ErrorBox(this Control control, string text) {
			MessageBox.Show(text, control.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		public static void ErrorBox(this Control control, string title, Exception exception) {
			var s = new System.Text.StringBuilder(title);
			s.AppendLine().AppendLine();
			s.AppendLine(exception.Message);
#if DEBUG
			s.AppendLine(exception.StackTrace);
#endif
			while ((exception = exception.InnerException) != null) {
				s.AppendLine();
				s.Append(exception.Message);
			}
			MessageBox.Show(s.ToString(), control.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		internal static void InfoBox(string text) {
			MessageBox.Show(text, Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		internal static DialogResult YesNoBox(string text) {
			return MessageBox.Show(text, Constants.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
		}
		internal static DialogResult YesNoCancelBox(string text) {
			return MessageBox.Show(text, Constants.AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
		}
		public static bool ConfirmOKBox(string text) {
			return MessageBox.Show(text, Constants.AppName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK;
		}
		public static bool ConfirmOKBox(this Control control, string text) {
			return MessageBox.Show(text, control.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK;
		}
		public static bool ConfirmYesBox(this Control control, string text) {
			return MessageBox.Show(text, control.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
		}
		public static int SendCopyDataMessage(this System.Diagnostics.Process process, string text) {
			var s = new CopyDataStruct(text);
			var r = NativeMethods.SendMessage(process.MainWindowHandle, ProcMsg, 0, ref s);
			s.Dispose();
			return r;
		}
		public static string GetCopyDataContent(ref Message message) {
			if (message.Msg == ProcMsg) {
				return Marshal.PtrToStringUni(((CopyDataStruct)Marshal.PtrToStructure(message.LParam, typeof(CopyDataStruct))).lpData);
			}
			return null;
		}

		sealed class FormEventHandler
		{
			readonly Form _Form;
			readonly Action _Handler;

			public FormEventHandler(Form form, Action handler) {
				_Form = form;
				_Handler = handler;
				form.Load += OnLoadHandler;
			}
			public void OnLoadHandler(object s, EventArgs args) {
				_Form.Load -= OnLoadHandler;
				_Handler();
			}
		}
		sealed class UserControlLoadHandler
		{
			readonly UserControl _Control;
			readonly Action _Handler;

			public UserControlLoadHandler(UserControl control, Action handler) {
				_Control = control;
				_Handler = handler;
				control.Load += OnLoadHandler;
			}
			public void OnLoadHandler(object s, EventArgs args) {
				_Control.Load -= OnLoadHandler;
				_Handler();
			}
		}

		static class NativeMethods
		{
			const string User32DLL = "User32.dll";
			internal const int WM_COPYDATA = 0x004A;

			[DllImport(User32DLL, SetLastError = false, CharSet = CharSet.Unicode)]
			public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, ref CopyDataStruct lParam);
			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern IntPtr LocalFree(IntPtr p);
		}

		struct CopyDataStruct(string text) : IDisposable
		{
			readonly IntPtr dwData = (IntPtr)1;
			readonly int cbData = (text.Length + 1) * 2;
			internal IntPtr lpData = Marshal.StringToBSTR(text);

			public void Dispose() {
				NativeMethods.LocalFree(lpData);
				lpData = IntPtr.Zero;
			}
		}
	}
}
