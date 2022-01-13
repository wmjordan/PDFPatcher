using System;

namespace PDFPatcher.Processor
{
	interface IInfoDocProcessor
	{
		bool Process(System.Xml.XmlElement item);
	}
}