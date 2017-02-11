using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UI_CONTROL : MonoBehaviour 
{
    public static bool editMode;

    public GameObject Edit_Border;
    public Button editButton;
    public Sprite_Paint SprPaint;
    public List<Button> tileIcon = new List<Button>();
    public Button curButton;
    public Button iconButton;
    public GameObject editBorder;

    void Awake ()
    {
        SprPaint = FindObjectOfType<Sprite_Paint>();
    }
	
	public void Start ()
	{
        for(int i = 0; i < 9; i++)
        {
            Button b = Instantiate(iconButton);
            b.transform.position = new Vector3(iconButton.transform.position.x, 400 - (i * 35f) + 5, 1);
            b.onClick.AddListener(() => ChangeSpritePaint());
            b.transform.SetParent(editBorder.transform);
            b.name = i.ToString();
            b.image.sprite = getTile(i % 3, i / 3);
            tileIcon.Add(b);
        }
	}
	
    public void ChangeSpritePaint()
    {
        Debug.Log("Sprite Changed");
        Sprite imgPull = (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite);

        Texture2D croppedTexture = new Texture2D(16, 16);

        Color[] pixels = new Color[(int)imgPull.rect.width * (int)imgPull.rect.height];

         pixels = imgPull.texture.GetPixels((int)imgPull.rect.x,
                                            (int)imgPull.rect.y,
                                            (int)imgPull.rect.width,
                                            (int)imgPull.rect.height);

        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();
        SprPaint.ChangePaint(croppedTexture);
    }

    Sprite getTile(int i, int j)
    {
        Texture2D imgLoad = Resources.Load("TileMaps/Tile_Sheet_00") as Texture2D;
        return g_Utils.getSprite(imgLoad, i, j, 16, 16);
    }

	public void Edit()
    {
        editMode = !editMode;
    }

	void Update () 
	{

        if(editMode)
        {
            Edit_Border.SetActive(true);
        }
        else
        {
            Edit_Border.SetActive(false);
        }
	}

	void OnDrawGizmos()
	{
	}
}
