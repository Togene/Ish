using UnityEngine;
using System.Collections;

public class Sprite_Paint : MonoBehaviour 
{
    public GameObject Map, m_Icon;
    public Map_Sprite_Editor mapTMP2D;
    public int ResX, ResY, iteration;

    int x, y, oldX, oldY;
    bool update = true, skip;
    Color[,] brushColor, curTileColor;
    Vector2 pixelUV;

    void Awake ()
	{
        mapTMP2D = FindObjectOfType<Map_Sprite_Editor>();

        brushColor = new Color[ResX, ResY];
        curTileColor = new Color[ResX, ResY];
    }

    public void ChangePaint(Texture2D img)
    {
        m_Icon.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = img;
        img.filterMode = FilterMode.Point;
        for (int i = 0; i < ResX; i++)
        {
            for (int j = 0; j < ResY; j++)
            {
                brushColor[i, j] = img.GetPixel(i, j);
            }
        }
    }

	void Update () 
	{
        if (UI_CONTROL.editMode)
        {
            EditMap();
        }
	}

    void EditMap()
    {

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            pixelUV = hit.textureCoord;
            pixelUV.x *= mapTMP2D.mapTMP.width;
            pixelUV.y *= mapTMP2D.mapTMP.height;
            x = g_Utils.roundNearest(pixelUV.x - 8, mapTMP2D.mapTMP.width / (float)10);
            y = g_Utils.roundNearest(pixelUV.y - 8, mapTMP2D.mapTMP.height / (float)10);
            //if Index Has Changed, update that shit

            if (Input.GetMouseButton(0))
            {
                PaintBlock(x, y);
                iteration = 0;
                update = true;
            }
            else
            {
                if (update)
                {
                    GetPaintBlockInfo(x, y);
                    update = false;
                }

                if (oldX != x && iteration >= 1
                 || oldY != y && iteration >= 1)
                {
                    ResetPaintBlock(oldX, oldY);
                    update = true;
                }
            }
        }

        else
        {
            if (iteration >= 1)
            {
                ResetPaintBlock(oldX, oldY);
                update = true;
            }
        }


    }

    void PaintBlock(int _x, int _y)
    {
        for (int i = 0; i < ResX; i++)
        {
            for (int j = 0; j < ResY; j++)
            {
                //Then Apply HighLight to Area
                mapTMP2D.mapTMP.SetPixel(_x + i, _y + j, brushColor[i, j]);
            }
        }

        mapTMP2D.mapTMP.Apply();
    }

    void GetPaintBlockInfo(int _x, int _y)
    {
        for (int i = 0; i < ResX; i++)
        {
            for (int j = 0; j < ResY; j++)
            {
                //Get Current Tile Colors Before Applying
                curTileColor[i, j] = mapTMP2D.mapTMP.GetPixel(_x + i, _y + j);
                
                //Then Apply HighLight to Area
                mapTMP2D.mapTMP.SetPixel(_x + i, _y + j, Color.red * curTileColor[i, j]);
            }
        }

        //Saving Prevois Pixel Data and index
        CaptureBlockInfo(_x, _y, curTileColor);
        mapTMP2D.mapTMP.Apply();
        iteration++;
    }

    void CaptureBlockInfo(int _x, int _y, Color[,] col)
    {
        oldX = _x;
        oldY = _y;       

        curTileColor = col;
    }

    void ResetPaintBlock(int oldx, int oldy)
    {
        for (int i = 0; i < ResX; i++)
        {
            for (int j = 0; j < ResY; j++)
            {
                mapTMP2D.mapTMP.SetPixel(oldx + i, oldy + j, curTileColor[i, j]);
            }
        }

        mapTMP2D.mapTMP.Apply();
    }

    void Log<T>(T _log)
    {
         Debug.Log(_log);
    }

    void OnDrawGizmos()
	{
        if(Application.isPlaying)
        {
        
        }
	}
}
