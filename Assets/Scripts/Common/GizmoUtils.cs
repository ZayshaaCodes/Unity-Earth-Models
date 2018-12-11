using System;
using System.Collections.Generic;
using UnityEngine;

class GizmoUtils
{

    public static void DrawAxis(Vector3 pos, Quaternion rot, float length)
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(pos, rot * Vector3.right * length);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(pos, rot * Vector3.up * length);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(pos, rot * Vector3.forward * length);


    }
}