using System;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor.Parts
{
	sealed class CurrentPageBoxHandler
	{
		readonly ToolStripTextBox _PageBox;
		readonly ViewerControl _ViewerBox;

		public CurrentPageBoxHandler(ToolStripTextBox pageBox, ViewerControl viewer) {
			_PageBox = pageBox;
			_ViewerBox = viewer;

			pageBox.KeyUp += HandlePageKeyUp;
			((TextBox)pageBox.Control).MouseWheel += HandlePageBoxMouseWheel;
			viewer.DocumentLoaded += HandleViewerDocumentLoaded;
			viewer.PageChanged += HandleViewerPageChanged;
		}

		void HandlePageBoxMouseWheel(object sender, MouseEventArgs e) {
			if (e.Delta < 0) {
				_ViewerBox.ExecuteCommand(EditorCommands.NextPage);
			}
			else if (e.Delta > 0) {
				_ViewerBox.ExecuteCommand(EditorCommands.PreviousPage);
			}
		}

		void HandlePageKeyUp(object sender, KeyEventArgs e) {
			int d;
			switch (e.KeyCode) {
				case Keys.Enter:
					d = 0;
					break;
				case Keys.Up:
				case Keys.OemMinus:
					d = -1;
					break;
				case Keys.Down:
				case Keys.Add:
					d = 1;
					break;
				case Keys.Home:
					_ViewerBox.CurrentPageNumber = 1;
					return;
				case Keys.End:
					_ViewerBox.CurrentPageNumber = -1;
					return;
				default:
					return;
			}
			if (_PageBox.Text.TryParse(out int p)) {
				_ViewerBox.CurrentPageNumber = p + d;
			}
		}

		void HandleViewerDocumentLoaded(object sender, EventArgs e) {
			_PageBox.ToolTipText = $"文档共{_ViewerBox.Document.PageCount}页\nHome：转到第一页\nEnd：转到最后一页";
		}

		void HandleViewerPageChanged(object sender, EventArgs e) {
			_PageBox.Text = _ViewerBox.CurrentPageNumber.ToText();
		}
	}
}
