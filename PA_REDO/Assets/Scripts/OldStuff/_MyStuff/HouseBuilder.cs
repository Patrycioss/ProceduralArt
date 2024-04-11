using System.Collections.Generic;
using _Scripts._MyStuff;
using UnityEngine;

namespace _Scripts.OldStuff._MyStuff
{
	public class HouseBuilder : MonoBehaviour
	{
		private RNG _RNG;
		
		[SerializeField] private List<Building> _buildings;
		private Building _chosenBuilding;

		public void SetRNG(RNG pRNG)
		{
			_RNG = pRNG;
		}
		
		public void Build()
		{
			while (transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);
			if (_RNG == null)
			{
				_RNG = gameObject.GetComponent<RNG>();

				if (_RNG == null) _RNG = gameObject.AddComponent<RNG>();
			}

			_chosenBuilding = _buildings[_RNG.Int(0, _buildings.Count-1)];

			GameObject bottom = Instantiate(_chosenBuilding.bottomPrefabs[_RNG.Int(0, _chosenBuilding.bottomPrefabs.Count-1)], Vector3.zero, Quaternion.identity);
			bottom.name = "Bottom";
			bottom.transform.parent = this.transform;
			bottom.transform.localPosition = Vector3.zero;

			int floorCount;
			if (_chosenBuilding.randomFloorCount)
				floorCount = _RNG.Int(_chosenBuilding.minFloorCount, _chosenBuilding.maxFloorCount);
			else floorCount = _chosenBuilding.minFloorCount;

			for (int i = 0; i < floorCount; i++)
			{
				float y = 1 + i;
				GameObject floor =
					Instantiate(
						_chosenBuilding.floorPrefabs[
							_RNG.Int(0, _chosenBuilding.floorPrefabs.Count-1)],
						Vector3.zero, Quaternion.identity);
				floor.name = "Floor " + i + 1;			
				floor.transform.parent = this.transform;
				floor.transform.localPosition = new Vector3(0, y, 0);

			}

			GameObject roof =
				Instantiate(_chosenBuilding.roofPrefabs[_RNG.Int(0, _chosenBuilding.roofPrefabs.Count-1)],
					Vector3.zero, Quaternion.identity);

			roof.name = "Roof";
			roof.transform.parent = transform;
			roof.transform.localPosition = new Vector3(0, floorCount + 1, 0);
		}
		
	}
}