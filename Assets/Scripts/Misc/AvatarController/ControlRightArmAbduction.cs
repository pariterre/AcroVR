using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlRightArmAbduction : ControlSegmentGeneric
{
    public override string dofName { get { return "RightArmAbduction"; } }
    public override int avatarIndex { get { return 3; } }
    public override int jointSubIndex { get { return 1; } }
    public override int qIndex { get { return 2; } }
    protected override DrawingCallback drawingCallback { get {return drawManager.ControlRightArmAbduction;} }
    protected override Vector3 arrowOrientation { get {return new Vector3(0.3f, 0.2f, 0.1f);} }
    protected override Quaternion circleOrientation { get { return Quaternion.Euler(90, 0, 0); } }
    public override int direction { get { return -1; } }
}
