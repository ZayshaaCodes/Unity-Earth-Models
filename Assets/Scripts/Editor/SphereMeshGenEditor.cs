using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SphereMeshGen))]
public class SphereMeshGenEditor : Editor {
    

    public override void OnInspectorGUI()
    {

        SphereMeshGen tar = target as SphereMeshGen;

        DrawDefaultInspector();

        if (GUILayout.Button("Gen Sphere Mesh"))
        {
            tar.genSphereMesh();
        }

        GUILayout.BeginVertical();

        bool flag = false;
        int c = 0;
        for (int i = 0; i < tar.states.Count; i++)
        {
            var state = tar.states[i];

            if (state == null)
                continue;

            if (c == 0)
            {
                GUILayout.BeginHorizontal();
            }

            c++;

            if (GUILayout.Button(state.name))
            {
                if (Application.isPlaying)
                {
                    Debug.Log("playing");
                    tar.StartCoroutine(tar.LerpMeshState(new SphereMeshState("", tar), state, 2 ));
                } else
                {
                    tar.SetState(state);
                }
            }


            if (c % 4 == 0 && c > 0)
            {
                GUILayout.EndHorizontal();
                flag = false;
                c -= 4;
            }

        }
        if (flag)
        {
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        

    }
}


