using UnityEngine;

public class CookieGizmo
{
    public static void DrawWireCircle(Vector3 position, float radius, Vector3 axis, Vector3 upAxis, int segments = 16)
    {
        var normal = Vector3.Cross(axis, upAxis).normalized;
        var binormal = Vector3.Cross(axis, normal).normalized;

        Vector3 point = binormal * radius;
        var rotationStep = Quaternion.AngleAxis(360f / segments, axis);

        for (int i = 0; i < segments; i++)
        {
            Gizmos.DrawLine(position + point, position + rotationStep * point); point = rotationStep * point;
        }
    }

    /// <summary>
    /// Draws a line aling the array of points
    /// </summary>
    /// <param name="points"></param>
    public static void DrawPath(Vector3[] points)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }

    /// <summary>
    /// Same as DrawPath() but also colors the line segments based on their length. interpolated.
    /// </summary>
    public static void DrawDeltaColorPath(Vector3[] points, Color minColor, Color maxColor, float maxColorMagnitude, int pathDrawStep)
    {
        for (int i = 0; i < points.Length - pathDrawStep; i += pathDrawStep)
        {
            Gizmos.color = Color.Lerp(minColor, maxColor, Mathf.InverseLerp(0, maxColorMagnitude, (points[i] - points[i + pathDrawStep]).magnitude));
            Gizmos.DrawLine(points[i], points[i + pathDrawStep]);
        }
    }

    public static void DrawDeltaColorPath(Vector3[] points, Gradient patchGradient, float maxColorMagnitude, int pathDrawStep)
    {
        for (int i = 0; i < points.Length - pathDrawStep; i += pathDrawStep)
        {
            Gizmos.color = patchGradient.Evaluate(Mathf.InverseLerp(0, maxColorMagnitude, (points[i] - points[i + pathDrawStep]).magnitude));
            Gizmos.DrawLine(points[i], points[i + pathDrawStep]);
        }
    }


}