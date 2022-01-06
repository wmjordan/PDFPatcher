using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using PDFPatcher.Model;

namespace PDFPatcher
{
	public class AutoBookmarkOptions
	{
		public readonly static Regex NumericPattern = new Regex("^[0-9]+$", RegexOptions.Compiled);

		#region 识别选项
		/// <summary>
		/// 页码范围。
		/// </summary>
		//[XmlAttribute ("页码范围")]
		[XmlIgnore]
		[PowerJson.JsonInclude(false)]
		public string PageRanges { get; set; }

		/// <summary>
		/// 最小标题字体尺寸。
		/// </summary>
		[XmlAttribute("最小标题尺寸")]
		public float TitleThreshold { get; set; }

		/// <summary>
		/// 每页第一行作为标题。
		/// </summary>
		[XmlAttribute("第一行为标题")]
		public bool FirstLineAsTitle { get; set; }

		/// <summary>
		/// 忽略只有一个字符的标题。
		/// </summary>
		[XmlAttribute("忽略单字符标题")]
		public bool IgnoreSingleCharacterTitle { get; set; }

		/// <summary>
		/// 忽略只有数字的标题。
		/// </summary>
		[XmlAttribute("忽略数字标题")]
		public bool IgnoreNumericTitle { get; set; }

		/// <summary>
		/// 合并近邻的同级别标题。
		/// </summary>
		[XmlAttribute("合并相邻标题")]
		public bool MergeAdjacentTitles { get; set; }

		/// <summary>
		/// 是否允许合并不同字体尺寸的标题。
		/// </summary>
		[XmlAttribute("合并不同尺寸标题")]
		public bool MergeDifferentSizeTitles { get; set; }

		[XmlAttribute("合并不同字体标题")]
		public bool MergeDifferentFontTitles { get; set; }

		[XmlAttribute("忽略重叠文本")]
		public bool IgnoreOverlappedText { get; set; }

		private readonly Collection<MatchPattern> _IgnorePatterns = new Collection<MatchPattern>();
		/// <summary>
		/// 忽略指定的表达式。
		/// </summary>
		[XmlArray("忽略表达式")]
		[XmlArrayItem("表达式")]
		public Collection<MatchPattern> IgnorePatterns => _IgnorePatterns;

		private readonly Collection<LevelAdjustmentOption> _LevelAdjustment = new Collection<LevelAdjustmentOption>();
		[XmlElement("级别调整")]
		[PowerJson.JsonField("级别调整")]
		public Collection<LevelAdjustmentOption> LevelAdjustment => _LevelAdjustment;

		[XmlAttribute("自动组织标题层次")]
		public bool AutoHierarchicalArrangement { get; set; }

		[XmlAttribute("列出字体统计信息")]
		public bool DisplayFontStatistics { get; set; }

		[XmlAttribute("列出所有字体")]
		public bool DisplayAllFonts { get; set; }

		[XmlAttribute("排版")]
		public WritingDirection WritingDirection { get; set; }

		[XmlAttribute("最大合并行距")]
		public float MaxDistanceBetweenLines { get; set; }

		[XmlAttribute("识别分栏")]
		public bool DetectColumns { get; set; }

		[XmlAttribute("为首页生成书签")]
		public bool CreateBookmarkForFirstPage { get; set; }

		/// <summary>
		/// 首页书签名称。指定此属性，则为首页生成书签。
		/// </summary>
		internal string FirstPageTitle { get; set; }
		#endregion

		#region 定位选项
		/// <summary>
		/// 连接目标的页面 Y 轴偏移量。
		/// </summary>
		[XmlAttribute("Y轴偏移")]
		public float YOffset { get; set; }

		/// <summary>
		/// 定位到页面顶端的标题级别。
		/// </summary>
		[XmlAttribute("定位到页面顶端")]
		public int PageTopForLevel { get; set; }
		#endregion

		/// <summary>
		/// 是否导出文本的位置信息。
		/// </summary>
		[XmlAttribute("导出文本位置信息")]
		public bool ExportTextCoordinates { get; set; }

		public AutoBookmarkOptions() {
			AutoHierarchicalArrangement = true;
			CreateBookmarkForFirstPage = true;
			ExportTextCoordinates = false;
			IgnoreNumericTitle = false;
			IgnoreSingleCharacterTitle = false;
			MergeAdjacentTitles = true;
			MergeDifferentFontTitles = true;
			MergeDifferentSizeTitles = false;
			TitleThreshold = 13;
			YOffset = 1.0f;
			DisplayFontStatistics = true;
			MaxDistanceBetweenLines = 1.5f;
			DetectColumns = true;
		}

		public class LevelAdjustmentOption
		{
			[XmlElement(AutoBookmarkCondition.MultiCondition.ThisName, typeof(AutoBookmarkCondition.MultiCondition))]
			[XmlElement(AutoBookmarkCondition.FontNameCondition.ThisName, typeof(AutoBookmarkCondition.FontNameCondition))]
			[XmlElement(AutoBookmarkCondition.TextSizeCondition.ThisName, typeof(AutoBookmarkCondition.TextSizeCondition))]
			[XmlElement(AutoBookmarkCondition.TextPositionCondition.ThisName, typeof(AutoBookmarkCondition.TextPositionCondition))]
			[XmlElement(AutoBookmarkCondition.PageRangeCondition.ThisName, typeof(AutoBookmarkCondition.PageRangeCondition))]
			[XmlElement(AutoBookmarkCondition.TextCondition.ThisName, typeof(AutoBookmarkCondition.TextCondition))]
			public AutoBookmarkCondition Condition { get; set; }

			[XmlAttribute("合并前筛选")]
			public bool FilterBeforeMergeTitle { get; set; }

			[XmlAttribute("相对级别调整")]
			public bool RelativeAdjustment { get; set; }

			/// <summary>
			/// 标题的调整级别。
			/// </summary>
			[XmlAttribute("调整级别")]
			public float AdjustmentLevel { get; set; }

			internal LevelAdjustmentOption Clone() {
				return new LevelAdjustmentOption() {
					Condition = Condition.Clone() as AutoBookmarkCondition,
					RelativeAdjustment = RelativeAdjustment,
					AdjustmentLevel = AdjustmentLevel
				};
			}
		}

	}

}
