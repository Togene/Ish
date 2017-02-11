using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class Distance_Transform : MonoBehaviour
{
    public Material DT_Final;


    public Texture RampText;
    Vector3 camScreen;
    // Use this for initialization
    public RenderTexture Outline_Texture;
    public Texture Noise_Texture;
    public GameObject canvas;
    void OnValidate()
    {
       
        Shader.SetGlobalTexture("_Outline_Tex", Outline_Texture);
        Shader.SetGlobalTexture("_Noise", Noise_Texture);
        //Shader.SetGlobalTexture("_JPA_Store", JPA_STORE);
        //Shader.SetGlobalTexture("_SNAPSHOT_Store", JPA_SNAPSHOT);
        Shader.SetGlobalTexture("_RampTex", RampText);
        //canvas.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = JPA_Snap2;
    }
    
    void Update()
    {
        //JPA_Snap2 = getTexture2DFromRenderTexture(Outline_Texture);
        Vector2 M_Pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        DT_Final.SetVector("_M", M_Pos);

    }

    public Texture2D getTexture2DFromRenderTexture(RenderTexture rTex)
    {
    
        Texture2D texture2D = new Texture2D(rTex.width, rTex.height);
        RenderTexture.active = rTex;
        texture2D.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        texture2D.Apply();
        return texture2D;

    }
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
       
        //Graphics.Blit(Outline_Texture, JPA_STORE, DT_JPA);
        //Graphics.Blit(JPA_STORE, JPA_SNAPSHOT, DT_SnapeShot);
        Graphics.Blit(src, dst, DT_Final);
        //DT_Final.mainTexture = screenRT;
    }

}