using System.Collections.Generic;
using BuildingParts;
using UnityEngine;

public class GenericBuildingGenerator : MonoBehaviour
{
	[SerializeField] private List<Block> blocks;
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

				GameObject blockObject = new GameObject($"Block {blocks.Count + 1}");
				blockObject.transform.parent = transform;
				blockObject.transform.position = partPosition;

				Transform blockTransform = blockObject.transform;
				Block block = blockObject.AddComponent<Block>();
				blocks.Add(block);

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

					if (TryMakeSection(amount, height, skip, out WallSection section))
					{
						Transform sectionTransform = section.transform;
						sectionTransform.name = "Left";
						sectionTransform.parent = blockTransform;
						sectionTransform.position = partPosition + new Vector3(-0.5f, 0, 0);
						section.transform.Rotate(0,90,0);
						block.LeftWall = section;
					}
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

					if (TryMakeSection(amount, height, skip, out WallSection section))
					{
						Transform sectionTransform = section.transform;
						sectionTransform.name = "Right";
						sectionTransform.parent = blockTransform;
						sectionTransform.position = partPosition + new Vector3(0.5f, 0, 0);
						section.transform.Rotate(0,270,0);
						block.RightWall = section;
					}
				}

				// Front
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
					
					if (TryMakeSection(amount, height, skip, out WallSection section))
					{
						Transform sectionTransform = section.transform;
						sectionTransform.name = "Front";
						sectionTransform.parent = blockTransform;
						sectionTransform.position = partPosition + new Vector3(0, 0, -0.5f);
						block.FrontWall = section;
					}
				}

				// Back
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

					if (TryMakeSection(amount, height, skip, out WallSection section))
					{
						Transform sectionTransform = section.transform;
						sectionTransform.name = "Back";
						sectionTransform.parent = blockTransform;
						sectionTransform.position = partPosition + new Vector3(0, 0, 0.5f);
						section.transform.Rotate(0,180,0);
						block.BackWall = section;
					}
				}
				
			}
		}
	}

	private bool TryMakeSection(int amount, int height, int skip, out WallSection section)
	{
		if (height <= 0 || amount <= 0)
		{
			section = null;
			return false;
		}

		GameObject sectionObject = new GameObject("Block");
		WallSection wallSection = sectionObject.AddComponent<WallSection>();
		
		wallSection.InjectDependencies(buildingPartManager, rng);
		wallSection.Generate(amount, height, skip);
		section = wallSection;
		return true;
	}

	public void Clear()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject);
		}

		blocks.Clear();
	}
}