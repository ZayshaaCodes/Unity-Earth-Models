using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LocalPositionOffest : MonoBehaviour
{
    private Vector3 startLocalPostion = Vector3.zero;
    private Vector3 pressedLocalPostion = Vector3.zero;

    public bool pressed = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        pressedLocalPostion = transform.localPosition - transform.parent.InverseTransformDirection(transform.forward*.25f);
        startLocalPostion = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pressed = !pressed;
            if (pressed)
            {
                transform.localPosition = pressedLocalPostion;
            }
            else

            {
                transform.localPosition = startLocalPostion;
            }
            
        }
    }
}
