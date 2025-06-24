using System;
using System.Windows.Forms;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor
{
	sealed class BookmarkPageCommand : IEditorCommand
	{
		readonly int _number;
		bool _takeFollowing;

		public BookmarkPageCommand(int number, bool takeFollowing = false) {
			_number = number;
			_takeFollowing = takeFollowing;
		}

		public void Process(Controller controller, params string[] parameters) {
			var n = _number;
			if (_number == 0) {
				using (var form = new ShiftPageNumberEntryForm() { TakeFollowing = _takeFollowing }) {
					if (form.ShowDialog() != DialogResult.OK
						|| form.ShiftNumber == 0 && !form.TakeFollowing) {
						return;
					}
					n = form.ShiftNumber;
					_takeFollowing = form.TakeFollowing;
				}
			}
			controller.ProcessBookmarks(new ChangePageNumberProcessor(n, false, true, _takeFollowing));
		}

	}
}
