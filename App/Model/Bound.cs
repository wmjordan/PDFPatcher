using System;
using System.Diagnostics;

namespace PDFPatcher.Model
{
	[DebuggerDisplay("T={Top},L={Left},B={Bottom},R={Right}; H={Height},W={Width}")]
	public class Bound
	{
		internal float Top { get; private set; }
		internal float Left { get; private set; }
		internal float Bottom { get; private set; }
		internal float Right { get; private set; }

		/// <summary>
		/// 获取此区域坐标是否属于笛卡尔坐标系。
		/// </summary>
		internal bool IsTopUp { get; private set; }
		/// <summary>
		/// 获取此坐标区域是否属于绘图坐标系。
		/// </summary>
		internal bool IsTopDown { get; private set; }

		internal float Height { get; private set; }
		internal float Width { get; private set; }

		internal float Middle { get; private set; }
		internal float Center { get; private set; }

		private Bound() {
			IsTopDown = true;
			IsTopUp = true;
		}

		public Bound(float left, float bottom, float right, float top) {
			if (right < left) {
				Debug.WriteLine("右端坐标不能小于左端坐标。");
				var t = right;
				right = left;
				left = t;
			}
			Left = left;
			Bottom = bottom;
			Right = right;
			Top = top;
			RecalculateSize();
		}
		/// <summary>
		/// 创建宽度和高度均为 0 的区域（点）实例。
		/// </summary>
		/// <param name="x">横坐标。</param>
		/// <param name="y">纵坐标。</param>
		public Bound(float x, float y) : this(x, y, x, y) {
		}

		/// <summary>
		/// 从指定区域复制副本。
		/// </summary>
		/// <param name="source">需要复制副本的区域。</param>
		public Bound(Bound source) : this(source.Left, source.Bottom, source.Right, source.Top) {
		}

		private void RecalculateSize() {
			IsTopUp = Top >= Bottom;
			IsTopDown = Top <= Bottom;
			Height = IsTopUp ? Top - Bottom : Bottom - Top;
			Middle = (Top + Bottom) / 2;
			Width = Right - Left;
			Center = (Left + Right) / 2;
		}

		internal Bound Merge(Bound source) {
			// 笛卡尔坐标
			if (IsTopUp) {
				if (Top < source.Top) {
					Top = source.Top;
				}
				if (Bottom > source.Bottom) {
					Bottom = source.Bottom;
				}
			}
			else {
				if (Top > source.Top) {
					Top = source.Top;
				}
				if (Bottom < source.Bottom) {
					Bottom = source.Bottom;
				}
			}
			if (Left > source.Left) {
				Left = source.Left;
			}
			if (Right < source.Right) {
				Right = source.Right;
			}
			RecalculateSize();
			return this;
		}

