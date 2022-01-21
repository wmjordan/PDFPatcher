using System;
using System.Collections.Generic;
using System.Diagnostics;
using PDFPatcher.Common;

namespace MuPdfSharp
{
	public interface IMuBoundedElement
	{
		Rectangle BBox { get; }
	}

	[DebuggerDisplay("(Abort: {_Abort}, Progress: {_Progress}/{_ProgressMax}, Errors: {_Errors}, Incomplete: {_Incomplete})")]
	public struct MuCookie
	{
		int _Abort;
		readonly int _Progress;
		readonly int _ProgressMax;
		readonly int _Errors;
		readonly int _IncompleteOk;
		readonly int _Incomplete;

		public bool IsCancellationPending => _Abort != 0;
		public bool IsRunning => _ProgressMax == -1 || _Progress > 0 && _Progress < _ProgressMax - 1;
		public int ErrorCount => _Errors;

		public void CancelAsync() { _Abort = 1; }
	}

	/// <summary>
	/// MuPDF 引擎的工作方式。
	/// </summary>
	[Flags]
	public enum DeviceHints
	{
		None = 0,
		IgnoreImage = 1,
		IgnoreShade = 2,
		DontInterperateImages = 4,
		MaintainContainerStack = 8,
		NoCache = 16
	}

	/// <summary>
	/// 渲染页面的颜色空间。
	/// </summary>
	public enum ColorSpace
	{
		Rgb,
		Bgr,
		Cmyk,
		Gray
	}

	/// <summary>
	/// 保存渲染页面的文件格式。
	/// </summary>
	public enum ImageFormat
	{
		Png,
		Jpeg,
		Tiff
	}

	[DebuggerDisplay("From: {FromPageNumber}, Format: {Format (1)}")]
	public readonly struct PageLabel : IComparable<PageLabel>
	{
		public readonly string Prefix;
		public readonly PageLabelStyle NumericStyle;
		public readonly int StartAt;
		public readonly int FromPageNumber;
		public static PageLabel Empty = new PageLabel(-1, -1, null, PageLabelStyle.Default);
		public bool IsEmpty => FromPageNumber < 0;

		public PageLabel(int pageNumber, int startAt, string prefix, PageLabelStyle numericStyle) {
			FromPageNumber = pageNumber;
			StartAt = startAt;
			Prefix = prefix;
			NumericStyle = numericStyle;
		}

		int IComparable<PageLabel>.CompareTo(PageLabel other) {
			return FromPageNumber.CompareTo(other.FromPageNumber);
		}

		public string Format(int pageNumber) {
			var n = pageNumber - FromPageNumber + (StartAt < 1 ? 0 : StartAt - 1);
			switch (NumericStyle) {
				case PageLabelStyle.Default:
				case PageLabelStyle.Digit:
					return String.Concat(Prefix, n.ToText());
				case PageLabelStyle.UpperRoman:
					return String.Concat(Prefix, ValueHelper.ToRoman(n));
				case PageLabelStyle.LowerRoman:
					return String.Concat(Prefix, ValueHelper.ToRoman(n)).ToLowerInvariant();
				case PageLabelStyle.UpperAlphabetic:
					return String.Concat(Prefix, ValueHelper.ToAlphabet(n, true));
				case PageLabelStyle.LowerAlphabetic:
					return String.Concat(Prefix, ValueHelper.ToAlphabet(n, false));
				default:
					goto case PageLabelStyle.Digit;
			}
		}
	}

	public enum PageLabelStyle : byte
	{
		Default = 0,
		Digit = (byte)'d',
		UpperRoman = (byte)'R',
		LowerRoman = (byte)'r',
		UpperAlphabetic = (byte)'A',
		LowerAlphabetic = (byte)'a'
	}

