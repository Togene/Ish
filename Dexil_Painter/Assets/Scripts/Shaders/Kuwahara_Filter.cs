using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class Kuwahara_Filter : MonoBehaviour
{

    //public Shader m_Shader = null;
    public Material Oil_Mat;

    [Range(1, 10)]
    public int Iterations = 1;
    Vector3 camScreen;
    public RenderTexture Outline_Texture;
    public RenderTexture DT_Texture;
    // Use this for initialization
    RenderTexture screenRT;
    void Start()
    {
        screenRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
        screenRT.wrapMode = TextureWrapMode.Repeat;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {

        Oil_Mat.SetTexture("_Outline_Tex", Outline_Texture);
        Oil_Mat.SetTexture("_DT_Tex", DT_Texture);
        Oil_Mat.mainTexture = screenRT;
        //render
        RenderTexture rt = RenderTexture.GetTemporary(src.width, src.height);
        Graphics.Blit(src, rt);


        for(int i = 0; i < Iterations; i++)
        {
            RenderTexture rt2 = RenderTexture.GetTemporary(rt.width, rt.height);
            Graphics.Blit(rt, rt2, Oil_Mat);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }

            Graphics.Blit(rt, dst, Oil_Mat);
            RenderTexture.ReleaseTemporary(rt);
    }
    
}