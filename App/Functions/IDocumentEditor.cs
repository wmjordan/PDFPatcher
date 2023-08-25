using System;

namespace PDFPatcher.Functions
{
	interface IDocumentEditor
	{
		string DocumentPath { get; }
		bool IsBusy { get; }
		event EventHandler<DocumentChangedEventArgs> DocumentChanged;
		void CloseDocument();
		void Reopen();
	}

	public sealed class DocumentChangedEventArgs : EventArgs
	{
		public string Path { get; private set; }
		internal DocumentChangedEventArgs(string path) {
			Path = path;
		}
	}


}
