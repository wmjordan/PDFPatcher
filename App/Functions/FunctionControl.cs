using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions;

public class FunctionControl : UserControl
{
	public EventHandler ListRecentFiles;

	public EventHandler<ToolStripItemClickedEventArgs> RecentFileItemClicked;

	protected FunctionControl() {
		ListRecentFiles = (s, args) => {
			ToolStripDropDownItem m = (ToolStripDropDownItem)s;
			ToolStripItemCollection l = m.DropDown.Items;
			l.ClearDropDownItems();
			l.AddSourcePdfFiles();
		};
		RecentFileItemClicked = (s, args) => ExecuteCommand(Commands.OpenFile, args.ClickedItem.ToolTipText);
	}

	[Browsable(false)] public virtual string FunctionName => null;
	[Browsable(false)] public virtual Bitmap IconImage => null;
	[Browsable(false)] public virtual Button DefaultButton => null;

	public void ExecuteCommand(ToolStripItem item) {
		item.HidePopupMenu();
		ExecuteCommand(item.Name);
	}

	public virtual void ExecuteCommand(string commandName, params string[] parameters) {
		if (Commands.OpenFile == commandName) {
			// 将第一个文本框设置为文件路径
			if (parameters.Length > 0 && string.IsNullOrEmpty(parameters[0]) == false
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

	internal void SetupMenu(ToolStripMenuItem menu) {
		SetupMenu(menu.DropDownItems);
	}

	internal void SetupMenu(ToolStripItemCollection items) {
		bool pvs = false; // 前一个可见项目是否为分隔符
		foreach (ToolStripItem item in items) {
			switch (item.Name) {
				case Commands.Action:
					if (DefaultButton != null) {
						Button b = DefaultButton;
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
					ToolStripMenuItem m = item as ToolStripMenuItem;
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
				bool s = item is ToolStripSeparator;
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