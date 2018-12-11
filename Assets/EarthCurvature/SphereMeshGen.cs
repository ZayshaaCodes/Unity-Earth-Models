using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;    
#endif

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class SphereMeshGen : MonoBehaviour {

    public List<SphereMeshState> states = new List<SphereMeshState>();

    private MeshFilter mf;
    private MeshRenderer mr;

    public Mesh mesh;

    public Color color;

    public float r = 5;

    public int LatSegs = 10;
    public int LonSegs = 10;

    Vector3[] points;
    int[] tris;
    Vector2[] uvs;

    //public int showCount;

    [HideInInspector]
    public int pointCount;
    [HideInInspector]
    public int triCount;
    
    public bool squareSymetry = true;
    [Range(-180, 180)]
    public float startSweepAngle = -5;
    [Range(-180, 180)]
    public float endSweepAngle = 5;

    [Range(-90, 90)]
    public float startAngle = -5;
    [Range(-90, 90)]
    public float endAngle = 5;

    public bool generateEndCaps = false;

    public bool drawGizmo = true;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    //void OnGUI() {

    //    if (showCount > pointCount)
    //    {
    //        showCount = pointCount;
    //    }

    //    for (int i = 0; i < showCount; i++)
    //    {
    //        Vector3 sp = Camera.main.WorldToScreenPoint(transform.position + points[i]);

    //        sp.y = Screen.height - sp.y;

    //        GUI.Label(new Rect(sp.x , sp.y, 40, 20), i.ToString());
    //    }

    //}

    void OnValidate()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();

        if (LatSegs < 3)
        {
            LatSegs = 3;
        }
        if (LonSegs < 3)
        {
            LonSegs = 3;
        }

        genSphereMesh();

    }

    public void genSphereMesh()
    {


        mesh = new Mesh();

        float pi = Mathf.PI;
        points = new Vector3[(LatSegs - 1) * (LonSegs + 1) + 2];
        pointCount = points.Length;

        float latAngleStep = ((endAngle - startAngle) * Mathf.Deg2Rad) / LatSegs;
        float lonAngleStep = ((endSweepAngle - startSweepAngle) * Mathf.Deg2Rad) / LonSegs;

        for (int latIndex = 0; latIndex <= LatSegs; latIndex++)
        {
            if (latIndex == 0) //first
            {
                float y = r * Mathf.Sin(latAngleStep * latIndex + startAngle * Mathf.Deg2Rad);
                float x = 0;
                float z = 0;
                points[0] = new Vector3(x, y, z);
                continue;
            }
            else if (latIndex == LatSegs) //last
            {
                float y = r * Mathf.Sin(latAngleStep * latIndex + startAngle * Mathf.Deg2Rad);
                float x = 0;
                float z = 0;
                points[(LatSegs - 1) * (LonSegs + 1) + 1] = new Vector3(x, y, z);
                continue;
            }
            else // middle
            {
                for (int lon = 0; lon <= LonSegs; lon++)
                {
                    float br = r * Mathf.Cos(latAngleStep * latIndex + startAngle * Mathf.Deg2Rad);

                    float y = r * Mathf.Sin(latAngleStep * latIndex + startAngle * Mathf.Deg2Rad);
                    float x = br * Mathf.Cos(lonAngleStep * lon + startSweepAngle * Mathf.Deg2Rad);
                    float z = br * Mathf.Sin(lonAngleStep * lon + startSweepAngle * Mathf.Deg2Rad);

                    //
                    points[(latIndex - 1) * (LonSegs + 1) + 1 + lon] = new Vector3(x, y, z);

                }
            }

        }
        mesh.vertices = points;



        tris = new int[(LonSegs * 2 * (LatSegs - 1)) * 6];

        triCount = 0;

        for (int lat = 0; lat < LatSegs; lat++)
        {


            if (lat == 0) // Top Ring
            {

                for (int lon = 0; lon < LonSegs; lon++)
                {
                    tris[lon * 3 + 0] = 0;
                    tris[lon * 3 + 1] = lon + 1;
                    tris[lon * 3 + 2] = lon + 2;

                    triCount++;
                }
            }
            else if (lat < LatSegs - 1) // middle Rings
            {

                for (int lon = 0; lon < LonSegs; lon++)
                {
                    int offset = 1 + (LonSegs + 1) * (lat - 1) + lon;

                    tris[triCount * 3 + 0] = offset;
                    tris[triCount * 3 + 1] = offset + LonSegs + 1;
                    tris[triCount * 3 + 2] = offset + LonSegs + 2;

                    tris[triCount * 3 + 3] = offset + LonSegs + 2;
                    tris[triCount * 3 + 4] = offset + 1;
                    tris[triCount * 3 + 5] = offset;

                    triCount += 2;

                }
            }
            else
            {
                for (int lon = 0; lon < LonSegs; lon++)
                {

                    int offset = 1 + (LonSegs + 1) * (lat - 1) + lon;

                    tris[triCount * 3 + 0] = points.Length - 1;
                    tris[triCount * 3 + 1] = offset + 1;
                    tris[triCount * 3 + 2] = offset;

                    triCount++;
                }

            }
        }

        uvs = new Vector2[points.Length];
        for (int lat = 0; lat < LatSegs + 1; lat++)
        {
            if (lat == 0) //first
            {
                float u = .5f;
                float v = 0f;
                uvs[0] = new Vector2(u, v);
                continue;
            }
            else if (lat == LatSegs) //last
            {
                float u = .5f;
                float v = 1f;

                uvs[points.Length - 1] = new Vector2(u, v);
                continue;
            }
            else // middle
            {
                for (int lon = 0; lon <= LonSegs; lon++)
                {
                    float u = (float)lon / LonSegs;
                    float v = (float)lat / LatSegs;

                    uvs[(lat - 1) * (LonSegs + 1) + 1 + lon] = new Vector2(u, v);

                }
            }

        }

        mesh.vertices = points;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        mf.sharedMesh = mesh;

    }
    public IEnumerator LerpMeshState(SphereMeshState start, SphereMeshState end, float speed)
    {

        SphereMeshState lerpState = new SphereMeshState("", start);

        float t = 0;
        while (t <1)
        {
            t += Time.deltaTime / speed;
            if (t > 1)
            {
                t = 1;
            }

            lerpState.LatSegs = (int)Mathf.Lerp(start.LatSegs, end.LatSegs, t);
            lerpState.LonSegs = (int)Mathf.Lerp(start.LonSegs, end.LonSegs, t);
            lerpState.startSweepAngle = Mathf.Lerp(start.startSweepAngle, end.startSweepAngle, t);
            lerpState.endSweepAngle = Mathf.Lerp(start.endSweepAngle, end.endSweepAngle, t);
            lerpState.startAngle = Mathf.Lerp(start.startAngle, end.startAngle, t);
            lerpState.endAngle = Mathf.Lerp(start.endAngle, end.endAngle, t);

            SetState(lerpState);

#if UNITY_EDITOR
            SceneView.RepaintAll();
#endif

            yield return new WaitForEndOfFrame();

        }
    }

    public void SetState(SphereMeshState state)
    {
        LatSegs = state.LatSegs;
        LonSegs = state.LonSegs;
        startSweepAngle = state.startSweepAngle;
        endSweepAngle = state.endSweepAngle;
        startAngle = state.startAngle;
        endAngle = state.endAngle;

        genSphereMesh();
    }


    void OnDrawGizmos()
    {

        Vector3 d1 = (Quaternion.Euler(startSweepAngle, 0, startAngle) * Vector3.up);
        Vector3 d2 = (Quaternion.Euler(startSweepAngle, 0, endAngle) * Vector3.up);
        Vector3 d3 = (Quaternion.Euler(endSweepAngle, 0, endAngle) * Vector3.up);
        Vector3 d4 = (Quaternion.Euler(endSweepAngle, 0, startAngle) * Vector3.up);


        if (drawGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + d1 * r);
            Gizmos.DrawLine(transform.position, transform.position + d2 * r);
            Gizmos.DrawLine(transform.position, transform.position + d3 * r);
            Gizmos.DrawLine(transform.position, transform.position + d4 * r);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + d1 * r, transform.position + d1 * r * 1.125f);
            Gizmos.DrawLine(transform.position + d2 * r, transform.position + d2 * r * 1.125f);
            Gizmos.DrawLine(transform.position + d3 * r, transform.position + d3 * r * 1.125f);
            Gizmos.DrawLine(transform.position + d4 * r, transform.position + d4 * r * 1.125f);
        }

        //foreach (var p in points)
        //{
        //    Gizmos.color = new Color(p.normalized.x, p.normalized.y, p.normalized.z, .5f);
        //    Gizmos.DrawWireCube(transform.position + p, Vector3.one * .125f);

        //}

        //Gizmos.color = color;

        //if (uvs == null)
        //    return;
        //foreach (Vector2 uv in uvs)
        //{
        //    //Gizmos.color = new Color(p.normalized.x, p.normalized.y, p.normalized.z, .5f);

        //    Gizmos.DrawWireCube(transform.position + new Vector3(uv.x, uv.y, 0f), Vector3.one * .125f);
        //}
    }
}


