using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Cubil_Painter : MonoBehaviour
{

    [Range(0, 6)]
    public int Resolution = 0;

    Mesh CubilMesh;
    GameObject Cubil;

    Vector3[,] Grid;

    Vector3 m_Input;

    public static Vector3 sp;

    public static bool pointInCube;
   
    public bool ColorRegions, DEBUG;

    public List<Quad> QuadList = new List<Quad>();

    public Color GridColor, EditorCubeColor, DebugCubilMergeColor, QuadColor;

    void Awake()
    {
        CubilMesh = new Mesh();
        CreateGrid(25, 25);
        Cubil = FindObjectOfType<MeshFilter>().gameObject;
    }

    void Update()
    {
        UserInput();
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
        sp = ManageMouseInput();

        pointInCube = g_Utils.pointInCube(sp, new Vector3(0, 0, 0), new Vector3(16, 16, 16));

        if (pointInCube)
        {
           // SelectedQuad.SetQuad(sp);
        }
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
        if(Application.isPlaying && DEBUG)
        {
            //Vector3 intersection = transform.position + (ray.direction * rayLength);
            //Gizmos.DrawSphere(intersection, .25f);

            //Debug.Log(sp2);

            DrawGrid();
            DrawEditorCube();
         
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
    #endregion
}
