using System.Collections.Generic;
using _Scripts._MyStuff;
using _Scripts._MyStuff.CityLayout;
using UnityEngine;
using UnityEngine.Serialization;

namespace OldStuff._MyStuff
{
	[RequireComponent(typeof(_Scripts._MyStuff.RNG))]
	public class CityLayoutGenerator : MonoBehaviour
	{
		[FormerlySerializedAs("_saveData")] [SerializeField] private SaveData saveData;
		
		[FormerlySerializedAs("_horizontal")] [SerializeField] private int horizontal = 5;
		[FormerlySerializedAs("_vertical")] [SerializeField] private int vertical = 5;

		[FormerlySerializedAs("_minStepSize")] [SerializeField] private float minStepSize = 1.0f;
		[FormerlySerializedAs("_maxStepSize")] [SerializeField] private float maxStepSize = 3.0f;
		
		[FormerlySerializedAs("_showIntersections")] [SerializeField] private bool showIntersections;
		
		private List<Node> intersections = new();
		private _Scripts._MyStuff.RNG rng;
		
		public void DestroyChildren()
		{
			while (transform.childCount > 0) 
				DestroyImmediate(transform.GetChild(0).gameObject);
		}
		
		public void Generate()
		{
			if (rng == null)
			{
				rng = GetComponent<_Scripts._MyStuff.RNG>();
				if (rng == null) Debug.LogError("This should not be possible");
			}
			
			rng.UpdateRandToSeed();
			Clear();
			
			Node[] firstRow = new Node[horizontal];

			float cumulativeX = 0;
			
			for (int i = 0; i < horizontal; i++)
			{
				float step = rng.Float(minStepSize, maxStepSize);
				cumulativeX += step;
				
				firstRow[i] = new Node()
				{
					x = cumulativeX,
					z = 0,
				};
				intersections.Add(firstRow[i]);
			}
			
			float cumulativeZ = 0;
			
			for (int i = 0; i < vertical; i++)
			{
				float step = rng.Float(minStepSize, maxStepSize);
				cumulativeZ += step;
				
				for (int f = 0; f < firstRow.Length; f++)
				{
					Node newNode = new()
					{
						x = firstRow[f].x,
						z = cumulativeZ,
					};
					intersections.Add(newNode);
				}
			}
			Debug.Log($"Generated {intersections.Count} intersections");
			
			MakeConnections();
		}
		

		public void Save()
		{
			if (saveData != null) saveData.data.intersections = new List<Node>(intersections);
			else Debug.LogError("No SaveData Selected");
			Debug.Log("Intersections saved");
		}

		public void Load()
		{
			if (saveData != null)
			{
				Clear();
				intersections = new List<Node>(saveData.data.intersections);
			}
			else Debug.LogError("No SaveData Selected");
			Debug.Log($"Loaded {intersections.Count} intersections");
		}
		
		public void Clear()
		{
			intersections.Clear();
			
		}
		
		public void DeleteChild(string name)
		{
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				if (transform.GetChild(i).name == name)
				{
					DestroyImmediate(transform.GetChild(i).gameObject);
					return;
				}
			}
		}
		
		private void MakeConnections()
		{
			foreach (Node intersection in intersections)
			{
				List<Connection> newConnections = new List<Connection>();

				foreach (Node.Direction direction in intersection.connections.Keys)
				{
					if (intersection.connections[direction] != null) continue;
					Connection? newConnection = MakeConnectionForDirection(intersection, direction);
					if (newConnection == null) continue;
					newConnections.Add((Connection) newConnection);
				}

				
				foreach (Connection connection in newConnections)
				{
					connection.InformNodes();
				}
			}
		}

		
		private Connection? MakeConnectionForDirection(Node pIntersection, Node.Direction pDirection)
        {
            if (pIntersection.connections[pDirection] == null)
            {
                Node bestConnection = null;
                float smallestDistance = float.MaxValue;
                
                    
                foreach (Node other in intersections)
                {
                    if (pIntersection == other) continue;
                    if (other.connections[Node.GetOppositeDirection(pDirection)] != null) continue;
                    
                    if (Node.VectorToExactDirection(other.ToVector3() - pIntersection.ToVector3()) != pDirection) continue;

                    float distance = Vector3.Distance(other.ToVector3(), other.ToVector3());
                    if (distance < smallestDistance)
                    {
                        bestConnection = other;
                        smallestDistance = distance;
                    }
                }

                if (bestConnection == null) return null;
                Connection newConnection = new()
                {
                    a = pIntersection,
                    b = bestConnection,
                    aDirection = pDirection,
                    bDirection = Node.GetOppositeDirection(pDirection)
                };
                return newConnection;
            }
            return null;
        }

		private void OnDrawGizmos()
		{
			if (showIntersections)
			{
				foreach (Node intersection in intersections)
				{
					foreach (KeyValuePair<Node.Direction, Node> pair in intersection.connections)
					{
						Node.Direction direction = pair.Key;
						Node node = pair.Value;

						if (node == null) Gizmos.color = Color.red;
						else
						{
							Gizmos.color = Color.blue;
							Gizmos.DrawLine(intersection.ToVector3() + this.transform.position, intersection.connections[direction].ToVector3() + this.transform.position);
							Gizmos.color = Color.green;
						} 
						Gizmos.DrawSphere(intersection.ToVector3() + this.transform.position +Node.DirectionToVector(direction), 0.2f);
					}
				}
			}
		}
	}
}