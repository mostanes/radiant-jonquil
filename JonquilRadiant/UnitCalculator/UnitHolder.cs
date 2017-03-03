using System;
using System.Collections.Generic;
using System.Linq;

namespace JonquilRadiant
{
	public struct Modifier
	{
		public readonly string[] Prefixes;
		public readonly double Multiplier;

		public Modifier (string[] prefixes, double multiplier)
		{
			Prefixes = prefixes;
			Multiplier = multiplier;
		}
	}

	public class Unit
	{
		public readonly Dimension type;
		public readonly string unitName;
		public readonly Unit baseUnit;
		public readonly double baseMultiplier;
		public readonly string shorthand;

		public Unit (Dimension Type, string UnitName, string ShortHand, Unit BaseUnit, double BaseMultiplier)
		{
			type = Type;
			unitName = UnitName;
			baseUnit = BaseUnit;
			baseMultiplier = BaseMultiplier;
			shorthand = ShortHand;
		}

		public override string ToString ()
		{
			return shorthand;
		}
	}

	public struct UnitPower
	{
		public readonly Unit unit;
		public readonly int power;

		public UnitPower (Unit u, int p)
		{
			unit = u;
			power = p;
		}
		/* Beware of equality when non-holder types are used; comparison is done on ref of unit */

		public override string ToString ()
		{
			if (power == 0) return string.Empty;
			string s = unit.ToString ();
			if (power != 1) s = "(" + s + ")^" + power.ToString();
			return s;
		}
	}

	public class CompositeUnit : Unit
	{
		public readonly UnitPower[] Factors;

		public CompositeUnit (UnitPower[] Factors) : base (null, null, null, null, 1.0)
		{
			this.Factors = Factors;
		}

		public CompositeUnit (UnitPower[] Factors, string Name, string ShortHand) : base (null, Name, ShortHand, null, 1.0)
		{
			this.Factors = Factors;
		}

		public CompositeUnit (string[] factors, int[] powers, string name, string shorthand) : base (null, name, shorthand, null, 1.0)
		{
			if (factors.Length != powers.Length) throw new ArgumentException ();
			this.Factors = new UnitPower[factors.Length];
			int i;
			for (i = 0; i < factors.Length; i++) this.Factors[i] = new UnitPower (UnitHolder.GetRegisteredUnit (factors[i]), powers[i]);
		}

		public override string ToString ()
		{
			string s = string.Empty;
			foreach (UnitPower up in Factors) s += up.ToString();
			return s;
		}
	}

	public class Dimension
	{
		public readonly string Name;

		public Dimension (string name)
		{
			Name = name;
		}
	}

	public class UnitHolder
	{
		static List<Modifier> modifiers;

		static List<Dimension> dimensions;

		static List<Unit> baseUnits = new List<Unit> ();

		static List<Unit> units;

		public static List<Unit> Units {
			get {
				return units;
			}}

		public static Unit GetRegisteredUnit (string name)
		{
			foreach (Unit u in units) {
				if (name == u.unitName) return u;
				if (name == u.shorthand) return u;
			}
			foreach (Unit u in units) {
				if (name.EndsWith (u.shorthand)) {
					string sl = name.Substring (0, name.Length - u.shorthand.Length);
					Modifier m = modifiers.Find ((Modifier obj) => obj.Prefixes.Any ((string s) => s == sl));
					Unit k = new Unit (u.type, m.Prefixes[1] + u.unitName, m.Prefixes[0] + u.shorthand, u, m.Multiplier);
					units.Add (k);
					return k;
				}
				if (name.EndsWith (u.unitName)) {
					string sl = name.Substring (0, name.Length - u.unitName.Length);
					Modifier m = modifiers.Find ((Modifier obj) => obj.Prefixes.Any ((string s) => s == sl));
					Unit k = new Unit (u.type, m.Prefixes[1] + u.unitName, m.Prefixes[0] + u.shorthand, u, m.Multiplier);
					units.Add (k); return k;
				}
			}
			return null;
		}

