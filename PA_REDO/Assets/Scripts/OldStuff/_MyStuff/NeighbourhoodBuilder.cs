using System.Collections.Generic;
using _Scripts._MyStuff;
using _Scripts._MyStuff.CityLayout;
using _Scripts.OldStuff._MyStuff;
using _Scripts.SchoolMeshGeneration;
using UnityEngine;

namespace OldStuff._MyStuff
{
	[RequireComponent(typeof(_Scripts._MyStuff.RNG))]
	public class NeighbourhoodBuilder : MonoBehaviour
	{
		[SerializeField] private SaveData _saveData;
		[SerializeField] private IntersectionMaker _intersectionMaker;
		[SerializeField] private Material _foundationMaterial;

		[SerializeField] private float _wallWidth = 1.0f;
		[SerializeField] private float _houseScale = 1;
		[SerializeField] private int _houseWidth = 2;

		[SerializeField] private GameObject _proceduralBuilding;

		private RNG _RNG;

		private List<Node> _intersections = new();
		private List<Area> _buildingAreas = new();

		private Dictionary<Area, GameObject> _neighbourHoods = new();

		private Dictionary<Area, List<Area>> _housePositions = new();

		private Vector3 _houseSize;
		

		private float _hIwidth => _intersectionMaker.hIntersectionWidth;

		private void OnEnable()
		{
			_RNG = GetComponent<RNG>();
		}

		private void OnValidate()
		{
			_RNG = GetComponent<RNG>();
		}

		public void Clear()
		{
			_buildingAreas.Clear();
			DeleteChild("Neighbourhoods");
		}

