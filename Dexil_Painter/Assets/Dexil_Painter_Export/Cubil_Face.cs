using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Cubil_Face : MonoBehaviour
{
    Mesh CubilMesh;
    GameObject Cubil;

    Quad intesectingQuad, SelectedQuad, ConvexQuad;

    public Quad BigBoy;

    List<Quad> IntersectingQuadList = new List<Quad>(); List<Quad> antiQuadList = new List<Quad>();

    List<Vertex> intersectingVertices = new List<Vertex>();
    List<Vertex> antiVertices = new List<Vertex>();
    List<Vertex> segmentIntersectionRightVertices = new List<Vertex>();
    List<Vertex> segmentIntersectionLeftVertices = new List<Vertex>();
    List<Vertex> segmentIntersectionBottomVertices = new List<Vertex>();
    List<Vertex> segmentIntersectionTopVertices = new List<Vertex>();

    List<int> indexLeft = new List<int>();
    List<int> indexRight = new List<int>();

    bool UPDATE;
   
    public bool ColorRegions, DEBUG;

    public Color QuadFaceColor;

    public List<Quad> QuadList = new List<Quad>();

    public float TotalArea;

    [Range(0, 10)]
    public int iterations;

    public Color[] customColors;


    private Vertex BigBoyBottomLeft, BigBoyBottomRight, 
                   BigBoyRightSideBottom, BigBoyRightSideTop,
                   BigBoyLeftSideBottom, BigBoyLeftSideTop,
                   BigBoyTopLeft, BigBoyTopRight;

    private Quad BBQuadLeft, BBQuadRight, BBQuadTop, BBQuadBottom;

    public int area, LargestArea;

    void Awake()
    {
        ConvexQuad = new Quad();
        SelectedQuad = Quad.create(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        CubilMesh = new Mesh();
        Cubil = FindObjectOfType<MeshFilter>().gameObject;
    }

    void Update()
    {
        UserInput();
    }

    void BigBoyAssimulation() //Handles Bigger Quads simplifying the Mesh
    {
        if (QuadList.Count != 0)
        {

            QuadList = QuadList.OrderByDescending(o => o.area).ToList();

            for (int i = 0; i < QuadList.Count; i++)
            {
                BigBoy = QuadList[i]; //if sorted right this should be the largest Area
                //BigBoy.quadColor = Color.black;
                
                BigBoy.CalculateQuadArea();
                BigBoy.CalculateCentrePoints();
                BigBoy.CalculateCentre();
        
                CheckBigBoySides(BigBoy);
            }
        }
           
    }

    void CheckBigBoySides(Quad BigBoy)
    {

        BigBoyBottomLeft      = null;
        BigBoyBottomRight     = null;

        BigBoyRightSideBottom = null;
        BigBoyRightSideTop    = null;

        BigBoyLeftSideBottom  = null;
        BigBoyLeftSideTop     = null;

        BigBoyTopLeft         = null;
        BigBoyTopRight        = null;

        segmentIntersectionRightVertices = new List<Vertex>();
        segmentIntersectionLeftVertices = new List<Vertex>();
        segmentIntersectionBottomVertices = new List<Vertex>();
        segmentIntersectionTopVertices = new List<Vertex>();

        BigBoyBottomLeft = new Vertex(new Vector3(BigBoy.vertexPoints[0].vertice.x, 0, BigBoy.vertexPoints[0].vertice.z), new Vector3(0,0,1), BigBoy.centre);
        BigBoyBottomRight = new Vertex(new Vector3(BigBoy.vertexPoints[1].vertice.x, 0, BigBoy.vertexPoints[1].vertice.z), new Vector3(0, 0, 1), BigBoy.centre);

        BigBoyRightSideBottom = new Vertex(new Vector3(16, BigBoy.vertexPoints[1].vertice.y, BigBoy.vertexPoints[0].vertice.z), new Vector3(0, 0, 1), BigBoy.centre);
        BigBoyRightSideTop = new Vertex(new Vector3(16, BigBoy.vertexPoints[3].vertice.y, BigBoy.vertexPoints[1].vertice.z), new Vector3(0, 0, 1), BigBoy.centre);

        BigBoyLeftSideBottom = new Vertex(new Vector3(0, BigBoy.vertexPoints[0].vertice.y, BigBoy.vertexPoints[0].vertice.z), new Vector3(0, 0, 1), BigBoy.centre);
        BigBoyLeftSideTop = new Vertex(new Vector3(0, BigBoy.vertexPoints[2].vertice.y, BigBoy.vertexPoints[1].vertice.z), new Vector3(0, 0, 1), BigBoy.centre);

        BigBoyTopLeft = new Vertex(new Vector3(BigBoy.vertexPoints[2].vertice.x, 16, BigBoy.vertexPoints[0].vertice.z), new Vector3(0, 0, 1), BigBoy.centre);
        BigBoyTopRight = new Vertex(new Vector3(BigBoy.vertexPoints[3].vertice.x, 16, BigBoy.vertexPoints[1].vertice.z), new Vector3(0, 0, 1), BigBoy.centre);

        if (QuadList.Count != 0)
        {
            for (int i = 0; i < QuadList.Count; i++)
            {

                if (QuadList[i] == BigBoy)
                    continue;

                Vertex bb0 = BigBoy.vertexPoints[0];
                Vertex bb1 = BigBoy.vertexPoints[1];
                Vertex bb2 = BigBoy.vertexPoints[2];
                Vertex bb3 = BigBoy.vertexPoints[3];

                //RightSide 
                EvaluateVerticalIntersection(QuadList[i], bb1, bb3, BigBoyRightSideTop, BigBoyRightSideBottom, segmentIntersectionRightVertices);

                //LeftSide 
                EvaluateVerticalIntersection(QuadList[i], bb0, bb2, BigBoyLeftSideTop, BigBoyLeftSideBottom, segmentIntersectionLeftVertices);

                //BottomSide
                EvaluateHorizontalIntersection(QuadList[i], bb0, bb1, BigBoyBottomRight, BigBoyBottomLeft, segmentIntersectionBottomVertices);

                //TopSide
                EvaluateHorizontalIntersection(QuadList[i], bb2, bb3, BigBoyTopRight, BigBoyTopLeft, segmentIntersectionTopVertices);

            }
        }

        //Determin if there are quads and if there area is worth assimulating 
        AssimilationQuads(BigBoy);
    }

    void AssimilationQuads(Quad BigBoy)
    {
        BBQuadLeft = new Quad();
        BBQuadRight = new Quad();
        BBQuadTop = new Quad();
        BBQuadBottom = new Quad();

        //------------------------------------------------------Right
        SortAndCreateAssiumulationQuad(segmentIntersectionRightVertices, BigBoy, BBQuadRight, Color.yellow);
        EvaluateAssiumulationQuadRight(segmentIntersectionRightVertices, BigBoy, BBQuadRight, Color.yellow);
        //------------------------------------------------------

        //------------------------------------------------------Left
        SortAndCreateAssiumulationQuad(segmentIntersectionLeftVertices, BigBoy, BBQuadLeft, Color.green);
        EvaluateAssiumulationQuadLeft(segmentIntersectionLeftVertices, BigBoy, BBQuadLeft, Color.green);
        //------------------------------------------------------Left

        //------------------------------------------------------Top
        SortAndCreateAssiumulationQuad(segmentIntersectionTopVertices, BigBoy, BBQuadTop, Color.red);
        EvaluateAssiumulationQuadTop(segmentIntersectionTopVertices, BigBoy, BBQuadTop, Color.red);
        //------------------------------------------------------Top

        //------------------------------------------------------Bottom
        SortAndCreateAssiumulationQuad(segmentIntersectionBottomVertices, BigBoy, BBQuadBottom, Color.blue);
        EvaluateAssiumulationQuadBottom(segmentIntersectionBottomVertices, BigBoy, BBQuadBottom, Color.blue);
        //------------------------------------------------------Bottom

        //Quads should be done and evaulated and ready to be checked at this point
        //asking wether we should be going verticall or horizontal

        int Left = BBQuadLeft.area;
        int Right = BBQuadRight.area;

        int Top = BBQuadTop.area;
        int Bottom = BBQuadBottom.area;

        int vertical = Top + Bottom + BigBoy.area;
        int horizontal = Left + Right + BigBoy.area;

        if (horizontal > vertical) 
        {
           if (BBQuadRight.area != 0)
           RightQuadAssimulate(horizontal);
           
           if (BBQuadLeft.area != 0)
           LeftQuadAssimulate(horizontal);
        }

         if(horizontal < vertical)
        {
            if (BBQuadTop.area != 0)
               TopQuadAssimulate(vertical);

            if (BBQuadBottom.area != 0)
               BottomQuadAssimulate(vertical);
        }

    }

    void RightQuadAssimulate(int area)
    {
        List<Quad> intersectingQuads = new List<Quad>();

        //Finding All Quads currently being effected by assimulation

        BBQuadRight.CalculateQuadArea();
        BBQuadRight.CalculateCentre();
        BBQuadRight.CalculateCentrePoints();

        for (int i = 0; i < QuadList.Count; i++)
        {
            if (QuadList[i] == BigBoy)
                continue;

            for (int j = 0; j < BBQuadRight.centrePoints.Count; j++)
            {
                if (QuadList[i].inFace(BBQuadRight.centrePoints[j]))
                {
                    if (!intersectingQuads.Contains(QuadList[i])) intersectingQuads.Add(QuadList[i]);
                }
            }
        }

        //MakeSure Non of the intersecting quads will be larger then the new quad
        //since bigger is better

        if(intersectingQuads.Count != 0)
        {
            for(int i = 0; i < intersectingQuads.Count; i++)
            {
                if (intersectingQuads[i].area >= area)
                    return;
            }
        }


        //Check the conditions of the quads

        //All quads vertices completly outside
        List<Quad> quadstoCut = new List<Quad>();

        //Some quads vertices inside 
        List<Quad> quadstoPush = new List<Quad>();

        //All quads vertices inside
        List<Quad> quadstoRemove = new List<Quad>();

        //Corner in the Quads
        List<Quad> quadstoSplit = new List<Quad>();


        if (intersectingQuads.Count != 0)
        {
            //Find and Sort Quads into there respective lists based on vertice information
            for (int i = 0; i < intersectingQuads.Count; i++)
            {
                int vertexPointCount = 0;

                for (int j = 0; j < intersectingQuads[i].vertexPoints.Length; j++)
                {
                    if (BBQuadRight.inFace(intersectingQuads[i].vertexPoints[j].vertice))
                    {
                        vertexPointCount++;
                    }
                }

                if (vertexPointCount == 4)
                {
                    if (!quadstoRemove.Contains(intersectingQuads[i])) quadstoRemove.Add(intersectingQuads[i]);
                }

                if (vertexPointCount != 0 && vertexPointCount < 3)
                {
                    if (!quadstoPush.Contains(intersectingQuads[i])) quadstoPush.Add(intersectingQuads[i]);
                }

                if (vertexPointCount == 0)
                {
                    if (!quadstoCut.Contains(intersectingQuads[i])) quadstoCut.Add(intersectingQuads[i]);
                }

                if (vertexPointCount == 1)
                {
                    if (!quadstoSplit.Contains(intersectingQuads[i])) quadstoSplit.Add(intersectingQuads[i]);
                }
            }

            //Push Quads who are only slightly inside the Assimulation Quad
            if (quadstoPush.Count != 0)
            {
                for (int i = 0; i < quadstoPush.Count; i++)
                {
                    Vector3 vert0 = quadstoPush[i].vertexPoints[0].vertice;
                    Vector3 vert1 = quadstoPush[i].vertexPoints[1].vertice;
                    Vector3 vert2 = quadstoPush[i].vertexPoints[2].vertice;
                    Vector3 vert3 = quadstoPush[i].vertexPoints[3].vertice;

                    //This is Bottom
                    if (BBQuadRight.inFace(vert0) && BBQuadRight.inFace(vert1))
                    {
                        quadstoPush[i].vertexPoints[0].vertice.y = BBQuadRight.vertexPoints[2].vertice.y;
                        quadstoPush[i].vertexPoints[1].vertice.y = BBQuadRight.vertexPoints[2].vertice.y;
                    }
                    //This is Top
                    if (BBQuadRight.inFace(vert2) && BBQuadRight.inFace(vert3))
                    {
                        quadstoPush[i].vertexPoints[2].vertice.y = BBQuadRight.vertexPoints[0].vertice.y;
                        quadstoPush[i].vertexPoints[3].vertice.y = BBQuadRight.vertexPoints[0].vertice.y;
                    }
                    //This is Left
                    if (BBQuadRight.inFace(vert0) && BBQuadRight.inFace(vert2))
                    {
                        quadstoPush[i].vertexPoints[2].vertice.x = BBQuadRight.vertexPoints[1].vertice.x;
                        quadstoPush[i].vertexPoints[0].vertice.x = BBQuadRight.vertexPoints[1].vertice.x;
                    }
                }
            }

            //Cut Quads who are larger either in X or Y inside the Assimulation Quad
            if (quadstoCut.Count != 0)
            {
                for (int i = 0; i < quadstoCut.Count; i++)
                {
                    //quad to cut will always have a top and a bottom quad so EZ
                    //Top Quad 
                    Quad topQuad = new Quad(new Vector3(0, 0, 0), quadstoCut[i].vertexPoints[0].normal);

                    topQuad.vertexPoints[0].vertice = new Vector3(quadstoCut[i].vertexPoints[0].vertice.x, BBQuadRight.vertexPoints[2].vertice.y, quadstoCut[i].vertexPoints[0].vertice.z);
                    topQuad.vertexPoints[1].vertice = new Vector3(quadstoCut[i].vertexPoints[1].vertice.x, BBQuadRight.vertexPoints[2].vertice.y, quadstoCut[i].vertexPoints[1].vertice.z);
                    topQuad.vertexPoints[2] = quadstoCut[i].vertexPoints[2];
                    topQuad.vertexPoints[3] = quadstoCut[i].vertexPoints[3];

                    topQuad.CalculateQuadArea();
                    topQuad.CalculateCentre();
                    topQuad.CalculateCentrePoints();

                    //Bottom Quad
                    Quad bottomQuad = new Quad(new Vector3(0, 0, 0), quadstoCut[i].vertexPoints[0].normal);

                    bottomQuad.vertexPoints[0] = quadstoCut[i].vertexPoints[0];
                    bottomQuad.vertexPoints[1] = quadstoCut[i].vertexPoints[1];
                    bottomQuad.vertexPoints[2].vertice = new Vector3(quadstoCut[i].vertexPoints[2].vertice.x, BBQuadRight.vertexPoints[1].vertice.y, quadstoCut[i].vertexPoints[2].vertice.z);
                    bottomQuad.vertexPoints[3].vertice = new Vector3(quadstoCut[i].vertexPoints[3].vertice.x, BBQuadRight.vertexPoints[1].vertice.y, quadstoCut[i].vertexPoints[3].vertice.z);

                    bottomQuad.CalculateQuadArea();
                    bottomQuad.CalculateCentre();
                    bottomQuad.CalculateCentrePoints();

                    if (QuadList.Contains(quadstoCut[i])) QuadList.Remove(quadstoCut[i]);

                    if (!QuadList.Contains(bottomQuad)) QuadList.Add(bottomQuad);
                    if (!QuadList.Contains(topQuad)) QuadList.Add(topQuad);
                }
            }

            //Seperate split Quad list beased on vertices based on which vertices is within the Quad
            List<Quad> splitTop = new List<Quad>();
            List<Quad> splitBottom = new List<Quad>();


            Vector3 splitTopPoint = new Vector3();
            Vector3 splitBottomPoint = new Vector3();

            //Seperate Quads into Top and Bottom Quads
   
                for (int i = 0; i < quadstoSplit.Count; i++)
                {
                    Vertex p0 = quadstoSplit[i].vertexPoints[0];
                    Vertex p1 = quadstoSplit[i].vertexPoints[1];
                    Vertex p2 = quadstoSplit[i].vertexPoints[2];
                    Vertex p3 = quadstoSplit[i].vertexPoints[3];


                    //Top Check
                    if (BBQuadRight.inFace(p0.vertice))
                    {
                        if (!splitTop.Contains(quadstoSplit[i])) splitTop.Add(quadstoSplit[i]);
                    }

                    if (BBQuadRight.inFace(p1.vertice))
                    {
                        if (!splitTop.Contains(quadstoSplit[i])) splitTop.Add(quadstoSplit[i]);
                    }

                    //bottom Check
                    if (BBQuadRight.inFace(p2.vertice))
                    {
                        if (!splitBottom.Contains(quadstoSplit[i])) splitBottom.Add(quadstoSplit[i]);
                    }

                    if(BBQuadRight.inFace(p3.vertice))
                    {
                        if (!splitBottom.Contains(quadstoSplit[i])) splitBottom.Add(quadstoSplit[i]);
                    }

                }

                // Debug.Log("Top Point " + splitTopPoint);
                //Debug.Log("Bottom Point " + splitBottomPoint);

                //Splitting the Top Quad at the Point 3
                if (splitTop.Count != 0)
                {

                splitTopPoint = FindPoint3(splitTop, BBQuadRight);

                for (int i = 0; i < splitTop.Count; i++)
                    {
                        splitTop[i].CalculateQuadArea();
                        splitTop[i].CalculateCentre();
                        splitTop[i].CalculateCentrePoints();

                        intesectingQuad = splitTop[i];

                        FractureQuads(splitTopPoint);
                        //break;
                    }
                }

                if (splitBottom.Count != 0)
                {

                splitBottomPoint = FindPoint1(splitBottom, BBQuadRight);

                for (int j = 0; j < splitBottom.Count; j++)
                    {
                        splitBottom[j].CalculateQuadArea();
                        splitBottom[j].CalculateCentre();
                        splitBottom[j].CalculateCentrePoints();

                        intesectingQuad = splitBottom[j];

                        FractureQuads(splitBottomPoint);
                        // break;
                    }
                }


                for (int i = 0; i < QuadList.Count; i++)
                {
                    if (QuadList[i] == BigBoy)
                        continue;

                    for (int j = 0; j < BBQuadRight.centrePoints.Count; j++)
                    {
                        if (QuadList[i].inFace(BBQuadRight.centrePoints[j]))
                        {
                            if (!intersectingQuads.Contains(QuadList[i])) intersectingQuads.Add(QuadList[i]);
                        }
                    }
                }

                //Find and Sort Quads into there respective lists based on vertice information
                for (int i = 0; i < intersectingQuads.Count; i++)
                {
                    int vertexPointCount = 0;

                    for (int j = 0; j < intersectingQuads[i].vertexPoints.Length; j++)
                    {
                        if (BBQuadRight.inFace(intersectingQuads[i].vertexPoints[j].vertice))
                        {
                            vertexPointCount++;

                        }
                    }

                    if (vertexPointCount == 4)
                    {
                        if (!quadstoRemove.Contains(intersectingQuads[i])) quadstoRemove.Add(intersectingQuads[i]);
                    }
                }

                //Removing Quads Who are Completly inside the Assimulation Quad
                RemoveQuads(quadstoRemove);
            

            if (BBQuadRight != null)
            {
                BigBoy.vertexPoints[1].vertice = BBQuadRight.vertexPoints[1].vertice;
                BigBoy.vertexPoints[3].vertice = BBQuadRight.vertexPoints[3].vertice;

                BBQuadRight = null;
                segmentIntersectionRightVertices = new List<Vertex>();

                BigBoy.CalculateQuadArea();
                BigBoy.CalculateCentre();
                BigBoy.CalculateCentrePoints();
            }
        }
    }

    void LeftQuadAssimulate(int area)
    {
        List<Quad> intersectingQuads = new List<Quad>();

        BBQuadLeft.CalculateQuadArea();
        BBQuadLeft.CalculateCentre();
        BBQuadLeft.CalculateCentrePoints();

        //Finding All Quads currently being effected by assimulation

        for (int i = 0; i < QuadList.Count; i++)
        {
            if (QuadList[i] == BigBoy)
                continue;

            for (int j = 0; j < BBQuadLeft.centrePoints.Count; j++)
            {
                if (QuadList[i].inFace(BBQuadLeft.centrePoints[j]))
                {
                    if (!intersectingQuads.Contains(QuadList[i])) intersectingQuads.Add(QuadList[i]);
                }
            }
        }


        if (intersectingQuads.Count != 0)
        {
            for (int i = 0; i < intersectingQuads.Count; i++)
            {
                if (intersectingQuads[i].area >= area)
                    return;
            }
        }

        //Check the conditions of the quads

        //All quads vertices completly outside
        List<Quad> quadstoCut = new List<Quad>();

        //Some quads vertices inside 
        List<Quad> quadstoPush = new List<Quad>();

        //All quads vertices inside
        List<Quad> quadstoRemove = new List<Quad>();

        //All quads vertices inside
        List<Quad> quadstoSplit = new List<Quad>();


        if (intersectingQuads.Count != 0)
        {
            //Find and Sort Quads into there respective lists based on vertice information
            for (int i = 0; i < intersectingQuads.Count; i++)
            {
                int vertexPointCount = 0;

                for (int j = 0; j < intersectingQuads[i].vertexPoints.Length; j++)
                {
                    if (BBQuadLeft.inFace(intersectingQuads[i].vertexPoints[j].vertice))
                    {
                        vertexPointCount++;

                    }
                }

                if (vertexPointCount == 4)
                {
                    if (!quadstoRemove.Contains(intersectingQuads[i])) quadstoRemove.Add(intersectingQuads[i]);
                }

                if (vertexPointCount != 0 && vertexPointCount < 3)
                {
                    if (!quadstoPush.Contains(intersectingQuads[i])) quadstoPush.Add(intersectingQuads[i]);
                }

                if (vertexPointCount == 0)
                {
                    if (!quadstoCut.Contains(intersectingQuads[i])) quadstoCut.Add(intersectingQuads[i]);
                }

                if (vertexPointCount == 1)
                {
                    if (!quadstoSplit.Contains(intersectingQuads[i])) quadstoSplit.Add(intersectingQuads[i]);
                }
            }

            //Push Quads who are only slightly inside the Assimulation Quad
            if (quadstoPush.Count != 0)
            {
                for (int i = 0; i < quadstoPush.Count; i++)
                {

                    Vector3 vert0 = quadstoPush[i].vertexPoints[0].vertice;
                    Vector3 vert1 = quadstoPush[i].vertexPoints[1].vertice;
                    Vector3 vert2 = quadstoPush[i].vertexPoints[2].vertice;
                    Vector3 vert3 = quadstoPush[i].vertexPoints[3].vertice;

                    //This is Bottom
                    if (BBQuadLeft.inFace(vert0) && BBQuadLeft.inFace(vert1))
                    {
                        quadstoPush[i].vertexPoints[0].vertice.y = BBQuadLeft.vertexPoints[2].vertice.y;
                        quadstoPush[i].vertexPoints[1].vertice.y = BBQuadLeft.vertexPoints[2].vertice.y;
                    }
                    //This is Top
                    if (BBQuadLeft.inFace(vert2) && BBQuadLeft.inFace(vert3))
                    {
                        quadstoPush[i].vertexPoints[2].vertice.y = BBQuadLeft.vertexPoints[0].vertice.y;
                        quadstoPush[i].vertexPoints[3].vertice.y = BBQuadLeft.vertexPoints[0].vertice.y;
                    }
                    //This is Right
                    if (BBQuadLeft.inFace(vert1) && BBQuadLeft.inFace(vert3))
                    {
                        quadstoPush[i].vertexPoints[3].vertice.x = BBQuadLeft.vertexPoints[0].vertice.x;
                        quadstoPush[i].vertexPoints[1].vertice.x = BBQuadLeft.vertexPoints[0].vertice.x;
                    }

                }
            }

            //Cut Quads who are larger either in X or Y inside the Assimulation Quad
            if (quadstoCut.Count != 0)
            {
                for (int i = 0; i < quadstoCut.Count; i++)
                {
                    //quad to cut will always have a top and a bottom quad so EZ
                    //Top Quad 
                    Quad topQuad = new Quad(new Vector3(0, 0, 0), quadstoCut[i].vertexPoints[0].normal);

                    topQuad.vertexPoints[0].vertice = new Vector3(quadstoCut[i].vertexPoints[0].vertice.x, BBQuadLeft.vertexPoints[2].vertice.y, quadstoCut[i].vertexPoints[0].vertice.z);
                    topQuad.vertexPoints[1].vertice = new Vector3(quadstoCut[i].vertexPoints[1].vertice.x, BBQuadLeft.vertexPoints[2].vertice.y, quadstoCut[i].vertexPoints[1].vertice.z);
                    topQuad.vertexPoints[2] = quadstoCut[i].vertexPoints[2];
                    topQuad.vertexPoints[3] = quadstoCut[i].vertexPoints[3];

                    topQuad.CalculateQuadArea();
                    topQuad.CalculateCentre();
                    topQuad.CalculateCentrePoints();

                    //Bottom Quad
                    Quad bottomQuad = new Quad(new Vector3(0, 0, 0), quadstoCut[i].vertexPoints[0].normal);

                    bottomQuad.vertexPoints[0] = quadstoCut[i].vertexPoints[0];
                    bottomQuad.vertexPoints[1] = quadstoCut[i].vertexPoints[1];
                    bottomQuad.vertexPoints[2].vertice = new Vector3(quadstoCut[i].vertexPoints[2].vertice.x, BBQuadLeft.vertexPoints[1].vertice.y, quadstoCut[i].vertexPoints[2].vertice.z);
                    bottomQuad.vertexPoints[3].vertice = new Vector3(quadstoCut[i].vertexPoints[3].vertice.x, BBQuadLeft.vertexPoints[1].vertice.y, quadstoCut[i].vertexPoints[3].vertice.z);

                    bottomQuad.CalculateQuadArea();
                    bottomQuad.CalculateCentre();
                    bottomQuad.CalculateCentrePoints();

                    if (QuadList.Contains(quadstoCut[i])) QuadList.Remove(quadstoCut[i]);

                    if (!QuadList.Contains(bottomQuad)) QuadList.Add(bottomQuad);
                    if (!QuadList.Contains(topQuad)) QuadList.Add(topQuad);
                }
            }

            //Seperate split Quad list beased on vertices based on which vertices is within the Quad
            List<Quad> splitTop = new List<Quad>();
            List<Quad> splitBottom = new List<Quad>();


            Vector3 splitTopPoint = new Vector3();
            Vector3 splitBottomPoint = new Vector3();

            //Seperate Quads into Top and Bottom Quads
            if (quadstoSplit.Count != 0)
            {
                for (int i = 0; i < quadstoSplit.Count; i++)
                {
                    Vertex p0 = quadstoSplit[i].vertexPoints[0];
                    Vertex p1 = quadstoSplit[i].vertexPoints[1];
                    Vertex p2 = quadstoSplit[i].vertexPoints[2];
                    Vertex p3 = quadstoSplit[i].vertexPoints[3];


                    //Top Check
                    if (BBQuadLeft.inFace(p0.vertice))
                    {
                        if (!splitTop.Contains(quadstoSplit[i])) splitTop.Add(quadstoSplit[i]);
                    }

                    if(BBQuadLeft.inFace(p1.vertice))
                    {
                        if (!splitTop.Contains(quadstoSplit[i])) splitTop.Add(quadstoSplit[i]);
                    }

                    //bottom Check
                    if (BBQuadLeft.inFace(p2.vertice))
                    {
                        if (!splitBottom.Contains(quadstoSplit[i])) splitBottom.Add(quadstoSplit[i]);
                    }

                    if (BBQuadLeft.inFace(p3.vertice))
                    {
                        if (!splitBottom.Contains(quadstoSplit[i])) splitBottom.Add(quadstoSplit[i]);
                    }

                }
            }

            // Debug.Log("Top Point " + splitTopPoint);
            //Debug.Log("Bottom Point " + splitBottomPoint);

            //Splitting the Top Quad at the Point 3

            if (splitTop.Count != 0)
            {
                splitTopPoint = FindPoint2(splitTop, BBQuadLeft);

                for (int i = 0; i < splitTop.Count; i++)
                {
                    splitTop[i].CalculateQuadArea();
                    splitTop[i].CalculateCentre();
                    splitTop[i].CalculateCentrePoints();

                    intesectingQuad = splitTop[i];

                    FractureQuads(splitTopPoint);
                    //break;
                }
            }

            if (splitBottom.Count != 0)
            {
                splitBottomPoint = FindPoint0(splitBottom, BBQuadLeft);

                for (int j = 0; j < splitBottom.Count; j++)
                {
                    splitBottom[j].CalculateQuadArea();
                    splitBottom[j].CalculateCentre();
                    splitBottom[j].CalculateCentrePoints();

                    intesectingQuad = splitBottom[j];

                    FractureQuads(splitBottomPoint);
                    // break;
                }
            }


            for (int i = 0; i < QuadList.Count; i++)
            {
                if (QuadList[i] == BigBoy)
                    continue;

                for (int j = 0; j < BBQuadLeft.centrePoints.Count; j++)
                {
                    if (QuadList[i].inFace(BBQuadLeft.centrePoints[j]))
                    {
                        if (!intersectingQuads.Contains(QuadList[i])) intersectingQuads.Add(QuadList[i]);
                    }
                }
            }

            //Find and Sort Quads into there respective lists based on vertice information
            for (int i = 0; i < intersectingQuads.Count; i++)
            {
                int vertexPointCount = 0;

                for (int j = 0; j < intersectingQuads[i].vertexPoints.Length; j++)
                {
                    if (BBQuadLeft.inFace(intersectingQuads[i].vertexPoints[j].vertice))
                    {
                        vertexPointCount++;

                    }
                }

                if (vertexPointCount == 4)
                {
                    if (!quadstoRemove.Contains(intersectingQuads[i])) quadstoRemove.Add(intersectingQuads[i]);
                }
            }

            //Removing Quads Who are Completly inside the Assimulation Quad
            RemoveQuads(quadstoRemove);


            if (BBQuadLeft != null)
            {
                BigBoy.vertexPoints[0].vertice = BBQuadLeft.vertexPoints[0].vertice;
                BigBoy.vertexPoints[2].vertice = BBQuadLeft.vertexPoints[2].vertice;

                BBQuadLeft = null;
                segmentIntersectionLeftVertices = new List<Vertex>();

                BigBoy.CalculateQuadArea();
                BigBoy.CalculateCentre();
                BigBoy.CalculateCentrePoints();
            }
        }

    }

    void TopQuadAssimulate(int area)
    {
        List<Quad> intersectingQuads = new List<Quad>();

        //Finding All Quads currently being effected by assimulation

        BBQuadTop.CalculateQuadArea();
        BBQuadTop.CalculateCentre();
        BBQuadTop.CalculateCentrePoints();

        for (int i = 0; i < QuadList.Count; i++)
        {
            if (QuadList[i] == BigBoy)
                continue;

            for (int j = 0; j < BBQuadTop.centrePoints.Count; j++)
            {
                if (QuadList[i].inFace(BBQuadTop.centrePoints[j]))
                {
                    if (!intersectingQuads.Contains(QuadList[i])) intersectingQuads.Add(QuadList[i]);
                }
            }
        }


        if (intersectingQuads.Count != 0)
        {
            for (int i = 0; i < intersectingQuads.Count; i++)
            {
                if (intersectingQuads[i].area >= area)
                    return;
            }
        }

        //Check the conditions of the quads

        //All quads vertices completly outside
        List<Quad> quadstoCut = new List<Quad>();

        //Some quads vertices inside 
        List<Quad> quadstoPush = new List<Quad>();

        //All quads vertices inside
        List<Quad> quadstoRemove = new List<Quad>();

        //Debug.Log(intersectingQuads.Count);
        List<Quad> quadstoSplit = new List<Quad>();


        if (intersectingQuads.Count != 0)
        {
            for (int i = 0; i < intersectingQuads.Count; i++)
            {
                int vertexPointCount = 0;

                for (int j = 0; j < intersectingQuads[i].vertexPoints.Length; j++)
                {
                    if (BBQuadTop.inFace(intersectingQuads[i].vertexPoints[j].vertice))
                    {
                        vertexPointCount++;
                    }
                }

                if (vertexPointCount == 4)
                {
                    if (!quadstoRemove.Contains(intersectingQuads[i])) quadstoRemove.Add(intersectingQuads[i]);
                }

                if (vertexPointCount != 0 && vertexPointCount < 3)
                {
                    if (!quadstoPush.Contains(intersectingQuads[i])) quadstoPush.Add(intersectingQuads[i]);
                }

                if (vertexPointCount == 0)
                {
                    if (!quadstoCut.Contains(intersectingQuads[i])) quadstoCut.Add(intersectingQuads[i]);
                }
                if (vertexPointCount == 1)
                {
                    if (!quadstoSplit.Contains(intersectingQuads[i])) quadstoSplit.Add(intersectingQuads[i]);
                }
            }

            //Push Quads who are only slightly inside the Assimulation Quad
            if (quadstoPush.Count != 0)
            {
                for (int i = 0; i < quadstoPush.Count; i++)
                {

                    Vector3 vert0 = quadstoPush[i].vertexPoints[0].vertice;
                    Vector3 vert1 = quadstoPush[i].vertexPoints[1].vertice;
                    Vector3 vert2 = quadstoPush[i].vertexPoints[2].vertice;
                    Vector3 vert3 = quadstoPush[i].vertexPoints[3].vertice;

                    //This is Bottom
                    if (BBQuadTop.inFace(vert0) && BBQuadTop.inFace(vert1))
                    {
                        quadstoPush[i].vertexPoints[0].vertice.y = BBQuadTop.vertexPoints[2].vertice.y;
                        quadstoPush[i].vertexPoints[1].vertice.y = BBQuadTop.vertexPoints[2].vertice.y;
                    }

                    //This is Left
                    if (BBQuadTop.inFace(vert0) && BBQuadTop.inFace(vert2))
                    {
                        quadstoPush[i].vertexPoints[0].vertice.x = BBQuadTop.vertexPoints[1].vertice.x;
                        quadstoPush[i].vertexPoints[2].vertice.x = BBQuadTop.vertexPoints[1].vertice.x;
                    }
                    //This is Right
                    if (BBQuadTop.inFace(vert1) && BBQuadTop.inFace(vert3))
                    {
                        quadstoPush[i].vertexPoints[3].vertice.x = BBQuadTop.vertexPoints[0].vertice.x;
                        quadstoPush[i].vertexPoints[1].vertice.x = BBQuadTop.vertexPoints[0].vertice.x;
                    }

                }
            }

            //Cut Quads who are larger either in X or Y inside the Assimulation Quad
            if (quadstoCut.Count != 0)
            {
                for (int i = 0; i < quadstoCut.Count; i++)
                {
                    //quad to cut will always have a top and a bottom quad so EZ
                    //Top Quad 
                    Quad leftQuad = new Quad(new Vector3(0, 0, 0), quadstoCut[i].vertexPoints[0].normal);

                    leftQuad.vertexPoints[0].vertice = new Vector3(BBQuadTop.vertexPoints[1].vertice.x, quadstoCut[i].vertexPoints[0].vertice.y, quadstoCut[i].vertexPoints[0].vertice.z);
                    leftQuad.vertexPoints[1] = quadstoCut[i].vertexPoints[1];
                    leftQuad.vertexPoints[2].vertice = new Vector3(BBQuadTop.vertexPoints[1].vertice.x, quadstoCut[i].vertexPoints[2].vertice.y, quadstoCut[i].vertexPoints[0].vertice.z);
                    leftQuad.vertexPoints[3] = quadstoCut[i].vertexPoints[3];

                    leftQuad.CalculateQuadArea();
                    leftQuad.CalculateCentre();
                    leftQuad.CalculateCentrePoints();

                    //Bottom Quad
                    Quad rightQuad = new Quad(new Vector3(0, 0, 0), quadstoCut[i].vertexPoints[0].normal);

                    rightQuad.vertexPoints[0] = quadstoCut[i].vertexPoints[0];
                    rightQuad.vertexPoints[1].vertice = new Vector3(BBQuadTop.vertexPoints[0].vertice.x, quadstoCut[i].vertexPoints[1].vertice.y, quadstoCut[i].vertexPoints[0].vertice.z);
                    rightQuad.vertexPoints[2] = quadstoCut[i].vertexPoints[2];
                    rightQuad.vertexPoints[3].vertice = new Vector3(BBQuadTop.vertexPoints[0].vertice.x, quadstoCut[i].vertexPoints[3].vertice.y, quadstoCut[i].vertexPoints[0].vertice.z);


                    rightQuad.CalculateQuadArea();
                    rightQuad.CalculateCentre();
                    rightQuad.CalculateCentrePoints();

                    if (QuadList.Contains(quadstoCut[i])) QuadList.Remove(quadstoCut[i]);

                    if (!QuadList.Contains(rightQuad)) QuadList.Add(rightQuad);
                    if (!QuadList.Contains(leftQuad)) QuadList.Add(leftQuad);
                }
            }


            //Seperate split Quad list beased on vertices based on which vertices is within the Quad
            List<Quad> splitLeft = new List<Quad>();
            List<Quad> splitRight = new List<Quad>();


            Vector3 splitLeftPoint = new Vector3();
            Vector3 splitRightPoint = new Vector3();

            //Seperate Quads into Top and Bottom Quads
            if (quadstoSplit.Count != 0)
            {
                for (int i = 0; i < quadstoSplit.Count; i++)
                {
                    Vertex p0 = quadstoSplit[i].vertexPoints[0];
                    Vertex p1 = quadstoSplit[i].vertexPoints[1];
                    Vertex p2 = quadstoSplit[i].vertexPoints[2];
                    Vertex p3 = quadstoSplit[i].vertexPoints[3];


                    //Top Check
                    if (BBQuadTop.inFace(p1.vertice))
                    {
                        if (!splitLeft.Contains(quadstoSplit[i])) splitLeft.Add(quadstoSplit[i]);
                    }

                    if (BBQuadTop.inFace(p3.vertice))
                    {
                        if (!splitLeft.Contains(quadstoSplit[i])) splitLeft.Add(quadstoSplit[i]);
                    }

                    //bottom Check
                    if (BBQuadTop.inFace(p0.vertice))
                    {
                        if (!splitRight.Contains(quadstoSplit[i])) splitRight.Add(quadstoSplit[i]);
                    }

                    if (BBQuadTop.inFace(p2.vertice))
                    {
                        if (!splitRight.Contains(quadstoSplit[i])) splitRight.Add(quadstoSplit[i]);
                    }

                }
            }

            //Splitting the Top Quad at the Point 3
            if (splitLeft.Count != 0)
            {
                splitLeftPoint = FindPoint2(splitLeft, BBQuadTop);

                for (int i = 0; i < splitLeft.Count; i++)
                {
                    splitLeft[i].CalculateQuadArea();
                    splitLeft[i].CalculateCentre();
                    splitLeft[i].CalculateCentrePoints();

                    intesectingQuad = splitLeft[i];

                    FractureQuads(splitLeftPoint);
                    //break;
                }
            }

            if (splitRight.Count != 0)
            {
                splitRightPoint = FindPoint3(splitRight, BBQuadTop);

                for (int j = 0; j < splitRight.Count; j++)
                {
                    splitRight[j].CalculateQuadArea();
                    splitRight[j].CalculateCentre();
                    splitRight[j].CalculateCentrePoints();

                    intesectingQuad = splitRight[j];

                    FractureQuads(splitRightPoint);
                    // break;
                }
            }

            for (int i = 0; i < QuadList.Count; i++)
            {
                if (QuadList[i] == BigBoy)
                    continue;

                for (int j = 0; j < BBQuadTop.centrePoints.Count; j++)
                {
                    if (QuadList[i].inFace(BBQuadTop.centrePoints[j]))
                    {
                        if (!intersectingQuads.Contains(QuadList[i])) intersectingQuads.Add(QuadList[i]);
                    }
                }
            }

            //Find and Sort Quads into there respective lists based on vertice information
            for (int i = 0; i < intersectingQuads.Count; i++)
            {
                int vertexPointCount = 0;

                for (int j = 0; j < intersectingQuads[i].vertexPoints.Length; j++)
                {
                    if (BBQuadTop.inFace(intersectingQuads[i].vertexPoints[j].vertice))
                    {
                        vertexPointCount++;

                    }
                }

                if (vertexPointCount == 4)
                {
                    if (!quadstoRemove.Contains(intersectingQuads[i])) quadstoRemove.Add(intersectingQuads[i]);
                }
            }

            //Removing Quads Who are Completly inside the Assimulation Quad
            RemoveQuads(quadstoRemove);


            if (BBQuadTop != null)
            {
                BigBoy.vertexPoints[2].vertice = BBQuadTop.vertexPoints[2].vertice;
                BigBoy.vertexPoints[3].vertice = BBQuadTop.vertexPoints[3].vertice;

                BBQuadTop = null;
                segmentIntersectionTopVertices = new List<Vertex>();

                BigBoy.CalculateQuadArea();
                BigBoy.CalculateCentre();
                BigBoy.CalculateCentrePoints();
            }
        }
    }

    void BottomQuadAssimulate(int area)
    {
        List<Quad> intersectingQuads = new List<Quad>();
        
        BBQuadBottom.CalculateQuadArea();
        BBQuadBottom.CalculateCentre();
        BBQuadBottom.CalculateCentrePoints();
        
        //Finding All Quads currently being effected by assimulation

        for (int i = 0; i < QuadList.Count; i++)
        {
            if (QuadList[i] == BigBoy)
                continue;

            for (int j = 0; j < BBQuadBottom.centrePoints.Count; j++)
            {
                if (QuadList[i].inFace(BBQuadBottom.centrePoints[j]))
                {
                    QuadList[i].quadColor = Color.blue;
                    if (!intersectingQuads.Contains(QuadList[i])) intersectingQuads.Add(QuadList[i]);
                }
            }
        }

        if (intersectingQuads.Count != 0)
        {
            for (int i = 0; i < intersectingQuads.Count; i++)
            {
                if (intersectingQuads[i].area >= area)
                    return;
            }
        }
        

        //Check the conditions of the quads

        //All quads vertices completly outside
        List<Quad> quadstoCut = new List<Quad>();

        //Some quads vertices inside 
        List<Quad> quadstoPush = new List<Quad>();

        //All quads vertices inside
        List<Quad> quadstoRemove = new List<Quad>();

        List<Quad> quadstoSplit = new List<Quad>();
        //Debug.Log(intersectingQuads.Count);

        if (intersectingQuads.Count != 0)
        {

            for (int i = 0; i < intersectingQuads.Count; i++)
            {
                int vertexPointCount = 0;

                for (int j = 0; j < intersectingQuads[i].vertexPoints.Length; j++)
                {
                    if (BBQuadBottom.inFace(intersectingQuads[i].vertexPoints[j].vertice))
                    {
                        vertexPointCount++;
                    }
                }

                if (vertexPointCount == 4)
                {
                    if (!quadstoRemove.Contains(intersectingQuads[i])) quadstoRemove.Add(intersectingQuads[i]);
                }

                if (vertexPointCount != 0 && vertexPointCount < 3)
                {
                    if (!quadstoPush.Contains(intersectingQuads[i])) quadstoPush.Add(intersectingQuads[i]);
                }

                if (vertexPointCount == 0)
                {
                    if (!quadstoCut.Contains(intersectingQuads[i])) quadstoCut.Add(intersectingQuads[i]);
                }

                if (vertexPointCount == 1)
                {
                    if (!quadstoSplit.Contains(intersectingQuads[i])) quadstoSplit.Add(intersectingQuads[i]);
                }
            }

           //Push Quads who are only slightly inside the Assimulation Quad
           if (quadstoPush.Count != 0)
           {
               for (int i = 0; i < quadstoPush.Count; i++)
               {
                   Vector3 vert0 = quadstoPush[i].vertexPoints[0].vertice;
                   Vector3 vert1 = quadstoPush[i].vertexPoints[1].vertice;
                   Vector3 vert2 = quadstoPush[i].vertexPoints[2].vertice;
                   Vector3 vert3 = quadstoPush[i].vertexPoints[3].vertice;
         
                   //This is Top
                   if (BBQuadBottom.inFace(vert2) && BBQuadBottom.inFace(vert3))
                   {
                       quadstoPush[i].vertexPoints[2].vertice.y = BBQuadBottom.vertexPoints[1].vertice.y;
                       quadstoPush[i].vertexPoints[3].vertice.y = BBQuadBottom.vertexPoints[1].vertice.y;
                   }
                   //This is Left
                   if (BBQuadBottom.inFace(vert1) && BBQuadBottom.inFace(vert3))
                   {
                       quadstoPush[i].vertexPoints[1].vertice.x = BBQuadBottom.vertexPoints[0].vertice.x;
                       quadstoPush[i].vertexPoints[3].vertice.x = BBQuadBottom.vertexPoints[0].vertice.x;
                   }
                   //This is Right
                   if (BBQuadBottom.inFace(vert0) && BBQuadBottom.inFace(vert2))
                   {
                       quadstoPush[i].vertexPoints[2].vertice.x = BBQuadBottom.vertexPoints[1].vertice.x;
                       quadstoPush[i].vertexPoints[0].vertice.x = BBQuadBottom.vertexPoints[1].vertice.x;
                   }
               }
           }
            
            //Cut Quads who are larger either in X or Y inside the Assimulation Quad
            if (quadstoCut.Count != 0)
            {
                for (int i = 0; i < quadstoCut.Count; i++)
                {
                    //quad to cut will always have a top and a bottom quad so EZ
                    //Left Quad 
                    Quad leftQuad = new Quad(new Vector3(0, 0, 0), quadstoCut[i].vertexPoints[0].normal);

                    leftQuad.vertexPoints[0].vertice = new Vector3(BBQuadBottom.vertexPoints[1].vertice.x, quadstoCut[i].vertexPoints[0].vertice.y, quadstoCut[i].vertexPoints[0].vertice.z);
                    leftQuad.vertexPoints[1] = quadstoCut[i].vertexPoints[1];
                    leftQuad.vertexPoints[2].vertice = new Vector3(BBQuadBottom.vertexPoints[1].vertice.x, quadstoCut[i].vertexPoints[2].vertice.y, quadstoCut[i].vertexPoints[0].vertice.z);
                    leftQuad.vertexPoints[3] = quadstoCut[i].vertexPoints[3];

                    leftQuad.CalculateQuadArea();
                    leftQuad.CalculateCentre();
                    leftQuad.CalculateCentrePoints();

                    //Right Quad
                    Quad rightQuad = new Quad(new Vector3(0, 0, 0), quadstoCut[i].vertexPoints[0].normal);

                    rightQuad.vertexPoints[0] = quadstoCut[i].vertexPoints[0];
                    rightQuad.vertexPoints[1].vertice = new Vector3(BBQuadBottom.vertexPoints[0].vertice.x, quadstoCut[i].vertexPoints[1].vertice.y, quadstoCut[i].vertexPoints[0].vertice.z);
                    rightQuad.vertexPoints[2] = quadstoCut[i].vertexPoints[2];
                    rightQuad.vertexPoints[3].vertice = new Vector3(BBQuadBottom.vertexPoints[0].vertice.x, quadstoCut[i].vertexPoints[3].vertice.y, quadstoCut[i].vertexPoints[0].vertice.z);

                    rightQuad.CalculateQuadArea();
                    rightQuad.CalculateCentre();
                    rightQuad.CalculateCentrePoints();

                    if (QuadList.Contains(quadstoCut[i])) QuadList.Remove(quadstoCut[i]);

                    if (!QuadList.Contains(rightQuad)) QuadList.Add(rightQuad);
                    if (!QuadList.Contains(leftQuad)) QuadList.Add(leftQuad);
                }
            }


            //Seperate split Quad list beased on vertices based on which vertices is within the Quad
            List<Quad> splitLeft = new List<Quad>();
            List<Quad> splitRight = new List<Quad>();

            Vector3 splitLeftPoint = new Vector3();
            Vector3 splitRightPoint = new Vector3();

            //Seperate Quads into Top and Bottom Quads
           if (quadstoSplit.Count != 0)
            { 
            for (int i = 0; i < quadstoSplit.Count; i++)
            {
                Vertex p0 = quadstoSplit[i].vertexPoints[0];
                Vertex p1 = quadstoSplit[i].vertexPoints[1];
                Vertex p2 = quadstoSplit[i].vertexPoints[2];
                Vertex p3 = quadstoSplit[i].vertexPoints[3];
         
                //Left Check
                if (BBQuadBottom.inFace(p0.vertice) || BBQuadBottom.inFace(p2.vertice))
                {
                    if (!splitLeft.Contains(quadstoSplit[i])) splitLeft.Add(quadstoSplit[i]);
                }
         
                //Right Check
                if (BBQuadBottom.inFace(p1.vertice) || BBQuadBottom.inFace(p3.vertice))
                {
                    if (!splitRight.Contains(quadstoSplit[i])) splitRight.Add(quadstoSplit[i]);
                }
            }
         
            if (splitLeft.Count != 0)
            {
                splitLeftPoint = FindPoint1(splitLeft, BBQuadBottom);
         
                for (int i = 0; i < splitLeft.Count; i++)
                {
                    splitLeft[i].CalculateQuadArea();
                    splitLeft[i].CalculateCentre();
                    splitLeft[i].CalculateCentrePoints();
          
                    intesectingQuad = splitLeft[i];
        
                    FractureQuads(splitLeftPoint);
                }
            }
        
            if (splitRight.Count != 0)
            {
                splitRightPoint = FindPoint0(splitRight, BBQuadBottom);
        
                for (int j = 0; j < splitRight.Count; j++)
                {
                    splitRight[j].CalculateQuadArea();
                    splitRight[j].CalculateCentre();
                    splitRight[j].CalculateCentrePoints();
        
                    intesectingQuad = splitRight[j];
        
                    FractureQuads(splitRightPoint);
                }
            }
        }
        
           for (int i = 0; i < QuadList.Count; i++)
           {
               if (QuadList[i] == BigBoy)
                   continue;
        
               for (int j = 0; j < BBQuadBottom.centrePoints.Count; j++)
               {
                   if (QuadList[i].inFace(BBQuadBottom.centrePoints[j]))
                   {
                       if (!intersectingQuads.Contains(QuadList[i])) intersectingQuads.Add(QuadList[i]);
                   }
               }
           }
        
           for (int i = 0; i < intersectingQuads.Count; i++)
           {
               int vertexPointCount = 0;
        
               for (int j = 0; j < intersectingQuads[i].vertexPoints.Length; j++)
               {
                   if (BBQuadBottom.inFace(intersectingQuads[i].vertexPoints[j].vertice))
                   {
                       vertexPointCount++;
                   }
               }
        
               if (vertexPointCount == 4)
               {
                   if (!quadstoRemove.Contains(intersectingQuads[i])) quadstoRemove.Add(intersectingQuads[i]);
               }
           }

            //Removing Quads Who are Completly inside the Assimulation Quad
           RemoveQuads(quadstoRemove);

            if (BBQuadBottom.area != 0)
            {
                BigBoy.vertexPoints[0].vertice = BBQuadBottom.vertexPoints[0].vertice;
                BigBoy.vertexPoints[1].vertice = BBQuadBottom.vertexPoints[1].vertice;

                BBQuadBottom = new Quad();
                segmentIntersectionBottomVertices = new List<Vertex>();

                BigBoy.CalculateQuadArea();
                BigBoy.CalculateCentre();
                BigBoy.CalculateCentrePoints();
            }
        }

    }

    Vector3 FindPoint3(List<Quad> list, Quad quad)
    {
        Vector3 result = new Vector3();

        List<Vector3> centrePointsInside = new List<Vector3>();

        if (list.Count != 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                //First Gadda Grab All the points inside the New Quad
                for (int j = 0; j < list[i].centrePoints.Count; j++)
                {
                    Vector3 p = list[i].centrePoints[j];

                    if (quad.inFace(p))
                    {
                        if (!centrePointsInside.Contains(p)) centrePointsInside.Add(p);
                    }
                }

                //Find the Top right fron the new list
                float x = centrePointsInside[0].x;
                float y = centrePointsInside[0].y;

                for (int j = 0; j < centrePointsInside.Count; j++)
                {
                    if (centrePointsInside[j].x > x)
                    {
                        x = centrePointsInside[j].x;
                    }

                    if (centrePointsInside[j].y > y)
                    {
                        y = centrePointsInside[j].y;
                    }
                }

                for (int j = 0; j < centrePointsInside.Count; j++)
                {
                    if (centrePointsInside[j].x == x && centrePointsInside[j].y == y)
                    {
                        result = centrePointsInside[j];
                    }

                }

            }
        }

        return result;
    }
                                  
    Vector3 FindPoint1(List<Quad> list, Quad quad)
    {

        Vector3 result = new Vector3();

        //First Gadda Grab All the points inside the New Quad
        List<Vector3> centrePointsInside = new List<Vector3>();

        if (list.Count != 0)
        {
            for (int i = 0; i < list.Count; i++)
            {

                for (int j = 0; j < list[i].centrePoints.Count; j++)
                {
                    Vector3 p = list[i].centrePoints[j];

                    if (quad.inFace(p))
                    {
                        if (!centrePointsInside.Contains(p)) centrePointsInside.Add(p);
                    }
                }

                //Find the Top right fron the new list


                float x = centrePointsInside[0].x;
                float y = centrePointsInside[0].y;


                for (int j = 0; j < centrePointsInside.Count; j++)
                {
                    if (centrePointsInside[j].x > x)
                    {
                        x = centrePointsInside[j].x;
                    }

                    if (centrePointsInside[j].y < y)
                    {
                        y = centrePointsInside[j].y;
                    }
                }

                for (int j = 0; j < centrePointsInside.Count; j++)
                {
                    if (centrePointsInside[j].x == x && centrePointsInside[j].y == y)
                    {
                        result = centrePointsInside[j];
                    }

                }

            }
        }

        return result;
    }
                                 
    Vector3 FindPoint2(List<Quad> list, Quad quad)
    {

        Vector3 result = new Vector3();

        //First Gadda Grab All the points inside the New Quad
        List<Vector3> centrePointsInside = new List<Vector3>();

        if (list.Count != 0)
        {
            for (int i = 0; i < list.Count; i++)
            {

                for (int j = 0; j < list[i].centrePoints.Count; j++)
                {
                    Vector3 p = list[i].centrePoints[j];

                    if (quad.inFace(p))
                    {
                        if (!centrePointsInside.Contains(p)) centrePointsInside.Add(p);
                    }
                }

                //Find the Top right fron the new list

                float x = centrePointsInside[0].x;
                float y = centrePointsInside[0].y;


                for (int j = 0; j < centrePointsInside.Count; j++)
                {
                    if (centrePointsInside[j].x < x)
                    {
                        x = centrePointsInside[j].x;
                    }

                    if (centrePointsInside[j].y > y)
                    {
                        y = centrePointsInside[j].y;
                    }
                }

                for (int j = 0; j < centrePointsInside.Count; j++)
                {
                    if (centrePointsInside[j].x == x && centrePointsInside[j].y == y)
                    {
                        result = centrePointsInside[j];
                    }

                }

            }
        }

        return result;
    }

    Vector3 FindPoint0(List<Quad> list, Quad quad)
    {
        Vector3 result = new Vector3();

        //First Gadda Grab All the points inside the New Quad
        List<Vector3> centrePointsInside = new List<Vector3>();

        if (list.Count != 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].centrePoints.Count; j++)
                {
                    Vector3 p = list[i].centrePoints[j];

                    if (quad.inFace(p))
                    {
                        if (!centrePointsInside.Contains(p)) centrePointsInside.Add(p);
                    }
                }

                float x = centrePointsInside[0].x;
                float y = centrePointsInside[0].y;

                for (int f = 0; f < centrePointsInside.Count; f++)
                {
                    if (centrePointsInside[f].x < x)
                    {
                        x = centrePointsInside[f].x;
                    }

                    if (centrePointsInside[f].y < y)
                    {
                        y = centrePointsInside[f].y;
                    }
                }

                for (int k = 0; k < centrePointsInside.Count; k++)
                {
                    if (centrePointsInside[k].x == x && centrePointsInside[k].y == y)
                    {
                        result = centrePointsInside[k];
                    }

                }

            }
        }

        return result;
    }

    void RemoveQuads(List<Quad> quadsToRemove)
    {
        if (quadsToRemove.Count != 0)
        {
            for (int i = 0; i < quadsToRemove.Count; i++)
            {
                if (QuadList.Contains(quadsToRemove[i])) QuadList.Remove(quadsToRemove[i]);
            }
        }
    }

    void EvaluateAssiumulationQuadRight(List<Vertex> list, Quad bigBoy, Quad BBQuad, Color col)
    {
        //Taking the Data we just created and Doing adjustments before finally Checking its Area

        //Create Anti points to Check
        CheckForAntiPoints(BBQuad);

        if (BBQuad.antiPoints.Count > 0 && BBQuad.centrePoints.Count != 0)
        {


            if(BBQuad.antiPoints.Count > 1)
            {
                Vector3 smallestX = BBQuad.antiPoints[0];

                for(int i = 0; i < BBQuad.antiPoints.Count; i++)
                {
                    if(BBQuad.antiPoints[i].x < smallestX.x)
                    {
                        smallestX = BBQuad.antiPoints[i];
                    }
               
                }

                Vertex[] phantomVertices = BBQuad.CreateTestVertices(smallestX, BBQuad.vertexPoints[0].normal);


                BBQuad.vertexPoints[1].vertice.x = phantomVertices[0].vertice.x;
                BBQuad.vertexPoints[3].vertice.x = phantomVertices[0].vertice.x;
                //BBQuad.antiPoints = BBQuad.antiPoints.OrderByDescending(o => BBQuad.antiPoints.x).ToList(); 

                //Sort threw and find the smallest
            }
            else
            {
                //Generate Phantom vertices and readjust the quad

               Vertex[] phantomVertices = BBQuad.CreateTestVertices(BBQuad.antiPoints[0], BBQuad.vertexPoints[0].normal);


                BBQuad.vertexPoints[1].vertice.x = phantomVertices[0].vertice.x;
                BBQuad.vertexPoints[3].vertice.x = phantomVertices[0].vertice.x;
            }

            // QuadList = QuadList.OrderByDescending(o => o.area).ToList(); 

            //Sort from left to right on X
            //Need to get the smallest X
            //then use phantom quad vertexpoint 0's x 


        }
        else if (BBQuad.antiPoints.Count == 0 && BBQuad.centrePoints.Count != 0)
        {
            //Debug.Log(BBQuad.area);
        }
    }

    void EvaluateAssiumulationQuadLeft(List<Vertex> list, Quad bigBoy, Quad BBQuad, Color col)
    {
        //Taking the Data we just created and Doing adjustments before finally Checking its Area

        //Create Anti points to Check
        CheckForAntiPoints(BBQuad);

        if (BBQuad.antiPoints.Count > 0 && BBQuad.centrePoints.Count != 0)
        {


            if (BBQuad.antiPoints.Count > 1)
            {
                Vector3 greatestX = BBQuad.antiPoints[0];

                for (int i = 0; i < BBQuad.antiPoints.Count; i++)
                {
                    if (BBQuad.antiPoints[i].x > greatestX.x)
                    {
                        greatestX = BBQuad.antiPoints[i];
                    }

                }

                Vertex[] phantomVertices = BBQuad.CreateTestVertices(greatestX, BBQuad.vertexPoints[0].normal);


                BBQuad.vertexPoints[0].vertice.x = phantomVertices[1].vertice.x;
                BBQuad.vertexPoints[2].vertice.x = phantomVertices[1].vertice.x;
                //BBQuad.antiPoints = BBQuad.antiPoints.OrderByDescending(o => BBQuad.antiPoints.x).ToList(); 

            }
            else
            {
                //Generate Phantom vertices and readjust the quad

                Vertex[] phantomVertices = BBQuad.CreateTestVertices(BBQuad.antiPoints[0], BBQuad.vertexPoints[0].normal);

                BBQuad.vertexPoints[0].vertice.x = phantomVertices[1].vertice.x;
                BBQuad.vertexPoints[2].vertice.x = phantomVertices[1].vertice.x;
            }


        }
        else if (BBQuad.antiPoints.Count == 0 && BBQuad.centrePoints.Count != 0)
        {
            //Debug.Log(BBQuad.area);
        }
    }

    void EvaluateAssiumulationQuadTop(List<Vertex> list, Quad bigBoy, Quad BBQuad, Color col)
    {
        //Taking the Data we just created and Doing adjustments before finally Checking its Area

        //Create Anti points to Check
        CheckForAntiPoints(BBQuad);

        if (BBQuad.antiPoints.Count > 0 && BBQuad.centrePoints.Count != 0)
        {
            if (BBQuad.antiPoints.Count > 1)
            {
                Vector3 smallestY = BBQuad.antiPoints[0];

                for (int i = 0; i < BBQuad.antiPoints.Count; i++)
                {
                    if (BBQuad.antiPoints[i].y < smallestY.y)
                    {
                        smallestY = BBQuad.antiPoints[i];
                    }

                }

                Vertex[] phantomVertices = BBQuad.CreateTestVertices(smallestY, BBQuad.vertexPoints[0].normal);


                BBQuad.vertexPoints[2].vertice.y = phantomVertices[1].vertice.y;
                BBQuad.vertexPoints[3].vertice.y = phantomVertices[1].vertice.y;
                //BBQuad.antiPoints = BBQuad.antiPoints.OrderByDescending(o => BBQuad.antiPoints.x).ToList(); 

                //Sort threw and find the smallest
            }
            else
            {
                //Generate Phantom vertices and readjust the quad

                Vertex[] phantomVertices = BBQuad.CreateTestVertices(BBQuad.antiPoints[0], BBQuad.vertexPoints[0].normal);

                BBQuad.vertexPoints[2].vertice.y = phantomVertices[1].vertice.y;
                BBQuad.vertexPoints[3].vertice.y = phantomVertices[1].vertice.y;
            }

            // QuadList = QuadList.OrderByDescending(o => o.area).ToList(); 

            //Sort from left to right on X
            //Need to get the smallest X
            //then use phantom quad vertexpoint 0's x 


        }
        else if (BBQuad.antiPoints.Count == 0 && BBQuad.centrePoints.Count != 0)
        {
            //Debug.Log(BBQuad.area);
        }
    }

    void EvaluateAssiumulationQuadBottom(List<Vertex> list, Quad bigBoy, Quad BBQuad, Color col)
    {
        //Taking the Data we just created and Doing adjustments before finally Checking its Area

        //Create Anti points to Check
        CheckForAntiPoints(BBQuad);

        if (BBQuad.antiPoints.Count > 0 && BBQuad.centrePoints.Count != 0)
        {
            if (BBQuad.antiPoints.Count != 0)
            {
                Vector3 greatestY = BBQuad.antiPoints[0];

                for (int i = 0; i < BBQuad.antiPoints.Count; i++)
                {
                    if (BBQuad.antiPoints[i].y > greatestY.y)
                    {
                        greatestY = BBQuad.antiPoints[i];
                    }

                }

                Vertex[] phantomVertices = BBQuad.CreateTestVertices(greatestY, BBQuad.vertexPoints[0].normal);

                BBQuad.vertexPoints[0].vertice.y = phantomVertices[2].vertice.y;
                BBQuad.vertexPoints[1].vertice.y = phantomVertices[2].vertice.y;
                //BBQuad.antiPoints = BBQuad.antiPoints.OrderByDescending(o => BBQuad.antiPoints.x).ToList(); 

                //Sort threw and find the smallest
            }

            // QuadList = QuadList.OrderByDescending(o => o.area).ToList(); 

            //Sort from left to right on X
            //Need to get the smallest X
            //then use phantom quad vertexpoint 0's x 


        }
        else if (BBQuad.antiPoints.Count == 0 && BBQuad.centrePoints.Count != 0)
        {
            //Debug.Log(BBQuad.area);
        }
    }

    void SortAndCreateAssiumulationQuad(List<Vertex> list, Quad bigBoy, Quad BBQuad, Color col)
    {
            if (list.Count >= 4)
        {
            float sX = list[0].vertice.x;
            float sY = list[0].vertice.y;
            float gX = list[0].vertice.x;
            float gY = list[0].vertice.y;


            int intersectionCheck = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (bigBoy.inFace(list[i].vertice))
                    intersectionCheck++;

                //Find the 4 Best Vertices to Make the Quad
                if (list[i].vertice.x < sX)
                    sX = list[i].vertice.x;

                if (list[i].vertice.x > gX)
                    gX = list[i].vertice.x;

                if (list[i].vertice.y < sY)
                    sY = list[i].vertice.y;

                if (list[i].vertice.y > gY)
                    gY = list[i].vertice.y;
            }

            if (intersectionCheck < 2)
                return;

            Vertex v0 = new Vertex(new Vector3(sX, sY, list[0].vertice.z), new Vector3(0,0,-1), list[0].centre);
            Vertex v1 = new Vertex(new Vector3(gX, sY, list[0].vertice.z), new Vector3(0,0,-1), list[0].centre);
            Vertex v2 = new Vertex(new Vector3(sX, gY, list[0].vertice.z), new Vector3(0,0,-1), list[0].centre);
            Vertex v3 = new Vertex(new Vector3(gX, gY, list[0].vertice.z), new Vector3(0,0,-1), list[0].centre);

            Quad caneryQuad = new Quad(bigBoy.centre, new Vector3(0, 1, 0));

            caneryQuad.quadColor = col;

            caneryQuad.vertexPoints[0] = v0;
            caneryQuad.vertexPoints[1] = v1;
            caneryQuad.vertexPoints[2] = v2;
            caneryQuad.vertexPoints[3] = v3;

            caneryQuad.CalculateCentre();
            caneryQuad.CalculateCentrePoints();

            //Checking for Holes First Before going ahead
            CheckForAntiPoints(caneryQuad);

            //We dont want no phantom quads touching bigboy
            //And any gaps on the sides of big boy will result
            //in canceling the method as there is now way to construct

            if(CheckAntiVerticesPoints(caneryQuad, BigBoy))
            {
                //caneryQuad died :(
                return;
            }

            BBQuad.quadColor = col;
            BBQuad.vertexPoints[0] = caneryQuad.vertexPoints[0];
            BBQuad.vertexPoints[1] = caneryQuad.vertexPoints[1];
            BBQuad.vertexPoints[2] = caneryQuad.vertexPoints[2];
            BBQuad.vertexPoints[3] = caneryQuad.vertexPoints[3];


            BBQuad.CalculateCentre();
            BBQuad.CalculateCentrePoints();

            //Check BBQuad for Anti Points/Holes
            //CheckForAntiPoints(BBQuad);
        }
    }

    bool CheckAntiVerticesPoints(Quad testQuad, Quad bigboy)
    {
        //For each point, Create Phantom Quads to Test against the main list
        //Checking if there phantoms are touching and if so return false 
        for (int i = 0; i < testQuad.antiPoints.Count; i++)
        {
            Vertex[] testies = testQuad.CreateTestVertices(testQuad.antiPoints[i], testQuad.vertexPoints[0].normal);

            for(int j = 0; j < 4; j++)
            {
                //Checking if phantoms balls are touching big boy's parimetre
                if (bigboy.inFace(testies[j].vertice))
                {
                    return true;
                }
                for (int k = 0; k < bigboy.vertexPoints.Length; k++)
                {
                    if (testies[j] == bigboy.vertexPoints[k])
                    {
                        ///Debug.Log("Phantom Balls Touching the BigBoy!");
                        return true;
                    }                  
                }     
            }
        }
        return false;
    }

    void CheckForAntiPoints(Quad q)
    {
        //sort the centres points and find out which 
        //ones are currently not part of the main Quad list

        for (int j = 0; j < q.centrePoints.Count; j++)
        {
            bool inFace = false;

            for (int i = 0; i < QuadList.Count; i++)
            {
                if (QuadList[i].inFace(q.centrePoints[j]))
                {
                    inFace = true;
                    break;
                }
            }

            if (inFace)
                continue;

            if (!q.antiPoints.Contains(q.centrePoints[j])) q.antiPoints.Add(q.centrePoints[j]);
        }

    }

    Vertex SegmentIntersection(Vertex p0, Vertex p1, Vertex p2, Vertex p3)
    {
        //A1 is the change in Y
        //B1 is the change in X

        //Ax + By = C  => Standard Form

        float A1 = p1.vertice.y - p0.vertice.y;
        float B1 = p0.vertice.x - p1.vertice.x;
        float C1 = A1 * p0.vertice.x + B1 * p0.vertice.y;


        float A2 = p3.vertice.y - p2.vertice.y;
        float B2 = p2.vertice.x - p3.vertice.x;
        float C2 = A2 * p2.vertice.x + B2 * p2.vertice.y;
               
        float denominator = A1 * B2 - A2 * B1; /// _/

        float sectX = (B2 * C1 - B1 * C2) / denominator;
        float sectY = (A1 * C2 - A2 * C1) / denominator;

        float rx0 = (sectX - p0.vertice.x) / (p1.vertice.x - p0.vertice.x),
              ry0 = (sectY - p0.vertice.y) / (p1.vertice.y - p0.vertice.y);

        float rx1 = (sectX - p2.vertice.x) / (p3.vertice.x - p2.vertice.x),
              ry1 = (sectY - p2.vertice.y) / (p3.vertice.y - p2.vertice.y);

        if (((rx0 >= 0 && rx0 <= 1) || (ry0 >= 0 && ry0 <= 1)) && ((rx1 >= 0 && rx1 <= 1) || (ry1 >= 0 && ry1 <= 1)))
        {
            return new Vertex(new Vector3(sectX, sectY, p0.vertice.z), new Vector3(0, 0, 1), p3.centre);
        }
        else
            return null; 

    } 

    void EvaluateVerticalIntersection(Quad q, Vertex bb0, Vertex bb1, Vertex BB0, Vertex BB1, List<Vertex> list)
    {
        Vertex p0 = q.vertexPoints[0];
        Vertex p1 = q.vertexPoints[1];
        Vertex p2 = q.vertexPoints[2];
        Vertex p3 = q.vertexPoints[3];

        Vertex r0 = SegmentIntersection(p1, p3, bb1, BB0);
        Vertex r1 = SegmentIntersection(p1, p3, bb0, BB1);

        Vertex r2 = SegmentIntersection(p0, p2, bb1, BB0);
        Vertex r3 = SegmentIntersection(p0, p2, bb0, BB1);

        if (r0 != null)
        {
            if (!list.Contains(r0)) list.Add(r0);
        }

        if (r1 != null)
        {
            if (!list.Contains(r1)) list.Add(r1);
        }

        if (r2 != null)
        {
            if (!list.Contains(r2)) list.Add(r2);
        }

        if (r3 != null)
        {
            if (!list.Contains(r3)) list.Add(r3);
        }

    }

    void EvaluateHorizontalIntersection(Quad q, Vertex bb0, Vertex bb1, Vertex BB0, Vertex BB1, List<Vertex> list)
    {
        Vertex p0 = q.vertexPoints[0];
        Vertex p1 = q.vertexPoints[1];
        Vertex p2 = q.vertexPoints[2];
        Vertex p3 = q.vertexPoints[3];


        Vertex r0 = SegmentIntersection(p0, p1, bb1, BB0);
        Vertex r1 = SegmentIntersection(p0, p1, bb0, BB1);


        Vertex r2 = SegmentIntersection(p2, p3, bb1, BB0);
        Vertex r3 = SegmentIntersection(p2, p3, bb0, BB1);

        if (r0 != null)
        {
            if (!list.Contains(r0)) list.Add(r0);
        }

        if (r1 != null)
        {
            if (!list.Contains(r1)) list.Add(r1);
        }

        if (r2 != null)
        {
            if (!list.Contains(r2)) list.Add(r2);
        }

        if (r3 != null)
        {
            if (!list.Contains(r3)) list.Add(r3);
        }

    }

    void UserInput()
    {
        FaceConstruction();
        //ColorCronenCells();
    }
    
    void FaceConstruction()
    {
        Vector3 sp = Cubil_Painter.sp; //ManageMouseInput();

        CheckForInterSectingQuads();

        if (Cubil_Painter.pointInCube)
            {
                SelectedQuad.SetQuad(sp);

                if (!PointinFace(sp))
                {
                    if (Input.GetMouseButton(0))
                    {
                        CreateNewQuad(sp);
                        UPDATE = true;
                    }
                }
                else if (PointinFace(sp))
                {
                    if (Input.GetMouseButton(1))
                    {
                        FractureQuads(sp);
                        UPDATE = true;
                    }
                }
 
            }

        QuadCalculateCentre();

        if (UPDATE)
        {
            CreateMesh();

            CleanUpQuads(); //Last CleanUp

            for (int i = 0; i < iterations; i++)
                BigBoyAssimulation();  //Second Cleanup 

            UPDATE = false;
        }

        if (QuadList.Count != 0)
        {
            for (int i = 0; i < QuadList.Count; i++)
            {
                QuadList[i].quadColor = Color.white;
            }
        }

        
    }

    void QuadCalculateCentre()
    {
        if (QuadList.Count != 0)
        {
            for (int i = 0; i < QuadList.Count; i++)
            {
                QuadList[i].CalculateCentre();
                QuadList[i].CalculateCentrePoints();
            }
        }
    }

    #region Removing Quads
    // ------------------------------------------------ Removing Quads -----------------------------------------------------------------
    void FractureQuads(Vector3 sp)
    {
        Quad newAntiQuad = new Quad(sp, new Vector3(0, 0, -1));

        newAntiQuad.quadColor = Color.black;

        newAntiQuad.CalculateQuadArea();
        newAntiQuad.CalculateCentre();
        newAntiQuad.CalculateCentrePoints();

        antiQuadList.Add(newAntiQuad);
        FindBordersAndBreak(newAntiQuad);
    }

    void FindBordersAndBreak(Quad newAntiQuad)
    {
        CreateCornerFractures(newAntiQuad.vertexPoints[0], 0);
        CreateCornerFractures(newAntiQuad.vertexPoints[1], 1);
        CreateCornerFractures(newAntiQuad.vertexPoints[2], 2);
        CreateCornerFractures(newAntiQuad.vertexPoints[3], 3);

        CreateOpposingFractures(newAntiQuad);

        newAntiQuad = null;
        antiQuadList.Clear();

        QuadList.Remove(intesectingQuad);
        //CheckAndManageBrokenCronenBergs(intesectingQuad, intesectingQuad);
        intesectingQuad = null;

       // EvalauteCronens();
    }

    void CreateOpposingFractures(Quad innerQuad)
    {
        Vertex vert0 = innerQuad.vertexPoints[0];
        Vertex sideVert0 = GetSideVertex(innerQuad.vertexPoints[0], 0);
        Vertex oppVert0 = GetOppVertex(innerQuad.vertexPoints[0], 0);

        Vertex vert1 = innerQuad.vertexPoints[1];
        Vertex sideVert1 = GetSideVertex(innerQuad.vertexPoints[1], 1);
        Vertex oppVert1 = GetOppVertex(innerQuad.vertexPoints[1], 1);

        Vertex vert2 = innerQuad.vertexPoints[2];
        Vertex sideVert2 = GetSideVertex(innerQuad.vertexPoints[2], 2);
        Vertex oppVert2 = GetOppVertex(innerQuad.vertexPoints[2], 2);

        Vertex vert3 = innerQuad.vertexPoints[3];
        Vertex sideVert3 = GetSideVertex(innerQuad.vertexPoints[3], 3);
        Vertex oppVert3 = GetOppVertex(innerQuad.vertexPoints[3], 3);

        if (Mathf.Abs(vert0.vertice.y - oppVert0.vertice.y) > 0 || Mathf.Abs(vert1.vertice.y - oppVert1.vertice.y) > 0)
        {
            Quad opp0 = new Quad(innerQuad.centre, innerQuad.vertexPoints[0].normal);
            opp0.vertexPoints[0] = new Vertex(oppVert0.vertice, oppVert0.normal, oppVert0.centre);
            opp0.vertexPoints[1] = new Vertex(oppVert1.vertice, oppVert1.normal, oppVert1.centre);
            opp0.vertexPoints[2] = new Vertex(vert0.vertice, vert0.normal, vert0.centre);
            opp0.vertexPoints[3] = new Vertex(vert1.vertice, vert1.normal, vert1.centre);

            opp0.CalculateQuadArea();
            opp0.CalculateCentre();
            opp0.CalculateCentrePoints();

            //opp0.quadColor = customColors[0];
            QuadList.Add(opp0);
            //CheckAndManageBrokenCronenBergs(intesectingQuad, opp0);
        }

        if (Mathf.Abs(vert1.vertice.x - sideVert1.vertice.x) > 0 || Mathf.Abs(vert3.vertice.x - sideVert3.vertice.x) > 0)
        {
            Quad side0 = new Quad(innerQuad.centre, innerQuad.vertexPoints[1].normal);
            side0.vertexPoints[0] = new Vertex(vert1.vertice, vert1.normal, vert1.centre);
            side0.vertexPoints[1] = new Vertex(sideVert1.vertice, sideVert1.normal, sideVert1.centre);
            side0.vertexPoints[2] = new Vertex(vert3.vertice, vert3.normal, vert3.centre);
            side0.vertexPoints[3] = new Vertex(sideVert3.vertice, sideVert3.normal, sideVert3.centre);

            side0.CalculateQuadArea();
            side0.CalculateCentre();
            side0.CalculateCentrePoints();

            //side0.quadColor = customColors[1];
            QuadList.Add(side0);
            //CheckAndManageBrokenCronenBergs(intesectingQuad, side0);
        }

        if (Mathf.Abs(vert2.vertice.y - oppVert2.vertice.y) > 0 || Mathf.Abs(vert3.vertice.y - oppVert3.vertice.y) > 0)
        {
            Quad opp1 = new Quad(innerQuad.centre, innerQuad.vertexPoints[1].normal);
            opp1.vertexPoints[0] = new Vertex(vert2.vertice, vert2.normal, vert2.centre);
            opp1.vertexPoints[1] = new Vertex(vert3.vertice, vert3.normal, vert3.centre);
            opp1.vertexPoints[2] = new Vertex(oppVert2.vertice, oppVert2.normal, oppVert2.centre);
            opp1.vertexPoints[3] = new Vertex(oppVert3.vertice, oppVert3.normal, oppVert3.centre);

            opp1.CalculateQuadArea();
            opp1.CalculateCentre();
            opp1.CalculateCentrePoints();

            // opp1.quadColor = customColors[2];
            QuadList.Add(opp1);
            //CheckAndManageBrokenCronenBergs(intesectingQuad, opp1);
        }

        if (Mathf.Abs(sideVert0.vertice.x - vert0.vertice.x) > 0 || Mathf.Abs(sideVert2.vertice.x - vert2.vertice.x) > 0)
        {
            Quad side1 = new Quad(innerQuad.centre, innerQuad.vertexPoints[1].normal);
            side1.vertexPoints[0] = new Vertex(sideVert0.vertice, sideVert0.normal, sideVert0.centre);
            side1.vertexPoints[1] = new Vertex(vert0.vertice, vert0.normal, vert0.centre);
            side1.vertexPoints[2] = new Vertex(sideVert2.vertice, sideVert2.normal, sideVert2.centre);
            side1.vertexPoints[3] = new Vertex(vert2.vertice, vert2.normal, vert2.centre);

            side1.CalculateQuadArea();
            side1.CalculateCentre();
            side1.CalculateCentrePoints();

            //side1.quadColor = customColors[3];
            QuadList.Add(side1);
            //CheckAndManageBrokenCronenBergs(intesectingQuad, side1);
        }
    }

    Vertex GetSideVertex(Vertex _innerVertex, int index)
    {
        Vector3 vert = new Vector3(intesectingQuad.vertexPoints[index].vertice.x, _innerVertex.vertice.y, _innerVertex.vertice.z);
        Vertex Sidevertex = new Vertex(vert, _innerVertex.normal, _innerVertex.centre);

        return Sidevertex;
    }

    Vertex GetOppVertex(Vertex _innerVertex, int index)
    {
        Vector3 vert2 = new Vector3(_innerVertex.vertice.x, intesectingQuad.vertexPoints[index].vertice.y, _innerVertex.vertice.z);
        Vertex Oppvertex = new Vertex(vert2, _innerVertex.normal, _innerVertex.centre);

        return Oppvertex;
    }

    void CreateCornerFractures(Vertex _innerVertex, int index)
    {
        int flipindex = 3 - index;
        int oppIndex = (index % 2 == 0) ? index + 1 : index - 1;
        int sideIndex = (index + 2) % 4;

        Vector3 vert = new Vector3(intesectingQuad.vertexPoints[index].vertice.x, _innerVertex.vertice.y, _innerVertex.vertice.z);
        Vertex Sidevertex = new Vertex(vert, _innerVertex.normal, _innerVertex.centre);
        
        if(Mathf.Abs((_innerVertex.vertice.x - Sidevertex.vertice.x)) == 0) { return; }

        Vector3 vert2 = new Vector3(_innerVertex.vertice.x, intesectingQuad.vertexPoints[index].vertice.y, _innerVertex.vertice.z);
        Vertex Oppvertex = new Vertex(vert2, _innerVertex.normal, _innerVertex.centre);

        if (Mathf.Abs((_innerVertex.vertice.y - Oppvertex.vertice.y)) == 0) { return; }

        Quad shatteredQuad = new Quad(intesectingQuad.centre, intesectingQuad.vertexPoints[0].normal);

        shatteredQuad.vertexPoints[flipindex] = _innerVertex;
        shatteredQuad.vertexPoints[index] = intesectingQuad.vertexPoints[index];
        shatteredQuad.vertexPoints[oppIndex] = Oppvertex;
        shatteredQuad.vertexPoints[sideIndex] = Sidevertex;

        shatteredQuad.CalculateQuadArea();
        shatteredQuad.CalculateCentrePoints();
        shatteredQuad.CalculateCentre();

        QuadList.Add(shatteredQuad);

        antiVertices.Add(Sidevertex);
        antiVertices.Add(Oppvertex);
    }
    // ------------------------------------------------ Removing Quads -----------------------------------------------------------------

    #endregion

    void CheckForInterSectingQuads()

    {
        if (QuadList.Count != 0)
        {
            for (int i = 0; i < QuadList.Count; i++)
            {
                if (IntersectingVertices(QuadList[i], SelectedQuad))
                {
                    if (!IntersectingQuadList.Contains(QuadList[i]))
                        IntersectingQuadList.Add(QuadList[i]);
                }
            }

            IntersectingQuadList.Clear();
        }
    }

    bool IntersectingVertices(Quad left, Quad right)
    {
        bool status = false;

        indexLeft.Clear();
        indexRight.Clear();

        if (left == right)
            return false;

        for (int i = 0; i < left.vertexPoints.Length; i++)
        {
            for (int j = 0; j < right.vertexPoints.Length; j++)
            {
                if (left.vertexPoints[i] == right.vertexPoints[j])
                {
                    if (!intersectingVertices.Contains(left.vertexPoints[i])) { intersectingVertices.Add(left.vertexPoints[i]); }
                    if (!intersectingVertices.Contains(right.vertexPoints[j])) { intersectingVertices.Add(right.vertexPoints[j]); }

                     indexLeft.Add(i); 
                     indexRight.Add(j); 

                    status = true;
                }
                else
                {
                    //left.quadColor = left.quadColor;
                }
            }
        }

        if (indexLeft.Count > 2)
        {
            Debug.Log("there are 2 or more Quads");
        }

        if (indexLeft.Count < 2)
        { status = false; }

        if (indexRight.Count > 2)
        { Debug.Log("Right Indices Are Max"); }//status = false; }

        intersectingVertices.Clear();

        return status;
    }

    void Log<T>(List<T> obj, string name)
    {
        Debug.Log(name + " " + obj.Count);
    }

    bool PointinFace(Vector3 sp)
    {
        bool status = false;

        if (QuadList.Count != 0)
        {
            for (int i = 0; i < QuadList.Count; i++)
            {
                if (QuadList[i].inFace(sp))
                {
                    intesectingQuad = QuadList[i];
                    status = true;
                    //QuadList[i].quadColor = Color.red;
                }
                else if (intesectingQuad != (QuadList[i]))
                {
                   // QuadList[i].quadColor = Color.white;
                }
            }
        }    
        return status;
    }

    void CreateNewQuad(Vector3 sp)
    {
        Quad newQuad = new Quad(sp, new Vector3(0, 0, -1));
        newQuad.quadColor = QuadFaceColor;

        newQuad.CalculateQuadArea();
        newQuad.CalculateCentre();
        newQuad.CalculateCentrePoints();

        QuadList.Add(newQuad);
    }

    void CleanUpQuads()
    {
        Quad currentQuad = new Quad();
        Quad nextQuad = new Quad();

        if (QuadList.Count >= 2)
            {
                //Do a Quad Check and Merge
                for (int i = 0; i < QuadList.Count; i++)
                {
                    for (int j = 0; j < QuadList.Count; j++)
                    {
                        if (QuadList[i] == QuadList[j])
                        {
                            continue;
                        }

                        if (QuadList[i] == BigBoy)
                        {
                            continue;
                        }

                    if (IntersectingVertices(QuadList[i], QuadList[j]))
                        {
                            currentQuad = QuadList[i];
                            nextQuad = QuadList[j];

                            if (currentQuad.quadColor == Color.white)
                                currentQuad.quadColor = nextQuad.quadColor;

                            Quad MergedQuad = currentQuad.MergeQuads(nextQuad, indexLeft.ToArray(), currentQuad.quadColor);

                            QuadList.Remove(currentQuad);
                            QuadList.Remove(nextQuad);

                            QuadList.Add(MergedQuad);

                            break;
                        }
                }
                }
            }
    
        CreateMesh();
    }

    public void CalculateTotalQuadArea()
    {
        TotalArea = 0f;

        for (int i = 0; i < QuadList.Count; i++)
        {
            TotalArea += QuadList[i].area;
        }
    }

    public void CheckConvexQuad()
    {
        //if (ConvexQuad.area == TotalArea && QuadList.Count > 2 && !Input.GetMouseButtonDown(1))
        //{
        //    Debug.Log("It Was All Me Baby In The Cronen!");
        //
        //    QuadList.Clear();
        //    CronenbergList.Clear();
        //
        //    if (!QuadList.Contains(ConvexQuad)) QuadList.Add(ConvexQuad);
        //    CronenbergList.Add(new CronenbergQuad(ConvexQuad));
        //
        //    SetConvexQuad();
        //}
    }

    public void CalculateConvexInformation()
    {
       // if (QuadList.Count <= 1)
       // {
       //     SetConvexQuad();
       // }
       // else
       // {
       //     EvalauteConvexQuad();
       // }
    }  

    public void SetConvexQuad()
    {
        //ConvexQuad = new Quad();
        //QuadList[0].vertexPoints.CopyTo(ConvexQuad.vertexPoints, 0);
        //ConvexQuad.quadColor = Color.white;
    }

    public void EvalauteConvexQuad()
    {
       // float leftx = ConvexQuad.vertexPoints[0].vertice.x;
       // float rightx = ConvexQuad.vertexPoints[3].vertice.x;
       // float topy = ConvexQuad.vertexPoints[2].vertice.y;
       // float bottomy = ConvexQuad.vertexPoints[1].vertice.y;
       //
       // for (int i = 0; i < QuadList.Count; i++)
       // {
       //     for (int j = 0; j < QuadList[i].vertexPoints.Length; j++)
       //     {
       //         Vertex vertPoint = QuadList[i].vertexPoints[j];
       //
       //         if (vertPoint.vertice.x < leftx)
       //         {
       //             leftx = vertPoint.vertice.x;
       //         }
       //
       //         if (vertPoint.vertice.x > rightx)
       //         {
       //             rightx = vertPoint.vertice.x;
       //         }
       //
       //         if (vertPoint.vertice.y > topy)
       //         {
       //             topy = vertPoint.vertice.y;
       //         }
       //
       //         if (vertPoint.vertice.y < bottomy)
       //         {
       //             bottomy = vertPoint.vertice.y;
       //         }
       //     }
       // }
       //
       // Vector3 norm = QuadList[0].vertexPoints[0].normal;
       // Vector3 centre = QuadList[0].vertexPoints[0].centre;
       //
       // float z = QuadList[0].vertexPoints[0].vertice.z;
       //
       // Vertex vert0 = new Vertex(new Vector3(leftx, bottomy, z), norm, centre);
       // Vertex vert1 = new Vertex(new Vector3(rightx, bottomy, z), norm, centre);
       // Vertex vert2 = new Vertex(new Vector3(leftx, topy, z), norm, centre);
       // Vertex vert3 = new Vertex(new Vector3(rightx, topy, z), norm, centre);
       //
       // ConvexQuad.vertexPoints[0] = vert0;
       //
       // ConvexQuad.vertexPoints[1] = vert1;
       //
       // ConvexQuad.vertexPoints[2] = vert2;
       //
       // ConvexQuad.vertexPoints[3] = vert3;
       //
       // ConvexQuad.CalculateQuadArea();
    }

    void CreateMesh()
    {
        CubilMesh = new Mesh();
        Vector3[] MeshNorms = new Vector3[QuadList.Count * 4];
        Vector3[] MeshVerts = new Vector3[QuadList.Count * 4];
        int[] meshTris = new int[QuadList.Count * 6];
        Vector2[] uvs = new Vector2[17 * 17];

        //Normals and Verts UnPacking
        for (int i = 0; i < QuadList.Count; i++)
        {
            uvs[i] = new Vector2(i / (float)16, i / (float)16);

            MeshVerts[(i * 4) + 0] = QuadList[i].vertexPoints[0].vertice;
            MeshVerts[(i * 4) + 1] = QuadList[i].vertexPoints[1].vertice;
            MeshVerts[(i * 4) + 2] = QuadList[i].vertexPoints[2].vertice;
            MeshVerts[(i * 4) + 3] = QuadList[i].vertexPoints[3].vertice;

            MeshNorms[(i * 4) + 0] = QuadList[i].vertexPoints[0].normal;
            MeshNorms[(i * 4) + 1] = QuadList[i].vertexPoints[1].normal;
            MeshNorms[(i * 4) + 2] = QuadList[i].vertexPoints[2].normal;
            MeshNorms[(i * 4) + 3] = QuadList[i].vertexPoints[3].normal;

            meshTris[(i * 6) + 0] = QuadList[i].t1.indexArray[0] + (i * 4);
            meshTris[(i * 6) + 1] = QuadList[i].t1.indexArray[1] + (i * 4);
            meshTris[(i * 6) + 2] = QuadList[i].t1.indexArray[2] + (i * 4);
                                                                  
            meshTris[(i * 6) + 3] = QuadList[i].t2.indexArray[0] + (i * 4);
            meshTris[(i * 6) + 4] = QuadList[i].t2.indexArray[1] + (i * 4);
            meshTris[(i * 6) + 5] = QuadList[i].t2.indexArray[2] + (i * 4);
        }

        CubilMesh.vertices = MeshVerts;
        CubilMesh.normals = MeshNorms;
        CubilMesh.triangles = meshTris;

        Cubil.GetComponent<MeshFilter>().mesh = CubilMesh;

    }

    #region Debugging

    void OnDrawGizmos()
    {
        if(Application.isPlaying && DEBUG)
        {
            //Vector3 intersection = transform.position + (ray.direction * rayLength);
            //Gizmos.DrawSphere(intersection, .25f);

            //Debug.Log(sp2);

            SelectedQuad.DrawQuad(.1f);
            ConvexQuad.DrawQuad(.1f);

            for (int i = 0; i < antiVertices.Count; i ++)
            {
                //Gizmos.color = Color.magenta;
                //
                //Gizmos.DrawSphere(antiVertices[i + 0].vertice, .15f);
            }

            if (QuadList.Count != 0)
            {
                for (int i = 0; i < QuadList.Count; i++)
                {
                    QuadList[i].DrawQuad(.1f);
                   // Gizmos.DrawSphere(QuadList[i].centre, 0.1f);
                }
            }

            if (intersectingVertices.Count != 0)
            {
                for (int i = 0; i < intersectingVertices.Count; i++)
                {
                    Gizmos.color = intersectingVertices[i].col;
                    Gizmos.DrawSphere(intersectingVertices[i].vertice, .1f);
                }
            }
            //for (int i = 0; i < antiQuadList.Count; i++)
            //    antiQuadList[i].DrawQuad(.1f);
            Gizmos.color = Color.red;

            if (BigBoy != null) { Gizmos.DrawSphere(BigBoy.centre, 0.2f); }

            Gizmos.color = Color.cyan;
            // Gizmos.DrawSphere(new Vector3(8f, 8f, 8f), 1f);

           if(BigBoyBottomLeft != null)        Gizmos.DrawLine(BigBoyBottomLeft.vertice, BigBoy.vertexPoints[0].vertice);
           if(BigBoyBottomRight != null)       Gizmos.DrawLine(BigBoyBottomRight.vertice, BigBoy.vertexPoints[1].vertice);
                                               
           if (BigBoyRightSideBottom != null)  Gizmos.DrawLine(BigBoyRightSideBottom.vertice, BigBoy.vertexPoints[1].vertice);
           if (BigBoyRightSideTop != null)     Gizmos.DrawLine(BigBoyRightSideTop.vertice, BigBoy.vertexPoints[3].vertice);
                                               
           if (BigBoyLeftSideBottom != null)   Gizmos.DrawLine(BigBoyLeftSideBottom.vertice, BigBoy.vertexPoints[0].vertice);
           if (BigBoyLeftSideTop != null)      Gizmos.DrawLine(BigBoyLeftSideTop.vertice, BigBoy.vertexPoints[2].vertice);
                                               
           if (BigBoyTopLeft != null)          Gizmos.DrawLine(BigBoyTopLeft.vertice, BigBoy.vertexPoints[2].vertice);
           if (BigBoyTopRight != null)         Gizmos.DrawLine(BigBoyTopRight.vertice, BigBoy.vertexPoints[3].vertice);


            Gizmos.color = Color.magenta;

            if (segmentIntersectionBottomVertices.Count != 0)
            {
                for (int fuckinghellthisisfucked = 0; fuckinghellthisisfucked < segmentIntersectionBottomVertices.Count; fuckinghellthisisfucked++)
                {
                    Gizmos.DrawSphere(segmentIntersectionBottomVertices[fuckinghellthisisfucked].vertice, 0.15f);
                }
            }

            if (segmentIntersectionLeftVertices.Count != 0)
            {
                for (int fuckinghellthisisfucked = 0; fuckinghellthisisfucked < segmentIntersectionLeftVertices.Count; fuckinghellthisisfucked++)
                {
                    Gizmos.DrawSphere(segmentIntersectionLeftVertices[fuckinghellthisisfucked].vertice, 0.15f);
                }
            }

            if (segmentIntersectionRightVertices.Count != 0)
            {
                for (int fuckinghellthisisfucked = 0; fuckinghellthisisfucked < segmentIntersectionRightVertices.Count; fuckinghellthisisfucked++)
                {
                    Gizmos.DrawSphere(segmentIntersectionRightVertices[fuckinghellthisisfucked].vertice, 0.15f);
                }
            }

            if (segmentIntersectionTopVertices.Count != 0)
            {
                for (int fuckinghellthisisfucked = 0; fuckinghellthisisfucked < segmentIntersectionTopVertices.Count; fuckinghellthisisfucked++)
                {
                    Gizmos.DrawSphere(segmentIntersectionTopVertices[fuckinghellthisisfucked].vertice, 0.15f);
                }
            }

            //Gizmos.color = Color.magenta;
            if(BBQuadRight != null) BBQuadRight.DrawQuad(.15f);

            if (BBQuadLeft != null) BBQuadLeft.DrawQuad(.15f);

            if (BBQuadTop != null)  BBQuadTop.DrawQuad(.15f);
            if (BBQuadBottom != null) BBQuadBottom.DrawQuad(.15f);
        }
    }

    #endregion
}