		public static void InitHolder ()
		{
			modifiers = new List<Modifier> () {
			new Modifier (new string[] { "k", "kilo", "K", "Kilo" }, Math.Pow(10, 3)),
			new Modifier (new string[] { "M", "Mega" }, Math.Pow(10, 6)),
			new Modifier (new string[] { "G", "Giga" }, Math.Pow(10, 9)),
			new Modifier (new string[] { "T", "Tera" }, Math.Pow(10, 12)),
			new Modifier (new string[] { "P", "Peta" }, Math.Pow(10, 15)),
			new Modifier (new string[] { "E", "Exa" }, Math.Pow(10, 18)),
			new Modifier (new string[] { "Z", "Zetta" }, Math.Pow(10, 21)),
			new Modifier (new string[] { "Y", "Yotta" }, Math.Pow(10, 24)),
			new Modifier(new string[] { "h", "hecto" }, Math.Pow(10, 2)),
			new Modifier(new string[] { "da", "deca" }, Math.Pow(10, 1)),
			new Modifier(new string[] { "d", "deci" }, Math.Pow(10, -1)),
			new Modifier(new string[] { "c", "centi" }, Math.Pow(10, -2)),
			new Modifier(new string[] { "m", "milli" }, Math.Pow(10, -3)),
			new Modifier(new string[] { "μ", "micro" }, Math.Pow(10, -6)),
			new Modifier(new string[] { "n", "nano" }, Math.Pow(10, -9)),
			new Modifier(new string[] { "p", "pico" }, Math.Pow(10, -12)),
			new Modifier(new string[] { "f", "femto" }, Math.Pow(10, -15)),
			new Modifier(new string[] { "a", "atto" }, Math.Pow(10, -18)),
			new Modifier(new string[] { "z", "zepto" }, Math.Pow(10, -21)),
			new Modifier(new string[] { "y", "yocto" }, Math.Pow(10, -24))
			};

			dimensions = new List<Dimension> () {
			new Dimension ("Mass"),
			new Dimension ("Time"),
			new Dimension ("Length"),
			new Dimension ("Electric charge"),
			new Dimension ("Temperature")
			};

			units = new List<Unit> (){
			new Unit(dimensions[0], "gram", "g", null, 1.0),
			new Unit(dimensions[1], "second", "s", null, 1.0),
			new Unit(dimensions[2], "meter", "m", null, 1.0),
			new Unit(dimensions[3], "Coulomb", "C", null, 1.0),
			new Unit(dimensions[4], "Kelvin", "K", null, 1.0),
			};

			units.AddRange (new Unit[] {
				new CompositeUnit(new string[]{"second"}, new int[]{-1}, "Hertz", "Hz"),
				new CompositeUnit(new string[] {"kilogram", "meter", "second"}, new int[]{1,1,-2}, "Newton", "N"),
				new CompositeUnit(new string[] {"kilogram", "meter", "second"}, new int[]{1,-1,-2}, "Pascal", "Pa"),
				new CompositeUnit(new string[] { "kilogram", "meter", "second"}, new int[] {1,2,-3}, "Watt", "W"),
				new CompositeUnit(new string[] { "Coulomb", "second" }, new int[]{1,-1}, "Ampere", "A"),
				new CompositeUnit(new string[] { "meter", "second" }, new int[] { 2,-2}, "Gray", "Gy"),
				new CompositeUnit(new string[] { "meter", "second" }, new int[] { 2,-2}, "Sievert", "Sv"),
				new CompositeUnit(new string[] { "second" }, new int[] { -1}, "Bequerel", "Bq"),
				new CompositeUnit(new string[] { "kilogram", "meter", "second" }, new int[] { 1, 2, -2}, "Joule", "J")
			});

			units.AddRange (new Unit[] {
				new CompositeUnit(new string[] { "Watt", "Ampere" }, new int[] { 1, -1}, "Volt", "V"),
				new CompositeUnit(new string[] { "Coulomb", "Joule" }, new int[] {2,-1}, "Farad", "F"),
				new CompositeUnit(new string[] { "Watt", "Ampere" }, new int[] { 1, -2}, "Ohm", "Ω"),
				new CompositeUnit(new string[] { "Watt", "Ampere" }, new int[] { -1, 2}, "Siemens", "S"),
				new CompositeUnit(new string[] { "Joule", "Ampere" }, new int[] {1,-1}, "Weber", "Wb"),
				new CompositeUnit(new string[] { "kilogram", "second", "Ampere" }, new int[] { 1, -2, -1}, "Tesla", "T"),
				new CompositeUnit(new string[] { "Joule", "Ampere" }, new int[] {1,-2}, "Henry", "H")
			});
		}

		public UnitHolder ()
		{
		}
	}
}

