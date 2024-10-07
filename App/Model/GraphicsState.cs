using System;
using iTextSharp.text.pdf.parser;

namespace PDFPatcher.Model
{
	internal sealed class GraphicsState
	{
		internal Matrix TransMatrix { get; set; }
		internal float CharacterSpacing { get; set; }
		internal float WordSpacing { get; set; }
		internal float HorizontalScaling { get; set; }
		internal float Leading { get; set; }
		internal int FontID { get; set; }
		internal FontInfo Font { get; set; }
		internal float FontSize { get; set; }
		internal int RenderMode { get; set; }
		internal float Rise { get; set; }
		internal bool KnockOut { get; set; }

		/**
         * Constructs a new Graphics State object with the default values.
         */
		public GraphicsState() {
			TransMatrix = new Matrix();
			CharacterSpacing = 0;
			WordSpacing = 0;
			HorizontalScaling = 1.0f;
			Leading = 0;
			Font = null;
			FontSize = 0;
			RenderMode = 0;
			Rise = 0;
			KnockOut = true;
		}

		public GraphicsState Copy() {
			return MemberwiseClone() as GraphicsState;
		}
	}
}
