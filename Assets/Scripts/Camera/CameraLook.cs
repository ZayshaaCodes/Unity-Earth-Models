using UnityEngine;
using System.Collections;

public class CameraLook : MonoBehaviour {

    public Camera cam;
    public float verticalSensitivity = 10f;
    public float horizontalSensitivity = 10f;
    public float zoomSpeed = 10f;

    public float moveSpeed = 10f;
    public float strafeSpeed = 10f;

    public float multiplier = 10f;

    public bool navigate = true;

    float lastTime = 0;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        lastTime = Time.realtimeSinceStartup;
    }
    
    void Update()
    {

        //if (Cursor.lockState != CursorLockMode.Locked)
        //    return;

        
        var mouseY = Input.GetAxis("Mouse Y");
        var mouseX = Input.GetAxis("Mouse X");

        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");
        
        float dt = Time.realtimeSinceStartup - lastTime;
        lastTime = Time.realtimeSinceStartup;

        Vector3 camMove = new Vector3(
            h * strafeSpeed * multiplier * Time.deltaTime, 0, 
            v * moveSpeed * multiplier * Time.deltaTime);

        if (Mathf.Abs(camMove.magnitude) < .0001f)
        {
            camMove = Vector3.zero;
        }

        cam.transform.position += cam.transform.forward * camMove.z + cam.transform.right * camMove.x;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            multiplier = 10;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            multiplier = .1f;
        }else
        {
            multiplier = 1;
        }

        if (!Input.GetMouseButton(1))
            return;

        // IF ALT DOWN
        if (Input.GetKey(KeyCode.LeftAlt))
        {

            cam.transform.position += cam.transform.TransformDirection(Vector3.forward) * mouseY * 10f;
            cam.fieldOfView += dt * mouseX * zoomSpeed;
            
        // IF SHIFT DOWN
        } else if (Input.GetKey(KeyCode.LeftShift)) {
    
            cam.transform.Rotate(cam.transform.TransformDirection(Vector3.left), dt * verticalSensitivity * mouseY / 2, Space.World);
            cam.transform.Rotate(Vector3.up, dt * horizontalSensitivity * mouseX / 2, Space.World);

        } else {

            transform.Rotate(Vector3.up, dt * horizontalSensitivity * mouseX, Space.World);
            transform.Rotate(cam.transform.TransformDirection(Vector3.left), dt * verticalSensitivity * mouseY, Space.World);

        }

    }
}
