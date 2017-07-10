using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectSpawnerAxis))]
public class ObjectSpawnerAxisEditor : Editor {


    public override void OnInspectorGUI()
    {

        ObjectSpawnerAxis spawner = target as ObjectSpawnerAxis;

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
