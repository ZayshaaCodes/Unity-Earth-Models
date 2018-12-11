using System;
using System.Collections.Generic;
using UnityEngine;

class DoubleVector3
{

    double[] vals = new double[3];

    public double x
    {
        get { return vals[0]; }
        set { vals[0] = value; }
    }

    public double y
    {
        get { return vals[1]; }
        set { vals[1] = value; }
    }

    public double z
    {
        get { return vals[2]; }
        set { vals[2] = value; }
    }

    public DoubleVector3(Vector3 inVec)
    {
        vals[0] = inVec.x;
        vals[1] = inVec.y;
        vals[2] = inVec.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3((float)x, (float)y, (float)z);
    }
}