using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SurfaceSpawner)), CanEditMultipleObjects]
public class SurfaceSpawnerEditor : Editor
{
    public SurfaceSpawner tar;

    private void Awake()
    {
        tar = target as SurfaceSpawner;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("SpawnInstances"))
        {
            tar.SpawnInstances();
        }
        if (GUILayout.Button("ClearInstances"))
        {
            tar.ClearInstances();
        }
    }
}
