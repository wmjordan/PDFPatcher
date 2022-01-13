using System;
using System.Drawing;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions;

public partial class RenamePreviewForm : Form
{
	public RenamePreviewForm(string[] sourceFiles, string[] targetFiles) {
		InitializeComponent();
		int l = sourceFiles.Length;
		ListView.ListViewItemCollection c = _RenamePreviewBox.Items;
		FilePath s, t;
		for (int i = 0; i < l; i++) {
			s = sourceFiles[i];
			t = targetFiles[i];
			if (t.IsEmpty) {
				continue;
			}

			if (t.ToString().IndexOf('<') == -1) {
				c.Add(new ListViewItem(new string[] { s.FileName, t.FileName, s.Directory, t.Directory }));
			}
			else {
				ListViewItem item = c.Add(new ListViewItem(new string[] { s.FileName, t, s.Directory, string.Empty }));
				item.UseItemStyleForSubItems = false;
				item.SubItems[1].BackColor = Color.LightYellow;
			}
		}

		foreach (ColumnHeader item in _RenamePreviewBox.Columns) {
			item.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
		}
	}

	private void _OKButton_Click(object sender, EventArgs e) {
		Close();
	}
}