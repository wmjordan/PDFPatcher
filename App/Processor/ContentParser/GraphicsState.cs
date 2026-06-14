using System;
using MuPDF;

namespace PDFPatcher.Processor.ContentParser;

sealed class GraphicsState
{
	internal FontDescriptor CurrentFont { get; set; }
	public string CurrentFontName => CurrentFont?.Font?.Name;
	public double FontSize { get; internal set; }
	public double CharSpacing { get; internal set; }
	public double WordSpacing { get; internal set; }
	public double HorizontalScaling { get; internal set; }
	public double TextLeading { get; internal set; }
	public double TextRise { get; internal set; }
	public Matrix TextMatrix { get; internal set; }
	public Matrix TextLineMatrix { get; internal set; }
	public Matrix Ctm { get; internal set; } // Current Transformation Matrix
	public string Text { get; internal set; } // last output text

	public GraphicsState Clone() => (GraphicsState)MemberwiseClone();
}