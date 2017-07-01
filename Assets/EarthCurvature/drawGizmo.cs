using UnityEngine;
using System.Collections;

public class drawGizmo : MonoBehaviour {

    public bool enabled = true;

    public Color color = Color.white;
    public Color color2 = Color.red;
    public Vector3 scale = Vector3.one;

    public bool wire = false;
    public bool drawCube = false;
    public bool drawCollider = false;

    public bool drawRay = false;
    public bool rayBothDirections = true;
    public Vector3 rayDir = Vector3.forward;

    public bool drawfustrum = false;
    public float nearClip = 1f;
    public float farClip = 10f;

    public bool drawSphere = false;
    public float radius = 1f;

    public bool drawMesh = false;
    public Mesh mesh;

    public bool drawConnector = false;
    public GameObject tar;

    void OnDrawGizmos()
    {
        if (!enabled)
            return;

        Gizmos.color = color;
        Gizmos.matrix = transform.localToWorldMatrix;

        if (drawfustrum)
        {
            Camera cam = GetComponent<Camera>();
            if (cam != null)
            {
                Gizmos.DrawFrustum(Vector3.zero, cam.fieldOfView + .1f, farClip, nearClip, cam.aspect);
            }
        }

        if (drawRay)
        {
            Gizmos.DrawRay(Vector3.zero, rayDir);

            if (rayBothDirections)
            {
                Gizmos.color = color2;
                Gizmos.DrawRay(Vector3.zero, -rayDir);
                Gizmos.color = color;
            }


        }

        if (drawCube)
        {
            if (wire)
            {
                //Gizmos.DrawWireCube(transform.position, transform.localScale);
                Gizmos.DrawWireCube(Vector3.zero, scale);
            } else
            {
                //Gizmos.DrawCube(transform.position, transform.localScale);
                Gizmos.DrawCube(Vector3.zero, scale);
            }
        }

        if (drawSphere)
        {
            if (wire)
            {
                Gizmos.DrawWireSphere(Vector3.zero, radius);
            }
            else
            {
                Gizmos.DrawSphere(Vector3.zero, radius);
            }
        }

        if (drawCollider)
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }

        }

        if (drawMesh && mesh != null)
        {
            Gizmos.DrawWireMesh(mesh);
        }


        Gizmos.matrix = Matrix4x4.identity;

        
        if (drawConnector)
        {
            Gizmos.DrawLine(transform.position, tar.transform.position);
        }

    }

}
