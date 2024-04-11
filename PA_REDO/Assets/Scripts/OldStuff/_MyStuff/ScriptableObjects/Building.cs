using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts._MyStuff
{
	[CreateAssetMenu(fileName = "Building", menuName = "Building", order = 0)]
	public class Building : ScriptableObject
	{
		[Tooltip("If false uses maxFloorCount to set amount")]
		[SerializeField] private bool _randomFloorCount = true;
		[SerializeField] private int _maxFloorCount;
		[SerializeField] private int _minFloorCount;

		[SerializeField] private List<GameObject> _bottomPrefabs;
		[SerializeField] private List<GameObject> _floorPrefabs;
		[SerializeField] private List<GameObject> _roofPrefabs;

		public bool randomFloorCount => _randomFloorCount;
		public int maxFloorCount => _maxFloorCount;
		public int minFloorCount => _minFloorCount;
		public List<GameObject> bottomPrefabs => _bottomPrefabs;
		public List<GameObject> floorPrefabs => _floorPrefabs;
		public List<GameObject> roofPrefabs => _roofPrefabs;
	}
}