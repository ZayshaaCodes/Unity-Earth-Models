using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class SphereMeshGenNoUvSeam : MonoBehaviour {


    private MeshFilter mf;
    private MeshRenderer mr;

    public Mesh mesh;

    public Color color;

    public float r = 5;

    public int LatSegs = 10;
    public int lonSegs = 10;

    Vector3[] points;
    int[] tris;
    Vector2[] uvs;

    public int showCount;

    public int pointCount;
    public int triCount;

    public bool hemisphere;

    [Range(0, 180)]
    public float startAngle;
    [Range(0, 180)]
    public float endAngle;

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
        if (lonSegs < 3)
        {
            lonSegs = 3;
        }

        genSphereMesh();

        mf.sharedMesh = mesh;
    }

    public void genSphereMesh()
    {


        mesh = new Mesh();

        float pi = Mathf.PI;
        points = new Vector3[(LatSegs - 1) * lonSegs + 2];
        pointCount = points.Length;

        float latAngleStep = ((endAngle - startAngle) * Mathf.Deg2Rad) / LatSegs;
        float lonAngleStep = (360 * Mathf.Deg2Rad) / lonSegs;

        for (int latIndex = 0 ; latIndex <= LatSegs; latIndex++)
        {
            if (latIndex == 0) //first
            {
                float y = r * Mathf.Sin(latAngleStep * latIndex - (90 - startAngle) * Mathf.Deg2Rad);
                float x = 0;
                float z = 0;

                points[0] = new Vector3(x, y, z);

                print("0 : " + points[0]);

                continue;
            }
            else if (latIndex == LatSegs) //last
            {
                float y = r * Mathf.Sin(latAngleStep * latIndex - (90 - startAngle) * Mathf.Deg2Rad);
                float x = 0;
                float z = 0;

                int index = (LatSegs - 1) * lonSegs + 1;
                points[index] = new Vector3(x, y, z);

                print(index + " : " + points[index]);

            }
            else // middle
            {
                for (int lon = 0; lon < lonSegs; lon++)
                {
                    float br = r * Mathf.Cos(latAngleStep * latIndex - (90 - startAngle) * Mathf.Deg2Rad);

                    float y = r * Mathf.Sin(latAngleStep * latIndex - (90 - startAngle) * Mathf.Deg2Rad);
                    float x = br * Mathf.Cos(lonAngleStep * lon);
                    float z = br * Mathf.Sin(lonAngleStep * lon);
                    
                    int index = (latIndex - 1) * lonSegs + 1 + lon;
                    points[index] = new Vector3(x, y, z);

                    print(index + " : " + points[index]);

                }
            }

        }
        mesh.vertices = points;

        // (8 + 1 * 4 * 2)

        tris = new int[(lonSegs * 2 + (LatSegs - 2) * lonSegs * 2) * 3];

        triCount = 0;

        for (int lat = 0; lat < LatSegs; lat++)
        {

            if (lat == 0) // Top Ring
            {
                for (int lon = 0; lon < lonSegs - 1 ; lon++)
                {
                    //top center first point
                    tris[lon * 3 + 0] = 0;
                    tris[lon * 3 + 1] = lon + 1;
                    tris[lon * 3 + 2] = lon + 2;

                    triCount++;
                }


                tris[(lonSegs - 1) * 3 + 0] = 0;
                tris[(lonSegs - 1) * 3 + 1] = lonSegs;
                tris[(lonSegs - 1) * 3 + 2] = 1;

                triCount++;

            }
            else if (lat < LatSegs - 1) // middle Rings
            {
                //index offset to the first vertex in the latitude ring
                int offset;

                for (int lon = 0; lon < lonSegs - 1; lon++)
                {
                    
                    offset = 1 + ((lat - 1) * lonSegs) + lon;

                    tris[triCount * 3 + 0] = offset;
                    tris[triCount * 3 + 1] = offset + lonSegs ;
                    tris[triCount * 3 + 2] = offset + lonSegs + 1;

                    tris[triCount * 3 + 3] = offset + lonSegs + 1;
                    tris[triCount * 3 + 4] = offset + 1;
                    tris[triCount * 3 + 5] = offset;

                    triCount += 2;

                }

                //4
                offset = ((lat) * lonSegs);

                tris[triCount * 3 + 0] = offset;
                tris[triCount * 3 + 1] = offset + lonSegs;
                tris[triCount * 3 + 2] = offset + 1;

                tris[triCount * 3 + 3] = offset + 1;
                tris[triCount * 3 + 4] = offset - lonSegs + 1;
                tris[triCount * 3 + 5] = offset;

                triCount += 2;
            }
            else
            {
                for (int lon = 0; lon < lonSegs - 1; lon++)
                {

                    int offset = 1 + ((lat - 1) * lonSegs) + lon;

                    tris[triCount * 3 + 0] = points.Length - 1;
                    tris[triCount * 3 + 1] = offset + 1;
                    tris[triCount * 3 + 2] = offset;

                    triCount++;
                }

            }
        }

        //uvs = new Vector2[points.Length];
        //for (int lat = 0; lat < LatSegs + 1; lat++)
        //{
        //    if (lat == 0) //first
        //    {
        //        float u = .5f;
        //        float v = 0f;
        //        uvs[0] = new Vector2(u, v);
        //        continue;
        //    }
        //    else if (lat == LatSegs) //last
        //    {
        //        float u = .5f;
        //        float v = 1f;
        //        uvs[(LatSegs - 1) * (lonSegs) + 1] = new Vector2(u, v);
        //        continue;
        //    }
        //    else // middle
        //    {
        //        for (int lon = 0; lon <= lonSegs; lon++)
        //        {
        //            float u = (float)lon / lonSegs;
        //            float v = (float)lat / LatSegs;
                    
        //            //print(u);
        //            //
        //            uvs[(lat-1) * (lonSegs+1) + 1 + lon] = new Vector2(u, v);

        //        }
        //    }

        //}

        mesh.vertices = points;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

    }

    void DrawTriangles()
    {

    }

    void OnDrawGizmos()
    {
        foreach (var p in points)
        {
            Gizmos.color = new Color(p.normalized.x, p.normalized.y, p.normalized.z, .5f);
            Gizmos.DrawWireCube(transform.position + p, Vector3.one * 50);

        }

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
