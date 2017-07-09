using UnityEngine;
using System.Collections;

public class MouseMovement : MonoBehaviour {

    public float xTurnSpeed = 10;
    public float yTurnSpeed = 10;
    
    public float moveSpeed = 10; // units per second

    public float zoomSpeed = 10;

    public float h;
    public float v;

    private Camera cam;

	// Use this for initialization
	void Start () {
        cam = gameObject.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {

        float mouseX = -Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        float h = Input.GetAxis("Horizontal") * moveSpeed;
        float v = Input.GetAxis("Vertical") * moveSpeed;

        if (Input.GetMouseButton(1))
        {

            if (Input.GetKey(KeyCode.LeftAlt))
            {
                cam.fieldOfView += mouseY / (1/cam.fieldOfView) * zoomSpeed * Time.deltaTime;
                cam.fieldOfView = Mathf.Abs(cam.fieldOfView);
                if (cam.fieldOfView >= 160)
                    cam.fieldOfView = 160;

            } else
            {
                transform.Rotate(Vector3.up, mouseX * -xTurnSpeed * Time.deltaTime, Space.World);
                transform.Rotate(transform.TransformDirection(Vector3.left), mouseY * -yTurnSpeed * Time.deltaTime, Space.World);
            }
        }

        
        cam.transform.position += cam.transform.forward * v * Time.deltaTime;
        cam.transform.position += cam.transform.right * h * Time.deltaTime;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        moveSpeed += scroll * 50 * Time.deltaTime;

    }
}
