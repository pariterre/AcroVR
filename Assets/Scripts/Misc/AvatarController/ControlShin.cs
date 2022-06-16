using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlShin : ControlSegmentGeneric
{
    protected override string dofName { get { return "KneeFlexion"; } }
    protected override int index { get { return 1; } }

    public override void Init(GameObject _avatar, double _dof)
    {
        base.Init(_avatar, _dof);
    }

    void HandleDof(float _value)
    {
        transform.rotation = Quaternion.Euler(0, -_value, 0);
        dof = -_value / 30;

        MainParameters.Instance.joints.nodes[1].Q[node] = (float)dof;
        ToolBox.GetInstance().GetManager<GameManager>().InterpolationDDL();
        ToolBox.GetInstance().GetManager<GameManager>().DisplayDDL(1, true);

        ToolBox.GetInstance().GetManager<DrawManager>().ControlShin((float)dof);
    }
}
