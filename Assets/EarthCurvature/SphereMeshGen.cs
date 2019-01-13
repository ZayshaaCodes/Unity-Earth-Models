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
        if (mesh == null)
        {
            mesh = new Mesh();
        }

        int top = LonSegs + 1;
        int bottom = LonSegs + 1;
        int mid = (LonSegs + 1) * (LatSegs - 1);

        points = new Vector3[top + bottom + mid];
        uvs = new Vector2[top + bottom + mid];
        
        var latStep = (endLatAngle - startLatAngle) / LatSegs;
        var lonStep = (endLonAngle - startLonAngle) / LonSegs;

        //! Verts
        var pntIndex = 0;
        for (int i = 0; i <= LatSegs; i++)
        {
            var lat = startLatAngle + latStep * i;
            var v = Mathf.InverseLerp(-90, 90, lat);

            if (i == 0)
            {
                for (int j = 0; j < top; j++)
                {
                    var lon = startLonAngle + lonStep * j;

                    var clampedlon = (lon + 180) % 360 - 180;
                    clampedlon = (clampedlon - 180) % 360 + 180;
                    var uTile = Mathf.FloorToInt((lon + 180) / 360);

                    var u = Mathf.InverseLerp(-180, 180, clampedlon) + uTile;

                    Vector3 sp = GetSpherePoint(startLatAngle, lon);
                    //Debug.DrawLine(Vector3.zero, sp, Color.blue);
                    points[pntIndex] = sp;
                    uvs[pntIndex] = new Vector2(u, v);
                    pntIndex++;
                }
            } else if (i == LatSegs)
            {
                for (int j = 0; j < bottom; j++)
                {
                    var lon = startLonAngle + lonStep * j;

                    var clampedlon = (lon + 180) % 360 - 180;
                    clampedlon = (clampedlon - 180) % 360 + 180;
                    var uTile = Mathf.FloorToInt((lon + 180) / 360);

                    var u = Mathf.InverseLerp(-180, 180, clampedlon) + uTile;

                    Vector3 sp = GetSpherePoint(endLatAngle, lon);
                    //Debug.DrawLine(Vector3.zero, sp, Color.red);
                    points[pntIndex] = sp;
                    uvs[pntIndex] = new Vector2(u, v);
                    pntIndex++;
                }
            } else
            {
                for (int j = 0; j < LonSegs + 1; j++)
                {
                    var lon = startLonAngle + lonStep * j;

                    var clampedlon = (lon + 180) % 360 - 180;
                    clampedlon = (clampedlon - 180) % 360 + 180;
                    var uTile = Mathf.FloorToInt((lon + 180) / 360);

                    var u = Mathf.InverseLerp(-180, 180, clampedlon) + uTile;

                    Vector3 sp = GetSpherePoint(lat, lon);
                    //Debug.DrawLine(Vector3.zero, GetSpherePoint(startLatAngle + latStep * i, startLonAngle + lonAngle * j),Color.yellow);

                    points[pntIndex] = sp;
                    uvs[pntIndex] = new Vector2(u,v);
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
                tris[ti + 1] = f + 2 + LonSegs;
                tris[ti + 2] = f + 1;

                tris[ti + 3] = f + 2 + LonSegs;
                tris[ti + 4] = f;
                tris[ti + 5] = f + 1 + LonSegs;

                ti += 6;
                //Debug.DrawLine(Vector3.zero, GetSpherePoint(startLatAngle + latStep * i, startLonAngle + lonAngle * j),Color.yellow);
            }

        }

        mesh.triangles = null;
        mesh.vertices = points;
        mesh.triangles = tris;
        mesh.uv = uvs;
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
        //Gizmos.color = new Color(0f, 0f, 0f, 0.375f);
        //Gizmos.matrix = transform.localToWorldMatrix;
        //Gizmos.DrawWireMesh(mesh);
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
