using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BuildingParts
{
	public class WallSection : MonoBehaviour
	{
		public int WallPartCount { get; set; }
		public int DecoPartCount { get; set; }
		
		
		[SerializeField] private List<Wall> walls = new List<Wall>();
		[SerializeField] private List<GameObject> wallDecos = new List<GameObject>();

		[SerializeField] private BuildingPartManager buildingPartManager;
		[SerializeField] private RNG rng;

		public void InjectDependencies(BuildingPartManager buildingPartManager, RNG rng)
		{
			this.buildingPartManager = buildingPartManager;
			this.rng = rng;
		}

		public string[] GetWallOptions()
		{
			return buildingPartManager.WallGroup.entries.Select(entry => entry.ToString()).ToArray();
		}
		
		public void Generate(int wallAmount, int decoAmount)
		{
			Clear();
			Debug.Log($"Called WallSection.Generate() with wallAmount: {wallAmount} and decoAmount: {decoAmount}");

			Transform thisTransform = transform;
			Vector3 thisPos = thisTransform.position;

			
			WallPartCount = wallAmount;
			DecoPartCount = decoAmount;

			bool even = wallAmount % 2 == 0;
			int a = wallAmount / 2;
			float x0 = thisPos.x - a + (even ? 0.5f : 0f);
			
			for (int i = 0; i < wallAmount; i++)
			{
				ExtensionMethods.InstantiationDetails details = new()
				{
					Parent = transform,
					Position = new Vector3(x0 + 1 * i, thisPos.y, thisPos.z),
					Rotation = Quaternion.identity,
				};
				
				if (buildingPartManager.WallGroup.TryInstantiateRandomEntry(rng, details, out GameObject part, out string identifier))
				{
					int addIndex = walls.Count;
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
				Rotation = Quaternion.identity,
			};
				
			if (buildingPartManager.WallGroup.TryInstantiateEntryAtIndex(rng, partIdentifier, details, out GameObject part, out string identifier))
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
				Debug.LogError($"Can't set wallDeco when index is out of range! Amount: {wallDecos.Count} Index: {index}");
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

		private GameObject GetWallPart(int index)
		{
			return null;
		}

		private GameObject GetWallDeco(int index)
		{
			return null;
		}
	}
}