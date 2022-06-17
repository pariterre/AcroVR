using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlShin : ControlSegmentGeneric
{
    protected override string dofName { get { return "KneeFlexion"; } }
    protected override int jointIndex { get { return 1; } }
    protected override DrawingCallback drawingCallback { get {return drawManager.ControlShin;} }

}
