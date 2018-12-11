using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CookieMath
{

    public static Vector3 SnapToNearestIncrement(this Vector3 vec, float stepSize)
    {
        var roundScale = 1f / stepSize;
        float x = vec.x, y = vec.y, z = vec.z;



        return Vector3.zero;
    }

    public static Vector3 MultiplyComponents(this Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    public static Vector3 DivideComponents(this Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
    }


    public static Vector3 AddComponents(this Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    public static Vector3 AddToComponents(this Vector3 vector, float value)
    {
        return new Vector3(vector.x + value, vector.y + value, vector.z + value);
    }

    private static float Smooth(float t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

}
