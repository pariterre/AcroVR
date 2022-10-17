using UnityEngine;

public class ControlLeftArmFlexion : ControlSegmentGeneric
{
    public override string dofName { get { return "LeftArmFlexion"; } }
    public override int avatarIndexDDL { get { return 4; } }
    public override int jointSubIndex { get { return 0; } }
    public override int qIndex { get { return 5; } }
    protected override DrawingCallback drawingCallback { get { return avatarManager.SetLeftArmFlexion; } }
    protected override Vector3 arrowOrientation { get { return new Vector3(0.3f, 0.2f, 0.1f); } }
    protected override Quaternion circleOrientation { get { return Quaternion.Euler(0, 90, 90); } }
    public override int direction { get { return -1; } }
}