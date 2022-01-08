namespace PDFPatcher.Model
{
	interface ITextRegion
	{
		string Text { get; }
		Bound Region { get; }
	}

	interface IDirectionalBoundObject : ITextRegion
	{
		WritingDirection Direction { get; }
	}
}
