using UnityEngine;

namespace DataStructures
{
	/// <summary>
	/// Draw shapes using unity Gizmos.
	/// <remarks>Can only be called from Gizmo drawing functions!</remarks>
	/// </summary>
	public class ShapeDrawer
	{
		public static void DrawRectangle(Vector3 topLeft, Vector3 topRight, Vector3 bottomRight, Vector3 bottomLeft)
		{
			Gizmos.DrawLine(topLeft, topRight);
			Gizmos.DrawLine(topRight, bottomRight);
			Gizmos.DrawLine(bottomRight, bottomLeft);
			Gizmos.DrawLine(bottomLeft, topLeft);
		}
	}
}