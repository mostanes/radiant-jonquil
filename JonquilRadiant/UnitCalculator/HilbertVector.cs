using System;
using System.Collections.Generic;
using System.Linq;

namespace JonquilRadiant
{
	public struct HilbertVector
	{
		SuperDouble value;
		List<UnitPower> vector;

		const double PreCVal = 100000.0 * 100000.0 * 100000.0;

		public HilbertVector (SuperDouble sd)
		{
			value = sd;
			vector = new List<UnitPower> ();
		}

		public HilbertVector (Unit u, int power)
		{
			value = (SuperDouble)1.0;
			vector = new List<UnitPower> () { new UnitPower (u, power) };
			while (vector.Any ((UnitPower arg) => arg.unit is CompositeUnit | arg.unit.baseUnit != null)) {
				int j = vector.FindIndex ((UnitPower obj) => obj.unit is CompositeUnit | obj.unit.baseUnit != null);
				if (vector[j].unit is CompositeUnit) {
					CompositeUnit comu = vector[j].unit as CompositeUnit;
					int pw = vector[j].power;
					vector.RemoveAt (j);
					foreach (UnitPower z in comu.Factors) vector.Add (new UnitPower (z.unit, z.power * pw));
				} else {
					value *= ((SuperDouble)vector[j].unit.baseMultiplier) ^ (SuperDouble)vector[j].power;
					vector[j] = new UnitPower (vector[j].unit.baseUnit, vector[j].power);
				}
			}
		}

		public HilbertVector (SuperDouble sd, List<UnitPower> power)
		{
			value = sd;
			vector = power;
		}

		public static HilbertVector operator + (HilbertVector a, HilbertVector b)
		{
			foreach (UnitPower up in a.vector) {
				if (!b.vector.Contains (up)) throw new ArgumentException ("Not matching dimension types");
			}
			HilbertVector hv = new HilbertVector (a.value + b.value, a.vector);
			return hv;
		}

		public static HilbertVector operator - (HilbertVector a, HilbertVector b)
		{
			foreach (UnitPower up in a.vector) {
				if (!b.vector.Contains (up)) throw new ArgumentException ("Not matching dimension types");
			}
			HilbertVector hv = new HilbertVector (a.value - b.value, a.vector);
			return hv;
		}


		public static HilbertVector operator * (HilbertVector a, HilbertVector b)
		{
			List<UnitPower> up = new List<UnitPower> ();
			foreach (UnitPower ua in a.vector) {
				int ind = b.vector.FindIndex ((UnitPower obj) => obj.unit == ua.unit);
				if (ind == -1) { up.Add (ua); continue; }
				UnitPower ub = b.vector[ind];
				b.vector.RemoveAt (ind);
				UnitPower nu = new UnitPower (ua.unit, ua.power + ub.power);
				up.Add (nu);
			}
			up.AddRange (b.vector);
			return new HilbertVector (a.value * b.value, up);
		}

		public static HilbertVector operator / (HilbertVector a, HilbertVector b)
		{
			List<UnitPower> up = new List<UnitPower> ();
			foreach (UnitPower ua in a.vector) {
				int ind = b.vector.FindIndex ((UnitPower obj) => obj.unit == ua.unit);
				if (ind == -1) { up.Add (ua); continue; }
				UnitPower ub = b.vector[ind];
				b.vector.RemoveAt (ind);
				UnitPower nu = new UnitPower (ua.unit, ua.power - ub.power);
				up.Add (nu);
			}
			up.AddRange (b.vector.Select ((UnitPower o) => new UnitPower (o.unit, -o.power)));
			return new HilbertVector (a.value / b.value, up);
		}

		public static HilbertVector operator ^ (HilbertVector a, HilbertVector b)
		{
			if (b.vector.Count != 0) throw new DimensionException ();
			if (a.vector.Count == 0) return new HilbertVector (a.value ^ b.value);
			if (b.value > (SuperDouble)1000) throw new DimensionException("Dimension Exponent Too Large");
			double d = (double)b.value;
			if (Math.Abs (Math.Round (d) - d) * PreCVal > Math.Abs (d)) throw new DimensionException();
			int bv = (int)d;
			List<UnitPower> np = a.vector.Select ((UnitPower arg) => new UnitPower (arg.unit, arg.power * bv)).ToList ();
			HilbertVector hv = new HilbertVector (a.value ^ b.value, np);
			return hv;
		}

		public override string ToString ()
		{
			string w = value.ToString ();
			foreach (UnitPower up in vector) w += up.ToString ();
			return w;
		}
	}
}

