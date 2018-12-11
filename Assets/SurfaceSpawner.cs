using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class SurfaceSpawner : MonoBehaviour
{
    private SphereObject so;
    private PlaneObject po;
    public Transform startPoint;
    public Transform endPoint;

    public GameObject spawnPrefab;

    public float instanceSpacing = .1f;

    public Vector3 postionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    public Vector3 instanceScale = Vector3.one;
    public float scaleMultiplier = 1;

    public bool useFixedSpacing = true;

    private void OnEnable()
    {
        so = GetComponentInParent<SphereObject>();
        po = GetComponentInParent<PlaneObject>();

        if (startPoint == null && transform.childCount > 0)
        {
            startPoint = transform;
        }
        if (endPoint == null && transform.childCount > 0)
        {
            endPoint = transform.GetChild(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        (Vector3[] positions, Quaternion[] rotations) pts;
        if (so)
        {
            pts = so.GetPointsOnArc(startPoint.position, endPoint.position, instanceSpacing, useFixedSpacing);
        }
        else
        {
            pts = po.GetPointsOnLine(startPoint.position, endPoint.position, instanceSpacing, useFixedSpacing);
        }

        var lr = GetComponentInChildren<LineRenderer>();
        if (lr)
        {
            lr.positionCount = pts.positions.Length;
            lr.SetPositions(pts.positions);
        }

        if (spawnPrefab)
        {
            Matrix4x4[] renderxforms = new Matrix4x4[pts.positions.Length];

            for (int i = 0; i < renderxforms.Length; i++)
            {
                renderxforms[i] = Matrix4x4.TRS(pts.positions[i] + postionOffset, pts.rotations[i] * Quaternion.Euler(rotationOffset), instanceScale * scaleMultiplier);
            }

            var mfs = spawnPrefab.GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter meshFilter in mfs)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Material[] mats = meshFilter.GetComponent<MeshRenderer>()?.sharedMaterials;

                if (mats != null && mesh != null)
                    for (var i = 0; i < mats.Length; i++)
                        Graphics.DrawMeshInstanced(mesh, i, mats[i], renderxforms, renderxforms.Length);
            }
        }
    }

    public (Vector3[] positions, Quaternion[] rotations) GetPoints(PlaneObject planeObject)
    {
        return (null, null);
    }


    private void OnDrawGizmos()
    {
        if (startPoint && endPoint)
        {
            //Gizmos.DrawWireSphere(so.ProjectToSphereSurface(startPoint.position), .1f);
            //Gizmos.DrawWireSphere(so.ProjectToSphereSurface(endPoint.position), .1f);

            Gizmos.DrawLine(startPoint.position, so.ProjectToSphereSurface(startPoint.position));
            Gizmos.DrawLine(endPoint.position, so.ProjectToSphereSurface(endPoint.position));
        }

    }
}