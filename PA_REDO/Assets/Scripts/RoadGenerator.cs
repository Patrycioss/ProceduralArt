using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(LayoutGenerator))]
public class RoadGenerator : MonoBehaviour
{
	private const float FLOAT_EQUALITY_TOLERANCE = 0.001f;

	private struct Intersection
	{
		public Vector3 Position;
		public Rectangle Rectangle;
		public Dictionary<Direction, bool> Directions;

		public Intersection(Vector3 pPosition, Rectangle pRectangle)
		{
			Position = pPosition;
			Rectangle = pRectangle;
			Directions = new Dictionary<Direction, bool>()
			{
				{Direction.North, false},
				{Direction.East, false},
				{Direction.South, false},
				{Direction.West, false}
			};
		}
	}

	[Serializable]
	private struct Road
	{
		public Vector3 Center;
		public int Size;
		public bool Horizontal;

		public Road(Vector3 pCenter, int pSize, bool pHorizontal)
		{
			Center = pCenter;
			Size = pSize;
			Horizontal = pHorizontal;
		}
	}

	[Serializable]
	private class RoadInfo
	{
		public Rectangle Rect;
		public Vector3 Center;
		public int Amount;
	}

	private enum Direction
	{
		North,
		East,
		South,
		West
	}

	[SerializeField] private Material material;
	[SerializeField] private Material crossWalkMaterial;
	[SerializeField] private Material intersectionMaterial;

	private List<Rectangle> rects = new();
	private List<Road> roads;

	public void Generate()
	{
		Clear();

		IReadOnlyDictionary<Rectangle, HashSet<Rectangle>> rectangleNeighbours = gameObject.GetComponent<LayoutGenerator>().RectanglesWithNeighbours;
		rects = rectangleNeighbours.Keys.ToList();

		List<Intersection> intersections = DetermineIntersections(rects);
		roads = DetermineRoads(intersections);
		MakeRoads(roads);
		MakeIntersections(intersections);
	}

	private void MakeRoads(List<Road> roadsToMake)
	{
		GameObject roadContainer = new GameObject($"Road Container");
		roadContainer.transform.parent = transform;
		roadContainer.transform.position = transform.position;

		int number = 1;
		foreach (Road road in roadsToMake)
		{
			GameObject roadObject = new GameObject($"Road {number}");
			roadObject.transform.localPosition = transform.position + new Vector3(road.Center.x, 0, road.Center.z);
			roadObject.transform.parent = roadContainer.transform;
			number++;

			if (road.Size == 0)
			{
				continue;
			}

			float hSize = road.Size / 2.0f;

			Vector3 add = new Vector3((road.Horizontal ? 1 : 0), 0, (road.Horizontal ? 0 : 1));

			Vector3 startPos = (-hSize * add) + (add * 0.5f);


			for (int i = 0; i < road.Size; i++)
			{
				GameObject roadPart = new GameObject($"RoadPart {i + 1}");
				roadPart.transform.parent = roadObject.transform;
				roadPart.transform.localPosition =
					startPos + (i * add);

				if (road.Horizontal)
				{
					roadPart.transform.Rotate(0, 90, 0);
				}

				Mesh mesh = new Mesh
				{
					vertices = new[]
					{
						new Vector3(-0.5f, 0, 0.5f), //TL
						new Vector3(-0.5f, 0, -0.5f), //BL
						new Vector3(0.5f, 0, -0.5f), //BR
						new Vector3(0.5f, 0, 0.5f), //TR
					},
					uv = new[]
					{
						new Vector2(0, 1), //TL
						new Vector2(0, 0), //BL
						new Vector2(1, 0), //BR
						new Vector2(1, 1), //TR
					},
					triangles = new[]
					{
						0, 3, 2, //TR
						2, 1, 0, //BL
					}
				};

				mesh.Optimize();
				mesh.RecalculateBounds();
				mesh.RecalculateNormals();
				mesh.RecalculateTangents();

				Material materialToUse = material;
				
				if (i == 0 || i == road.Size - 1)
				{
					materialToUse = crossWalkMaterial;
				}

				roadPart.AddComponent<MeshFilter>().mesh = mesh;
				roadPart.AddComponent<MeshRenderer>().material = materialToUse;
			}
		}
	}

