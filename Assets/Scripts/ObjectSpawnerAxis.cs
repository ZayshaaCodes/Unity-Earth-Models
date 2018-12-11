using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnerAxis : MonoBehaviour {


    public GameObject spawnPrefab;

    public Vector3 rotationCenter;
    public GameObject rotationCenterObject;
    public Vector3 rotationAxis;

    public float stepDistance;
    
    double unitsPerDegree { get { return (2f * Mathf.PI * rotationCenter.magnitude / 360f); } }
    
    public double radius { get { return rotationCenter.magnitude; } }
    
    public Vector3 initialSpawnRotation;
    public int count = 10;

    List<GameObject> spawnedObjects = new List<GameObject>();

    public bool drawLineGizmos = false;
    public bool drawCubeGizmos = false;
    [Range(0f, .1f)]
    public float gizmoCubeSize = .002f;
    [Range(0f, .1f)]
    public float gizmoHeightOffset = .001f;

    public void Spawn()
    {

        if (rotationCenterObject)
        {
            rotationCenter = rotationCenterObject.transform.position - transform.position ;
        }

        if (spawnedObjects.Count > 0)
            Clear();

        for (int i = 0; i < count; i++)
        {
            var newObj = Instantiate(spawnPrefab,transform.position, transform.rotation * Quaternion.Euler(initialSpawnRotation), transform);

            newObj.transform.RotateAround(transform.TransformPoint(rotationCenter), transform.TransformDirection(rotationAxis), (float)(stepDistance / unitsPerDegree) * i);

            newObj.transform.localScale = Vector3.one;

            spawnedObjects.Add(newObj); 
        }
    }

    public void Clear()
    {
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(spawnedObjects[i]);
        }

        spawnedObjects.Clear();
    }

    public void OnDrawGizmos()
{
        if (!drawCubeGizmos && !drawLineGizmos)
            return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;

        var p1 = -rotationCenter.normalized * gizmoHeightOffset;
        if (drawCubeGizmos)
            Gizmos.DrawWireCube(p1, Vector3.one * gizmoCubeSize);

        for (int i = 1; i < count; i++)
        {

            var p2 = (Quaternion.AngleAxis((float)((stepDistance / unitsPerDegree) * i), rotationAxis) * -rotationCenter - (rotationCenter.normalized * gizmoHeightOffset)) + rotationCenter;

            Gizmos.color = Color.yellow;

            if (drawLineGizmos)
                Gizmos.DrawLine( p1, p2 );

            Gizmos.color = Color.red;

            if (drawCubeGizmos)
                Gizmos.DrawWireCube(p2, Vector3.one * gizmoCubeSize);

            p1 = p2;

        }

    }
}

