using System.Xml;

namespace PDFPatcher.Processor;

internal interface IInfoDocProcessor
{
    bool Process(XmlElement item);
}