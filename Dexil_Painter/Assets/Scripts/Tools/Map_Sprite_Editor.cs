using UnityEngine;
using System.Collections;

public class Map_Sprite_Editor : MonoBehaviour 
{
    public Texture2D mapTMP;
    public Texture2D loadedTexture;

    public bool autoUpdate;

    void Awake ()
	{
        if (Resources.Load("Textures/Saved_Map") as Texture2D != null)
        {
            loadedTexture = Resources.Load("Textures/Saved_Map") as Texture2D;
            loadedTexture.filterMode = FilterMode.Point;
            loadedTexture.Apply();
        }
        else
        NewMap();
    }
	
	void Start ()
	{
        mapTMP.SetPixels(loadedTexture.GetPixels());
        mapTMP.Apply();
        this.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = mapTMP;
    }
			
	void Update () 
	{
	}

    public void Undo()
    {
        mapTMP.SetPixels(loadedTexture.GetPixels());
        mapTMP.Apply();
        this.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = mapTMP;
    }

    public void Reset()
    {
    }

    public void Load()
    {
  
    }

    public void Save()
    {
        GetComponent<PNGUploader>().UploadPNG(mapTMP);
    }

    public void NewMap()
    {

        mapTMP = new Texture2D(16 * 10, 16 * 10);
        mapTMP.name = "Saved_Map";
        mapTMP.filterMode = FilterMode.Point;
        mapTMP.Apply();

        GetComponent<PNGUploader>().UploadPNG(mapTMP);
        this.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = mapTMP;
    }


    void OnApplicationQuit()
    {
        this.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = loadedTexture;
    }

    void OnDrawGizmos()
	{
	}

}
