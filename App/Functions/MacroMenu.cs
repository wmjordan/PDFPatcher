using System.ComponentModel;
using System.Windows.Forms;

namespace PDFPatcher.Functions;

internal sealed class MacroMenu : ContextMenuStrip
{
	internal const string InsertText = "插入";

	private readonly TextBox __editOperationWrapper = new();
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
	public MacroMenu() { }
	public MacroMenu(IContainer container) : base(container) { }

	internal void AddInsertMacroMenuItem(string text) {
		Items.Add(InsertText + text);
	}

	internal void LoadStandardSourceFileMacros() {
		Items.AddRange(new ToolStripItem[] {
			new ToolStripMenuItem(InsertText + Constants.FileNameMacros.PathName),
			new ToolStripMenuItem(InsertText + Constants.FileNameMacros.FileName),
			new ToolStripMenuItem(InsertText + Constants.FileNameMacros.FolderName)
		});
	}

	internal void LoadStandardInfoMacros() {
		Items.AddRange(new ToolStripItem[] {
			new ToolStripMenuItem(InsertText + Constants.FileNameMacros.TitleProperty),
			new ToolStripMenuItem(InsertText + Constants.FileNameMacros.AuthorProperty),
			new ToolStripMenuItem(InsertText + Constants.FileNameMacros.SubjectProperty),
			new ToolStripMenuItem(InsertText + Constants.FileNameMacros.KeywordsProperty)
		});
	}

	internal void ProcessInsertMacroCommand(object sender, ToolStripItemClickedEventArgs e) {
		string t = e.ClickedItem.Text;
		if (!t.StartsWith(InsertText)) {
			return;
		}

		t = t.Substring(InsertText.Length);
		if (SourceControl is TextBoxBase c) {
			c.SelectedText = t;
			return;
		}

		if (SourceControl is ComboBox cb) {
			cb.SelectedText = t;
		}
	}
}