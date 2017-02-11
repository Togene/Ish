using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class renderBox
{
    public Vector2 centre;
    public Vector2 topLeft;
    public Vector2 topRight;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;

    public byte sizeW;
    public byte sizeH;

    public int size;

    public renderBox()
    {
        centre = Camera.main.transform.position;

        Vector2 centreScreen = Camera.main.WorldToViewportPoint(centre);

        topLeft = Camera.main.ViewportToWorldPoint(new Vector2(-0.5f, 0.5f) + centreScreen);

        topRight =      Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f) + centreScreen);

        bottomLeft =    Camera.main.ViewportToWorldPoint(new Vector2(-0.5f, -0.5f) + centreScreen);

        bottomRight =   Camera.main.ViewportToWorldPoint(new Vector2(0.5f, -0.5f) + centreScreen);

        sizeW = (byte)Mathf.RoundToInt(Mathf.Abs(bottomLeft.x - bottomRight.x));
        sizeH = (byte)Mathf.RoundToInt(Mathf.Abs(bottomLeft.y - topLeft.y));

        size = sizeW * sizeH;
    }

    void readData()
    {
        //OverWorld.overWorldGrid.
    }

    public void updateBox()
    {
        centre = Camera.main.transform.position;

        Vector2 centreScreen = Camera.main.WorldToViewportPoint(centre);

        topLeft = Camera.main.ViewportToWorldPoint(new Vector2(-0.5f, 0.5f) + centreScreen);

        topRight = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f) + centreScreen);

        bottomLeft = Camera.main.ViewportToWorldPoint(new Vector2(-0.5f, -0.5f) + centreScreen);

        bottomRight = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, -0.5f) + centreScreen);

        sizeW = (byte)Mathf.RoundToInt(Mathf.Abs(bottomLeft.x - bottomRight.x));
        sizeH = (byte)Mathf.RoundToInt(Mathf.Abs(bottomLeft.y - topLeft.y));

        size = sizeW * sizeH;
    }


}

public class CamWorldView : MonoBehaviour
{
    public static renderBox viewBox;
    public SpriteRenderer worldSprRnd;
    public int inti;
    public Sprite tile00, tile01, tile02, tile03;
    public GameObject[,] viewPortData;

    void Start()
    {
        viewBox = new renderBox();
        viewPortData = new GameObject[(Mathf.RoundToInt((viewBox.sizeH/16) * 2)), (Mathf.RoundToInt((viewBox.sizeW / 16) * 2))];

        for (int i = 0; i < Mathf.RoundToInt(viewBox.sizeH/16) * 2; i+= 1)
        {
            for (int j = 0; j < Mathf.RoundToInt(viewBox.sizeW/16) * 2; j += 1)
            {
                //GameObject obj = new GameObject();
                //obj.transform.position = (new Vector3(((i * 16)) - viewBox.sizeH, ((j * 16)) - viewBox.sizeW, 10));
                //obj.transform.parent = this.transform;
                //SpriteRenderer sprRnd = obj.AddComponent<SpriteRenderer>();
                //sprRnd.sprite = tile00;
                ////sprRnd.material.color = g_Utils.RandomColor();
                //viewPortData[i, j] = obj;
                //inti++;
                //obj.name = inti.ToString();
            }
        }

    }
	
	void Update ()
    {
        viewBox.updateBox();
        //DrawTile();
        //
        //Vector3 mPosView1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //GameObject curObj1 = viewPortData[2, 2].gameObject;
        //Vector3 curObjPos1 = Camera.main.ScreenToViewportPoint(curObj1.transform.position);


        for (int i = 0; i < Mathf.RoundToInt(viewBox.sizeH / 16) * 2; i += 1)
        {
            for (int j = 0; j < Mathf.RoundToInt(viewBox.sizeW / 16) * 2; j += 1)
            {
               //GameObject curObj = viewPortData[i, j].gameObject;
               //
               //Vector3 w_p1 = new Vector3(curObj.transform.position.x - 8, 
               //                           curObj.transform.position.y - 8, 
               //                           curObj.transform.position.z);
               //
               //Vector3 w_p2 = new Vector3(curObj.transform.position.x + 8,
               //                           curObj.transform.position.y + 8,
               //                           curObj.transform.position.z);
               //
               //
               //Vector3 p1 = w_p1;//Camera.main.ScreenToViewportPoint(w_p1);
               //Vector3 p2 = w_p2;// Camera.main.ScreenToViewportPoint(w_p2);
               //
               //Vector3 mPosView = Camera.main.ScreenToWorldPoint(Input.mousePosition);
               //
               //SpriteRenderer curObjSprRnd = curObj.GetComponent<SpriteRenderer>();
               //
               //if (g_Utils.pointInRect(mPosView.x, mPosView.y, p2, p1) && Input.GetMouseButton(0))
               //{
               //    Debug.Log("Anus");
               //    //Color
               //    //curObjSprRnd.material.color = new Color(curObjSprRnd.material.color.r - 1.0f, curObjSprRnd.material.color.g -1.0f, curObjSprRnd.material.color.b - 1.0f, 1.0f);
               //    curObjSprRnd.sprite = tile03;
               //}
               //else
               //{
               //    //curObjSprRnd.material.color = Color.white;
               //}
            }
        }
    }

    void OnDrawGizmos()
    {

        if (Application.isPlaying)
        {
            for (int i = 0; i < Mathf.RoundToInt(viewBox.sizeH / 16) * 2; i++)
            {
                for (int j = 0; j < Mathf.RoundToInt(viewBox.sizeW / 16) * 2; j++)
                {
                    
                        //Gizmos.color = Color.red;
                        //
                        //GameObject curObj = viewPortData[i, j].gameObject;
                        //Vector3 w_p1 = new Vector3(curObj.transform.position.x,
                        //                           curObj.transform.position.y,
                        //                           curObj.transform.position.z);
                        //
                        //Vector3 w_p2 = new Vector3(curObj.transform.position.x,
                        //                           curObj.transform.position.y,
                        //                           curObj.transform.position.z);
                        //
                        //
                        //Vector3 p1 = Camera.main.ScreenToViewportPoint(w_p1);
                        //Vector3 p2 = Camera.main.ScreenToViewportPoint(w_p2);
                        //
                       //// Gizmos.DrawCube(w_p1, new Vector3(16.0f, 16.0f, 1.0f));
  
                }
            }

        }
    }
}
