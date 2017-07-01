using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class forceNearClip : MonoBehaviour {

    public Camera cam;

    public float nearClip = .001f;
    public float fovOverride = .1f;

    public bool overrideFov = true;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {

        cam.nearClipPlane = nearClip;

        if (overrideFov)
        {
            cam.fieldOfView = fovOverride;
        }
    }
}
