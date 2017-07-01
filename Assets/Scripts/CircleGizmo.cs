using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class circleData
{
    public Color circleColor;
    public Color cubeColor;

    public float radius;

    public bool showCubes;

    public float cubeSize;

    public int segments;

    public int lastSegments;

    public Vector3[] points;
    
    public Vector3 offset;
    public Vector3 rotationOffset;

    public circleData()
    {
        circleColor = Color.red;
        cubeColor = Color.black;

        radius = 10;

        showCubes = false;

        cubeSize = 1;

        segments = 16;

        lastSegments = 0;

        points = new Vector3[segments + 1];

        offset = Vector3.zero;
        rotationOffset = Vector3.zero;
    }

    public circleData(Color color, float r, int segs)
    {
        circleColor = color;
        cubeColor = Color.black;
        radius = r;
        showCubes = false;
        cubeSize = 1;
        segments = segs;
        lastSegments = 0;
        points = new Vector3[segments + 1];
        offset = Vector3.zero;
        rotationOffset = Vector3.zero;
    }
}

public class CircleGizmo : MonoBehaviour {

    public float defaultRadius = 1;
    public int latSegs = 18;
    public int lonSegs = 36;
    public Color defaultColor = Color.red;

    public List<circleData> circles = new List<circleData>();
    

    public static void DrawCircle(circleData circle, Transform xform)
    {
        DrawCircle(circle, xform.worldToLocalMatrix);
    }

    public static void DrawCircle(circleData circle , Matrix4x4 matrix)
    {

        if (circle.segments <= 2)
            return;

        if (circle.lastSegments != circle.segments)
        {
            circle.points = getPointsNormalized(circle);
        }
        circle.lastSegments = circle.segments;

        Gizmos.matrix = matrix;

        Gizmos.color = circle.circleColor;
        for (int i = 0; i < circle.points.Length; i++)
        {
            Gizmos.DrawLine((circle.offset + circle.points[i] * circle.radius), circle.offset + circle.points[(i + 1) % circle.segments] * circle.radius);
        }

        if (circle.showCubes)
        {
            Gizmos.color = circle.cubeColor;
            for (int i = 0; i < circle.points.Length; i++)
            {
                Gizmos.DrawWireCube((circle.offset + circle.points[i] * circle.radius), Vector3.one * circle.cubeSize);
            }
        }
    }

    void OnDrawGizmos()
    {

        for (int c = 0; c < circles.Count; c++)
        { 
            circleData circle = circles[c];

            if (circle.segments <= 2)
                return;

            if (circle.lastSegments != circle.segments)
            {
                circle.points = getPointsNormalized(circle);
            }
            circle.lastSegments = circle.segments;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation * Quaternion.Euler(circle.rotationOffset), transform.lossyScale);

            Gizmos.color = circle.circleColor;
            for (int i = 0; i < circle.points.Length; i++)
            {
                Gizmos.DrawLine((circle.offset + circle.points[i] * circle.radius), circle.offset + circle.points[(i + 1) % circle.segments] * circle.radius);
            }

            if (circle.showCubes)
            {
                Gizmos.color = circle.cubeColor;
                for (int i = 0; i < circle.points.Length; i++)
                {
                    Gizmos.DrawWireCube((circle.offset + circle.points[i] * circle.radius), Vector3.one * circle.cubeSize);
                }
            }
        }
        Gizmos.matrix = Matrix4x4.identity;
    }

    static Vector3[] getPointsNormalized(circleData circle)
    {
        Vector3[] newPoints = new Vector3[circle.segments];

        double a = 0, step = Mathf.PI / circle.segments * 2;
        for (int i = 0; i < circle.segments; i++)
        {
            newPoints[i] = Vector3.forward * Mathf.Cos((float)a) + Vector3.left * Mathf.Sin((float)a);

            a += step;
        }

        return newPoints;

    }
}
