using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Cock
/// </summary>
[Serializable]
public struct BoolInt
{
    public int I;
    public bool Status;

    public BoolInt(int i, bool status)
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
                if (!cellQuadList.Contains(Q)) cellQuadList.Add(Q);
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
    public List<Vertex> CronenEdgeVertices = new List<Vertex>();
    public List<Vertex> vertexList = new List<Vertex>();
    public Color cronenColor;
    public Quad CronenConvexQuad;
    public float TotalCronenArea;

    public CronenbergQuad()
    {
        cronenQuadList = new List<Quad>();
        CronenConvexQuad = new Quad();
    }

    public CronenbergQuad(Quad Q)
    {
        cronenQuadList = new List<Quad>();
        cronenQuadList.Add(Q);
        CronenConvexQuad = new Quad();
    }

    public CronenbergQuad(Quad Q0, Quad Q1)
    {
        cronenQuadList = new List<Quad>();
        cronenQuadList.Add(Q0);
        cronenQuadList.Add(Q1);
        CronenConvexQuad = new Quad();
    }

    public void EvalauteCronen(List<CronenbergQuad> masterList)
    {

        if (cronenQuadList.Count == 0)
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

        for (int o = 1; o < miniCronens.Count; o++)
        {
            CronenbergQuad newBerg = new CronenbergQuad();

            for (int j = 0; j < miniCronens[o].cellQuadList.Count; j++)
            {
                if (!newBerg.cronenQuadList.Contains(miniCronens[o].cellQuadList[j]))
                    newBerg.cronenQuadList.Add(miniCronens[o].cellQuadList[j]);

                newBerg.SetConvexQuad();
            }

            if (!masterList.Contains(newBerg)) masterList.Add(newBerg);
        }

        cronenQuadList.Clear();

        for (int j = 0; j < miniCronens[0].cellQuadList.Count; j++)
        {
            if (!cronenQuadList.Contains(miniCronens[0].cellQuadList[j]))
                cronenQuadList.Add(miniCronens[0].cellQuadList[j]);
        }

        miniCronens[0].cellQuadList[0].vertexPoints.CopyTo(CronenConvexQuad.vertexPoints, 0);
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

    public void ColorCells(Color[] colors)
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

            if (cronenQuadList.Count == 0)
            {
                _list.Remove(this);
                return false;
            }
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
        for (int i = 0; i < cronenQuadList.Count; i++)
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

    public bool CheckPointInQuadExludingBorder(Vector3 v)
    {
        for (int i = 0; i < cronenQuadList.Count; i++)
        {
            if (cronenQuadList[i].inFaceExcludeBorder(v))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckPointInQuad(Vector3 v)
    {
        for (int i = 0; i < cronenQuadList.Count; i++)
        {
            if (cronenQuadList[i].inFace(v))
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckCroneneConvexInBounds(Vector3 c)
    {
        return true;
    }

    public bool CheckVertexMatch(Vector3 v)
    {
        for (int i = 0; i < cronenQuadList.Count; i++)
        {
            if (Quad.MatchVerts(cronenQuadList[i], (v)))
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckPointPinchInCronen(Vector3 v, int numQuads, bool noVert)
    {
        //Check Pinches Without Verts
        if (noVert)
        {
            int counter = 0;

            for (int i = 0; i < cronenQuadList.Count; i++)
            {
                if (cronenQuadList[i].inFace(v) && !Quad.MatchVerts(cronenQuadList[i], v))
                {
                    counter++;

                    if (counter >= numQuads)
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            //Check Pinches With Verts 
            int counter = 0;
            int vertCounter = 0;

            for (int i = 0; i < cronenQuadList.Count; i++)
            {
                if (cronenQuadList[i].inFace(v))
                {
                    counter++;

                    if (Quad.MatchVerts(cronenQuadList[i], v))
                    {
                        vertCounter++;
                    }

                    //if vert counter is the same as the number of Quads
                    //that means that all corners are touching
                    if (counter >= numQuads && vertCounter != counter)
                    {
                        return true;
                    }
                }
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

        for (int i = 0; i < left.Count; i++)
        {
            if (!newCronen.cronenQuadList.Contains(left[i]))
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

    public void EvaluateCronenEdges()
    {
        CronenEdgeVertices.Clear();

        CreateCheckGrid();
    }

    public void CreateCheckGrid()
    {
        for (float i = CronenConvexQuad.vertexPoints[0].vertice.x; i <= CronenConvexQuad.vertexPoints[1].vertice.x; i += 0.25f)
        {
            for (float j = CronenConvexQuad.vertexPoints[1].vertice.y; j <= CronenConvexQuad.vertexPoints[3].vertice.y; j += 0.25f)
            {
                Vertex newVert = new Vertex();
                Vector3 vertice = new Vector3(i, j, CronenConvexQuad.vertexPoints[0].vertice.z);
                newVert.vertice = vertice;
                newVert.col = Color.magenta;

                if ((!CheckPointInQuadExludingBorder(vertice)) && (!CheckPointPinchInCronen(vertice, 2, true)))
                {
                    if (CheckPointInQuad(vertice))
                    {
                        if (!CheckPointPinchInCronen(vertice, 3, false))
                        {
                            if (CheckVertexMatch(vertice))
                            {
                                if (!CronenEdgeVertices.Contains(newVert)) CronenEdgeVertices.Add(newVert);
                            }

                        }
                    }
                }
            }
        }
    }

    public void CalculateTotalQuadArea()
    {
        TotalCronenArea = 0f;

        for (int i = 0; i < cronenQuadList.Count; i++)
        {
            TotalCronenArea += cronenQuadList[i].area;
        }
    }

    public void CheckConvexQuad(List<Quad> _MasterQuadList)
    {
        //if (CronenConvexQuad.area == TotalCronenArea && cronenQuadList.Count > 2 && !Input.GetMouseButtonDown(1))
        //{
        //    Debug.Log("It Was All Me Baby In The Cronen!");
        //
        //    Quad newQuad = new Quad();
        //
        //    for (int i = 0; i < cronenQuadList.Count; i++)
        //    {
        //        _MasterQuadList.Remove(cronenQuadList[i]);
        //    }
        //    cronenQuadList.Clear();
        //
        //    CronenConvexQuad.vertexPoints.CopyTo(newQuad.vertexPoints, 0);
        //    if (!cronenQuadList.Contains(CronenConvexQuad)) cronenQuadList.Add(CronenConvexQuad);
        //    if (!_MasterQuadList.Contains(CronenConvexQuad)) _MasterQuadList.Add(CronenConvexQuad);
        //    SetConvexQuad();
        //}
    }

    public void CalculateConvexInformation()
    {
        CronenConvexQuad = new Quad();

        if (cronenQuadList.Count <= 1)
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
        CronenConvexQuad = new Quad();
        cronenQuadList[0].vertexPoints.CopyTo(CronenConvexQuad.vertexPoints, 0);
        CronenConvexQuad.quadColor = Color.green;
    }

    public void EvalauteConvexQuad()
    {
        float leftx = CronenConvexQuad.vertexPoints[0].vertice.x;
        float rightx = CronenConvexQuad.vertexPoints[3].vertice.x;
        float topy = CronenConvexQuad.vertexPoints[2].vertice.y;
        float bottomy = CronenConvexQuad.vertexPoints[1].vertice.y;

        for (int i = 0; i < cronenQuadList.Count; i++)
        {
            for (int j = 0; j < cronenQuadList[i].vertexPoints.Length; j++)
            {
                Vertex vertPoint = cronenQuadList[i].vertexPoints[j];

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

        Vector3 norm = cronenQuadList[0].vertexPoints[0].normal;
        Vector3 centre = cronenQuadList[0].vertexPoints[0].centre;

        float z = cronenQuadList[0].vertexPoints[0].vertice.z;

        Vertex vert0 = new Vertex(new Vector3(leftx, bottomy, z), norm, centre);
        Vertex vert1 = new Vertex(new Vector3(rightx, bottomy, z), norm, centre);
        Vertex vert2 = new Vertex(new Vector3(leftx, topy, z), norm, centre);
        Vertex vert3 = new Vertex(new Vector3(rightx, topy, z), norm, centre);

        CronenConvexQuad.vertexPoints[0] = vert0;

        CronenConvexQuad.vertexPoints[1] = vert1;

        CronenConvexQuad.vertexPoints[2] = vert2;

        CronenConvexQuad.vertexPoints[3] = vert3;

        CronenConvexQuad.CalculateQuadArea();
    }

    public void UpdateCronenVertices(Vector3 c)
    {
        for(int i = 0; i < cronenQuadList.Count; i++)
        {
            cronenQuadList[i].UpdateQuad(c);
        }
    }

    public void DrawConvexQuad()
    {
        if (CronenConvexQuad != null)
        {
            CronenConvexQuad.DrawQuad(.1f);
        }
    }

    public void DrawEdgeVertices(Color[] cols)
    {
        for (int i = 0; i < CronenEdgeVertices.Count; i++)
        {
            Gizmos.color = CronenEdgeVertices[i].col;
            //Gizmos.color = cols[i % cols.Length];
            Gizmos.DrawCube(CronenEdgeVertices[i].vertice, new Vector3(.2f, .2f, .2f));
        }
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

