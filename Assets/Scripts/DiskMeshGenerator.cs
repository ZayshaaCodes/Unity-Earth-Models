using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DiskMeshGenerator : MonoBehaviour
{

    public double radius = 3959;

    [Range(3, 100)]
    public int RadialSegments = 5;

    [Range(1,100)]
    public int AxialSegments = 1;

    public int pointCount => 1 + AxialSegments * (RadialSegments + 1);
    public int triCount => (RadialSegments + (RadialSegments * (AxialSegments - 1)) * 2) * 3;

    public Mesh generatedMesh;
    public Material mat;

    private void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        BuildMesh();

        Graphics.DrawMesh(generatedMesh, transform.localToWorldMatrix, mat, 0);

    }


    Vector3[] points;
    int[] tris;
    public void BuildMesh()
    {
        points = new Vector3[pointCount];
        tris = new int[triCount];

        points[0] = Vector3.zero;

        double angleStep = 2 * Math.PI / RadialSegments;

        int i = 1;

        for (int j = 0; j <= RadialSegments; j++)
        {
            points[i] = new Vector3((float) (radius * Math.Cos(angleStep * j)),
                0, (float) (radius * Math.Sin(angleStep * j)));
            i++;
        }

        var t = 0;
        for (int j = 1; j <= RadialSegments; j++)
        {
            tris[t] = 0;
            tris[t + 1] = j + 1;
            tris[t + 2] = j;
            t += 3;
        }

        if (generatedMesh == null)
             generatedMesh = new Mesh();

        generatedMesh.vertices = points;
        generatedMesh.triangles = tris;
        generatedMesh.RecalculateNormals();
        generatedMesh.RecalculateBounds();

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0f, 0f, 0.42f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireMesh(generatedMesh);
    }
}
