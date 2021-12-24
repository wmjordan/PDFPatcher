using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using PDFPatcher.Common;
using PDFPatcher.Processor;
using PowerJson;

namespace PDFPatcher.Model
{
	public abstract class AutoBookmarkCondition : ICloneable
	{
		[XmlAttribute (Constants.AutoBookmark.IsInclusive)]
		public bool IsInclusive { get; set; }

		public abstract string Description { get; }
		public abstract string Name { get; }

		public abstract object Clone ();
		internal abstract AutoBookmarkFilter CreateFilter ();
		internal abstract bool IsTextLineFilter { get; }

		[JsonTypeAlias(ThisName)]
		public class MultiCondition : AutoBookmarkCondition
		{
			internal const string ThisName = "条件组";
			readonly Collection<AutoBookmarkCondition> _Conditions = new Collection<AutoBookmarkCondition> ();

			[XmlElement(FontNameCondition.ThisName, typeof(FontNameCondition))]
			[XmlElement(TextSizeCondition.ThisName, typeof(TextSizeCondition))]
			[XmlElement(PageRangeCondition.ThisName, typeof(PageRangeCondition))]
			[XmlElement(TextCondition.ThisName, typeof(TextCondition))]
			[JsonField("条件")]
			public Collection<AutoBookmarkCondition> Conditions => _Conditions;

			public MultiCondition () { }

			public MultiCondition (AutoBookmarkCondition condition) {
				var m = condition as MultiCondition;
				if (m != null) {
					foreach (var item in m.Conditions) {
						Conditions.Add (item.Clone () as AutoBookmarkCondition);
					}
				}
				else {
					Conditions.Add (condition);
				}
			}

			public override string Description {
				get {
					var s = new string[Conditions.Count];
					for (int i = 0; i < s.Length; i++) {
						s[i] = _Conditions[i].Description;
					}
					return String.Join (";", s);
				}
			}

			public override string Name => "多条件组合";

			internal override bool IsTextLineFilter {
				get {
					foreach (var item in Conditions) {
						if (item.IsTextLineFilter) {
							return true;
						}
					}
					return false;
				}
			}

			public override object Clone () {
				var m = new MultiCondition ();
				foreach (var item in Conditions) {
					m.Conditions.Add (item.Clone () as AutoBookmarkCondition);
				}
				return m;
			}

			internal override AutoBookmarkFilter CreateFilter () {
				return new MultiConditionFilter (this);
			}
		}
		[JsonTypeAlias(ThisName)]
		public class FontNameCondition : AutoBookmarkCondition
		{
			internal const string ThisName = "字体名称";
			/// <summary>
			/// 需要调整级别的字体名称。
			/// </summary>
			[XmlAttribute (ThisName)]
			public string FontName { get; set; }

			/// <summary>
			/// 是否匹配字体全名。
			/// </summary>
			[XmlAttribute ("匹配字体全名")]
			public bool MatchFullName { get; set; }

			public override string Description => String.Concat(ThisName, (MatchFullName ? "为" : "包含"), "“", FontName, "”");

			public override string Name => ThisName;

			internal override bool IsTextLineFilter => false;

			public FontNameCondition () { }

			internal FontNameCondition (string fontName, bool matchFullName) {
				FontName = fontName;
				MatchFullName = matchFullName;
			}

			internal override AutoBookmarkFilter CreateFilter () {
				return new FontNameFilter (FontName, MatchFullName);
			}
			public override object Clone () {
				return new FontNameCondition (FontName, MatchFullName);
			}

		}
		[JsonTypeAlias(ThisName)]
		public class TextSizeCondition : AutoBookmarkCondition
		{
			internal const string ThisName = "字体尺寸";
			float _minSize, _maxSize;
			string _description;

			[XmlAttribute ("最小尺寸")]
			[DefaultValue (0)]
			public float MinSize {
				get => _minSize;
				set {
					_minSize = value;
					_description = null;
				}
			}
			[XmlAttribute ("最大尺寸")]
			[DefaultValue (0)]
			public float MaxSize {
				get => _maxSize;
				set {
					_maxSize = value;
					_description = null;
				}
			}

			public override string Description {
				get {
					if (_description == null) {
						UpdateRangeDescription ();
					}
					return _description;
				}
			}
			public override string Name => ThisName;

			internal override bool IsTextLineFilter => false;

			public TextSizeCondition () { }

			internal TextSizeCondition (float size) {
				SetRange (size, size);
			}

			internal TextSizeCondition (float minSize, float maxSize) {
				SetRange (minSize, maxSize);
			}

