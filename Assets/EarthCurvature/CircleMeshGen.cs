using UnityEngine;
using System.Collections;

public class CircleMeshGen : MonoBehaviour {

    public Mesh m;

    public Material mat;

    public float depth = 10;
    public float radius = 100;
    public int segments = 50;
    [Range(0,360)]
    public float angleOfArc = 20;
    [Range(.01f, 1f)]
    public float thickness = 10; 

    

    // Use this for initialization
    void Start () {
        m = new Mesh();
        GetComponent<MeshFilter>().mesh = m;

        GenMesh(radius, segments, angleOfArc);
	}
	
    public void GenMesh(float radius, int segments, float arcAngle )
    {
        m = new Mesh();
        Vector3[] verts = new Vector3[(segments + 1) * 4];
        Vector2[] uvs = new Vector2[(segments + 1) * 4];
        int[] faces = new int[segments*6*3];
        

        double a = 0, step = System.Math.PI / segments * 2 * ((Mathf.Deg2Rad * arcAngle) / System.Math.PI / 2);
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 newVert;
            Vector3 newUv;

            a = step * (i % (segments + 1));

            float v = (float)(i % (segments + 1)) / (float)segments * (arcAngle * 69*.5f);

            //print(v);

            // Zero degrees = UP, CW rotation
            if (i < segments + 1)                                       // 1st row
            {
                newVert = 
                    Vector3.up * (float)((radius - thickness) * System.Math.Cos(a) - (radius)) +
                    (radius - thickness) * Vector3.right * (float)System.Math.Sin(a);
                newUv = Vector2.right + Vector2.up * v;

            } else if (i < (segments + 1) * 2)                          // 2nd row
            {
                newVert = 
                    Vector3.up * (float)(radius * System.Math.Cos(a) - radius) +
                    radius * Vector3.right * (float)System.Math.Sin(a);
                newUv = Vector2.right * .75f + Vector2.up * v;

            } else if (i < (segments + .75) * 3)                          //3rd row
            {
                newVert = 
                    Vector3.up * (float)(radius * System.Math.Cos(a) - radius) +
                    radius * Vector3.right * (float)System.Math.Sin(a) + 
                    Vector3.forward * depth;
                newUv = Vector2.right * .25f + Vector2.up * v;

            } else                                                      //4rd row
            {
                newVert = 
                    Vector3.up * (float)((radius - thickness) * System.Math.Cos(a) - (radius)) + 
                    (radius - thickness) * Vector3.right * (float)System.Math.Sin((float)a) +
                    Vector3.forward * depth;
                newUv = Vector2.right * .0f + Vector2.up * v;

                //print("UV: " + newUv.x + ", " + newUv.y);

            }

            verts[i] = newVert;
            uvs[i] = newUv;
        }

        for (int row = 0; row < 3; row++)
        {
            for (int seg = 0; seg < segments ; seg++)
            {
                //
                faces[(row * segments * 6) + (seg) * 6 + 0] = row * (segments + 1) + seg;
                faces[(row * segments * 6) + (seg) * 6 + 1] = row * (segments + 1) + seg + (segments + 1);
                faces[(row * segments * 6) + (seg) * 6 + 2] = row * (segments + 1) + seg + (segments + 2);

                faces[(row * segments * 6) + (seg) * 6 + 3] = row * (segments + 1) + seg + (segments + 2);
                faces[(row * segments * 6) + (seg) * 6 + 4] = row * (segments + 1) + seg + 1;
                faces[(row * segments * 6) + (seg) * 6 + 5] = row * (segments + 1) + seg;
            }
        }

        m.vertices = verts;
        m.uv = uvs;
        m.triangles = faces;
        m.RecalculateNormals();
        //string s = "";
        //for (int i = 0; i < faces.Length; i++)
        //{
        //    s += faces[i] + ", ";
        //}
        //print(s);

        GetComponent<MeshFilter>().mesh = m;
        GetComponent<MeshRenderer>().material = mat;
        //GetComponent<drawGizmo>().mesh = m;
    }

    void OnValidate()
    {
        GenMesh(radius, segments, angleOfArc);

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf)
            mf.mesh = m;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr)
            mr.material = mat;
            
        drawGizmo dg = GetComponent<drawGizmo>();
        if (dg)
            dg.mesh = m;
        
    }

    void OnActivate()
    {
        GenMesh(radius, segments, angleOfArc);

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf)
            mf.mesh = m;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr)
            mr.material = mat;

        drawGizmo dg = GetComponent<drawGizmo>();
        if (dg)
            dg.mesh = m;
    }
}
