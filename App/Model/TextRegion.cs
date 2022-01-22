using System.Collections.Generic;
using System.Diagnostics;

namespace PDFPatcher.Model;

[DebuggerDisplay("{Direction}({Region.Top},{Region.Left})Lines={Lines.Count}")]
internal sealed class TextRegion
{
	internal TextRegion() {
		Lines = new List<TextLine>();
	}

	internal TextRegion(TextLine text) : this() {
		Region = new Bound(text.Region);
		AddLine(text);
	}

	internal WritingDirection Direction { get; set; }
	internal Bound Region { get; private set; }

	/// <summary>
	///     获取文本区域中的行。
	///     不应该调用此属性的 Add 方法添加行，而应使用 <see cref="TextRegion.AddLine" /> 方法。
	/// </summary>
	internal List<TextLine> Lines { get; private set; }

	internal void AddLine(TextLine line) {
		if (Direction == WritingDirection.Unknown) {
			DistanceInfo d = Region.GetDistance(line.Region, WritingDirection.Unknown);
			Direction = d.Location is DistanceInfo.Placement.Up or DistanceInfo.Placement.Down
				? WritingDirection.Vertical
				: d.Location is DistanceInfo.Placement.Left or DistanceInfo.Placement.Right
					? WritingDirection.Horizontal
					: WritingDirection.Unknown;
		}

		Lines.Add(line);
		Region.Merge(line.Region);
	}
}