using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CamSmoothStepSequence))]
[CanEditMultipleObjects]
public class CamSmoothStepSequenceEditor : Editor
{

    SerializedProperty index;
    CamSmoothStepSequence tar;


    bool showFeet = false;

    void OnEnable()
    {

        tar = (CamSmoothStepSequence)serializedObject.targetObject;

        // Setup the SerializedProperties.
        index = serializedObject.FindProperty("index");
    }




    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        CamSmoothStepSequence controller = (CamSmoothStepSequence)target;
        GUILayout.BeginVertical("Box");
        showFeet = GUILayout.Toggle(showFeet, "Show Feet (otherwise Miles)");

        for (int i = 0; i < tar.stopPoints.Count; i++)
        {

            CamSmoothStepSequence.Stop stop = tar.stopPoints[i];

            //ID BUTTON
            EditorGUILayout.BeginHorizontal("Textfield");

                if (GUILayout.Button(i.ToString() + ((i == index.intValue) ? "*" : ""), GUILayout.Width(25)))
                {
                    index.intValue = i;
                    tar.GotoStop(i);
                }

                GUILayout.BeginVertical(GUILayout.Width(75));
                    stop.name = GUILayout.TextField(stop.name);

                    GUILayout.BeginHorizontal();

                        GUILayout.Label("fov", GUILayout.Width(23));
                        float result;
                        if (float.TryParse(GUILayout.TextField(stop.fov.ToString()), out result))
                            stop.fov = result;

                    GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.BeginVertical();

                    stop.pos = EditorGUILayout.Vector3Field("", stop.pos * (showFeet ? 5280 : 1), GUILayout.ExpandWidth(true)) / (showFeet ? 5280 : 1);
                    stop.angle = Quaternion.Euler(EditorGUILayout.Vector3Field("", stop.angle.eulerAngles , GUILayout.ExpandWidth(true)));

                GUILayout.EndVertical();

                tar.stopPoints[i] = stop;

                GUILayout.BeginVertical(GUILayout.ExpandWidth(false), GUILayout.Width(15));
                    if (GUILayout.Button("",GUILayout.Height(15)))
                    {
                        CamSmoothStepSequence.Stop temp;
                        int targetIndex = ((i == 0) ? tar.stopPoints.Count - 1 : i - 1);
                        temp = tar.stopPoints[targetIndex];
                        tar.stopPoints[targetIndex] = tar.stopPoints[i];
                        tar.stopPoints[i] = temp;
                    }
                    if (GUILayout.Button("",GUILayout.Height(15)))
                    {
                        CamSmoothStepSequence.Stop temp;
                        int targetIndex = ((i == tar.stopPoints.Count - 1) ? 0 : i + 1);
                        temp = tar.stopPoints[targetIndex];
                        tar.stopPoints[targetIndex] = tar.stopPoints[i];
                        tar.stopPoints[i] = temp;
                    }
                GUILayout.EndVertical();


                if (GUILayout.Button("X",GUILayout.Width(20)))
                    {
                        tar.stopPoints.RemoveAt(i);
                    }
            EditorGUILayout.EndHorizontal();
            
        }

        GUILayout.EndVertical();
        if (GUILayout.Button("Add Point"))
        {

            var cTransform = controller.gameObject.transform;
            var cam = cTransform.gameObject.GetComponent<Camera>();

            controller.addPoint(
                name = "",
                controller.gameObject.transform.localPosition,
                controller.gameObject.transform.localRotation,
                cam.nearClipPlane,
                cam.farClipPlane,
                cam.fieldOfView,
                cam.aspect);
            //controller.toggleInstances();

            //force an update
            EditorUtility.SetDirty((CamSmoothStepSequence)target);
        }
        if (GUILayout.Button("CyclePositions"))
        {
            
            int temp;
            if (index.intValue + 1 >= controller.stopPoints.Count)
            {
                temp = 0;
            }
            else
            {
                temp = index.intValue + 1;
            }

            index.intValue = temp;

            tar.GotoStop(temp);
            
            EditorUtility.SetDirty((CamSmoothStepSequence)target);

        }
        if (GUILayout.Button("UpdatePosition"))
        {

            var cam = controller.gameObject.GetComponent<Camera>();
            
            CamSmoothStepSequence.Stop s = new CamSmoothStepSequence.Stop
            {
                name = controller.stopPoints[index.intValue].name,
                pos = controller.transform.localPosition,
                angle = controller.transform.localRotation,
                nearClip = cam.nearClipPlane,
                farClip = cam.farClipPlane,
                fov = cam.fieldOfView,
                aspect = cam.aspect
            };

            controller.stopPoints[index.intValue] = s;


            EditorUtility.SetDirty((CamSmoothStepSequence)target);
            
        }

        if (GUI.changed)
            serializedObject.ApplyModifiedProperties();

        DrawDefaultInspector();

    }


}
