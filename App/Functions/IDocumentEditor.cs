using System;

namespace PDFPatcher.Functions
{
	interface IDocumentSource
	{
		string DocumentPath { get; }
	}

	interface IDocumentEditor : IDocumentSource
	{
		bool IsBusy { get; }
		bool IsDirty { get; }
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
