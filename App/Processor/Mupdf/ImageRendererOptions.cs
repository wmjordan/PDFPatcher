using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

namespace MuPdfSharp
{
	public class ImageRendererOptions
	{
		[XmlAttribute ("自动指定输出位置")]
		public bool AutoOutputFolder { get; set; }

		///<summary>获取或指定导出图片的格式。</summary>
		[XmlAttribute ("图片格式")]
		public ImageFormat FileFormat { get; set; }
		public string FileFormatExtension => FileFormat == ImageFormat.Jpeg ? ".jpg" : FileFormat == ImageFormat.Tiff ? ".tif" : ".png";
		///<summary>获取或指定是否垂直翻转导出的图片。</summary>
		[XmlAttribute ("垂直翻转图片")]
		public bool VerticalFlipImages { get; set; }

		///<summary>获取或指定是否水平翻转导出的图片。</summary>
		[XmlAttribute ("水平翻转图片")]
		public bool HorizontalFlipImages { get; set; }

		/// <summary>获取或指定导出图片的颜色。</summary>
		[XmlAttribute ("图片颜色")]
		public ColorSpace ColorSpace { get; set; }

		/// <summary>获取或指定是否反转图片的颜色。</summary>
		[XmlAttribute ("反转图片颜色")]
		public bool InvertColor { get; set; }

		[XmlAttribute ("JPEG质量")]
		public int JpegQuality { get; set; }

		/// <summary>获取或指定旋转图片的角度。</summary>
		[XmlAttribute ("旋转角度")]
		public int Rotation { get; set; }

		[XmlAttribute ("图片宽度")]
		public int ImageWidth { get; set; }

		[XmlAttribute ("图片比例")]
		public float ScaleRatio { get; set; }

		float _Dpi = 72f;
		[XmlAttribute ("分辨率")]
		public float Dpi { get => _Dpi; set { _Dpi = value > 0 ? value : 72f; } }

		[XmlAttribute ("尺寸模式")]
		public bool UseSpecificWidth { get; set; }

		///<summary>获取或指定导出页面图像所保存的目录路径。</summary>
		[XmlAttribute ("导出路径")]
		public string ExtractImagePath { get; set; }

		///<summary>获取或指定导出文件的名称掩码。</summary>
		[XmlAttribute ("文件名称掩码")]
		public string FileMask { get; set; }

		[XmlIgnore]
		public string ExtractPageRange { get; set; }

		[XmlAttribute ("适合区域")]
		public bool FitArea { get; set; }

		[XmlAttribute ("隐藏批注")]
		public bool HideAnnotations { get; set; }

		[XmlAttribute ("减少颜色")]
		public bool Quantize { get; set; }

		[XmlAttribute("伽马校正")]
		public float Gamma { get; set; }

		[XmlAttribute("染色")]
		public int Tint {
			get => TintColor.ToArgb();
			set {
				TintColor = Color.FromArgb(value);
			}
		}
		[XmlIgnore]
		public Color TintColor { get; set; }

		internal bool LowQuality { get; set; }

		public ImageRendererOptions () {
			AutoOutputFolder = true;
			FileMask = "0000";
			ScaleRatio = 1f;
			Gamma = 1.0f;
			TintColor = Color.Transparent;
		}
	}
}
