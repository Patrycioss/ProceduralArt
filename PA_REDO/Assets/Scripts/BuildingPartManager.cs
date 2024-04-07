using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class BuildingPartManager : MonoBehaviour
{
	public AddressableAssetGroup WallGroup => wallGroup;
	public AddressableAssetGroup WallDecoGroup => wallDecoGroup;
	
	[SerializeField] private AddressableAssetGroup wallGroup;
	[SerializeField] private AddressableAssetGroup wallDecoGroup;

	public static bool TryGetFromList(RNG rng, IReadOnlyList<GameObject> parts, out GameObject part)
	{
		if (parts.Count == 0)
		{
			Debug.LogWarning($"No wall parts available!");
			part = null;
			return false;
		}
		
		int index = rng.Int(0, parts.Count - 1);
		part = parts[index];
		return true;
	}
}