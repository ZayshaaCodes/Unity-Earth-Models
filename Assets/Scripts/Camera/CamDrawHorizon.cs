using UnityEngine;
using UnityEditor;
using System.Collections;

public class CamDrawHorizon : MonoBehaviour {

    public bool showAstranomicalHorizon = false;
    public bool showCameraCenterH = true;
    public bool showCameraCenterV = true;

    public Color colorAstranomicalHorizon = Color.black;
    public Color ColorCenterHorizontal = Color.red;
    public Color ColorCenterVertical = Color.red;
    public float length = 10f;
    public float distance = 1f;

    [MenuItem("Tools/TestCode")]
    static void runCode()
    {

        var v = new Vector3(-.25f, .24f, -1.5f);
        Debug.Log(Vector3.Project(v, Vector3.right));
    }

    void OnDrawGizmos()
    {
        
        if (showAstranomicalHorizon)
        {
            Gizmos.color = colorAstranomicalHorizon;
            Gizmos.DrawLine(
                transform.position + Vector3.forward * length / 2 + Vector3.right * distance,
                transform.position + Vector3.back * length / 2 + Vector3.right * distance);
        }
        if (showCameraCenterH)
        {
            Gizmos.color = ColorCenterHorizontal;
            Gizmos.DrawLine(
                transform.position + transform.TransformDirection(Vector3.left) * length / 2 + transform.TransformDirection(Vector3.forward) * distance,
                transform.position + transform.TransformDirection(Vector3.right) * length / 2 + transform.TransformDirection(Vector3.forward) * distance);

        }
        if (showCameraCenterV)
        {
            Gizmos.color = ColorCenterHorizontal;
            Gizmos.DrawLine(
                transform.position + transform.TransformDirection(Vector3.up) * length / 2 + transform.TransformDirection(Vector3.forward) * distance,
                transform.position + transform.TransformDirection(Vector3.down) * length / 2 + transform.TransformDirection(Vector3.forward) * distance);

        }
    }

}