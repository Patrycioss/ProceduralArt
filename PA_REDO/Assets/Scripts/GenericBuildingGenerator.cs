using System.Collections.Generic;
using BuildingParts;
using Unity.Mathematics;
using UnityEngine;

public class GenericBuildingGenerator : MonoBehaviour
{
	[SerializeField] private List<WallSection> wallSections;
	[SerializeField] private FloorPlan floorPlan;
	[SerializeField] private RNG rng;
	[SerializeField] private BuildingPartManager buildingPartManager;

	public void Generate()
	{
		Clear();
		Row[] plan = floorPlan.Plan;

		float center = floorPlan.Size / 2.0f - 0.5f;
		Vector3 centerPos = transform.position;

		for (int i = 0; i < plan.Length; i++)
		{
			int[] data = floorPlan.Plan[i].Data;

			for (int j = 0; j < data.Length; j++)
			{
				int height = data[j];

				if (height == 0)
				{
					continue;
				}

				float distX = j - center;
				float distZ = i - center;

				Vector3 partPosition = centerPos + new Vector3(distX, 0, distZ);

				// Left
				{
					int jIndex = j - 1;
					int amount = height;
					int skip = 0;
					if (jIndex >= 0)
					{
						int other = data[jIndex];
						amount = height - other;

						if (amount > 0)
						{
							skip = other;
						}
					}

					Vector3 pos = partPosition + new Vector3(-0.5f, 0, 0);
					if (TryMakeSection(pos, 90, amount, height, skip, out WallSection wallSection))
					{
						
					};
				}

				// Right
				{
					int jIndex = j + 1;
					int amount = height;
					int skip = 0;
					if (jIndex < data.Length)
					{
						int other = data[jIndex];
						amount = height - other;
						
						if (amount > 0)
						{
							skip = other;
						}
					}

					Vector3 pos = partPosition + new Vector3(0.5f, 0, 0);
					if (TryMakeSection(pos, 270, amount, height, skip, out WallSection wallSection))
					{
						
					};
				}

				// Back
				{
					int iIndex = i - 1;
					int amount = height;
					int skip = 0;
					if (iIndex >= 0)
					{
						int other = plan[iIndex].Data[j];
						amount = height - other;
						
						if (amount > 0)
						{
							skip = other;
						}
					}
					
					Vector3 pos = partPosition + new Vector3(0, 0, -0.5f);
					if (TryMakeSection(pos, 0, amount, height, skip, out WallSection section))
					{
						
					};
				}

				// Front
				{
					int iIndex = i + 1;
					int amount = height;
					int skip = 0;
					if (iIndex < plan.Length)
					{
						int other = plan[iIndex].Data[j];
						amount = height - other;
						
						if (amount > 0)
						{
							skip = other;
						}
					}

					Vector3 pos = partPosition + new Vector3(0, 0, 0.5f);
					if (TryMakeSection(pos, 180, amount, height, skip, out WallSection section))
					{
						
					}
				}
			}
		}
	}

	private bool TryMakeSection(Vector3 position, float rotation, int amount, int height, int skip, out WallSection section)
	{
		if (height <= 0 || amount <= 0)
		{
			section = null;
			return false;
		}

		GameObject sectionObject = new GameObject($"WallSection {wallSections.Count + 1}")
		{
			transform =
			{
				parent = transform,
				position = position,
			}
		};
		
		sectionObject.transform.Rotate(0,rotation,0);
		
		WallSection wallSection = sectionObject.AddComponent<WallSection>();
		wallSection.InjectDependencies(buildingPartManager, rng);
		wallSection.Generate(amount, height, skip);
		section = wallSection;
		wallSections.Add(section);
		return true;
	}

	public void Clear()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject);
		}

		wallSections.Clear();
	}
}