namespace PDFPatcher.Model;

internal sealed class CoordinateTranslationSettings
{
	public CoordinateTranslationSettings() {
		XScale = YScale = 1;
	}

	internal float XScale { get; set; }
	internal float YScale { get; set; }
	internal float XTranslation { get; set; }
	internal float YTranslation { get; set; }
}