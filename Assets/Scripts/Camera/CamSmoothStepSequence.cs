using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CamSmoothStepSequence : MonoBehaviour {


    [Range(.01f, 20f)]
    public float moveSpeed = 2f;
    public AnimationCurve movementCurve = new AnimationCurve();

    public bool showGizmos = false;
    public bool selectedOnly = false;

    public int StartPos = 2;

    public int index = 0;
    [System.Serializable]
    public struct Stop
    {
        public string name;
        public float nearClip;
        public float farClip;
        public float fov;
        public float aspect;

        public Vector3 pos;
        public Quaternion angle;
    }

    public List<Stop> stopPoints = new List<Stop>();
    
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();


        if (stopPoints.Count < 1)
        {
            stopPoints.Add(new Stop { name = "Start", fov = cam.fieldOfView, nearClip = cam.nearClipPlane, farClip = cam.farClipPlane, pos = transform.localPosition, angle = transform.localRotation, aspect = cam.aspect });
        }

        var globalWaypoints = new Vector3[stopPoints.Count];
        for (int i = 0; i < stopPoints.Count; i++)
        {
            globalWaypoints[i] = stopPoints[i].pos + transform.position;
        }
        
        transform.localPosition = stopPoints[index].pos;
        transform.localRotation = stopPoints[index].angle;
        cam.fieldOfView = stopPoints[index].fov;
        cam.nearClipPlane = stopPoints[index].nearClip;
        cam.farClipPlane = stopPoints[index].farClip;
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            lerpPotato(stopPoints[0], moveSpeed);
            index = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            lerpPotato(stopPoints[1], moveSpeed);
            index = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            lerpPotato(stopPoints[2], moveSpeed);
            index = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            lerpPotato(stopPoints[3], moveSpeed);
            index = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            lerpPotato(stopPoints[4], moveSpeed);
            index = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            lerpPotato(stopPoints[5], moveSpeed);
            index = 5;
        }
    }

    public void addPoint(string name, Vector3 pos, Quaternion rot, float near, float far, float fov, float aspect)
    {
        stopPoints.Insert(index + 1, new Stop { name = name, pos = pos, angle = rot, nearClip = near, farClip = far, fov = fov, aspect = aspect });
    }

    void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        int i = gameObject.GetComponent<CamSmoothStepSequence>().index;
        //if (Application.isPlaying)
        //{
        Matrix4x4 temp = Gizmos.matrix;

        for (int j = 0; j < stopPoints.Count; j++)
        {
            Stop pos = stopPoints[j];

            if (i == j) { 
                Gizmos.color = new Color(.5f, 1f, 1f, .4f);
                Gizmos.matrix = Matrix4x4.TRS(pos.pos, pos.angle, Vector3.one);
                Gizmos.DrawFrustum(Vector3.zero, pos.fov, pos.farClip, pos.nearClip, pos.aspect);
            } else if (!selectedOnly) { 
                Gizmos.color = new Color(1, 1, 1, .125f);
                Gizmos.matrix = Matrix4x4.TRS(pos.pos, pos.angle, Vector3.one);
                Gizmos.DrawFrustum(Vector3.zero, pos.fov, pos.farClip, pos.nearClip, pos.aspect);
            }

        if (i == j && selectedOnly)
            {

            }
        }

        Gizmos.matrix = temp;
        //}
    }

    public void lerpPotato(Stop stop, float speed)
    {
        StartCoroutine(lerpToPosition(stop, speed));
    }

    public IEnumerator lerpToPosition(Stop stop, float speed)
    {

        //print("coRutine");
        float t = 0;
        float curveVal;

        var cam = GetComponent<Camera>();

        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;
        float startFov = cam.fieldOfView;
        float startNear = cam.nearClipPlane;
        float startFar = cam.farClipPlane;
        float startAspect = cam.aspect;

        while (t < 1f)
        {
            t += Time.deltaTime / speed;
            curveVal = movementCurve.Evaluate(t);

            //float lerpPoint = 0.5f - 0.5f * Mathf.Cos(Mathf.PI * curveVal);

            transform.localPosition = Vector3.Lerp(startPos, stop.pos, curveVal);
            transform.localRotation = Quaternion.Lerp(startRot, stop.angle, curveVal);
            cam.fieldOfView = Mathf.Lerp(startFov, stop.fov, curveVal);
            cam.nearClipPlane = Mathf.Lerp(startNear, stop.nearClip, curveVal);
            cam.farClipPlane = Mathf.Lerp(startFar, stop.farClip, curveVal);
            //cam.aspect = Mathf.Lerp(startAspect, stop.fov, curveVal);

            yield return new WaitForEndOfFrame();

            //yield return new WaitForEndOfFrame();
        }

        transform.localPosition = stop.pos;
        transform.localRotation = stop.angle;

        cam.fieldOfView = stop.fov ;
        cam.nearClipPlane = stop.nearClip ;
        cam.farClipPlane = stop.farClip;
    }


    public void GotoStop(int id)
    {

        if (Application.isPlaying)
        {
            lerpPotato(stopPoints[id], moveSpeed);
        }
        else
        {
            CamSmoothStepSequence.Stop s = stopPoints[id];
            Camera cam = GetComponent<Camera>();

            transform.localPosition = s.pos;
            transform.localRotation = s.angle;
            cam.fieldOfView = s.fov;
            cam.farClipPlane = s.farClip;
            cam.nearClipPlane = s.nearClip;
        }
    }

}
