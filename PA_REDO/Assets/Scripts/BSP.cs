using System;
using System.Collections.Generic;
using DataStructures;

public static class BSP
{
	public static List<Rectangle> Partition(Rectangle rootRect, RNG rng, int minimumWidth, int minimumDepth)
	{
		List<Rectangle> partitions = new List<Rectangle>();

        SplitRectangle(rootRect);

        return partitions;

        void SplitRectangle(Rectangle rectangle)
        {
            Rectangle newRectangle;
            
            if (rectangle.Width >= minimumWidth * 2)
            {
                int minX = (int) (rectangle.X + minimumWidth);
                int maxX = (int) (rectangle.Right - minimumWidth);
                float splitX;
                
                if (Math.Abs(minX - maxX) < 0.000001f)
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
                int minZ = (int) (rectangle.Z + minimumDepth);
                int maxZ = (int) (rectangle.Top - minimumDepth);
    
                float splitZ;
    
                if (Math.Abs(minZ - maxZ) < 0.000001f)
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
                partitions.Add(rectangle);
                return;
            }
            
            SplitRectangle(rectangle);
            SplitRectangle(newRectangle);
        }
    }

    public static Dictionary<Rectangle, HashSet<Rectangle>> FindNeighbours(List<Rectangle> rectangles)
    {
        Dictionary<Rectangle, HashSet<Rectangle>> sortedOuts = new Dictionary<Rectangle, HashSet<Rectangle>>();
        
        for (int i = rectangles.Count - 1; i >= 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                HandleOverlap(i, j);
            }
        }

        return sortedOuts;

        void HandleOverlap(int rectangleIndex, int otherIndex)
        {
            Rectangle rectangle = rectangles[rectangleIndex];
            Rectangle otherRectangle = rectangles[otherIndex];
           
            if (rectangle.Overlaps(rectangles[otherIndex]))
            {
                if (sortedOuts.ContainsKey(rectangle))
                {
                    sortedOuts[rectangle].Add(otherRectangle);
                }
                else
                {
                    sortedOuts.TryAdd(rectangle, new HashSet<Rectangle>()
                    {
                        otherRectangle
                    });
                }

                if (sortedOuts.ContainsKey(otherRectangle))
                {
                    sortedOuts[otherRectangle].Add(rectangle);
                }
                else
                {
                    sortedOuts.TryAdd(otherRectangle, new HashSet<Rectangle>()
                    {
                        rectangle
                    });
                }
            }
        }
    }
}