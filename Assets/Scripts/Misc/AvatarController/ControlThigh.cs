using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlThigh : ControlSegmentGeneric
{
    protected override string dofName { get { return "HipFlexion"; } }
    protected override int jointIndex { get { return 0; } }
    protected override DrawingCallback drawingCallback { get {return drawManager.ControlThigh;} }
}
