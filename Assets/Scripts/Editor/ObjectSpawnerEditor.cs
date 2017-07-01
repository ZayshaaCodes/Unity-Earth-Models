using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectSpawner))]
public class ObjectSpawnerEditor : Editor {


    public override void OnInspectorGUI()
    {

        ObjectSpawner spawner = target as ObjectSpawner;

        DrawDefaultInspector();

        if (GUILayout.Button("Spawn"))
        {
            spawner.Spawn();
        }
        if (GUILayout.Button("Clear"))
        {
            spawner.Clear();
        }
    }

}
