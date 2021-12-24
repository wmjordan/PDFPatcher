using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model
{
	internal sealed class CoordinateTranslationSettings
	{
		internal float XScale { get; set; }
		internal float YScale { get; set; }
		internal float XTranslation { get; set; }
		internal float YTranslation { get; set; }

		public CoordinateTranslationSettings () {
			XScale = YScale = 1;
		}

		public CoordinateTranslationSettings (float xScale, float yScale, float xTranslation, float yTranslation) {
			this.XScale = xScale;
			this.YScale = yScale;
			this.XTranslation = xTranslation;
			this.YTranslation = yTranslation;
		}

		internal iTextSharp.text.pdf.parser.Matrix GetMatrix () {
			return new iTextSharp.text.pdf.parser.Matrix (this.XScale, 0, 0, this.YScale, this.XTranslation, this.YTranslation);
		}
	}
}
