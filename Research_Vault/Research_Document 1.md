Patrick Schuur 510154
## Introduction
When thinking about my Procedural Art redo I realized that the Half Life City that I chose didn't appeal to me at all as I barely played Half Life and I didn't care much for the style. I also drew the conclusion that it would be easier to write a research document if the city that I chose was actually interesting to me.

For this reason I chose to recreate Gotham City from the Batman comics. I realized there are a lot of versions of Gotham City so I will take whatever I can find online and combine the things together that I like. It will mostly resemble a Gotham City like in this image from the comics.
![[Gotham_skyline1.webp]]


## City Layout 
The first thing that I wanted to look into was the layout of the city. Luckily for me I could find a nice map of how Gotham City is layed out:
![[Gotham_map.webp]]

The first thing that I noted was how Gotham City is made up of 3 large islands with a couple of small islands attached. I decided that I wanted to generate the islands procedurally because I don't want to make a one-on-one replica but a city that has the same vibes.
I started with dividing a big terrain piece in Unity up into smaller squares using binary space partitioning. 

![[Pasted image 20240222012822.png]]

```cs
//In Generate()
Vector3 size = terrainData.bounds.size;  
Rectangle rectangle = new Rectangle(0, 0, (int) size.x, (int) size.z);  
SplitRectangle(rectangle);
```
```cs
private void SplitRectangle(Rectangle rectangle)  
{  
    Rectangle newRectangle;  
    if (rectangle.Width >= minimumWidth * 2)  
    {        
	    int minX = rectangle.X + minimumWidth;  
        int maxX = rectangle.Right - minimumWidth;  
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
        rectangles.Add(rectangle);        
        return;  
    }    
    
    SplitRectangle(rectangle);  
    SplitRectangle(newRectangle);  
}
```

I then realised I could maybe make the islands by branching out into the neighbouring squares like this:

![[Pasted image 20240222013918.png]]

I first made an algorithm to assign the neighbours to the nodes like so:
```cs
// Replaced the list of rectangles with a list of:
private struct RectangleData  
{  
    public Rectangle Rectangle;  
    public HashSet<int> NeighbourIndexes;  
}

//In Generate()
for (int i = rectangles.Count - 1; i >= 0; i--)  
{  
    for (int j = i - 1; j >= 0; j--)  
    {       
	    HandleOverlap(i,j);  
    }
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

//An extension method to check whether two rectangles overlap.
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
```


I then picked a random room and made an island from that by branching through the neighboring rooms. I also added some editor settings. The green parts are the islands with the red spheres being the center squares of the islands:

![[Pasted image 20240222013228.png]]
![[Pasted image 20240222013242.png]]
```cs 
private struct Island  
{  
    public HashSet<int> islandIndexes;  
    public int islandStartIndex;  
}

//In Generate()
for (int i = 0; i < rng.Int(minimumIslands, maxIslands); i++)  
{  
    int index = rng.Int(0, rectangles.Count);  
  
    Island island = new Island  
    {  
        islandIndexes = new HashSet<int> { index },  
        islandStartIndex = index  
    };  
    
    island.islandIndexes.Add(index);  
    AddFromSquareToIsland(index, 0);  
    islands.Add(island);  
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
``` 

The next step was making terrain out of these islands. I translated the coordinates of every square's top-left and bottom-right corner to the appropriate coordinates on the heightmap and then I filled in all the coordinates on the heightmap between those bounds to create the terrain. 
![[Pasted image 20240310093844.png]]

```cs
float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];  
  
Vector3 pos = transform.position;  
  
foreach (Island island in islands)  
{  
    foreach (int a in island.islandIndexes)  
    {        
        Rectangle rect = rectangles[a].Rectangle;  
        
        int topLeftX = (int) ((rect.X - pos.x) / terrainData.size.x   
				* terrainData.heightmapResolution);  
		
		int topLeftZ = (int) ((rect.Top - pos.z) / terrainData.size.z   
				* terrainData.heightmapResolution);  
		
		int botRightX = (int) ((rect.Right - pos.x) / terrainData.size.x   
				* terrainData.heightmapResolution);  
		
		int botRightZ = (int) ((rect.Z - pos.z) / terrainData.size.z   
				* terrainData.heightmapResolution);
  
        for (int rX = topLeftX; rX < botRightX; rX++)  
        {            
	        for (int rZ = botRightZ; rZ < topLeftZ; rZ++)  
            {                
	            heights[rZ, rX] = islandHeight;  
            }        
        }    
    }
}  
  
terrainData.SetHeights(0, 0, heights);  
terrainData.SyncHeightmap();
```

Because these are meant to be islands there has to be water between them. Before I want to start making the islands look good I first want to get some water in. This will hopefully help me in seeing whether it looks good or not. I decided to use [this tutorial by Unity](https://youtu.be/gRq-IdShxpU)to implement a water shader.

First problem I ran into with this tutorial is that they use a plane with extra subdivisions but provide no way to get/make one. So I found [a tutorial](https://youtu.be/-3ekimUWb9I) to make a script that generates a plane with subdivisions. I took the code from the tutorial and slightly tweaked a bit and added an editor script so I can generate the mesh in the editor. 
![[Pasted image 20240310100536.png]]
```cs
[CustomEditor(typeof(PlaneGenerator))]  
public class PlaneGeneratorEditor : UnityEditor.Editor  
{  
    public override void OnInspectorGUI()  
    {       
       PlaneGenerator planeGenerator = (PlaneGenerator) target;  
       
       if (GUILayout.Button("Generate"))   
          planeGenerator.Generate();  
       
       base.OnInspectorGUI();  
    }
}
```

```cs 
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
```

I followed the rest of the tutorial to the best of my abilities. The tutorial however seems to be pretty outdated and my shader ended up not working, leaving me with a shadergraph and a blue transparent plane. Because my knowledge of shaders is rather limited I decided to look for another tutorial. I stumbled upon [this one](https://youtu.be/yJrc_ywpTJw)which is a lot more recent and thus hopefully works. 
![[Pasted image 20240401120030.png]]
With some tweaking I ended up with this, it's not the worst but it could be better. I now have to start focusing on making the actual city though because I'm wasting too much time.

## The city itself
Looking at Gotham city in the image below I can only make out the shape of the buildings. For the actual materials I will have to find different images. With these silhouettes I can at least make out the shape of the building and what buildings I want to recreate.
![[Gotham_skyline1.webp]]
What I'm thinking of is that I want to make a tower like the one on the top-left: ![[Pasted image 20240401120449.png]]
And some of the buildings that can be seen on the front.
The first thing that I can see is that the shapes are all very rectangular. Everything has hard edges and corners. The tower has columns that go over the length of the tower. The most noticeable thing about the buildings is the many windows. They're all in grid-like patterns distributed over the buildings. The artist here has chosen to either have no windows in an area or a lot of them close together. 
![[Pasted image 20240401121520.png]]
I'm going to split up the buildings in a grid style like this and generate the individual pieces for each side.

## Materials
My reference image is not detailed enough to know what materials I want to use so I have to pull inspiration from other sources. One source that I found is from the Tim Burton Batman movies, these films use a sort of dystopian urbanist architecture which I think is in line with my reference image.
![[1460658354962tim-burton-visual-analysis-17.avif]]

The main materials that I want to make are worn concrete like on the top-right and bricks like you can see on the building in the background.
For the roads the double lines down the middle.


![[unnamed.jpg]]
![[batman-gotham-city-chronicles-roleplaying-game-artwork.webp]]