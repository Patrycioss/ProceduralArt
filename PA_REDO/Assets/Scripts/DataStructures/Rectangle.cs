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
	public struct Rectangle
	{
		public int X;
		public int Z;
		public int Width;
		public int Depth;
		public int Right => X + Width;
		public int Top => Z + Depth;
		public int HWidth => Width / 2;
		public int HDepth => Depth / 2;
		public Vector2 Center => new Vector2(X + HWidth,Z + HDepth);
		public Vector2 TopLeft => new Vector2(X, Z + Depth);
		public Vector2 TopRight => new Vector2(X + Width, Z + Depth);
		public Vector2 BotLeft => new Vector2(X, Z);
		public Vector2 BotRight => new Vector2(X + Width, Z);

		public Rectangle(int x, int z, int width, int depth)
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