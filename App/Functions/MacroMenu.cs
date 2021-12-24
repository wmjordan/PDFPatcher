using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	internal sealed class MacroMenu : ContextMenuStrip
	{
		TextBox __editOperationWrapper = new TextBox ();

		internal const string InsertText = "插入";
		//internal const string Copy = "复制";
		//internal const string Paste = "粘贴";
		//internal const string Cut = "剪切";
		//internal const string Delete = "删除";

		//internal void LoadCopyEditMacros () {
		//    this.Items.AddRange (new ToolStripItem[] {
		//        new ToolStripMenuItem (Cut),
		//        new ToolStripMenuItem (Copy),
		//        new ToolStripMenuItem (Paste),
		//        new ToolStripMenuItem (Delete)
		//    });
		//}
		public MacroMenu () : base() { }
		public MacroMenu (System.ComponentModel.IContainer container) : base(container) { }

		internal void AddInsertMacroMenuItem (string text) {
			this.Items.Add (InsertText + text);
		}

		internal void LoadStandardSourceFileMacros () {
			this.Items.AddRange (new ToolStripItem[] {
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.PathName),
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.FileName),
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.FolderName)
			});
		}

		internal void LoadStandardInfoMacros () {
			this.Items.AddRange (new ToolStripItem[] {
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.TitleProperty),
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.AuthorProperty),
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.SubjectProperty),
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.KeywordsProperty)
			});
		}

		internal void ProcessInsertMacroCommand (object sender, ToolStripItemClickedEventArgs e) {
			var t = e.ClickedItem.Text;
			if (t.StartsWith (InsertText)) {
				t = t.Substring (InsertText.Length);
				var c = this.SourceControl as TextBoxBase;
				if (c != null) {
					c.SelectedText = t;
					return;
				}
				var cb = this.SourceControl as ComboBox;
				if (cb != null) {
					cb.SelectedText = t;
				}
			}
		}

	}
}
