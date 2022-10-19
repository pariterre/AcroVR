using UnityEngine;

public class ControlThigh : ControlSegmentGeneric
{
    public override string dofName { get { return "HipFlexion"; } }
    public override int avatarIndexDDL { get { return 0; } }
    public override int jointSubIndex { get { return 0; } }
    public override int qIndex { get { return 0; } }
    protected override DrawingCallback drawingCallback { get {return avatarManager.SetThigh;} }
    protected override Vector3 arrowOrientation { get {return new Vector3();} }
    protected override Quaternion circleOrientation { get { return Quaternion.Euler(90, 0, 0); } }
    public override int direction { get { return 1; } }
}
