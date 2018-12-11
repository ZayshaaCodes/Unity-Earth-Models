using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CookieLrUtilities
{
    //public static void DrawWireCircle(Vector3 position, float radius, Vector3 axis, Vector3 upAxis, int segments = 16)
    public static void SetLRCircle(this LineRenderer lr, Vector3 position, float radius, Vector3 axis, Vector3 upAxis, int segments = 16)
    {
        lr.positionCount = segments;
        lr.loop = true;

        Vector3 normal = Vector3.Cross(axis, upAxis).normalized;
        Vector3 binormal = Vector3.Cross(axis, normal).normalized;

        Vector3 point = binormal * radius;
        Quaternion rotationStep = Quaternion.AngleAxis(360f / segments, axis);

        for (int i = 0; i < segments; i++)
        {
            lr.SetPosition(i, position + point);
            point = rotationStep * point;
        }
    }

}
