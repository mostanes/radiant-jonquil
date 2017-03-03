using System;
namespace JonquilRadiant
{
	public struct SuperDouble
	{
		const long PivotExp = 50;
		const long PivotSquared = PivotExp * PivotExp;
		const long PivotGuarantee = 2 * PivotExp + 10;
		// Required by code
		const double PivotBase = 10.0;
		// Must be smaller than E+PivotExp and larger (in mod) than E-PivotExp
		readonly double val;
		readonly long exp;

		public SuperDouble (double value, long exponent)
		{
			long nep = (long)(Math.Floor (Math.Log10 (value)));
			if (nep * nep > PivotSquared) {
				exponent+= nep;
				value /= Math.Pow (PivotBase, nep);
			}
			exp = exponent;
			val = value;
		}

		public static bool IsInfinity (SuperDouble d)
		{
			return double.IsInfinity (d.val);
		}

		public static bool IsNaN (SuperDouble d)
		{
			return double.IsNaN (d.val);
		}

		internal static bool IsNegative (SuperDouble d)
		{
			return d.val < 0;
		}

		public static bool IsNegativeInfinity (SuperDouble d)
		{
			return d.val == double.NegativeInfinity;
		}

		public static bool IsPositiveInfinity (SuperDouble d)
		{
			return d.val == double.PositiveInfinity;
		}

		public static SuperDouble Parse (string s)
		{
			int pe = s.IndexOf ('e');
			if (pe == -1) pe = s.IndexOf ('E');
			if (pe != -1) {
				double dw = double.Parse (s.Substring (0, pe));
				long exp = long.Parse (s.Substring (pe + 1));
				return new SuperDouble (dw, exp);
			} else return new SuperDouble (double.Parse (s), 0);
		}


		//
		// Methods
		//
		public int CompareTo (SuperDouble value)
		{
			if (this < value) {
				return -1;
			}
			if (this > value) {
				return 1;
			}
			if (this == value) {
				return 0;
			}
			if (!double.IsNaN (val)) {
				return 1;
			}
			if (!double.IsNaN (value.val)) {
				return -1;
			}
			return 0;
		}

		public int CompareTo (object value)
		{
			if (value == null) {
				return 1;
			}
			if (!(value is SuperDouble)) {
				throw new ArgumentException ("Argument must be SuperDouble");
			}
			return CompareTo ((SuperDouble)value);
		}

		public bool Equals (SuperDouble obj)
		{
			return obj == this || (double.IsNaN (obj.val) && double.IsNaN (val));
		}

		public override bool Equals (object obj)
		{
			if (!(obj is SuperDouble)) {
				return false;
			}
			SuperDouble num = (SuperDouble)obj;
			return num == this || (double.IsNaN (num.val) && double.IsNaN (val));
		}

		public override int GetHashCode ()
		{
			return val.GetHashCode ();
		}

		public static explicit operator bool(SuperDouble value)
		{
			return value.val != 0.0;
		}

		public static explicit operator SuperDouble (decimal value)
		{
			SuperDouble result = new SuperDouble ((double)value, 0);
			return result;
		}

		public static explicit operator SuperDouble (double value)
		{
			return new SuperDouble (value, 0);
		}

		public static explicit operator SuperDouble (short value)
		{
			SuperDouble result = new SuperDouble ((double)value, 0);
			return result;
		}

		public static explicit operator SuperDouble (int value)
		{
			SuperDouble result = new SuperDouble ((double)value, 0);
			return result;
		}

		public static explicit operator SuperDouble (long value)
		{
			SuperDouble result = new SuperDouble ((double)value, 0);
			return result;
		}

		public static explicit operator SuperDouble (ushort value)
		{
			SuperDouble result = new SuperDouble ((double)value, 0);
			return result;
		}

		public static explicit operator SuperDouble (uint value)
		{
			SuperDouble result = new SuperDouble ((double)value, 0);
			return result;
		}

		public static explicit operator SuperDouble (ulong value)
		{
			SuperDouble result = new SuperDouble ((double)value, 0);
			return result;
		}
		public static explicit operator decimal (SuperDouble value)
		{
			return (decimal)((double)value);
		}

		public static explicit operator double (SuperDouble value)
		{
			double d = value.val * Math.Pow (10, value.exp);
			return d;
		}

		public static explicit operator short (SuperDouble value)
		{
			return (short)((double)value);
		}

		public static explicit operator int (SuperDouble value)
		{
			return (int)((double)value);
		}

		public static explicit operator long (SuperDouble value)
		{
			return (long)((double)value);
		}

		public static explicit operator ushort (SuperDouble value)
		{
			return (ushort)((double)value);
		}

		public static explicit operator uint (SuperDouble value)
		{
			return (uint)((double)value);
		}

		public static explicit operator ulong (SuperDouble value)
		{
			return (ulong)((double)value);
		}

		public static bool operator== (SuperDouble a, SuperDouble b)
		{
			return (a.val == b.val) & (a.exp == b.exp);
		}

