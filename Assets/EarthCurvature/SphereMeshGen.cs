using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;    
#endif

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
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
    [Range(-90, 90)]
    public float startLatAngle = -5;
    [Range(-90, 90)]
    public float endLatAngle = 5;

    [Range(-180, 180)]
    public float startLonAngle = -5;
    [Range(-180, 180)]
    public float endLonAngle = 5;

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

    void Update()
    {
        OnValidate();
    }

    void OnValidate()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();

        if (LatSegs < 2)
        {
            LatSegs = 2;
        }
        if (LonSegs < 2)
        {
            LonSegs = 2;
        }

        genSphereMesh();

    }

    public void genSphereMesh()
    {

        int top = 90 - Math.Abs(startLatAngle) < .0001f ? 1 : LonSegs + 1;
        int bottom = 90 - Math.Abs(endLatAngle) < .0001f ? 1 : LonSegs + 1;
        int mid = (LonSegs + 1) * (LatSegs - 1);

        points = new Vector3[top + bottom + mid];
        
        var latStep = (endLatAngle - startLatAngle) / LatSegs;
        var lonStep = (endLonAngle - startLonAngle) / LonSegs;

        //! Verts
        var pntIndex = 0;
        for (int i = 0; i <= LatSegs; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < top; j++)
                {
                    Vector3 sp = GetSpherePoint(startLatAngle, startLonAngle + j * lonStep);
                    //Debug.DrawLine(Vector3.zero, sp, Color.blue);
                    points[pntIndex] = sp;
                    pntIndex++;
                }
            } else if (i == LatSegs)
            {
                for (int j = 0; j < bottom; j++)
                {
                    Vector3 sp = GetSpherePoint(endLatAngle, startLonAngle + j * lonStep);
                    //Debug.DrawLine(Vector3.zero, sp, Color.red);
                    points[pntIndex] = sp;
                    pntIndex++;
                }
            } else
            {
                for (int j = 0; j < LonSegs + 1; j++)
                {
                    Vector3 sp = GetSpherePoint(startLatAngle + latStep * i, startLonAngle + lonStep * j);
                    //Debug.DrawLine(Vector3.zero, GetSpherePoint(startLatAngle + latStep * i, startLonAngle + lonAngle * j),Color.yellow);

                    points[pntIndex] = sp;
                    pntIndex++;
                }
            }
        }

        //! Tris
        tris = new int[LatSegs * LonSegs * 2 * 3];
        var ti = 0;
        for (int i = 0; i < LatSegs; i++)
        {
            for (int j = 0; j < LonSegs; j++)
            {
                var f = i * (LonSegs + 1) + j;

                tris[ti + 0] = f;
                tris[ti + 1] = f + 1;
                tris[ti + 2] = f + 1 + LonSegs;

                tris[ti + 3] = f + 1 + LonSegs;
                tris[ti + 4] = f + LonSegs;
                tris[ti + 5] = f;

                ti += 6;
                //Debug.DrawLine(Vector3.zero, GetSpherePoint(startLatAngle + latStep * i, startLonAngle + lonAngle * j),Color.yellow);
            }
        }

        mesh.vertices = points;
        mesh.triangles = tris;
        //mesh.uv = uvs;
        mesh.RecalculateNormals();

        mf.sharedMesh = mesh;

    }


    public Vector3 GetSpherePoint(double lat, double lon)
    {

        var degToRad = 2 * Math.PI / 360.0;
        var latr = lat * degToRad;
        var lonr = lon * degToRad;

        double x, y, z, baseR;

        y = r * Math.Sin(latr);
        baseR = r * Math.Cos(latr);

        z = baseR * Math.Sin(lonr);
        x = baseR * Math.Cos(lonr);

        return new Vector3((float) x,(float) y,(float) z);
    }

    public (double u, double v) ProjectLatLongToUVCoords(double lat, double lon)
    {
        var latVal = CookieMath.InverseLerp(-90, 90, lat);
        var lonVal= CookieMath.InverseLerp(-180, 180, lon);

        return (lonVal, latVal);
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
        startLatAngle = state.startSweepAngle;
        endLatAngle = state.endSweepAngle;
        startLonAngle = state.startAngle;
        endLonAngle = state.endAngle;

        genSphereMesh();
    }


    void OnDrawGizmos()
    {

        Vector3 d1 = (Quaternion.Euler(startLatAngle, 0, startLonAngle) * Vector3.up);
        Vector3 d2 = (Quaternion.Euler(startLatAngle, 0, endLonAngle) * Vector3.up);
        Vector3 d3 = (Quaternion.Euler(endLatAngle, 0, endLonAngle) * Vector3.up);
        Vector3 d4 = (Quaternion.Euler(endLatAngle, 0, startLonAngle) * Vector3.up);


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
        startSweepAngle = sGen.startLatAngle;
        endSweepAngle = sGen.endLatAngle;
        startAngle = sGen.startLonAngle;
        endAngle = sGen.endLonAngle;
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
