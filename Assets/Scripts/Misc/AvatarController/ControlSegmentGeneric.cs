using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlSegmentGeneric : MonoBehaviour
{
    public float angle { get; protected set;}
    int node;

    protected Vector3 mouseDistance;
    protected Vector3 lastPosition;

    protected GameObject arrowPrefab;
    protected GameObject arrow;
    protected abstract Vector3 arrowOrientation { get; }

    protected GameObject circlePrefab;
    protected GameObject circle;

    // Abstract getter
    public abstract string dofName { get; }
    public abstract int avatarIndex { get; }
    public abstract int qIndex { get; }
    protected abstract DrawingCallback drawingCallback { get; }
    public abstract int direction { get; }

    // Cache variables
    bool isInitialized = false;
    protected GameManager gameManager;
    protected DrawManager drawManager;

    // Delegate
    protected delegate void DrawingCallback(float value);
    public delegate int GetNodeCallback(int index);

    public virtual void Init(GetNodeCallback getNodeCallback)
    {
        node = getNodeCallback(avatarIndex);

        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();

        angle = (float)drawManager.qf[qIndex];

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

        Destroy(circle);
        Destroy(arrow);
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

        angle = direction * _value / 30; // Don't know what this 30 is for
        MainParameters.Instance.joints.nodes[avatarIndex].Q[node] = angle;
        gameManager.InterpolationDDL();
        gameManager.DisplayDDL(avatarIndex, true);

        // TODO: check why direction is needed twice...
        drawingCallback(direction * angle);
    }
}
