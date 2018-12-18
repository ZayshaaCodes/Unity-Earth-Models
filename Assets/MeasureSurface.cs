using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeasureSurface : MonoBehaviour
{
    public Transform p1;
    public Transform p2;

    public float distance;
    public float distnaceft;
    public float arcDistance;
    public float arcDistnaceft;
    public float angle;

    public bool snapToSurface = true;

    private void OnEnable()
    {
        p1 = transform.GetChild(0);
        p2 = transform.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {

        var so = GetComponentInParent<SphereObject>();

        if (p1 && p2 && so != null)
        {
            var ps1 = so.ProjectToSphereSurface(p1.position);
            var ps2 = so.ProjectToSphereSurface(p2.position);
            var pd1 = so.GetDeltaFromSphereCenter(p1.position);
            var pd2 = so.GetDeltaFromSphereCenter(p2.position);

            if (snapToSurface)
            {
                p1.position = ps1;
                p2.position = ps2;
            }

            Debug.DrawLine(ps1, ps2);

            distance = (ps2 - ps1).magnitude;
            distnaceft = distance * 5280;

            angle = (float)Vector3d.Angle(pd1, pd2);

            arcDistance = angle * Mathf.Deg2Rad * so.radius;
            arcDistnaceft = arcDistance * 5280;
        }
    }
}
