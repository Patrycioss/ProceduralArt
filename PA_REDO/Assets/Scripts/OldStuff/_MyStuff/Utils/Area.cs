using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts._MyStuff.CityLayout
{
	[Serializable]
	public class Area
	{
		public float x;
		public float z;

		private float _width;

		public float width
		{
			get => _width;
			set
			{
				_width = value;
				halfWidth = width / 2;
			}
		}

		private float _height;
		public float height
		{
			get => _height;
			set
			{
				_height = value;
				halfHeight = _height / 2;
			}
		}

		public float halfWidth { get; private set; }
		public float halfHeight { get; private set; }
		
		public Vector3 topLeft => new(x - halfWidth, 0, z + halfHeight);
		public Vector3 topRight => new(x + halfWidth, 0, z + halfHeight);
		public Vector3 bottomLeft => new(x - halfWidth, 0, z - halfHeight);
		public Vector3 bottomRight => new(x + halfWidth, 0, z - halfHeight);

		public Vector3 max => topRight;
		public Vector3 min => bottomLeft;
		
		public Vector3 position => new(x, 0, z);
		public Vector3 size => new(width, 0, height);

		public void Draw()
		{
			Gizmos.DrawLine(topLeft, topRight);
			Gizmos.DrawLine(topRight, bottomRight);
			Gizmos.DrawLine(bottomRight, bottomLeft);
			Gizmos.DrawLine(bottomLeft, topLeft);
		}
	}
}