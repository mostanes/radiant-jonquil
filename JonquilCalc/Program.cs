using System;

namespace JonquilCalc
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("JonquilRadiant!");
			JonquilRadiant.UnitHolder.InitHolder ();
			var z = LineParser.InterpretLine (Console.ReadLine ());
			Console.WriteLine (z.ToString ());
			Console.ReadKey ();
		}
	}
}
