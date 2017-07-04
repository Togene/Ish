using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public enum Direction
{
    FRONT,
    BACK,
    LEFT,
    RIGHT,
    TOP,
    BOTTOM,
    NULL
};

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

[Serializable]
public class Vertex
{
    public Vector3 originalPos;
    public Vector3 vertice;
    public Vector3 normal;
    public Vector3 centre;
    public Color col;

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
        vertice = originalPos + c;
        originalPos = vertice;
    }

    public void SetVert(Vector3 c)
    {
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


//Quad Structure Going From Left To Right

//     2--------3
//     |        |
//     |        |
//     |        |
//     0--------1
//
// Using Left To Right Method !!!!!!!!!!

[Serializable]
public class Quad
{
    public Vertex[] vertexPoints = new Vertex[4];
    public Triangle t1, t2;
    public Color quadColor;
    public Vector3 centre;
    public List<Vector3> centrePoints = new List<Vector3>();
    public List<Vector3> antiPoints = new List<Vector3>();
    public Direction faceDirection;
    public Quad q;
    public int area;

    public Quad(){ CreateTris(); vertexPoints = VertexData.CreateVertices(Direction.NULL, new Vector3(0,0,0), new Vector3(0, 0, 0)); }

    public void CalculateCentrePoints()
    {
        CalculateQuadArea();

        centrePoints = VertexData.CalculateCentrePoints(faceDirection, vertexPoints, area);
    }

    public static Quad create(Vector3 _c, Vector3 _n, Direction dir, Color _color)
    {
        Quad obj = new Quad();

        obj.faceDirection = dir;
        obj.quadColor = _color;
        obj.vertexPoints = VertexData.CreateVertices(dir, _c, _n);
        obj.centre = _c;
        obj.CreateTris();
        //obj.quadColor = Color.white;
        obj.CalculateQuadArea();

        return obj;
    }

    public Vertex[] CreateTestVertices(Vector3 _c, Vector3 _n)
    {                                                                            //
        return VertexData.CreateVertices(faceDirection, _c, _n);
    }                                                                            //

    //public Quad (Vector3 _c, Vector3 _n, Direction dir)
    //{
    //   vertexPoints = VertexData.CreateVertices(dir, _c, _n); //CreateVerts(_c, _dir);
    //   centre = _c;
    //   CreateTris();
    //   CalculateQuadArea();
    //   //quadColor = Color.white;
    //}

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
        area = VertexData.CalculateQuadArea(faceDirection, vertexPoints);
    }

    public void DrawQuad(float size)
    {

        Gizmos.color = quadColor;
        //Drawing Vertices and Normals
        //Gizmos.DrawSphere(vertexPoints[0].vertice, size);
        //Gizmos.DrawLine(vertexPoints[0].vertice, vertexPoints[0].vertice + vertexPoints[0].normal);
        //
       //// Gizmos.DrawSphere(vertexPoints[1].vertice, size);
        //Gizmos.DrawLine(vertexPoints[1].vertice, vertexPoints[1].vertice + vertexPoints[1].normal);
        //
        ////Gizmos.DrawSphere(vertexPoints[2].vertice, size);
        //Gizmos.DrawLine(vertexPoints[2].vertice, vertexPoints[2].vertice + vertexPoints[2].normal);
        //
       //// Gizmos.DrawSphere(vertexPoints[3].vertice, size);
        //Gizmos.DrawLine(vertexPoints[3].vertice, vertexPoints[3].vertice + vertexPoints[3].normal);

        //Drawing Traingles
        Gizmos.DrawLine(vertexPoints[t1.indexArray[0]].vertice, vertexPoints[t1.indexArray[1]].vertice);
        Gizmos.DrawLine(vertexPoints[t1.indexArray[1]].vertice, vertexPoints[t1.indexArray[2]].vertice);
        Gizmos.DrawLine(vertexPoints[t1.indexArray[2]].vertice, vertexPoints[t1.indexArray[0]].vertice);

        Gizmos.DrawLine(vertexPoints[t2.indexArray[0]].vertice, vertexPoints[t2.indexArray[1]].vertice);
        Gizmos.DrawLine(vertexPoints[t2.indexArray[1]].vertice, vertexPoints[t2.indexArray[2]].vertice);
        Gizmos.DrawLine(vertexPoints[t2.indexArray[2]].vertice, vertexPoints[t2.indexArray[0]].vertice);

       // Gizmos.DrawSphere(this.centre, size);


        if (centrePoints.Count > 0)
        {
            for (int i = 0; i < centrePoints.Count; i++)
            {
                Gizmos.DrawSphere(centrePoints[i], size);
            }
        }
    }

    public void UpdateQuad(Vector3 c)
    {
        vertexPoints[0].UpdateVert(c);
        vertexPoints[1].UpdateVert(c);
        vertexPoints[2].UpdateVert(c);
        vertexPoints[3].UpdateVert(c);
    }

    public void SetQuad(Vector3 c)
    {
        vertexPoints[0].SetVert(c);
        vertexPoints[1].SetVert(c);
        vertexPoints[2].SetVert(c);
        vertexPoints[3].SetVert(c);
    }

    bool BorderCheck(Vector3 v, Vector3 b0, Vector3 t0)
    {
        return (g_Utils.pointInCube(v, b0, t0));
    }

    public void CalculateCentre()
    {
        centre = VertexData.CalculateCentrePoint(faceDirection, vertexPoints);
    }

    public Quad MergeQuads(Quad right, int[] leftIndices, Color _color)
    {
        Quad newQuad = create(centre, vertexPoints[0].normal, faceDirection, quadColor);

        newQuad.vertexPoints = vertexPoints;
        newQuad.quadColor = _color;

        //Replacing
        for (int i = 0; i < leftIndices.Length; i++)
        {
            newQuad.vertexPoints[leftIndices[i]] = right.vertexPoints[leftIndices[i]];
        }

        newQuad.CalculateQuadArea();
        newQuad.CalculateCentrePoints();
        newQuad.CalculateCentre();
        return newQuad;
    }

    public bool inFace(Vector3 _p)
    {
        return (g_Utils.pointInRect(_p.x, _p.y, _p.z, vertexPoints[0].vertice, vertexPoints[3].vertice, faceDirection));
    }

    public static bool MatchVerts(Quad _q, Vector3 _p)
    {
        for (int i = 0; i < _q.vertexPoints.Length; i++)
        {
            if (_q.vertexPoints[i].vertice == _p)
            {
                return true;
            }
        }
        return false;
    }

    public bool inFaceExcludeBorder(Vector3 _p)
    {
        return (g_Utils.pointInRectExcludeBorder(_p.x, _p.y, vertexPoints[0].vertice, vertexPoints[3].vertice));
    }

    public static bool intersectingQuad(Quad _l, Quad _r)
    {
        return (g_Utils.rectIntersect(_l, _r));
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

[Serializable]
public class Cubil
{
    public Quad[] faces = new Quad[6];
    public Vector3 centre;

    public void SetCube(Vector3 _c)
    {
        for(int i = 0; i < 6; i++)
        {
            faces[i].SetQuad(_c);
        }
    }

    public Cubil(Vector3 _c)
    {
        centre = _c;

        //Creating Front Facing Quad
        faces[0] = Quad.create(_c, new Vector3(0, 0, -1), Direction.FRONT, Color.blue);
        faces[1] = Quad.create(_c, new Vector3(0, 0, +1), Direction.BACK, Color.blue);
        faces[2] = Quad.create(_c, new Vector3(-1, 0, 0), Direction.LEFT, Color.red);                                                
        faces[3] = Quad.create(_c, new Vector3(+1, 0, 0), Direction.RIGHT, Color.red);
        faces[4] = Quad.create(_c, new Vector3(0, +1, 0), Direction.TOP, Color.green);
        faces[5] = Quad.create(_c, new Vector3(0, -1, 0), Direction.BOTTOM, Color.green);
    }

    public void DrawCubil(float size)
    {
        for (int i = 0; i < 6; i++)
        {
            faces[i].DrawQuad(size);
        }
    }

}

public class VertexData
{
    public static Vertex[] CreateVertices(Direction faceDirection, Vector3 _c, Vector3 _n)
    {
        if (faceDirection == Direction.FRONT)
        {
            return FrontFaceVertexData(_c, _n);
        }
        else if (faceDirection == Direction.BACK)
        {
            return BackFaceVertexData(_c, _n);
        }
        else if (faceDirection == Direction.LEFT)
        {
            return LeftFaceVertexData (_c, _n);
        }
        else if (faceDirection == Direction.RIGHT)
        {
            return RightFaceVertexData(_c, _n);
        }
        else if (faceDirection == Direction.TOP)
        {
            return TopFaceVertexData(_c, _n);
        }
        else if (faceDirection == Direction.BOTTOM)
        {
            return BottomFaceVertexData(_c, _n);
        }
        else

        {
            return new Vertex[]
            {
            new Vertex(new Vector3(0, 0, 0.0f),new Vector3(0, 0, 0.0f), new Vector3(0, 0, 0.0f)), //0
            new Vertex(new Vector3(0, 0, 0.0f),new Vector3(0, 0, 0.0f), new Vector3(0, 0, 0.0f)), //1
            new Vertex(new Vector3(0, 0, 0.0f),new Vector3(0, 0, 0.0f), new Vector3(0, 0, 0.0f)), //2
            new Vertex(new Vector3(0, 0, 0.0f),new Vector3(0, 0, 0.0f), new Vector3(0, 0, 0.0f))  //3 
            };
        }

    }


    public static int CalculateQuadArea(Direction faceDirection, Vertex[] vertexPoints)
    {
        if (faceDirection == Direction.FRONT || faceDirection == Direction.BACK)
        {
            return (int)(Mathf.Abs(vertexPoints[0].vertice.x - vertexPoints[1].vertice.x) *
                     Mathf.Abs(vertexPoints[0].vertice.y - vertexPoints[2].vertice.y));
        }
        else if (faceDirection == Direction.LEFT || faceDirection == Direction.RIGHT)
        {
            return (int)(Mathf.Abs(vertexPoints[0].vertice.z - vertexPoints[1].vertice.z) *
                         Mathf.Abs(vertexPoints[0].vertice.y - vertexPoints[2].vertice.y));
        }
        else if (faceDirection == Direction.TOP || faceDirection == Direction.BOTTOM)
        {
            return (int)(Mathf.Abs(vertexPoints[0].vertice.x - vertexPoints[1].vertice.x) *
                         Mathf.Abs(vertexPoints[0].vertice.z - vertexPoints[2].vertice.z));
        }
        else
        {
            return 0;
        }
    }

    public static Vector3 CalculateCentrePoint(Direction faceDirection, Vertex[] vertexPoints)
    {
        if (faceDirection == Direction.FRONT || faceDirection == Direction.BACK)
        {
            return new Vector3(Mathf.Abs((vertexPoints[0].vertice.x + vertexPoints[1].vertice.x) / 2),
                                      Mathf.Abs((vertexPoints[0].vertice.y + vertexPoints[2].vertice.y) / 2),
                                      vertexPoints[0].vertice.z);
        }
        else if (faceDirection == Direction.LEFT || faceDirection == Direction.RIGHT)
        {
            return new Vector3(Mathf.Abs((vertexPoints[0].vertice.x + vertexPoints[1].vertice.x) / 2),
                                      vertexPoints[0].vertice.y,
                                 Mathf.Abs((vertexPoints[0].vertice.z + vertexPoints[1].vertice.z) / 2));
        }
        else if (faceDirection == Direction.TOP || faceDirection == Direction.BOTTOM)
        {
            return new Vector3(vertexPoints[0].vertice.x,
                                  Mathf.Abs((vertexPoints[0].vertice.y + vertexPoints[2].vertice.y) / 2),
                                  Mathf.Abs((vertexPoints[0].vertice.z + vertexPoints[1].vertice.z) / 2));
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    public static List<Vector3> CalculateCentrePoints(Direction faceDirection, Vertex[] vertexPoints, int area)
    {
        List<Vector3> centrePoints = new List<Vector3>(new Vector3[area]);

        if (centrePoints.Count != 0)
        {
            if (faceDirection == Direction.FRONT || faceDirection == Direction.BACK)
            {
                float width = Mathf.Abs((vertexPoints[1].vertice.x - vertexPoints[0].vertice.x));
                float height = Mathf.Abs((vertexPoints[3].vertice.y - vertexPoints[1].vertice.y));

                for (int i = 0; i < (int)width; i++)
                {
                    for (int j = 0; j < (int)height; j++)
                    {
                        centrePoints[i + (j * (int)width)] = new Vector3(i + vertexPoints[0].vertice.x + 0.5f, j + vertexPoints[0].vertice.y + 0.5f, vertexPoints[0].vertice.z);
                    }
                }
            }
            else if (faceDirection == Direction.LEFT || faceDirection == Direction.RIGHT)
            {
                float width = Mathf.Abs((vertexPoints[1].vertice.z - vertexPoints[0].vertice.z));
                float height = Mathf.Abs((vertexPoints[3].vertice.y - vertexPoints[1].vertice.y));

                for (int i = 0; i < (int)width; i++)
                {
                    for (int j = 0; j < (int)height; j++)
                    {
                        centrePoints[i + (j * (int)width)] = new Vector3(vertexPoints[0].vertice.x, j + vertexPoints[0].vertice.y + 0.5f, i + vertexPoints[0].vertice.z + 0.5f);
                    }
                }
            }
            else if (faceDirection == Direction.TOP || faceDirection == Direction.BOTTOM)
            {
                float width = Mathf.Abs((vertexPoints[1].vertice.x - vertexPoints[0].vertice.x));
                float height = Mathf.Abs((vertexPoints[3].vertice.z - vertexPoints[1].vertice.z));

                for (int i = 0; i < (int)width; i++)
                {
                    for (int j = 0; j < (int)height; j++)
                    {
                        centrePoints[i + (j * (int)width)] = new Vector3(i + vertexPoints[0].vertice.x + 0.5f, vertexPoints[0].vertice.y, j + vertexPoints[0].vertice.z + 0.5f);
                    }
                }
            }
        }
        return centrePoints;
    }

    private static Vertex[] FrontFaceVertexData(Vector3 _c, Vector3 _n)
    {
        return new Vertex[]
        {
            new Vertex(new Vector3(-.5f, -.5f, 0.0f) + _c, _n, _c), //0
            new Vertex(new Vector3(+.5f, -.5f, 0.0f) + _c, _n, _c), //1
            new Vertex(new Vector3(-.5f, +.5f, 0.0f) + _c, _n, _c), //2
            new Vertex(new Vector3(+.5f, +.5f, 0.0f) + _c, _n, _c)  //3 
        };
    }

    private static Vertex[] BackFaceVertexData(Vector3 _c, Vector3 _n)
    {
        return new Vertex[]
        {                           
            new Vertex(new Vector3(-.5f, -.5f, 1f) + _c, _n, _c), //0
            new Vertex(new Vector3(+.5f, -.5f, 1f) + _c, _n, _c), //1
            new Vertex(new Vector3(-.5f, +.5f, 1f) + _c, _n, _c), //2
            new Vertex(new Vector3(+.5f, +.5f, 1f) + _c, _n, _c)  //3 
        };
    }

    private static Vertex[] LeftFaceVertexData(Vector3 _c, Vector3 _n)
    {
        return new Vertex[]
        {
            new Vertex(new Vector3(-0.5f, -.5f,   0f) + _c, _n, _c), //0
            new Vertex(new Vector3(-0.5f, -.5f,  1f) + _c, _n, _c), //1
            new Vertex(new Vector3(-0.5f, +.5f,   0f) + _c, _n, _c), //2
            new Vertex(new Vector3(-0.5f, +.5f,  1f) + _c, _n, _c)  //3 
        };
    }

    private static Vertex[] RightFaceVertexData(Vector3 _c, Vector3 _n)
    {
        return new Vertex[]
        {
            new Vertex(new Vector3(.5f, -.5f,   0f) + _c, _n, _c), //0
            new Vertex(new Vector3(.5f, -.5f,  1f) + _c, _n, _c), //1
            new Vertex(new Vector3(.5f, +.5f,   0f) + _c, _n, _c), //2
            new Vertex(new Vector3(.5f, +.5f,  1f) + _c, _n, _c)  //3  
        };
    }

    private static Vertex[] TopFaceVertexData(Vector3 _c, Vector3 _n)
    {
        return new Vertex[]
        {
            new Vertex(new Vector3(-.5f, .5f, 0f) + _c, _n, _c), //0
            new Vertex(new Vector3(+.5f, .5f, 0f) + _c, _n, _c), //1
            new Vertex(new Vector3(-.5f, .5f, 1f) + _c, _n, _c), //2
            new Vertex(new Vector3(+.5f, .5f, 1f) + _c, _n, _c)  //3 
        };
    }

    private static Vertex[] BottomFaceVertexData(Vector3 _c, Vector3 _n)
    {
        return new Vertex[]
        {
            new Vertex(new Vector3(-.5f, -.5f, 0f) + _c, _n, _c), //0
            new Vertex(new Vector3(+.5f, -.5f, 0f) + _c, _n, _c), //1
            new Vertex(new Vector3(-.5f, -.5f, 1f) + _c, _n, _c), //2
            new Vertex(new Vector3(+.5f, -.5f, 1f) + _c, _n, _c)  //3 
        };
    }
}  