using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model
{
	[System.Diagnostics.DebuggerDisplay ("{Location}={Distance}")]
	sealed class DistanceInfo
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
		internal bool IsOverlapping { get { return (Location & Placement.Overlapping) != Placement.Unknown; } }
		internal bool IsLeft { get { return (Location & Placement.Left) != Placement.Unknown; } }
		internal bool IsRight { get { return (Location & Placement.Right) != Placement.Unknown; } }
		internal bool IsAbove { get { return (Location & Placement.Up) != Placement.Unknown; } }
		internal bool IsBelow { get { return (Location & Placement.Down) != Placement.Unknown; } }
		internal bool IsVerticallyAligned { get { return (Location & (Placement.Up | Placement.Down)) != Placement.Unknown; } }
		internal bool IsHorizontallyAligned { get { return (Location & (Placement.Left | Placement.Right)) != Placement.Unknown; } }

		internal float MinDistance {
			get {
				return (Location & Placement.Left) != Placement.Unknown || (Location & Placement.Right) != Placement.Unknown
					? DistanceX
					: (Location & Placement.Down) != Placement.Unknown || (Location & Placement.Up) != Placement.Unknown
					? DistanceY : DistanceRadial;
			}
		}
		internal float DistanceRadial {
			get {
				return DistanceX == Single.MaxValue || DistanceY == Single.MaxValue
					? Single.MaxValue
					: (float)Math.Sqrt (DistanceX * DistanceX + DistanceY * DistanceY);
			}
		}

		internal DistanceInfo (Placement location, float distanceX, float distanceY) {
			this.Location = location;
			this.DistanceX = distanceX;
			this.DistanceY = distanceY;
		}

	}
}
