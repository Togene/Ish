using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class Distance_SnapShot : MonoBehaviour
{
    public Material DT_SnapeShot;
    public RenderTexture JPA_SNAPSHOT;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, DT_SnapeShot);
    }

}