		public static bool operator != (SuperDouble a, SuperDouble b)
		{
			return !(a == b);
		}

		public override string ToString ()
		{
			double rval = val;
			long rexp = exp;
			if (exp != 0) {
				for (; rval < 1; rexp--) rval *= 10;
				for (; rval > 10; rexp++) rval /= 10;
			}
			string s = rval.ToString ();
			int i0 = s.IndexOf ('e');
			if (i0 == -1) i0 = s.IndexOf ('E');
			if (i0 != -1) {
				int epn = int.Parse (s.Substring (i0 + 2));
				long epoint = epn + rexp;
				return s.Substring (0, i0 + 2) + epoint.ToString ();
			} else { if (rexp != 0) s += "E+" + rexp.ToString (); }
			return s;
		}

		/* Broken */
		[Obsolete]
		public string ToString (string format)
		{
			double rval = val;
			long rexp = exp;
			if (exp != 0) {
				for (; rval < 1; rexp--) rval *= 10;
				for (; rval > 10; rexp++) rval /= 10;
			}
			string s = rval.ToString (format);
			int i0 = s.IndexOf ('e');
			if (i0 == -1) i0 = s.IndexOf ('E');
			if (i0 != -1) {
				int epn = int.Parse (s.Substring (i0 + 2));
				long epoint = epn + rexp;
				return s.Substring (0, i0 + 2) + epoint.ToString ();
			} else { if (rexp != 0) s += "E+" + rexp.ToString (); }
			return s;
		}

		public static bool operator < (SuperDouble a, SuperDouble b)
		{
			long deltaExp = b.exp - a.exp;
			if (deltaExp > PivotGuarantee) return true;
			if (deltaExp < -PivotGuarantee) return false;
			double v1 = b.val * Math.Pow (PivotBase, deltaExp);
			if (v1 > a.val) return true;
			else return false;
		}

		public static bool operator > (SuperDouble a, SuperDouble b)
		{
			long deltaExp = b.exp - a.exp;
			if (deltaExp < -PivotGuarantee) return true;
			if (deltaExp > PivotGuarantee) return false;
			double v1 = b.val * Math.Pow (PivotBase, deltaExp);
			if (v1 < a.val) return true;
			else return false;
		}

		public static SuperDouble operator * (SuperDouble a, SuperDouble b)
		{
			double v0 = a.val * b.val;
			long ep = a.exp + b.exp;
			long nep = (long)(Math.Floor (Math.Log10 (v0)));
			if (nep * nep > PivotSquared) {
				ep += nep;
				v0 /= Math.Pow (PivotBase, nep);
			}
			return new SuperDouble (v0, ep);
		}

		public static SuperDouble operator - (SuperDouble a, SuperDouble b)
		{
			long deltaExp = b.exp - a.exp;
			if (deltaExp < -PivotGuarantee) return a;
			if (deltaExp > PivotGuarantee) return new SuperDouble (-b.val, b.exp);
			double v1 = b.val * Math.Pow (PivotBase, deltaExp);
			double v0 = a.val - v1;
			long ep = a.exp;
			long nep = (long)(Math.Floor (Math.Log10 (v0)));
			if (nep * nep > PivotSquared) {
				ep += nep;
				v0 /= Math.Pow (PivotBase, nep);
			}
			return new SuperDouble (v0, ep);
		}

		public static SuperDouble operator + (SuperDouble a, SuperDouble b)
		{
			long deltaExp = b.exp - a.exp;
			if (deltaExp < -PivotGuarantee) return a;
			if (deltaExp > PivotGuarantee) return b;
			double v1 = b.val * Math.Pow (PivotBase, deltaExp);
			double v0 = a.val + v1;
			long ep = a.exp;
			long nep = (long)(Math.Floor (Math.Log10 (v0)));
			if (nep * nep > PivotSquared) {
				ep += nep;
				v0 /= Math.Pow (PivotBase, nep);
			}
			return new SuperDouble (v0, ep);
		}

		public static SuperDouble operator / (SuperDouble a, SuperDouble b)
		{
			double v0 = a.val / b.val;
			long ep = a.exp - b.exp;
			long nep = (long)(Math.Floor (Math.Log10 (v0)));
			if (nep * nep > PivotSquared) {
				ep += nep;
				v0 /= Math.Pow (PivotBase, nep);
			}
			return new SuperDouble (v0, ep);
		}

		public static SuperDouble operator ^ (SuperDouble a, SuperDouble b)
		{
			double za = Math.Log10 (a.val);
			za += a.exp;
			double rw = b.val * Math.Pow (10, b.exp);
			rw *= za;
			if (double.IsPositiveInfinity (rw)) return new SuperDouble (double.PositiveInfinity, 0);
			if (double.IsNegativeInfinity (rw)) return new SuperDouble (0, 0);
			long flr = (long)Math.Floor (rw);
			rw -= flr;
			return new SuperDouble (Math.Pow (10, rw), flr);
		}
	}
}

