#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GenericBuildingGenerator : MonoBehaviour
{
	private enum Orientation
	{
		North,
		East,
		South,
		West
	}

	[SerializeField] private RNG? rng;
	[SerializeField] private MeshFilter? meshFilter;
	[SerializeField] private MeshRenderer? meshRenderer;

	// [SerializeField] private AssetLabelReference wallsReference;
	[SerializeField] private AddressableAssetGroup? wallGroup;
	[SerializeField] private AddressableAssetGroup? roofGroup;
	
	private Task<GameObject?> InstantiateRandomWall(Transform parent)
	{
		if (wallGroup != null && wallGroup.entries != null)
		{
			int index = rng!.Int(0, wallGroup.entries.Count - 1);

			AddressableAssetEntry addressableAssetEntry = wallGroup.entries.ElementAt(index);
			AsyncOperationHandle<GameObject?> handle = Addressables.InstantiateAsync(addressableAssetEntry.address, parent);
			return Task.FromResult(handle.Result);
		}

		return Task.FromResult<GameObject?>(null);
	}

	public async void Generate()
	{
		// var wallPrefabs = await LoadPrefabsFromGroup(wallGroup);
		// var roofPrefabs = await LoadPrefabsFromGroup(roofGroup);


		GameObject? haha = await InstantiateRandomWall(transform);
		if (haha != null)
		{
			haha.transform.localPosition = Vector3.zero;
			haha.transform.localRotation = Quaternion.identity;
		}
	}

	private void MakeBuilding(int[,] floorPlan)
	{
		// IResourceLocation resourceLocation = Addressables.
	}

	private GameObject MakeSection(Orientation[] orientations)
	{
		GameObject sectionObject = new GameObject("Section");

		List<GameObject> walls = new();

		for (int i = 0; i < orientations.Length; i++)
		{
			// Addressables.InstantiateAsync(wallsReference.)
			// wallsReference

			// walls.Add();
		}

		foreach (Orientation orientation in orientations)
		{
			switch (orientation)
			{
				case Orientation.North:

					break;
				case Orientation.East:
					break;
				case Orientation.South:
					break;
				case Orientation.West:
					break;
			}
		}

		return sectionObject;
	}

	public void Clear()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			Destroy(transform.GetChild(i));
		}
	}
}