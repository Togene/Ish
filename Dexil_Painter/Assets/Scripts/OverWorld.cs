using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class OverWorld : MonoBehaviour
{
    public static OverWorld oWorld_instance = null;
    public int worldSize = 100;
    public int tileW, tileH;
    public WorldTileInfo[,] overWorldGrid;
    public WorldData worldData;


    public static OverWorld instance
    {
        get
        {
            if(oWorld_instance == null)
            {
                //Magic Time Baby
                oWorld_instance = FindObjectOfType(typeof(OverWorld)) as OverWorld;
            }

            if(oWorld_instance == null)
            {
                GameObject obj = new GameObject("OverWorld Manager");
                oWorld_instance = obj.AddComponent(typeof(OverWorld)) as OverWorld;
                Debug.Log("Couldnt Find OverWorld Manager, So I made a new one");
            }

            return oWorld_instance;
        }
    }

    void OnApplicationQuit()
    {
        oWorld_instance = null;
    }

	void Awake ()
    {
        GenereteWorld();
        worldData.createWorld();
        
    }

    void GenereteWorld()
    {
  
            //Initilizing/Creating World Data
            overWorldGrid = new WorldTileInfo[worldSize, worldSize];

            for (int i = 0; i < worldSize; i++)
            {
                for (int j = 0; j < worldSize; j++)
                {
                    overWorldGrid[i % worldSize, j % worldSize].tileCol = g_Utils.RandomColor();
                    overWorldGrid[i % worldSize, j % worldSize].tileID = Mathf.RoundToInt(UnityEngine.Random.Range(0, 255));
                    overWorldGrid[i % worldSize, j % worldSize].centre.x = (i * tileW);
                    overWorldGrid[i % worldSize, j % worldSize].centre.y = (j * tileH);

                    overWorldGrid[i % worldSize, j % worldSize].sizeX = tileW;
                    overWorldGrid[i % worldSize, j % worldSize].sizeY = tileH;

                    overWorldGrid[i % worldSize, j % worldSize].setup();
                }
            }
    }
}

public struct WorldTileInfo
{
    public int x1;
    public int y1;
    public int x2;
    public int y2;
    public int x3;
    public int y3;
    public int x4;
    public int y4;

    public Vector2 centre;

    public int sizeX, sizeY;
    public int tileID;

    public Color tileCol;

    public void setup()
    {

     int sXHalf = sizeX / 2;
     int sYHalf = sizeY / 2;

     x1 = Mathf.RoundToInt(centre.x - sXHalf);
     y1 = Mathf.RoundToInt(centre.y + sYHalf);
     x2 = Mathf.RoundToInt(centre.x + sXHalf);
     y2 = Mathf.RoundToInt(centre.y + sYHalf);                 
     x3 = Mathf.RoundToInt(centre.x + sXHalf);
     y3 = Mathf.RoundToInt(centre.y - sYHalf);                
     x4 = Mathf.RoundToInt(centre.x - sXHalf);
     y4 = Mathf.RoundToInt(centre.y - sYHalf);
    }

    public void DrawTile(Color col)
    {
        //Gizmos.color = col;
        //
        //Vector3 c = new Vector3(centre.x, centre.y, Camera.main.farClipPlane - 1);
        //
        //Vector3 p1 = new Vector3(x1, y1, Camera.main.farClipPlane - 1);
        //Vector3 p2 = new Vector3(x2, y2, Camera.main.farClipPlane - 1);
        //Vector3 p3 = new Vector3(x3, y3, Camera.main.farClipPlane - 1);
        //Vector3 p4 = new Vector3(x4, y4, Camera.main.farClipPlane - 1);
        //
        //Gizmos.color = Color.black;
        //Gizmos.DrawLine(p1, p2);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(p2, p3);
        //Gizmos.color = Color.magenta;
        //Gizmos.DrawLine(p3, p4);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(p4, p1);
        //
        ////Gizmos.DrawSphere(p1, .1f);
        ////Gizmos.DrawSphere(p2, .1f);
        ////Gizmos.DrawSphere(p3, .1f);
        ////Gizmos.DrawSphere(p4, .1f);
        //
        //Gizmos.color = Color.green;
        ////Gizmos.DrawSphere(c, .1f);
    }
}

public struct WorldData
{
    public int[,] worldData;

    public void createWorld()
    {
        worldData = NewWorld();
    }

     int[,] NewWorld()
    {
        int[,] newWorld = new int[,]
        {
            {1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 2, 2, 1, 1, 1},
            {1, 1, 1, 2, 2, 1, 1, 1},
            {1, 1, 1, 2, 2, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1}
        };

        return newWorld;
    }
}