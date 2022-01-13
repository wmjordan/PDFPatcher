using System;
using System.Diagnostics;

namespace PDFPatcher.Model;

[DebuggerDisplay("{Location}={Distance}")]
internal sealed class DistanceInfo
{
	internal DistanceInfo(Placement location, float distanceX, float distanceY) {
		Location = location;
		DistanceX = distanceX;
		DistanceY = distanceY;
	}

	internal Placement Location { get; private set; }
	internal float DistanceX { get; }
	internal float DistanceY { get; }
	internal bool IsOverlapping => (Location & Placement.Overlapping) != Placement.Unknown;
	internal bool IsLeft => (Location & Placement.Left) != Placement.Unknown;
	internal bool IsRight => (Location & Placement.Right) != Placement.Unknown;
	internal bool IsAbove => (Location & Placement.Up) != Placement.Unknown;
	internal bool IsBelow => (Location & Placement.Down) != Placement.Unknown;
	internal bool IsVerticallyAligned => (Location & (Placement.Up | Placement.Down)) != Placement.Unknown;
	internal bool IsHorizontallyAligned => (Location & (Placement.Left | Placement.Right)) != Placement.Unknown;

	internal float MinDistance => (Location & Placement.Left) != Placement.Unknown ||
								  (Location & Placement.Right) != Placement.Unknown
		? DistanceX
		: (Location & Placement.Down) != Placement.Unknown || (Location & Placement.Up) != Placement.Unknown
			? DistanceY
			: DistanceRadial;

	internal float DistanceRadial => DistanceX == float.MaxValue || DistanceY == float.MaxValue
		? float.MaxValue
		: (float)Math.Sqrt((DistanceX * DistanceX) + (DistanceY * DistanceY));

	[Flags]
	internal enum Placement
	{
		Unknown = 0,
		Overlapping = 1,
		Left = 2,
		Right = 4,
		Up = 8,
		Down = 16
	}
}