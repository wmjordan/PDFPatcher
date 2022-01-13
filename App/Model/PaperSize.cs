using System;
using System.Xml.Serialization;

namespace PDFPatcher.Model
{
	public class PaperSize
	{
		public const string AsPageSize = "等同原始内容尺寸";
		public const string FixedWidthAutoHeight = "固定页宽自动高度";
		public const string AsSpecificPage = "等同指定页面尺寸";
		public const string AsWidestPage = "宽度同最宽页面，自动高度";
		public const string AsNarrowestPage = "宽度同最窄页面，自动高度";
		public const string AsLargestPage = "等同最大页面尺寸";
		public const string AsSmallestPage = "等同最小页面尺寸";

		string _PaperName;

		[XmlAttribute("名称")]
		public string PaperName {
			get => _PaperName;
			set {
				_PaperName = value;
				switch (_PaperName) {
					case AsPageSize:
						SpecialSize = SpecialPaperSize.AsPageSize;
						break;
					case FixedWidthAutoHeight:
						SpecialSize = SpecialPaperSize.FixedWidthAutoHeight;
						break;
					case AsSpecificPage:
						SpecialSize = SpecialPaperSize.AsSpecificPage;
						break;
					case AsWidestPage:
						SpecialSize = SpecialPaperSize.AsWidestPage;
						break;
					case AsNarrowestPage:
						SpecialSize = SpecialPaperSize.AsNarrowestPage;
						break;
					case AsLargestPage:
						SpecialSize = SpecialPaperSize.AsLargestPage;
						break;
					case AsSmallestPage:
						SpecialSize = SpecialPaperSize.AsSmallestPage;
						break;
					default:
						SpecialSize = SpecialPaperSize.None;
						break;
				}
			}
		}

		[XmlIgnore] public SpecialPaperSize SpecialSize { get; private set; }

		private float _Height;

		///<summary>获取或指定页面高度的值。</summary>
		[XmlAttribute("高度")]
		public float Height {
			get => _Height;
			set {
				if (value < 0) {
					throw new ArgumentException("页面高度不可小于 0。");
				}

				_Height = value;
			}
		}

		private float _Width;

		///<summary>获取或指定页面宽度的值。</summary>
		[XmlAttribute("宽度")]
		public float Width {
			get => _Width;
			set {
				if (value < 0) {
					throw new ArgumentException("页面宽度不可小于 0。");
				}

				_Width = value;
			}
		}

		public PaperSize() { }

		public PaperSize(float width, float height) : this(null, width, height) {
		}

		public PaperSize(string paperName, float width, float height) {
			PaperName = paperName;
			Width = width;
			Height = height;
		}

		internal PaperSize Scale(float xFactor, float yFactor) {
			return new PaperSize(PaperName, Width * xFactor, Height * yFactor);
		}

		internal PaperSize Scale(float factor) {
			return new PaperSize(PaperName, Width * factor, Height * factor);
		}

		internal PaperSize Clone() {
			return (PaperSize)MemberwiseClone();
		}

		public override string ToString() {
			return PaperName;
		}
	}

	public enum SpecialPaperSize
	{
		None,
		AsPageSize,
		FixedWidthAutoHeight,
		AsSpecificPage,
		AsWidestPage,
		AsNarrowestPage,
		AsLargestPage,
		AsSmallestPage
	}
}