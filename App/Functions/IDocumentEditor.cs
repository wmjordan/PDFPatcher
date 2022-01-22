using System;

namespace PDFPatcher.Functions
{
	interface IDocumentEditor
	{
		event EventHandler<DocumentChangedEventArgs> DocumentChanged;
		string DocumentPath { get; }
		void CloseDocument();
		void Reopen();
	}

	public sealed class DocumentChangedEventArgs : EventArgs
	{
		public string Path { get; }
		internal DocumentChangedEventArgs(string path) {
			Path = path;
		}
	}


}
