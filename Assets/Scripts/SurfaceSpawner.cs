using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class SurfaceSpawner : MonoBehaviour
{
    private SphereObject so;
    private PlaneObject po;
    public Transform startPoint;
    public Transform endPoint;

    public SingleUnityLayer drawLayer;

    public GameObject spawnPrefab;

    public float instanceSpacing = .1f;

    public Vector3 postionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    public Vector3 instanceScale = Vector3.one;
    public float scaleMultiplier = 1;

    public bool useFixedSpacing = true;

    public float p1Angle = 0;
    public float p2Angle = 0;

    private void OnEnable()
    {
        so = GetComponentInParent<SphereObject>();
        po = GetComponentInParent<PlaneObject>();

        if (startPoint == null)
            startPoint = transform;
        if (endPoint == null && transform.childCount > 0)
            endPoint = transform.GetChild(0);

    }

    // Update is called once per frame
    void Update()
    {
        (Vector3[] positions, Quaternion[] rotations) pts;
        if (so)
        {
            if (endPoint)
                pts = so.GetPointsOnArc(startPoint.position, endPoint.position, instanceSpacing, useFixedSpacing);
            else
            {
                var pt = so.ProjectToSphereSurface(startPoint.position);
                var normal = so.GetDeltaFromSphereCenter(startPoint.position);

                p1Angle = Vector3.Angle(transform.up, normal);

                var binormal = Vector3.Cross(normal, startPoint.forward);
                var tangent = Vector3.Cross(binormal, normal);

                pts = (new [] {pt}, new[] {Quaternion.LookRotation(tangent, normal)});
            }
        }
        else if (po)
        {
            pts = po.GetPointsOnLine(startPoint.position, endPoint.position, instanceSpacing, useFixedSpacing);
        }
        else
        {
            return;
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
                        Graphics.DrawMeshInstanced(mesh, i, mats[i], renderxforms, renderxforms.Length,
                            new MaterialPropertyBlock(), ShadowCastingMode.On, false, drawLayer.LayerIndex);
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
            if (so)
            {
                Gizmos.DrawLine(startPoint.position, so.ProjectToSphereSurface(startPoint.position));
                Gizmos.DrawLine(endPoint.position, so.ProjectToSphereSurface(endPoint.position));
            } else if (po)
            {

                Gizmos.DrawLine(startPoint.position, po.ProjectToPlanarSurface(startPoint.position));
                Gizmos.DrawLine(endPoint.position, po.ProjectToPlanarSurface(endPoint.position));
            }
        }
    }

    public void SpawnInstances()
    {

    }

    public void ClearInstances()
    {
        
    }
}