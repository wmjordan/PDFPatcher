using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	public class FunctionControl : System.Windows.Forms.UserControl
	{
		[Browsable(false)] public virtual string FunctionName => null;
		[Browsable(false)] public virtual System.Drawing.Bitmap IconImage => null;
		[Browsable(false)] public virtual Button DefaultButton => null;

		public EventHandler ListRecentFiles;

		public EventHandler<ToolStripItemClickedEventArgs> RecentFileItemClicked;

		public void ExecuteCommand(ToolStripItem item) {
			item.HidePopupMenu();
			ExecuteCommand(item.Name);
		}

		public virtual void ExecuteCommand(string commandName, params string[] parameters) {
			if (Commands.OpenFile == commandName) {
				// 将第一个文本框设置为文件路径
				if (parameters.Length > 0 && String.IsNullOrEmpty(parameters[0]) == false
				                          && FileHelper.HasExtension(parameters[0], Constants.FileExtensions.Pdf)
				   ) {
					foreach (Control c in Controls) {
						if (c is SourceFileControl i) {
							i.Text = parameters[0];
							return;
						}
					}
				}
			}
		}

		public virtual void SetupCommand(ToolStripItem item) { }
		internal virtual void OnSelected() { }
		internal virtual void OnDeselected() { }

		protected FunctionControl() {
			ListRecentFiles = (s, args) => {
				var m = (ToolStripDropDownItem)s;
				var l = m.DropDown.Items;
				l.ClearDropDownItems();
				l.AddSourcePdfFiles();
			};
			RecentFileItemClicked = (s, args) => ExecuteCommand(Commands.OpenFile, args.ClickedItem.ToolTipText);
		}

		internal void SetupMenu(ToolStripMenuItem menu) {
			SetupMenu(menu.DropDownItems);
		}

		internal void SetupMenu(ToolStripItemCollection items) {
			var pvs = false; // 前一个可见项目是否为分隔符
			foreach (ToolStripItem item in items) {
				switch (item.Name) {
					case Commands.Action:
						if (DefaultButton != null) {
							var b = DefaultButton;
							item.Image = b.Image;
							item.Text = b.Text.Trim();
							item.ToolTipText = b.Tag as string;
						}

						EnableCommand(item, true, true);
						break;
					case Commands.SaveBookmark:
						item.Text = "保存书签文件(&Q)";
						item.ToolTipText = "将书签保存为 XML 格式的信息文件，可用于迁移书签";
						goto default;
					case Commands.ResetOptions:
						EnableCommand(item, this is IResettableControl, true);
						break;
					case Commands.ShowGeneralToolbar:
						var m = item as ToolStripMenuItem;
						m.Checked = AppContext.Toolbar.ShowGeneralToolbar;
						break;
					default:
						EnableCommand(item,
							Commands.DefaultDisabledItems.Contains(item.Name) == false,
							Commands.DefaultHiddenItems.Contains(item.Name) == false
						);
						break;
				}

				SetupCommand(item);
				if (item.Visible) {
					var s = item is ToolStripSeparator;
					if (s) {
						item.Visible = pvs == false;
						pvs = true;
					}
					else {
						pvs = false;
					}
				}
			}
		}

		internal void EnableCommand(ToolStripItem item, bool enabled, bool visible) {
			if (item == null) {
				return;
			}

			item.Enabled = enabled;
			item.Visible = visible;
		}
	}
}