		public void Load()
		{
			if (_saveData != null)
			{
				_intersections = new(_saveData.data.intersections);
			}
			else Debug.LogError("No SaveData Selected");
			Debug.Log($"Loaded {_intersections.Count} intersections");
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

		public void Generate()
		{
			DeleteChild("Neighbourhoods");
			_buildingAreas = new();
			_neighbourHoods = new();
			_housePositions = new();

	        Load();
	        GenerateAreas();
	        MakeFoundations();
	        
	        _houseSize = new Vector3(_wallWidth * _houseWidth, 0, _wallWidth * _houseWidth) * _houseScale;

	        
	        foreach (Area area in _buildingAreas) 
		        DetermineHousePositions(area);

	        foreach (Area area in _buildingAreas)
	        {
		        if (_housePositions.ContainsKey(area))
		        {
			        int index = 0;
			        foreach (Area pos in _housePositions[area])
			        {
				        GameObject house = Instantiate(_proceduralBuilding, pos.position, Quaternion.identity);
				        house.name = $"House {index}";
				        house.transform.parent = _neighbourHoods[area].transform;
				        HouseBuilder houseBuilder = house.GetComponent<HouseBuilder>();
				        houseBuilder.SetRNG(_RNG);
				        houseBuilder.Build();
				        house.transform.localScale = new(_houseScale, _houseScale, _houseScale);

				        index++;
			        }
		        }
		        else Debug.LogWarning($"Area at position {area.position} with index {_buildingAreas.IndexOf(area)} doesn't have positions");
	        }
		}

		private void DetermineHousePositions(Area pArea)
		{

			List<Area> determined = new();

			SplitArea(pArea);

			void SplitArea(Area s)
			{
				float split;
				if (s.width <= _houseSize.x * 2 && s.height <= _houseSize.z * 2)
				{
					determined.Add(s);
				}
				else if (s.width > s.height || s.width == s.height)
				{
					split = _RNG.Float(s.min.x + _houseSize.x, s.max.x - _houseSize.x);

					Area a = new()
					{
						x = split - ((split - s.min.x) / 2.0f),
						z = s.z,
						width = split - s.min.x,
						height = s.height,
					};

					Area b = new()
					{
						x = split + (s.max.x - split) / 2.0f,
						z = s.z,
						width = s.max.x - split,
						height = s.height,
					};
					
					SplitArea(a);
					SplitArea(b);
				}
				else
				{
					split = _RNG.Float(s.min.z + _houseSize.z, s.max.z - _houseSize.z);
					
					Area a = new()
					{
						x = s.x,
						z = split - ((split - s.min.z) / 2.0f),
						width = s.width,
						height = split - s.min.z,
					};

					Area b = new()
					{
						x = s.x,
						z = split + (s.max.z - split) / 2.0f,
						width = s.width,
						height = s.max.z - split,
					};
					
					SplitArea(a);
					SplitArea(b);
				}
			}

			_housePositions.Add(pArea, new());

			foreach (Area d in determined)
			{
				_housePositions[pArea].Add(d);
			}
		}

		private void MakeFoundations()
		{
			GameObject foundationContainer = new GameObject("Neighbourhoods");
			foundationContainer.transform.parent = this.transform;
			foundationContainer.transform.localPosition = Vector3.zero;

			MeshBuilder foundationBuilder = new();
			for (int index = 0; index < _buildingAreas.Count; index++)
			{
				Area area = _buildingAreas[index];
				GameObject foundation = new("Neighbourhood " + index);
				foundation.transform.position = area.position + new Vector3(0,_intersectionMaker.sideWalkHeight,0);
				foundation.transform.parent = foundationContainer.transform;
				_neighbourHoods.Add(area,foundation);

				int a = foundationBuilder.AddVertex(new(-area.width/2,0,area.height/2), new(0,0));
				int b = foundationBuilder.AddVertex(new(area.width/2,0,area.height/2), new(1,0));
				int c = foundationBuilder.AddVertex(new(area.width / 2, 0, -area.height / 2), new(1,1));
				int d = foundationBuilder.AddVertex(new(-area.width / 2, 0, -area.height / 2),new(0,1));
				
				foundationBuilder.AddTriangle(a,b,c);
				foundationBuilder.AddTriangle(c,d,a);

				foundation.AddComponent<MeshRenderer>().material = _foundationMaterial;
				foundation.AddComponent<MeshFilter>().mesh = foundationBuilder.CreateMesh();
				
				foundationBuilder.Clear();
			}
		}

		private void GenerateAreas()
		{
			for (int index = 0; index < _intersections.Count; index++)
			{
				Node intersection = _intersections[index];

				Node c = intersection.connections[Node.Direction.East];
				if (c == null) continue;

				Node a = intersection.connections[Node.Direction.North];
				if (a != null)
				{
					Node b = a.connections[Node.Direction.East];
					while (b == null && a.connections[Node.Direction.North] != null)
					{
						a = a.connections[Node.Direction.North];
						b = a.connections[Node.Direction.East];
					}

					if (b != null)
					{
						while (b != null && b.connections[Node.Direction.South] != c)
						{
							b = b.connections[Node.Direction.East];
						}

						if (b == null) continue;
						
						float width = (b.ToVector3().x - _hIwidth) - (a.ToVector3().x + _hIwidth);
						float height = (a.ToVector3().z - _hIwidth) - (intersection.ToVector3().z + _hIwidth);

						Vector3 position = intersection.ToVector3() + transform.position + new Vector3(_hIwidth, 0, _hIwidth) +
						                   new Vector3(width / 2, 0, height / 2);
						Area area = new()
						{
							width = width,
							height = height,
							x = position.x,
							z = position.z
						};
						_buildingAreas.Add(area);
					}
				}
			}
		}

		private void OnDrawGizmos()
		{
			foreach (Area area in _buildingAreas)
			{
				Gizmos.color = Color.red;
				area.Draw();
				
				Gizmos.color = Color.magenta;
				Gizmos.DrawSphere(area.position, 0.5f);

				
				Gizmos.color = Color.green;
				if (_housePositions.ContainsKey(area))
				{
					foreach (Area h in _housePositions[area])
					{
						h.Draw();
					}
				}
			}
		}
	}
}