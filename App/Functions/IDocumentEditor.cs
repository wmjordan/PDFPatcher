using System;

namespace PDFPatcher.Functions;

internal interface IDocumentEditor
{
	string DocumentPath { get; }
	void Reopen();
}

public sealed class DocumentChangedEventArgs : EventArgs
{
	internal DocumentChangedEventArgs(string path) {
		Path = path;
	}

	public string Path { get; }
}