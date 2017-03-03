using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JonquilRadiant;

namespace JonquilCalc
{
	public static class LineParser
	{

		public static List<string> symb = new List<string> () { "+", "-", "*", "/", "^" };
		static Dictionary<char, int> levels = new Dictionary<char, int> () { { '+', 1 }, { '-', 1 }, { '*', 2 }, { '/', 2 }, { '^', 3 } };

		const int LongestSymb = 2;

		/*
		public static void ParseSolveLine (string line)
		{
			string[] tokens = line.Split (' ');
			List<SuperDouble> pdou = new List<SuperDouble> ((tokens.Length + 1) / 2);
			int i;
			for (i = 0; i < tokens.Length; i += 2) pdou.Add (SuperDouble.Parse (tokens[i]));
			List<string> symb = new List<string> (tokens.Length / 2);
			for (i = 1; i < tokens.Length; i += 2) symb.Add (tokens[i]);

			while (symb.Contains ("^")) {
				int idx = symb.IndexOf ("^");
				SuperDouble sd2 = pdou[idx] ^ pdou[idx + 1];
				pdou[idx] = sd2;
				pdou.RemoveAt (idx + 1);
				symb.RemoveAt (idx);
			}
			while (symb.Contains ("*") | symb.Contains("/")) {
				int idx1 = symb.IndexOf ("*"), idx2 = symb.IndexOf ("/");
				int idx = idx1 < 0 ? idx2 : idx1;
				if (idx > idx2 & idx2 != -1) idx = idx2;
				SuperDouble sd2 = (symb[idx] == "*" ? pdou[idx] * pdou[idx + 1] : pdou[idx] / pdou[idx + 1]);
				pdou[idx] = sd2;
				pdou.RemoveAt (idx + 1);
				symb.RemoveAt (idx);
			}
			while (symb.Contains("+") | symb.Contains("-")){
				int idx1 = symb.IndexOf ("+"), idx2 = symb.IndexOf ("-");
				int idx = idx1 < 0 ? idx2 : idx1;
				if (idx > idx2 & idx2 != -1) idx = idx2;
				SuperDouble sd2 = (symb[idx] == "+" ? pdou[idx] + pdou[idx + 1] : pdou[idx] - pdou[idx + 1]);
				pdou[idx] = sd2;
				pdou.RemoveAt (idx + 1);
				symb.RemoveAt (idx);
			}
			SuperDouble res = pdou[0];
			Console.WriteLine (res);
			;
		}
		*/

		public static HilbertVector InterpretLine (string line)
		{
			StringBuilder sb = new StringBuilder (line);
			int i;
			for (i = 0; i < sb.Length; i++) if (char.IsWhiteSpace (sb[i])) sb.Remove (i, 1);
			int p0 = 0;
			Node w = ParseAreaLevel (sb, ref p0, 1);
			if (w == null) throw new ArgumentException ("Argument not parsed. Empty line?");
			HilbertVector hv = Compute (w);
			return hv;
		}

		static Node ParseAreaLevel (StringBuilder line, ref int pos, int level)
		{
			Node n;
			Node cn;
			Node tree = null;
			SuperDouble sd;
			HilbertVector hv;
			char op = '\0';
			bool flag = false;
			while (pos < line.Length) {
				while (true) {
					if (TryReadNumber (line, ref pos, out sd)) {
						hv = new HilbertVector (sd);
						HilbertVector h2;
						if (TryReadComplexUnit (line, ref pos, out h2)) hv *= h2;
						cn = new Node () { isLeaf = true, vect = hv };
						break;
					}
					if (line[pos] == '(') { pos++; cn = ParseAreaLevel (line, ref pos, 1); break; }
					/* if (line[pos] == ')') { pos++; return tree; } */
					throw new FormatException ("Could not parse");
				}
				if (pos >= line.Length) flag = true;
				if (!flag) flag = (line[pos] == ')');
				if (flag) {
					pos++;
					if (tree == null) tree = new Node () { isLeaf = false, ch = new List<Tuple<Node, char>> () { new Tuple<Node, char> (cn, op) } };
					else tree.ch.Add (new Tuple<Node, char> (cn, op));
					return tree;
				}
				char z = line[pos];
				if (levels[line[pos]] < level) {
					if (tree == null) return cn;
					tree.ch.Add (new Tuple<Node, char> (cn, op));
					return tree;
				}
				if (levels[line[pos]] == level) {
					if (tree == null) tree = new Node () { isLeaf = false, ch = new List<Tuple<Node, char>> () { new Tuple<Node, char> (cn, op) } };
					else tree.ch.Add (new Tuple<Node, char> (cn, op));
				}
				if (levels[line[pos]] > level) {
					int pp = pos + 1;
					n = new Node () { ch = new List<Tuple<Node, char>> () { new Tuple<Node, char> (cn, '\0'), new Tuple<Node, char> (ParseAreaLevel (line, ref pp, levels[line[pos]]), line[pos]) } };
					pos = pp;
					if (tree == null) tree = new Node () { isLeaf = false, ch = new List<Tuple<Node, char>> () };
					tree.ch.Add (new Tuple<Node, char>(n, op));
				}
				op = z;
				pos++;
			}
			return tree;
		}

