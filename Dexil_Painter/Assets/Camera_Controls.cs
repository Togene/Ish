using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controls : MonoBehaviour {

    public bool x;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        ManageCamera();

    }

    #region CameraControls
    //---------------------------- CameraControls -----------------------------------------------------
    void ManageCamera()
    {
        if (Input.GetKeyDown(KeyCode.Keypad5))
            Camera.main.orthographic = !Camera.main.orthographic;

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            x = !x;
            RotateCameraY(-(Mathf.PI / 2));
        }

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            x = !x;
            RotateCameraY(+Mathf.PI / 2);
        }

        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            if (!x)
                RotateCameraX(+Mathf.PI / 2);
            else
                RotateCameraZ(+Mathf.PI / 2);
        }


        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (!x)
                RotateCameraX(-Mathf.PI / 2);
            else
                RotateCameraZ(-Mathf.PI / 2);
        }

    }

    void RotateCameraY(float angle)
    {

        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);

        float dx = transform.position.x - 8f;
        float dz = transform.position.z - 8f;

        float x1 = (dx * c) - (dz * s);
        float z1 = (dz * c) + (dx * s);

        Vector3 newVec = new Vector3(x1 + 8, transform.position.y, z1 + 8);
        transform.position = newVec;

        transform.LookAt(new Vector3(8, 8, 8));
    }

    void RotateCameraX(float angle)
    {
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);

        float dy = transform.position.y - 8f;
        float dz = transform.position.z - 8f;

        float y1 = (dy * c) - (dz * s);
        float z1 = (dz * c) + (dy * s);

        Vector3 newVec = new Vector3(transform.position.x, y1 + 8, z1 + 8);
        transform.position = newVec;

        transform.LookAt(new Vector3(8, 8, 8));
    }

    void RotateCameraZ(float angle)
    {
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);

        float dy = transform.position.y - 8f;
        float dx = transform.position.x - 8f;

        float y1 = (dy * c) - (dx * s);
        float x1 = (dx * c) + (dy * s);

        Vector3 newVec = new Vector3(x1 + 8, y1 + 8, transform.position.z);
        transform.position = newVec;

        transform.LookAt(new Vector3(8, 8, 8));
    }
    //---------------------------- CameraControls -----------------------------------------------------
    #endregion

}
