using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DiskMeshGenerator : MonoBehaviour
{

    public int RadialSegments = 100;

    public int AxialSegments = 1;

    public Mesh generatedMesh;

    private void OnEnable()
    {
        if (!generatedMesh)
        {
            generatedMesh = new Mesh();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void BuildMesh()
    {

        Vector3[] points = new Vector3[2];
        int[] tris = new int[2];


    }
}
