using System.Windows.Forms;

namespace PDFPatcher.Functions
{
	internal sealed class MacroMenu : ContextMenuStrip
	{
		readonly TextBox __editOperationWrapper = new TextBox();

		internal const string InsertText = "插入";

		public MacroMenu() : base() { }
		public MacroMenu(System.ComponentModel.IContainer container) : base(container) { }

		internal void AddInsertMacroMenuItem(string text) {
			Items.Add(InsertText + text);
		}

		internal void LoadStandardSourceFileMacros() {
			Items.AddRange(new ToolStripItem[] {
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.PathName),
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.FileName),
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.FolderName)
			});
		}

		internal void LoadStandardInfoMacros() {
			Items.AddRange(new ToolStripItem[] {
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.TitleProperty),
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.AuthorProperty),
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.SubjectProperty),
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.KeywordsProperty),
				new ToolStripMenuItem (InsertText + Constants.FileNameMacros.PageCount),
			});
		}

		internal void ProcessInsertMacroCommand(object sender, ToolStripItemClickedEventArgs e) {
			var t = e.ClickedItem.Text;
			if (!t.StartsWith(InsertText)) {
				return;
			}

			t = t.Substring(InsertText.Length);
			var c = SourceControl as TextBoxBase;
			if (c != null) {
				c.SelectedText = t;
				return;
			}
			var cb = SourceControl as ComboBox;
			if (cb != null) {
				cb.SelectedText = t;
			}
		}
	}
}
