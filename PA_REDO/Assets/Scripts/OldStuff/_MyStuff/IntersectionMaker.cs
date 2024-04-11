using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts._MyStuff;
using _Scripts._MyStuff.CityLayout;
using _Scripts.SchoolMeshGeneration;
using UnityEngine;

public class IntersectionMaker : MonoBehaviour
{
    [SerializeField] private SaveData _saveData;

    [SerializeField] private float _baseIntersectionWidth;
    public float intersectionWidth => _baseIntersectionWidth + (_sideWalkWidth * 2);
    public float hIntersectionWidth => intersectionWidth / 2.0f;
    [SerializeField] private Material _intersectionMaterial;
    
    [SerializeField] private Material _roadMaterial;
    [SerializeField] private float _sideWalkHeight = 0.05f;
    public float sideWalkHeight => _sideWalkHeight;
    [SerializeField] private float _sideWalkWidth = 0.2f;
    public float sideWalkWidth => _sideWalkWidth;
    [SerializeField] private Material _sideWalkMaterial;
    
    private List<Node> _intersections = new();
    private MeshBuilder _meshBuilder = new();

    public void Clear()
    {
        _intersections.Clear();
        DeleteChild("IntersectionsContainer");
        DeleteChild("Roadscontainer");
    }
    
    public void DeleteChild(string name)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (transform.GetChild(i).name == name)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
                return;
            }
        }
    }
    
    public void Load()
    {
        if (_saveData != null)
            _intersections = new(_saveData.data.intersections);
        else Debug.LogError("No SaveData Selected");
        Debug.Log($"Loaded {_intersections.Count} intersections");
    }

    public void Generate()
    {
        while (transform.childCount > 0) 
            DestroyImmediate(transform.GetChild(0).gameObject);
        
        Load();

        GameObject intersectionsContainer = new GameObject("IntersectionsContainer");
        intersectionsContainer.transform.parent = this.transform;
        intersectionsContainer.transform.localPosition = Vector3.zero;

        GameObject roadsContainer = new GameObject("Roadscontainer");
        roadsContainer.transform.parent = this.transform;
        roadsContainer.transform.localPosition = Vector3.zero;
        int roadCount = 0;
        
        float hBWidth = _baseIntersectionWidth / 2.0f;

        Vector3[] baseCorners = new Vector3[4];
        baseCorners[0] = new Vector3(-hBWidth, 0, hBWidth);
        baseCorners[1] = new Vector3(hBWidth, 0, hBWidth);
        baseCorners[2] = new Vector3(hBWidth, 0, -hBWidth);
        baseCorners[3] = new Vector3(-hBWidth, 0, -hBWidth);
        
        foreach (Node intersection in _intersections)
        {

            GameObject intersectionObject = new("Intersection");
            intersectionObject.transform.parent = intersectionsContainer.transform;
            intersectionObject.transform.position = this.transform.position + intersection.ToVector3();

            float hSideWalkWidth = _sideWalkWidth / 2.0f;
            
            foreach (KeyValuePair<Node.Direction, Node> connection in intersection.connections)
            {
                if (connection.Value == null) continue;
                Node.Direction direction = connection.Key;
                if (direction is Node.Direction.North or Node.Direction.West)
                    continue;

                Node toNode = connection.Value;

                roadCount++;
                Vector3 roadPos = intersection.ToVector3() + roadsContainer.transform.position;
                GameObject road = new GameObject("Road " + roadCount);
                road.transform.parent = roadsContainer.transform;
                road.transform.position = roadPos;

                Vector3 distO = toNode.ToVector3() - intersection.ToVector3();

                MeshBuilder roadMeshBuilder = new();
                
                Vector3[] roadVertices = new Vector3[4];
                switch (connection.Key)
                {
                    case Node.Direction.South:
                        roadVertices[0] = baseCorners[3]; //fromLeft
                        roadVertices[1] = baseCorners[2]; //fromRight
                        roadVertices[2] = new Vector3(distO.x + hBWidth, 0, distO.z + hBWidth); //toRight
                        roadVertices[3] = new Vector3(distO.x - hBWidth, 0, distO.z + hBWidth); //toLeft
                        
                        roadMeshBuilder.AddVertex(roadVertices[0], new(0, 1));
                        roadMeshBuilder.AddVertex(roadVertices[1], new(0, 0));
                        roadMeshBuilder.AddVertex(roadVertices[2], new(1, 0));
                        roadMeshBuilder.AddVertex(roadVertices[3], new(1, 1));
                        break;
                    case Node.Direction.East:
                        roadVertices[0] = baseCorners[1]; //fromUp
                        roadVertices[3] = baseCorners[2]; //fromDown
                        roadVertices[1] = new Vector3(distO.x - hBWidth, 0, distO.z + hBWidth); //toUp
                        roadVertices[2] = new Vector3(distO.x - hBWidth, 0, distO.z - hBWidth); //toDown
                        
                        roadMeshBuilder.AddVertex(roadVertices[0], new(0, 1));
                        roadMeshBuilder.AddVertex(roadVertices[1], new(1, 1));
                        roadMeshBuilder.AddVertex(roadVertices[2], new(1, 0));
                        roadMeshBuilder.AddVertex(roadVertices[3], new(0, 0));
                        break;
                }

                roadMeshBuilder.AddTriangle(0, 1, 2);
                roadMeshBuilder.AddTriangle(2, 3, 0);
                
                MeshRenderer meshRenderer = road.AddComponent<MeshRenderer>();
                meshRenderer.material = _roadMaterial;
                Mesh roadMesh = road.AddComponent<MeshFilter>().mesh = roadMeshBuilder.CreateMesh();

                GameObject leftSideWalk = new GameObject("LeftSideWalk");
                leftSideWalk.transform.parent = road.transform;
                leftSideWalk.transform.localPosition = Vector3.zero;

                GameObject rightSideWalk = new GameObject("RightSideWalk");
                rightSideWalk.transform.parent = road.transform;

                float sideWalkDepth;
                float hSideWalkDepth;
                
                Vector3[] sideWalkVertices = new Vector3[4];
                Vector3[] extraVertices = new Vector3[4];

                Mesh lMesh = new();
                Mesh rMesh = new();

                MeshBuilder leftBuilder = new();
                MeshBuilder rightBuilder = new();

                int ttl; //toptopleft
                int ttr; //toptopright
                int tbr; //topbottomright
                int tbl; //topbottomleft
                int rbl; //rightbottomleft
                int rbr; //rightbottomright
                int lbl; //leftbottomleft
                int lbr; //leftbottomright
                int fbr; //frontbottomright
                int fbl; //frontbottomleft
                int bbr; //backbottomright
                int bbl; //backbottomleft
                
                
                switch (direction)
                {
                    case Node.Direction.South:
                        sideWalkDepth = roadMesh.bounds.max.z - roadMesh.bounds.min.z;
                        hSideWalkDepth = sideWalkDepth / 2.0f;
                        
                        leftSideWalk.transform.localPosition = roadMesh.bounds.center + 
                                                               new Vector3(-(hSideWalkWidth + 
                                                                             (roadMesh.bounds.max.x - roadMesh.bounds.min.x)/2), 
                                                                   _sideWalkHeight, 
                                                                   0);

                        rightSideWalk.transform.localPosition = roadMesh.bounds.center +
                                                                new Vector3((hSideWalkWidth + 
                                                                             (roadMesh.bounds.max.x - roadMesh.bounds.min.x)/2), 
                                                                    _sideWalkHeight, 
                                                                    0);
                    //Vertices
                        //Top
                        sideWalkVertices[0] = new Vector3(-hSideWalkWidth, 0, hSideWalkDepth);
                        sideWalkVertices[1] = new Vector3(hSideWalkWidth, 0, hSideWalkDepth);
                        sideWalkVertices[2] = new Vector3(hSideWalkWidth, 0, -hSideWalkDepth);
                        sideWalkVertices[3] = new Vector3(-hSideWalkWidth, 0, -hSideWalkDepth);
                        
                        ttl = leftBuilder.AddVertex(sideWalkVertices[0], new(0, 1));
                        ttr = leftBuilder.AddVertex(sideWalkVertices[1], new(0, 0));
                        tbr = leftBuilder.AddVertex(sideWalkVertices[2], new(1, 0));
                        tbl = leftBuilder.AddVertex(sideWalkVertices[3], new(1, 1));

                        //Right
                        rbl = leftBuilder.AddVertex(new(hSideWalkWidth, -_sideWalkHeight, -hSideWalkDepth), new(1,1));
                        rbr = leftBuilder.AddVertex(new(hSideWalkWidth, -_sideWalkHeight, hSideWalkDepth), new(0,1));
                    
                        //Front
                        fbr = leftBuilder.AddVertex(new(-hSideWalkWidth, -_sideWalkHeight, hSideWalkDepth), new(1,1));
                        
                        //Back
                        bbr = leftBuilder.AddVertex(leftBuilder.GetVertex(rbl), new(0,0));
                        bbl = leftBuilder.AddVertex(new(-hSideWalkWidth, -_sideWalkHeight, -hSideWalkDepth), new(0,1));
                        
                    //Triangles
                        //Top
                        leftBuilder.AddTriangle(ttl,ttr,tbr);
                        leftBuilder.AddTriangle(tbr,tbl,ttl);
                        
                        //Right
                        leftBuilder.AddTriangle(tbr,ttr,rbr);
                        leftBuilder.AddTriangle(rbr,rbl,tbr);
                        
                        //Front
                        leftBuilder.AddTriangle(ttr,ttl,fbr);
                        leftBuilder.AddTriangle(fbr,rbr,ttr);
                        
                        //Back
                        leftBuilder.AddTriangle(tbl,tbr,bbr);
                        leftBuilder.AddTriangle(bbr,bbl,tbl);

                        lMesh = leftBuilder.CreateMesh();
                        
                        ttl = rightBuilder.AddVertex(sideWalkVertices[0], new(0, 1));
                        ttr = rightBuilder.AddVertex(sideWalkVertices[1], new(0, 0));
                        tbr = rightBuilder.AddVertex(sideWalkVertices[2], new(1, 0));
                        tbl = rightBuilder.AddVertex(sideWalkVertices[3], new(1, 1));
                        
                        //Leftside
                        lbl = rightBuilder.AddVertex(new(-hSideWalkWidth, -_sideWalkHeight, -hSideWalkDepth), new(1,0));
                        lbr = rightBuilder.AddVertex(new(-hSideWalkWidth, -_sideWalkHeight, hSideWalkDepth), new(0,0));
                        
                        //FrontSide
                        fbl = rightBuilder.AddVertex(new(hSideWalkWidth, -_sideWalkHeight, hSideWalkDepth), new(0,1));
                        fbr = rightBuilder.AddVertex(rightBuilder.GetVertex(lbr), new(1,1));
                        
                        //BackSide
                        bbl = rightBuilder.AddVertex(new(hSideWalkWidth, -_sideWalkHeight, -hSideWalkDepth), new(0,0));
                        
                        //Top
                        rightBuilder.AddTriangle(ttl,ttr,tbr);
                        rightBuilder.AddTriangle(tbr,tbl,ttl);
                        
                        //Right
                        rightBuilder.AddTriangle(ttl,tbl,lbl);
                        rightBuilder.AddTriangle(lbl,lbr,ttl);
                        
                        //Front
                        rightBuilder.AddTriangle(ttr, ttl, fbr);
                        rightBuilder.AddTriangle(fbr,fbl,ttr);
                        
                        //Back
                        rightBuilder.AddTriangle(tbl,tbr,bbl);
                        rightBuilder.AddTriangle(bbl,lbl,tbl);
                        
                        rMesh = rightBuilder.CreateMesh();
                        
                        break;
                    
                    case Node.Direction.East:
                        sideWalkDepth = roadMesh.bounds.max.x - roadMesh.bounds.min.x;
                        hSideWalkDepth = sideWalkDepth/2.0f - _sideWalkWidth;

                        leftSideWalk.transform.localPosition = roadMesh.bounds.center + 
                                                               new Vector3(0, 
                                                                   _sideWalkHeight, 
                                                                   (hSideWalkWidth + 
                                                                    (roadMesh.bounds.max.z - roadMesh.bounds.min.z)/2));

                        rightSideWalk.transform.localPosition = roadMesh.bounds.center +
                                                                new Vector3(0, 
                                                                    _sideWalkHeight, 
                                                                    -(hSideWalkWidth + 
                                                                      (roadMesh.bounds.max.z - roadMesh.bounds.min.z)/2));

                        sideWalkVertices[0] = new Vector3(-hSideWalkDepth, 0, hSideWalkWidth);
                        sideWalkVertices[1] = new Vector3(hSideWalkDepth, 0, hSideWalkWidth);
                        sideWalkVertices[2] = new Vector3(hSideWalkDepth, 0, -hSideWalkWidth);
                        sideWalkVertices[3] = new Vector3(-hSideWalkDepth, 0, -hSideWalkWidth);
                        
                        ttl = leftBuilder.AddVertex(sideWalkVertices[0], new(0, 0));
                        ttr = leftBuilder.AddVertex(sideWalkVertices[1], new(1, 0));
                        tbr = leftBuilder.AddVertex(sideWalkVertices[2], new(1, 1));
                        tbl = leftBuilder.AddVertex(sideWalkVertices[3], new(0, 1));

                        rbl = leftBuilder.AddVertex(new(-hSideWalkDepth, -_sideWalkHeight, -hSideWalkWidth),new(0,0));
                        rbr = leftBuilder.AddVertex(new(hSideWalkDepth, -_sideWalkHeight, -hSideWalkWidth),new(1,0));
                        
                        leftBuilder.AddTriangle(ttl,ttr,tbr);
                        leftBuilder.AddTriangle(tbr,tbl,ttl);
                        leftBuilder.AddTriangle(tbl,tbr,rbr);
                        leftBuilder.AddTriangle(rbr,rbl,tbl);

                        lMesh = leftBuilder.CreateMesh();

                        
                        ttl = rightBuilder.AddVertex(sideWalkVertices[0], new(0, 0));
                        ttr = rightBuilder.AddVertex(sideWalkVertices[1], new(1, 0));
                        tbr = rightBuilder.AddVertex(sideWalkVertices[2], new(1, 1));
                        tbl = rightBuilder.AddVertex(sideWalkVertices[3], new(0, 1));
                        
                        lbr = rightBuilder.AddVertex(new(-hSideWalkDepth, -_sideWalkHeight, hSideWalkWidth),new(1,1));
                        lbl = rightBuilder.AddVertex(new(hSideWalkDepth, -_sideWalkHeight, hSideWalkWidth),new(0,1));
                        
                        rightBuilder.AddTriangle(ttl,ttr,tbr);
                        rightBuilder.AddTriangle(tbr,tbl,ttl);
                        rightBuilder.AddTriangle(ttr,ttl,lbr);
                        rightBuilder.AddTriangle(lbr,lbl,ttr);
                        
                        rMesh = rightBuilder.CreateMesh();
                        break;
                }
                
                leftSideWalk.AddComponent<MeshRenderer>().material = _sideWalkMaterial;
                leftSideWalk.AddComponent<MeshFilter>().mesh = lMesh;
                rightSideWalk.AddComponent<MeshRenderer>().material = _sideWalkMaterial;
                rightSideWalk.AddComponent<MeshFilter>().mesh = rMesh;
            }
            
            MeshBuilder intersectionBuilder = new();
            intersectionBuilder.AddVertex(baseCorners[0], new Vector2(0,0));
            intersectionBuilder.AddVertex(baseCorners[1], new Vector2(0,1));
            intersectionBuilder.AddVertex(baseCorners[2], new Vector2(1,1));
            intersectionBuilder.AddVertex(baseCorners[3], new Vector2(1,0));
            
            intersectionBuilder.AddTriangle(0,1,2);
            intersectionBuilder.AddTriangle(2,3,0);
            
            GameObject intersectionBaseObject = new("Base");
            intersectionBaseObject.transform.parent = intersectionObject.transform;
            intersectionBaseObject.transform.localPosition = Vector3.zero;
            
            MeshRenderer baseRenderer = intersectionBaseObject.AddComponent<MeshRenderer>();
            baseRenderer.material = _intersectionMaterial;
            intersectionBaseObject.AddComponent<MeshFilter>().mesh = intersectionBuilder.CreateMesh();
            intersectionBuilder.Clear();

            bool sidewalk = false;
            
            if (intersection.connections[Node.Direction.South] == null)
            {
                sidewalk = true;

                int ttl = intersectionBuilder.AddVertex(new(-hBWidth - _sideWalkWidth, _sideWalkHeight, -hBWidth),new(0,0));
                int ttr = intersectionBuilder.AddVertex(new(hBWidth + _sideWalkWidth, _sideWalkHeight, -hBWidth),new(1,0));
                int tbr = intersectionBuilder.AddVertex(new(hBWidth + _sideWalkWidth, _sideWalkHeight, -hBWidth - _sideWalkWidth), new(1,1));
                int tbl = intersectionBuilder.AddVertex(new(-hBWidth - _sideWalkWidth, _sideWalkHeight, -hBWidth - _sideWalkWidth), new(0,1));
                
                int fbr = intersectionBuilder.AddVertex(new(hBWidth + _sideWalkWidth, 0, -hBWidth), new(0,1));
                int fbl = intersectionBuilder.AddVertex(new(-hBWidth - _sideWalkWidth, 0, -hBWidth),new(1,1));
                
                //Front
                intersectionBuilder.AddTriangle(ttr,ttl,fbl);
                intersectionBuilder.AddTriangle(fbl,fbr,ttr);
                
                //Top
                intersectionBuilder.AddTriangle(ttl, ttr, tbr);
                intersectionBuilder.AddTriangle(tbr, tbl, ttl);
            }
            
            if (intersection.connections[Node.Direction.West] == null)
            {
                sidewalk = true;
                
                int tbl = intersectionBuilder.AddVertex(new(-hBWidth, _sideWalkHeight, -hBWidth),new(0,1)); 
                int tbr = intersectionBuilder.AddVertex(new(-hBWidth, _sideWalkHeight, hBWidth),new(1,1));
                int fbr = intersectionBuilder.AddVertex(new(-hBWidth,0, hBWidth),new(1,0));
                int fbl = intersectionBuilder.AddVertex(new(-hBWidth,0, -hBWidth),new(0,0));
                int ttl = intersectionBuilder.AddVertex(new(-hBWidth - _sideWalkWidth, _sideWalkHeight, -hBWidth), new(0,0));
                int ttr = intersectionBuilder.AddVertex(new(-hBWidth - _sideWalkWidth, _sideWalkHeight, hBWidth),new(1,0));

                //Front
                intersectionBuilder.AddTriangle(tbl,tbr,fbr);
                intersectionBuilder.AddTriangle(fbr,fbl,tbl);
                
                //Top
                intersectionBuilder.AddTriangle(ttl, ttr, tbr);
                intersectionBuilder.AddTriangle(tbr, tbl, ttl);
            }
            
            if (intersection.connections[Node.Direction.North] == null)
            {
                sidewalk = true;
                
                int tbl = intersectionBuilder.AddVertex(new(-hBWidth - _sideWalkWidth, _sideWalkHeight, hBWidth),new(0,1)); 
                int fbl = intersectionBuilder.AddVertex(new(-hBWidth - _sideWalkWidth, 0, hBWidth),new(0,0));
                int fbr = intersectionBuilder.AddVertex(new(hBWidth + _sideWalkWidth, 0, hBWidth),new(1,0));
                int tbr = intersectionBuilder.AddVertex(new(hBWidth + _sideWalkWidth, _sideWalkHeight, hBWidth),new(1,1));
                int ttl = intersectionBuilder.AddVertex(new(-hBWidth - _sideWalkWidth, _sideWalkHeight, hBWidth + _sideWalkWidth),new(0,0));
                int ttr = intersectionBuilder.AddVertex(new(hBWidth + _sideWalkWidth, _sideWalkHeight, hBWidth + _sideWalkWidth),new(1,0));
                
                //Front
                intersectionBuilder.AddTriangle(tbl,tbr,fbr);
                intersectionBuilder.AddTriangle(fbr,fbl,tbl);
                
                //Top
                intersectionBuilder.AddTriangle(ttl, ttr, tbr);
                intersectionBuilder.AddTriangle(tbr, tbl, ttl);
            }

            if (intersection.connections[Node.Direction.East] == null)
            {
                sidewalk = true;
                
                int tbl = intersectionBuilder.AddVertex(new(hBWidth, _sideWalkHeight, hBWidth),new(1,1));
                int tbr = intersectionBuilder.AddVertex(new(hBWidth, _sideWalkHeight, -hBWidth),new(0,1));
                int fbr = intersectionBuilder.AddVertex(new(hBWidth, 0, -hBWidth),new(0,0));
                int fbl = intersectionBuilder.AddVertex(new(hBWidth, 0, hBWidth),new(1,0));
                int ttl = intersectionBuilder.AddVertex(new(hBWidth + _sideWalkWidth, _sideWalkHeight, hBWidth),new(1,0));
                int ttr = intersectionBuilder.AddVertex(new(hBWidth + _sideWalkWidth, _sideWalkHeight, -hBWidth),new(0,0));
                
                //Front
                intersectionBuilder.AddTriangle(tbl,tbr,fbr);
                intersectionBuilder.AddTriangle(fbr,fbl,tbl);
                
                //Top
                intersectionBuilder.AddTriangle(ttl, ttr, tbr);
                intersectionBuilder.AddTriangle(tbr, tbl, ttl);
            }

            if (sidewalk)
            {
                GameObject intersectionSidewalkObject = new("Sidewalk");
                intersectionSidewalkObject.transform.parent = intersectionObject.transform;
                intersectionSidewalkObject.transform.localPosition = Vector3.zero;

                intersectionSidewalkObject.AddComponent<MeshRenderer>().material = _sideWalkMaterial;
                intersectionSidewalkObject.AddComponent<MeshFilter>().mesh = intersectionBuilder.CreateMesh();
            }
        }
    }
    
    private void AddVertices(MeshBuilder pMeshBuilder, Vector3[] pVertices)
    {
        foreach (Vector3 vertex in pVertices) pMeshBuilder.AddVertex(vertex);
    }
}
