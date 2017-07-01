using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(CircleGizmo))]
public class CircleGizmoEditor : Editor {
    
    
    public override void OnInspectorGUI()
    {
        var gizmo = target as CircleGizmo;

        serializedObject.Update();

        DrawDefaultInspector();

        EditorGUILayout.BeginVertical("Box");
        
        if (GUILayout.Button("Spawn Lat-Lon Grid"))
        {
            
            gizmo.circles.Clear();

            for (int i = 0; i < gizmo.lonSegs; i++)
            {
                var cd = new circleData(gizmo.defaultColor, gizmo.defaultRadius, 200);

                cd.rotationOffset = new Vector3(0, 0, i * 180 / gizmo.lonSegs);

                gizmo.circles.Add(cd);
            }

            for (int i = 1; i < gizmo.latSegs; i++)
            {
                var cd = new circleData(gizmo.defaultColor, gizmo.defaultRadius * Mathf.Sin((i * Mathf.PI / gizmo.latSegs)), 100);

                cd.rotationOffset = new Vector3(90, 0, 0);
                cd.offset = gizmo.defaultRadius * Mathf.Cos((i * Mathf.PI / gizmo.latSegs)) * Vector3.up;

                gizmo.circles.Add(cd);
            }

            EditorUtility.SetDirty(gizmo);

        }

        EditorGUILayout.EndVertical();
        
        if (GUI.changed)
            serializedObject.ApplyModifiedProperties();
    }

}
