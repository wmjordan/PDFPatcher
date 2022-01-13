using System.Xml;

namespace PDFPatcher.Processor;

internal interface IPdfInfoXmlProcessor
{
	string Name { get; }
	IUndoAction Process(XmlElement item);
}

internal interface IPdfInfoXmlProcessor<T> : IPdfInfoXmlProcessor
{
	T Parameter { get; set; }
}