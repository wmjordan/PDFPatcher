using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PDFPatcher.Common;

namespace PDFPatcher.Model;

[XmlRoot(Constants.Content.Page)]
public class PageSettings
{
	///<summary>获取或指定页面范围的值。</summary>
	[XmlAttribute(Constants.PageRange)]
	public string PageRange { get; set; }

	///<summary>获取或指定页面筛选的值。</summary>
	[XmlAttribute(Constants.PageFilterTypes.ThisName)]
	public string Filter { get; set; }

	///<summary>获取或指定页面尺寸的值。</summary>
	[XmlAttribute(Constants.Content.PageSettings.MediaBox)]
	public string PageSize { get; set; }

	///<summary>获取或指定裁剪框的值。</summary>
	[XmlAttribute(Constants.Content.PageSettings.CropBox)]
	public string CropBox { get; set; }

	///<summary>获取或指定修剪框的值。</summary>
	[XmlAttribute(Constants.Content.PageSettings.TrimBox)]
	public string TrimBox { get; set; }

	///<summary>获取或指定艺术框的值。</summary>
	[XmlAttribute(Constants.Content.PageSettings.ArtBox)]
	public string ArtBox { get; set; }

	///<summary>获取或指定出血框的值。</summary>
	[XmlAttribute(Constants.Content.PageSettings.BleedBox)]
	public string BleedBox { get; set; }

	///<summary>获取或指定旋转角度的值。</summary>
	[XmlAttribute(Constants.Content.PageSettings.Rotation)]
	[DefaultValue(0)]
	public int Rotation { get; set; }

	internal static PageSettings FromReader(PdfReader reader, int pageIndex, UnitConverter converter) {
		Rectangle b;
		PageSettings s = new();
		b = reader.GetPageSize(pageIndex);
		s.PageSize = ConvertPageSize(b, converter);
		b = reader.GetCropBox(pageIndex);
		s.CropBox = b != null ? ConvertPageSize(b, converter) : null;
		b = reader.GetBoxSize(pageIndex, "trim");
		s.TrimBox = b != null ? ConvertPageSize(b, converter) : null;
		b = reader.GetBoxSize(pageIndex, "art");
		s.ArtBox = b != null ? ConvertPageSize(b, converter) : null;
		b = reader.GetBoxSize(pageIndex, "bleed");
		s.BleedBox = b != null ? ConvertPageSize(b, converter) : null;
		s.Rotation = reader.GetPageRotation(pageIndex);
		return s;
	}

	private static string ConvertPageSize(Rectangle b, UnitConverter converter) {
		string[] p = new string[4];
		p[0] = converter.FromPoint(b.Left).ToText("0.###");
		p[1] = converter.FromPoint(b.Bottom).ToText("0.###");
		p[2] = converter.FromPoint(b.Right).ToText("0.###");
		p[3] = converter.FromPoint(b.Top).ToText("0.###");
		return string.Join(" ", p);
	}

	internal static bool HavingSameDimension(PageSettings s1, PageSettings s2) {
		if (s1 == null && s2 == null) {
			return true;
		}

		if (s1 == null || s2 == null) {
			return false;
		}

		if (s1.Rotation != s2.Rotation || s1.PageSize != s2.PageSize
		                               || s1.CropBox != s2.CropBox || s1.TrimBox != s2.TrimBox
		                               || s1.BleedBox != s2.BleedBox || s1.ArtBox != s2.ArtBox) {
			return false;
		}

		return true;
	}

	internal void WriteXml(XmlWriter writer) {
		if (string.IsNullOrEmpty(PageRange)) {
			Debug.WriteLine("Empty page range.");
			return;
		}

		writer.WriteAttributeString(Constants.PageRange, PageRange);
		writer.WriteAttributeString(Constants.Content.PageSettings.MediaBox, PageSize);
		if (CropBox != null) {
			writer.WriteAttributeString(Constants.Content.PageSettings.CropBox, CropBox);
		}

		if (TrimBox != null) {
			writer.WriteAttributeString(Constants.Content.PageSettings.TrimBox, TrimBox);
		}

		if (ArtBox != null) {
			writer.WriteAttributeString(Constants.Content.PageSettings.ArtBox, ArtBox);
		}

		if (BleedBox != null) {
			writer.WriteAttributeString(Constants.Content.PageSettings.BleedBox, BleedBox);
		}

		if (Rotation != 0) {
			writer.WriteAttributeString(Constants.Content.PageSettings.Rotation, Rotation.ToText());
		}
	}
}