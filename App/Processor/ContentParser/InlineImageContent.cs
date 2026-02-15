using System.Collections.Generic;

namespace PDFPatcher.Processor.ContentParser;

readonly struct InlineImageContent
{
	public Dictionary<string, Token> Dictionary { get; }
	public ByteSegment Data { get; }

	internal InlineImageContent(Dictionary<string, Token> dictionary, ByteSegment data) {
		Dictionary = dictionary;
		Data = data;
	}
}