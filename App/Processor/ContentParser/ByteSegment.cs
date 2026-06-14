namespace PDFPatcher.Processor.ContentParser;

readonly record struct ByteSegment(byte[] Buffer, int Offset, int Length);
