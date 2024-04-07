using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
	public struct Building
	{
		public enum Direction
		{
			North, East, South, West
		}

		public Dictionary<Direction, Wall> Walls;
	}

	public struct Wall
	{
		public Vector3 TopLeft;
		public Vector3 BotRight;
		public Mesh Mesh;
	}
}