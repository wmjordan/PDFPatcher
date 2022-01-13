using System.Windows.Forms;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor;

internal sealed class BookmarkPageCommand : IEditorCommand
{
	private readonly int _number;

	public BookmarkPageCommand(int number) {
		_number = number;
	}

	public void Process(Controller controller, params string[] parameters) {
		int n = _number;
		if (_number == 0) {
			using (ShiftPageNumberEntryForm form = new()) {
				if (form.ShowDialog() != DialogResult.OK || form.ShiftNumber == 0) {
					return;
				}

				n = form.ShiftNumber;
			}
		}

		controller.ProcessBookmarks(new ChangePageNumberProcessor(n, false, true));
	}
}