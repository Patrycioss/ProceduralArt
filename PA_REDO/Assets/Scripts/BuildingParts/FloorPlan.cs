using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingParts
{
	[Serializable]
	public struct FloorPlan
	{
		public int Size;
		public Row[] Plan;
	}

	[Serializable]
	public struct Row
	{
		public int[] Data;

		public Row(IEnumerable<int> pData)
		{
			Data = pData.ToArray();
		}
	}
}