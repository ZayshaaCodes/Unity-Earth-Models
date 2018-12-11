using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChildLookAtTarget : MonoBehaviour {


    public Transform rotationLockTarget;


    //swayRotation
    public Transform target;
    public Vector3 lookAxis;

    private void Update()
    {

        var currentAngle = Vector3.Angle(transform.up, rotationLockTarget.up);

        //transform.Rotate(transform.forward, currentAngle + 90f);

        //sway rotation
        var delta = target.position - transform.position;
        var axis = Vector3.Cross(transform.TransformDirection(lookAxis),delta);
        float angle = Vector3.Angle(transform.TransformDirection(lookAxis), delta);

        //print(angle);
        Debug.DrawRay(transform.position, axis, Color.red);
        transform.Rotate(axis, angle/2, Space.World);

    }
}
