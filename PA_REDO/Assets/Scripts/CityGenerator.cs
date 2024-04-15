using System.Collections.Generic;
using DataStructures;
using UnityEngine;

[RequireComponent(typeof(LayoutGenerator))]
[RequireComponent(typeof(RoadGenerator))]
[RequireComponent(typeof(SideWalkBuilder))]
public class CityGenerator : MonoBehaviour
{
    [SerializeField] private Material foundationMaterial;
    [SerializeField] private float foundationTileSize = 1.0f;

    private const float RoadWidth = 1.0f;
    
    public void FullGenerate()
    {
        // Maybe introduce a big scriptable object that contains all the settings so I can just pass that along here
        LayoutGenerator layoutGenerator = GetComponent<LayoutGenerator>();
        layoutGenerator.Generate();
        IReadOnlyDictionary<Rectangle, HashSet<Rectangle>> layout = layoutGenerator.RectanglesWithNeighbours;

        RoadGenerator roadGenerator = GetComponent<RoadGenerator>();
        roadGenerator.Generate();

        SideWalkBuilder sideWalkBuilder = GetComponent<SideWalkBuilder>();
        sideWalkBuilder.Build();

        float sideWalkHeight = sideWalkBuilder.SideWalkHeight;
        float sideWalkWidth = sideWalkBuilder.SideWalkWidth;

        GameObject foundationsContainer = new("FoundationsContainer");
        foundationsContainer.transform.position = transform.position;
        foundationsContainer.transform.parent = transform;

        int index = 0;
        foreach (Rectangle rect in layout.Keys)
        {
            GameObject foundationObject = new($"Foundation {index + 1}");
            foundationObject.transform.position = transform.position + rect.Center3 + new Vector3(0, sideWalkHeight, 0);
            foundationObject.transform.parent = foundationsContainer.transform;

            Mesh mesh = CreateFoundation(rect, sideWalkWidth);
            foundationObject.AddComponent<MeshFilter>().mesh = mesh;
            foundationObject.AddComponent<MeshRenderer>().material = foundationMaterial;
            index++;


            Rectangle foundationRectangle = new Rectangle()
            {
                X = rect.X,
                Z = rect.Z,
                Depth = rect.Depth - RoadWidth - (sideWalkWidth * 2),
                Width = rect.Width - RoadWidth - (sideWalkWidth * 2),
            };

        }
    }

    public void FullClear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private Mesh CreateFoundation(Rectangle rect, float sideWalkWidth)
    {
        float width = rect.Width - (sideWalkWidth * 2) - RoadWidth;
        float depth = rect.Depth - (sideWalkWidth * 2) - RoadWidth;

        float xAmount = width / foundationTileSize;
        float zAmount = depth / foundationTileSize;

        float hWidth = width / 2.0f;
        float hDepth = depth / 2.0f;

        Vector3 topLeft = new(-hWidth, 0, hDepth);

        List<Vector3> vertices = new();
        List<Vector2> uvs = new();
        List<int> triangles = new();

        for (int x = 0; x < xAmount; x++)
        {
            for (int z = 0; z < zAmount; z++)
            {
                int offset = vertices.Count;

                float xRemainder = xAmount - x;
                float zRemainder = zAmount - z;
                float xFactor = 1.0f;
                float zFactor = 1.0f;
                if (xRemainder is > 0 and < 1)
                {
                    xFactor = xRemainder;
                }

                if (zRemainder is > 0 and < 1)
                {
                    zFactor = zRemainder;
                }

                vertices.AddRange(new[]
                {
                    topLeft + new Vector3((x * foundationTileSize), 0, -(z * foundationTileSize)), //tl
                    topLeft + new Vector3((x * foundationTileSize) + (foundationTileSize * xFactor), 0,
                        -(z * foundationTileSize)), //tr
                    topLeft + new Vector3((x * foundationTileSize) + (foundationTileSize * xFactor), 0,
                        -(z * foundationTileSize) - (zFactor * foundationTileSize)), //br
                    topLeft + new Vector3((x * foundationTileSize), 0,
                        -(z * foundationTileSize) - (zFactor * foundationTileSize)), //bl
                });

                uvs.AddRange(new[]
                {
                    new Vector2(0, 1), //tl
                    new Vector2(1, 1), //tr
                    new Vector2(1, 0), //br
                    new Vector2(0, 0), //bl
                });

                triangles.AddRange(new[]
                {
                    0 + offset, 1 + offset, 2 + offset,
                    2 + offset, 3 + offset, 0 + offset,
                });
            }
        }

        Mesh mesh = new()
        {
            vertices = vertices.ToArray(),
            uv = uvs.ToArray(),
            triangles = triangles.ToArray(),
        };

        mesh.Optimize();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }
}