using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Returns a Current Status aswell as a Array Position
/// </summary>
[Serializable]
public struct BoolInt
{
    public int I;
    public bool Status;

    public BoolInt (int i, bool status)
    {
        I = i;
        Status = status;
    }
}

[Serializable]
public class CronenCell
{
    public List<Quad> cellQuadList = new List<Quad>();

    public CronenCell()
    {
        cellQuadList = new List<Quad>();
    }

    public CronenCell(Quad Q)
    {
        cellQuadList = new List<Quad>();
        cellQuadList.Add(Q);
    }

    public CronenCell(Quad Q0, Quad Q1)
    {
        cellQuadList = new List<Quad>();
        cellQuadList.Add(Q0);
        cellQuadList.Add(Q1);
    }

    public bool CheckCellContains(Quad Q)
    {
        for (int i = 0; i < cellQuadList.Count; i++)
        {
            if (cellQuadList.Contains(Q))
                return true;
        }

        return false;
    }

    public bool IntersectingWithCell(Quad Q)
    {
        bool status = false;

        for (int i = 0; i < cellQuadList.Count; i++)
        {
            if (Quad.intersectingQuad(cellQuadList[i], Q))
            {
                if(!cellQuadList.Contains(Q)) cellQuadList.Add(Q);
            }
        }

        return status;
    }

    public bool CellQuadIntercepting(CronenCell right)
    {
        bool status = false;

        for (int i = 0; i < cellQuadList.Count; i++)
        {
            for (int j = 0; j < right.cellQuadList.Count; j++)
            {
                if (Quad.intersectingQuad(cellQuadList[i], right.cellQuadList[j]))
                {
                    //Debug.Log("Anus Pee Pee 2 2");
                    status = true;
                }
            }
        }

        return status;
    }

    public static CronenCell MergeCells(CronenCell left, CronenCell right)
    {
        CronenCell newCell = new CronenCell();

        for (int i = 0; i < left.cellQuadList.Count; i++)
        {
            if (!newCell.cellQuadList.Contains(left.cellQuadList[i]))
                 newCell.cellQuadList.Add(left.cellQuadList[i]);
        }

        for (int j = 0; j < right.cellQuadList.Count; j++)
        {
            if (!newCell.cellQuadList.Contains(right.cellQuadList[j]))
                 newCell.cellQuadList.Add(right.cellQuadList[j]);
        }

        return newCell;
    }

    public void MergeCells(CronenCell right)
    {
        for (int j = 0; j < right.cellQuadList.Count; j++)
        {
            if (!cellQuadList.Contains(right.cellQuadList[j]))
                 cellQuadList.Add(right.cellQuadList[j]);
        }
    }

