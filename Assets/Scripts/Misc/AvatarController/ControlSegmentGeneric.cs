using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlSegmentGeneric : MonoBehaviour
{
    public float angle { get; protected set;}
    [HideInInspector] public int node = 0;

    protected Vector3 mouseDistance;
    protected Vector3 lastPosition;
    protected GameObject avatar;

    protected GameObject arrowPrefab;
    protected GameObject arrow;

    protected GameObject circlePrefab;
    protected GameObject circle;

    protected abstract string dofName { get; }
    protected abstract int jointIndex { get; }
    protected abstract DrawingCallback drawingCallback { get; }

    // Cache variables
    bool isInitialized = false;
    protected GameManager gameManager;
    protected DrawManager drawManager;
    protected delegate void DrawingCallback(float value);

    public virtual void Init(GameObject _avatar, float _angle)
    {
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();

        avatar = _avatar;
        angle = _angle;

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

        isInitialized = true;
    }

    public void DestroyCircle()
    {
        if (!isInitialized) return;

        circle.SetActive(false);
        arrow.SetActive(false);
    }

    void Update()
    {
        if (!isInitialized) return;

        if (arrow)
            arrow.transform.position = gameObject.transform.position + new Vector3(0, 0, 0.1f);

        if (circle)
            circle.transform.position = gameObject.transform.position;
    }

    void OnMouseDown()
    {
        if (!isInitialized) return;

        if(drawManager.isEditing)
        {
            lastPosition = Input.mousePosition;
            mouseDistance.x = angle * 30f;
            ToolBox.GetInstance().GetManager<StatManager>().dofName = dofName;
        }
    }

    void OnMouseDrag()
    {
        if (!isInitialized) return;

        if (drawManager.isEditing)
        {
            Vector3 newPosition = Input.mousePosition;
            mouseDistance += newPosition - lastPosition;

            HandleDof(mouseDistance.x);
            lastPosition = newPosition;
        }
    }

    void OnMouseUp()
    {
        if (!isInitialized) return;

        if (drawManager.isEditing)
        {
            mouseDistance = Vector3.zero;
            lastPosition = Vector3.zero;
        }
    }

    protected virtual void HandleDof(float _value)
    {
        if (!isInitialized) return;

        Debug.LogWarning(_value);
        transform.rotation = Quaternion.Euler(0, -_value, 0);
        angle = -_value / 30; // Don't know what this 30 is for

        MainParameters.Instance.joints.nodes[jointIndex].Q[node] = angle;
        gameManager.InterpolationDDL();
        gameManager.DisplayDDL(jointIndex, true);

        drawingCallback(angle);
    }
}
