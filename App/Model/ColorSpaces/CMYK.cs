namespace Devcorp.Controls.Design;

/// <summary>
///     Structure to define CMYK.
/// </summary>
public struct CMYK
{
	/// <summary>
	///     Gets an empty CMYK structure;
	/// </summary>
	public static readonly CMYK Empty = new();

	#region Fields

	private double c;
	private double m;
	private double y;
	private double k;

	#endregion

	#region Operators

	public static bool operator ==(CMYK item1, CMYK item2) {
		return item1.Cyan == item2.Cyan
			   && item1.Magenta == item2.Magenta
			   && item1.Yellow == item2.Yellow
			   && item1.Black == item2.Black;
	}

	public static bool operator !=(CMYK item1, CMYK item2) {
		return item1.Cyan != item2.Cyan
			   || item1.Magenta != item2.Magenta
			   || item1.Yellow != item2.Yellow
			   || item1.Black != item2.Black;
	}

	#endregion

	#region Accessors

	public double Cyan {
		get => c;
		set {
			c = value;
			c = c > 1 ? 1 : c < 0 ? 0 : c;
		}
	}

	public double Magenta {
		get => m;
		set {
			m = value;
			m = m > 1 ? 1 : m < 0 ? 0 : m;
		}
	}

	public double Yellow {
		get => y;
		set {
			y = value;
			y = y > 1 ? 1 : y < 0 ? 0 : y;
		}
	}

	public double Black {
		get => k;
		set {
			k = value;
			k = k > 1 ? 1 : k < 0 ? 0 : k;
		}
	}

	#endregion

	/// <summary>
	///     Creates an instance of a CMYK structure.
	/// </summary>
	public CMYK(double c, double m, double y, double k) {
		this.c = c;
		this.m = m;
		this.y = y;
		this.k = k;
	}

	#region Methods

	public override bool Equals(object obj) {
		if (obj == null || GetType() != obj.GetType()) {
			return false;
		}

		return this == (CMYK)obj;
	}

	public override int GetHashCode() {
		return Cyan.GetHashCode() ^ Magenta.GetHashCode() ^ Yellow.GetHashCode() ^ Black.GetHashCode();
	}

	#endregion
}