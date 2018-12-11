using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Environments;
using UnityEngine;

[ExecuteInEditMode]
public class SphereObject : MonoBehaviour
{

    //[Range(0,10000)]
    public float radius;

    public float Circumference => 2 * Mathf.PI * radius;
    //the distance per 1 degree along the spheres surface;
    public double DegreeDistance => Circumference / 360.0;

    public Transform observer;

    public Vector3 WorldCenter => transform.localToWorldMatrix.MultiplyPoint(LocalCenter);
    public Vector3 LocalCenter => Vector3.down * radius;

    public Transform hextants;
    
    public SurfaceSpawner[] surfaceSpawners;

    public LineRenderer horizonLineRenderer;
    public LineRenderer aHorizonLineRenderer;
    [Range(0,10)]
    public float horizontalLineThickness;

    [SerializeField] private bool drawGizmos;

    void OnEnable()
    {
        var pd = FindObjectOfType<ParamDisplay>();

        var param = new ParamDisplay.Param
        {
            prefix = "Radius: ",
            displayFunction = () => radius.ToString("N2") + "mi"
        };

        pd.AddParam(param);

        if (observer)
        {
            pd.AddParam(new ParamDisplay.Param() { prefix = "Observer Height: ", displayFunction =() =>
                {
                    float mi = (observer.transform.position - (Vector3)WorldCenter).magnitude - radius;
                    double ft = mi * 5280.0;

                    return $"{mi:N2}mi ({ft:N2}ft)";
                }
            });
        }
    }

    private void OnValidate()
    {
        if (hextants)
        {    
            for (int i = 0; i < hextants.childCount; i++)
            {
                SphereMeshGen2 mg2 = hextants.GetChild(i).GetComponent<SphereMeshGen2>();
                mg2.radius = radius;
                mg2.GenerateMesh();
                if (!mg2.offsetCenter)
                {
                    mg2.transform.localPosition = Vector3.down * radius;
                }
            }

            var mg = GetComponentInChildren<SphereMeshGen>();
            if (mg)
            {
                mg.r = radius;
                mg.transform.localPosition = Vector3.down * radius;
                mg.genSphereMesh();
            }
        }
    }

    // Update is called once per frame
	void Update () {
	    if (horizonLineRenderer)
	    {
            //MATHS: 
	        var localDelta = transform.worldToLocalMatrix.MultiplyVector(observer.position - (Vector3)WorldCenter);

	        var rev = -localDelta.normalized;
            
	        double rad = (double)radius;

	        double h = localDelta.magnitude - rad;

	        if (h < 0)
	        {
	            horizonLineRenderer.enabled = false;
	            return;
	        }
	        if (!horizonLineRenderer.enabled)
	            horizonLineRenderer.enabled = true;

	        float alpha = (float)Math.Acos(rad / (rad + h));

	        double d = (rad + h) * Mathf.Sin(alpha);

	        double x = d * Mathf.Sin(alpha);
	        double y = d * Mathf.Cos(alpha);
	        Vector3 pos = (LocalCenter + localDelta) + rev * (float)x;

            horizonLineRenderer.SetLRCircle(pos, (float)y, localDelta, Vector3.forward, 360);

            //dense line that calculates the thinkness of the line in world units based on distance to the camera and the pixel height of the screen
	        float unitsPerPixelAtDistance = (float)d * Mathf.Atan(Camera.main.fieldOfView * Mathf.Deg2Rad) / Camera.main.pixelHeight * 2 * horizontalLineThickness;

            //print(fovHeightAtDistance);

	        horizonLineRenderer.widthMultiplier = unitsPerPixelAtDistance;

	    }
	    if (aHorizonLineRenderer)
	    {
	        //MATHS: 
	        var localDelta = transform.worldToLocalMatrix.MultiplyVector(observer.position - (Vector3)WorldCenter);

	        aHorizonLineRenderer.SetLRCircle(observer.position, 20, localDelta, Vector3.forward, 360);

        }
    }

    public Vector3 ProjectToSphereSurface(Vector3 worldPos)
    {
        Vector3 delta = worldPos - WorldCenter;
        return (WorldCenter + delta.normalized * radius);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;
        
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(1f, 0f, 0f, 0.09f);
        CookieGizmo.DrawWireCircle(new Vector3(0, -radius, 0), radius, Vector3.left, Vector3.forward, 720);
        CookieGizmo.DrawWireCircle(new Vector3(0, -radius, 0), radius, Vector3.up, Vector3.forward, 720);
        CookieGizmo.DrawWireCircle(new Vector3(0, -radius, 0), radius, Vector3.forward, Vector3.up, 720);

        //1 degree
        Gizmos.DrawRay(LocalCenter, Vector3.up * (radius + 1));
        Gizmos.DrawRay(LocalCenter, Quaternion.AngleAxis(1,Vector3.right) * Vector3.up * (radius + 1));

        var miAngle =  360f/(2 * Mathf.PI * radius);
        //1 unit
        Gizmos.DrawRay(LocalCenter, Vector3.up * (radius + 1));
        Gizmos.DrawRay(LocalCenter, Quaternion.AngleAxis(miAngle, Vector3.right) * Vector3.up * (radius + 1));

    }

    //given a start point, end point, and spacing, return points along a great circle path;
    public (Vector3[] positions, Quaternion[] rotations) GetPointsOnArc(Vector3 start, Vector3 end, double spacing, bool fixedSpacing = true)
    {
        if (spacing < .000001)
        {
            print("Spacing is too small to make any sense");
            return (null, null);
        }

        //get the delta from the center of the sphere to each point, 
        //these are vectors pointing from the center of the earth to each point
        Vector3 startDelta = start - WorldCenter;
        Vector3 endDelta = end - WorldCenter;

        //get the binormal for the great circle this will be the axis each instance is rotated along


        Vector3 binormal = Vector3.Cross(startDelta, endDelta);

        //get the angle along the great circle;
        double angle = Vector3.Angle((Vector3) startDelta, (Vector3) endDelta);

        //DistanceAlongSurface;
        double arcDistance = DegreeDistance * angle;

        //how many instances will we need?
        int count = Mathf.CeilToInt((float)(arcDistance / spacing));

        //the array of points and array of rotations
        //we'll need both to render or spawn an object at each position
        (Vector3[] positions, Quaternion[] rotations) points = (new Vector3[count+1], new Quaternion[count+1]);
        
        //the rotation we'll apply each step

        double spanAngle = fixedSpacing ? (float)(spacing * count / DegreeDistance) : angle;

        // do a loop along the path between the 2 points (using an angle from the start)
        for (int i = 0; i < count+1; i++)
        {
            //vector pointing to the to the surface point
            Vector3 surfaceVector = Quaternion.AngleAxis((float)(spanAngle * ((float)i / count)), binormal) * startDelta;

            //this is the surface tangent, facing forward.
            var tangent = Vector3.Cross(binormal, surfaceVector).normalized;

            points.positions[i] = (Vector3)((Vector3d) WorldCenter + ((Vector3d) surfaceVector).normalized * radius);
            points.rotations[i] = Quaternion.LookRotation(tangent, surfaceVector);
        }

        return points;
    }
}
