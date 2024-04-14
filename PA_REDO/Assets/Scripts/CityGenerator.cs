using System.Collections.Generic;
using DataStructures;
using UnityEngine;

[RequireComponent(typeof(LayoutGenerator))]
[RequireComponent(typeof(RoadGenerator))]
[RequireComponent(typeof(SideWalkBuilder))]
public class CityGenerator : MonoBehaviour
{
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
    }

    public void FullClear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}