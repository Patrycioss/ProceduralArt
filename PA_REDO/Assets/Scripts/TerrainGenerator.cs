using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStructures;
using UnityEngine;

[RequireComponent(typeof(Terrain))]

public class TerrainGenerator : MonoBehaviour
{
    private struct RectangleNeighbours
    {
        public Rectangle Rectangle;
        public HashSet<int> NeighbourIndexes;
    }

    private struct Island
    {
        public HashSet<int> islandIndexes;
        public int islandStartIndex;
    }
    
    [SerializeField] private int minimumWidth = 400;
    [SerializeField] private int minimumDepth = 30;
    [SerializeField] private Terrain terrain;
    [SerializeField] private RNG rng;
    
    [Header("Islands")]
    [SerializeField] private int maxIslands = 5;
    [SerializeField] private int minimumIslands = 2;
    [SerializeField] private int recursionDepth = 2;

    private List<RectangleNeighbours> rectangles = new List<RectangleNeighbours>();

    private List<Island> islands = new List<Island>();

    public void Generate()
    {
        Clear();
        
        TerrainData terrainData = terrain.terrainData;
        int heightmapResolution = terrainData.heightmapResolution;
        Vector3 size = terrainData.bounds.size;
        
        float[,] mesh = new float[heightmapResolution, heightmapResolution];

        Rectangle rectangle = new Rectangle(0, 0, (int) size.x, (int) size.z);
        SplitRectangle(rectangle);

        List<Task> tasks = new List<Task>();
        
        for (int i = rectangles.Count - 1; i >= 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
               HandleOverlap(i,j);
            }
        }

        for (int i = 0; i < rectangles.Count; i++)
        {
            Debug.Log($"Rectangle {i}: {rectangles[i]}");
        }

        for (int i = 0; i < rng.Int(minimumIslands, maxIslands); i++)
        {
            int index = rng.Int(0, rectangles.Count);

            Island island = new Island
            {
                islandIndexes = new HashSet<int>
                {
                    index
                },
                islandStartIndex = index
            };

            island.islandIndexes.Add(index);
            AddFromSquareToIsland(index, 0);
            islands.Add(island);
            continue;

            void AddFromSquareToIsland(int squareIndex, int count)
            {
                count++;

                foreach (int neighbourIndex in rectangles[squareIndex].NeighbourIndexes)
                {
                    island.islandIndexes.Add(neighbourIndex);

                    if (count != recursionDepth)
                    {
                        AddFromSquareToIsland(neighbourIndex, count);
                    }
                }
            }
        }
    }

    public void Clear()
    {
        islands.Clear();
        rectangles.Clear();
    }

    private void HandleOverlap(int rectangleIndex, int otherIndex)
    {
        Rectangle rectangle = rectangles[rectangleIndex].Rectangle;
        if (rectangle.Overlaps(rectangles[otherIndex].Rectangle))
        {
            rectangles[rectangleIndex].NeighbourIndexes.Add(otherIndex);
            rectangles[otherIndex].NeighbourIndexes.Add(rectangleIndex);
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        for (int index = 0; index < rectangles.Count; index++)
        {
            RectangleNeighbours pair = rectangles[index];

            RectangleExtensions.Color = Color.blue;
            pair.Rectangle.Draw();

            // Gizmos.color = Color.magenta;
            // for (int i = 0; i < pair.NeighbourIndexes.Count; i++)
            // {
            //     int neighbour = pair.NeighbourIndexes.ElementAt(i);
            //     Gizmos.DrawLine(RectangleExtensions.ToVector3(pair.Rectangle.Center + new Vector2(Mathf.Sin(index) * 10, Mathf.Cos(index) * 10)),
            //         RectangleExtensions.ToVector3(rectangles[neighbour].Rectangle.Center +
            //                                       new Vector2(Mathf.Sin(index) * 10, Mathf.Cos(index) * 10)));
            // }
        }

        foreach (Island island in islands)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(RectangleExtensions.ToVector3(rectangles[island.islandStartIndex].Rectangle.Center), 20);
            
            RectangleExtensions.Color = Color.green;
        
            foreach (int index in island.islandIndexes)
            {
                rectangles[index].Rectangle.Draw();
            }
        }
    }

    private void SplitRectangle(Rectangle rectangle)
    {
        Rectangle newRectangle;
        
        Debug.Log($"Splitting Rectangle: {rectangle}");
        
        if (rectangle.Width >= minimumWidth * 2)
        {
            int minX = rectangle.X + minimumWidth;
            int maxX = rectangle.Right - minimumWidth;
            Debug.Log($"MinX: {minX}, MaxX: {maxX}");

            int splitX;
            
            if (minX >= maxX)
            {
                splitX = minX;
            }
            else
            {
                splitX = rng.Int(minX, maxX);
            }

            newRectangle = new Rectangle(splitX, rectangle.Z, rectangle.Right - splitX, 
                rectangle.Depth);
            
            rectangle.Width = splitX - rectangle.X;
        }
        else if (rectangle.Depth >= minimumDepth * 2)
        {
            int minZ = rectangle.Z + minimumDepth;
            int maxZ = rectangle.Top - minimumDepth;

            int splitZ;

            if (minZ == maxZ)
            {
                splitZ = minZ;
            }
            else
            {
                splitZ = rng.Int(minZ, maxZ);
            }
            
            newRectangle = new Rectangle(rectangle.X, splitZ, rectangle.Width,
                rectangle.Top - splitZ);
            
            rectangle.Depth = splitZ - rectangle.Z;
        }
        else
        {
            rectangles.Add(new RectangleNeighbours
            {
                Rectangle = rectangle,
                NeighbourIndexes = new HashSet<int>()
            });
            return;
        }
        
        SplitRectangle(rectangle);
        SplitRectangle(newRectangle);
    }
}
