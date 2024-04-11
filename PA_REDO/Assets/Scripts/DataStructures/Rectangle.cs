using System;
using UnityEngine;

namespace DataStructures
{
	/// <summary>
	///  2 x 2 Rectangle example:
	/// 
	/// 0,1 ------- 1,1      ^
	///   |          |       |
	///   |     *    |       Z 
	///   |          |
	/// 0,0 ------- 1,0     X -->
	/// </summary>
	[Serializable]
	public struct Rectangle
	{
		public float X;
		public float Z;
		public float Width;
		public float Depth;
		public float Right => X + Width;
		public float Top => Z + Depth;
		public float HWidth => Width / 2;
		public float HDepth => Depth / 2;
		public Vector2 Center => new Vector2(X + HWidth,Z + HDepth);
		public Vector2 TopLeft => new Vector2(X, Z + Depth);
		public Vector2 TopRight => new Vector2(X + Width, Z + Depth);
		public Vector2 BotLeft => new Vector2(X, Z);
		public Vector2 BotRight => new Vector2(X + Width, Z);

		public Vector3 Center3 => new Vector3(X + HWidth, 0, Z + HDepth);
		public Vector3 TopLeft3 => new Vector3(X, 0, Z + Depth);
		public Vector3 TopRight3 => new Vector3(X + Width, 0, Z + Depth);
		public Vector3 BotLeft3 => new Vector3(X, 0, Z);
		public Vector3 BotRight3 => new Vector3(X + Width, 0, Z);
		

		public Rectangle(float x, float z, float width, float depth)
		{
			X = x;
			Z = z;
			Width = width;
			Depth = depth;
		}

		public override string ToString()
		{
			return $"X: {X}, Z: {Z}, Width: {Width}, Depth: {Depth}";
		}
	}
}