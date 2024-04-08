using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class ExtensionMethods
{
	public static bool TryGetRandomGameObject(this IReadOnlyList<GameObject> entries, RNG rng, out GameObject gameObject)
	{
		if (entries.Count == 0)
		{
			Debug.LogWarning($"No GameObjects available!");
			gameObject = null;
			return false;
		}
		
		int index = rng.Int(0, entries.Count - 1);
		gameObject = entries[index];
		return true;
	}

	public struct InstantiationDetails
	{
		public Transform Parent;
		public Quaternion Rotation;
		public Vector3 Position;
	}

	public static bool TryInstantiateRandomEntry(this AddressableAssetGroup group, RNG rng, InstantiationDetails details, out GameObject gameObject, out string identifier)
	{
		if (group.entries.Count == 0)
		{
			Debug.LogWarning($"No entries available!");
			gameObject = null;
			identifier = null;
			return false;
		}
		
		int index = rng.Int(0, group.entries.Count - 1);
		AddressableAssetEntry entry = group.entries.ElementAt(index);
		
		AsyncOperationHandle<GameObject> handle =
			Addressables.InstantiateAsync(entry.address, details.Position, details.Rotation, details.Parent);
		gameObject = handle.Result;
		identifier = entry.ToString();
		return true;
	}
	
	public static bool TryInstantiateEntryAtIndex(this AddressableAssetGroup group, string newIdentifier, InstantiationDetails details, out GameObject gameObject, out string identifier)
	{
		if (string.IsNullOrEmpty(newIdentifier))
		{
			Debug.LogWarning($"Identifier is empty or null!");
			gameObject = null;
			identifier = null;
			return false;
		}

		AddressableAssetEntry entry = group.entries.FirstOrDefault(entry => entry.ToString() == newIdentifier);
		
		if (entry == null)
		{
			Debug.LogWarning($"Can't find entry with identifier: {newIdentifier}");
			gameObject = null;
			identifier = null;
			return false;
		}
		
		
		AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(entry.address, details.Position, details.Rotation, details.Parent);
		gameObject = handle.Result;
		identifier = entry.ToString();
		return true;
	}
}