using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlLeftArmAbduction : ControlSegmentGeneric
{
    public override string dofName { get { return "LeftArmAbduction"; } }
    public override int avatarIndex { get { return 5; } }
    public override int jointSubIndex { get { return 1; } }
    public override int qIndex { get { return 4; } }
    protected override DrawingCallback drawingCallback { get {return drawManager.ControlLeftArmAbduction;} }
    protected override Vector3 arrowOrientation { get {return new Vector3(0.3f, 0.2f, 0.1f);} }
    protected override Quaternion circleOrientation { get { return Quaternion.Euler(90, 0, 0); } }
    public override int direction { get { return -1; } }
}
