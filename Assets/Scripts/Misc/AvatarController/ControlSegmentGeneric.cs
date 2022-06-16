using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlSegmentGeneric : MonoBehaviour
{
    [HideInInspector] public double dof;
    [HideInInspector] public int node = 0;

    protected Vector3 mouseDistance;
    protected Vector3 lastPosition;
    protected GameObject girl;

    protected GameObject arrowPrefab;
    protected GameObject arrow;

    protected GameObject circlePrefab;
    protected GameObject circle;

    protected abstract string dofName { get; }
    protected abstract int index { get; }

    public virtual void Init(GameObject _girl, double _dof)
    {
        girl = _girl;
        dof = _dof;

        if (!arrow)
        {
            arrowPrefab = (GameObject)Resources.Load("Arrow", typeof(GameObject));
            arrow = Instantiate(arrowPrefab, gameObject.transform.position + new Vector3(0, 0, 0.1f), Quaternion.identity);
        }
        else
            arrow.SetActive(true);

        if (!circle)
        {
            circlePrefab = (GameObject)Resources.Load("NodeCircle", typeof(GameObject));
            circle = Instantiate(circlePrefab, gameObject.transform.position, Quaternion.identity);
            circle.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        else
            circle.SetActive(true);
    }

    public void DestroyCircle()
    {
        circle.SetActive(false);
        arrow.SetActive(false);
    }

    private void Update()
    {
        if (arrow)
            arrow.transform.position = gameObject.transform.position + new Vector3(0, 0, 0.1f);

        if (circle)
            circle.transform.position = gameObject.transform.position;

    }

    void OnMouseDown()
    {
        if(ToolBox.GetInstance().GetManager<DrawManager>().isEditing)
        {
            lastPosition = Input.mousePosition;
            mouseDistance.x = (float)dof * 30f;
            ToolBox.GetInstance().GetManager<StatManager>().dofName = dofName;
        }
    }

    void OnMouseDrag()
    {
        if (ToolBox.GetInstance().GetManager<DrawManager>().isEditing)
        {
            Vector3 newPosition = Input.mousePosition;
            mouseDistance += newPosition - lastPosition;

            HandleDof(mouseDistance.x);
            lastPosition = newPosition;
        }
    }

    void OnMouseUp()
    {
        if (ToolBox.GetInstance().GetManager<DrawManager>().isEditing)
        {
            mouseDistance = Vector3.zero;
            lastPosition = Vector3.zero;
        }
    }

    protected virtual void HandleDof(float _value)
    {
        transform.rotation = Quaternion.Euler(0, -_value, 0);
        dof = -_value / 30;

        MainParameters.Instance.joints.nodes[1].Q[node] = (float)dof;
        ToolBox.GetInstance().GetManager<GameManager>().InterpolationDDL();
        ToolBox.GetInstance().GetManager<GameManager>().DisplayDDL(1, true);

        ToolBox.GetInstance().GetManager<DrawManager>().ControlShin((float)dof);
    }
}
