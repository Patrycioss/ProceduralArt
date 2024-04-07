using System;
using UnityEngine;

namespace BuildingParts
{
	[Serializable]
	public struct Wall
	{
		[HideInInspector]
		public string Type;
		public GameObject Object;
		public WallSection WallSection;
	}
}