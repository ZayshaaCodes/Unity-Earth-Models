using UnityEngine;
using System.Collections;
using UnityEditor;

//[CustomEditor(typeof(CircleMeshGen))]
[CanEditMultipleObjects()]
public class CircleMeshGenEditor : Editor {
    

    //public override void OnInspectorGUI()
    //{
    //    CircleMeshGen tar = target as CircleMeshGen;

    //    if (GUILayout.Button("generate"))
    //    {
    //        tar.GenMesh(tar.radius, tar.segments, tar.angleOfArc);
    //    }


    //    base.DrawDefaultInspector();
    //}

}
