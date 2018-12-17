using System;
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

    // Clamps a value between a minimum float and maximum float value.
    public static double ClampDouble(double value, double min, double max)
    {
        if (value < min)
            value = min;
        else if (value > max)
            value = max;
        return value;
    }

    // Clamps value between min and max and returns value.
    // Set the position of the transform to be that of the time
    // but never less than 1 or more than 3
    //
    public static int Clamp(int value, int min, int max)
    {
        if (value < min)
            value = min;
        else if (value > max)
            value = max;
        return value;
    }

    // Calculates the ::ref::Lerp parameter between of two values.
    public static double InverseLerp(double a, double b, double value)
    {
        if (Math.Abs(a - b) > double.Epsilon)
            return Clamp01Double((value - a) / (b - a));
        return 0.0f;
    }

    // Clamps value between 0 and 1 and returns value
    public static double Clamp01Double(double value)
    {
        if (value < 0F)
            return 0F;
        return value > 1F ? 1F : value;
    }

    // Interpolates between /a/ and /b/ by /t/. /t/ is clamped between 0 and 1.
    public static double LerpDouble(double a, double b, double t)
    {
        return a + (b - a) * Clamp01Double(t);
    }

    // Interpolates between /a/ and /b/ by /t/ without clamping the interpolant.
    public static double LerpUnclampedDouble(double a, double b, double t)
    {
        return a + (b - a) * t;
    }

    // Same as ::ref::Lerp but makes sure the values interpolate correctly when they wrap around 360 degrees.
    public static double LerpAngleDoube(double a, double b, double t)
    {
        double delta = RepeatDouble((b - a), 360.0);
        if (delta > 180)
            delta -= 360;
        return a + delta * Clamp01Double(t);
    }

    // Loops the value t, so that it is never larger than length and never smaller than 0.
    public static double RepeatDouble(double t, double length)
    {
        return ClampDouble(t - Math.Floor(t / length) * length, 0.0, length);
    }


}
