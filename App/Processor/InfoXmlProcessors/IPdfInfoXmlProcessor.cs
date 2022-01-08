namespace PDFPatcher.Processor
{
	interface IPdfInfoXmlProcessor
	{
		string Name { get; }
		IUndoAction Process(System.Xml.XmlElement item);
	}

	interface IPdfInfoXmlProcessor<T> : IPdfInfoXmlProcessor
	{
		T Parameter { get; set; }
	}
}
