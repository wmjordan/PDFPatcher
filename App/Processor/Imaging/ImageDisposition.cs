using System;
using System.Diagnostics;
using iTextSharp.text.pdf.parser;
using PDFPatcher.Common;

namespace PDFPatcher.Processor.Imaging
{
	[DebuggerDisplay("{Image.InlineImage}: {X}*{XScale},{Y}*{YScale};Flip={Image.VerticalFlip}")]
	internal sealed class ImageDisposition : IComparable<ImageDisposition>
	{
		public ImageInfo Image { get; set; }
		public float X { get; }
		public float Y { get; }
		public float Z { get; }
		public Matrix Ctm { get; }
		public float XScale => Ctm[Matrix.I11] / Image.Width;
		public float YScale => Ctm[Matrix.I22] / Image.Height;
		public ImageDisposition(Matrix ctm, ImageInfo image) {
			Image = image;
			Ctm = ctm;
			var v = new Vector(0, 0, 1).Cross(ctm);
			X = v[0]; Y = v[1]; Z = v[2];
			image.VerticalFlip = Ctm[Matrix.I22] < 0;
		}
		public override string ToString() {
			return $"{(Image.InlineImage.PdfRef != null ? Image.InlineImage.PdfRef.Number.ToText() : "内嵌图像")}:{X},{Y},{Z}";
		}

		#region IComparable<ImageDisposition> 成员

		public int CompareTo(ImageDisposition other) {
			return Y < other.Y ? 1 :
				Y > other.Y ? -1 :
				X > other.X ? 1 :
				X < other.X ? -1 : 0;
		}

		#endregion
	}

}
