using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class V_Painter : MonoBehaviour
{

    //public Shader m_Shader = null;
    public Material Voronoi_Diagram_Material;

    RenderTexture screenRT;
    RenderTexture finalRT;
    RenderTexture depthRT;
    public Texture BrushDistort;

    public RenderTexture OutPut;

    public Shader VORONOI_PAINTER_SHADER;

    public Camera Cam;
    // Use this for initialization
    void Awake()
    {
        screenRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
        screenRT.wrapMode = TextureWrapMode.Repeat;

        finalRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
        finalRT.wrapMode = TextureWrapMode.Repeat;

        depthRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Depth);
        depthRT.wrapMode = TextureWrapMode.Repeat;
 
        depthRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Depth);
        depthRT.wrapMode = TextureWrapMode.Repeat;


        VORONOI_PAINTER_SHADER = Shader.Find("Custom/Gene/PostEffect/Voronoi_Diagram");
        Voronoi_Diagram_Material = new Material(VORONOI_PAINTER_SHADER);

      
       Cam = GetComponent<Camera>();
       Cam.depthTextureMode = DepthTextureMode.DepthNormals;
       Cam.SetTargetBuffers(screenRT.colorBuffer, depthRT.depthBuffer);
       Cam.cullingMask &= ~(1 << LayerMask.NameToLayer("OilPanter"));

        if (VORONOI_PAINTER_SHADER && Voronoi_Diagram_Material)
        {
            Cam.targetTexture = OutPut;
        }
    }

    void OnPostRender()
    {
            //Final = Screen + Depth
   
        screenRT.DiscardContents();
        finalRT.DiscardContents();
        OutPut.DiscardContents();
    }

    void Update()
    {
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (VORONOI_PAINTER_SHADER && Voronoi_Diagram_Material)
        {
            Graphics.Blit(src, dst, Voronoi_Diagram_Material);
            Voronoi_Diagram_Material.SetTexture("_Brush_Map_Tex", (Texture)BrushDistort);
            Voronoi_Diagram_Material.mainTexture = screenRT;

        }
    }
}