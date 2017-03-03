using System;
using System.Collections.Generic;

namespace JonquilCalc
{
	public class Node
	{
		internal bool isLeaf;
		internal JonquilRadiant.HilbertVector vect;
		internal List<Tuple<Node, char>> ch;

		public Node ()
		{
		}

	}
}

