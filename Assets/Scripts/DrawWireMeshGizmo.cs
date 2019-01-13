using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DrawWireMeshGizmo : MonoBehaviour
{

    public Color color = Color.black;

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = color;

        var mf = GetComponent<MeshFilter>();

        if (mf && mf.sharedMesh)
            Gizmos.DrawWireMesh(mf.sharedMesh);
    }
}
