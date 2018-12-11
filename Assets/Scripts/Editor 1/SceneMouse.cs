using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public static class SceneMouse
{
    public static Ray mouseRay;

    static SceneMouse()
    {
        SceneView.onSceneGUIDelegate += view =>
        {
            var e = Event.current;
            if (e != null)
            {
                mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                //Debug.Log(mouseRay);
            }
        };
    }
}