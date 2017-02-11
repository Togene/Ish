using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Distance_JPA : MonoBehaviour
{

    //public Shader m_Shader = null;
    public Material DT_JPA;
    public RenderTexture JPA_STORE;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, DT_JPA);
    }

}