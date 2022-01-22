using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	public partial class RenamePreviewForm : Form
	{
		public RenamePreviewForm(IList<string> sourceFiles, IList<string> targetFiles) {
			InitializeComponent();
			var l = sourceFiles.Count;
			var c = _RenamePreviewBox.Items;
			for (int i = 0; i < l; i++) {
				FilePath s = sourceFiles[i];
				FilePath t = targetFiles[i];
				if (t.IsEmpty) {
					continue;
				}
				if (t.ToString().IndexOf('<') == -1) {
					c.Add(new ListViewItem(new string[]{
						s.FileName,
						t.FileName,
						s.Directory,
						t.Directory
					}));
				}
				else {
					var item = c.Add(new ListViewItem(new string[]{
						s.FileName,
						t,
						s.Directory,
						String.Empty
					}));
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
}
