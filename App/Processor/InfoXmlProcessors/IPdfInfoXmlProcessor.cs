using System;

namespace PDFPatcher.Processor;

internal interface IPdfInfoXmlProcessor
{
	string Name { get; }
	IUndoAction Process(System.Xml.XmlElement item);
}

internal interface IPdfInfoXmlProcessor<T> : IPdfInfoXmlProcessor
{
	T Parameter { get; set; }
}