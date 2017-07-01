using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Tri
{
    public int p1, p2, p3;
}

public class MeshGenerator
{
    public List<Vector3> points;
    public List<Tri> faces;
    
    public void startMesh()
    {
        points = new List<Vector3>();
        faces = new List<Tri>();
    }
    
    public int addPoint(int pIndex, Vector3 newPoint)
    {
        points.Add(newPoint);

        return pIndex++;
    }

    public int AddTriangle(int triIndex, int p0, int p1, int p2)
    {
        return triIndex + 3;
    }

    //actually 2 triangles
    public int AddQuad(int triIndex, int p0, int p1, int p2, int p3)
    {

        return triIndex + 6;
    }

    public Mesh GetMesh()
    {
        Mesh m = new Mesh();

        m.vertices = points.ToArray();
        
        return m;
    }

    public void DrawGizmos()
    {
        
    }
}