		/// <summary>
		/// 获取区域 <paramref name="other"/> 到当前区域之间的距离。
		/// </summary>
		/// <param name="other">另一个区域。</param>
		/// <param name="writingDirection">假设书写方向。</param>
		/// <returns><paramref name="other"/> 相对于此区域的距离关系。</returns>
		internal DistanceInfo GetDistance(Bound other, WritingDirection writingDirection) {
			if (IsTopDown != other.IsTopDown && IsTopUp != other.IsTopUp) {
				throw new ArgumentException("区域坐标系不同。");
			}

			float hd = float.MaxValue, vd = float.MaxValue;
			var hp = DistanceInfo.Placement.Unknown;
			var vp = DistanceInfo.Placement.Unknown;
			float au, ad, bu, bd;
			if (IsTopDown) {
				au = Top;
				ad = Bottom;
				bu = other.Top;
				bd = other.Bottom;
			}
			else {
				au = -Top;
				ad = -Bottom;
				bu = -other.Top;
				bd = -other.Bottom;
			}

			bool ov = false;
			if (IntersectWith(other)) {
				ov = true;
				hd = other.Center - Center;
				if (hd > 0) {
					hp = DistanceInfo.Placement.Right;
				}
				else if (hd < 0) {
					hp = DistanceInfo.Placement.Left;
					hd = -hd;
				}
				vd = other.Middle - Middle;
				if (vd > 0) {
					vp = IsTopUp ? DistanceInfo.Placement.Up : DistanceInfo.Placement.Down;
				}
				else if (vd < 0) {
					vp = IsTopUp ? DistanceInfo.Placement.Down : DistanceInfo.Placement.Up;
				}
				if (vd == 0 && hd == 0) {
					return new DistanceInfo(DistanceInfo.Placement.Overlapping, 0, 0);
				}
				else if (vd == 0) {
					return new DistanceInfo(DistanceInfo.Placement.Overlapping | hp, hd, vd);
				}
				else if (hp == 0) {
					return new DistanceInfo(DistanceInfo.Placement.Overlapping | vp, hd, vd);
				}
				else {
					return new DistanceInfo(DistanceInfo.Placement.Overlapping, hd, vd);
				}
			}

			if (other.Left >= Right) {
				hp = DistanceInfo.Placement.Right;
				hd = other.Left - Right;
			}
			else if (other.Right <= Left) {
				hp = DistanceInfo.Placement.Left;
				hd = Left - other.Right;
			}
			if (bd <= au) {
				vp = DistanceInfo.Placement.Up;
				vd = au - bd;
			}
			else if (bu >= ad) {
				vp = DistanceInfo.Placement.Down;
				vd = bu - ad;
			}
			if (hp == DistanceInfo.Placement.Unknown && vp == DistanceInfo.Placement.Unknown) {
				throw new ArgumentOutOfRangeException("位置错误。");
			}
			var v = new DistanceInfo(ov ? DistanceInfo.Placement.Overlapping | vp : vp, hd, vd);
			var h = new DistanceInfo(ov ? DistanceInfo.Placement.Overlapping | hp : hp, hd, vd);
			return writingDirection switch {
				WritingDirection.Vertical => hp != DistanceInfo.Placement.Unknown ? h : v,
				WritingDirection.Horizontal => vp != DistanceInfo.Placement.Unknown ? v : h,
				_ => (hd < vd) ? h : v
			};
		}

		/// <summary>
		/// 返回当前区域是否和指定区域在同一行上（中心点是否落在 <paramref name="other"/> 的两个边缘之间）。
		/// </summary>
		/// <param name="other">需要比较的区域。</param>
		/// <param name="direction">比较方向。</param>
		/// <returns>在同一行上时返回 true。</returns>
		internal bool IsAlignedWith(Bound other, WritingDirection direction) {
			switch (direction) {
				case WritingDirection.Horizontal:
					return IsTopDown ? (other.Top < Middle && Middle < other.Bottom || Top < other.Middle && other.Middle < Bottom) : (other.Bottom < Middle && Middle < other.Top || Bottom < other.Middle && other.Middle < Top);
				case WritingDirection.Vertical:
					return other.Left < Center && Center < other.Right
						|| Left < other.Center && other.Center < Right;
				default:
					return IntersectWith(other);
			}
		}

		internal bool IntersectWith(Bound other) {
			return other.Left < Right && Left < other.Right &&
				(IsTopDown
					? (other.Top < Bottom && Top < other.Bottom)
					: (other.Bottom < Top && Bottom < other.Top));
		}

		internal bool Contains(float x, float y) {
			float y1, y2;
			float x1 = Left;
			float x2 = Right;
			if (IsTopUp) {
				y1 = Bottom;
				y2 = Top;
			}
			else {
				y1 = Top;
				y2 = Bottom;
			}
			return x1 <= x && x <= x2 && y1 <= y && y <= y2;
		}

		public static bool operator ==(Bound a, Bound b) {
			return a.Top == b.Top && a.Bottom == b.Bottom && a.Left == b.Left && a.Right == b.Right;
		}
		public static bool operator !=(Bound a, Bound b) {
			return a.Top != b.Top || a.Bottom != b.Bottom || a.Left != b.Left || a.Right != b.Right;
		}
		public override bool Equals(object obj) {
			return this == (Bound)obj;
		}
		public override int GetHashCode() {
			return Top.GetHashCode() ^ Bottom.GetHashCode() ^ Left.GetHashCode() ^ Right.GetHashCode();
		}
		public static implicit operator System.Drawing.RectangleF(Bound bound) {
			return new System.Drawing.RectangleF(Math.Min(bound.Left, bound.Right), Math.Min(bound.Top, bound.Bottom), Math.Abs(bound.Left - bound.Right), Math.Abs(bound.Top - bound.Bottom));
		}
	}
}
