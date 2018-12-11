using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEditor;

public class MenuStuff : Editor
{

    [MenuItem("Stuff/Rename")]
    static void RenameChildren()
    {

        GameObject[] sel = Selection.gameObjects;

        foreach (var go in sel)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                go.transform.GetChild(i).name = go.name + " " + i.ToString();
            }
        }
    }
    

    [MenuItem("Stuff/Move To Mouse #%z")]
    public static void MoveSelToMouse()
    {       
        RaycastHit hit;
        
        if (Physics.Raycast(SceneMouse.mouseRay, out hit, 100f))
        {
            var sel = Selection.gameObjects;
            for (int i = 0; i < sel.Length; i++)
            {
                var obj = sel[i];


                obj.transform.position = hit.point;
            }
        }
    }

    [MenuItem("Stuff/Collapse TempHide Hierarchy #&c")]
    public static void Collapse()
    {
        GameObject go = null;
        foreach (var obj in SceneRoots())
        {

            if (obj.name == "TempHide")
            {
                go = obj;
                Debug.Log(obj.name);
                Selection.activeObject = go;
                break;
            }
        }
        if (go == null) return;
        if (go.transform.childCount == 0) return;

        // get a reference to the hierarchy window
        FocusOnWindow("Hierarchy");
        var hierarchy = EditorWindow.focusedWindow;

        // select our go
        // create a new key event (RightArrow for collapsing, LeftArrow for folding)
        var key = new Event { keyCode = KeyCode.LeftArrow, type = EventType.KeyDown };
        // finally, send the window the event
        hierarchy.SendEvent(key);
    }

    [MenuItem("Stuff/Rename")]
    static void SelectAllOfTagWizard()
    {
        GameObject.FindGameObjectsWithTag("");
    }

    [MenuItem("Stuff/Layout Children")]
    static void LayoutChidren()
    {
        GameObject[] sel = Selection.gameObjects;

        foreach (var item in sel)
        {
            int c = item.transform.childCount;
            for (int i = 0; i < c; i++)
            {
                var child = item.transform.GetChild(i);

                child.transform.position = item.transform.position - item.transform.right * i * .5f;
            }
        }
    }

    private static GameObject clipboard;
    [MenuItem("Stuff/Copy Object #%&c")]
    static void CopySelectedObject()
    {
        clipboard = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject);
    }

    [MenuItem("Stuff/Paste Prefab Over Selection Object %#&v")]
    static void PasteSelectedObject()
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            var newObj = (GameObject)PrefabUtility.InstantiatePrefab(clipboard, selectedObject.scene);
            newObj.transform.parent = selectedObject.transform.parent;
            newObj.transform.position = selectedObject.transform.position;
            newObj.transform.rotation = selectedObject.transform.rotation;
            newObj.transform.localScale = selectedObject.transform.localScale;
            DestroyImmediate(selectedObject);
        }
    }

    [MenuItem("Stuff/Group Into Child")]
    static void BuildGroup()
    {
        GameObject[] sel = Selection.gameObjects;

        if (sel.Length < 1)
            return;

        var go = new GameObject("Group");
        go.transform.parent = sel[0].transform.parent;
        
        //average all selected positions to find center.
        var center = Vector3.zero;
        foreach (var item in sel)
        {
            center += item.transform.position;
        }

        go.transform.position = center / sel.Length;

        foreach (var item in sel)
        {
            item.transform.parent = go.transform;
        }
    }


    [MenuItem("Stuff/Randomize Rotation 90's")]
    static void RandomRotation90()
    {
        GameObject[] sel = Selection.gameObjects;
        for (int i = 0; i < sel.Length; i++)
        {
            var obj = sel[i].transform;

            var up = obj.up;
            var right = obj.right;
            var forward = obj.forward;

            obj.Rotate(up, 90 * Random.Range(0, 3));
            obj.Rotate(right, 90 * Random.Range(0, 3));
            obj.Rotate(forward, 90 * Random.Range(0, 3));
            obj.Rotate(up, 90 * Random.Range(0, 3));
            obj.Rotate(right, 90 * Random.Range(0, 3));
            obj.Rotate(forward, 90 * Random.Range(0, 3));
        }
    }

    [MenuItem("Stuff/Raycast to floor")]
    static void RaycastToFloor()
    {

        GameObject[] sel = Selection.gameObjects;

        RaycastHit hitInfo;
        for (int i = 0; i < sel.Length; i++)
        {
            var gObj = sel[i];

            var bottomCenter = GetBottomCenterPoint(gObj);


        }


    }


    private static Vector3 GetBottomCenterPoint(GameObject targetObject)
    {

        var meshFilters = targetObject.GetComponentsInChildren<MeshFilter>();
        float f = 0f;
        foreach (var filter in meshFilters)
        {
            var sizeVector = filter.sharedMesh.bounds.size;

            //use the largest axis;
            if (sizeVector.x > f)
                f = sizeVector.x;
            if (sizeVector.y > f)
                f = sizeVector.y;
            if (sizeVector.z > f)
                f = sizeVector.z;

            return Vector3.one / f;
        }

        return Vector3.one;
    }


    public static void FocusOnWindow(string window)
    {
        EditorApplication.ExecuteMenuItem("Window/General/" + window);
    }

    public static GameObject[] SceneRoots()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
    }

}

public class NewGroupWindow : EditorWindow
{

    string newName = "Group";

    [MenuItem("Stuff/windowTest")]
    static void ShowWindow()
    {
        var win = new NewGroupWindow();
        win.ShowUtility();
    }

    private void OnGUI()
    {
        newName = EditorGUILayout.TextField("New Group Name", newName);
    }
}