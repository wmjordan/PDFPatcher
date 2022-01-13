namespace Devcorp.Controls.Design;

/// <summary>
///     Structure to define YUV.
/// </summary>
public struct YUV
{
	/// <summary>
	///     Gets an empty YUV structure.
	/// </summary>
	public static readonly YUV Empty = new();

	#region Fields

	private double y;
	private double u;
	private double v;

	#endregion

	#region Operators

	public static bool operator ==(YUV item1, YUV item2) {
		return item1.Y == item2.Y
		       && item1.U == item2.U
		       && item1.V == item2.V;
	}

	public static bool operator !=(YUV item1, YUV item2) {
		return item1.Y != item2.Y
		       || item1.U != item2.U
		       || item1.V != item2.V;
	}

	#endregion

	#region Accessors

	public double Y {
		get => y;
		set {
			y = value;
			y = y > 1 ? 1 : y < 0 ? 0 : y;
		}
	}

	public double U {
		get => u;
		set {
			u = value;
			u = u > 0.436 ? 0.436 : u < -0.436 ? -0.436 : u;
		}
	}

	public double V {
		get => v;
		set {
			v = value;
			v = v > 0.615 ? 0.615 : v < -0.615 ? -0.615 : v;
		}
	}

	#endregion

	/// <summary>
	///     Creates an instance of a YUV structure.
	/// </summary>
	public YUV(double y, double u, double v) {
		this.y = y;
		this.u = u;
		this.v = v;
	}

	#region Methods

	public override bool Equals(object obj) {
		if (obj == null || GetType() != obj.GetType()) {
			return false;
		}

		return this == (YUV)obj;
	}

	public override int GetHashCode() {
		return Y.GetHashCode() ^ U.GetHashCode() ^ V.GetHashCode();
	}

	#endregion
}