using UnityEngine;
using System.Collections;
using System.IO;

public class PNGUploader : MonoBehaviour
{
    //Take a shot immeditely
    // IEnumerator Start()
    //{
    // yield return UploadPNG();
    // }

    public void UploadPNG(Texture2D tex)
    {
        //We should only read the screen buffer after rendering is complete
        //Create
        tex.filterMode = FilterMode.Point;
        byte[] bytes = tex.EncodeToPNG();
        string texName = tex.name + ".png";
        //For testing purposes, also write to a gile in the project folder
        File.WriteAllBytes(Application.dataPath + "/../Assets/Resources/Textures/" + texName, bytes);
        //Create a Web Form
        WWWForm form = new WWWForm();
        form.AddBinaryData("fileUpload", bytes);

        //Upload to a cgi script??????
        WWW w = new WWW("http://localhost/cgi-bin/env.cgi?post", form);
        //yield return w;


        if(w.error != null)
        {
            Debug.Log(w.error);
        }
        else
        {
            Debug.Log("FINISHED");
        }
    }
}
