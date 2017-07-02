using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class SphereMeshGen2 : MonoBehaviour {


    public int xSegments, ySegments;

    public bool oneToOne;

    public GameObject heightController;

    [Range(0,90)]
    public float xAngle, yAngle;

    double xStep, yStep;
    public double radius = 3959;

    public bool showGizmos = true;

    Vector3[] points;
    Vector2[] uvs;
    int[] tris;

    public bool update = true;

    public int radialLineCount = 100;
    public float angleBeweenRadialLines = 1f / 69f;
    public float gizmoSize = .03f;
    public Color gizmoColor = Color.yellow;
    public float verticalLineHeightMiles = .002f;

    public Mesh mesh;

    public circleData HorizonCircle;
    public Color HorizonCircleColor = Color.white;

	// Use this for initialization
	void Start () {
	}

    void OnValidate()
    {
        if (!mesh)
        {
            mesh = new Mesh();
            mesh.name = "SphereHextant";
        }
        if (oneToOne)
        {
            ySegments = xSegments;
            yAngle = xAngle;
        }
        GenerateMesh();
    }

    void GenerateMesh()
    {

        if (heightController)
        {
            float d = Vector3.Distance(heightController.transform.position, transform.position);
            //print(d);
            xAngle = Mathf.Acos((float)radius / d) * Mathf.Rad2Deg * 2.375f ;
            if (xAngle > 90f)
                xAngle = 90f;

            yAngle = xAngle;
        }

        points = new Vector3[(xSegments + 1) * (ySegments + 1)];
        uvs = new Vector2[(xSegments + 1) * (ySegments + 1)];
        xStep = (xAngle / 90 * 2) / xSegments;
        yStep = (yAngle / 90 * 2) / ySegments;
        double minx = -(xAngle / 2 / 45);
        double miny = -(yAngle / 2 / 45);

        for (int i = 0; i < xSegments + 1; i++)
        {
            for (int j = 0; j < ySegments + 1; j++)
            {
                double x, y, z;
                x = minx + i * xStep;
                y = miny + j * yStep;
                z = -1.0;
                uvs[i * (ySegments + 1) + j] = new Vector2((float)x, (float)y);

                double x2 = x * x;
                double y2 = y * y;
                double z2 = z * z;

                x = x * System.Math.Sqrt(1.0 - (y2 * 0.5) - (z2 * 0.5) + ((y2 * z2) / 3.0)) * radius;
                y = y * System.Math.Sqrt(1.0 - (z2 * 0.5) - (x2 * 0.5) + ((z2 * x2) / 3.0)) * radius;
                z = z * System.Math.Sqrt(1.0 - (x2 * 0.5) - (y2 * 0.5) + ((x2 * y2) / 3.0)) * radius;


                points[i * (ySegments + 1) + j] = new Vector3((float)x, (float)y, (float)z);

            }
        }

        tris = new int[xSegments * ySegments * 6];
        int triCount = 0;
        for (int i = 0; i < xSegments; i++)
        {
            for (int j = 0; j < ySegments; j++)
            {

                int corner = i * (ySegments + 1) + j;

                tris[triCount * 3 + 0] = corner;
                tris[triCount * 3 + 1] = corner + 1;
                tris[triCount * 3 + 2] = corner + ySegments + 1;

                tris[triCount * 3 + 3] = corner + 1;
                tris[triCount * 3 + 4] = corner + 1 + ySegments + 1;
                tris[triCount * 3 + 5] = corner + ySegments + 1;
                    
                triCount += 2;
            }
        }

        var mf = GetComponent<MeshFilter>();
        if (mf)
        {
            mesh.vertices = points;
            mesh.uv = uvs;
            mesh.triangles = tris;


            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            mf.mesh = mesh;
        }
    }
	
	// Update is called once per frame
    float lastHeight = 0;

	void Update () {

        if (heightController && !Mathf.Approximately(Vector3.Distance(heightController.transform.position, transform.position), lastHeight))
        {
            update = true;
        }

        if (update)
        {
            GenerateMesh();
            update = false;
        }
    }

    void OnDrawGizmos()
    {

        if (heightController)
        {
            var r = (float)radius;
            var h = Vector3.Distance(heightController.transform.position, transform.position) - r;

            var theta = Mathf.Acos(r / (r + h));

            var a = r * Mathf.Sin(theta);
            var b = r * Mathf.Cos(theta);

            CircleGizmo.DrawCircle(new circleData(HorizonCircleColor, a, 1000), Matrix4x4.TRS(transform.position + new Vector3(0, b, 0), Quaternion.Euler(0, 0, 0), Vector3.one));

        }

        Gizmos.matrix = transform.localToWorldMatrix;
        for (int i = 0; i < radialLineCount; i++)
        {
            float radians =  angleBeweenRadialLines * Mathf.Deg2Rad ;
            float lineDistance = (float)radius + verticalLineHeightMiles;

            Vector3 dir = new Vector3(Mathf.Sin(radians * i), 0, -Mathf.Cos(radians * i));

            Gizmos.DrawLine(dir * (float)radius, dir * lineDistance);
        }

        //circleData circle = new circleData(Color.red, )
    }

}
