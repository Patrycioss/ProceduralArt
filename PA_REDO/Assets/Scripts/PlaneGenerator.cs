using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlaneGenerator based on this tutorial: https://youtu.be/-3ekimUWb9I
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlaneGenerator : MonoBehaviour
{
	[SerializeField] private Vector2 size;
	[SerializeField] private int resolution;

	private Mesh mesh;
	private MeshFilter meshFilter;
	private List<Vector3> vertices;
	private List<int> triangles;
	
	public void Generate()
	{
		mesh = new Mesh();
		meshFilter = GetComponent<MeshFilter>();
		vertices = new List<Vector3>();

		resolution = Mathf.Clamp(resolution, 1, 50);

		float xPerStep = size.x / resolution;
		float yPerStep = size.y / resolution;

		for (int y = 0; y < resolution + 1; y++)
		{
			for (int x = 0; x < resolution + 1; x++)
			{
				vertices.Add(new Vector3(x * xPerStep, 0, y * yPerStep));
			}
		}

		triangles = new List<int>();
		for (int row = 0; row < resolution; row++)
		{
			for (int column = 0; column < resolution; column++)
			{
				int i = (row * resolution) + row + column;

				triangles.Add(i);
				triangles.Add(i + resolution + 1);
				triangles.Add(i + resolution + 2);
				
				triangles.Add(i);
				triangles.Add(i + resolution + 2);
				triangles.Add(i + 1);
			}
		}

		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();

		meshFilter.mesh = mesh;
	}		
}