using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraPositionPresets))]
public class CameraPositionPresetsEditor : Editor
{
    private CameraPositionPresets tar;

    private void Awake()
    {
        tar = target as CameraPositionPresets;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Add Preset"))
        {
            tar.presets.Add(new CameraPositionPresets.Preset()
            {
                rot =  tar.transform.rotation,
                pos = tar.transform.position,
                hFov = tar.cam.fieldOfView
            });
        }
        if (GUILayout.Button("Update Preset"))
        {
            tar.presets[tar.selectedPreset] = new CameraPositionPresets.Preset()
            {
                rot = tar.transform.rotation,
                pos = tar.transform.position,
                hFov = tar.cam.fieldOfView
            };
        }
    }
}
