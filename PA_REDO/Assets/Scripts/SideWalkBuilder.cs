using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures;
using UnityEngine;
using UnityEngine.Rendering;

public class SideWalkBuilder : MonoBehaviour
{
    public float SideWalkHeight => sideWalkHeight;
    public float SideWalkWidth => sideWalkWidth;
    
    [SerializeField] private float sideWalkHeight = 0.2f;
    [SerializeField] private float sideWalkWidth = 1.0f;
    [SerializeField] private Material material;

    private GameObject currentSideWalkObject;

    private class MeshData
    {
        public readonly List<Vector3> Vertices = new();
        public readonly List<Vector2> UVs = new();
        public readonly List<int> Triangles = new();

        public void AddMeshData(MeshData meshData)
        {
            int offset = Vertices.Count;
            Vertices.AddRange(meshData.Vertices);
            UVs.AddRange(meshData.UVs);
            Triangles.AddRange(meshData.Triangles.Select(triangle => triangle + offset).ToList());
        }

        public void RotateAround(Vector3 point, Vector3 rotDegrees)
        {
            for (int index = 0; index < Vertices.Count; index++)
            {
                // Vertex
                {
                    Vector3 vertex = Vertices[index];
                    Vector3 direction = vertex - point;
                    direction = Quaternion.Euler(rotDegrees) * direction;
                    Vertices[index] = direction + point;
                }

                // // UV
                // {
                // 	Vector2 uv = UVs[index];
                // 	Vector3 direction = new Vector3(uv.x, 0, uv.y) - point;
                // 	direction = Quaternion.Euler(rotDegrees) * direction;
                // 	Vector3 result = direction + point;
                // 	UVs[index] = new Vector2(result.x, result.z);
                // }
            }
        }
    }

    private MeshData NewMakeSide(Vector3 _centerPosition, int _size, float _sideWalkSize, float _sideWalkHeight,
        bool _makeSides)
    {
        MeshData meshData = new();
        float amount = _size / 2.0f;
        Vector3 startPos = _centerPosition - new Vector3(amount * _sideWalkSize, 0, 0);

        for (int i = 0; i < _size; i++)
        {
            int offset = meshData.Vertices.Count;
            Vector3 itl = new Vector3(startPos.x + i * _sideWalkSize, startPos.y, startPos.z);

            meshData.Vertices.AddRange(new[]
            {
                itl + new Vector3(0, _sideWalkHeight, 0), //ttl 
                itl + new Vector3(_sideWalkSize, _sideWalkHeight, 0), //ttr
                itl + new Vector3(_sideWalkSize, _sideWalkHeight, -_sideWalkSize), //tbr
                itl + new Vector3(0, _sideWalkHeight, -_sideWalkSize), //tbl
                itl + new Vector3(sideWalkWidth, 0, 0), //fbl
                itl, //fbr
            });

            meshData.UVs.AddRange(new[]
            {
                new Vector2(0, 1), // ttl
                new Vector2(1, 1), // ttr
                new Vector2(1, 0), // tbr
                new Vector2(0, 0), // tbl
                new Vector2(1, 0), // fbl
                new Vector2(0, 0), // fbr
            });

            meshData.Triangles.AddRange(new[]
            {
                0 + offset, 1 + offset, 2 + offset,
                2 + offset, 3 + offset, 0 + offset,
                1 + offset, 0 + offset, 4 + offset,
                0 + offset, 5 + offset, 4 + offset,
            });
        }

        if (_makeSides)
        {
            Vector3 offsetPos = startPos + new Vector3(_sideWalkSize * _size, 0, 0);
            int newOffset = meshData.Vertices.Count;
            
            meshData.Vertices.AddRange(new[]
            {
                // ltl, ltr, lbl, lbr
                startPos + new Vector3(0,_sideWalkHeight,0), //ltl
                startPos + new Vector3(0, _sideWalkHeight, -_sideWalkSize), //ltr
                startPos + new Vector3(0, 0, -_sideWalkSize), //lbr
                startPos, //lbl
                
                // rtl, rbr, rbl, rbr
                offsetPos + new Vector3(0, _sideWalkHeight, -_sideWalkSize), //rtl
                offsetPos + new Vector3(0, _sideWalkHeight, 0), //rtr
                offsetPos, //rbr
                offsetPos + new Vector3(0, 0, -_sideWalkSize), //rbl
            });
                
            meshData.UVs.AddRange(new []
            {
                new Vector2(0,0), //ltl
                new Vector2(1,0), //ltr
                new Vector2(1,1), //lbr
                new Vector2(0,1), //lbl
                
                new Vector2(0,0), //rtl
                new Vector2(1,0), //rtr
                new Vector2(1,1), //rbr
                new Vector2(0,1), //rbl
            });
            
            meshData.Triangles.AddRange(new[]
            {
                //left
                0 + newOffset, 1 + newOffset, 2 + newOffset,
                2 + newOffset, 3 + newOffset, 0 + newOffset,
                    
                // //right
                4 + newOffset, 5 + newOffset, 6 + newOffset,
                6 + newOffset, 7 + newOffset, 4 + newOffset,
            });
        }

        return meshData;
    }

