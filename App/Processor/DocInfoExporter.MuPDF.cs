using System;
using System.Xml;
using MuPDF;
using PDFPatcher.Common;

namespace PDFPatcher.Processor
{
	partial class DocInfoExporter
	{
		internal static void ExportColor(PdfArray color, XmlWriter target) {
			if (color is null) {
				return;
			}
			switch (color.Count) {
				case 0:
					target.WriteAttributeString(Constants.Color, Constants.Colors.Transparent);
					break;
				case 1:
					target.WriteAttributeString(Constants.Colors.Gray, color.Get(0).FloatValue.ToText());
					break;
				case 3:
					target.WriteAttributeString(Constants.Colors.Red, color.Get(0).FloatValue.ToText());
					target.WriteAttributeString(Constants.Colors.Green, color.Get(1).FloatValue.ToText());
					target.WriteAttributeString(Constants.Colors.Blue, color.Get(2).FloatValue.ToText());
					break;
				case 4:
					target.WriteAttributeString(Constants.Colors.Cyan, color.Get(0).FloatValue.ToText());
					target.WriteAttributeString(Constants.Colors.Magenta, color.Get(1).FloatValue.ToText());
					target.WriteAttributeString(Constants.Colors.Yellow, color.Get(2).FloatValue.ToText());
					target.WriteAttributeString(Constants.Colors.Black, color.Get(3).FloatValue.ToText());
					break;
			}
		}
	}
}