using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public struct Matrix3D
{
    public float r0c0, r0c1, r0c2;
    public float r1c0, r1c1, r1c2;
    public float r2c0, r2c1, r2c2;

    public Matrix3D(float _r0c0 = 1f, float _r0c1 = 0f, float _r0c2 = 0f, 
                    float _r1c0 = 0f, float _r1c1 = 1f, float _r1c2 = 0f,
                    float _r2c0 = 0f, float _r2c1 = 0f, float _r2c2 = 1f)
    {
        r0c0 = _r0c0;
        r0c1 = _r0c1;
        r0c2 = _r0c2;
        r1c0 = _r1c0;
        r1c1 = _r1c1;
        r1c2 = _r1c2;
        r2c0 = _r2c0;
        r2c1 = _r2c1;
        r2c2 = _r2c2;
    }

    public void DebugMatrix()
    {
        //Debug.Log("r0c0 : " + r0c0);
    }

    public Matrix3D RotateZ(float angle)
    {
        float Cosine = Mathf.Cos(angle);
        float Sine = Mathf.Sin(angle);

        Matrix3D Rz = new Matrix3D
            (
               Cosine, -Sine, 0,
               Sine, Cosine, 0,
                  0, 0, 1
            );

        return Rz;
    }

    public Matrix3D RotateX(float angle)
    {
        float Cosine = Mathf.Cos(angle);
        float Sine = Mathf.Sin(angle);

        Matrix3D Rx = new Matrix3D
        (
           1, 0, 0,
           0, Cosine, -Sine,
           0, Sine, Cosine
        );

        return Rx;
    }

    public Matrix3D RotateY(float angle)
    {
        float Cosine = Mathf.Cos(angle);
        float Sine = Mathf.Sin(angle);

        Matrix3D Ry = new Matrix3D
            (
               Cosine, 0, Sine,
                    0, 1, 0,
               -Sine, 0, Cosine
            );

        return Ry;
    }

    public static Vector3 operator *(Matrix3D matrix, Vector3 vector)
    {
        return new Vector3
            (
        matrix.r0c0 * vector.x + matrix.r0c1 * vector.y + matrix.r0c2 * vector.z,
        matrix.r1c0 * vector.x + matrix.r1c1 * vector.y + matrix.r1c2 * vector.z,
        matrix.r2c0 * vector.x + matrix.r2c1 * vector.y + matrix.r2c2 * vector.z);
    }


    public static Matrix3D operator *(Matrix3D left, Matrix3D right)
	{
		
		return new Matrix3D(
        (left.r0c0 * right.r0c0) + (left.r0c1 * right.r1c0) + (left.r0c2 * right.r2c0),
        (left.r0c0 * right.r0c1) + (left.r0c1 * right.r1c1) + (left.r0c2 * right.r2c1),
        (left.r0c0 * right.r0c2) + (left.r0c1 * right.r1c2) + (left.r0c2 * right.r2c2),

        (left.r1c0 * right.r0c0) + (left.r1c1 * right.r1c0) + (left.r1c2 * right.r2c0),
        (left.r1c0 * right.r0c1) + (left.r1c1 * right.r1c1) + (left.r1c2 * right.r2c1),
        (left.r1c0 * right.r0c2) + (left.r1c1 * right.r1c2) + (left.r1c2 * right.r2c2),

        (left.r2c0 * right.r0c0) + (left.r2c1 * right.r1c0) + (left.r2c2 * right.r2c0),
        (left.r2c0 * right.r0c1) + (left.r2c1 * right.r1c1) + (left.r2c2 * right.r2c1),
        (left.r2c0 * right.r0c2) + (left.r2c1 * right.r1c2) + (left.r2c2 * right.r2c2));
	}

    public static Matrix3D operator +(Matrix3D left, Matrix3D right)
    {

        return new Matrix3D(
        (left.r0c0 + right.r0c0) , (left.r0c1 + right.r0c1) , (left.r0c2 + right.r0c2),
        (left.r1c0 + right.r1c0) , (left.r1c1 + right.r1c1) , (left.r1c2 + right.r1c2),
        (left.r2c0 + right.r2c0) , (left.r2c1 + right.r2c1) , (left.r2c2 + right.r2c2));
    }                                                       

}
/// <summary>
/// fuck my asshole
/// </summary>
public class Water_Diagnostic : MonoBehaviour {

    
    public Transform mast;
    public float angleX, angleY, angleZ;
    Mesh mesh;
    Vector3[] normals;
    Matrix3D op;
    void Awake()
    {
        //Quaternion
        //transform.Rotate
        mesh = GetComponent<MeshFilter>().mesh;
        normals = mesh.normals;
    }
	// Use this for initialization
	void Start ()
    {
        op = new Matrix3D(1, 0, 0,
                          0, 1, 0,
                          0, 0, 1);

    }

    // Update is called once per frame
    void Update ()
    {
    }

    void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {

            op = op.RotateX(angleX) * op.RotateY(angleY) * op.RotateZ(angleZ);
            for (uint i = 0; i < normals.Length; i++)
            {

                    Gizmos.color = Color.red;
                    Vector3 Vector = transform.TransformDirection(mesh.vertices[i]) + transform.TransformDirection(mesh.normals[i]);

                for (uint j = 0; j < normals.Length; j++)
                {          
                    Vector = op * Vector;
                }
                ////Original Vertex + Normal Direction
                ////Drawing The Vector..Vector 
                Gizmos.DrawRay(transform.TransformDirection(mesh.vertices[i]), Vector);

                //Vector3 normalsDir = op * ((Vector - mast.transform.position) * (Mathf.Sin(Time.time) * 0.5f));
                //Gizmos.color = Color.blue;

                ////Tip To direction - the current Mast
                //Gizmos.DrawRay(Vector, normalsDir / 4);

                //Vector3 Vector2 = (Vector + normalsDir / 4);

                //Vector3 normalDir2 = transform.TransformDirection(mesh.vertices[i]) - Vector2;

                //Gizmos.color = Color.green;

                //Gizmos.DrawRay(Vector2, normalDir2);

            }

               

        }
    }
}
