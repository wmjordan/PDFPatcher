using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model
{
	[System.Diagnostics.DebuggerDisplay("{Direction}({Region.Top},{Region.Left})Lines={Lines.Count}")]
	sealed class TextRegion
	{
		internal WritingDirection Direction { get; set; }
		internal Bound Region { get; private set; }

		/// <summary>
		/// 获取文本区域中的行。
		/// 不应该调用此属性的 Add 方法添加行，而应使用 <see cref="TextRegion.AddLine"/> 方法。
		/// </summary>
		internal List<TextLine> Lines { get; private set; }

		internal TextRegion() {
			Lines = new List<TextLine>();
		}

		internal TextRegion(TextLine text) : this() {
			Region = new Bound(text.Region);
			AddLine(text);
		}

		internal void AddLine(TextLine line) {
			if (Direction == WritingDirection.Unknown) {
				var d = Region.GetDistance(line.Region, WritingDirection.Unknown);
				Direction = (d.Location == DistanceInfo.Placement.Up || d.Location == DistanceInfo.Placement.Down)
					? WritingDirection.Vertical
					: (d.Location == DistanceInfo.Placement.Left || d.Location == DistanceInfo.Placement.Right)
						? WritingDirection.Hortizontal
						: WritingDirection.Unknown;
			}

			Lines.Add(line);
			Region.Merge(line.Region);
		}
	}
}