		static HilbertVector Compute (Node n)
		{
			if (n.isLeaf) return n.vect;
			HilbertVector vec = default (HilbertVector);
			foreach (var cz in n.ch) {
				switch (cz.Item2) {
				case '\0':
					vec = Compute (cz.Item1);
					break;
				case '+':
					vec += Compute (cz.Item1);
					break;
				case '-':
					vec -= Compute (cz.Item1);
					break;
				case '*':
					vec *= Compute (cz.Item1);
					break;
				case '/':
					vec /= Compute (cz.Item1);
					break;
				case '^':
					vec ^= Compute (cz.Item1);
					break;
				}
			}
			return vec;
		}

		static bool TryReadNumber (StringBuilder line, ref int pos, out SuperDouble val)
		{
			if (pos >= line.Length) { val = default (SuperDouble); return false; }
			if (!char.IsDigit (line[pos])) { val = default(SuperDouble); return false; }
			SuperDouble sd = ReadSD (line, ref pos);
			val = sd;
			return true;
		}

		static bool TryReadBasicUnit (StringBuilder line, ref int pos, out Unit val)
		{
			if (pos >= line.Length) { val = default (Unit); return false; }
			if (!char.IsLetter (line[pos])) { val = default (Unit); return false; }
			string sd = ReadStr (line, ref pos);
			val = UnitHolder.GetRegisteredUnit (sd);
			return true;
		}

		static bool TryReadComplexUnit (StringBuilder line, ref int pos, out HilbertVector val)
		{
			int p0 = pos;
			Unit vl;
			if (!TryReadBasicUnit (line, ref pos, out vl)) { val = default (HilbertVector); return false; }
			int pw;
			int cp = pos;
			/* for (cp = pos; char.IsWhiteSpace (line[cp]); cp++) ; */
			if (cp < line.Length) {
				if (line[cp] != '^') pw = 1;
				else {
					cp++;
					SuperDouble sd;
					if (!TryReadNumber (line, ref cp, out sd)) { pos = p0; val = default (HilbertVector); return false; }
					pw = (int)sd;
					pos = cp;
				}
			} else { val = new HilbertVector (vl, 1); return true; }
			HilbertVector hv = new HilbertVector (vl, pw);
			HilbertVector h2;
			HilbertVector ret;
			if (cp >= line.Length) { val = hv; return true; }
			if (line[cp] == '*') {
				cp++;
				if (TryReadComplexUnit (line, ref cp, out h2)) { ret = hv * h2; pos = cp; } else ret = hv;
			} else if (line[cp] == '/') {
				cp++;
				if (TryReadComplexUnit (line, ref cp, out h2)) { ret = hv / h2; pos = cp; } else ret = hv;
			} else ret = hv;
			val = ret;
			return true;
		}

		public static SuperDouble ReadSD (StringBuilder line, ref int pos)
		{
			int cp;
			for (cp = pos; cp < line.Length; cp++) if (!char.IsDigit (line[cp])) break;
			if (cp != line.Length) {
				if (line[cp] == 'E' | line[cp] == 'e') cp++;
				for (; cp < line.Length; cp++) if (!char.IsDigit (line[cp])) break;
			}
			string sl = line.ToString (pos, cp - pos);
			SuperDouble sd = SuperDouble.Parse (sl);
			pos = cp;
			return sd;
		}

		public static string ReadStr (StringBuilder line, ref int pos)
		{
			int cp;
			for (cp = pos; cp < line.Length; cp++) if (!char.IsLetter (line[cp])) break;
			string sl = line.ToString (pos, cp - pos);
			pos = cp;
			return sl;
		}
	}
}

