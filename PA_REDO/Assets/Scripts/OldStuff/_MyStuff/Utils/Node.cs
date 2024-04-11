using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts._MyStuff.CityLayout
{
	[Serializable]
	public class Node
	{
		public float x;
		public float z;

		// [CanBeNull] public Area area;

		public readonly List<Node> connectionsOld = new();
		// [FormerlySerializedAs("parentArea")] public AreaOld _parentAreaOld;

		public Dictionary<Direction, Node> connections;

		public Node()
		{
			connections = new Dictionary<Direction, Node>()
			{
				{Direction.North, null},
				{Direction.West, null},
				{Direction.South, null},
				{Direction.East, null},
			};
		}

		public int ConnectionCount(bool pDebug = false)
		{
			int count = 0;
			foreach (Direction direction in connections.Keys)
			{
				if (connections[direction] != null)
				{
					if (pDebug)	Debug.Log(direction);
					count++;
				}
			}

			return count;
		}

		public enum Direction
		{
			North, West, South, East, None
		}

		public static Direction GetOppositeDirection(Direction pDirection)
		{
			return pDirection switch
			{
				Direction.North => Direction.South,
				Direction.West => Direction.East,
				Direction.South => Direction.North,
				Direction.East => Direction.West,
				Direction.None => Direction.None,
				_ => throw new ArgumentOutOfRangeException(nameof(pDirection), pDirection, null)
			};
		}

		public static void ConnectNodes(Node pA, Node pB)
		{
			Direction dirAToB = VectorToDirection(pB.ToVector3() - pA.ToVector3());
			Direction dirBToA = VectorToDirection(pA.ToVector3() - pB.ToVector3());
			
			pA.connections.Add(dirAToB, pB);
			pB.connections.Add(dirBToA, pA);
		}

		public static Vector3 DirectionToVector(Direction pDirection)
		{
			return pDirection switch
			{
				Direction.North => new Vector3(0, 0, 1),
				Direction.West => new Vector3(-1, 0, 0),
				Direction.South => new Vector3(0, 0, -1),
				Direction.East => new Vector3(1, 0, 0),
				Direction.None => Vector3.zero,
				_ => throw new ArgumentOutOfRangeException(nameof(pDirection), pDirection, null)
			};
		}

		public static Direction? VectorToExactDirection(Vector3 pVector)
		{
			Vector3 normalized = pVector.normalized;
			int x = Round(normalized.x);
			int z = Round(normalized.z);

			int Round(float pFloat)
			{
				return pFloat switch
				{
					>= 0.999f => 1,
					<= -0.999f => -1,
					_ => 0
				};
			}

			return x switch
			{
				0 when z == 1 => Direction.North,
				-1 when z == 0 => Direction.West,
				0 when z == -1 => Direction.South,
				1 when z == 0 => Direction.East,
				_ => null
			};
		}
		
		public static Direction VectorToDirection(Vector3 pVector)
		{
			Vector3 normalized = pVector.normalized;
			float x = normalized.x;
			float y = normalized.z;

			if (x is <= 0 and >= -1 
			    && y is <= 1 and >= -1)
			{
				return Direction.West;
			}
			
			if (x is >= -1 and <= 1 
			    && y is <= 0 and >= -1)
			{
				return Direction.South;
			}
			
			if (x is >= 0 and <= 1 
			    && y is <= 1 and >= -1)
			{
				return Direction.East;
			}
			
			if (x is >= -1 and <= 1 
			    && y is <= 1 and >= 0)
			{
				return Direction.North;
			}

			return Direction.None;
		}

		public Vector3 ToVector3()
		{
			return new Vector3(x, 0, z);
		}

		public Vector2 ToVector2()
		{
			return new Vector2(x, z);
		}
		
		public float DistanceTo(Node pNode)
		{
			return Vector3.Distance(ToVector3(), pNode.ToVector3());
		}
	}
}