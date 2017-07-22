using UnityEngine;
using System.Collections;
using UnityEditor;

public class TexSize : EditorWindow
{

    public Texture2D _tex;
    public GUILayoutOption[] par;

    [MenuItem("Tools/TexSize")]
    static void OpenWindow()
    {
        TexSize window = (TexSize)EditorWindow.GetWindow(typeof(TexSize));
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        _tex = (Texture2D)EditorGUILayout.ObjectField("L", _tex, typeof(Texture2D), false, par);
        if (GUILayout.Button("Apply", par))
        {
            string path = AssetDatabase.GetAssetPath(_tex);
            TextureImporter t = AssetImporter.GetAtPath(path) as TextureImporter;
            t.maxTextureSize = 8192;
            AssetDatabase.ImportAsset(path);
        }
    }
}

public class TexSize2 : EditorWindow
{

    public Cubemap _tex;
    public GUILayoutOption[] par;

    [MenuItem("Tools/TexSizeCubemap")]
    static void OpenWindow()
    {
        TexSize window = (TexSize)EditorWindow.GetWindow(typeof(TexSize));
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        _tex = (Cubemap)EditorGUILayout.ObjectField("L", _tex, typeof(Cubemap), false, par);
        if (GUILayout.Button("Apply", par))
        {
            string path = AssetDatabase.GetAssetPath(_tex);
            TextureImporter t = AssetImporter.GetAtPath(path) as TextureImporter;
            t.maxTextureSize = 8192;
            AssetDatabase.ImportAsset(path);
        }
    }



}
