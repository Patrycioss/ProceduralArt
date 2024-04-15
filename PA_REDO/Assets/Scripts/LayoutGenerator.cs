using System.Collections.Generic;
using DataStructures;
using UnityEngine;
using Color = UnityEngine.Color;
using Rectangle = DataStructures.Rectangle;

public class LayoutGenerator : MonoBehaviour
{
	public IReadOnlyDictionary<Rectangle, HashSet<Rectangle>> RectanglesWithNeighbours => rectanglesWithNeighbours;
	
	[Tooltip("Size in unity meters!")]
	[SerializeField] private Vector2Int size = new Vector2Int(100,100);

	[SerializeField] private RNG rng;
	[SerializeField] private int minSpaceWidth = 2;
	[SerializeField] private int minSpaceDepth = 2;

	private List<Rectangle> rectangles;
	private Dictionary<Rectangle, HashSet<Rectangle>> rectanglesWithNeighbours = new Dictionary<Rectangle, HashSet<Rectangle>>();
	
	public void Generate()
	{
		Clear();
		
		Rectangle rectangle = new Rectangle((int)(-size.x/2.0f),(int)(-size.x/2.0f),size.x, size.y);
		rectangles = BSP.Partition(rectangle, rng, minSpaceWidth, minSpaceDepth);

		rectanglesWithNeighbours = BSP.FindNeighbours(rectangles);
	}

	public void Clear()
	{
		rectanglesWithNeighbours.Clear();
	}

	private void OnDrawGizmos()
	{
		// Vector3 tPos = transform.position;
		//
		// foreach (Rectangle rectangle in rectanglesWithNeighbours.Keys)
		// {
		// 	RectangleExtensions.Color = Color.green;
		// 	RectangleExtensions.Radius = 0.1f;
		// 	rectangle.Draw(tPos);
		// }
	}
}