using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {


    public GameObject spawnPrefab;

    public Vector3 direction;
    public Vector3 IterativeRotation;
    public Vector3 IterativeRotation2;

    public Vector3 initrotation;
    public int count = 10;
    

    public void Spawn()
    {

        var dir = transform.TransformDirection(direction);
        Quaternion rot = Quaternion.Euler(initrotation);
        var pos = transform.position;

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(spawnPrefab, pos, rot, transform);

            dir = Quaternion.Euler(IterativeRotation) * dir;
            rot *= Quaternion.Euler(IterativeRotation2);
            pos = pos + dir;
        }
    }

    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
