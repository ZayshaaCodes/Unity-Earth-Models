using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class forceNearClip : MonoBehaviour {

    public Camera cam;

    public bool overrideNearClip = true;
    public float nearClip = .001f;

    public bool overrideFov = true;
    [Range(.1f,90f)]
    public float fovOverride = .1f;


	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        if (overrideNearClip)
        {
            cam.nearClipPlane = nearClip;
        }

        if (overrideFov)
        {
            cam.fieldOfView = fovOverride;
        }
    }
}
