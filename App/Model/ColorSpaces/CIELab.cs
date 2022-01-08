using System;

namespace Devcorp.Controls.Design
{
	/// <summary>
	/// Structure to define CIE L*a*b*.
	/// </summary>
	public struct CIELab
	{
		/// <summary>
		/// Gets an empty CIELab structure.
		/// </summary>
		public static readonly CIELab Empty = new CIELab();

		#region Fields
		private double l;
		private double a;
		private double b;

		#endregion

		#region Operators
		public static bool operator ==(CIELab item1, CIELab item2) {
			return (
				item1.L == item2.L
				&& item1.A == item2.A
				&& item1.B == item2.B
				);
		}

		public static bool operator !=(CIELab item1, CIELab item2) {
			return (
				item1.L != item2.L
				|| item1.A != item2.A
				|| item1.B != item2.B
				);
		}

		#endregion

		#region Accessors
		/// <summary>
		/// Gets or sets L component.
		/// </summary>
		public double L {
			get => l;
			set => l = value;
		}

		/// <summary>
		/// Gets or sets a component.
		/// </summary>
		public double A {
			get => a;
			set => a = value;
		}

		/// <summary>
		/// Gets or sets a component.
		/// </summary>
		public double B {
			get => b;
			set => b = value;
		}

		#endregion

		public CIELab(double l, double a, double b) {
			this.l = l;
			this.a = a;
			this.b = b;
		}

		#region Methods
		public override bool Equals(Object obj) {
			if (obj == null || GetType() != obj.GetType()) return false;

			return (this == (CIELab)obj);
		}

		public override int GetHashCode() {
			return L.GetHashCode() ^ a.GetHashCode() ^ b.GetHashCode();
		}

		#endregion
	}
}