    public void Build()
    {
        Vector3 tPosition = transform.position;

        IReadOnlyDictionary<Rectangle, HashSet<Rectangle>> rectangleNeighbours =
            gameObject.GetComponent<LayoutGenerator>().RectanglesWithNeighbours;

        GameObject sideWalk = new GameObject("SideWalk");
        sideWalk.transform.parent = transform;
        sideWalk.transform.position = tPosition;

        Vector3 sideWalkPosition = sideWalk.transform.position;


        // Construct Mesh
        Mesh mesh = new();
        mesh.indexFormat = IndexFormat.UInt32;
        MeshData meshData = new();


        foreach (Rectangle rect in rectangleNeighbours.Keys)
        {
            // TOP
            {
                // Vector3 direction = new Vector3(1, 0, -1);
                Vector3 topLeft = rect.TopLeft3 + new Vector3(0.5f, 0, -0.5f);
                Vector3 topRight = rect.TopRight3 + new Vector3(-0.5f, 0, -0.5f);
                Vector3 center = (topRight + topLeft) / 2.0f;
                int width = (int)Math.Ceiling(Vector3.Distance(topRight, topLeft) / sideWalkWidth);
                meshData.AddMeshData(NewMakeSide(center, width, sideWalkWidth, sideWalkHeight, true));
            }

            // BOT
            {
                Vector3 topLeft = rect.BotRight3 + new Vector3(-0.5f, 0, 0.5f);
                Vector3 topRight = rect.BotLeft3 + new Vector3(0.5f, 0, 0.5f);
                Vector3 center = (topRight + topLeft) / 2.0f;
                int width = (int)Math.Ceiling(Vector3.Distance(topRight, topLeft) / sideWalkWidth);

                MeshData data = NewMakeSide(center, width, sideWalkWidth, sideWalkHeight, true);
                data.RotateAround(center, new Vector3(0, 180, 0));
                meshData.AddMeshData(data);
            }

            // Left
            {
                Vector3 topLeft = rect.BotLeft3 + new Vector3(0.5f, 0, 0.5f);
                Vector3 topRight = rect.TopLeft3 + new Vector3(0.5f, 0, -0.5f);
                Vector3 center = (topRight + topLeft) / 2.0f;
                int width = (int)Math.Ceiling(Vector3.Distance(topRight, topLeft) / sideWalkWidth) -
                            2; // Account for overlap

                MeshData data = NewMakeSide(center, width, sideWalkWidth, sideWalkHeight, false);
                data.RotateAround(center, new Vector3(0, -90, 0));
                meshData.AddMeshData(data);
            }

            // Right
            {
                Vector3 topLeft = rect.TopRight3 + new Vector3(-0.5f, 0, -0.5f);
                Vector3 topRight = rect.BotRight3 + new Vector3(-0.5f, 0, 0.5f);
                Vector3 center = (topRight + topLeft) / 2.0f;
                int width = (int)Math.Ceiling(Vector3.Distance(topRight, topLeft) / sideWalkWidth) -
                            2; // Account for overlap

                MeshData data = NewMakeSide(center, width, sideWalkWidth, sideWalkHeight, false);
                data.RotateAround(center, new Vector3(0, 90, 0));
                meshData.AddMeshData(data);
            }
        }

        mesh.vertices = meshData.Vertices.ToArray();
        mesh.uv = meshData.UVs.ToArray();
        mesh.triangles = meshData.Triangles.ToArray();
        
        mesh.Optimize();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        sideWalk.AddComponent<MeshFilter>().mesh = mesh;
        sideWalk.AddComponent<MeshRenderer>().material = material;

        if (currentSideWalkObject != null)
        {
            DestroyImmediate(currentSideWalkObject);
        }

        currentSideWalkObject = sideWalk;
    }
}