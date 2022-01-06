using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using PDFPatcher.Common;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor
{
	sealed class SimpleBookmarkCommand<T> : IEditorCommand where T : IPdfInfoXmlProcessor, new()
	{
		public void Process(Controller controller, params string[] parameters) {
			var b = controller.View.Bookmark;
			if (b.FocusedItem == null) {
				return;
			}
			controller.ProcessBookmarks(new T());
		}
	}

	sealed class SimpleBookmarkCommand<T, P> : IEditorCommand where T : IPdfInfoXmlProcessor<P>, new()
	{
		readonly P _parameter;

		public SimpleBookmarkCommand(P parameter) {
			_parameter = parameter;
		}

		public void Process(Controller controller, params string[] parameters) {
			var b = controller.View.Bookmark;
			if (b.FocusedItem == null) {
				return;
			}
			controller.ProcessBookmarks(new T() { Parameter = _parameter });
		}

	}
}
