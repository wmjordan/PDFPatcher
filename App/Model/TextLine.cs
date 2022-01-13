using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model
{
	[System.Diagnostics.DebuggerDisplay("{Direction}({Region.Middle},{Region.Center}):Text = {Text}")]
	sealed class TextLine : IDirectionalBoundObject
	{
		readonly List<TextInfo> _Texts;

		/// <summary>获取此行内包含的文本。</summary>
		internal IEnumerable<TextInfo> Texts => _Texts;

		/// <summary>获取 <see cref="Texts"/> 内的第一个 <see cref="TextInfo"/>。</summary>
		internal TextInfo FirstText => _Texts[0];

		public WritingDirection Direction { get; private set; }
		public Bound Region { get; private set; }
		internal bool SuppressTextInfoArrangement { get; set; }

		/// <summary>
		/// 默认的书写方向。
		/// </summary>
		internal static WritingDirection DefaultDirection { get; set; }

		string _Text;

		/// <summary>
		/// 获取将 <see cref="Texts"/> 内所有文本串联起来的字符串。
		/// </summary>
		public string Text {
			get {
				if (_Text == null) {
					_Text = GetConcatinatedText();
				}

				return _Text;
			}
		}

		private TextLine() {
			_Texts = new List<TextInfo>();
			Direction = DefaultDirection;
		}

		internal TextLine(TextInfo text) : this() {
			_Texts.Add(text);
			Region = new Bound(text.Region);
			if (text.Text.Length > 2 && text.Region.Height > 0 && text.Region.Width > text.Region.Height * 2) {
				Direction = WritingDirection.Hortizontal;
			}
		}

		internal void AddText(TextInfo text) {
			if (Direction == WritingDirection.Unknown) {
				var d = GetDistance(text.Region);
				Direction = InferWritingDirection(d);
				if (Direction == WritingDirection.Unknown) {
					d = GetDistance(new Bound(text.Region.Center, text.Region.Middle));
					Direction = InferWritingDirection(d);
				}
			}

			_Text = null;
			_Texts.Add(text);
			Region.Merge(text.Region);
		}

		private static WritingDirection InferWritingDirection(DistanceInfo d) {
			return d.IsVerticallyAligned ? WritingDirection.Vertical
				: d.IsHorizontallyAligned ? WritingDirection.Hortizontal
				: WritingDirection.Unknown;
		}

		internal void Merge(TextLine source) {
			_Text = null;
			Region.Merge(source.Region);
			_Texts.AddRange(source.Texts);
		}

		/// <summary>
		/// 获取区域 <paramref name="other"/> 到当前文本行之间的距离。
		/// </summary>
		/// <param name="other">另一个区域。</param>
		/// <returns><paramref name="other"/> 相对于此区域的距离关系。</returns>
		internal DistanceInfo GetDistance(Bound other) {
			return Region.GetDistance(other, Direction);
		}

		/// <summary>
		/// 获取将 <see cref="Texts"/> 内所有文本串联起来的字符串。
		/// </summary>
		private string GetConcatinatedText() {
			var l = _Texts.Count;
			if (l == 0) {
				return String.Empty;
			}
			else if (l == 1) {
				return _Texts[0].Text;
			}

			var tl = _Texts;
			if (SuppressTextInfoArrangement == false) {
				if (Direction == WritingDirection.Vertical) {
					tl.Sort(TextInfo.CompareRegionY);
				}
				else {
					tl.Sort(TextInfo.CompareRegionX);
				}
			}

			var cs = GetAverageCharSize();
			var sb = new StringBuilder();
			sb.Append(tl[0].Text);
			char c;
			for (int i = 1; i < l; i++) {
				if (cs > 0) {
					var dx = Direction == WritingDirection.Vertical
						? tl[i].Region.Top - tl[i - 1].Region.Bottom
						: tl[i].Region.Left - tl[i - 1].Region.Right;
					if (dx > cs) {
						var t = tl[i - 1].Text;
						// 调整标点留下的空白
						if (t.Length > 0) {
							c = t[t.Length - 1];
							if (Char.IsPunctuation(c) && c > 128) {
								dx -= cs;
							}
						}

						t = tl[i].Text;
						if (t.Length > 0) {
							c = tl[i].Text[0];
							if (Char.IsPunctuation(c) && c > 128) {
								dx -= cs;
							}
						}
					}

					while ((dx -= cs) > 0) {
						sb.Append(' ');
					}
				}

				sb.Append(tl[i].Text);
			}

			return sb.ToString();
		}

		/// <summary>获取 <see cref="Texts"/> 内文字或数字的平均尺寸。</summary>
		/// <returns>返回平均字符尺寸。</returns>
		internal float GetAverageCharSize() {
			var tl = _Texts;
			float ts = 0, cc = 0;
			if (Direction == WritingDirection.Vertical) {
				tl.ForEach((t) => {
					ts += t.LetterWidth;
					cc += t.Text.Length;
				});
			}
			else {
				foreach (var t in tl) {
					ts += t.LetterWidth;
					foreach (var c in t.Text) {
						if (Char.IsLetterOrDigit(c) == false) {
							continue;
						}

						cc += (c > 0x36F ? 2 : 1);
					}
				}
			}

			return ts / cc; // 平均字符宽度
		}
	}
}