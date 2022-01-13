using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using PDFPatcher.Model;

namespace PDFPatcher
{
	public class OcrOptions
	{
		//[XmlAttribute ("页码范围")]
		[XmlIgnore] public string PageRanges { get; set; }

		#region 光学字符识别选项

		[XmlAttribute("识别语言")] public int OcrLangID { get; set; }
		[XmlAttribute("旋转校正")] public bool OrientPage { get; set; }
		[XmlAttribute("拉伸校正")] public bool StretchPage { get; set; }
		[XmlIgnore] public float QuantitativeFactor { get; set; }
		[XmlAttribute("排版")] public WritingDirection WritingDirection { get; set; }
		[XmlAttribute("识别分栏")] public bool DetectColumns { get; set; }
		[XmlAttribute("目录识别模式")] public bool DetectContentPunctuations { get; set; }
		[XmlAttribute("压缩空白")] public bool CompressWhiteSpaces { get; set; }
		[XmlAttribute("删除汉字间空白")] public bool RemoveWhiteSpacesBetweenChineseCharacters { get; set; }
		[XmlAttribute("识别前保留图像颜色")] public bool PreserveColor { get; set; }
		[XmlAttribute("导出原始识别结果")] public bool OutputOriginalOcrResult { get; set; }
		[XmlAttribute("在屏幕输出识别文本")] public bool PrintOcrResult { get; set; }
		[XmlIgnore] public string SaveOcredImagePath { get; set; }

		#endregion

		public OcrOptions() {
			OcrLangID = 2052;
			DetectColumns = true;
		}
	}
}