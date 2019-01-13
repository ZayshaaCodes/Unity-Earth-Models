using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProjectToSurface : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 rotation;

    public SphereObject so;

    private void OnEnable()
    {
        so = GetComponentInParent<SphereObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (so)
        {
            var surfacePos = so.ProjectToSphereSurface(transform.position);

            var delta = so.GetDeltaFromSphereCenter(transform.position);

            var binormal = Vector3.Cross(delta, transform.forward);
            var tangent = Vector3.Cross(delta, binormal);

            transform.position = surfacePos;
            transform.rotation = Quaternion.Euler(rotation)*Quaternion.LookRotation(tangent,delta);
        }
    }
}