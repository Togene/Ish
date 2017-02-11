using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class Kuwahara : MonoBehaviour
{

 //public Shader m_Shader = null;
    private Material m_Material;

    RenderTexture screenRT;
    RenderTexture finalRT;
    RenderTexture depthRT;
    public Texture2D m_noiseTextre;

    Shader SSAO_Shader;

    public Camera mainCam;

    // Use this for initialization
    void Awake()
    {
        screenRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
        screenRT.wrapMode = TextureWrapMode.Repeat;

        finalRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
        finalRT.wrapMode = TextureWrapMode.Repeat;

        depthRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Depth);
        depthRT.wrapMode = TextureWrapMode.Repeat;
        //depthRT.DepthTextureMode = DepthTextureMode.DepthNormals;

        depthRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Depth);
        depthRT.wrapMode = TextureWrapMode.Repeat;


        SSAO_Shader = Shader.Find("Custom/Gene/PostEffect/OilPainter");
        m_Material = new Material(SSAO_Shader);

        mainCam = GetComponent<Camera>();
        mainCam.depthTextureMode = DepthTextureMode.DepthNormals;
        mainCam.SetTargetBuffers(screenRT.colorBuffer, depthRT.depthBuffer);
        mainCam.cullingMask &= ~(1 << LayerMask.NameToLayer("OilPanter"));
    }

    void OnPostRender()
    {
        //Final = Screen + Depth
        screenRT.DiscardContents();
        finalRT.DiscardContents();
    }

    void Update()
    {
      
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (SSAO_Shader && m_Material)
        {
            Graphics.Blit(src, dst, m_Material);
           // Graphics.Blit(src, screenRT, m_Material);

            m_Material.mainTexture = screenRT;
           // m_Material.SetTexture("_NoiseTex", m_noiseTextre);
        }
    }
}