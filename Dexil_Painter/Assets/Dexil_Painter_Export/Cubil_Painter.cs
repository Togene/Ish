using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Cubil_Painter : MonoBehaviour
{
    //public CronenbergQuad selectedBerg;

    Mesh CubilMesh;
    GameObject Cubil;

    Quad intesectingQuad;

    List<Quad> IntersectingQuadList = new List<Quad>(); List<Quad> antiQuadList = new List<Quad>();
    List<Vertex> intersectingVertices = new List<Vertex>(); public List<Vertex> antiVertices = new List<Vertex>();
    List<CronenbergQuad> intersectingCronens = new List<CronenbergQuad>();

    List<int> indexLeft = new List<int>(); List<int> indexRight = new List<int>();
    Vector3[,] Grid;
    Vector3 m_Input;
    //Ray ray;
    public Quad BigBoy;
    bool x;

    //public List<QuadList> histogramList = new List<QuadList>();

    public bool ColorRegions, pointInCube, UPDATE;
    public List<CronenbergQuad> CronenbergList = new List<CronenbergQuad>();
    public List<Quad> QuadList = new List<Quad>();
    public float TotalArea;
    public int iterations;
    public Color[] cronenColors, customColors; //Color Information
    public Color GridColor, EditorCubeColor, DebugCubilMergeColor, QuadColor;
    public Quad SelectedQuad, ConvexQuad;
    public float highestQuad;
    //float rayLength;

    private Vertex BigBoyBottomLeft, BigBoyBottomRight, 
                  BigBoyRightSideBottom, BigBoyRightSideTop,
                  BigBoyLeftSideBottom, BigBoyLeftSideTop,
                  BigBoyTopLeft, BigBoyTopRight;

    public Quad BBQuadLeft, BBQuadRight, BBQuadTop, BBQuadBottom;

    public List<Vertex> segmentIntersectionRightVertices = new List<Vertex>();
    private List<Vertex> segmentIntersectionLeftVertices = new List<Vertex>();
    private List<Vertex> segmentIntersectionBottomVertices = new List<Vertex>();
    private List<Vertex> segmentIntersectionTopVertices = new List<Vertex>();

    void Awake()
    {
        ConvexQuad = new Quad();
        SelectedQuad = Quad.create(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        CubilMesh = new Mesh();
        CreateGrid(25, 25);
        Cubil = FindObjectOfType<MeshFilter>().gameObject;
    }

    void Update()
    {
        UserInput();
    }

    void BigBoyAssimulation() //Handles Bigger Quads simplifying the Mesh
    {

        if (QuadList.Count > 1)
            BigBoy = QuadList[0]; //if sorted right this should be the largest Area

        for (int i = 0; i < QuadList.Count; i++)
        {
            QuadList[i].quadColor = Color.white;
        }

        BigBoy.quadColor = Color.black;

        if (BigBoy.area > 1)
        {
            CheckBigBoySides(BigBoy);
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

        for(int i = 0; i < QuadList.Count; i++)
        {

            if (QuadList[i] == BigBoy)
                continue;

            Vertex bb0 = BigBoy.vertexPoints[0];
            Vertex bb1 = BigBoy.vertexPoints[1];
            Vertex bb2 = BigBoy.vertexPoints[2];
            Vertex bb3 = BigBoy.vertexPoints[3];

            //RightSide 
            EvaluateVerticalIntersection(QuadList[i], bb1, bb3, BigBoyRightSideTop, BigBoyRightSideBottom, segmentIntersectionRightVertices);

            //BottomSide
            EvaluateHorizontalIntersection(QuadList[i], bb0, bb1, BigBoyBottomRight, BigBoyBottomLeft, segmentIntersectionBottomVertices);

            //LeftSide 
            EvaluateVerticalIntersection(QuadList[i], bb0, bb2, BigBoyLeftSideTop, BigBoyLeftSideBottom, segmentIntersectionLeftVertices);

            //TopSide
            EvaluateHorizontalIntersection(QuadList[i], bb2, bb3, BigBoyTopRight, BigBoyTopLeft, segmentIntersectionTopVertices);

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

        //Count needs to be greater then 4 in order to create a Quad...Duh

        //Right Quad
        SortAndCreateQuad(segmentIntersectionRightVertices, BigBoy, BBQuadRight, Color.yellow);

        //Left
        SortAndCreateQuad(segmentIntersectionLeftVertices, BigBoy, BBQuadLeft, Color.green);

        //Left
        SortAndCreateQuad(segmentIntersectionBottomVertices, BigBoy, BBQuadBottom, Color.blue);
       
       // //Top
        SortAndCreateQuad(segmentIntersectionTopVertices, BigBoy, BBQuadTop, Color.red);
    }

    void SortAndCreateQuad(List<Vertex> list, Quad bigBoy, Quad BBQuad, Color col)
    {
        //Reverse the vertice order, right quad 

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

                    //just setting a Quick defualt for comparison reasons

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

           // BBQuad = new Quad();
            Quad newQuad = new Quad(bigBoy.centre, new Vector3(0, 1, 0));

            newQuad.quadColor = col;

            newQuad.vertexPoints[0] = v0;
            newQuad.vertexPoints[1] = v1;
            newQuad.vertexPoints[2] = v2;
            newQuad.vertexPoints[3] = v3;

            newQuad.CalculateQuadArea();
            newQuad.CalculateCentre();
            newQuad.CalculateCentrePoints();

            LookForAntiPoints(newQuad); //Looking For points in the mesh that arnt part of the Main QuadMesh

            BBQuad.quadColor = col;

            BBQuad.vertexPoints[0] = newQuad.vertexPoints[0];
            BBQuad.vertexPoints[1] = newQuad.vertexPoints[1];
            BBQuad.vertexPoints[2] = newQuad.vertexPoints[2];
            BBQuad.vertexPoints[3] = newQuad.vertexPoints[3];

            BBQuad.CalculateQuadArea();
            BBQuad.CalculateCentre();
            BBQuad.CalculateCentrePoints();
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

    void LookForAntiPoints(Quad quad)
    {
        //Points not on the mesh 

        quad.antiPoints = new List<Vector3>();

        for(int i = 0; i < quad.centrePoints.Count; i++)
        {
            bool isAnti = true;

           // quad.antiPoints.Add(quad.centrePoints[i]);

            for (int j = 0; j < QuadList.Count; j++)
            {

                if(QuadList[j].inFace(quad.centrePoints[i]))
                {
                    //If even 1 quad returns true (its ontop of a quad) 
                    //then its not a hole/anti Quad
                    isAnti = false;
                    break;
                }
            }

          if (isAnti)
          {
              quad.antiPoints.Add(quad.centrePoints[i]);
          }
        }

        Debug.Log(quad.antiPoints.Count);
    }

    Vector3 ManageMouseInput()
    {
        m_Input = Camera.main.ScreenToWorldPoint(Input.mousePosition - new Vector3(0, 0, transform.position.z));
        //ray = new Ray(transform.position, Vector3.Normalize(m_Input));
        float ratio = Camera.main.fieldOfView / (Camera.main.fieldOfView / m_Input.z);

        Vector3 dir = Vector3.Normalize(m_Input - transform.position) * ratio;

        Debug.DrawRay(transform.position, dir);

        float sp_X = g_Utils.roundNearest(m_Input.x, m_Input.x / 15);
        float sp_Y = g_Utils.roundNearest(m_Input.y, m_Input.y / 15);
        float sp_Z = m_Input.z;//g_Utils.roundNearest(m_Input.z, (m_Input.z) % 15);

        return new Vector3(sp_X + .5f, sp_Y + .5f, sp_Z);
    }

    void UserInput()
    {
        TransformFace();
        FaceConstruction();
        ManageCamera();
        //ColorCronenCells();
    }
    
    void TransformFace()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCronen(new Vector3(0, +1, 0));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveCronen(new Vector3(0, -1, 0));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveCronen(new Vector3(-1, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveCronen(new Vector3(+1, 0, 0));
        }
    }

    void MoveCronen(Vector3 c)
    {
       // if (CronenbergList[0].CheckPointInQuad(c))
       // {
       //     CronenbergList[0].UpdateCronenVertices(c);
       // }
       // 
       // UpdateCronenConvex();
       //
       // int _j = 0;
       //
       // for (int i = 0; i < CronenbergList.Count; i++)
       // {
       //     ManageCronenMerge(i, _j);
       // }
    }

    void FaceConstruction()
    {

        Vector3 sp = ManageMouseInput();
        pointInCube = g_Utils.pointInCube(sp, new Vector3(0, 0, 0), new Vector3(16, 16, 16));

        if (pointInCube)
        {
            CheckForInterSectingQuads();
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


            if (UPDATE)
            {
                if (ColorRegions)
                    ColorCronen();

                FindCronenEdgeQuads();
                UpdateCronenConvex();
                CreateMesh();
                MarchingSquaresSearch();

                UPDATE = false;
            }

            #region Cleanup

            CleanUpQuads(); //First Initial CleanUp
            QuadList = QuadList.OrderByDescending(o => o.area).ToList(); // Sort From Largest to smallest 
            BigBoyAssimulation();  //Second Cleanup 

            //---------------------------- CleanUp -----------------------------------------------------

            #endregion

            QuadCalculateCentre();
        }
    }

    void QuadCalculateCentre()
    {
        foreach (Quad q in QuadList)
        {
            q.CalculateCentre();
            q.CalculateCentrePoints();
        }
    }


    #region CameraControls
    //---------------------------- CameraControls -----------------------------------------------------
    void ManageCamera()
    {
        if (Input.GetKeyDown(KeyCode.Keypad5))
            Camera.main.orthographic = !Camera.main.orthographic;

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            x = !x;
            RotateCameraY(-(Mathf.PI / 2));
        }

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            x = !x;
            RotateCameraY(+Mathf.PI / 2);
        }

        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            if(!x)       
            RotateCameraX(+Mathf.PI / 2);
            else
            RotateCameraZ(+Mathf.PI / 2);
        }


        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (!x)
            RotateCameraX(-Mathf.PI / 2);
            else
            RotateCameraZ(-Mathf.PI / 2);
        }

    }

    void RotateCameraY(float angle)
    {

        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);

        float dx = transform.position.x - 8f;
        float dz = transform.position.z - 8f;

        float x1 = (dx * c) - (dz * s);
        float z1 = (dz * c) + (dx * s);

        Vector3 newVec = new Vector3(x1 + 8, transform.position.y, z1 + 8);
        transform.position = newVec;

        transform.LookAt(new Vector3(8, 8, 8));
    }

    void RotateCameraX(float angle)
    {
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);

        float dy = transform.position.y - 8f;
        float dz = transform.position.z - 8f;

        float y1 = (dy * c) - (dz * s);
        float z1 = (dz * c) + (dy * s);

        Vector3 newVec = new Vector3(transform.position.x, y1 + 8, z1 + 8);
        transform.position = newVec;

        transform.LookAt(new Vector3(8, 8, 8));
    }

    void RotateCameraZ(float angle)
    {
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);

        float dy = transform.position.y - 8f;
        float dx = transform.position.x - 8f;

        float y1 = (dy * c) - (dx * s);
        float x1 = (dx * c) + (dy * s);

        Vector3 newVec = new Vector3(x1 + 8, y1 + 8, transform.position.z);
        transform.position = newVec;

        transform.LookAt(new Vector3(8, 8, 8));
    }
    //---------------------------- CameraControls -----------------------------------------------------
    #endregion

    #region CronenManagment 
    //---------------------------- CronenManagment -----------------------------------------------------
    void UpdateCronenConvex()
    {
        if (QuadList.Count != 0)
        {
            SetConvexQuad();
            CalculateConvexInformation();
            CalculateTotalQuadArea();
            CheckConvexQuad();
        }
    }

    void FindCronenEdgeQuads()
    {
        for (int i = 0; i < CronenbergList.Count; i++)
        {
            CronenbergList[i].CalculateConvexInformation();
            CronenbergList[i].CalculateTotalQuadArea();
            CronenbergList[i].CheckConvexQuad(QuadList);
            CronenbergList[i].EvaluateCronenEdges();
        }
    }

    void ColorCronen()
    {
        if (CronenbergList.Count != 0)
        {
            for (int i = 0; i < CronenbergList.Count; i++)
            {
                for (int j = 0; j < CronenbergList[i].cronenQuadList.Count; j++)
                {
                    Color col = cronenColors[i % cronenColors.Length];

                    CronenbergList[i].cronenQuadList[j].quadColor = col;
                }
            }
        }
    }

    void ColorCronenCells()
    {
        if (CronenbergList.Count != 0)
        {
            for (int i = 0; i < CronenbergList.Count; i++)
            {
                CronenbergList[i].ColorCells(cronenColors);
            }
        }
    }

    void EvalauteCronens()
    {
        if (CronenbergList.Count != 0)
        {
            for (int i = 0; i < CronenbergList.Count; i++)
            {
                CronenbergList[i].EvalauteCronen(CronenbergList);

                for (int j = 0; j < CronenbergList.Count; j++)
                {
                    if (CronenbergList[i] == CronenbergList[j])
                        continue;

                    if (CronenbergList[i].CronenQuadAndMergeIntercepting(CronenbergList[j]))
                    {
                        Debug.Log("There Touching Again");
                    }
                }
            }
        }
    }

    bool CheckAndManageBrokenCronenBergs(Quad Qinterecpt, Quad Q1)
    {
        if (CronenbergList.Count != 0)
        {
            for (int i = 0; i < CronenbergList.Count; i++)
            {
                if (CronenbergList[i].FractorCronenQuads(Qinterecpt, Q1, CronenbergList))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void CheckAndIniatiateChainCheckBergs(Quad Q0, Quad Q1, Quad Qmerged)
    {
        if (CronenbergList.Count != 0)
        {
            for (int i = 0; i < CronenbergList.Count; i++)
            {
                CronenbergList[i].MergeCronenQuads(Q0, Q1, Qmerged);
            }
        }
    }

    bool CheckCronenContainsBergsForInterSection(Quad Q0)
    {
        bool status = false;
        int _j = 0;

        if (CronenbergList.Count != 0)
        {
            for (int i = 0; i < CronenbergList.Count; i++)
            {
                if (!CronenbergList[i].IntersectingWithCronen(Q0))
                {
                }
                else
                {
                    ManageCronenMerge(i, _j);
                    status = true;
                }
            }
        }

        intersectingCronens.Clear();
        return status;
    }

    void ManageCronenMerge(int i, int _j)
    {
        for (int j = 0; j < CronenbergList.Count; j++)
        {
            if (CronenbergList[i] == CronenbergList[j])
            {
                continue;
            }

            if (CronenbergList[i].CronenQuadAndMergeIntercepting(CronenbergList[j]))
            {
                if (!intersectingCronens.Contains(CronenbergList[j])) intersectingCronens.Add(CronenbergList[j]);
            }

            if (intersectingCronens.Count < 1)
            {
                //Debug.Log(intersectingCronens.Count);
            }
            else
            {
                if (intersectingCronens.Count != 0)
                {
                    for (int k = 0; k < intersectingCronens.Count; k++)
                    {
                        if (CronenbergList.Contains(CronenbergList[_j])) CronenbergList.Remove(intersectingCronens[k]);
                    }
                }
            }
        }
    }

    void RemoveFromCronen(Quad Q0)
    {
        if (CronenbergList.Count != 0)
        {
            for (int i = 0; i < CronenbergList.Count; i++)
            {
                if (CronenbergList[i].CheckCronenContains(Q0)) CronenbergList[i].Remove(Q0);
            }
        }
    }
    //---------------------------- CronenManagment -----------------------------------------------------
    #endregion

    #region Removing Quads
    // ------------------------------------------------ Removing Quads -----------------------------------------------------------------
    void FractureQuads(Vector3 sp)
    {
        Quad newAntiQuad = new Quad(sp, new Vector3(0, 0, -1));

        newAntiQuad.quadColor = Color.black;      
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
        CheckAndManageBrokenCronenBergs(intesectingQuad, intesectingQuad);
        intesectingQuad = null;

        EvalauteCronens();
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
            //opp0.quadColor = customColors[0];
            QuadList.Add(opp0);
            CheckAndManageBrokenCronenBergs(intesectingQuad, opp0);
        }

        if (Mathf.Abs(vert1.vertice.x - sideVert1.vertice.x) > 0 || Mathf.Abs(vert3.vertice.x - sideVert3.vertice.x) > 0)
        {
            Quad side0 = new Quad(innerQuad.centre, innerQuad.vertexPoints[1].normal);
            side0.vertexPoints[0] = new Vertex(vert1.vertice, vert1.normal, vert1.centre);
            side0.vertexPoints[1] = new Vertex(sideVert1.vertice, sideVert1.normal, sideVert1.centre);
            side0.vertexPoints[2] = new Vertex(vert3.vertice, vert3.normal, vert3.centre);
            side0.vertexPoints[3] = new Vertex(sideVert3.vertice, sideVert3.normal, sideVert3.centre);
            side0.CalculateQuadArea();
            //side0.quadColor = customColors[1];
            QuadList.Add(side0);
            CheckAndManageBrokenCronenBergs(intesectingQuad, side0);
        }

        if (Mathf.Abs(vert2.vertice.y - oppVert2.vertice.y) > 0 || Mathf.Abs(vert3.vertice.y - oppVert3.vertice.y) > 0)
        {
            Quad opp1 = new Quad(innerQuad.centre, innerQuad.vertexPoints[1].normal);
            opp1.vertexPoints[0] = new Vertex(vert2.vertice, vert2.normal, vert2.centre);
            opp1.vertexPoints[1] = new Vertex(vert3.vertice, vert3.normal, vert3.centre);
            opp1.vertexPoints[2] = new Vertex(oppVert2.vertice, oppVert2.normal, oppVert2.centre);
            opp1.vertexPoints[3] = new Vertex(oppVert3.vertice, oppVert3.normal, oppVert3.centre);
            opp1.CalculateQuadArea();
           // opp1.quadColor = customColors[2];
            QuadList.Add(opp1);
            CheckAndManageBrokenCronenBergs(intesectingQuad, opp1);
        }

        if (Mathf.Abs(sideVert0.vertice.x - vert0.vertice.x) > 0 || Mathf.Abs(sideVert2.vertice.x - vert2.vertice.x) > 0)
        {
            Quad side1 = new Quad(innerQuad.centre, innerQuad.vertexPoints[1].normal);
            side1.vertexPoints[0] = new Vertex(sideVert0.vertice, sideVert0.normal, sideVert0.centre);
            side1.vertexPoints[1] = new Vertex(vert0.vertice, vert0.normal, vert0.centre);
            side1.vertexPoints[2] = new Vertex(sideVert2.vertice, sideVert2.normal, sideVert2.centre);
            side1.vertexPoints[3] = new Vertex(vert2.vertice, vert2.normal, vert2.centre);
            side1.CalculateQuadArea();
            //side1.quadColor = customColors[3];
            QuadList.Add(side1);
            CheckAndManageBrokenCronenBergs(intesectingQuad, side1);
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

        QuadList.Add(shatteredQuad);

        CheckAndManageBrokenCronenBergs(intesectingQuad, shatteredQuad);

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
                   
                        //if(!indexLeft.Contains(i))
                        { indexLeft.Add(i);}
                        //if (!indexRight.Contains(j))
                        { indexRight.Add(j);}

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
            { status = false;}
        
        if (indexRight.Count > 2)
        { Debug.Log("Right Indices Are Max"); }//status = false; }

        intersectingVertices.Clear();

        return status;
    }

    bool InverseIntersectingVertices(Quad left, Quad right)
    {
        bool status = false;

        indexLeft.Clear();
        indexRight.Clear();

        if (left == right)
            return false;

        for (int i = left.vertexPoints.Length - 1; i > 0; i--)
        {
            for (int j = right.vertexPoints.Length - 1; j > 0; j--)
            {
                if (left.vertexPoints[i] == right.vertexPoints[j])
                {
                    if (!intersectingVertices.Contains(left.vertexPoints[i])) { intersectingVertices.Add(left.vertexPoints[i]); }
                    if (!intersectingVertices.Contains(right.vertexPoints[j])) { intersectingVertices.Add(right.vertexPoints[j]); }

                    //if(!indexLeft.Contains(i))
                    { indexLeft.Add(i); }
                    //if (!indexRight.Contains(j))
                    { indexRight.Add(j); }

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

    void CreateNewQuad(Vector3 sp)
    {
        Quad newQuad = new Quad(sp, new Vector3(0, 0, -1));
        newQuad.quadColor = QuadColor;
        QuadList.Add(newQuad);

        if(CronenbergList.Count != 0)
        {
            if (!CheckCronenContainsBergsForInterSection(newQuad))
            {
                //Debug.Log("There Wasnt Quad Intersection");
                CronenbergList.Add(new CronenbergQuad(newQuad));
            }
        }
        else
        {
            CronenbergList.Add(new CronenbergQuad(newQuad));
        }
    }

    void CleanUpQuads()
    {
        Quad currentQuad = new Quad();
        Quad nextQuad = new Quad();

         QuadList = QuadList.OrderByDescending(o => o.area).ToList();

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

                            CheckAndIniatiateChainCheckBergs(currentQuad, nextQuad, MergedQuad);

                            break;
                        }
                        else if (InverseIntersectingVertices(QuadList[i], QuadList[j]))
                        {
                            currentQuad = QuadList[i];
                            nextQuad = QuadList[j];

                            if (currentQuad.quadColor == Color.white)
                                currentQuad.quadColor = nextQuad.quadColor;

                            Quad MergedQuad = currentQuad.MergeQuads(nextQuad, indexLeft.ToArray(), currentQuad.quadColor);

                            QuadList.Remove(currentQuad);
                            QuadList.Remove(nextQuad);
                            QuadList.Add(MergedQuad);

                            CheckAndIniatiateChainCheckBergs(currentQuad, nextQuad, MergedQuad);

                            break;
                        }
                }
                }
            }
    
        CreateMesh();
    }

    void MarchingSquaresSearch()
    {
        //Find Sqaures By Comparing and Sorting threw Cronen outer points...yay

        for(int i = 0; i < CronenbergList.Count; i++)
        {
            for(int j = 0; j < CronenbergList[i].cronenQuadList.Count; j++)
            {

            }
        }
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
        if (QuadList.Count <= 1)
        {
            SetConvexQuad();
        }
        else
        {
            EvalauteConvexQuad();
        }
    }

    public void SetConvexQuad()
    {
        ConvexQuad = new Quad();
        QuadList[0].vertexPoints.CopyTo(ConvexQuad.vertexPoints, 0);
        ConvexQuad.quadColor = Color.white;
    }

    public void EvalauteConvexQuad()
    {
        float leftx = ConvexQuad.vertexPoints[0].vertice.x;
        float rightx = ConvexQuad.vertexPoints[3].vertice.x;
        float topy = ConvexQuad.vertexPoints[2].vertice.y;
        float bottomy = ConvexQuad.vertexPoints[1].vertice.y;

        for (int i = 0; i < QuadList.Count; i++)
        {
            for (int j = 0; j < QuadList[i].vertexPoints.Length; j++)
            {
                Vertex vertPoint = QuadList[i].vertexPoints[j];

                if (vertPoint.vertice.x < leftx)
                {
                    leftx = vertPoint.vertice.x;
                }

                if (vertPoint.vertice.x > rightx)
                {
                    rightx = vertPoint.vertice.x;
                }

                if (vertPoint.vertice.y > topy)
                {
                    topy = vertPoint.vertice.y;
                }

                if (vertPoint.vertice.y < bottomy)
                {
                    bottomy = vertPoint.vertice.y;
                }
            }
        }

        Vector3 norm = QuadList[0].vertexPoints[0].normal;
        Vector3 centre = QuadList[0].vertexPoints[0].centre;

        float z = QuadList[0].vertexPoints[0].vertice.z;

        Vertex vert0 = new Vertex(new Vector3(leftx, bottomy, z), norm, centre);
        Vertex vert1 = new Vertex(new Vector3(rightx, bottomy, z), norm, centre);
        Vertex vert2 = new Vertex(new Vector3(leftx, topy, z), norm, centre);
        Vertex vert3 = new Vertex(new Vector3(rightx, topy, z), norm, centre);

        ConvexQuad.vertexPoints[0] = vert0;

        ConvexQuad.vertexPoints[1] = vert1;

        ConvexQuad.vertexPoints[2] = vert2;

        ConvexQuad.vertexPoints[3] = vert3;

        ConvexQuad.CalculateQuadArea();
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

    void CreateGrid(int _x, int _z)
    {
        Grid = new Vector3[_x, _z];

        for (int x = 0; x < _x; x++)
        {
            for (int z = 0; z < _z; z++)
            {
                Grid[x, z] = new Vector3(x - 4, 0, z - 4);
            }
        }

    }

    void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            //Vector3 intersection = transform.position + (ray.direction * rayLength);
            //Gizmos.DrawSphere(intersection, .25f);

            //Debug.Log(sp2);

            DrawGrid();
            DrawEditorCube();

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

            if (CronenbergList.Count != 0)
            {
                for (int i = 0; i < CronenbergList.Count; i++)
                {
                    //CronenbergList[i].DrawEdgeVertices(cronenColors);
                    //CronenbergList[i].DrawConvexQuad();
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

            Gizmos.DrawSphere(BigBoy.centre, 0.2f);

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

    void DrawGrid()
    {
        Gizmos.color = GridColor;

        for (int x = 0; x < 25; x++)
        {
            for (int z = 0; z < 25; z++)
            {
                Vector3 gPoint0 = Grid[x, z];
                Vector3 gPoint1 = Grid[(x + 1) % 25, z];
                Vector3 gPoint2 = Grid[x, (z + 1) % 25];

                Gizmos.DrawLine(gPoint0, gPoint1);
                Gizmos.DrawLine(gPoint0, gPoint2);
            }
        }
    }

    void DrawEditorCube()
    {
        Gizmos.color = EditorCubeColor;

        Vector3 gPoint0 = new Vector3 (0,  0, 0  );
        Vector3 gPoint1 = new Vector3 (16, 0, 0  );
        Vector3 gPoint2 = new Vector3 (16, 0, 16 );
        Vector3 gPoint3 = new Vector3 (0, 0, 16  );
                      
        Vector3 gPoint4 = new Vector3 (0, 16, 0  );
        Vector3 gPoint5 = new Vector3 (16, 16, 0 );
        Vector3 gPoint6 = new Vector3 (16, 16, 16);
        Vector3 gPoint7 = new Vector3 (0, 16, 16 );

        Gizmos.DrawLine(gPoint0, gPoint4);
        Gizmos.DrawLine(gPoint1, gPoint5);
        Gizmos.DrawLine(gPoint2, gPoint6);
        Gizmos.DrawLine(gPoint3, gPoint7);

        Gizmos.DrawLine(gPoint0, gPoint1);
        Gizmos.DrawLine(gPoint1, gPoint2);
        Gizmos.DrawLine(gPoint2, gPoint3);
        Gizmos.DrawLine(gPoint3, gPoint0);

        Gizmos.DrawLine(gPoint4, gPoint5);
        Gizmos.DrawLine(gPoint5, gPoint6);
        Gizmos.DrawLine(gPoint6, gPoint7);
        Gizmos.DrawLine(gPoint7, gPoint4);
    }
}
