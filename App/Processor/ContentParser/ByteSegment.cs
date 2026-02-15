namespace PDFPatcher.Processor.ContentParser;

readonly struct ByteSegment
{
	public readonly byte[] Buffer;
	public readonly int Offset;
	public readonly int Length;

	internal ByteSegment(byte[] buffer, int offset, int length) {
		Buffer = buffer;
		Offset = offset;
		Length = length;
	}
}