	/// <summary>
	/// 表示点。
	/// </summary>
	[DebuggerDisplay("({X},{Y})")]
	public readonly struct Point : IEquatable<Point>
	{
		public readonly float X, Y;
		public override string ToString() {
			return String.Concat("(", X, ",", Y, ")");
		}
		public Point(float x, float y) {
			X = x; Y = y;
		}
		/// <summary>
		/// 将 PDF 页面坐标点转换为渲染页面坐标点。
		/// </summary>
		/// <param name="pageVisualBound">页面的可视区域。</param>
		/// <returns>转换为页面坐标的点。</returns>
		public Point ToPageCoordinate(Rectangle pageVisualBound) {
			return new Point((X - pageVisualBound.Left), pageVisualBound.Height - (Y - pageVisualBound.Top));
		}
		public static explicit operator System.Drawing.Point(Point point) {
			return new System.Drawing.Point(point.X.ToInt32(), point.Y.ToInt32());
		}
		public static implicit operator System.Drawing.PointF(Point point) {
			return new System.Drawing.PointF(point.X, point.Y);
		}
		public static implicit operator Point(System.Drawing.Point point) {
			return new Point(point.X, point.Y);
		}
		public static implicit operator Point(System.Drawing.PointF point) {
			return new Point(point.X, point.Y);
		}

		public override bool Equals(object obj) {
			return obj is Point && this == (Point)obj;
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		public bool Equals(Point other) {
			return this == other;
		}

		public static bool operator ==(Point left, Point right) {
			return left.X == right.X && left.Y == right.Y;
		}

		public static bool operator !=(Point left, Point right) {
			return !(left == right);
		}
	}

	/// <summary>
	/// 表示边框（坐标值为整数的矩形）。
	/// 在 MuPDF 中，BBox 的 <see cref="Bottom"/> 值应大于 <see cref="Top"/> 值。
	/// </summary>
	[DebuggerDisplay("({Left},{Top})-({Right},{Bottom})")]
	public readonly struct BBox : IEquatable<BBox>
	{
		public readonly int Left, Top, Right, Bottom;
		public BBox(int left, int top, int right, int bottom) {
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}
		public System.Drawing.Size Size => new System.Drawing.Size(Width, Height);
		public bool IsEmpty => Left == Right || Top == Bottom;
		public bool IsInfinite => Left > Right || Top > Bottom;
		public int Width => Right - Left;
		public int Height => Bottom - Top;

		public bool Contains(Point point) {
			return (Right >= point.X && Left <= point.X && Top <= point.Y && Bottom >= point.Y);
		}

		public static implicit operator System.Drawing.Rectangle(BBox rect) {
			return new System.Drawing.Rectangle(
				rect.Left,
				rect.Top < rect.Bottom ? rect.Top : rect.Bottom,
				rect.Width,
				rect.Height);
		}

		public override bool Equals(object obj) {
			return obj is BBox && this == (BBox)obj;
		}

		public bool Equals(BBox other) {
			return Left == other.Left &&
				   Top == other.Top &&
				   Right == other.Right &&
				   Bottom == other.Bottom;
		}

		public override int GetHashCode() {
			var hashCode = -1819631549;
			hashCode = hashCode * -1521134295 + Left;
			hashCode = hashCode * -1521134295 + Top;
			hashCode = hashCode * -1521134295 + Right;
			hashCode = hashCode * -1521134295 + Bottom;
			return hashCode;
		}

		public static bool operator ==(BBox left, BBox right) {
			return left.Equals(right);
		}

		public static bool operator !=(BBox left, BBox right) {
			return !(left == right);
		}
	}

	/// <summary>
	/// 表示使用浮点数为坐标的矩形。
	/// </summary>
	[DebuggerDisplay("({Left},{Top})-({Right},{Bottom})")]
	public readonly struct Rectangle : IEquatable<Rectangle>
	{
		public readonly float Left, Top, Right, Bottom;
		public static readonly Rectangle Infinite = new Rectangle(1, 1, -1, -1);
		public static readonly Rectangle Empty = new Rectangle(0, 0, 0, 0);
		public static readonly Rectangle Unit = new Rectangle(0, 0, 1, 1);

		public Rectangle(float left, float top, float right, float bottom) {
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		private static int SafeInt(double f) {
			return (f > int.MaxValue) ? int.MaxValue : ((f < int.MinValue) ? int.MinValue : (int)f);
		}
		public System.Drawing.SizeF Size => new System.Drawing.SizeF(Width, Height);
		public bool IsEmpty => Left == Right || Top == Bottom;
		public bool IsInfinite => Left > Right || Top > Bottom;
		public float Width => Right - Left;
		public float Height => Bottom - Top;
		public BBox Round => new BBox(
					SafeInt(Math.Floor(Left + 0.001)),
					SafeInt(Math.Floor(Top + 0.001)),
					SafeInt(Math.Ceiling(Right - 0.001)),
					SafeInt(Math.Ceiling(Bottom - 0.001))
				);
		public override string ToString() {
			return string.Concat("(", Left, ",", Top, ")-(", Right, ",", Bottom, ")");
		}

		/// <summary>
		/// 返回当前矩形区域是否包含另一个点。
		/// </summary>
		/// <param name="point">另一个点。</param>
		/// <returns>包含点时返回 true。</returns>
		public bool Contains(Point point) {
			return Right >= point.X && Left <= point.X && Top <= point.Y && Bottom >= point.Y;
		}
		public bool Contains(float pointX, float pointY) {
			return Right >= pointX && Left <= pointX && Top <= pointY && Bottom >= pointY;
		}
		/// <summary>返回当前矩形区域是否与另一个矩形区域存在交集。</summary>
		/// <param name="other">另一个矩形区域。</param>
		/// <returns>包含矩形区域时返回 true。</returns>
		public bool Contains(Rectangle other) {
			if (IsEmpty || other.IsInfinite) {
				return false;
			}
			else if (IsInfinite || other.IsEmpty) {
				return true;
			}
			return Contains(other.Left, other.Top) && Contains(other.Right, other.Bottom);
		}
		/// <summary>返回当前矩形区域与另一个矩形区域的交集。</summary>
		/// <param name="other">另一个矩形区域。</param>
		/// <returns>返回两个矩形区域的交集。</returns>
		public Rectangle Intersect(Rectangle other) {
			if (IsEmpty || other.IsEmpty) {
				return Rectangle.Empty;
			}
			if (other.IsInfinite) {
				return this;
			}
			if (IsInfinite) {
				return other;
			}
			float x0, y0, x1, y1;
			x0 = Left < other.Left ? other.Left : Left;
			y0 = Top < other.Top ? other.Top : Top;
			x1 = Right > other.Right ? other.Right : Right;
			y1 = Bottom > other.Bottom ? other.Bottom : Bottom;
			if (x1 < x0 || y1 < y0) {
				return Rectangle.Empty;
			}
			return new Rectangle(x0, y0, x1, y1);
		}
		/// <summary>返回将当前矩形区域乘以指定比例的区域。</summary>
		/// <param name="multiplier">比例乘数。</param>
		/// <returns>拉伸后的矩形区域。</returns>
		public Rectangle Multiply(float multiplier) {
			return new Rectangle(Left * multiplier, Top * multiplier, Right * multiplier, Bottom * multiplier);
		}

		/// <summary>返回包含两个矩形区域的新矩形区域。</summary>
		/// <param name="other">另一个矩形区域。</param>
		/// <returns>包含两个矩形区域的新矩形区域。</returns>
		public Rectangle Union(Rectangle other) {
			if (IsEmpty || other.IsInfinite) {
				return other;
			}
			if (other.IsEmpty || IsInfinite) {
				return this;
			}
			return new Rectangle(
				Left > other.Left ? other.Left : Left,
				Top > other.Top ? other.Top : Top,
				Right < other.Right ? other.Right : Right,
				Bottom < other.Bottom ? other.Bottom : Bottom
				);
		}

		internal static Rectangle FromArray(MuPdfObject array) {
			var r = array.AsArray();
			var a = r[0].FloatValue;
			var b = r[1].FloatValue;
			var c = r[2].FloatValue;
			var d = r[3].FloatValue;
			return new Rectangle(Math.Min(a, c), Math.Min(b, d), Math.Max(a, c), Math.Max(b, d));
		}

		public static implicit operator System.Drawing.RectangleF(Rectangle rect) {
			return new System.Drawing.RectangleF(
				rect.Left,
				rect.Top < rect.Bottom ? rect.Top : rect.Bottom,
				rect.Width,
				rect.Height);
		}
		public static implicit operator System.Drawing.Rectangle(Rectangle rect) {
			return new System.Drawing.Rectangle(
				rect.Left.ToInt32(),
				(rect.Top < rect.Bottom ? rect.Top : rect.Bottom).ToInt32(),
				rect.Width.ToInt32(),
				rect.Height.ToInt32());
		}
		public static Rectangle operator &(Rectangle r1, Rectangle r2) {
			return r1.Intersect(r2);
		}
		public static Rectangle operator |(Rectangle r1, Rectangle r2) {
			return r1.Union(r2);
		}
		/// <summary>返回两个矩形重叠区域与两矩形最小包容区域的占比。</summary>
		public static float operator /(Rectangle r1, Rectangle r2) {
			var i = r1.Intersect(r2);
			if (i.IsEmpty) {
				return 0f;
			}
			var u = r1.Union(r1);
			return (i.Height * i.Width) / (u.Height * u.Width);
		}

		internal Rectangle ToPageCoordinate(Rectangle pageVisualBound) {
			return new Rectangle(
				Left - pageVisualBound.Left,
				pageVisualBound.Height - (Top - pageVisualBound.Top),
				Right - pageVisualBound.Left,
				pageVisualBound.Height - (Bottom - pageVisualBound.Top)
			);
		}

		public override bool Equals(object obj) {
			return obj is Rectangle && Equals((Rectangle)obj);
		}

		public override int GetHashCode() {
			return Left.GetHashCode()
				^ (Right.GetHashCode() << 13 | Right.GetHashCode() >> 19)
				^ (Top.GetHashCode() << 26 | Top.GetHashCode() >> 6)
				^ (Bottom.GetHashCode() << 7 | Bottom.GetHashCode() >> 25);
		}

		public bool Equals(Rectangle other) {
			return Left == other.Left && Right == other.Right && Top == other.Top && Bottom == other.Bottom;
		}

		public static bool operator ==(Rectangle left, Rectangle right) {
			return left.Equals(right);
		}

		public static bool operator !=(Rectangle left, Rectangle right) {
			return !(left == right);
		}
	}

	/// <summary>
	/// 表示四个坐标构成的矩形。
	/// </summary>
	public readonly struct Quad : IEquatable<Quad>
	{
		public readonly Point UpperLeft, UpperRight, LowerLeft, LowerRight;

		public Quad(Point upperLeft, Point upperRight, Point lowerLeft, Point lowerRight) {
			UpperLeft = upperLeft;
			UpperRight = upperRight;
			LowerLeft = lowerLeft;
			LowerRight = lowerRight;
		}

		public Quad Union(Quad other) {
			var x1 = Math.Min(Math.Min(UpperLeft.X, other.UpperLeft.X), Math.Min(LowerLeft.X, other.LowerLeft.X));
			var x2 = Math.Max(Math.Max(UpperLeft.X, other.UpperLeft.X), Math.Max(LowerLeft.X, other.LowerLeft.X));
			var y1 = Math.Min(Math.Min(UpperLeft.Y, other.UpperLeft.Y), Math.Min(LowerLeft.Y, other.LowerLeft.Y));
			var y2 = Math.Max(Math.Max(UpperLeft.Y, other.UpperLeft.Y), Math.Max(LowerLeft.Y, other.LowerLeft.Y));
			return new Quad(new Point(x1, y1), new Point(x2, y2), new Point(x1, y2), new Point(x2, y2));
		}
		public Rectangle ToRectangle() {
			var x1 = Math.Min(Math.Min(UpperLeft.X, UpperRight.X), Math.Min(LowerLeft.X, LowerRight.X));
			var x2 = Math.Max(Math.Max(UpperLeft.X, UpperRight.X), Math.Max(LowerLeft.X, LowerRight.X));
			var y1 = Math.Min(Math.Min(UpperLeft.Y, UpperRight.Y), Math.Min(LowerLeft.Y, LowerRight.Y));
			var y2 = Math.Max(Math.Max(UpperLeft.Y, UpperRight.Y), Math.Max(LowerLeft.Y, LowerRight.Y));
			return new Rectangle(x1, y1, x2, y2);
		}

		public override bool Equals(object obj) {
			return obj is Quad && Equals((Quad)obj);
		}

		public bool Equals(Quad other) {
			return UpperLeft.Equals(other.UpperLeft) &&
				   UpperRight.Equals(other.UpperRight) &&
				   LowerLeft.Equals(other.LowerLeft) &&
				   LowerRight.Equals(other.LowerRight);
		}

		public override int GetHashCode() {
			var hashCode = -1690381272;
			hashCode = hashCode * -1521134295 + EqualityComparer<Point>.Default.GetHashCode(UpperLeft);
			hashCode = hashCode * -1521134295 + EqualityComparer<Point>.Default.GetHashCode(UpperRight);
			hashCode = hashCode * -1521134295 + EqualityComparer<Point>.Default.GetHashCode(LowerLeft);
			hashCode = hashCode * -1521134295 + EqualityComparer<Point>.Default.GetHashCode(LowerRight);
			return hashCode;
		}

		public static bool operator ==(Quad quad1, Quad quad2) {
			return quad1.Equals(quad2);
		}

		public static bool operator !=(Quad quad1, Quad quad2) {
			return !(quad1 == quad2);
		}
	}

	/// <summary>
	/// 表示转置矩阵。
	/// </summary>
	[DebuggerDisplay("({A},{B},{C},{D},{E},{F})")]
	public readonly struct Matrix : IEquatable<Matrix>
	{
		public readonly float A, B, C, D, E, F;
		/// <summary>
		/// 返回矩阵的放大方向是否对齐坐标轴（没有斜向拉伸或90整数倍以外的旋转）。
		/// </summary>
		public bool IsRectilinear => Math.Abs(B) < Single.Epsilon && Math.Abs(C) < Single.Epsilon
					|| Math.Abs(A) < Single.Epsilon && Math.Abs(D) < Single.Epsilon;
		/// <summary>
		/// 返回矩阵大致的放大比例。
		/// </summary>
		public float Expansion => (float)Math.Sqrt(Math.Abs(A * D - B * C));
		private static float Min4(float a, float b, float c, float d) {
			return Math.Min(Math.Min(a, b), Math.Min(c, d));
		}
		private static float Max4(float a, float b, float c, float d) {
			return Math.Max(Math.Max(a, b), Math.Max(c, d));
		}
		/// <summary>
		/// 单元矩阵。
		/// </summary>
		public static readonly Matrix Identity = new Matrix(1, 0, 0, 1, 0, 0);
		/// <summary>
		/// 垂直翻转矩阵。
		/// </summary>
		public static readonly Matrix VerticalFlip = new Matrix(1, 0, 0, -1, 0, 0);
		/// <summary>
		/// 水平翻转矩阵。
		/// </summary>
		public static readonly Matrix HorizontalFlip = new Matrix(-1, 0, 0, 1, 0, 0);
		public Matrix(float a, float b, float c, float d, float e, float f) {
			A = a;
			B = b;
			C = c;
			D = d;
			E = e;
			F = f;
		}
		/// <summary>
		/// 将两个矩阵相乘。
		/// </summary>
		/// <param name="one">被乘的矩阵。</param>
		/// <param name="two">乘数矩阵。</param>
		/// <returns>相乘后的新矩阵。</returns>
		public static Matrix Concat(Matrix one, Matrix two) {
			return new Matrix(
				one.A * two.A + one.B * two.C,
				one.A * two.B + one.B * two.D,
				one.C * two.A + one.D * two.C,
				one.C * two.B + one.D * two.D,
				one.E * two.A + one.F * two.C + two.E,
				one.E * two.B + one.F * two.D + two.F);
		}
		public static Matrix Scale(float x, float y) {
			return new Matrix(x, 0, 0, y, 0, 0);
		}
		public Matrix ScaleTo(float x, float y) {
			return Concat(this, Scale(x, y));
		}
		public static Matrix Shear(float h, float v) {
			return new Matrix(1, v, h, 1, 0, 0);
		}
		public Matrix ShearTo(float x, float y) {
			return Concat(this, Shear(x, y));
		}
		public static Matrix Rotate(float theta) {
			float s;
			float c;

			while (theta < 0)
				theta += 360;
			while (theta >= 360)
				theta -= 360;

			if (Math.Abs(0 - theta) < float.Epsilon) {
				s = 0;
				c = 1;
			}
			else if (Math.Abs(90.0f - theta) < float.Epsilon) {
				s = 1;
				c = 0;
			}
			else if (Math.Abs(180.0f - theta) < float.Epsilon) {
				s = 0;
				c = -1;
			}
			else if (Math.Abs(270.0f - theta) < float.Epsilon) {
				s = -1;
				c = 0;
			}
			else {
				s = (float)Math.Sin(theta * Math.PI / 180f);
				c = (float)Math.Cos(theta * Math.PI / 180f);
			}

			return new Matrix(c, s, -s, c, 0, 0);
		}
		public Matrix RotateTo(float theta) {
			return Concat(this, Rotate(theta));
		}
		public static Matrix Translate(float tx, float ty) {
			return new Matrix(1, 0, 0, 1, tx, ty);
		}
		public Matrix TranslateTo(float tx, float ty) {
			return Concat(this, Translate(tx, ty));
		}
		public Point Transform(Point p) {
			return new Point(p.X * A + p.Y * C + E, p.X * B + p.Y * D + F);
		}
		public Point Transform(float x, float y) {
			return new Point(x * A + y * C + E, x * B + y * D + F);
		}
		public Rectangle Transform(Rectangle rect) {
			Point s, t, u, v;

			if (rect.IsInfinite)
				return rect;

			s = Transform(rect.Left, rect.Top);
			t = Transform(rect.Left, rect.Bottom);
			u = Transform(rect.Right, rect.Bottom);
			v = Transform(rect.Right, rect.Top);
			return new Rectangle(Min4(s.X, t.X, u.X, v.X),
				Min4(s.Y, t.Y, u.Y, v.Y),
				Max4(s.X, t.X, u.X, v.X),
				Max4(s.Y, t.Y, u.Y, v.Y)
				);
		}

		public override bool Equals(object obj) {
			return obj is Matrix && Equals((Matrix)obj);
		}

		public bool Equals(Matrix other) {
			return A == other.A && B == other.B && C == other.C && D == other.D && E == other.E && F == other.F;
		}

		public override int GetHashCode() {
			var hashCode = 165473199;
			hashCode = hashCode * -1521134295 + A.GetHashCode();
			hashCode = hashCode * -1521134295 + B.GetHashCode();
			hashCode = hashCode * -1521134295 + C.GetHashCode();
			hashCode = hashCode * -1521134295 + D.GetHashCode();
			hashCode = hashCode * -1521134295 + E.GetHashCode();
			hashCode = hashCode * -1521134295 + F.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(Matrix matrix1, Matrix matrix2) {
			return matrix1.Equals(matrix2);
		}

		public static bool operator !=(Matrix matrix1, Matrix matrix2) {
			return !(matrix1 == matrix2);
		}
	}

}
