using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlShin : ControlSegmentGeneric
{
    public override string dofName { get { return "KneeFlexion"; } }
    public override int avatarIndex { get { return 1; } }
    public override int qIndex { get { return 1; } }
    protected override DrawingCallback drawingCallback { get {return drawManager.ControlShin;} }
    protected override Vector3 arrowOrientation { get {return new Vector3();} }
    protected override Quaternion circleOrientation { get { return Quaternion.Euler(90, 0, 0); } }
    public override int direction { get { return -1; } }
}
