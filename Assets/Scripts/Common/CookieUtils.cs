using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CookieUtils
{
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindDeepChild(aName);
            if (result != null)
                return result;
        }
        return null;
    }
    
    public static float DistanceFromPlane(this Vector3 pointPos, Vector3 planeNormal, Vector3 planePos)
    {
        //Positive distance denotes that the point p is on the front side of the plane 
        //Negative means it's on the back side
        float distance = Vector3.Dot(planeNormal, pointPos - planePos);

        return distance;
    }

    public static void FromMatrix(this Transform transform, Matrix4x4 matrix)
    {
        transform.localScale = matrix.ExtractScale();
        transform.rotation = matrix.ExtractRotation();
        transform.position = matrix.ExtractPosition();
    }
    
    public static Quaternion ExtractRotation(this Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    public static Vector3 ExtractPosition(this Matrix4x4 matrix)
    {
        Vector3 position;
        position.x = matrix.m03;
        position.y = matrix.m13;
        position.z = matrix.m23;
        return position;
    }

    public static Vector3 ExtractScale(this Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }

    //returns the linear velocity for a point on a rotating rigidbody
    public static Vector3 PointVelocity(this Rigidbody rb, Vector3 point)
    {
        Vector3 delta = rb.transform.position - point;
        return rb.velocity + Vector3.Cross(delta, rb.angularVelocity);
    }



}
