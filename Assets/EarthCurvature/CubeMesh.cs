using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeMesh : MonoBehaviour {

    public int xSegments = 3;
    public int ySegments = 5;

    public float xSize = 3f;
    public float ySize = 5f;

    MeshGenerator mg = new MeshGenerator();

    Mesh mesh;

    void OnValidate()
    {
        mg.startMesh();

        for (int i = 0; i < xSegments; i++)
        {
            
        }
        
    }
}