	private void MakeIntersections(List<Intersection> intersections)
	{
		GameObject intersectionContainer = new GameObject($"Intersection Container");
		intersectionContainer.transform.parent = transform;
		intersectionContainer.transform.position = transform.position;

		for (int index = 0; index < intersections.Count; index++)
		{
			Intersection intersection = intersections[index];
			GameObject intersectionObject = new GameObject($"Intersection {index + 1}");
			intersectionObject.transform.localPosition = transform.position + new Vector3(intersection.Position.x, 0, intersection.Position.z);
			intersectionObject.transform.parent = intersectionContainer.transform;

			Mesh mesh = new Mesh
			{
				vertices = new[]
				{
					new Vector3(-0.5f, 0, 0.5f), //TL
					new Vector3(-0.5f, 0, -0.5f), //BL
					new Vector3(0.5f, 0, -0.5f), //BR
					new Vector3(0.5f, 0, 0.5f), //TR
				},
				uv = new[]
				{
					new Vector2(0, 1), //TL
					new Vector2(0, 0), //BL
					new Vector2(1, 0), //BR
					new Vector2(1, 1), //TR
				},
				triangles = new[]
				{
					0, 3, 2, //TR
					2, 1, 0, //BL
				}
			};

			mesh.Optimize();
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();

			intersectionObject.AddComponent<MeshFilter>().mesh = mesh;
			intersectionObject.AddComponent<MeshRenderer>().material = intersectionMaterial;
		}
	}

	private List<Road> DetermineRoads(List<Intersection> intersections)
	{
		HashSet<Vector3> centers = new HashSet<Vector3>();
		List<Road> roads = new List<Road>();

		foreach (Intersection intersection in intersections)
		{
			Dictionary<Direction, (int, float)> directionBest = new Dictionary<Direction, (int, float)>();

			for (int index = 0; index < intersections.Count; index++)
			{
				Vector3 pos = intersection.Position;

				Intersection otherIntersection = intersections[index];
				Vector3 otherPos = otherIntersection.Position;

				// North
				if (Math.Abs(pos.x - otherPos.x) < FLOAT_EQUALITY_TOLERANCE && pos.z < otherPos.z)
				{
					HandleDist(Direction.North);
				}

				// East
				if (pos.x < otherPos.x && Math.Abs(pos.z - otherPos.z) < FLOAT_EQUALITY_TOLERANCE)
				{
					HandleDist(Direction.East);
				}

				// South
				if (Math.Abs(pos.x - otherPos.x) < FLOAT_EQUALITY_TOLERANCE && pos.z > otherPos.z)
				{
					HandleDist(Direction.South);
				}

				// West
				if (pos.x > otherPos.x && Math.Abs(pos.z - otherPos.z) < FLOAT_EQUALITY_TOLERANCE)
				{
					HandleDist(Direction.West);
				}

				continue;

				void HandleDist(Direction direction)
				{
					float distance = Vector3.Distance(pos, otherPos);

					if (directionBest.TryGetValue(direction, out (int currentIndex, float currentDistance) info))
					{
						if (info.currentDistance > distance)
						{
							directionBest[direction] = (index, distance);
						}
					}
					else
					{
						directionBest.Add(direction, (index, distance));
					}
				}
			}

			foreach (var other in directionBest)
			{
				Intersection otherIntersection = intersections[other.Value.Item1];

				if (!intersection.Rectangle.Equals(otherIntersection.Rectangle))
				{
					if (!intersection.Rectangle.Overlaps(intersections[other.Value.Item1].Rectangle))
					{
						continue;
					}
				}

				intersection.Directions[other.Key] = true;
				Vector3 center = (intersection.Position + otherIntersection.Position) / 2.0f;
				if (centers.Add(center))
				{
					int size = (int) Math.Ceiling(other.Value.Item2 - 1);

					bool horizontal = other.Key is Direction.East or Direction.West;

					roads.Add(new Road(center, size, horizontal));
				}
			}
		}

		return roads;
	}

	public void Clear()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject);
		}
	}

	private List<Intersection> DetermineIntersections(List<Rectangle> rects)
	{
		HashSet<Intersection> intersections = new HashSet<Intersection>();

		// Make unique intersections
		foreach (Rectangle rect in rects)
		{
			intersections.Add(new Intersection(rect.TopLeft3, rect));
			intersections.Add(new Intersection(rect.TopRight3, rect));
			intersections.Add(new Intersection(rect.BotRight3, rect));
			intersections.Add(new Intersection(rect.BotLeft3, rect));
		}

		return intersections.ToList();
	}
}