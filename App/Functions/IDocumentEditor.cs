using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Functions;

internal interface IDocumentEditor
{
	event EventHandler<DocumentChangedEventArgs> DocumentChanged;
	string DocumentPath { get; }
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