[System.Serializable]
public class SphereMeshState
{
    public string name = "";

    public float r = 5;

    public int LatSegs = 10;
    public int LonSegs = 10;

    public float startSweepAngle = 0;
    public float endSweepAngle = 360;

    public float startAngle = 0;
    public float endAngle = 180;


    public SphereMeshState(string name, SphereMeshGen sGen)
    {

        this.name = name;
        LatSegs = sGen.LatSegs;
        LonSegs = sGen.LonSegs;
        startSweepAngle = sGen.startSweepAngle;
        endSweepAngle = sGen.endSweepAngle;
        startAngle = sGen.startAngle;
        endAngle = sGen.endAngle;
    }


    public SphereMeshState(string name, SphereMeshState sGen)
    {

        this.name = name;
        LatSegs = sGen.LatSegs;
        LonSegs = sGen.LonSegs ;
        startSweepAngle = sGen.startSweepAngle;
        endSweepAngle = sGen.endSweepAngle;
        startAngle = sGen.startAngle;
        endAngle = sGen.endAngle;
    }

    public SphereMeshState(string name, float r, int LatSegs, int LonSegs, float startSweepAngle, 
        float endSweepAngle, float startAngle, float endAngle)
    { 
        this.name = name;

        this.LatSegs = LatSegs;
        this.LonSegs = LonSegs;
        this.startSweepAngle = startSweepAngle;
        this.endSweepAngle = endSweepAngle;
        this.startAngle = startAngle;
        this.endAngle = endAngle;
    }



}
