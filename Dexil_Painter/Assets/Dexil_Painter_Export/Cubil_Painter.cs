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

    public static Mesh CubilMesh;
    public static GameObject Cubil;

    public Cubil selectionCube;

    Vector3[,] Grid;

    Vector3 m_Input;

    public static Vector3 sp;

    public static bool pointInCube;
   
    public bool ColorRegions, DEBUG, UPDATE;

    public Color GridColor, EditorCubeColor, DebugCubilMergeColor, QuadColor;

    public Cubil_Face_Manager[] faceManagers = new Cubil_Face_Manager[6];

    public List<Vector3> finalVerticesList = new List<Vector3>();
    public List<Vector3> finalNormalList = new List<Vector3>();
    public List<int> finalTrianglesList = new List<int>();

    void Awake()
    {
        selectionCube = new Cubil(new Vector3(0,0,0));
       
        CreateGrid(25, 25);

        CubilMesh = new Mesh();
        Cubil = FindObjectOfType<MeshFilter>().gameObject;

        InitializeManagers();
    }

    void InitializeManagers()
    {
        for (int i = 0; i < 2; i++)
        {
            faceManagers[i] = new Cubil_Face_Manager();
        }

        faceManagers[0].FaceManagerAwake(CubilMesh, Cubil, new Vector3(0,0,+1), Direction.FRONT , Color.blue);
        faceManagers[1].FaceManagerAwake(CubilMesh, Cubil, new Vector3(0,0,-1), Direction.BACK  , Color.blue);

        faceManagers[2].FaceManagerAwake(CubilMesh, Cubil, new Vector3(-1,0,0), Direction.LEFT  , Color.red);
        faceManagers[3].FaceManagerAwake(CubilMesh, Cubil, new Vector3(+1,0,0), Direction.RIGHT , Color.red);
        
        faceManagers[4].FaceManagerAwake(CubilMesh, Cubil, new Vector3(0,+1,0), Direction.TOP   , Color.green);
        faceManagers[5].FaceManagerAwake(CubilMesh, Cubil, new Vector3(0,-1,0), Direction.BOTTOM, Color.green);

    }

    void PaintCubeFaces()
    {
        for (int i = 0; i < 6; i++)
        {
            faceManagers[i].FaceConstruction();
        }
    }

    void DrawCubeFaces()
    {
        for (int i = 0; i < 6; i++)
        {
            faceManagers[i].FaceDrawGizmos();
        }
    }

    void Update()
    {
        UserInput();
        PaintCubeFaces();
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
            PaintCubeFaces();
          

            if (Input.GetMouseButton(0))
            {
                UPDATE = true;
            }
        
            if (Input.GetMouseButton(1))
            {
                UPDATE = true;
            }

        }

        if (UPDATE)
        {
            CreateMesh();
            UPDATE = false;
        }
    }

    bool ZeroCheck <T>(T[] list)
    {
        if (list.ToList().Count != 0) return true;

        return false;
    }

   void CreateMesh()
   {
        CubilMesh = new Mesh();

        int QuadListCountFinal = faceManagers[0].QuadList.Count +
                                 faceManagers[1].QuadList.Count +
                                 faceManagers[2].QuadList.Count +
                                 faceManagers[3].QuadList.Count +
                                 faceManagers[4].QuadList.Count +
                                 faceManagers[5].QuadList.Count;

        Vector3[] finalNormalList = new Vector3[QuadListCountFinal * 4];
        Vector3[] finalVerticesList = new Vector3[QuadListCountFinal * 4];
        int[] finalTrianglesList = new int[QuadListCountFinal * 6];
        List<Quad> finalQuadList = new List<Quad>();

        finalQuadList.AddRange(faceManagers[0].QuadList);
        finalQuadList.AddRange(faceManagers[1].QuadList);
        finalQuadList.AddRange(faceManagers[2].QuadList);
        finalQuadList.AddRange(faceManagers[3].QuadList);
        finalQuadList.AddRange(faceManagers[4].QuadList);
        finalQuadList.AddRange(faceManagers[5].QuadList);

        Vector2[] uvs = new Vector2[17 * 17];

        //Normals and Verts UnPacking

            for (int i = 0; i < finalQuadList.Count; i++)
            {
                uvs[i] = new Vector2(i / (float)16, i / (float)16);
                finalVerticesList[( i * 4) + 0] = finalQuadList[i].vertexPoints[0].vertice;
                finalVerticesList[( i * 4) + 1] = finalQuadList[i].vertexPoints[1].vertice;
                finalVerticesList[( i * 4) + 2] = finalQuadList[i].vertexPoints[2].vertice;
                finalVerticesList[( i * 4) + 3] = finalQuadList[i].vertexPoints[3].vertice;
                finalNormalList[(   i * 4) + 0] = finalQuadList[i].vertexPoints[0].normal;
                finalNormalList[(   i * 4) + 1] = finalQuadList[i].vertexPoints[1].normal;
                finalNormalList[(   i * 4) + 2] = finalQuadList[i].vertexPoints[2].normal;
                finalNormalList[(   i * 4) + 3] = finalQuadList[i].vertexPoints[3].normal;
                finalTrianglesList[(i * 6) + 0] = finalQuadList[i].t1.indexArray[0] + (i * 4);
                finalTrianglesList[(i * 6) + 1] = finalQuadList[i].t1.indexArray[1] + (i * 4);
                finalTrianglesList[(i * 6) + 2] = finalQuadList[i].t1.indexArray[2] + (i * 4);                       
                finalTrianglesList[(i * 6) + 3] = finalQuadList[i].t2.indexArray[0] + (i * 4);
                finalTrianglesList[(i * 6) + 4] = finalQuadList[i].t2.indexArray[1] + (i * 4);
                finalTrianglesList[(i * 6) + 5] = finalQuadList[i].t2.indexArray[2] + (i * 4);
            }

        CubilMesh.vertices =  finalVerticesList;
        CubilMesh.normals =   finalNormalList;
        CubilMesh.triangles = finalTrianglesList;

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
            DrawCubeFaces();          
            //Vector3 intersection = transform.position + (ray.direction * rayLength);
            //Gizmos.DrawSphere(intersection, .25f);

            //Debug.Log(sp2);
            selectionCube.DrawCubil(0.1f);

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
