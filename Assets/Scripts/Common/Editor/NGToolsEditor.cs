// /!\ Save this script in an Editor folder /!\
// Open the window at Tools/NG Scene Camera to enable & configure the speeds.
// Use the Scroll Wheel to change speed on the fly.
// Press Ctrl to use the sub-speed.
// Use Ctrl + Scroll Wheel to change the sub-speed.

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
    [InitializeOnLoad]
    public class NGSceneCameraWindow : EditorWindow
    {
        public const string Title = "NG Scene Camera";

        private static readonly FieldInfo s_FlySpeed;

        private int currentSpeed;

        private GUIStyle rightToLeftLabel;
        private Vector2 scrollPosition;
        private List<float> moveSpeeds;

        static NGSceneCameraWindow()
        {
            var SceneViewMotion = typeof(Editor).Assembly.GetType("UnityEditor.SceneViewMotion");

            if (SceneViewMotion != null)
                s_FlySpeed = SceneViewMotion.GetField("s_FlySpeed", BindingFlags.NonPublic | BindingFlags.Static);
        }

        [MenuItem("Tools/" + Title)]
        public static void Open()
        {
            GetWindow<NGSceneCameraWindow>(Title);
        }

        protected virtual void OnEnable()
        {
            if (s_FlySpeed != null)
            {
                var n = EditorPrefs.GetInt(Title + ".moveSpeeds", -1);
                if (n > 0)
                {
                    moveSpeeds = new List<float>(n);

                    for (var i = 0; i < n; i++)
                        moveSpeeds.Add(EditorPrefs.GetFloat(Title + ".moveSpeeds." + i));
                }

                if (moveSpeeds == null || moveSpeeds.Count == 0)
                    moveSpeeds = new List<float> {.125f, .25f, .5f, 1f, 2f, 4f, 8f, 16f, 32f};

                currentSpeed = Mathf.Clamp(currentSpeed, 0, moveSpeeds.Count - 1);

                SceneView.onSceneGUIDelegate += OnSceneDelegate;
            }
        }

        protected virtual void OnDestroy()
        {
            if (s_FlySpeed != null)
            {
                SceneView.onSceneGUIDelegate -= OnSceneDelegate;

                EditorPrefs.SetInt(Title + ".moveSpeeds", moveSpeeds.Count);
                for (var i = 0; i < moveSpeeds.Count; i++)
                    EditorPrefs.SetFloat(Title + ".moveSpeeds." + i, moveSpeeds[i]);
            }
        }

        protected virtual void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                DrawSpeedList("Speed", moveSpeeds);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawSpeedList(string label, List<float> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    list[i] = EditorGUILayout.FloatField(label + " " + (i + 1), list[i]);

                    if (list.Count > 1 && GUILayout.Button("X", GUILayout.Width(30F)))
                    {
                        list.RemoveAt(i);
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add " + label, GUILayout.Width(100F)))
                    list.Add(0F);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnSceneDelegate(SceneView sceneView)
        {
            if (rightToLeftLabel == null)
            {
                rightToLeftLabel = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleRight
                };

                var state = new GUIStyleState();
                state.textColor = Color.white;

                rightToLeftLabel.fontStyle = FontStyle.Bold;
                rightToLeftLabel.normal = state;
            }

            Handles.BeginGUI();
            {
                var r = sceneView.position;

                r.x = r.width - 150F + 50F; // Hide text field.
                r.y = 120F;
                r.width = 150F;
                r.height = 20F;

                if (Event.current.type == EventType.Repaint && Event.current.control == false &&
                    Tools.viewTool == ViewTool.FPS)
                {
                    var r2 = r;
                    r2.xMax -= 55F;
                    r2.yMin += 4F;
                    r2.yMax -= 6F;
                    EditorGUI.DrawRect(r2, new Color(0.41f, 0.09f, 0.61f, 0.36f));
                }

                currentSpeed = (int) EditorGUI.Slider(r, currentSpeed, 0, moveSpeeds.Count - 1);
                r.x -= r.width;
                GUI.Label(r, moveSpeeds[currentSpeed].ToString(), rightToLeftLabel);
                r.x += r.width;

            }
            Handles.EndGUI();

            if (Tools.viewTool == ViewTool.FPS)
            {
                if (Event.current.type == EventType.ScrollWheel)
                {
                    currentSpeed = Mathf.Clamp(currentSpeed + (Event.current.delta.y < 0F ? 1 : -1), 0,
                        moveSpeeds.Count - 1);

                    Event.current.Use();
                }

                s_FlySpeed.SetValue(null, moveSpeeds[currentSpeed]);
            }
        }
    }
}