using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataStructures;
using UnityEngine;

[RequireComponent(typeof(Terrain))]

public class TerrainGenerator : MonoBehaviour
{
    private struct RectangleData
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
    [SerializeField] private float islandHeight = 0.2f;
    

    private List<RectangleData> rectangles = new List<RectangleData>();

    private List<Island> islands = new List<Island>();

    public void Generate()
    {
        Clear();
        
        TerrainData terrainData = terrain.terrainData;
        Vector3 size = terrainData.bounds.size;
        Rectangle rectangle = new Rectangle(0, 0, (int) size.x, (int) size.z);
        SplitRectangle(rectangle);

        List<Task> tasks = new List<Task>();
        LimitedConcurrencyLevelTaskScheduler lcts = new(8);
        
        TaskFactory factory = new TaskFactory(lcts);
        
        
        for (int i = rectangles.Count - 1; i >= 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                HandleOverlap(i, j);
            }
        }

        for (int i = 0; i < rng.Int(minimumIslands, maxIslands); i++)
        {
            int index = rng.Int(0, rectangles.Count);

            Island island = new Island
            {
                islandIndexes = new HashSet<int> { index },
                islandStartIndex = index
            };

            island.islandIndexes.Add(index);
            AddFromSquareToIsland(index, 0, island);
            islands.Add(island);
        }

        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];

        Vector3 pos = transform.position;

        Vector3 terrainSize = terrainData.size;
        int heightmapResolution = terrainData.heightmapResolution;
        
        foreach (Island island in islands)
        {
            Task t = factory.StartNew(() =>
            {
                foreach (int a in island.islandIndexes)
                {
                    Rectangle rect = rectangles[a].Rectangle;

                    int topLeftX = (int) ((rect.X - pos.x) / terrainSize.x * heightmapResolution);
                    int topLeftZ = (int) ((rect.Top - pos.z) / terrainSize.z * heightmapResolution);
                    int botRightX = (int) ((rect.Right - pos.x) / terrainSize.x * heightmapResolution);
                    int botRightZ = (int) ((rect.Z - pos.z) / terrainSize.z * heightmapResolution);

                    for (int rX = topLeftX; rX < botRightX; rX++)
                    {
                        for (int rZ = botRightZ; rZ < topLeftZ; rZ++)
                        {
                            heights[rZ, rX] = islandHeight;
                        }
                    }
                }
            });
            
            tasks.Add(t);
        }

        Task.WaitAll(tasks.ToArray());
        
        terrainData.SetHeights(0, 0, heights);
        terrainData.SyncHeightmap();
    }

    private void AddFromSquareToIsland(int squareIndex, int count, Island island)
    {
        count++;

        foreach (int neighbourIndex in rectangles[squareIndex].NeighbourIndexes)
        {
            island.islandIndexes.Add(neighbourIndex);

            if (count != recursionDepth)
            {
                AddFromSquareToIsland(neighbourIndex, count, island);
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
            RectangleData pair = rectangles[index];
        
            RectangleExtensions.Color = Color.blue;
            pair.Rectangle.Draw(transform.position,drawCorners:false);
        
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
        
            rectangles[island.islandStartIndex].Rectangle.Draw(transform.position);

            foreach (int index in island.islandIndexes)
            {
                rectangles[index].Rectangle.Draw(transform.position);
            }
        }
    }

    private void SplitRectangle(Rectangle rectangle)
    {
        Rectangle newRectangle;
        
        if (rectangle.Width >= minimumWidth * 2)
        {
            float minX = rectangle.X + minimumWidth;
            float maxX = rectangle.Right - minimumWidth;
            float splitX;
            
            if (minX >= maxX)
            {
                splitX = minX;
            }
            else
            {
                splitX = rng.Float(minX, maxX);
            }

            newRectangle = new Rectangle(splitX, rectangle.Z, rectangle.Right - splitX, 
                rectangle.Depth);
            
            rectangle.Width = splitX - rectangle.X;
        }
        else if (rectangle.Depth >= minimumDepth * 2)
        {
            float minZ = rectangle.Z + minimumDepth;
            float maxZ = rectangle.Top - minimumDepth;

            float splitZ;

            if (Math.Abs(minZ - maxZ) < 0.001f)
            {
                splitZ = minZ;
            }
            else
            {
                splitZ = rng.Float(minZ, maxZ);
            }
            
            newRectangle = new Rectangle(rectangle.X, splitZ, rectangle.Width,
                rectangle.Top - splitZ);
            
            rectangle.Depth = splitZ - rectangle.Z;
        }
        else
        {
            rectangles.Add(new RectangleData
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
