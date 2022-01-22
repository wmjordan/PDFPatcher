﻿using System.Diagnostics;
using System.Linq;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

[DebuggerDisplay("P {Position}: {MinValue}->{MaxValue}")]
internal sealed class TextPositionFilter : AutoBookmarkFilter
{
	public TextPositionFilter(byte position, float min, float max) {
		Position = position;
		MaxValue = max;
		MinValue = min;
	}

	public byte Position { get; private set; }
	public float MaxValue { get; private set; }
	public float MinValue { get; private set; }

	internal override bool Matches(AutoBookmarkContext context) {
		return context.TextLine == null ? MatchPosition(context.TextInfo.Region) : context.TextLine.Texts.Any(item => MatchPosition(item.Region));
	}

	private bool MatchPosition(Bound bound) {
		switch (Position) {
			case 1: return bound.Top > MinValue && bound.Top < MaxValue;
			case 2: return bound.Bottom > MinValue && bound.Bottom < MaxValue;
			case 3: return bound.Left > MinValue && bound.Left < MaxValue;
			case 4: return bound.Right > MinValue && bound.Right < MaxValue;
			default:
				return false;
		}
	}

	internal override void Reset() {
	}
}