using System;

namespace PDFPatcher.Processor;

internal interface IInfoDocProcessor
{
	bool Process(System.Xml.XmlElement item);
}