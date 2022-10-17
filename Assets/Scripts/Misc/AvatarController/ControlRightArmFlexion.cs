using UnityEngine;

public class ControlRightArmFlexion : ControlSegmentGeneric
{
    public override string dofName { get { return "RightArmFlexion"; } }
    public override int avatarIndexDDL { get { return 2; } }
    public override int jointSubIndex { get { return 0; } }
    public override int qIndex { get { return 3; } }
    protected override DrawingCallback drawingCallback { get { return avatarManager.SetRightArmFlexion; } }
    protected override Vector3 arrowOrientation { get { return new Vector3(0.3f, 0.2f, 0.1f); } }
    protected override Quaternion circleOrientation { get { return Quaternion.Euler(0, 90, 90); } }
    public override int direction { get { return -1; } }
}
