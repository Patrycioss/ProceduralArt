using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BuildingParts
{
	public class WallSection : MonoBehaviour
	{
		public int WallPartCount { get; set; }
		public int Height { get; set; }
		public int Skip { get; set; }
		
		[SerializeField] private List<Wall> walls = new List<Wall>();
		[SerializeField] private List<GameObject> wallDecos = new List<GameObject>();

		private BuildingPartManager buildingPartManager;
		private RNG rng;

		public void InjectDependencies(BuildingPartManager buildingPartManager, RNG rng)
		{
			this.buildingPartManager = buildingPartManager;
			this.rng = rng;
		}

		public IEnumerable<string> GetWallOptions()
		{
			return buildingPartManager.WallGroup.entries.Select(entry => entry.ToString()).ToArray();
		}

		public void Generate(int wallAmount, int height, int skip)
		{
			Clear();
			// Debug.Log($"Called WallSection.Generate() with wallAmount: {wallAmount} and height: {height}");

			Transform thisTransform = transform;
			Vector3 thisPos = thisTransform.position;

			WallPartCount = wallAmount;
			Height = height;
			Skip = skip;

			for (int i = 0; i < wallAmount; i++)
			{
				ExtensionMethods.InstantiationDetails details = new()
				{
					Parent = transform,
					// Position = new Vector3(thisPos.x, y0 - i, thisPos.z),
					Position = new Vector3(thisPos.x,  i + 0.5f + skip, thisPos.z),
					Rotation = thisTransform.rotation,
				};

				if (buildingPartManager.WallGroup.TryInstantiateRandomEntry(rng, details, out GameObject part,
					    out string identifier))
				{
					walls.Add(new Wall
					{
						Type = identifier,
						Object = part,
						WallSection = this,
					});
				}
			}
		}

		public void CombineMeshes()
		{
			throw new NotImplementedException();
		}


		public void SetWall(Wall wall, string partIdentifier)
		{
			int index = -1;

			for (int i = 0; i < walls.Count; i++)
			{
				Wall storedWall = walls[i];
				if (storedWall.Object == wall.Object)
				{
					index = i;
				}
			}

			if (index == -1)
			{
				Debug.LogWarning($"Wall does not exist in the internal list!");
				return;
			}

			if (string.IsNullOrEmpty(partIdentifier))
			{
				Debug.LogWarning($"Part identifier is null or empty!");
				return;
			}

			ExtensionMethods.InstantiationDetails details = new()
			{
				Parent = transform,
				Position = wall.Object.transform.position,
				Rotation = transform.rotation,
			};

			if (buildingPartManager.WallGroup.TryInstantiateEntryAtIndex(partIdentifier, details,
				    out GameObject part, out string identifier))
			{
				Wall oldWall = walls[index];

				walls[index] = new Wall
				{
					Type = identifier,
					Object = part,
					WallSection = this,
				};

				DestroyImmediate(oldWall.Object);
			}
		}

		public void SetWallDeco(int index, int partIndex)
		{
			if (index > wallDecos.Count)
			{
				Debug.LogError(
					$"Can't set wallDeco when index is out of range! Amount: {wallDecos.Count} Index: {index}");
				return;
			}
		}

		public void Clear()
		{
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				DestroyImmediate(transform.GetChild(i).gameObject);
			}

			walls.Clear();
			wallDecos.Clear();
		}
	}
}