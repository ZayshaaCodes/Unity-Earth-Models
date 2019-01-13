using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SphereHeightmapManager : MonoBehaviour
{
    [Range(-180,180)]
    public double lon;
    [Range(-90,90)]
    public double lat;

    public Color gizmoColor1 = Color.white;

    [Range(.1f,3959)]
    public double radius;
    public double latShift = 0;
    [Range(-1,1)]
    public float day = 0;

    public Material skyMaterial;
    public SphereMeshGen latLonMeshGenerator;
    public float visibleRange = 5;

    public static double degToRad = Math.PI / 180;
    public static double radToDeg = 180 / Math.PI;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (skyMaterial)
            //skyMaterial.SetFloat("_Latitude", latAngleFrom);
            if (skyMaterial.HasProperty("_Longitude"))
                skyMaterial.SetFloat("_Longitude", (float) lon + day *360);
            if (skyMaterial.HasProperty("_Latitude"))
                skyMaterial.SetFloat("_Latitude", (float) lat);
        if (latLonMeshGenerator)
        {
            latLonMeshGenerator.startLatAngle = Mathf.Clamp((float) (lat - visibleRange), -90, 90);
            latLonMeshGenerator.endLatAngle = Mathf.Clamp((float) (lat + visibleRange), -90, 90);
            latLonMeshGenerator.startLonAngle = (float) (lon - visibleRange);
            latLonMeshGenerator.endLonAngle = (float) (lon + visibleRange);
        }
    }

    public void OnDrawGizmos()
    {
        if (transform.childCount > 0)
        {
            transform.GetChild(0).localRotation = Quaternion.Euler((float)(90 - lat), 0, 0 ) * Quaternion.Euler(0, (float) (90 + lon), 0);
            transform.GetChild(0).localScale = Vector3.one * (float) radius * 2; 
            transform.GetChild(0).localPosition = Vector3.down * (float) radius; 
        }

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = gizmoColor1;

        for (int i = -80; i <= 80; i+=10)
        {
            DrawLatArc(i, 1,1,.1);
        }


        return;

        var cir = 2 * System.Math.PI * radius;
        var distancePerDegree = cir / 360;
        //var distancePerRadian = cir / (Mathf.PI * 2);


        var steps = 360;
        var stepDistanceAngle = 1;


        Vector3 last = Vector3.zero;
        for (int i = -steps / 2; i <= steps / 2; i++)
        {
            var angle = stepDistanceAngle * i;

            var cur = GetRelativeLonLinePoint(0, angle); ;

            if (i == -steps / 2)
            {
                last = cur;
            }
            else
            {
                Gizmos.DrawLine(last, cur);
                last = cur;
            }
        }
    }

    public void DrawLatArc(double latitude, double startLon, double endLon, double angularStepResolution)
    {
        double deltaAngle = endLon - startLon;
        int count = (int)Math.Round(deltaAngle / angularStepResolution);
        double step = deltaAngle / count;

        var lDistance = radius * Math.Sin(latitude * degToRad);
        var centerZ = lDistance * Math.Cos(lat * degToRad);
        var centerY = lDistance * Math.Sin(lat * degToRad);
        Vector3d circleCenter = new Vector3d(0, -radius + centerY, centerZ);
        Gizmos.DrawWireCube(circleCenter, Vector3.one);

        var lRad = radius * Math.Cos(latitude * degToRad);
        var zRad = lRad * Math.Sin(lat * degToRad);
        var yRad = lRad * Math.Cos(lat * degToRad);

        for (int i = 0; i < 36; i++)
        {
            var x = lRad * Math.Sin((-lon + i * 10) * degToRad);
            var y = yRad * Math.Cos((-lon + i * 10) * degToRad);
            var z = -zRad * Math.Cos((-lon + i * 10) * degToRad);
            Vector3d p1 = circleCenter + new Vector3d(x, y, z);

            Gizmos.DrawWireCube(p1, Vector3.one);
        }


    }


    private Vector3 GetRelativeLonLinePoint(double latAngleFrom, double lonAngleFrom)
    {
        var d = (lat + latAngleFrom) * degToRad;
        var cosd = Math.Cos(d);
        var radCosD = radius * cosd;
        var omca = (1 - Math.Cos(lonAngleFrom * degToRad));

        var x = radCosD * Math.Sin(lonAngleFrom * degToRad);
        var y = -radCosD * omca * cosd;
        var z = radius * Math.Sin(d) * omca * cosd;

        return new Vector3((float) x, (float) y, (float) z);
    }
}
