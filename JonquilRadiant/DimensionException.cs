using System;
namespace JonquilRadiant
{
	public class DimensionException : Exception
	{
		public DimensionException () : base ("Dimensional quantity used where dimensionless quantity was expected")
		{
		}

		public DimensionException (string message) : base (message)
		{
		}
	}
}

