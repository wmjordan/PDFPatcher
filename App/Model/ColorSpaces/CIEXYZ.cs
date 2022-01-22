namespace Devcorp.Controls.Design;

/// <summary>
///     Structure to define CIE XYZ.
/// </summary>
public struct CIEXYZ
{
    /// <summary>
    ///     Gets an empty CIEXYZ structure.
    /// </summary>
    public static readonly CIEXYZ Empty = new();

    /// <summary>
    ///     Gets the CIE D65 (white) structure.
    /// </summary>
    public static readonly CIEXYZ D65 = new(0.9505, 1.0, 1.0890);

    #region Fields

    private double x;
    private double y;
    private double z;

    #endregion

    #region Operators

    public static bool operator ==(CIEXYZ item1, CIEXYZ item2) {
        return item1.X == item2.X
               && item1.Y == item2.Y
               && item1.Z == item2.Z;
    }

    public static bool operator !=(CIEXYZ item1, CIEXYZ item2) {
        return item1.X != item2.X
               || item1.Y != item2.Y
               || item1.Z != item2.Z;
    }

    #endregion

    #region Accessors

    /// <summary>
    ///     Gets or sets X component.
    /// </summary>
    public double X {
        get => x;
        set => x = value > 0.9505 ? 0.9505 : value < 0 ? 0 : value;
    }

    /// <summary>
    ///     Gets or sets Y component.
    /// </summary>
    public double Y {
        get => y;
        set => y = value > 1.0 ? 1.0 : value < 0 ? 0 : value;
    }

    /// <summary>
    ///     Gets or sets Z component.
    /// </summary>
    public double Z {
        get => z;
        set => z = value > 1.089 ? 1.089 : value < 0 ? 0 : value;
    }

    #endregion

    public CIEXYZ(double x, double y, double z) {
        this.x = x > 0.9505 ? 0.9505 : x < 0 ? 0 : x;
        this.y = y > 1.0 ? 1.0 : y < 0 ? 0 : y;
        this.z = z > 1.089 ? 1.089 : z < 0 ? 0 : z;
    }

    #region Methods

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }

        return this == (CIEXYZ)obj;
    }

    public override int GetHashCode() {
        return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
    }

    #endregion
}