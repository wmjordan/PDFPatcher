using System;

namespace PDFPatcher.Model;

[System.Diagnostics.DebuggerDisplay("{Location}={Distance}")]
internal sealed class DistanceInfo
{
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

	internal Placement Location { get; private set; }
	internal float DistanceX { get; private set; }
	internal float DistanceY { get; private set; }
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

	internal DistanceInfo(Placement location, float distanceX, float distanceY) {
		Location = location;
		DistanceX = distanceX;
		DistanceY = distanceY;
	}
}