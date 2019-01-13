using Cookie042.EarthHeightmap;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LatLonProjection))]
public class LatLonProjectionEditor : Editor
{
    string sourcePath = @"https://tile.mapzen.com/mapzen/terrain/v1/terrarium/{1}/{2}/{3}.png";

    private LatLonProjection tar;
    private GUIStyle style;

    private void OnEnable()
    {
        style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;

        tar = target as LatLonProjection;
        // Remove delegate listener if it has previously
        // been assigned.
        SceneView.onSceneGUIDelegate -= OnSceneGui;
        // Add (or re-add) the delegate.
        SceneView.onSceneGUIDelegate += OnSceneGui;
    }

    private void OnDisable()
    {

        SceneView.onSceneGUIDelegate -= OnSceneGui;
    }

    private void OnSceneGui(SceneView sceneView)
    {
        if (tar == null)
            return;
        for (int i = 0; i < tar.transform.childCount; i++)
        {
            Vector3 pos = tar.GetLocalWorldPos(tar.transform.GetChild(i).position , true);
            Handles.Label(tar.transform.GetChild(i).position + Vector3.up * .1f, $"lat:{pos.x:F3}\nlon:{pos.y:F3}\nelev:{pos.z:F4}", style);
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //tar.chunkX = EditorGUILayout.IntSlider("x", tar.chunkX, 0, tar.blockDivisions - 1);
        //tar.chunkY = EditorGUILayout.IntSlider("y", tar.chunkY, 0, tar.blockDivisions - 1);

        if (GUILayout.Button("Load Selection"))
        {
            tar.StartCoroutine(tar.LoadSelection());
        }

        //if (GUILayout.Button("Load Chunk"))
        //{
        //    tar.LoadSomeChunk();
        //}
        //if (GUILayout.Button("Load Block"))
        //{
        //    tar.LoadFullBlock();
        //}
        //if (GUILayout.Button("Load Data"))
        //{
        //    tar.LoadSomeHgtData();
        //}
        //if (GUILayout.Button("Download Data"))
        //{
        //    tar.DownloadSomeData();
        //}

        const float angleStep = 1 / 16f;

        var dd = tar.radiusEquator * Mathf.PI * 2 / 360f;

        GUILayout.Label($"Circumference: {tar.radiusEquator * Mathf.PI * 2:f4}");
        GUILayout.Label($"Degree Distance: {dd:f4}");
        GUILayout.Label($"Chunk Distance: {dd/(tar.blockDivisions - 1):f4}");

        //GUILayout.Label("Chunk Range:");
        //GUILayout.Label($"Lat: {tar.latitude + tar.chunkY * angleStep:F4} to {tar.latitude + (tar.chunkY + 1) * angleStep:F4}");
        //GUILayout.Label($"Lon: {tar.longitude + tar.chunkX * angleStep:F4} to {tar.longitude + (tar.chunkX + 1) * angleStep:F4}");

        if (tar.activeChunk != null && tar.activeChunk.texture != null)
        {
            //EditorGUILayout.ObjectField("texture", tar.activeChunk.texture, typeof(Texture2D),false);
            var rect = EditorGUILayout.GetControlRect(false, 225);
            rect.width = 225;

            EditorGUI.DrawPreviewTexture(rect, tar.activeChunk.texture);
        }

        if (tar.equirectangularProjectionMap)
        {

            var h = Screen.width / 2;
            var rect2 = EditorGUILayout.GetControlRect(false, h);
            EditorGUI.DrawPreviewTexture(rect2, tar.equirectangularProjectionMap);
            EditorGUI.DrawRect(new Rect(rect2.xMin + h * 2 * Mathf.InverseLerp(-180, 180, tar.longitude),
                rect2.yMin + h * Mathf.InverseLerp(90, -90, tar.latitude), h / 180f, h / 180f), Color.red);
        }

    }

}
