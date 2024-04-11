using UnityEngine;
using static _Scripts._MyStuff.CityLayout.Node;

namespace _Scripts._MyStuff.CityLayout
{
	public struct Connection
	{
		public Node a;
		public Node b;
		public Direction aDirection;
		public Direction bDirection;

		public void InformNodes()
		{
			a.connections[aDirection] = b;
			b.connections[bDirection] = a;
		}
	}
}