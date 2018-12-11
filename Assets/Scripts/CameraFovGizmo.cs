using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFovGizmo : MonoBehaviour {

    public struct GizmoLine
    {
        public Vector3 p1;
        public Vector3 p2;
        public Color color;
    }
    
    public Color lineColor = Color.white;
    public List<GizmoLine> lines = new List<GizmoLine>();
    public float stepAngle = 5.0f;
    public float drawRadius;

    public float hFov;
    public float vFov;

    public float sizeX = 0.5f;

    public float aspect;

    private void Update()
    {
        UpdateFrame();
    }

    private void OnValidate()
    {

        if (stepAngle < .01f)
            stepAngle = .01f;

        UpdateFrame();
    }

    public void UpdateFrame()
    {

        Screen.orientation = ScreenOrientation.Landscape;
        lines.Clear();

        aspect = (float)Screen.width / Screen.height;
        vFov = hFov / aspect;
        
        float dist = (sizeX / 2) / Mathf.Tan(hFov / 2 * Mathf.Deg2Rad);

        var xVec = Vector3.right * sizeX / 2;
        var yVec = Vector3.up * sizeX / 2 / aspect; 
        var zVec = Vector3.forward * dist;
        
        lines.Add(new GizmoLine
        {
            p1 = -xVec - yVec + zVec,
            p2 = -xVec + yVec + zVec,
            color = lineColor
        });

        lines.Add(new GizmoLine
        {
            p1 = -xVec + yVec + zVec,
            p2 = xVec + yVec + zVec,
            color = lineColor
        });

        lines.Add(new GizmoLine
        {
            p1 = xVec + yVec + zVec,
            p2 = xVec - yVec + zVec,
            color = lineColor
        });

        lines.Add(new GizmoLine
        {
            p1 = xVec - yVec + zVec,
            p2 = -xVec - yVec + zVec,
            color = lineColor
        });


        // Center

        lines.Add(new GizmoLine
        {
            p1 = yVec + zVec,
            p2 = yVec - Vector3.up * .03f * sizeX + zVec,
            color = Color.blue
        });
    

        // Left side
        for (int i = 1; i < hFov / stepAngle / 2; i++)
        {
            var v = new Vector3(
                i * stepAngle / hFov * sizeX,
                sizeX / 2 / aspect,
                dist);

            lines.Add(new GizmoLine
            {
                p1 = v,
                p2 = v - Vector3.up * sizeX * (i % 5 == 0 ? 0.03f : 0.02f),
                color = Color.blue

            });

        }

        // Right side
        for (int i = 1; i < hFov / stepAngle / 2; i++)
        {
            var v = new Vector3(
                -i * stepAngle / hFov * sizeX,
                sizeX / 2 / aspect,
                dist);

            lines.Add(new GizmoLine
            {
                p1 = v,
                p2 = v - Vector3.up * sizeX * (i % 5 == 0 ? 0.03f : 0.02f),
                color = Color.blue

            });

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        for (int i = 0; i < lines.Count; i++)
        {
            Gizmos.color = (lines[i].color != null) ? lines[i].color : lineColor;

            Gizmos.DrawLine(lines[i].p1, lines[i].p2);
        }

    }

}
