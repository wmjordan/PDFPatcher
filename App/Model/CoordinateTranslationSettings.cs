using iTextSharp.text.pdf.parser;

namespace PDFPatcher.Model;

internal sealed class CoordinateTranslationSettings
{
	public CoordinateTranslationSettings() {
		XScale = YScale = 1;
	}

	public CoordinateTranslationSettings(float xScale, float yScale, float xTranslation, float yTranslation) {
		XScale = xScale;
		YScale = yScale;
		XTranslation = xTranslation;
		YTranslation = yTranslation;
	}

	internal float XScale { get; set; }
	internal float YScale { get; set; }
	internal float XTranslation { get; set; }
	internal float YTranslation { get; set; }

	internal Matrix GetMatrix() {
		return new Matrix(XScale, 0, 0, YScale, XTranslation, YTranslation);
	}
}