    public static bool operator ==(CronenCell left, CronenCell right)
    {
        if (object.ReferenceEquals(left, null))
        {
            if (object.ReferenceEquals(right, null))
            {
                return true;
            }

            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(CronenCell left, CronenCell right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is CronenCell))
            return false;
        else
            return (cellQuadList.SequenceEqual(((CronenCell)(obj)).cellQuadList));
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

[Serializable]
public class CronenbergQuad
{
    public List<CronenCell> miniCronens = new List<CronenCell>();

    public List<Quad> cronenQuadList = new List<Quad>();

    public CronenbergQuad()
    {
        cronenQuadList = new List<Quad>();
    }

    public CronenbergQuad(Quad Q)
    {
        cronenQuadList = new List<Quad>();
        cronenQuadList.Add(Q);
    }

    public CronenbergQuad(Quad Q0, Quad Q1)
    {
        cronenQuadList = new List<Quad>();
        cronenQuadList.Add(Q0);
        cronenQuadList.Add(Q1);
    }

    public void EvalauteCronen(List<CronenbergQuad> masterList)
    {

        if(cronenQuadList.Count == 0)
        {
            masterList.Remove(this);
            return;
        }

        if (miniCronens.Count == 0)
        {
            CronenCell newCell = new CronenCell(cronenQuadList[0]);
            CheckAndAddtoCronen(newCell);
            miniCronens.Add(newCell);
        }

        if (miniCronens.Count != 0)
        {
            for (int i = 0; i < cronenQuadList.Count; i++)
            {
                BoolInt Status = CheckTheMinisForQuad(cronenQuadList[i]);

                if (!Status.Status)
                {
                    CronenCell newCell = new CronenCell(cronenQuadList[i]);
                    CheckAndAddtoCronen(newCell);
                    miniCronens.Add(newCell);
                }
            }
        }


        for (int u = 0; u < miniCronens.Count; u++)
        {
            for (int v = 0; v < miniCronens.Count; v++)
            {
                if (miniCronens[u] == miniCronens[v])
                    continue;

                if (miniCronens[u].CellQuadIntercepting(miniCronens[v]))
                {
                    miniCronens[u].MergeCells(miniCronens[v]);
                    if (miniCronens.Contains(miniCronens[v])) miniCronens.Remove(miniCronens[v]);

                    continue;
                }
            }
        }

        cronenQuadList.Clear();
        cronenQuadList = miniCronens[0].cellQuadList;

        for(int o = 1; o < miniCronens.Count; o++)
        {
            CronenbergQuad newBerg = new CronenbergQuad();
            newBerg.cronenQuadList = miniCronens[o].cellQuadList;

            if(!masterList.Contains(newBerg)) masterList.Add(newBerg);
        }

        miniCronens.Clear();
    }

    public void CheckAndAddtoCronen(CronenCell newCell)
    {
        for (int i = 0; i < cronenQuadList.Count; i++)
            newCell.IntersectingWithCell(cronenQuadList[i]);

        for (int i = 0; i < cronenQuadList.Count; i++)
            newCell.IntersectingWithCell(cronenQuadList[i]);
    }

    public bool CheckMiniForIntercection(int cellIndex, Quad _Q)
    {
        return miniCronens[cellIndex].IntersectingWithCell(_Q);
    }

    public void ColorCells (Color[] colors)
    {
        for (int i = 0; i < miniCronens.Count; i++)
        {
            for (int j = 0; j < miniCronens[i].cellQuadList.Count; j++)
            {
                Color col = colors[i % colors.Length];
                miniCronens[i].cellQuadList[j].quadColor = col;
            }
        }
    }

    public BoolInt CheckTheMinisForQuad(Quad _Q)
    {
        for (int i = 0; i < miniCronens.Count; i++)
        {
            if (miniCronens[i].CheckCellContains(_Q))
                return new BoolInt(i, true);
        }

        return new BoolInt(-1, false);
    }

    public void MergeCronenQuads(Quad Q0, Quad Q1, Quad Q3)
    {
        if (cronenQuadList.Contains(Q0) && cronenQuadList.Contains(Q1))
        {
            cronenQuadList.Remove(Q0);
            cronenQuadList.Remove(Q1);
            cronenQuadList.Add(Q3);
        }
    }

    public bool FractorCronenQuads(Quad Qinterseption, Quad Q1, List<CronenbergQuad> _list)
    {
        if (Qinterseption == Q1)
        {
            cronenQuadList.Remove(Q1);
        }

        if (cronenQuadList.Contains(Qinterseption))
        {
            CheckCronenContainsAndAdd(Q1);
            return true;
        }

        return false;
    }

    public bool CheckCronenContains(Quad Q)
    {
        for(int i = 0; i < cronenQuadList.Count; i++)
        {
            if (cronenQuadList.Contains(Q))
            return true;
        }

        return false;
    }

    public bool CheckCronenContainsAndAdd(Quad Q)
    {
        if (!cronenQuadList.Contains(Q))
        {
            cronenQuadList.Add(Q);
            return true;
        }
        return false;
    }

    public bool CheckCronenContainsAndRemove(Quad Q)
    {
        if (cronenQuadList.Contains(Q))
        {
            cronenQuadList.Remove(Q);
            return true;
        }
        return false;
    }

    public bool IntersectingWithCronen(Quad Q)
    {
        for (int i = 0; i < cronenQuadList.Count; i++)
        {
            if (Quad.intersectingQuad(cronenQuadList[i], Q))
            {     
                cronenQuadList.Add(Q);
                return true;
            }
        }

        return false;
    }

    public void Remove(Quad Q)
    {
        if (cronenQuadList.Contains(Q)) { cronenQuadList.Remove(Q); }
    }

    public void Add(Quad Q)
    {
        if (!cronenQuadList.Contains(Q)) { cronenQuadList.Add(Q); }
    }

    public static List<Quad> MergeCronens(List<Quad> left, List<Quad> right)
    {
        CronenbergQuad newCronen = new CronenbergQuad();

        for(int i = 0; i < left.Count; i++)
        {
            if(!newCronen.cronenQuadList.Contains(left[i]))
                newCronen.cronenQuadList.Add(left[i]);
        }

        for (int i = 0; i < right.Count; i++)
        {
            if (!newCronen.cronenQuadList.Contains(right[i]))
                newCronen.cronenQuadList.Add(right[i]);
        }

        return newCronen.cronenQuadList;
    }

    public bool CronenQuadIntercepting(CronenbergQuad right)
    {
        for (int i = 0; i < cronenQuadList.Count; i++)
        {
            for (int j = 0; j < right.cronenQuadList.Count; j++)
            {
                if (Quad.intersectingQuad(cronenQuadList[i], right.cronenQuadList[j]))
                {
                    //Debug.Log("Anus Pee Pee 2 2");
                    return true;
                }
            }
        }

        return false;
    }

    public bool CronenQuadAndMergeIntercepting(CronenbergQuad right)
    {
        for (int i = 0; i < cronenQuadList.Count; i++)
        {
            for (int j = 0; j < right.cronenQuadList.Count; j++)
            {
                if (Quad.intersectingQuad(cronenQuadList[i], right.cronenQuadList[j]))
                {
                    Debug.Log("Intercepting And Merging Cronens");
              
                    cronenQuadList = MergeCronens(cronenQuadList, right.cronenQuadList);
                    return true;
                }
            }
        }
        return false;
    }

    public static bool operator ==(CronenbergQuad left, CronenbergQuad right)
    {
        if (object.ReferenceEquals(left, null))
        {
            if (object.ReferenceEquals(right, null))
            {
                return true;
            }

            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(CronenbergQuad left, CronenbergQuad right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is CronenbergQuad))
            return false;
        else
            return (cronenQuadList.SequenceEqual(((CronenbergQuad)(obj)).cronenQuadList));
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class Cubil_Painter : MonoBehaviour
{
    public Color GridColor, EditorCubeColor, DebugCubilMergeColor;

    public Color[] cronenColors, customColors;

    Mesh CubilMesh;
    public GameObject Cubil;

    public Quad mockQuad, parametreQuad, intesectingQuad;

    public List<Quad> QuadList = new List<Quad>();

    List<Quad> IntersectingQuadList = new List<Quad>();

    public List<Quad> antiQuadList = new List<Quad>(); 

    public List<Vertex> intersectingVertices = new List<Vertex>(); public List<Vertex> antiVertices = new List<Vertex>();
    public List<Vertex> edgeVertices = new List<Vertex>();
    List<CronenbergQuad> intersectingCronens = new List<CronenbergQuad>();

    public List<CronenbergQuad> CronenbergList = new List<CronenbergQuad>();

    List<int> indexLeft = new List<int>(); List<int> indexRight = new List<int>();

    public float TotalArea;

    public int iterations;

    Vector3[,] Grid;

    Vector3 m_Input;

    void Awake()
    {
        parametreQuad = Quad.create(new Vector3(0,0,0), new Vector3(0,0,0));   
        mockQuad = Quad.create(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        CubilMesh = new Mesh();
        CreateGrid(25, 25);
    }

    void Update()
    {       
        UserInput();
    }

    void UserInput()
    {
        m_Input = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float sp_X = g_Utils.roundNearest(m_Input.x, m_Input.x / 15);
        float sp_Y = g_Utils.roundNearest(m_Input.y, m_Input.y / 15);
        float sp_Z = m_Input.z;//g_Utils.roundNearest(m_Input.z, (m_Input.z) % 15);

        Vector3 sp = new Vector3(sp_X + .5f, sp_Y + .5f, sp_Z);

        CheckForInterSectingQuads();

        if (g_Utils.pointInCube(m_Input, new Vector3(0, 0, 0), new Vector3(16, 16, 16)))
        {

            mockQuad.UpdateQuad(sp);
            FaceInPoint(sp);

            if (!FaceInPoint(sp))
            {
                if (Input.GetMouseButton(0))
                {
                    CreateNewQuad(sp);
                    CreateMesh();
                }
            }
            else if (FaceInPoint(sp))
            {
                if (Input.GetMouseButton(1))
                {
                    FractureQuads(sp);
                }
            }
        }

        for (int i = 0; i < iterations; i++)
        { QuadCheck(); SimplifyQuads(); }

        EvaluateEdges();
        EvaluateParametreQuad();
        ColorCronen();
        //ColorCronenCells();
    }

    void SimplifyQuads()
    {
        if (QuadList.Count > 1 && QuadList.Count != 0)
            {

            parametreQuad = new Quad(QuadList[0].centre, QuadList[0].vertexPoints[0].normal);

            //Do a Quad Check and Merge
            for (int i = 0; i < QuadList.Count; i++)
                {
                    AssignBorders(QuadList[i].vertexPoints);
                }
            }
    }

    void EvaluateParametreQuad()
    {
       if (parametreQuad.area == TotalArea && QuadList.Count > 2 && !Input.GetMouseButtonDown(1))
        {
            //Debug.Log("It Was All Me Baby!");
            QuadList.Clear();
            CronenbergList.Clear();
            QuadList.Add(parametreQuad);
            CronenbergList.Add(new CronenbergQuad(parametreQuad));
            CreateMesh();
        } 
    }

    void CalculateTotalQuadArea()
    {
        TotalArea = 0f;

        for (int i = 0; i < QuadList.Count; i++)
        {
            TotalArea += QuadList[i].area;
        }
    }

    void AssignBorders(Vertex[] q_verts)
    {
        float leftX = parametreQuad.vertexPoints[0].vertice.x;
        float rightX = parametreQuad.vertexPoints[1].vertice.x;
        float topY = parametreQuad.vertexPoints[3].vertice.y;
        float bottomY = parametreQuad.vertexPoints[1].vertice.y;

        for(int v = 0; v < q_verts.Length; v++)
        {
            if (q_verts[v].vertice.x < leftX)
            {
                leftX = q_verts[v].vertice.x;
            }

            if (q_verts[v].vertice.x > rightX)
            {
                rightX = q_verts[v].vertice.x;
            }

            if (q_verts[v].vertice.y < bottomY)
            {
                bottomY = q_verts[v].vertice.y;
            }

            if (q_verts[v].vertice.y > topY)
            {
                topY = q_verts[v].vertice.y;
            }
        }

        UpdateParemetreQuad(leftX, rightX, topY, bottomY);
        parametreQuad.CalculateQuadArea();
        CalculateTotalQuadArea();
    }

    void EvaluateEdges()
    {
        edgeVertices.Clear();

        if (QuadList.Count != 0)
        {
            for (int i = 0; i < QuadList.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    //Left Side
                    if (QuadList[i].vertexPoints[j].vertice.x == parametreQuad.vertexPoints[0].vertice.x)
                    {
                        if (!edgeVertices.Contains(QuadList[i].vertexPoints[j]))
                            edgeVertices.Add(QuadList[i].vertexPoints[j]);
                    }

                    //Right Side
                    if (QuadList[i].vertexPoints[j].vertice.x == parametreQuad.vertexPoints[3].vertice.x)
                    {
                        if (!edgeVertices.Contains(QuadList[i].vertexPoints[j]))
                            edgeVertices.Add(QuadList[i].vertexPoints[j]);
                    }

                    //Top Side
                    if (QuadList[i].vertexPoints[j].vertice.y == parametreQuad.vertexPoints[2].vertice.y)
                    {
                        if (!edgeVertices.Contains(QuadList[i].vertexPoints[j]))
                            edgeVertices.Add(QuadList[i].vertexPoints[j]);
                    }

                    //Bottom Side
                    if (QuadList[i].vertexPoints[j].vertice.y == parametreQuad.vertexPoints[1].vertice.y)
                    {
                        if (!edgeVertices.Contains(QuadList[i].vertexPoints[j]))
                            edgeVertices.Add(QuadList[i].vertexPoints[j]);
                    }
                }
            }
        }
    }

    void UpdateParemetreQuad(float leftX, float rightX, float topY, float bottomY)
    {
        parametreQuad.vertexPoints[0].vertice.x = leftX;
        parametreQuad.vertexPoints[0].vertice.y = bottomY;

        parametreQuad.vertexPoints[1].vertice.x = rightX;
        parametreQuad.vertexPoints[1].vertice.y = bottomY;

        parametreQuad.vertexPoints[2].vertice.x = leftX;
        parametreQuad.vertexPoints[2].vertice.y = topY;

        parametreQuad.vertexPoints[3].vertice.x = rightX;
        parametreQuad.vertexPoints[3].vertice.y = topY;
    }

    void CheckForInterSectingQuads()
    {
        if (QuadList.Count != 0)
        {
            for (int i = 0; i < QuadList.Count; i++)
            {
                if (IntersectingVertices(QuadList[i], mockQuad))
                {
                    if (!IntersectingQuadList.Contains(QuadList[i]))
                        IntersectingQuadList.Add(QuadList[i]);
                }
            }

            IntersectingQuadList.Clear();
        }
    }

    void FractureQuads(Vector3 sp)
    {
        Quad newAntiQuad = new Quad(sp, new Vector3(0, 0, -1));

        newAntiQuad.quadColor = Color.black;      
        antiQuadList.Add(newAntiQuad);
        FindBorders(newAntiQuad);
    }

    void FindBorders(Quad newAntiQuad)
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
        Vector3 vert = new Vector3(intesectingQuad.vertexPoints[index].vertice.x, _innerVertex.vertice.y, 1.299988f);
        Vertex Sidevertex = new Vertex(vert, _innerVertex.normal, _innerVertex.centre);

        return Sidevertex;
    }

    Vertex GetOppVertex(Vertex _innerVertex, int index)
    {
        Vector3 vert2 = new Vector3(_innerVertex.vertice.x, intesectingQuad.vertexPoints[index].vertice.y, 1.299988f);
        Vertex Oppvertex = new Vertex(vert2, _innerVertex.normal, _innerVertex.centre);

        return Oppvertex;
    }

    void CreateCornerFractures(Vertex _innerVertex, int index)
    {
        int flipindex = 3 - index;
        int oppIndex = (index % 2 == 0) ? index + 1 : index - 1;
        int sideIndex = (index + 2) % 4;

        Vector3 vert = new Vector3(intesectingQuad.vertexPoints[index].vertice.x, _innerVertex.vertice.y, 1.299988f);
        Vertex Sidevertex = new Vertex(vert, _innerVertex.normal, _innerVertex.centre);
        
        if(Mathf.Abs((_innerVertex.vertice.x - Sidevertex.vertice.x)) == 0) { return; }

        Vector3 vert2 = new Vector3(_innerVertex.vertice.x, intesectingQuad.vertexPoints[index].vertice.y, 1.299988f);
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
                            for (int k = 0; k < intersectingCronens.Count; k++)
                            {
                                if (CronenbergList.Contains(CronenbergList[_j])) CronenbergList.Remove(intersectingCronens[k]);
                            }
                        }
                    }

                    status = true;
                }
            }
        }

        intersectingCronens.Clear();
        return status;
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

    bool FaceInPoint(Vector3 sp)
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
                        if (!intersectingVertices.Contains( left.vertexPoints[i])) { intersectingVertices.Add(left.vertexPoints[i]); }
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

    void CreateNewQuad(Vector3 sp)
    {
        Quad newQuad = new Quad(sp, new Vector3(0, 0, -1));
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

    void QuadCheck()
    {
        Quad currentQuad = new Quad();
        Quad nextQuad = new Quad();

        if (QuadList.Count >= 2 && !Input.GetMouseButton(1))
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
                }
            }
        }
    
        CreateMesh();
    }

    void FindSquares()
    {
        //Find Sqaures By Comparing and Sorting threw Cronen outer points...yay
        for(int i = 0; i < CronenbergList.Count; i++)
        {
            for(int j = 0; j < CronenbergList[i].cronenQuadList.Count; j++)
            {

            }
        }
    }

    void CreateMesh()
    {
        CubilMesh = new Mesh();
        Vector3[] MeshNorms = new Vector3[QuadList.Count * 4];
        Vector3[] MeshVerts = new Vector3[QuadList.Count * 4];
        int[] meshTris = new int[QuadList.Count * 6];

        //Normals and Verts UnPacking
        for(int i = 0; i < QuadList.Count; i++)
        {
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
            DrawGrid();
            DrawEditorCube();

            mockQuad.DrawQuad(.1f);

           //if (parametreQuad.vertexPoints[0] != null)
             //  parametreQuad.DrawQuad(.1f);

            for (int i = 0; i < antiVertices.Count; i ++)
            {
                //Gizmos.color = Color.magenta;
                //
                //Gizmos.DrawSphere(antiVertices[i + 0].vertice, .15f);
            }

            if (QuadList.Count != 0)
            {
                for (int i = 0; i < QuadList.Count; i++)
                    QuadList[i].DrawQuad(.1f);
            }

            if (intersectingVertices.Count != 0)
            {
                for (int i = 0; i < intersectingVertices.Count; i++)
                    Gizmos.DrawSphere(intersectingVertices[i].vertice, .1f);
            }

            if (edgeVertices.Count != 0)
            {
                for (int i = 0; i < edgeVertices.Count; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(edgeVertices[i].vertice, .1f);
                }
            }

            //for (int i = 0; i < antiQuadList.Count; i++)
            //    antiQuadList[i].DrawQuad(.1f);

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
