using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcroVrVector3 : AcroVrVector
{
    public AcroVrVector3()
        : base(3)
    {

    }
    public AcroVrVector3(double v0, double v1, double v2)
        : base(3)
    {
        Value[0, 0] = v0;
        Value[1, 0] = v1;
        Value[2, 0] = v2;
    }
}
