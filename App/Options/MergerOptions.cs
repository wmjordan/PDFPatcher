using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using PDFPatcher.Model;

namespace PDFPatcher
{
	public class MergerOptions : DocumentOptions
	{
		public MergerOptions() {
			AutoBookmarkTitle = true;
			AutoMaskBWImages = true;
			AutoScaleDown = true;
			FullCompression = true;
			IgnoreLeadingNumbers = true;
			KeepBookmarks = true;
			NumericAwareSort = true;
			RemoveOrphanBookmarks = true;
			PageSettings = new PageBoxSettings();
			SubFolderBeforeFiles = true;
		}

		[XmlElement("页面布局")]
		public PageBoxSettings PageSettings { get; set; }

		///<summary>获取或指定是否自动缩小图片以适合页面。</summary>
		[XmlAttribute("自动缩小")]
		[DefaultValue(true)]
		public bool AutoScaleDown { get; set; }

		///<summary>获取或指定是否自动放大图片以填满页面。</summary>
		[XmlAttribute("自动放大")]
		[DefaultValue(false)]
		public bool AutoScaleUp { get; set; }

		[XmlAttribute("压缩冗余数据")]
		[DefaultValue(false)]
		public bool Deduplicate { get; set; }

		/// <summary>
		/// 获取页面除去上下留白的高度。
		/// </summary>
		[XmlIgnore]
		public float ContentHeight {
			get {
				var ps = PageSettings;
				return ps.PaperSize.Height - ps.Margins.Top - ps.Margins.Bottom;
			}
		}
		/// <summary>
		/// 获取页面除去左右留白的宽度。
		/// </summary>
		[XmlIgnore]
		public float ContentWidth {
			get {
				var ps = PageSettings;
				return ps.PaperSize.Width - ps.Margins.Left - ps.Margins.Right;
			}
		}

		[XmlAttribute("校正图片旋转角度")]
		public bool DeskewImages { get; set; }

		[XmlAttribute("优化黑白图片压缩算法")]
		public bool RecompressWithJbig2 { get; set; }

		//[XmlAttribute ("压缩比")]
		//[DefaultValue (-1)]
		//public int CompressionLevel { get; set; }

		///<summary>获取或指定是否为黑白图片自动设为透明（在阅读器中不能用图像工具选中）。</summary>
		[XmlAttribute("黑白图片自动透明")]
		public bool AutoMaskBWImages { get; set; }

		#region 文件列表选项
		///<summary>获取或指定排序文件时是否按数值和文本排序。</summary>
		[XmlAttribute("按数值排序文件")]
		[DefaultValue(true)]
		public bool NumericAwareSort { get; set; }

		///<summary>获取或指定排序文件时是否按超星阅读器的文件命名排序。</summary>
		[XmlAttribute("按超星阅读器排序文件")]
		[DefaultValue(false)]
		public bool CajSort { get; set; }

		///<summary>获取或指定添加目录时是否将子目录排列在文件前面。</summary>
		[XmlAttribute("子目录排在文件前")]
		[DefaultValue(true)]
		public bool SubFolderBeforeFiles { get; set; }
		#endregion

		#region 自动生成书签选项
		///<summary>获取或指定忽略文件名的前导数值。</summary>
		[XmlAttribute("自动生成书签文本")]
		[DefaultValue(true)]
		public bool AutoBookmarkTitle { get; set; }

		///<summary>获取或指定忽略文件名的前导数值。</summary>
		[XmlAttribute("忽略前导数字")]
		[DefaultValue(true)]
		public bool IgnoreLeadingNumbers { get; set; }

		///<summary>获取或指定是否保留 PDF 文档的书签。</summary>
		[XmlAttribute("保留书签")]
		[DefaultValue(true)]
		public bool KeepBookmarks { get; set; }

		///<summary>获取或指定是否删除没有目标（页面失效）的书签。</summary>
		[XmlAttribute("删除失效书签")]
		[DefaultValue(true)]
		public bool RemoveOrphanBookmarks { get; set; }
		#endregion


	}
}
