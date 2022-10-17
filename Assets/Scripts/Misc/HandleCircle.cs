using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCircle : MonoBehaviour
{
    [HideInInspector] public double[] dof;

    protected AvatarManager avatarManager;

    private Vector3 mouseDistance;
    private Vector3 lastPosition;
    private int directionRotate = 0;
    private GameObject girl;
    public GameObject target;
    public int node = 0;

    void Start()
    {
        avatarManager = ToolBox.GetInstance().GetManager<AvatarManager>();
    }

    public void Init(GameObject _girl, GameObject _target, double[] _dof)
    {
        girl = _girl;
        target = _target;
        dof = _dof;
    }

    void OnMouseDown()
    {
        directionRotate = 0;
        lastPosition = Input.mousePosition;
        if (target.name == "shin.L" || target.name == "shin.R")
        {
            mouseDistance.x = (float)dof[1] * 30f;
        }
        else if (target.name == "thigh.L" || target.name == "thigh.R")
        {
            mouseDistance.x = (float)dof[0] * 30f;
        }
        else if (target.name == "upper_arm.L")
        {
            mouseDistance.x = (float)dof[3] * 30f;
            mouseDistance.y = (float)dof[2] * 30f;
        }
        else if (target.name == "upper_arm.R")
        {
            mouseDistance.x = (float)dof[5] * 30f;
            mouseDistance.y = (float)dof[4] * 30f;
        }
    }

    void OnMouseDrag()
    {
        Vector3 newPosition = Input.mousePosition;
        mouseDistance += newPosition - lastPosition;

        int _avatarIndex = 0;
        if (target.name == "shin.L" || target.name == "shin.R")
        {
            HandleDof(_avatarIndex, 1, mouseDistance.x);
            ToolBox.GetInstance().GetManager<StatManager>().dofName = "KneeFlexion";
        }
        else if (target.name == "thigh.L" || target.name == "thigh.R")
        {
            HandleDof(_avatarIndex, 0, -mouseDistance.x);
            ToolBox.GetInstance().GetManager<StatManager>().dofName = "HipFlexion";
        }
        else if (target.name == "upper_arm.L")
        {
            if (directionRotate == 0)
            {
                if (Mathf.Abs(newPosition.x - lastPosition.x) - Mathf.Abs(newPosition.y - lastPosition.y) > 2) directionRotate = 1;
                else if(Mathf.Abs(newPosition.x - lastPosition.x) - Mathf.Abs(newPosition.y - lastPosition.y) < -2) directionRotate = 2;
            }
            else if (directionRotate == 1)
            {
                HandleDof(_avatarIndex, 3, mouseDistance.x);
                ToolBox.GetInstance().GetManager<StatManager>().dofName = "LeftArmAbduction";
            }
            else if (directionRotate == 2)
            {
                HandleDof(_avatarIndex, 2, mouseDistance.y);
                ToolBox.GetInstance().GetManager<StatManager>().dofName = "LeftArmFlexion";
            }
        }
        else if (target.name == "upper_arm.R")
        {
            if (directionRotate == 0)
            {
                if (Mathf.Abs(newPosition.x - lastPosition.x) - Mathf.Abs(newPosition.y - lastPosition.y) > 2) directionRotate = 1;
                else if (Mathf.Abs(newPosition.x - lastPosition.x) - Mathf.Abs(newPosition.y - lastPosition.y) < -2) directionRotate = 2;
            }
            else if (directionRotate == 1)
            {
                HandleDof(_avatarIndex, 5, mouseDistance.x);
                ToolBox.GetInstance().GetManager<StatManager>().dofName = "RightArmAbduction";
            }
            else if (directionRotate == 2)
            {
                HandleDof(_avatarIndex, 4, mouseDistance.y);
                ToolBox.GetInstance().GetManager<StatManager>().dofName = "RightArmFlexion";
            }
        }

        lastPosition = newPosition;
    }

    void OnMouseUp()
    {
        mouseDistance = Vector3.zero;
        directionRotate = 0;
    }

    void HandleDof(int _avatarIndex, int _dof, float _value)
    {
        if (directionRotate == 2) transform.rotation = Quaternion.Euler(-_value, 0, 0);
        else transform.rotation = Quaternion.Euler(0, -_value, 0);

        if (girl.transform.forward.x >= 0)
            dof[_dof] = _value / 30;
        else
            dof[_dof] = -_value / 30;

        avatarManager.LoadedModels[0].Joints.nodes[_dof].Q[node] = (float)dof[_dof];
        ToolBox.GetInstance().GetManager<GameManager>().InterpolationDDL(_avatarIndex);
        ToolBox.GetInstance().GetManager<GameManager>().DisplayDDL(_dof, true);
    }
}