			private void UpdateRangeDescription () {
				_description = ThisName + (_minSize == _maxSize ? "等于" + _minSize.ToText() : "介于" + _minSize.ToText() + "和" + _maxSize.ToText());
			}

			public override object Clone () {
				var f = new TextSizeCondition () { _minSize = _minSize, _maxSize = _maxSize };
				f.UpdateRangeDescription ();
				return f;
			}

			internal void SetRange (float a, float b) {
				if (a > b) {
					_minSize = b;
					_maxSize = a;
				}
				else {
					_minSize = a;
					_maxSize = b;
				}
				_description = null;
			}


			internal override AutoBookmarkFilter CreateFilter () {
				return new TextSizeFilter (_minSize, _maxSize);
			}
		}
		[JsonTypeAlias(ThisName)]
		public class TextPositionCondition : AutoBookmarkCondition
		{
			internal const string ThisName = "文本坐标";
			byte _position;
			float _minValue, _maxValue;
			string _description;

			[XmlAttribute ("坐标值")]
			[DefaultValue (0)]
			public byte Position {
				get => _position;
				set {
					_position = value;
					_description = null;
				}
			}

			[XmlAttribute ("坐标最小值")]
			[DefaultValue (0)]
			public float MinValue {
				get => _minValue;
				set {
					_minValue = value;
					_description = null;
				}
			}
			[XmlAttribute ("坐标最大值")]
			[DefaultValue (0)]
			public float MaxValue {
				get => _maxValue;
				set {
					_maxValue = value;
					_description = null;
				}
			}

			public override string Description {
				get {
					if (_description == null) {
						UpdateRangeDescription ();
					}
					return _description;
				}
			}
			public override string Name => ThisName;

			internal override bool IsTextLineFilter => false;

			public TextPositionCondition () { }

			internal TextPositionCondition (byte position, float value) {
				SetRange (position, value, value);
			}

			internal TextPositionCondition (byte position, float value1, float value2) {
				SetRange (position, value1, value2);
			}

			private void UpdateRangeDescription () {
				_description = String.Concat (ThisName,
					_position == 1 ? "上" : _position == 2 ? "下" : _position == 3 ? "左" : _position == 4 ? "右" : String.Empty,
					"坐标",
					_minValue == _maxValue
						? "等于" + ValueHelper.ToText(_minValue)
						: "介于" + ValueHelper.ToText(_minValue) + "和" + _maxValue
				);
			}

			public override object Clone () {
				var f = new TextPositionCondition () {
					_position = _position,
					_minValue = _minValue,
					_maxValue = _maxValue
				};
				f.UpdateRangeDescription ();
				return f;
			}

			internal void SetRange (byte position, float value1, float value2) {
				_position = position;
				if (value1 > value2) {
					_minValue = value2;
					_maxValue = value1;
				}
				else {
					_minValue = value1;
					_maxValue = value2;
				}
				_description = null;
			}


			internal override AutoBookmarkFilter CreateFilter () {
				return new TextPositionFilter (_position, _minValue, _maxValue);
			}
		}
		[JsonTypeAlias(ThisName)]
		public class PageRangeCondition : AutoBookmarkCondition
		{
			internal const string ThisName = "页码范围";

			[XmlAttribute (ThisName)]
			public string PageRange { get; set; }

			public override string Description => "页码范围为“" + PageRange + "”";

			public override string Name => ThisName;

			internal override bool IsTextLineFilter => false;

			public override object Clone () {
				return new PageRangeCondition () { PageRange = PageRange };
			}

			internal override AutoBookmarkFilter CreateFilter () {
				return new PageRangeFilter (PageRange);
			}
		}
		[JsonTypeAlias(ThisName)]
		public class TextCondition : AutoBookmarkCondition
		{
			internal const string ThisName = "文本内容";

			[XmlElement ("文本模式")]
			public MatchPattern Pattern { get; set; }

			public override string Description => String.Concat(ThisName,
					Pattern.MatchCase ? "区分大小写" : String.Empty,
					Pattern.FullMatch ? "完全匹配" : "符合",
					Pattern.UseRegularExpression ? "正则表达式" : String.Empty,
					Pattern.Text);

			public override string Name => ThisName;

			internal override bool IsTextLineFilter => true;

			public override object Clone () {
				return new TextCondition (Pattern);
			}

			internal override AutoBookmarkFilter CreateFilter () {
				return new TextFilter (Pattern);
			}

			public TextCondition () {
				Pattern = new MatchPattern ();
			}
			private TextCondition (MatchPattern pattern) {
				Pattern = pattern.Clone () as MatchPattern;
			}
		}

	}


}
