﻿using UnityEngine;
using System.Collections;
using System;

public class Triangle
{
    public int[] indexArray = new int[3];

    public Triangle(int i0, int i1, int i2)
    {
        indexArray[0] = i0;
        indexArray[1] = i1;
        indexArray[2] = i2;
    }
}

public class Vertex
{
    public Vector3 originalPos;
    public Vector3 vertice;
    public Vector3 normal;
    public Vector3 centre;
    Color col;

    public Vertex() { }

    public Vector3 Vertice
    {
        get { return vertice; } set { vertice = value; }
    }

    public Vertex(Vector3 _vert, Vector3 _norm, Vector3 _c)
    {
        vertice = _vert;
        originalPos = _vert;
        centre = _c;
        normal = _norm;
    }

    public float GetLength()
    {
        return 1f;
    }

    public void UpdateVert(Vector3 c)
    {
        //Translation of Vertex
        Vertice = originalPos + c;
    }

    public static bool operator ==(Vertex left, Vertex right)
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

    public static bool operator !=(Vertex left, Vertex right)
    {
        return !(left == right);
    }
   
   public override bool Equals(object obj)
   {
       if (obj == null || !(obj is Vertex))
           return false;
       else
           return (vertice == ((Vertex)obj).vertice &&
                   normal == ((Vertex)obj).normal);
   }
   
   public override int GetHashCode()
   {
       return base.GetHashCode();
   }
}

[Serializable]
public class Quad
{
    public Vertex[] vertexPoints = new Vertex[4];
    public Triangle t1, t2;
    public Color quadColor;
    public Vector3 centre;
    public Quad q;
    public float area;

    public Quad(){ CreateTris();}

    public static Quad create(Vector3 _c, Vector3 _dir)
    {
        Quad obj = new Quad();

        obj.vertexPoints = CreateVerts(_c, _dir);
        obj.centre = _c;
        obj.CreateTris();
        obj.quadColor = Color.white;
        obj.CalculateQuadArea();
        return obj;
    }

    public Quad (Vector3 _c, Vector3 _dir)
    {
       vertexPoints = CreateVerts(_c, _dir);
       centre = _c;
       CreateTris();
        CalculateQuadArea();
       quadColor = Color.white;
    }

    ~Quad()
    {
        //Debug.Log("I DED");
    }

    public void CreateTris()
    {
        t1 = new Triangle(0, 3, 1);
        t2 = new Triangle(0, 2, 3);
    }

    public void CalculateQuadArea()
    {
        area = Mathf.Abs(vertexPoints[0].vertice.x - vertexPoints[1].vertice.x) *
               Mathf.Abs(vertexPoints[0].vertice.y - vertexPoints[2].vertice.y);
    }

    public void DrawQuad(float size)
    {

        Gizmos.color = quadColor;
        //Drawing Vertices and Normals
        Gizmos.DrawSphere(vertexPoints[0].vertice, size);
        Gizmos.DrawLine(vertexPoints[0].vertice, vertexPoints[0].vertice + vertexPoints[0].normal);

        Gizmos.DrawSphere(vertexPoints[1].vertice, size);
        Gizmos.DrawLine(vertexPoints[1].vertice, vertexPoints[1].vertice + vertexPoints[1].normal);

        Gizmos.DrawSphere(vertexPoints[2].vertice, size);
        Gizmos.DrawLine(vertexPoints[2].vertice, vertexPoints[2].vertice + vertexPoints[2].normal);

        Gizmos.DrawSphere(vertexPoints[3].vertice, size);
        Gizmos.DrawLine(vertexPoints[3].vertice, vertexPoints[3].vertice + vertexPoints[3].normal);

        //Drawing Traingles
        Gizmos.DrawLine(vertexPoints[t1.indexArray[0]].vertice, vertexPoints[t1.indexArray[1]].vertice);
        Gizmos.DrawLine(vertexPoints[t1.indexArray[1]].vertice, vertexPoints[t1.indexArray[2]].vertice);
        Gizmos.DrawLine(vertexPoints[t1.indexArray[2]].vertice, vertexPoints[t1.indexArray[0]].vertice);

        Gizmos.DrawLine(vertexPoints[t2.indexArray[0]].vertice, vertexPoints[t2.indexArray[1]].vertice);
        Gizmos.DrawLine(vertexPoints[t2.indexArray[1]].vertice, vertexPoints[t2.indexArray[2]].vertice);
        Gizmos.DrawLine(vertexPoints[t2.indexArray[2]].vertice, vertexPoints[t2.indexArray[0]].vertice);
    }

    public void UpdateQuad(Vector3 c)
    {
        vertexPoints[0].UpdateVert(c);
        vertexPoints[1].UpdateVert(c);
        vertexPoints[2].UpdateVert(c);
        vertexPoints[3].UpdateVert(c);
    }

    public Quad MergeQuads(Quad right, int[] leftIndices, Color _color)
    {
        Quad newQuad = Quad.create(centre, new Vector3(0, 0, 0)); //new Quad(centre, new Vector3(0, 0, 0));

        newQuad.vertexPoints = vertexPoints;
        newQuad.quadColor = _color;

        for (int i = 0; i < leftIndices.Length; i++)
        {
            newQuad.vertexPoints[leftIndices[i]] = right.vertexPoints[leftIndices[i]];
        }

        newQuad.CalculateQuadArea();

        return newQuad;
    }

    public bool inFace(Vector3 _p)
    {
        bool status = false;

        if (g_Utils.pointInRect(_p.x, _p.y, vertexPoints[0].vertice, vertexPoints[3].vertice))
        { status = true; }
        return status;
    }

    public static bool intersectingQuad(Quad _l, Quad _r)
    {
        if (g_Utils.rectIntersect(_l, _r))
        { return true; }

        return false;
    }

    public static Vertex[] CreateVerts(Vector3 c, Vector3 _dir)
    {
        Vertex[] VertTemp = new Vertex[]
        {
            new Vertex(new Vector3(-.5f, -.5f, .5f) + c, _dir, c), //0
            new Vertex(new Vector3(+.5f, -.5f, .5f) + c, _dir, c), //1
            new Vertex(new Vector3(-.5f, +.5f, .5f) + c, _dir, c), //2
            new Vertex(new Vector3(+.5f, +.5f, .5f) + c, _dir, c)  //3 
        };

        return VertTemp;
    }

    public static bool operator ==(Quad left, Quad right)
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

    public static bool operator !=(Quad left, Quad right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Quad))
            return false;
        else
            return (vertexPoints[0] == ((Quad)obj).vertexPoints[0] &&
                    vertexPoints[1] == ((Quad)obj).vertexPoints[1] &&
                    vertexPoints[2] == ((Quad)obj).vertexPoints[2] &&
                    vertexPoints[3] == ((Quad)obj).vertexPoints[3]) ;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

