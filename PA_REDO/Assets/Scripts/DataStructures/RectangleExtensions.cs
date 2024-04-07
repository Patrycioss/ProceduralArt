using UnityEngine;

namespace DataStructures
{
	public static class RectangleExtensions
	{
		public static Color Color;
		
		public static void Draw(this Rectangle rectangle, bool drawLines = true, bool drawCorners = true)
		{
			Vector3 tL = ToVector3(rectangle.TopLeft);
			Vector3 tR = ToVector3(rectangle.TopRight);
			Vector3 bR = ToVector3(rectangle.BotRight);
			Vector3 bL = ToVector3(rectangle.BotLeft);
			
			if (drawLines)
			{
				Gizmos.color = Color;
				ShapeDrawer.DrawRectangle(tL,tR,bR,bL);
			}

			if (drawCorners)
			{
				Gizmos.color = Color;
				Gizmos.DrawSphere(tL, 4);
				Gizmos.DrawSphere(tR, 4);
				Gizmos.DrawSphere(bR, 4);
				Gizmos.DrawSphere(bL, 4);
			}
		}

		public static bool Overlaps(this Rectangle rect, Rectangle other)
		{
			bool xAxisOverlaps = 
				other.X >= rect.X && other.X <= rect.Right
			    || other.Right >= rect.X && other.Right <= rect.Right
			    || other.X <= rect.X && other.Right >= rect.Right;

			bool zAxisOverlaps =
				(other.Top >= rect.Top && other.Z <= rect.Z)
				|| (other.Top <= rect.Top && other.Top >= rect.Z)
				|| (other.Z <= rect.Top && other.Z >= rect.Z);

			return xAxisOverlaps && zAxisOverlaps;
		}

		public static Vector3 ToVector3(Vector2 v2)
		{
			return new Vector3(v2.x, 0, v2.y);
		}
	}
}